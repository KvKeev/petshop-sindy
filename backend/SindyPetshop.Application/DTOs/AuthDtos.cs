namespace SindyPetshop.Application.DTOs;

public record RegistroDto(string Nombre, string Email, string Password);

public record LoginDto(string Email, string Password);

public record ActivarCuentaDto(Guid Token, string Password);

public record AuthResponseDto(string Token, string Nombre, string Email, string Rol);

public enum ResultadoRegistro
{
    Ok,
    EmailDuplicado,
    NombreInvalido,
    // Nuevo: el email ya existía con PasswordHash null (cuenta creada por compra de
    // invitado) - no se rechaza, se reenvía un link de activación por mail.
    CuentaInvitadaDetectada,
}

public enum ResultadoActivacion
{
    Ok,
    TokenInvalidoOVencido,
    PasswordInvalida,
}