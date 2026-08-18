using Microsoft.AspNetCore.Mvc;
using SindyPetshop.Application.Services;

namespace SindyPetshop.Api.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class ProductosController : ControllerBase
{
    private readonly ProductoService _productoService;

    public ProductosController(ProductoService productoService)
    {
        _productoService = productoService;
    }

    // GET /api/v1/productos?pagina=1&tamanioPagina=20&categoriaId=3
    [HttpGet]
    public async Task<IActionResult> GetCatalogo(
        [FromQuery] int pagina = 1,
        [FromQuery] int tamanioPagina = 20,
        [FromQuery] int? categoriaId = null)
    {
        var resultado = await _productoService.GetCatalogoAsync(pagina, tamanioPagina, categoriaId);
        return Ok(resultado);
    }

    // GET /api/v1/productos/5
    [HttpGet("{id}")]
    public async Task<IActionResult> GetDetalle(int id)
    {
        var producto = await _productoService.GetDetalleAsync(id);
        if (producto is null) return NotFound();

        return Ok(producto);
    }
}