using SindyPetshop.Application.DTOs;
using SindyPetshop.Domain.Entities;
using SindyPetshop.Domain.Interfaces;
using SindyPetshop.Application.Validaciones;

namespace SindyPetshop.Application.Services;

public class MascotaService
{
    private readonly IMascotaRepository _mascotaRepository;
    private readonly IProductoRepository _productoRepository; // NUEVO: valida el ProductoId del alimento favorito

    public MascotaService(IMascotaRepository mascotaRepository, IProductoRepository productoRepository)
    {
        _mascotaRepository = mascotaRepository;
        _productoRepository = productoRepository;
    }

    public async Task<IEnumerable<MascotaDto>> GetMisMascotasAsync(int clienteId)
    {
        var mascotas = await _mascotaRepository.GetByClienteIdAsync(clienteId);
        return mascotas.Select(MapearDto);
    }

public async Task<(ResultadoCrearMascota Resultado, MascotaDto? Dto)> CrearAsync(int clienteId, CrearMascotaDto dto)
    {
        if (!NombreValidator.EsValido(dto.Nombre))
            return (ResultadoCrearMascota.NombreInvalido, null);

        if (!Enum.TryParse<TipoMascota>(dto.Tipo, ignoreCase: true, out var tipo))
            return (ResultadoCrearMascota.TipoInvalido, null);

        var mascota = new Mascota
        {
            ClienteId = clienteId,
            Nombre = dto.Nombre,
            Tipo = tipo
        };

        await _mascotaRepository.AddAsync(mascota);
        await _mascotaRepository.SaveChangesAsync();

        return (ResultadoCrearMascota.Ok, MapearDto(mascota));
    }

    // Responde "¿qué come esta mascota?" con el historial REAL de compras (sin cambios de lógica)
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

    // NUEVO: elección pura, nunca inferida de compras. Puede editarla el dueño o un Admin.
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

    private static MascotaDto MapearDto(Mascota m) => new(
        m.Id,
        m.Nombre,
        m.Tipo.ToString(),
        m.AlimentoFavoritoProducto?.Nombre,
        m.AlimentoFavoritoDescripcion,
        m.AlimentoFavoritoActualizadoEn,
        m.AlimentoFavoritoActualizadoPor
    );
}