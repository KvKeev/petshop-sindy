namespace SindyPetshop.Application.DTOs;

// Para el listado del catálogo (vista resumida)
public record ProductoDto(
    int Id,
    string Nombre,
    string? ImagenUrl,
    string CategoriaNombre,
    decimal PrecioDesde
);

// Para el detalle de un producto (con todas sus variantes)
public record ProductoDetalleDto(
    int Id,
    string Nombre,
    string? Descripcion,
    string? ImagenUrl,
    CategoriaDto Categoria,
    IEnumerable<VarianteProductoDto> Variantes
);