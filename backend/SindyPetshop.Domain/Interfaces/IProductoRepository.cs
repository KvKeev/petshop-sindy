using SindyPetshop.Domain.Entities;

namespace SindyPetshop.Domain.Interfaces;

// Extiende el genérico con consultas específicas del catálogo
public interface IProductoRepository : IRepository<Producto>
{
    // Paginado + filtro opcional por categoría, solo productos activos
    Task<(IEnumerable<Producto> Items, int Total)> GetPaginadoAsync(
        int pagina, int tamanioPagina, int? categoriaId = null);

    // Trae el producto con sus variantes cargadas (para el detalle)
    Task<Producto?> GetConVariantesAsync(int id);
}