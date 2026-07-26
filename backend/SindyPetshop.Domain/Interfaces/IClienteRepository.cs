using SindyPetshop.Domain.Entities;

namespace SindyPetshop.Domain.Interfaces;

public interface IClienteRepository : IRepository<Cliente>
{
    Task<Cliente?> GetByEmailAsync(string email);
}