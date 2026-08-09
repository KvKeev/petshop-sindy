using SindyPetshop.Application.Avatares;
using SindyPetshop.Application.DTOs;
using SindyPetshop.Domain.Entities;
using SindyPetshop.Domain.Interfaces;

namespace SindyPetshop.Application.Services;

public class MascotaService
{
    private readonly IMascotaRepository _mascotaRepository;
    private readonly IProductoRepository _productoRepository;
    private readonly IFileStorageService _fileStorageService;

    public MascotaService(
        IMascotaRepository mascotaRepository,
        IProductoRepository productoRepository,
        IFileStorageService fileStorageService)
    {
        _mascotaRepository = mascotaRepository;
        _productoRepository = productoRepository;
        _fileStorageService = fileStorageService;
    }

    public async Task<IEnumerable<MascotaDto>> GetMisMascotasAsync(int clienteId)
    {
        var mascotas = await _mascotaRepository.GetByClienteIdAsync(clienteId);
        return mascotas.Select(MapearDto);
    }

    public async Task<MascotaDto?> CrearAsync(int clienteId, CrearMascotaDto dto)
    {
        if (!Enum.TryParse<TipoMascota>(dto.Tipo, ignoreCase: true, out var tipo))
            return null;

        var mascota = new Mascota
        {
            ClienteId = clienteId,
            Nombre = dto.Nombre,
            Tipo = tipo
        };

        await _mascotaRepository.AddAsync(mascota);
        await _mascotaRepository.SaveChangesAsync();

        return MapearDto(mascota);
    }

    public async Task<(ResultadoConsulta Resultado, MascotaConHistorialDto? Dto)> GetConHistorialAsync(
        int mascotaId, int clienteIdSolicitante, bool esAdmin)
    {
        var mascota = await _mascotaRepository.GetConHistorialComprasAsync(mascotaId);
        if (mascota is null)
            return (ResultadoConsulta.NoEncontrada, null);

        if (mascota.ClienteId != clienteIdSolicitante && !esAdmin)
            return (ResultadoConsulta.NoAutorizado, null);

        var historial = mascota.ComprasAsociadas
            .OrderByDescending(d => d.Pedido!.Fecha)
            .Select(d => new CompraMascotaDto(
                d.Variante!.Producto!.Nombre,
                $"{d.Variante.Atributo}: {d.Variante.Valor}",
                d.Pedido!.Fecha,
                d.Cantidad
            ));

        var dto = new MascotaConHistorialDto(
            mascota.Id, mascota.Nombre, mascota.Tipo.ToString(),
            mascota.AlimentoFavoritoProducto?.Nombre,
            mascota.AlimentoFavoritoDescripcion,
            mascota.AlimentoFavoritoActualizadoEn,
            mascota.AlimentoFavoritoActualizadoPor,
            historial
        );

        return (ResultadoConsulta.Ok, dto);
    }

    public async Task<(ResultadoConsulta Resultado, string? Detalle, MascotaDto? Dto)> ActualizarAlimentoFavoritoAsync(
        int mascotaId, int clienteIdSolicitante, bool esAdmin, ActualizarAlimentoFavoritoDto dto, string actualizadoPor)
    {
        var mascota = await _mascotaRepository.GetConAlimentoFavoritoAsync(mascotaId);
        if (mascota is null)
            return (ResultadoConsulta.NoEncontrada, null, null);

        if (mascota.ClienteId != clienteIdSolicitante && !esAdmin)
            return (ResultadoConsulta.NoAutorizado, null, null);

        if (dto.ProductoId.HasValue)
        {
            var producto = await _productoRepository.GetByIdAsync(dto.ProductoId.Value);
            if (producto is null)
                return (ResultadoConsulta.NoEncontrada, "El producto indicado no existe", null);
        }

        mascota.AlimentoFavoritoProductoId = dto.ProductoId;
        mascota.AlimentoFavoritoDescripcion = dto.Descripcion;
        mascota.AlimentoFavoritoActualizadoEn = DateTime.UtcNow;
        mascota.AlimentoFavoritoActualizadoPor = actualizadoPor;

        _mascotaRepository.Update(mascota);
        await _mascotaRepository.SaveChangesAsync();

        var actualizada = await _mascotaRepository.GetConAlimentoFavoritoAsync(mascotaId);
        return (ResultadoConsulta.Ok, null, MapearDto(actualizada!));
    }

    // NUEVO: sube una foto propia para la mascota (dueño o Admin)
    public async Task<(ResultadoSubirFotoMascota Resultado, MascotaDto? Dto)> SubirFotoAsync(
        int mascotaId, int clienteIdSolicitante, bool esAdmin, Stream contenido, long tamanioBytes, string nombreArchivo)
    {
        var mascota = await _mascotaRepository.GetByIdAsync(mascotaId);
        if (mascota is null)
            return (ResultadoSubirFotoMascota.NoEncontrada, null);

        if (mascota.ClienteId != clienteIdSolicitante && !esAdmin)
            return (ResultadoSubirFotoMascota.NoAutorizado, null);

        var url = await _fileStorageService.GuardarAsync(contenido, tamanioBytes, nombreArchivo, "mascotas", $"mascota{mascotaId}");
        if (url is null)
            return (ResultadoSubirFotoMascota.ArchivoInvalido, null);

        _fileStorageService.EliminarSiEsSubida(mascota.FotoUrl);

        mascota.FotoUrl = url;
        _mascotaRepository.Update(mascota);
        await _mascotaRepository.SaveChangesAsync();

        return (ResultadoSubirFotoMascota.Ok, MapearDto(mascota));
    }

    // NUEVO: elige un avatar de la galería, filtrado por el Tipo de esta mascota (dueño o Admin)
    public async Task<(ResultadoSeleccionarAvatarMascota Resultado, MascotaDto? Dto)> SeleccionarAvatarAsync(
        int mascotaId, int clienteIdSolicitante, bool esAdmin, SeleccionarAvatarMascotaDto dto)
    {
        var mascota = await _mascotaRepository.GetByIdAsync(mascotaId);
        if (mascota is null)
            return (ResultadoSeleccionarAvatarMascota.NoEncontrada, null);

        if (mascota.ClienteId != clienteIdSolicitante && !esAdmin)
            return (ResultadoSeleccionarAvatarMascota.NoAutorizado, null);

        var tipoStr = mascota.Tipo.ToString();
        if (!AvatarCatalog.EsValidoMascotaAvatar(tipoStr, dto.AvatarId))
            return (ResultadoSeleccionarAvatarMascota.AvatarInvalido, null);

        _fileStorageService.EliminarSiEsSubida(mascota.FotoUrl);

        mascota.FotoUrl = AvatarCatalog.GetMascotaAvatares(tipoStr).First(a => a.Id == dto.AvatarId).Url;
        _mascotaRepository.Update(mascota);
        await _mascotaRepository.SaveChangesAsync();

        return (ResultadoSeleccionarAvatarMascota.Ok, MapearDto(mascota));
    }

    // NUEVO: lista los avatares disponibles para un tipo de animal (galería)
    public List<AvatarDto> GetAvataresPorTipo(string tipo) => AvatarCatalog.GetMascotaAvatares(tipo);

    private static MascotaDto MapearDto(Mascota m) => new(
        m.Id,
        m.Nombre,
        m.Tipo.ToString(),
        m.FotoUrl,
        m.AlimentoFavoritoProducto?.Nombre,
        m.AlimentoFavoritoDescripcion,
        m.AlimentoFavoritoActualizadoEn,
        m.AlimentoFavoritoActualizadoPor
    );
}