using Microsoft.EntityFrameworkCore;
using SindyPetshop.Domain.Entities;
using SindyPetshop.Infrastructure.Data;

namespace SindyPetshop.Api.BackgroundServices;

// Cada pocos minutos, cancela pedidos PendientePago cuya reserva ya venció.
// No hace falta "liberar" el stock a mano: StockReservado se calcula en tiempo real
// sumando pedidos PendientePago no vencidos, así que al pasar a Cancelado deja de contar.
public class LiberacionReservasBackgroundService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<LiberacionReservasBackgroundService> _logger;
    private static readonly TimeSpan Intervalo = TimeSpan.FromMinutes(5);

    public LiberacionReservasBackgroundService(
        IServiceProvider serviceProvider,
        ILogger<LiberacionReservasBackgroundService> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await LiberarReservasVencidasAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error liberando reservas de stock vencidas");
            }

            await Task.Delay(Intervalo, stoppingToken);
        }
    }

    private async Task LiberarReservasVencidasAsync()
    {
        // BackgroundService es singleton pero DbContext es scoped -> hay que crear un scope acá
        using var scope = _serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<SindyPetshopDbContext>();

        var vencidos = await context.Pedidos
            .Where(p => p.Estado == EstadoPedido.PendientePago
                     && p.ExpiraReservaEn != null
                     && p.ExpiraReservaEn < DateTime.UtcNow)
            .ToListAsync();

        if (vencidos.Count == 0) return;

        foreach (var pedido in vencidos)
            pedido.Estado = EstadoPedido.Cancelado;

        await context.SaveChangesAsync();
        _logger.LogInformation("Liberadas {Cantidad} reservas de stock vencidas", vencidos.Count);
    }
}