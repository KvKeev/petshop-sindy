using SindyPetshop.Application.DTOs;
using SindyPetshop.Domain.Entities;
using SindyPetshop.Domain.Interfaces;

namespace SindyPetshop.Application.Services;

public class MascotaService
{
    private readonly IMascotaRepository _mascotaRepository;

    public MascotaService(IMascotaRepository mascotaRepository)
    {
        _mascotaRepository = mascotaRepository;
    }

    public async Task<IEnumerable<MascotaDto>> GetMisMascotasAsync(int clienteId)
    {
        var mascotas = await _mascotaRepository.GetByClienteIdAsync(clienteId);
        return mascotas.Select(m => new MascotaDto(m.Id, m.Nombre, m.Tipo.ToString()));
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

        return new MascotaDto(mascota.Id, mascota.Nombre, mascota.Tipo.ToString());
    }

    // Responde "¿qué come esta mascota?" con el historial real de compras
    public async Task<MascotaConHistorialDto?> GetConHistorialAsync(int mascotaId)
    {
        var mascota = await _mascotaRepository.GetConHistorialComprasAsync(mascotaId);
        if (mascota is null) return null;

        var historial = mascota.ComprasAsociadas
            .OrderByDescending(d => d.Pedido!.Fecha)
            .Select(d => new CompraMascotaDto(
                d.Variante!.Producto!.Nombre,
                $"{d.Variante.Atributo}: {d.Variante.Valor}",
                d.Pedido!.Fecha,
                d.Cantidad
            ));

        return new MascotaConHistorialDto(
            mascota.Id, mascota.Nombre, mascota.Tipo.ToString(), historial
        );
    }
}