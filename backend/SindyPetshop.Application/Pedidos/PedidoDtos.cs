namespace SindyPetshop.Application.DTOs;

// Lo que manda el frontend al armar el pedido desde el carrito
public record ItemPedidoDto(int VarianteId, int Cantidad, int? MascotaId);

// Dirección de entrega tal como la escribe quien compra, siempre nueva por pedido
// (nunca reutiliza una dirección guardada - el mismo cliente puede pedir a distintas
// direcciones en cada compra).
public record DireccionEnvioDto(string Calle, string Ciudad, string? PisoDepto);

public record CrearPedidoDto(
    IEnumerable<ItemPedidoDto> Items,
    string MetodoEntrega,        // "Retiro" o "Envio"
    string MetodoPago,           // "Online" o "PagoEnEntrega"
    string? SubMetodoPagoEntrega, // "Efectivo" | "CuentaDNI_QR" | "Transferencia" - obligatorio solo si MetodoPago == PagoEnEntrega
    DireccionEnvioDto? Direccion, // obligatorio solo si MetodoEntrega == Envio
    // Datos de invitado: se ignoran si el request viene con JWT válido.
    // Obligatorios los tres si no hay JWT.
    string? NombreInvitado,
    string? EmailInvitado,
    string? TelefonoInvitado
);

public record DetallePedidoDto(
    string ProductoNombre,
    string VarianteDescripcion, // ej: "Peso: 15kg"
    int Cantidad,
    decimal PrecioUnitario
);

public record DireccionPedidoDto(string Calle, string Ciudad, string? PisoDepto);

public record PedidoDto(
    int Id,
    DateTime Fecha,
    string Estado,
    string MetodoEntrega,
    string MetodoPago,
    string? SubMetodoPagoEntrega,
    string Origen,
    decimal CostoEnvio,
    decimal Total,
    DateTime? ExpiraReservaEn,
    IEnumerable<DetallePedidoDto> Detalles,
    string? LinkPago,
    Guid TrackingToken,
    DireccionPedidoDto? Direccion
);