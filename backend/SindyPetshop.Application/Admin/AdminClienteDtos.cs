namespace SindyPetshop.Application.DTOs;

// Fila del listado paginado de clientes
public record AdminClienteListDto(
    int Id,
    string Nombre,
    string Email,
    string Rol,
    DateTime FechaRegistro,
    int CantidadMascotas,
    int CantidadPedidos
);

// Ficha completa de un cliente
public record AdminClienteDetalleDto(
    int Id,
    string Nombre,
    string Email,
    string Rol,
    DateTime FechaRegistro,
    List<AdminDireccionResumenDto> Direcciones,
    List<AdminMascotaResumenDto> Mascotas,
    List<AdminPedidoResumenDto> Pedidos
);

public record AdminDireccionResumenDto(
    int Id,
    string Calle,
    string Ciudad
);

public record AdminMascotaResumenDto(
    int Id,
    string Nombre,
    string Tipo,
    string? AlimentoFavoritoNombre,
    string? AlimentoFavoritoDescripcion
);

public record AdminPedidoResumenDto(
    int Id,
    DateTime Fecha,
    string Estado,
    string MetodoPago,
    string MetodoEntrega,
    decimal Total
);

// Fila del listado paginado de mascotas (con su dueño)
public record AdminMascotaListDto(
    int Id,
    string Nombre,
    string Tipo,
    int ClienteId,
    string ClienteNombre,
    string ClienteEmail
);