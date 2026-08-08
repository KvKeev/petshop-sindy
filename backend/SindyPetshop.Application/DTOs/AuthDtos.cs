namespace SindyPetshop.Application.DTOs;

public record RegistroDto(string Nombre, string Email, string Password);

public record LoginDto(string Email, string Password);

public record AuthResponseDto(string Token, string Nombre, string Email, string Rol);

// NUEVO: distingue el motivo de fallo del registro (antes solo existía el caso de email duplicado)
public enum ResultadoRegistro
{
    Ok,
    EmailDuplicado,
    NombreInvalido
}