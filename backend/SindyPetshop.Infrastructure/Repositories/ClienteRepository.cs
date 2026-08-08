using Microsoft.EntityFrameworkCore;
using SindyPetshop.Domain.Entities;
using SindyPetshop.Domain.Interfaces;
using SindyPetshop.Infrastructure.Data;

namespace SindyPetshop.Infrastructure.Repositories;

public class ClienteRepository : RepositoryBase<Cliente>, IClienteRepository
{
    public ClienteRepository(SindyPetshopDbContext context) : base(context) { }

    public async Task<Cliente?> GetByEmailAsync(string email)
    {
        return await _dbSet.FirstOrDefaultAsync(c => c.Email == email);
    }
    public async Task<Cliente?> GetConDireccionesAsync(int clienteId)
    {
        return await _dbSet
            .Include(c => c.Direcciones)
            .FirstOrDefaultAsync(c => c.Id == clienteId);
    }

public async Task<(IEnumerable<Cliente> Items, int Total)> GetPaginadoAsync(
        int pagina, int tamanioPagina, string? nombre, string? email)
    {
        var query = _dbSet
            .Include(c => c.Mascotas)
            .Include(c => c.Pedidos)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(nombre))
            query = query.Where(c => EF.Functions.Like(c.Nombre, $"%{nombre}%"));

        if (!string.IsNullOrWhiteSpace(email))
            query = query.Where(c => EF.Functions.Like(c.Email, $"%{email}%"));

        var total = await query.CountAsync();

        var items = await query
            .OrderBy(c => c.Nombre)
            .Skip((pagina - 1) * tamanioPagina)
            .Take(tamanioPagina)
            .ToListAsync();

        return (items, total);
    }

    public async Task<Cliente?> GetConDetalleCompletoAsync(int clienteId)
    {
        return await _dbSet
            .Include(c => c.Direcciones)
            .Include(c => c.Mascotas)
            .Include(c => c.Pedidos)
            .FirstOrDefaultAsync(c => c.Id == clienteId);
    }
}