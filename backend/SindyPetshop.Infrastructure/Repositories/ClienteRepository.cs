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
}