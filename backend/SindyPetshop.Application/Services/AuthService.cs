using SindyPetshop.Application.DTOs;
using SindyPetshop.Application.Validaciones;
using SindyPetshop.Domain.Entities;
using SindyPetshop.Domain.Interfaces;

namespace SindyPetshop.Application.Services;

public class AuthService
{
    private readonly IClienteRepository _clienteRepository;
    private readonly ITokenService _tokenService;
    private readonly IEmailService _emailService;

    private const int HorasExpiracionActivacion = 48;

    public AuthService(IClienteRepository clienteRepository, ITokenService tokenService, IEmailService emailService)
    {
        _clienteRepository = clienteRepository;
        _tokenService = tokenService;
        _emailService = emailService;
    }

    public async Task<(ResultadoRegistro Resultado, AuthResponseDto? Dto)> RegistrarAsync(RegistroDto dto)
    {
        if (!NombreValidator.EsValido(dto.Nombre))
            return (ResultadoRegistro.NombreInvalido, null);

        var existente = await _clienteRepository.GetByEmailAsync(dto.Email);

        if (existente is not null)
        {
            if (existente.PasswordHash is not null)
                return (ResultadoRegistro.EmailDuplicado, null);

            // Cuenta creada antes por una compra de invitado: en vez de rechazar el
            // registro, se genera (o renueva) el token y se reenvía el mail de activación.
            existente.ActivacionToken = Guid.NewGuid();
            existente.ActivacionTokenExpira = DateTime.UtcNow.AddHours(HorasExpiracionActivacion);
            _clienteRepository.Update(existente);
            await _clienteRepository.SaveChangesAsync();

            await _emailService.EnviarActivacionCuentaAsync(
                existente.Email, existente.Nombre, existente.ActivacionToken.Value);

            return (ResultadoRegistro.CuentaInvitadaDetectada, null);
        }

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
        return (ResultadoRegistro.Ok, new AuthResponseDto(token, cliente.Nombre, cliente.Email, cliente.Rol.ToString()));
    }

    public async Task<AuthResponseDto?> LoginAsync(LoginDto dto)
    {
        var cliente = await _clienteRepository.GetByEmailAsync(dto.Email);
        if (cliente is null) return null;

        if (cliente.PasswordHash is null) return null; // cuenta creada por compra de invitado, todavía sin contraseña

        var passwordValida = BCrypt.Net.BCrypt.Verify(dto.Password, cliente.PasswordHash);
        if (!passwordValida) return null;

        var token = _tokenService.GenerarToken(cliente);
        return new AuthResponseDto(token, cliente.Nombre, cliente.Email, cliente.Rol.ToString());
    }

    public async Task<(ResultadoActivacion Resultado, AuthResponseDto? Dto)> ActivarCuentaAsync(ActivarCuentaDto dto)
    {
        var cliente = await _clienteRepository.GetByActivacionTokenAsync(dto.Token);

        if (cliente is null
            || cliente.ActivacionTokenExpira is null
            || cliente.ActivacionTokenExpira < DateTime.UtcNow)
            return (ResultadoActivacion.TokenInvalidoOVencido, null);

        if (dto.Password.Length < 6)
            return (ResultadoActivacion.PasswordInvalida, null);

        cliente.PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password);
        cliente.ActivacionToken = null;
        cliente.ActivacionTokenExpira = null;
        _clienteRepository.Update(cliente);
        await _clienteRepository.SaveChangesAsync();

        var token = _tokenService.GenerarToken(cliente);
        return (ResultadoActivacion.Ok, new AuthResponseDto(token, cliente.Nombre, cliente.Email, cliente.Rol.ToString()));
    }
}