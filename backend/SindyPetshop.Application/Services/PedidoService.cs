using Microsoft.Extensions.Logging;
using SindyPetshop.Application.DTOs;
using SindyPetshop.Domain.Entities;
using SindyPetshop.Domain.Interfaces;

namespace SindyPetshop.Application.Services;

public class PedidoService
{
    private readonly IPedidoRepository _pedidoRepository;
    private readonly IProductoRepository _productoRepository;
    private readonly IClienteRepository _clienteRepository;
    private readonly IMercadoPagoService _mercadoPagoService;
    private readonly ICostoEnvioService _costoEnvioService;
    private readonly ILogger<PedidoService> _logger;

    private const int MinutosExpiracionReserva = 15;

    public PedidoService(
        IPedidoRepository pedidoRepository,
        IProductoRepository productoRepository,
        IClienteRepository clienteRepository,
        IMercadoPagoService mercadoPagoService,
        ICostoEnvioService costoEnvioService,
        ILogger<PedidoService> logger)
    {
        _pedidoRepository = pedidoRepository;
        _productoRepository = productoRepository;
        _clienteRepository = clienteRepository;
        _mercadoPagoService = mercadoPagoService;
        _costoEnvioService = costoEnvioService;
        _logger = logger;
    }

    public async Task<(ResultadoCrearPedido Resultado, string? Detalle, PedidoDto? Pedido)> CrearAsync(
        int clienteId, CrearPedidoDto dto)
    {
        if (dto.Items is null || !dto.Items.Any())
            return (ResultadoCrearPedido.CarritoVacio, null, null);

        if (!Enum.TryParse<MetodoEntrega>(dto.MetodoEntrega, ignoreCase: true, out var metodoEntrega))
            return (ResultadoCrearPedido.MetodoInvalido, "MetodoEntrega inválido. Valores: Retiro, Envio", null);

        if (!Enum.TryParse<MetodoPago>(dto.MetodoPago, ignoreCase: true, out var metodoPago))
            return (ResultadoCrearPedido.MetodoInvalido, "MetodoPago inválido. Valores: Online, PagoEnEntrega", null);

        int? direccionId = null;
        Cliente? cliente = null;
        Direccion? direccionSeleccionada = null;
        if (metodoEntrega == MetodoEntrega.Envio)
        {
            cliente = await _clienteRepository.GetConDireccionesAsync(clienteId);
            var direccionValida = cliente?.Direcciones.Any(d => d.Id == dto.DireccionId) ?? false;

            if (dto.DireccionId is null)
                return (ResultadoCrearPedido.DireccionRequerida, null, null);

            if (!direccionValida)
                return (ResultadoCrearPedido.DireccionInvalida, null, null);

            direccionId = dto.DireccionId;
            direccionSeleccionada = cliente!.Direcciones.First(d => d.Id == dto.DireccionId);
        }

        var pedido = new Pedido
        {
            ClienteId = clienteId,
            DireccionId = direccionId,
            MetodoEntrega = metodoEntrega,
            MetodoPago = metodoPago,
            Origen = OrigenPedido.Web,
        };

        decimal total = 0;
        var itemsParaPreferencia = new List<(string Titulo, int Cantidad, decimal PrecioUnitario)>();

        foreach (var item in dto.Items)
        {
            var variante = await _productoRepository.GetVarianteConProductoAsync(item.VarianteId);
            if (variante is null)
                return (ResultadoCrearPedido.VarianteInvalida, $"La variante {item.VarianteId} no existe", null);

            var reservado = await _pedidoRepository.GetCantidadReservadaAsync(item.VarianteId);
            var disponible = Math.Max(0, variante.StockFisico - variante.StockMinimoWeb - reservado);

            if (item.Cantidad > disponible)
                return (ResultadoCrearPedido.StockInsuficiente,
                    $"Stock insuficiente para {variante.Producto?.Nombre} ({variante.Atributo}: {variante.Valor}). Disponible: {disponible}",
                    null);

            pedido.Detalles.Add(new DetallePedido
            {
                VarianteId = variante.Id,
                MascotaId = item.MascotaId,
                Cantidad = item.Cantidad,
                PrecioUnitario = variante.Precio,
            });

            total += variante.Precio * item.Cantidad;
            itemsParaPreferencia.Add(($"{variante.Producto?.Nombre} ({variante.Atributo}: {variante.Valor})", item.Cantidad, variante.Precio));

            if (metodoPago == MetodoPago.PagoEnEntrega)
            {
                variante.StockFisico -= item.Cantidad;

                _pedidoRepository.RegistrarMovimientoStock(new HistorialStock
                {
                    VarianteId = variante.Id,
                    TipoMovimiento = TipoMovimientoStock.Venta,
                    Cantidad = item.Cantidad,
                });
            }
        }

        var costoEnvio = _costoEnvioService.Calcular(metodoEntrega, direccionSeleccionada);
        pedido.CostoEnvio = costoEnvio;
        pedido.Total = total + costoEnvio;

        // Si hay costo de envío, se agrega como ítem aparte de la preferencia de MercadoPago -
        // si no, el cliente pagaría online solo el subtotal de productos y el envío quedaría sin cobrar.
        if (costoEnvio > 0)
            itemsParaPreferencia.Add(("Costo de envío", 1, costoEnvio));

        if (metodoPago == MetodoPago.Online)
        {
            pedido.Estado = EstadoPedido.PendientePago;
            pedido.ExpiraReservaEn = DateTime.UtcNow.AddMinutes(MinutosExpiracionReserva);
        }
        else
        {
            pedido.Estado = EstadoPedido.Confirmado;
        }

        await _pedidoRepository.AddAsync(pedido);
        await _pedidoRepository.SaveChangesAsync();

        // Recién con el pedido ya guardado (tiene Id real) se genera la preferencia de pago.
        // Si MercadoPago falla acá, el pedido igual queda creado (con stock reservado) -
        // simplemente no va a tener MercadoPagoInitPoint, y se puede reintentar más adelante si hace falta.
        if (metodoPago == MetodoPago.Online)
        {
            cliente ??= await _clienteRepository.GetByIdAsync(clienteId);
            var preferencia = await _mercadoPagoService.CrearPreferenciaAsync(
                pedido.Id, cliente?.Email ?? "", itemsParaPreferencia);

            if (preferencia is not null)
            {
                pedido.MercadoPagoPreferenceId = preferencia.Value.PreferenceId;
                pedido.MercadoPagoInitPoint = preferencia.Value.InitPoint;
                _pedidoRepository.Update(pedido);
                await _pedidoRepository.SaveChangesAsync();
            }
            else
            {
                _logger.LogWarning("No se pudo generar la preferencia de MercadoPago para el pedido {PedidoId}", pedido.Id);
            }
        }

        var pedidoConDetalles = await _pedidoRepository.GetConDetallesAsync(pedido.Id);
        return (ResultadoCrearPedido.Ok, null, MapearDto(pedidoConDetalles!));
    }

