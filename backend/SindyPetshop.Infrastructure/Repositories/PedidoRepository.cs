using Microsoft.EntityFrameworkCore;
using SindyPetshop.Domain.Entities;
using SindyPetshop.Domain.Interfaces;
using SindyPetshop.Infrastructure.Data;

namespace SindyPetshop.Infrastructure.Repositories;

public class PedidoRepository : RepositoryBase<Pedido>, IPedidoRepository
{
    public PedidoRepository(SindyPetshopDbContext context) : base(context) { }

    public async Task<Pedido?> GetConDetallesAsync(int pedidoId)
    {
        return await _dbSet
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
            .Where(p => p.Estado == EstadoPedido.PendientePago
                     && p.ExpiraReservaEn != null
                     && p.ExpiraReservaEn < ahora)
            .ToListAsync();
    }

    public async Task<int> GetCantidadReservadaAsync(int varianteId)
    {
        var ahora = DateTime.UtcNow;

        return await _context.DetallesPedido
            .Where(d => d.VarianteId == varianteId
                     && d.Pedido!.Estado == EstadoPedido.PendientePago
                     && d.Pedido.ExpiraReservaEn != null
                     && d.Pedido.ExpiraReservaEn > ahora)
            .SumAsync(d => (int?)d.Cantidad) ?? 0;
    }

    public void RegistrarMovimientoStock(HistorialStock movimiento)
    {
        _context.HistorialStock.Add(movimiento);
    }
}