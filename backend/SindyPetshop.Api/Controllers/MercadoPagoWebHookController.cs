using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using SindyPetshop.Application.Services;
using SindyPetshop.Domain.Interfaces;

namespace SindyPetshop.Api.Controllers;

[ApiController]
[Route("api/v1/webhooks/mercadopago")]
public class MercadoPagoWebhookController : ControllerBase
{
    private readonly IMercadoPagoService _mercadoPagoService;
    private readonly AdminPedidoService _adminPedidoService;
    private readonly IConfiguration _configuration;
    private readonly ILogger<MercadoPagoWebhookController> _logger;

    public MercadoPagoWebhookController(
        IMercadoPagoService mercadoPagoService,
        AdminPedidoService adminPedidoService,
        IConfiguration configuration,
        ILogger<MercadoPagoWebhookController> logger)
    {
        _mercadoPagoService = mercadoPagoService;
        _adminPedidoService = adminPedidoService;
        _configuration = configuration;
        _logger = logger;
    }

    // Sin [Authorize]: lo llama MercadoPago, no un usuario logueado.
    // La seguridad acá pasa por validar la firma (x-signature), no por JWT.
    [HttpPost]
    public async Task<IActionResult> Recibir(
        [FromQuery(Name = "data.id")] string? dataId,
        [FromQuery] string? id)
    {
        var paymentIdStr = dataId ?? id;
        if (string.IsNullOrWhiteSpace(paymentIdStr))
            return Ok(); // nada que procesar; devolvemos 200 igual para que MP no reintente en loop

        var xSignature = Request.Headers["x-signature"].ToString();
        var xRequestId = Request.Headers["x-request-id"].ToString();

        if (!ValidarFirma(paymentIdStr, xRequestId, xSignature))
        {
            _logger.LogWarning("Webhook de MercadoPago recibido con firma inválida o ausente");
            return Unauthorized();
        }

        if (!long.TryParse(paymentIdStr, out var paymentId))
            return Ok();

        var resultado = await _mercadoPagoService.ConsultarPagoAsync(paymentId);
        if (resultado is null)
        {
            _logger.LogWarning("No se pudo consultar el pago {PaymentId} contra la API de MercadoPago", paymentId);
            return Ok();
        }

        var (status, externalReference) = resultado.Value;

        if (status != "approved" || string.IsNullOrEmpty(externalReference))
            return Ok(); // pago rechazado, pendiente, etc. - no hacemos nada

        if (!int.TryParse(externalReference, out var pedidoId))
            return Ok();

        // Reutiliza la máquina de estados + el fix de stock de la Parte 1.
        // Es idempotente: si el pedido ya no está en PendientePago (webhook duplicado), CambiarEstadoAsync
        // devuelve Exito=false silenciosamente acá - no rompemos nada ni duplicamos el descuento de stock.
        await _adminPedidoService.CambiarEstadoAsync(pedidoId, "Pagado");

        return Ok();
    }

    private bool ValidarFirma(string dataId, string xRequestId, string xSignature)
    {
        var secret = _configuration["MercadoPago:WebhookSecret"];
        if (string.IsNullOrEmpty(secret) || string.IsNullOrEmpty(xSignature))
            return false;

        string? ts = null;
        string? hash = null;

        foreach (var parte in xSignature.Split(','))
        {
            var kv = parte.Split('=', 2);
            if (kv.Length != 2) continue;

            var clave = kv[0].Trim();
            var valor = kv[1].Trim();

            if (clave == "ts") ts = valor;
            if (clave == "v1") hash = valor;
        }

        if (ts is null || hash is null) return false;

        var manifest = $"id:{dataId.ToLowerInvariant()};request-id:{xRequestId};ts:{ts};";

        using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(secret));
        var computado = Convert.ToHexString(hmac.ComputeHash(Encoding.UTF8.GetBytes(manifest))).ToLowerInvariant();

        return computado == hash;
    }
}