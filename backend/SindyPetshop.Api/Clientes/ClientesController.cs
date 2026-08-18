using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SindyPetshop.Application.DTOs;
using SindyPetshop.Application.Services;

namespace SindyPetshop.Api.Controllers;

[ApiController]
[Route("api/v1/clientes")]
[Authorize]
public class ClientesController : ControllerBase
{
    private readonly ClientePerfilService _clientePerfilService;

    public ClientesController(ClientePerfilService clientePerfilService)
    {
        _clientePerfilService = clientePerfilService;
    }

    private int ObtenerClienteId()
    {
        var claim = User.FindFirst(ClaimTypes.NameIdentifier)!.Value;
        return int.Parse(claim);
    }

    [HttpGet("perfil")]
    public async Task<IActionResult> GetPerfil()
    {
        var perfil = await _clientePerfilService.GetPerfilAsync(ObtenerClienteId());
        return perfil is null ? NotFound() : Ok(perfil);
    }

    [HttpPut("perfil")]
    public async Task<IActionResult> ActualizarPerfil(ActualizarPerfilDto dto)
    {
        var (resultado, perfil) = await _clientePerfilService.ActualizarPerfilAsync(ObtenerClienteId(), dto);

        return resultado switch
        {
            ResultadoActualizarPerfil.NombreInvalido => BadRequest(new { mensaje = "El nombre solo puede contener letras, números y espacios" }),
            ResultadoActualizarPerfil.EmailInvalido => BadRequest(new { mensaje = "El email no tiene un formato válido" }),
            ResultadoActualizarPerfil.EmailDuplicado => Conflict(new { mensaje = "Ese email ya está en uso por otra cuenta" }),
            _ => Ok(perfil),
        };
    }

    [HttpPut("perfil/password")]
    public async Task<IActionResult> CambiarPassword(CambiarPasswordDto dto)
    {
        var resultado = await _clientePerfilService.CambiarPasswordAsync(ObtenerClienteId(), dto);

        return resultado switch
        {
            ResultadoCambiarPassword.PasswordActualIncorrecta => BadRequest(new { mensaje = "La contraseña actual no es correcta" }),
            ResultadoCambiarPassword.PasswordNuevaInvalida => BadRequest(new { mensaje = "La nueva contraseña debe tener al menos 6 caracteres" }),
            _ => Ok(new { mensaje = "Contraseña actualizada correctamente" }),
        };
    }

    [HttpPost("perfil/foto")]
    public async Task<IActionResult> SubirFoto(IFormFile archivo)
    {
        if (archivo is null || archivo.Length == 0)
            return BadRequest(new { mensaje = "No se recibió ningún archivo" });

        using var stream = archivo.OpenReadStream();
        var (resultado, perfil) = await _clientePerfilService.SubirFotoAsync(
            ObtenerClienteId(), stream, archivo.Length, archivo.FileName);

        return resultado switch
        {
            ResultadoSubirFoto.ArchivoInvalido => BadRequest(new { mensaje = "Archivo inválido. Formatos permitidos: jpg, jpeg, png, webp. Tamaño máximo: 5MB" }),
            _ => Ok(perfil),
        };
    }

    [HttpPut("perfil/avatar")]
    public async Task<IActionResult> SeleccionarAvatar(SeleccionarAvatarDto dto)
    {
        var (resultado, perfil) = await _clientePerfilService.SeleccionarAvatarAsync(ObtenerClienteId(), dto);

        return resultado switch
        {
            ResultadoSeleccionarAvatar.AvatarInvalido => BadRequest(new { mensaje = "Avatar inválido" }),
            _ => Ok(perfil),
        };
    }

    [HttpGet("avatares")]
    public IActionResult GetAvatares()
    {
        return Ok(_clientePerfilService.GetAvataresDisponibles());
    }
}