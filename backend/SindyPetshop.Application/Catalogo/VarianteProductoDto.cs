namespace SindyPetshop.Application.DTOs;

// Nota: exponemos StockDisponibleWeb, NO StockFisico (ese es información interna del local)
public record VarianteProductoDto(
    int Id,
    string Atributo,
    string Valor,
    decimal Precio,
    int StockDisponibleWeb
);