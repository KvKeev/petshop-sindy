using SindyPetshop.Domain.Entities;

namespace SindyPetshop.Domain.Interfaces;

public interface IClienteRepository : IRepository<Cliente>
{
    Task<Cliente?> GetByEmailAsync(string email);
    Task<Cliente?> GetConDireccionesAsync(int clienteId);

    //listado admin paginado con filtros opcionales por nombre y email
    Task<(IEnumerable<Cliente> Items, int Total)> GetPaginadoAsync(
        int pagina, int tamanioPagina, string? nombre, string? email);

    //detalle completo para la ficha de cliente en el panel admin
    Task<Cliente?> GetConDetalleCompletoAsync(int clienteId);
    Task<Cliente?> GetByActivacionTokenAsync(Guid token);
}