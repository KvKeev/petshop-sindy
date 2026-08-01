namespace SindyPetshop.Application.DTOs;

// Lo que manda el frontend al armar el pedido desde el carrito
public record ItemPedidoDto(int VarianteId, int Cantidad, int? MascotaId);

public record CrearPedidoDto(
    IEnumerable<ItemPedidoDto> Items,
    string MetodoEntrega,   // "Retiro" o "Envio"
    string MetodoPago,      // "Online" o "PagoEnEntrega"
    int? DireccionId        // requerido solo si MetodoEntrega == Envio
);

// Lo que se devuelve
public record DetallePedidoDto(
    string ProductoNombre,
    string VarianteDescripcion, // ej: "Peso: 15kg"
    int Cantidad,
    decimal PrecioUnitario
);

public record PedidoDto(
    int Id,
    DateTime Fecha,
    string Estado,
    string MetodoEntrega,
    string MetodoPago,
    string Origen,
    decimal Total,
    DateTime? ExpiraReservaEn,
    IEnumerable<DetallePedidoDto> Detalles
);