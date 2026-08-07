using SindyPetshop.Application.DTOs;
using SindyPetshop.Domain.Entities;
using SindyPetshop.Domain.Interfaces;
using SindyPetshop.Infrastructure.Data;

namespace SindyPetshop.Application.Services;

public class AdminPedidoService
{
    private readonly IPedidoRepository _pedidoRepository;
    private readonly SindyPetshopDbContext _context; // solo para devolver stock al cancelar

    // Estados en los que YA se descontó StockFisico (no reserva) — si se cancela desde acá, hay que devolver
    private static readonly HashSet<EstadoPedido> EstadosConStockDescontado = new()
    {
        EstadoPedido.Confirmado, EstadoPedido.Pagado, EstadoPedido.ListoParaRetirar, EstadoPedido.Enviado
    };

    public AdminPedidoService(IPedidoRepository pedidoRepository, SindyPetshopDbContext context)
    {
        _pedidoRepository = pedidoRepository;
        _context = context;
    }

    public async Task<PagedResult<PedidoAdminDto>> GetListadoAsync(FiltrosPedidoAdmin filtros)
    {
        var (items, total) = await _pedidoRepository.GetListadoAdminAsync(
            filtros.Pagina, filtros.TamanioPagina, filtros.Estado, filtros.Desde, filtros.Hasta,
            filtros.ClienteId, filtros.MetodoPago, filtros.MetodoEntrega);

        return new PagedResult<PedidoAdminDto>(items.Select(MapearDto), total, filtros.Pagina, filtros.TamanioPagina);
    }

    public async Task<(bool Exito, string? Error, PedidoAdminDto? Dto)> CambiarEstadoAsync(
        int pedidoId, string nuevoEstadoStr)
    {
        if (!Enum.TryParse<EstadoPedido>(nuevoEstadoStr, ignoreCase: true, out var nuevoEstado))
            return (false, "Estado inválido", null);

        var pedido = await _pedidoRepository.GetConDetallesAsync(pedidoId);
        if (pedido is null)
            return (false, "Pedido no encontrado", null);

        if (!EsTransicionValida(pedido, nuevoEstado))
            return (false, $"No se puede pasar de {pedido.Estado} a {nuevoEstado} para este pedido", null);

        // Cancelación con devolución automática de stock, si ya se había descontado
        if (nuevoEstado == EstadoPedido.Cancelado && EstadosConStockDescontado.Contains(pedido.Estado))
        {
            foreach (var detalle in pedido.Detalles)
            {
                var variante = await _context.VariantesProducto.FindAsync(detalle.VarianteId);
                if (variante is null) continue; // no debería pasar, pero no rompemos la cancelación por esto

                variante.StockFisico += detalle.Cantidad;

                _context.HistorialStock.Add(new HistorialStock
                {
                    VarianteId = detalle.VarianteId,
                    TipoMovimiento = TipoMovimientoStock.DevolucionCancelacion,
                    Cantidad = detalle.Cantidad,
                    Detalle = $"Devolución por cancelación del pedido #{pedido.Id}",
                });
            }
        }

        pedido.Estado = nuevoEstado;
        _pedidoRepository.Update(pedido);
        await _pedidoRepository.SaveChangesAsync();

        var actualizado = await _pedidoRepository.GetConDetallesAsync(pedidoId);
        return (true, null, MapearDto(actualizado!));
    }

    // La máquina de estados: qué transición es válida según el pedido concreto
    private static bool EsTransicionValida(Pedido pedido, EstadoPedido nuevoEstado)
    {
        var actual = pedido.Estado;

        // Estados terminales: no se sale de ellos
        if (actual == EstadoPedido.Cancelado || actual == EstadoPedido.Completado)
            return false;

        // Cancelar: permitido desde cualquier estado no terminal, sin importar en qué paso esté
        if (nuevoEstado == EstadoPedido.Cancelado)
            return true;

        return (actual, nuevoEstado, pedido.MetodoPago, pedido.MetodoEntrega) switch
        {
            (EstadoPedido.PendientePago, EstadoPedido.Pagado, MetodoPago.Online, _) => true,

            (EstadoPedido.Confirmado, EstadoPedido.ListoParaRetirar, MetodoPago.PagoEnEntrega, MetodoEntrega.Retiro) => true,
            (EstadoPedido.Confirmado, EstadoPedido.Enviado, MetodoPago.PagoEnEntrega, MetodoEntrega.Envio) => true,

            (EstadoPedido.Pagado, EstadoPedido.ListoParaRetirar, MetodoPago.Online, MetodoEntrega.Retiro) => true,
            (EstadoPedido.Pagado, EstadoPedido.Enviado, MetodoPago.Online, MetodoEntrega.Envio) => true,

            (EstadoPedido.ListoParaRetirar, EstadoPedido.Completado, _, MetodoEntrega.Retiro) => true,
            (EstadoPedido.Enviado, EstadoPedido.Completado, _, MetodoEntrega.Envio) => true,

            _ => false,
        };
    }

    private static PedidoAdminDto MapearDto(Pedido p) => new(
        p.Id,
        p.Cliente?.Nombre ?? string.Empty,
        p.Cliente?.Email ?? string.Empty,
        p.Fecha,
        p.Estado.ToString(),
        p.MetodoEntrega.ToString(),
        p.MetodoPago.ToString(),
        p.Origen.ToString(),
        p.Total,
        p.ExpiraReservaEn,
        p.Detalles.Select(d => new DetallePedidoDto(
            d.Variante?.Producto?.Nombre ?? string.Empty,
            $"{d.Variante?.Atributo}: {d.Variante?.Valor}",
            d.Cantidad,
            d.PrecioUnitario
        ))
    );
}