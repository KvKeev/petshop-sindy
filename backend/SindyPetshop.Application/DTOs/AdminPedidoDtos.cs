using SindyPetshop.Domain.Entities;

namespace SindyPetshop.Application.DTOs;

public record PedidoAdminDto(
    int Id,
    string ClienteNombre,
    string ClienteEmail,
    DateTime Fecha,
    string Estado,
    string MetodoEntrega,
    string MetodoPago,
    string Origen,
    decimal CostoEnvio,
    decimal Total,
    DateTime? ExpiraReservaEn,
    IEnumerable<DetallePedidoDto> Detalles,
    string? LinkPago
);

public record CambiarEstadoPedidoDto(string NuevoEstado);

public record FiltrosPedidoAdmin(
    int Pagina,
    int TamanioPagina,
    EstadoPedido? Estado,
    DateTime? Desde,
    DateTime? Hasta,
    int? ClienteId,
    MetodoPago? MetodoPago,
    MetodoEntrega? MetodoEntrega
);