using Microsoft.EntityFrameworkCore;
using SindyPetshop.Application.DTOs;
using SindyPetshop.Domain.Entities;
using SindyPetshop.Domain.Interfaces;
using SindyPetshop.Infrastructure.Data;

namespace SindyPetshop.Application.Services;

public class AdminProductoService
{
    private readonly IProductoRepository _productoRepository;
    private readonly SindyPetshopDbContext _context; // solo para validar CategoriaId — mismo criterio
                                                       // ya usado en CategoriasController (lectura trivial)

    public AdminProductoService(IProductoRepository productoRepository, SindyPetshopDbContext context)
    {
        _productoRepository = productoRepository;
        _context = context;
    }

    public async Task<PagedResult<AdminProductoDto>> GetListadoAsync(int pagina, int tamanioPagina, int? categoriaId)
    {
        var (items, total) = await _productoRepository.GetPaginadoAdminAsync(pagina, tamanioPagina, categoriaId);
        return new PagedResult<AdminProductoDto>(items.Select(MapearDto), total, pagina, tamanioPagina);
    }

    public async Task<AdminProductoDto?> GetDetalleAsync(int id)
    {
        var producto = await _productoRepository.GetConVariantesAsync(id);
        return producto is null ? null : MapearDto(producto);
    }

    public async Task<(bool Exito, string? Error, AdminProductoDto? Dto)> CrearAsync(CrearProductoDto dto)
    {
        var categoriaExiste = await _context.Categorias.AnyAsync(c => c.Id == dto.CategoriaId);
        if (!categoriaExiste)
            return (false, "La categoría indicada no existe", null);

        if (dto.PrimeraVariante.Precio <= 0)
            return (false, "El precio debe ser mayor a cero", null);

        if (dto.PrimeraVariante.StockFisico < 0 || dto.PrimeraVariante.StockMinimoWeb < 0)
            return (false, "El stock no puede ser negativo", null);

        var producto = new Producto
        {
            Nombre = dto.Nombre,
            Descripcion = dto.Descripcion,
            ImagenUrl = dto.ImagenUrl,
            CategoriaId = dto.CategoriaId,
            Activo = true,
        };

        producto.Variantes.Add(new VarianteProducto
        {
            Atributo = dto.PrimeraVariante.Atributo,
            Valor = dto.PrimeraVariante.Valor,
            Precio = dto.PrimeraVariante.Precio,
            StockFisico = dto.PrimeraVariante.StockFisico,
            StockMinimoWeb = dto.PrimeraVariante.StockMinimoWeb,
        });

        await _productoRepository.AddAsync(producto);
        await _productoRepository.SaveChangesAsync();

        var creado = await _productoRepository.GetConVariantesAsync(producto.Id);
        return (true, null, MapearDto(creado!));
    }

    public async Task<(ResultadoConsulta Resultado, AdminProductoDto? Dto)> ActualizarAsync(
        int id, ActualizarProductoDto dto)
    {
        var producto = await _productoRepository.GetByIdAsync(id);
        if (producto is null) return (ResultadoConsulta.NoEncontrada, null);

        var categoriaExiste = await _context.Categorias.AnyAsync(c => c.Id == dto.CategoriaId);
        if (!categoriaExiste) return (ResultadoConsulta.NoEncontrada, null);

        producto.Nombre = dto.Nombre;
        producto.Descripcion = dto.Descripcion;
        producto.ImagenUrl = dto.ImagenUrl;
        producto.CategoriaId = dto.CategoriaId;

        _productoRepository.Update(producto);
        await _productoRepository.SaveChangesAsync();

        var actualizado = await _productoRepository.GetConVariantesAsync(id);
        return (ResultadoConsulta.Ok, MapearDto(actualizado!));
    }

    public async Task<(ResultadoConsulta Resultado, AdminProductoDto? Dto)> CambiarEstadoAsync(int id, bool activo)
    {
        var producto = await _productoRepository.GetByIdAsync(id);
        if (producto is null) return (ResultadoConsulta.NoEncontrada, null);

        producto.Activo = activo;
        _productoRepository.Update(producto);
        await _productoRepository.SaveChangesAsync();

        var actualizado = await _productoRepository.GetConVariantesAsync(id);
        return (ResultadoConsulta.Ok, MapearDto(actualizado!));
    }

    public async Task<(ResultadoConsulta Resultado, string? Error, AdminProductoDto? Dto)> AgregarVarianteAsync(
        int productoId, CrearVarianteDto dto)
    {
        var producto = await _productoRepository.GetConVariantesAsync(productoId);
        if (producto is null) return (ResultadoConsulta.NoEncontrada, null, null);

        if (dto.Precio <= 0)
            return (ResultadoConsulta.Ok, "El precio debe ser mayor a cero", null); // ver nota abajo

        producto.Variantes.Add(new VarianteProducto
        {
            ProductoId = productoId,
            Atributo = dto.Atributo,
            Valor = dto.Valor,
            Precio = dto.Precio,
            StockFisico = dto.StockFisico,
            StockMinimoWeb = dto.StockMinimoWeb,
        });

        await _productoRepository.SaveChangesAsync();

        var actualizado = await _productoRepository.GetConVariantesAsync(productoId);
        return (ResultadoConsulta.Ok, null, MapearDto(actualizado!));
    }

    public async Task<(bool Exito, string? Error, AdminVarianteDto? Dto)> AjustarStockAsync(
        int varianteId, AjustarStockDto dto)
    {
        var variante = await _context.VariantesProducto.FirstOrDefaultAsync(v => v.Id == varianteId);
        if (variante is null) return (false, "La variante no existe", null);

        var nuevoStock = variante.StockFisico + dto.Cantidad;
        if (nuevoStock < 0)
            return (false, $"El ajuste dejaría el stock en negativo (actual: {variante.StockFisico})", null);

        variante.StockFisico = nuevoStock;

        _context.HistorialStock.Add(new HistorialStock
        {
            VarianteId = varianteId,
            TipoMovimiento = TipoMovimientoStock.AjusteManual,
            Cantidad = dto.Cantidad, // signado: positivo entrada, negativo salida
            Detalle = dto.Detalle,
        });

        await _context.SaveChangesAsync();

        var actualizado = new AdminVarianteDto(
            variante.Id, variante.Atributo, variante.Valor, variante.Precio,
            variante.StockFisico, variante.StockMinimoWeb, variante.StockDisponibleWeb
        );

        return (true, null, actualizado);
    }

    private static AdminProductoDto MapearDto(Producto p) => new(
        p.Id, p.Nombre, p.Descripcion, p.ImagenUrl, p.Activo,
        new CategoriaDto(p.Categoria!.Id, p.Categoria.Nombre),
        p.Variantes.Select(v => new AdminVarianteDto(
            v.Id, v.Atributo, v.Valor, v.Precio, v.StockFisico, v.StockMinimoWeb, v.StockDisponibleWeb
        ))
    );
}