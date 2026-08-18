using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SindyPetshop.Application.Services;

namespace SindyPetshop.Api.Controllers;

[ApiController]
[Route("api/v1/admin/mascotas")]
[Authorize(Roles = "Admin")]
public class AdminMascotasController : ControllerBase
{
    private readonly AdminClienteService _adminClienteService;

    public AdminMascotasController(AdminClienteService adminClienteService)
    {
        _adminClienteService = adminClienteService;
    }

    [HttpGet]
    public async Task<IActionResult> GetListado(int pagina = 1, int tamanioPagina = 20, string? nombre = null)
    {
        var resultado = await _adminClienteService.GetListadoMascotasAsync(pagina, tamanioPagina, nombre);
        return Ok(resultado);
    }
}