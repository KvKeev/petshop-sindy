using Microsoft.EntityFrameworkCore;
using SindyPetshop.Application.DTOs;
using SindyPetshop.Domain.Entities;
using SindyPetshop.Infrastructure.Data;

namespace SindyPetshop.Application.Services;

// Inyecta el DbContext directo (mismo criterio documentado que AdminProductoService/
// CategoriasController): es un módulo de solo lectura, de agregación pura, sin
// reglas de negocio que ameriten pasar por un repositorio.
public class AdminAnalyticsService
{
    private readonly SindyPetshopDbContext _context;

    // Se consideran "venta real" todos los pedidos que ya comprometieron stock en
    // firme. Quedan afuera PendientePago (todavía no se pagó, puede vencer solo) y
    // Cancelado (se revirtió).
    private static readonly EstadoPedido[] EstadosVenta =
    {
        EstadoPedido.Confirmado,
        EstadoPedido.Pagado,
        EstadoPedido.ListoParaRetirar,
        EstadoPedido.Enviado,
        EstadoPedido.Completado,
    };

    public AdminAnalyticsService(SindyPetshopDbContext context)
    {
        _context = context;
    }

    public async Task<ResumenVentasDto> GetResumenAsync(DateTime desde, DateTime hasta)
    {
        // Se trae el rango completo a memoria y se agrupa ahí: el volumen de un
        // petshop barrial no justifica agregaciones complejas en SQLite, y evita
        // problemas de traducción de LINQ (agrupar por día calendario, tuplas de enum).
        var pedidos = await _context.Pedidos
            .Include(p => p.Detalles)
            .Where(p => p.Fecha >= desde && p.Fecha <= hasta && EstadosVenta.Contains(p.Estado))
            .ToListAsync();

        var totalVentas = pedidos.Sum(p => p.Total);
        var cantidadPedidos = pedidos.Count;
        var totalUnidades = pedidos.Sum(p => p.Detalles.Sum(d => d.Cantidad));

        var porMetodoPago = pedidos
            .GroupBy(p => (p.MetodoPago, p.SubMetodoPagoEntrega))
            .Select(g => new VentaPorMetodoPagoDto(
                g.Key.MetodoPago.ToString(),
                g.Key.SubMetodoPagoEntrega?.ToString(),
                g.Count(),
                g.Sum(p => p.Total)
            ))
            .OrderByDescending(v => v.Total)
            .ToList();

        var porDia = pedidos
            .GroupBy(p => DateOnly.FromDateTime(p.Fecha))
            .Select(g => new VentaPorDiaDto(
                g.Key,
                g.Sum(p => p.Total),
                g.Count(),
                g.Sum(p => p.Detalles.Sum(d => d.Cantidad))
            ))
            .OrderBy(v => v.Fecha)
            .ToList();

        return new ResumenVentasDto(
            desde,
            hasta,
            totalVentas,
            cantidadPedidos,
            totalUnidades,
            cantidadPedidos == 0 ? 0 : Math.Round(totalVentas / cantidadPedidos, 2),
            porMetodoPago,
            porDia
        );
    }

    public async Task<IEnumerable<ProductoMasVendidoDto>> GetProductosMasVendidosAsync(
        DateTime desde, DateTime hasta, int top, string ordenarPor)
    {
        var detalles = await _context.DetallesPedido
            .Include(d => d.Variante!)
                .ThenInclude(v => v.Producto)
            .Include(d => d.Pedido)
            .Where(d => d.Pedido!.Fecha >= desde && d.Pedido.Fecha <= hasta && EstadosVenta.Contains(d.Pedido.Estado))
            .ToListAsync();

        var agrupado = detalles
            .GroupBy(d => new
            {
                d.Variante!.ProductoId,
                Nombre = d.Variante.Producto?.Nombre ?? string.Empty,
                d.Variante.Atributo,
                d.Variante.Valor,
            })
            .Select(g => new ProductoMasVendidoDto(
                g.Key.ProductoId,
                g.Key.Nombre,
                $"{g.Key.Atributo}: {g.Key.Valor}",
                g.Sum(d => d.Cantidad),
                g.Sum(d => d.Cantidad * d.PrecioUnitario)
            ));

        agrupado = ordenarPor.Equals("monto", StringComparison.OrdinalIgnoreCase)
            ? agrupado.OrderByDescending(p => p.MontoTotal)
            : agrupado.OrderByDescending(p => p.UnidadesVendidas);

        return agrupado.Take(top).ToList();
    }
}