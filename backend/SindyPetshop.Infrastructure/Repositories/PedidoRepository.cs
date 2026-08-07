using Microsoft.EntityFrameworkCore;
using SindyPetshop.Domain.Entities;
using SindyPetshop.Domain.Interfaces;
using SindyPetshop.Infrastructure.Data;

namespace SindyPetshop.Infrastructure.Repositories;

public class PedidoRepository : RepositoryBase<Pedido>, IPedidoRepository
{
    public PedidoRepository(SindyPetshopDbContext context)
        : base(context) { }

    public async Task<Pedido?> GetConDetallesAsync(int pedidoId)
    {
        return await _dbSet
            .Include(p => p.Cliente)
            .Include(p => p.Detalles)
                .ThenInclude(d => d.Variante!)
                    .ThenInclude(v => v.Producto)
            .Include(p => p.Direccion)
            .FirstOrDefaultAsync(p => p.Id == pedidoId);
    }

    public async Task<IEnumerable<Pedido>> GetByClienteIdAsync(int clienteId)
    {
        return await _dbSet
            .Where(p => p.ClienteId == clienteId)
            .Include(p => p.Detalles)
                .ThenInclude(d => d.Variante!)
                    .ThenInclude(v => v.Producto)
            .OrderByDescending(p => p.Fecha)
            .ToListAsync();
    }

    public async Task<IEnumerable<Pedido>> GetPendientesVencidosAsync(DateTime ahora)
    {
        return await _dbSet
            .Where(p =>
                p.Estado == EstadoPedido.PendientePago
                && p.ExpiraReservaEn != null
                && p.ExpiraReservaEn < ahora
            )
            .ToListAsync();
    }

    public async Task<int> GetCantidadReservadaAsync(int varianteId)
    {
        var ahora = DateTime.UtcNow;

        return await _context
                .DetallesPedido.Where(d =>
                    d.VarianteId == varianteId
                    && d.Pedido!.Estado == EstadoPedido.PendientePago
                    && d.Pedido.ExpiraReservaEn != null
                    && d.Pedido.ExpiraReservaEn > ahora
                )
                .SumAsync(d => (int?)d.Cantidad)
            ?? 0;
    }

    public void RegistrarMovimientoStock(HistorialStock movimiento)
    {
        _context.HistorialStock.Add(movimiento);
    }

    public async Task<(IEnumerable<Pedido> Items, int Total)> GetListadoAdminAsync(
        int pagina,
        int tamanioPagina,
        EstadoPedido? estado,
        DateTime? desde,
        DateTime? hasta,
        int? clienteId,
        MetodoPago? metodoPago,
        MetodoEntrega? metodoEntrega
    )
    {
        var query = _dbSet.AsQueryable();

        if (estado.HasValue)
            query = query.Where(p => p.Estado == estado.Value);
        if (desde.HasValue)
            query = query.Where(p => p.Fecha >= desde.Value);
        if (hasta.HasValue)
            query = query.Where(p => p.Fecha <= hasta.Value);
        if (clienteId.HasValue)
            query = query.Where(p => p.ClienteId == clienteId.Value);
        if (metodoPago.HasValue)
            query = query.Where(p => p.MetodoPago == metodoPago.Value);
        if (metodoEntrega.HasValue)
            query = query.Where(p => p.MetodoEntrega == metodoEntrega.Value);

        var total = await query.CountAsync();

        var items = await query
            .Include(p => p.Cliente)
            .Include(p => p.Detalles)
                .ThenInclude(d => d.Variante!)
                    .ThenInclude(v => v.Producto)
            .OrderByDescending(p => p.Fecha)
            .Skip((pagina - 1) * tamanioPagina)
            .Take(tamanioPagina)
            .ToListAsync();

        return (items, total);
    }
}
