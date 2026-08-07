using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SindyPetshop.Application.DTOs;
using SindyPetshop.Application.Services;
using SindyPetshop.Domain.Entities;

namespace SindyPetshop.Api.Controllers;

[ApiController]
[Route("api/v1/admin/pedidos")]
[Authorize(Roles = "Admin")]
public class AdminPedidosController : ControllerBase
{
    private readonly AdminPedidoService _adminPedidoService;

    public AdminPedidosController(AdminPedidoService adminPedidoService)
    {
        _adminPedidoService = adminPedidoService;
    }

    [HttpGet]
    public async Task<IActionResult> GetListado(
        int pagina = 1, int tamanioPagina = 20,
        string? estado = null, DateTime? desde = null, DateTime? hasta = null,
        int? clienteId = null, string? metodoPago = null, string? metodoEntrega = null)
    {
        var filtros = new FiltrosPedidoAdmin(
            pagina,
            tamanioPagina,
            Enum.TryParse<EstadoPedido>(estado, true, out var e) ? e : null,
            desde,
            hasta,
            clienteId,
            Enum.TryParse<MetodoPago>(metodoPago, true, out var mp) ? mp : null,
            Enum.TryParse<MetodoEntrega>(metodoEntrega, true, out var me) ? me : null
        );

        var resultado = await _adminPedidoService.GetListadoAsync(filtros);
        return Ok(resultado);
    }

    [HttpPatch("{id}/estado")]
    public async Task<IActionResult> CambiarEstado(int id, CambiarEstadoPedidoDto dto)
    {
        var (exito, error, pedido) = await _adminPedidoService.CambiarEstadoAsync(id, dto.NuevoEstado);
        return exito ? Ok(pedido) : BadRequest(new { mensaje = error });
    }
}