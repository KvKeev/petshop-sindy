using SindyPetshop.Application.Avatares;
using SindyPetshop.Application.DTOs;
using SindyPetshop.Application.Validaciones;
using SindyPetshop.Domain.Entities;
using SindyPetshop.Domain.Interfaces;

namespace SindyPetshop.Application.Services;

public class ClientePerfilService
{
    private readonly IClienteRepository _clienteRepository;
    private readonly IFileStorageService _fileStorageService;

    public ClientePerfilService(IClienteRepository clienteRepository, IFileStorageService fileStorageService)
    {
        _clienteRepository = clienteRepository;
        _fileStorageService = fileStorageService;
    }

    public async Task<ClientePerfilDto?> GetPerfilAsync(int clienteId)
    {
        var cliente = await _clienteRepository.GetByIdAsync(clienteId);
        return cliente is null ? null : MapearDto(cliente);
    }

    public async Task<(ResultadoActualizarPerfil Resultado, ClientePerfilDto? Dto)> ActualizarPerfilAsync(int clienteId, ActualizarPerfilDto dto)
    {
        if (!NombreValidator.EsValido(dto.Nombre))
            return (ResultadoActualizarPerfil.NombreInvalido, null);

        if (!EmailValidator.EsValido(dto.Email))
            return (ResultadoActualizarPerfil.EmailInvalido, null);

        var cliente = await _clienteRepository.GetByIdAsync(clienteId);
        if (cliente is null) return (ResultadoActualizarPerfil.EmailInvalido, null);

        var emailNormalizado = dto.Email.Trim();
        if (!string.Equals(cliente.Email, emailNormalizado, StringComparison.OrdinalIgnoreCase))
        {
            var existente = await _clienteRepository.GetByEmailAsync(emailNormalizado);
            if (existente is not null && existente.Id != clienteId)
                return (ResultadoActualizarPerfil.EmailDuplicado, null);
        }

        cliente.Nombre = dto.Nombre.Trim();
        cliente.Email = emailNormalizado;

        _clienteRepository.Update(cliente);
        await _clienteRepository.SaveChangesAsync();

        return (ResultadoActualizarPerfil.Ok, MapearDto(cliente));
    }

    public async Task<ResultadoCambiarPassword> CambiarPasswordAsync(int clienteId, CambiarPasswordDto dto)
    {
        var cliente = await _clienteRepository.GetByIdAsync(clienteId);
        if (cliente is null) return ResultadoCambiarPassword.PasswordActualIncorrecta;

        if (!BCrypt.Net.BCrypt.Verify(dto.PasswordActual, cliente.PasswordHash))
            return ResultadoCambiarPassword.PasswordActualIncorrecta;

        if (string.IsNullOrWhiteSpace(dto.PasswordNueva) || dto.PasswordNueva.Length < 6)
            return ResultadoCambiarPassword.PasswordNuevaInvalida;

        cliente.PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.PasswordNueva);
        _clienteRepository.Update(cliente);
        await _clienteRepository.SaveChangesAsync();

        return ResultadoCambiarPassword.Ok;
    }

    public async Task<(ResultadoSubirFoto Resultado, ClientePerfilDto? Dto)> SubirFotoAsync(
        int clienteId, Stream contenido, long tamanioBytes, string nombreArchivo)
    {
        var cliente = await _clienteRepository.GetByIdAsync(clienteId);
        if (cliente is null) return (ResultadoSubirFoto.ArchivoInvalido, null);

        var url = await _fileStorageService.GuardarAsync(contenido, tamanioBytes, nombreArchivo, "clientes", $"cliente{clienteId}");
        if (url is null) return (ResultadoSubirFoto.ArchivoInvalido, null);

        _fileStorageService.EliminarSiEsSubida(cliente.FotoUrl);

        cliente.FotoUrl = url;
        _clienteRepository.Update(cliente);
        await _clienteRepository.SaveChangesAsync();

        return (ResultadoSubirFoto.Ok, MapearDto(cliente));
    }

    public async Task<(ResultadoSeleccionarAvatar Resultado, ClientePerfilDto? Dto)> SeleccionarAvatarAsync(int clienteId, SeleccionarAvatarDto dto)
    {
        if (!AvatarCatalog.EsValidoClienteAvatar(dto.AvatarId))
            return (ResultadoSeleccionarAvatar.AvatarInvalido, null);

        var cliente = await _clienteRepository.GetByIdAsync(clienteId);
        if (cliente is null) return (ResultadoSeleccionarAvatar.AvatarInvalido, null);

        _fileStorageService.EliminarSiEsSubida(cliente.FotoUrl);

        cliente.FotoUrl = AvatarCatalog.Clientes.First(a => a.Id == dto.AvatarId).Url;
        _clienteRepository.Update(cliente);
        await _clienteRepository.SaveChangesAsync();

        return (ResultadoSeleccionarAvatar.Ok, MapearDto(cliente));
    }

    public List<AvatarDto> GetAvataresDisponibles() => AvatarCatalog.Clientes;

    private static ClientePerfilDto MapearDto(Cliente cliente) =>
        new(cliente.Id, cliente.Nombre, cliente.Email, cliente.FotoUrl, cliente.FechaRegistro);
}