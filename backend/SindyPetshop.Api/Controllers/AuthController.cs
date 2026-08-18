using Microsoft.AspNetCore.Mvc;
using SindyPetshop.Application.DTOs;
using SindyPetshop.Application.Services;
using Microsoft.AspNetCore.RateLimiting;

namespace SindyPetshop.Api.Controllers;

[ApiController]
[Route("api/v1/auth")]
public class AuthController : ControllerBase
{
    private readonly AuthService _authService;

    public AuthController(AuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("registro")]
    public async Task<IActionResult> Registro(RegistroDto dto)
    {
        var (resultado, respuesta) = await _authService.RegistrarAsync(dto);

        return resultado switch
        {
            ResultadoRegistro.NombreInvalido => BadRequest(new { mensaje = "El nombre solo puede contener letras, números y espacios" }),
            ResultadoRegistro.EmailDuplicado => Conflict(new { mensaje = "El email ya está registrado" }),
            ResultadoRegistro.CuentaInvitadaDetectada => Ok(new
            {
                mensaje = "¡Ya teníamos compras registradas con este email! Te enviamos un correo para activar tu cuenta."
            }),
            _ => Ok(respuesta),
        };
    }

    [HttpPost("login")]
    [EnableRateLimiting("LoginPolicy")]
    public async Task<IActionResult> Login(LoginDto dto)
    {
        var resultado = await _authService.LoginAsync(dto);
        if (resultado is null)
            return Unauthorized(new { mensaje = "Credenciales inválidas" });

        return Ok(resultado);
    }

    [HttpPost("activar-cuenta")]
    public async Task<IActionResult> ActivarCuenta(ActivarCuentaDto dto)
    {
        var (resultado, respuesta) = await _authService.ActivarCuentaAsync(dto);

        return resultado switch
        {
            ResultadoActivacion.TokenInvalidoOVencido => BadRequest(new
            {
                mensaje = "El link de activación no es válido o venció. Volvé a intentar registrarte con el mismo email para recibir uno nuevo."
            }),
            ResultadoActivacion.PasswordInvalida => BadRequest(new { mensaje = "La contraseña debe tener al menos 6 caracteres" }),
            _ => Ok(respuesta),
        };
    }
}