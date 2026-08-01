using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SindyPetshop.Application.DTOs;
using SindyPetshop.Application.Services;

namespace SindyPetshop.Api.Controllers;

[ApiController]
[Route("api/v1/pedidos")]
[Authorize]
public class PedidosController : ControllerBase
{
    private readonly PedidoService _pedidoService;

    public PedidosController(PedidoService pedidoService)
    {
        _pedidoService = pedidoService;
    }

    private int ObtenerClienteId()
    {
        var claim = User.FindFirst(ClaimTypes.NameIdentifier)!.Value;
        return int.Parse(claim);
    }

    [HttpPost]
    public async Task<IActionResult> Crear(CrearPedidoDto dto)
    {
        var clienteId = ObtenerClienteId();
        var (resultado, detalle, pedido) = await _pedidoService.CrearAsync(clienteId, dto);

        return resultado switch
        {
            ResultadoCrearPedido.Ok => Ok(pedido),
            ResultadoCrearPedido.CarritoVacio => BadRequest(new { mensaje = "El carrito está vacío" }),
            ResultadoCrearPedido.MetodoInvalido => BadRequest(new { mensaje = detalle }),
            ResultadoCrearPedido.VarianteInvalida => BadRequest(new { mensaje = detalle }),
            ResultadoCrearPedido.StockInsuficiente => Conflict(new { mensaje = detalle }),
            ResultadoCrearPedido.DireccionRequerida => BadRequest(new { mensaje = "Falta indicar la dirección de envío" }),
            ResultadoCrearPedido.DireccionInvalida => BadRequest(new { mensaje = "La dirección no pertenece al cliente" }),
            _ => BadRequest(),
        };
    }

    [HttpGet]
    public async Task<IActionResult> GetMisPedidos()
    {
        var clienteId = ObtenerClienteId();
        var pedidos = await _pedidoService.GetMisPedidosAsync(clienteId);
        return Ok(pedidos);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetDetalle(int id)
    {
        var clienteId = ObtenerClienteId();
        var esAdmin = User.IsInRole("Admin");
        var (resultado, dto) = await _pedidoService.GetDetalleAsync(id, clienteId, esAdmin);

        return resultado switch
        {
            ResultadoConsulta.NoEncontrada => NotFound(),
            ResultadoConsulta.NoAutorizado => Forbid(),
            _ => Ok(dto),
        };
    }
}