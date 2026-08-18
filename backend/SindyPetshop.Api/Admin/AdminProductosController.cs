using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SindyPetshop.Application.DTOs;
using SindyPetshop.Application.Services;

namespace SindyPetshop.Api.Controllers;

[ApiController]
[Route("api/v1/admin/productos")]
[Authorize(Roles = "Admin")] // TODO el controller es admin-only, sin excepciones
public class AdminProductosController : ControllerBase
{
    private readonly AdminProductoService _adminProductoService;

    public AdminProductosController(AdminProductoService adminProductoService)
    {
        _adminProductoService = adminProductoService;
    }

    [HttpGet]
    public async Task<IActionResult> GetListado(int pagina = 1, int tamanioPagina = 20, int? categoriaId = null)
    {
        var resultado = await _adminProductoService.GetListadoAsync(pagina, tamanioPagina, categoriaId);
        return Ok(resultado);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetDetalle(int id)
    {
        var producto = await _adminProductoService.GetDetalleAsync(id);
        return producto is null ? NotFound() : Ok(producto);
    }

    [HttpPost]
    public async Task<IActionResult> Crear(CrearProductoDto dto)
    {
        var (exito, error, producto) = await _adminProductoService.CrearAsync(dto);
        return exito ? Ok(producto) : BadRequest(new { mensaje = error });
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Actualizar(int id, ActualizarProductoDto dto)
    {
        var (resultado, producto) = await _adminProductoService.ActualizarAsync(id, dto);
        return resultado switch
        {
            ResultadoConsulta.NoEncontrada => NotFound(),
            _ => Ok(producto),
        };
    }

    [HttpPatch("{id}/estado")]
    public async Task<IActionResult> CambiarEstado(int id, CambiarEstadoProductoDto dto)
    {
        var (resultado, producto) = await _adminProductoService.CambiarEstadoAsync(id, dto.Activo);
        return resultado switch
        {
            ResultadoConsulta.NoEncontrada => NotFound(),
            _ => Ok(producto),
        };
    }

    [HttpPost("{id}/variantes")]
    public async Task<IActionResult> AgregarVariante(int id, CrearVarianteDto dto)
    {
        var (resultado, error, producto) = await _adminProductoService.AgregarVarianteAsync(id, dto);
        if (resultado == ResultadoConsulta.NoEncontrada) return NotFound();
        if (error is not null) return BadRequest(new { mensaje = error });
        return Ok(producto);
    }

    [HttpPut("variantes/{varianteId}/ajuste-stock")]
    public async Task<IActionResult> AjustarStock(int varianteId, AjustarStockDto dto)
    {
        var (exito, error, variante) = await _adminProductoService.AjustarStockAsync(varianteId, dto);
        return exito ? Ok(variante) : BadRequest(new { mensaje = error });
    }
}