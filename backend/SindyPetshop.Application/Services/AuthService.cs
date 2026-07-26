using SindyPetshop.Application.DTOs;
using SindyPetshop.Domain.Entities;
using SindyPetshop.Domain.Interfaces;

namespace SindyPetshop.Application.Services;

public class AuthService
{
    private readonly IClienteRepository _clienteRepository;
    private readonly ITokenService _tokenService;

    public AuthService(IClienteRepository clienteRepository, ITokenService tokenService)
    {
        _clienteRepository = clienteRepository;
        _tokenService = tokenService;
    }

    public async Task<AuthResponseDto?> RegistrarAsync(RegistroDto dto)
    {
        var existente = await _clienteRepository.GetByEmailAsync(dto.Email);
        if (existente is not null) return null; // el Controller decide qué código HTTP devolver

        var cliente = new Cliente
        {
            Nombre = dto.Nombre,
            Email = dto.Email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
            Rol = RolUsuario.Cliente
        };

        await _clienteRepository.AddAsync(cliente);
        await _clienteRepository.SaveChangesAsync();

        var token = _tokenService.GenerarToken(cliente);
        return new AuthResponseDto(token, cliente.Nombre, cliente.Email, cliente.Rol.ToString());
    }

    public async Task<AuthResponseDto?> LoginAsync(LoginDto dto)
    {
        var cliente = await _clienteRepository.GetByEmailAsync(dto.Email);
        if (cliente is null) return null;

        var passwordValida = BCrypt.Net.BCrypt.Verify(dto.Password, cliente.PasswordHash);
        if (!passwordValida) return null;

        var token = _tokenService.GenerarToken(cliente);
        return new AuthResponseDto(token, cliente.Nombre, cliente.Email, cliente.Rol.ToString());
    }
}