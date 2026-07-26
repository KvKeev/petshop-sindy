namespace SindyPetshop.Application.DTOs;

public record RegistroDto(string Nombre, string Email, string Password);

public record LoginDto(string Email, string Password);

public record AuthResponseDto(string Token, string Nombre, string Email, string Rol);