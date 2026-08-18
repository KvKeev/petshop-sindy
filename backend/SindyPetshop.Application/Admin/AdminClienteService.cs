using SindyPetshop.Application.DTOs;
using SindyPetshop.Domain.Interfaces;

namespace SindyPetshop.Application.Services;

public class AdminClienteService
{
    private readonly IClienteRepository _clienteRepository;
    private readonly IMascotaRepository _mascotaRepository;

    public AdminClienteService(IClienteRepository clienteRepository, IMascotaRepository mascotaRepository)
    {
        _clienteRepository = clienteRepository;
        _mascotaRepository = mascotaRepository;
    }

    public async Task<PagedResult<AdminClienteListDto>> GetListadoAsync(
        int pagina, int tamanioPagina, string? nombre, string? email)
    {
        var (items, total) = await _clienteRepository.GetPaginadoAsync(pagina, tamanioPagina, nombre, email);

        var dtos = items.Select(c => new AdminClienteListDto(
            c.Id,
            c.Nombre,
            c.Email,
            c.Rol.ToString(),
            c.FechaRegistro,
            c.Mascotas.Count,
            c.Pedidos.Count
        ));

        return new PagedResult<AdminClienteListDto>(dtos, total, pagina, tamanioPagina);
    }

    public async Task<AdminClienteDetalleDto?> GetDetalleAsync(int clienteId)
    {
        var cliente = await _clienteRepository.GetConDetalleCompletoAsync(clienteId);
        if (cliente is null) return null;

        return new AdminClienteDetalleDto(
            cliente.Id,
            cliente.Nombre,
            cliente.Email,
            cliente.Rol.ToString(),
            cliente.FechaRegistro,
            cliente.Direcciones.Select(d => new AdminDireccionResumenDto(d.Id, d.Calle, d.Ciudad)).ToList(),
            cliente.Mascotas.Select(m => new AdminMascotaResumenDto(
                m.Id, m.Nombre, m.Tipo.ToString(),
                m.AlimentoFavoritoProducto?.Nombre, m.AlimentoFavoritoDescripcion)).ToList(),
            cliente.Pedidos.Select(p => new AdminPedidoResumenDto(
                p.Id, p.Fecha, p.Estado.ToString(), p.MetodoPago.ToString(),
                p.MetodoEntrega.ToString(), p.Total)).ToList()
        );
    }

    public async Task<PagedResult<AdminMascotaListDto>> GetListadoMascotasAsync(
        int pagina, int tamanioPagina, string? nombre)
    {
        var (items, total) = await _mascotaRepository.GetPaginadoConClienteAsync(pagina, tamanioPagina, nombre);

        var dtos = items.Select(m => new AdminMascotaListDto(
            m.Id,
            m.Nombre,
            m.Tipo.ToString(),
            m.ClienteId,
            m.Cliente?.Nombre ?? "",
            m.Cliente?.Email ?? ""
        ));

        return new PagedResult<AdminMascotaListDto>(dtos, total, pagina, tamanioPagina);
    }
}