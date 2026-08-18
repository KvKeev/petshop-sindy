using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SindyPetshop.Application.Services;

namespace SindyPetshop.Api.Controllers;

[ApiController]
[Route("api/v1/admin/analytics")]
[Authorize(Roles = "Admin")]
public class AdminAnalyticsController : ControllerBase
{
    private readonly AdminAnalyticsService _analyticsService;

    public AdminAnalyticsController(AdminAnalyticsService analyticsService)
    {
        _analyticsService = analyticsService;
    }

    [HttpGet("resumen")]
    public async Task<IActionResult> GetResumen(
        [FromQuery] DateTime? desde,
        [FromQuery] DateTime? hasta,
        [FromQuery] string? preset,
        [FromQuery] int? anio,
        [FromQuery] int? mes)
    {
        var (rangoDesde, rangoHasta) = ResolverRango(desde, hasta, preset, anio, mes);
        var resumen = await _analyticsService.GetResumenAsync(rangoDesde, rangoHasta);
        return Ok(resumen);
    }

    [HttpGet("productos-mas-vendidos")]
    public async Task<IActionResult> GetProductosMasVendidos(
        [FromQuery] DateTime? desde,
        [FromQuery] DateTime? hasta,
        [FromQuery] string? preset,
        [FromQuery] int? anio,
        [FromQuery] int? mes,
        [FromQuery] int top = 10,
        [FromQuery] string ordenarPor = "unidades")
    {
        var (rangoDesde, rangoHasta) = ResolverRango(desde, hasta, preset, anio, mes);
        var productos = await _analyticsService.GetProductosMasVendidosAsync(rangoDesde, rangoHasta, top, ordenarPor);
        return Ok(productos);
    }

    // Prioridad de resolución del período: desde+hasta explícitos > anio+mes (mes
    // puntual, no necesariamente el actual) > preset (hoy/semana/mes/anio) > default
    // (últimos 30 días).
    private static (DateTime Desde, DateTime Hasta) ResolverRango(
        DateTime? desde, DateTime? hasta, string? preset, int? anio, int? mes)
    {
        var hoy = DateTime.UtcNow.Date;

        if (desde.HasValue && hasta.HasValue)
            return (desde.Value.Date, hasta.Value.Date.AddDays(1).AddTicks(-1));

        if (anio.HasValue && mes.HasValue)
        {
            var inicioMes = new DateTime(anio.Value, mes.Value, 1);
            return (inicioMes, inicioMes.AddMonths(1).AddTicks(-1));
        }

        return preset?.ToLowerInvariant() switch
        {
            "hoy" => (hoy, hoy.AddDays(1).AddTicks(-1)),
            "semana" => (hoy.AddDays(-6), hoy.AddDays(1).AddTicks(-1)),
            "mes" => (new DateTime(hoy.Year, hoy.Month, 1), hoy.AddDays(1).AddTicks(-1)),
            "anio" => (new DateTime(hoy.Year, 1, 1), hoy.AddDays(1).AddTicks(-1)),
            _ => (hoy.AddDays(-29), hoy.AddDays(1).AddTicks(-1)),
        };
    }
}