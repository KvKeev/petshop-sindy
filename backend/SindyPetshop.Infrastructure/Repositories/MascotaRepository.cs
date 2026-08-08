using Microsoft.EntityFrameworkCore;
using SindyPetshop.Domain.Entities;
using SindyPetshop.Domain.Interfaces;
using SindyPetshop.Infrastructure.Data;

namespace SindyPetshop.Infrastructure.Repositories;

public class MascotaRepository : RepositoryBase<Mascota>, IMascotaRepository
{
    public MascotaRepository(SindyPetshopDbContext context) : base(context) { }

    public async Task<IEnumerable<Mascota>> GetByClienteIdAsync(int clienteId)
    {
        return await _dbSet.Where(m => m.ClienteId == clienteId).ToListAsync();
    }

    public async Task<Mascota?> GetConHistorialComprasAsync(int mascotaId)
    {
        return await _dbSet
            .Include(m => m.ComprasAsociadas)
                .ThenInclude(d => d.Variante!)
                    .ThenInclude(v => v.Producto)
            .Include(m => m.ComprasAsociadas)
                .ThenInclude(d => d.Pedido)
            .FirstOrDefaultAsync(m => m.Id == mascotaId);
    }
    public async Task<Mascota?> GetConAlimentoFavoritoAsync(int mascotaId)
    {
        return await _dbSet
            .Include(m => m.AlimentoFavoritoProducto)
            .FirstOrDefaultAsync(m => m.Id == mascotaId);
    }

public async Task<(IEnumerable<Mascota> Items, int Total)> GetPaginadoConClienteAsync(
        int pagina, int tamanioPagina, string? nombre)
    {
        var query = _dbSet
            .Include(m => m.Cliente)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(nombre))
            query = query.Where(m => EF.Functions.Like(m.Nombre, $"%{nombre}%"));

        var total = await query.CountAsync();

        var items = await query
            .OrderBy(m => m.Nombre)
            .Skip((pagina - 1) * tamanioPagina)
            .Take(tamanioPagina)
            .ToListAsync();

        return (items, total);
    }
}