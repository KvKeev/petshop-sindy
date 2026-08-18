using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SindyPetshop.Application.DTOs;
using SindyPetshop.Application.Services;

namespace SindyPetshop.Api.Controllers;

[ApiController]
[Route("api/v1/pedidos")]
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

    // Devuelve el ClienteId solo si el request trae un JWT válido; null si es invitado.
    private int? ObtenerClienteIdAutenticado()
    {
        if (User.Identity?.IsAuthenticated != true) return null;
        var claim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        return claim is null ? null : int.Parse(claim);
    }

    [HttpPost]
    [AllowAnonymous]
    public async Task<IActionResult> Crear(CrearPedidoDto dto)
    {
        var clienteIdAutenticado = ObtenerClienteIdAutenticado();
        var (resultado, detalle, pedido) = await _pedidoService.CrearAsync(clienteIdAutenticado, dto);

        return resultado switch
        {
            ResultadoCrearPedido.Ok => Ok(pedido),
            ResultadoCrearPedido.CarritoVacio => BadRequest(new { mensaje = "El carrito está vacío" }),
            ResultadoCrearPedido.MetodoInvalido => BadRequest(new { mensaje = detalle }),
            ResultadoCrearPedido.VarianteInvalida => BadRequest(new { mensaje = detalle }),
            ResultadoCrearPedido.StockInsuficiente => Conflict(new { mensaje = detalle }),
            ResultadoCrearPedido.DireccionRequerida => BadRequest(new { mensaje = detalle }),
            ResultadoCrearPedido.ClienteInvalido => BadRequest(new { mensaje = detalle }),
            ResultadoCrearPedido.DatosInvitadoIncompletos => BadRequest(new { mensaje = detalle }),
            ResultadoCrearPedido.SubMetodoPagoRequerido => BadRequest(new { mensaje = detalle }),
            ResultadoCrearPedido.SubMetodoPagoInvalido => BadRequest(new { mensaje = detalle }),
            _ => BadRequest(),
        };
    }

    [HttpGet]
    [Authorize]
    public async Task<IActionResult> GetMisPedidos()
    {
        var clienteId = ObtenerClienteId();
        var pedidos = await _pedidoService.GetMisPedidosAsync(clienteId);
        return Ok(pedidos);
    }

    [HttpGet("{id:int}")]
    [Authorize]
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

    // Consulta pública sin login - link de seguimiento que se puede compartir/guardar.
    [HttpGet("seguimiento/{trackingToken:guid}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetSeguimiento(Guid trackingToken)
    {
        var pedido = await _pedidoService.GetPorTrackingTokenAsync(trackingToken);
        return pedido is null ? NotFound() : Ok(pedido);
    }
}