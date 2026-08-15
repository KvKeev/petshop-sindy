using SindyPetshop.Domain.Entities;

namespace SindyPetshop.Domain.Interfaces;

public interface IPedidoRepository : IRepository<Pedido>
{
    Task<Pedido?> GetConDetallesAsync(int pedidoId);
    Task<IEnumerable<Pedido>> GetByClienteIdAsync(int clienteId);
    Task<IEnumerable<Pedido>> GetPendientesVencidosAsync(DateTime ahora);
    Task<Pedido?> GetByTrackingTokenAsync(Guid trackingToken);

    // Suma las cantidades reservadas (pedidos PendientePago no vencidos) para una variante
    Task<int> GetCantidadReservadaAsync(int varianteId);

    // Registra un movimiento de stock sin hacer SaveChanges (se guarda junto con el resto del pedido)
    void RegistrarMovimientoStock(HistorialStock movimiento);
    // Listado para el panel admin, con filtros opcionales
    Task<(IEnumerable<Pedido> Items, int Total)> GetListadoAdminAsync(
        int pagina, int tamanioPagina, EstadoPedido? estado, DateTime? desde, DateTime? hasta,
        int? clienteId, MetodoPago? metodoPago, MetodoEntrega? metodoEntrega);
}