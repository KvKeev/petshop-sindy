using Microsoft.Extensions.Logging;
using SindyPetshop.Application.DTOs;
using SindyPetshop.Application.Validaciones;
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
    private readonly IEmailService _emailService;
    private readonly ILogger<PedidoService> _logger;

    private const int MinutosExpiracionReserva = 15;

    public PedidoService(
        IPedidoRepository pedidoRepository,
        IProductoRepository productoRepository,
        IClienteRepository clienteRepository,
        IMercadoPagoService mercadoPagoService,
        ICostoEnvioService costoEnvioService,
        IEmailService emailService,
        ILogger<PedidoService> logger)
    {
        _pedidoRepository = pedidoRepository;
        _productoRepository = productoRepository;
        _clienteRepository = clienteRepository;
        _mercadoPagoService = mercadoPagoService;
        _costoEnvioService = costoEnvioService;
        _emailService = emailService;
        _logger = logger;
    }

    // clienteIdAutenticado viene null cuando el request no trae JWT (compra como invitado)
    public async Task<(ResultadoCrearPedido Resultado, string? Detalle, PedidoDto? Pedido)> CrearAsync(
        int? clienteIdAutenticado, CrearPedidoDto dto)
    {
        if (dto.Items is null || !dto.Items.Any())
            return (ResultadoCrearPedido.CarritoVacio, null, null);

        if (!Enum.TryParse<MetodoEntrega>(dto.MetodoEntrega, ignoreCase: true, out var metodoEntrega))
            return (ResultadoCrearPedido.MetodoInvalido, "MetodoEntrega inválido. Valores: Retiro, Envio", null);

        if (!Enum.TryParse<MetodoPago>(dto.MetodoPago, ignoreCase: true, out var metodoPago))
            return (ResultadoCrearPedido.MetodoInvalido, "MetodoPago inválido. Valores: Online, PagoEnEntrega", null);

        SubMetodoPagoEntrega? subMetodo = null;
        if (metodoPago == MetodoPago.PagoEnEntrega)
        {
            if (string.IsNullOrWhiteSpace(dto.SubMetodoPagoEntrega))
                return (ResultadoCrearPedido.SubMetodoPagoRequerido,
                    "Falta indicar el submétodo de pago en entrega (Efectivo, CuentaDNI_QR o Transferencia)", null);

            if (!Enum.TryParse<SubMetodoPagoEntrega>(dto.SubMetodoPagoEntrega, ignoreCase: true, out var subMetodoParseado))
                return (ResultadoCrearPedido.SubMetodoPagoInvalido,
                    "SubMetodoPagoEntrega inválido. Valores: Efectivo, CuentaDNI_QR, Transferencia", null);

            subMetodo = subMetodoParseado;
        }

        // --- Resolución del cliente: logueado (JWT) o invitado (por email) ---
        Cliente? cliente;
        if (clienteIdAutenticado.HasValue)
        {
            cliente = await _clienteRepository.GetByIdAsync(clienteIdAutenticado.Value);
            if (cliente is null)
                return (ResultadoCrearPedido.ClienteInvalido, "El cliente indicado no existe", null);
        }
        else
        {
            if (string.IsNullOrWhiteSpace(dto.NombreInvitado)
                || string.IsNullOrWhiteSpace(dto.EmailInvitado)
                || string.IsNullOrWhiteSpace(dto.TelefonoInvitado))
                return (ResultadoCrearPedido.DatosInvitadoIncompletos,
                    "Para comprar sin cuenta hacen falta nombre, email y teléfono", null);

            if (!NombreValidator.EsValido(dto.NombreInvitado))
                return (ResultadoCrearPedido.DatosInvitadoIncompletos,
                    "El nombre solo puede contener letras, números y espacios", null);

            if (!EmailValidator.EsValido(dto.EmailInvitado))
                return (ResultadoCrearPedido.DatosInvitadoIncompletos, "El email indicado no es válido", null);

            var emailNormalizado = dto.EmailInvitado.Trim();
            cliente = await _clienteRepository.GetByEmailAsync(emailNormalizado);

            if (cliente is null)
            {
                // Primera compra con este email: se crea la cuenta con lo que ofreció acá,
                // sin contraseña - queda lista para activarse más adelante (Parte 2, magic link).
                cliente = new Cliente
                {
                    Nombre = dto.NombreInvitado.Trim(),
                    Email = emailNormalizado,
                    Telefono = dto.TelefonoInvitado.Trim(),
                    PasswordHash = null,
                    Rol = RolUsuario.Cliente,
                };
                await _clienteRepository.AddAsync(cliente);
                await _clienteRepository.SaveChangesAsync();
            }
            else if (string.IsNullOrWhiteSpace(cliente.Telefono))
            {
                // El mail ya es de un cliente existente - solo completamos el teléfono si
                // no tenía uno cargado, nunca pisamos un dato que el dueño real ya haya puesto.
                cliente.Telefono = dto.TelefonoInvitado.Trim();
                _clienteRepository.Update(cliente);
            }
        }

        Direccion? direccionNueva = null;
        if (metodoEntrega == MetodoEntrega.Envio)
        {
            if (dto.Direccion is null
                || string.IsNullOrWhiteSpace(dto.Direccion.Calle)
                || string.IsNullOrWhiteSpace(dto.Direccion.Ciudad))
                return (ResultadoCrearPedido.DireccionRequerida, "Falta indicar calle y ciudad de entrega", null);

            // Siempre se crea una dirección nueva - un mismo cliente puede pedir a
            // distintas direcciones en cada compra (regalo, casa de un familiar, etc.).
            direccionNueva = new Direccion
            {
                ClienteId = cliente.Id,
                Calle = dto.Direccion.Calle.Trim(),
                Ciudad = dto.Direccion.Ciudad.Trim(),
                PisoDepto = string.IsNullOrWhiteSpace(dto.Direccion.PisoDepto) ? null : dto.Direccion.PisoDepto.Trim(),
            };
        }

        var pedido = new Pedido
        {
            ClienteId = cliente.Id,
            Direccion = direccionNueva,
            MetodoEntrega = metodoEntrega,
            MetodoPago = metodoPago,
            SubMetodoPagoEntrega = subMetodo,
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

        var costoEnvio = _costoEnvioService.Calcular(metodoEntrega, direccionNueva);
        pedido.CostoEnvio = costoEnvio;
        pedido.Total = total + costoEnvio;

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

        if (metodoPago == MetodoPago.Online)
        {
            var preferencia = await _mercadoPagoService.CrearPreferenciaAsync(
                pedido.Id, cliente.Email, itemsParaPreferencia);

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

        // El mail de confirmación se manda siempre, logueado o invitado. Si falla, no
        // afecta la compra - ResendEmailService se traga cualquier error internamente.
        await _emailService.EnviarConfirmacionPedidoAsync(
            cliente.Email, cliente.Nombre, pedido.Id, pedido.Total, pedido.TrackingToken, pedido.MercadoPagoInitPoint);

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

    // Consulta pública sin login, vía el token no adivinable del pedido.
    public async Task<PedidoDto?> GetPorTrackingTokenAsync(Guid trackingToken)
    {
        var pedido = await _pedidoRepository.GetByTrackingTokenAsync(trackingToken);
        return pedido is null ? null : MapearDto(pedido);
    }

    private static PedidoDto MapearDto(Pedido pedido)
    {
        var detalles = pedido.Detalles.Select(d => new DetallePedidoDto(
            d.Variante?.Producto?.Nombre ?? string.Empty,
            $"{d.Variante?.Atributo}: {d.Variante?.Valor}",
            d.Cantidad,
            d.PrecioUnitario
        ));

        var direccion = pedido.Direccion is null
            ? null
            : new DireccionPedidoDto(pedido.Direccion.Calle, pedido.Direccion.Ciudad, pedido.Direccion.PisoDepto);

        return new PedidoDto(
            pedido.Id,
            pedido.Fecha,
            pedido.Estado.ToString(),
            pedido.MetodoEntrega.ToString(),
            pedido.MetodoPago.ToString(),
            pedido.SubMetodoPagoEntrega?.ToString(),
            pedido.Origen.ToString(),
            pedido.CostoEnvio,
            pedido.Total,
            pedido.ExpiraReservaEn,
            detalles,
            pedido.MercadoPagoInitPoint,
            pedido.TrackingToken,
            direccion
        );
    }
}