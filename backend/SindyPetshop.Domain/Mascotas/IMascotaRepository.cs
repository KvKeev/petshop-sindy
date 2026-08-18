using SindyPetshop.Domain.Entities;

namespace SindyPetshop.Domain.Interfaces;

public interface IMascotaRepository : IRepository<Mascota>
{
    Task<IEnumerable<Mascota>> GetByClienteIdAsync(int clienteId);
    Task<Mascota?> GetConHistorialComprasAsync(int mascotaId);
    Task<Mascota?> GetConAlimentoFavoritoAsync(int mascotaId);

    // NUEVO: listado admin paginado, con el Cliente dueño incluido, filtro opcional por nombre de mascota
    Task<(IEnumerable<Mascota> Items, int Total)> GetPaginadoConClienteAsync(
        int pagina, int tamanioPagina, string? nombre);
}