    public async Task<IEnumerable<PedidoDto>> GetMisPedidosAsync(int clienteId)
    {
        var pedidos = await _pedidoRepository.GetByClienteIdAsync(clienteId);
        return pedidos.Select(p => MapearDto(p));
    }

    public async Task<(ResultadoConsulta Resultado, PedidoDto? Dto)> GetDetalleAsync(
        int pedidoId, int clienteIdSolicitante, bool esAdmin)
    {
        var pedido = await _pedidoRepository.GetConDetallesAsync(pedidoId);
        if (pedido is null)
            return (ResultadoConsulta.NoEncontrada, null);

        if (pedido.ClienteId != clienteIdSolicitante && !esAdmin)
            return (ResultadoConsulta.NoAutorizado, null);

        return (ResultadoConsulta.Ok, MapearDto(pedido));
    }

    private static PedidoDto MapearDto(Pedido pedido)
    {
        var detalles = pedido.Detalles.Select(d => new DetallePedidoDto(
            d.Variante?.Producto?.Nombre ?? string.Empty,
            $"{d.Variante?.Atributo}: {d.Variante?.Valor}",
            d.Cantidad,
            d.PrecioUnitario
        ));

        return new PedidoDto(
            pedido.Id,
            pedido.Fecha,
            pedido.Estado.ToString(),
            pedido.MetodoEntrega.ToString(),
            pedido.MetodoPago.ToString(),
            pedido.Origen.ToString(),
            pedido.CostoEnvio,
            pedido.Total,
            pedido.ExpiraReservaEn,
            detalles,
            pedido.MercadoPagoInitPoint
        );
    }
}