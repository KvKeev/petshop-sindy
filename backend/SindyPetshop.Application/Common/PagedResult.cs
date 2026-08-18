namespace SindyPetshop.Application.DTOs;

public record PagedResult<T>(
    IEnumerable<T> Items,
    int Total,
    int Pagina,
    int TamanioPagina
);