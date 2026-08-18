using System.Net.Http.Headers;
using System.Net.Http.Json;
using Microsoft.Extensions.Logging;
using SindyPetshop.Domain.Interfaces;

namespace SindyPetshop.Infrastructure.Services;

public class ResendEmailService : IEmailService
{
    private readonly HttpClient _httpClient;
    private readonly string _fromEmail;
    private readonly string _fromNombre;
    private readonly string _frontendBaseUrl;
    private readonly ILogger<ResendEmailService> _logger;

    public ResendEmailService(
        HttpClient httpClient,
        string apiKey,
        string fromEmail,
        string fromNombre,
        string frontendBaseUrl,
        ILogger<ResendEmailService> logger)
    {
        _httpClient = httpClient;
        _httpClient.BaseAddress = new Uri("https://api.resend.com/");
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);
        _fromEmail = fromEmail;
        _fromNombre = fromNombre;
        _frontendBaseUrl = frontendBaseUrl.TrimEnd('/');
        _logger = logger;
    }

    public Task EnviarConfirmacionPedidoAsync(
        string destinatarioEmail,
        string destinatarioNombre,
        int pedidoId,
        decimal total,
        Guid trackingToken,
        string? linkPago)
    {
        var linkSeguimiento = $"{_frontendBaseUrl}/seguimiento/{trackingToken}";

        var botonPago = linkPago is null
            ? string.Empty
            : $@"<p><a href=""{linkPago}"">Completar el pago en MercadoPago</a></p>";

        var html = $@"
            <h2>¡Gracias por tu compra en Petshop Sindy, {destinatarioNombre}!</h2>
            <p>Registramos tu pedido #{pedidoId} por un total de ${total:N2}.</p>
            {botonPago}
            <p><a href=""{linkSeguimiento}"">Ver el estado de mi pedido</a></p>
        ";

        return EnviarAsync(destinatarioEmail, $"Pedido #{pedidoId} confirmado - Petshop Sindy", html);
    }

    public Task EnviarActivacionCuentaAsync(string destinatarioEmail, string destinatarioNombre, Guid tokenActivacion)
    {
        var linkActivacion = $"{_frontendBaseUrl}/activar-cuenta?token={tokenActivacion}";

        var html = $@"
            <h2>¡Hola {destinatarioNombre}!</h2>
            <p>Detectamos compras previas en Petshop Sindy con este email.</p>
            <p>Hacé clic para crear tu contraseña y gestionar tus pedidos y mascotas:</p>
            <p><a href=""{linkActivacion}"">Activar mi cuenta</a></p>
            <p>Este enlace vence en 48 horas.</p>
        ";

        return EnviarAsync(destinatarioEmail, "Activá tu cuenta en Petshop Sindy", html);
    }

    private async Task EnviarAsync(string destinatarioEmail, string asunto, string html)
    {
        try
        {
            var body = new
            {
                from = $"{_fromNombre} <{_fromEmail}>",
                to = new[] { destinatarioEmail },
                subject = asunto,
                html,
            };

            var respuesta = await _httpClient.PostAsJsonAsync("emails", body);

            if (!respuesta.IsSuccessStatusCode)
            {
                var error = await respuesta.Content.ReadAsStringAsync();
                _logger.LogWarning(
                    "Resend devolvió un error al enviar el mail a {Email}: {StatusCode} {Error}",
                    destinatarioEmail, respuesta.StatusCode, error);
            }
        }
        catch (Exception ex)
        {
            // Un fallo de email nunca debe tirar abajo el checkout ni el registro.
            _logger.LogWarning(ex, "No se pudo enviar el mail a {Email}", destinatarioEmail);
        }
    }
}