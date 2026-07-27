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
}