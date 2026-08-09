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

    private string ObtenerIdentificadorActor()
    {
        var email = User.FindFirst(ClaimTypes.Email)?.Value ?? "desconocido";
        return User.IsInRole("Admin") ? $"Admin ({email})" : email;
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
        var (resultado, mascota) = await _mascotaService.CrearAsync(clienteId, dto);

        return resultado switch
        {
            ResultadoCrearMascota.NombreInvalido => BadRequest(new { mensaje = "El nombre solo puede contener letras, números y espacios" }),
            ResultadoCrearMascota.TipoInvalido => BadRequest(new
            {
                mensaje = "Tipo de mascota inválido. Valores permitidos: Perro, Gato, Ave, Conejo, Hamster, Otro",
            }),
            _ => Ok(mascota),
        };
    }

    // GET /api/v1/mascotas/5/historial -> "¿qué come esta mascota?" (compras reales)
    [HttpGet("{id}/historial")]
    public async Task<IActionResult> GetHistorial(int id)
    {
        var clienteId = ObtenerClienteId();
        var esAdmin = User.IsInRole("Admin");

        var (resultado, dto) = await _mascotaService.GetConHistorialAsync(id, clienteId, esAdmin);

        return resultado switch
        {
            ResultadoConsulta.NoEncontrada => NotFound(),
            ResultadoConsulta.NoAutorizado => Forbid(),
            _ => Ok(dto),
        };
    }

    // PUT /api/v1/mascotas/5/alimento-favorito -> elección curada, admin o dueño
    [HttpPut("{id}/alimento-favorito")]
    public async Task<IActionResult> ActualizarAlimentoFavorito(int id, ActualizarAlimentoFavoritoDto dto)
    {
        var clienteId = ObtenerClienteId();
        var esAdmin = User.IsInRole("Admin");
        var actor = ObtenerIdentificadorActor();

        var (resultado, detalle, mascota) = await _mascotaService.ActualizarAlimentoFavoritoAsync(
            id, clienteId, esAdmin, dto, actor);

        return resultado switch
        {
            ResultadoConsulta.NoEncontrada => NotFound(new { mensaje = detalle }),
            ResultadoConsulta.NoAutorizado => Forbid(),
            _ => Ok(mascota),
        };
    }
    // GET /api/v1/mascotas/avatares?tipo=Perro -> galería de avatares para ese tipo de animal
    [HttpGet("avatares")]
    public IActionResult GetAvatares(string tipo)
    {
        return Ok(_mascotaService.GetAvataresPorTipo(tipo));
    }

    // POST /api/v1/mascotas/5/foto -> sube una foto propia (dueño o Admin)
    [HttpPost("{id}/foto")]
    public async Task<IActionResult> SubirFoto(int id, IFormFile archivo)
    {
        if (archivo is null || archivo.Length == 0)
            return BadRequest(new { mensaje = "No se recibió ningún archivo" });

        var clienteId = ObtenerClienteId();
        var esAdmin = User.IsInRole("Admin");

        using var stream = archivo.OpenReadStream();
        var (resultado, mascota) = await _mascotaService.SubirFotoAsync(
            id, clienteId, esAdmin, stream, archivo.Length, archivo.FileName);

        return resultado switch
        {
            ResultadoSubirFotoMascota.NoEncontrada => NotFound(),
            ResultadoSubirFotoMascota.NoAutorizado => Forbid(),
            ResultadoSubirFotoMascota.ArchivoInvalido => BadRequest(new { mensaje = "Archivo inválido. Formatos permitidos: jpg, jpeg, png, webp. Tamaño máximo: 5MB" }),
            _ => Ok(mascota),
        };
    }

    // PUT /api/v1/mascotas/5/avatar -> elige un avatar de la galería (dueño o Admin)
    [HttpPut("{id}/avatar")]
    public async Task<IActionResult> SeleccionarAvatar(int id, SeleccionarAvatarMascotaDto dto)
    {
        var clienteId = ObtenerClienteId();
        var esAdmin = User.IsInRole("Admin");

        var (resultado, mascota) = await _mascotaService.SeleccionarAvatarAsync(id, clienteId, esAdmin, dto);

        return resultado switch
        {
            ResultadoSeleccionarAvatarMascota.NoEncontrada => NotFound(),
            ResultadoSeleccionarAvatarMascota.NoAutorizado => Forbid(),
            ResultadoSeleccionarAvatarMascota.AvatarInvalido => BadRequest(new { mensaje = "Avatar inválido para este tipo de mascota" }),
            _ => Ok(mascota),
        };
    }
}