using SindyPetshop.Application.DTOs;
using SindyPetshop.Domain.Interfaces;

namespace SindyPetshop.Application.Services;

public class ProductoService
{
    private readonly IProductoRepository _productoRepository;
    private readonly IPedidoRepository _pedidoRepository;

    public ProductoService(IProductoRepository productoRepository, IPedidoRepository pedidoRepository)
    {
        _productoRepository = productoRepository;
        _pedidoRepository = pedidoRepository;
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

    // resta lo reservado por pedidos PendientePago no vencidos
    public async Task<ProductoDetalleDto?> GetDetalleAsync(int id)
    {
        var producto = await _productoRepository.GetConVariantesAsync(id);
        if (producto is null) return null;

        var variantesDto = new List<VarianteProductoDto>();
        foreach (var v in producto.Variantes)
        {
            var reservado = await _pedidoRepository.GetCantidadReservadaAsync(v.Id);
            var disponibleReal = Math.Max(0, v.StockFisico - v.StockMinimoWeb - reservado);
            variantesDto.Add(new VarianteProductoDto(v.Id, v.Atributo, v.Valor, v.Precio, disponibleReal));
        }

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