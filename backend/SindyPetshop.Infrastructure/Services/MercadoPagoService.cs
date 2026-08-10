using MercadoPago.Client.Payment;
using MercadoPago.Client.Preference;
using MercadoPago.Config;
using SindyPetshop.Domain.Interfaces;

namespace SindyPetshop.Infrastructure.Services;

public class MercadoPagoService : IMercadoPagoService
{
    private readonly string _accessToken;
    private readonly string? _notificationUrl;

    public MercadoPagoService(string accessToken, string? notificationUrl)
    {
        _accessToken = accessToken;
        _notificationUrl = notificationUrl;
    }

    public async Task<(string PreferenceId, string InitPoint)?> CrearPreferenciaAsync(
        int pedidoId,
        string clienteEmail,
        IEnumerable<(string Titulo, int Cantidad, decimal PrecioUnitario)> items)
    {
        try
        {
            MercadoPagoConfig.AccessToken = _accessToken;

            var request = new PreferenceRequest
            {
                Items = items.Select(i => new PreferenceItemRequest
                {
                    Title = i.Titulo,
                    Quantity = i.Cantidad,
                    CurrencyId = "ARS",
                    UnitPrice = i.PrecioUnitario,
                }).ToList(),
                ExternalReference = pedidoId.ToString(),
                Payer = new PreferencePayerRequest { Email = clienteEmail },
                NotificationUrl = _notificationUrl,
            };

            var client = new PreferenceClient();
            var preference = await client.CreateAsync(request);

            return (preference.Id, preference.InitPoint);
        }
        catch
        {
            // Si MercadoPago no responde o rechaza la preferencia, no rompemos el checkout entero
            // (el pedido ya se creó con stock reservado) - se maneja en PedidoService.
            return null;
        }
    }

    public async Task<(string? Status, string? ExternalReference)?> ConsultarPagoAsync(long paymentId)
    {
        try
        {
            MercadoPagoConfig.AccessToken = _accessToken;

            var client = new PaymentClient();
            var payment = await client.GetAsync(paymentId);

            return (payment.Status, payment.ExternalReference);
        }
        catch
        {
            return null;
        }
    }
}