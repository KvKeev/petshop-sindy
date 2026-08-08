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
}