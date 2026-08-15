namespace SindyPetshop.Application.DTOs;

public enum ResultadoCrearPedido
{
    Ok,
    CarritoVacio,
    MetodoInvalido,
    VarianteInvalida,
    StockInsuficiente,
    DireccionRequerida,
    ClienteInvalido,
    DatosInvitadoIncompletos,
    SubMetodoPagoRequerido,
    SubMetodoPagoInvalido,
}