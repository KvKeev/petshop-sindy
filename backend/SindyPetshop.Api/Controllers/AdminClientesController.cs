using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SindyPetshop.Application.Services;

namespace SindyPetshop.Api.Controllers;

[ApiController]
[Route("api/v1/admin/clientes")]
[Authorize(Roles = "Admin")]
public class AdminClientesController : ControllerBase
{
    private readonly AdminClienteService _adminClienteService;

    public AdminClientesController(AdminClienteService adminClienteService)
    {
        _adminClienteService = adminClienteService;
    }

    [HttpGet]
    public async Task<IActionResult> GetListado(int pagina = 1, int tamanioPagina = 20, string? nombre = null, string? email = null)
    {
        var resultado = await _adminClienteService.GetListadoAsync(pagina, tamanioPagina, nombre, email);
        return Ok(resultado);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetDetalle(int id)
    {
        var cliente = await _adminClienteService.GetDetalleAsync(id);
        return cliente is null ? NotFound() : Ok(cliente);
    }
}