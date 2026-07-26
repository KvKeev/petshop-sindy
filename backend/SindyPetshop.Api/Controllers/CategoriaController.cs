using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SindyPetshop.Application.DTOs;
using SindyPetshop.Infrastructure.Data;

namespace SindyPetshop.Api.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class CategoriasController : ControllerBase
{
    private readonly SindyPetshopDbContext _context;

    public CategoriasController(SindyPetshopDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var categorias = await _context.Categorias
            .Select(c => new CategoriaDto(c.Id, c.Nombre))
            .ToListAsync();

        return Ok(categorias);
    }
}