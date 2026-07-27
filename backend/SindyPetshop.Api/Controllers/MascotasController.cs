using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SindyPetshop.Application.DTOs;
using SindyPetshop.Application.Services;

namespace SindyPetshop.Api.Controllers;

[ApiController]
[Route("api/v1/mascotas")]
[Authorize]
public class MascotasController : ControllerBase
{
    private readonly MascotaService _mascotaService;

    public MascotasController(MascotaService mascotaService)
    {
        _mascotaService = mascotaService;
    }

    private int ObtenerClienteId()
    {
        var claim = User.FindFirst(ClaimTypes.NameIdentifier)!.Value;
        return int.Parse(claim);
    }

    [HttpGet]
    public async Task<IActionResult> GetMisMascotas()
    {
        var clienteId = ObtenerClienteId();
        var mascotas = await _mascotaService.GetMisMascotasAsync(clienteId);
        return Ok(mascotas);
    }

    [HttpPost]
    public async Task<IActionResult> Crear(CrearMascotaDto dto)
    {
        var clienteId = ObtenerClienteId();
        var resultado = await _mascotaService.CrearAsync(clienteId, dto);

        if (resultado is null)
            return BadRequest(new { mensaje = "Tipo de mascota inválido. Valores permitidos: Perro, Gato, Ave, Otro" });

        return Ok(resultado);
    }

    // GET /api/v1/mascotas/5/historial -> "¿qué come esta mascota?"
    [HttpGet("{id}/historial")]
    public async Task<IActionResult> GetHistorial(int id)
    {
        var resultado = await _mascotaService.GetConHistorialAsync(id);
        if (resultado is null) return NotFound();

        return Ok(resultado);
    }
}