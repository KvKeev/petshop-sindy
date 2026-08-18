namespace SindyPetshop.Application.DTOs;

public record VentaPorDiaDto(DateOnly Fecha, decimal Total, int CantidadPedidos, int Unidades);

public record VentaPorMetodoPagoDto(string MetodoPago, string? SubMetodoPagoEntrega, int CantidadPedidos, decimal Total);

public record ResumenVentasDto(
    DateTime Desde,
    DateTime Hasta,
    decimal TotalVentas,
    int CantidadPedidos,
    int TotalUnidades,
    decimal TicketPromedio,
    IEnumerable<VentaPorMetodoPagoDto> PorMetodoPago,
    IEnumerable<VentaPorDiaDto> PorDia
);

public record ProductoMasVendidoDto(
    int ProductoId,
    string ProductoNombre,
    string VarianteDescripcion,
    int UnidadesVendidas,
    decimal MontoTotal
);