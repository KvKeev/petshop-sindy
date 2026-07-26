using Microsoft.EntityFrameworkCore;
using SindyPetshop.Domain.Entities;
using SindyPetshop.Domain.Interfaces;
using SindyPetshop.Infrastructure.Data;

namespace SindyPetshop.Infrastructure.Repositories;

public class ProductoRepository : RepositoryBase<Producto>, IProductoRepository
{
    public ProductoRepository(SindyPetshopDbContext context) : base(context) { }

    public async Task<(IEnumerable<Producto> Items, int Total)> GetPaginadoAsync(
        int pagina, int tamanioPagina, int? categoriaId = null)
    {
        var query = _dbSet.Where(p => p.Activo);

        if (categoriaId.HasValue)
            query = query.Where(p => p.CategoriaId == categoriaId.Value);

        var total = await query.CountAsync();

        var items = await query
            .Include(p => p.Categoria)
            .Include(p => p.Variantes)
            .OrderBy(p => p.Nombre)
            .Skip((pagina - 1) * tamanioPagina)
            .Take(tamanioPagina)
            .ToListAsync();

        return (items, total);
    }

    public async Task<Producto?> GetConVariantesAsync(int id)
    {
        return await _dbSet
            .Include(p => p.Categoria)
            .Include(p => p.Variantes)
            .FirstOrDefaultAsync(p => p.Id == id);
    }
}