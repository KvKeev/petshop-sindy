using SindyPetshop.Domain.Entities;

namespace SindyPetshop.Domain.Interfaces;

public interface IMascotaRepository : IRepository<Mascota>
{
    Task<IEnumerable<Mascota>> GetByClienteIdAsync(int clienteId);
    Task<Mascota?> GetConHistorialComprasAsync(int mascotaId);
}