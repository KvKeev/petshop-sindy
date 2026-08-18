namespace SindyPetshop.Application.DTOs;

// El admin SÍ ve StockFisico — a diferencia de VarianteProductoDto (catálogo público),
// que nunca lo expone. Son DTOs separados a propósito.
public record AdminVarianteDto(
    int Id,
    string Atributo,
    string Valor,
    decimal Precio,
    int StockFisico,
    int StockMinimoWeb,
    int StockDisponibleWeb
);

public record AdminProductoDto(
    int Id,
    string Nombre,
    string? Descripcion,
    string? ImagenUrl,
    bool Activo,
    CategoriaDto Categoria,
    IEnumerable<AdminVarianteDto> Variantes
);

public record CrearVarianteDto(
    string Atributo,
    string Valor,
    decimal Precio,
    int StockFisico,
    int StockMinimoWeb
);

public record CrearProductoDto(
    string Nombre,
    string? Descripcion,
    int CategoriaId,
    string? ImagenUrl,
    CrearVarianteDto PrimeraVariante
);

public record ActualizarProductoDto(
    string Nombre,
    string? Descripcion,
    int CategoriaId,
    string? ImagenUrl
);

public record CambiarEstadoProductoDto(bool Activo);

// Cantidad puede ser positiva (entrada) o negativa (salida)
public record AjustarStockDto(int Cantidad, string? Detalle);