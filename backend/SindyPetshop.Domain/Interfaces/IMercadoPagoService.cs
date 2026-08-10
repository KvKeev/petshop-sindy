namespace SindyPetshop.Domain.Interfaces;

public interface IMercadoPagoService
{
    // Crea la preferencia de pago para un pedido Online. Devuelve null si falla la comunicación con MercadoPago.
    Task<(string PreferenceId, string InitPoint)?> CrearPreferenciaAsync(
        int pedidoId,
        string clienteEmail,
        IEnumerable<(string Titulo, int Cantidad, decimal PrecioUnitario)> items);

    // Consulta el estado real de un pago contra la API de MercadoPago (nunca se confía en el payload del webhook a ciegas).
    Task<(string? Status, string? ExternalReference)?> ConsultarPagoAsync(long paymentId);
}