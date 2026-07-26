using SindyPetshop.Application.DTOs;
using SindyPetshop.Domain.Interfaces;

namespace SindyPetshop.Application.Services;

public class ProductoService
{
    private readonly IProductoRepository _productoRepository;

    public ProductoService(IProductoRepository productoRepository)
    {
        _productoRepository = productoRepository;
    }

    public async Task<PagedResult<ProductoDto>> GetCatalogoAsync(
        int pagina, int tamanioPagina, int? categoriaId)
    {
        var (items, total) = await _productoRepository.GetPaginadoAsync(pagina, tamanioPagina, categoriaId);

        var dtos = items.Select(p => new ProductoDto(
            p.Id,
            p.Nombre,
            p.ImagenUrl,
            p.Categoria?.Nombre ?? string.Empty,
            p.Variantes.Any() ? p.Variantes.Min(v => v.Precio) : 0
        ));

        return new PagedResult<ProductoDto>(dtos, total, pagina, tamanioPagina);
    }

    public async Task<ProductoDetalleDto?> GetDetalleAsync(int id)
    {
        var producto = await _productoRepository.GetConVariantesAsync(id);
        if (producto is null) return null;

        var variantesDto = producto.Variantes.Select(v => new VarianteProductoDto(
            v.Id, v.Atributo, v.Valor, v.Precio, v.StockDisponibleWeb
        ));

        return new ProductoDetalleDto(
            producto.Id,
            producto.Nombre,
            producto.Descripcion,
            producto.ImagenUrl,
            new CategoriaDto(producto.Categoria!.Id, producto.Categoria.Nombre),
            variantesDto
        );
    }
}