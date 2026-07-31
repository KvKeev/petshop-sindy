using Microsoft.EntityFrameworkCore;
using SindyPetshop.Domain.Entities;

namespace SindyPetshop.Infrastructure.Data;

public class SindyPetshopDbContext : DbContext
{
    public SindyPetshopDbContext(DbContextOptions<SindyPetshopDbContext> options)
        : base(options) { }

    public DbSet<Categoria> Categorias => Set<Categoria>();
    public DbSet<Producto> Productos => Set<Producto>();
    public DbSet<VarianteProducto> VariantesProducto => Set<VarianteProducto>();
    public DbSet<HistorialStock> HistorialStock => Set<HistorialStock>();
    public DbSet<Cliente> Clientes => Set<Cliente>();
    public DbSet<Direccion> Direcciones => Set<Direccion>();
    public DbSet<Pedido> Pedidos => Set<Pedido>();
    public DbSet<DetallePedido> DetallesPedido => Set<DetallePedido>();
    public DbSet<Mascota> Mascotas => Set<Mascota>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        // --- Cliente ---
        modelBuilder.Entity<Cliente>(entity =>
        {
            entity.HasIndex(c => c.Email).IsUnique(); // no puede haber dos clientes con el mismo email
            entity.Property(c => c.Rol).HasConversion<string>();
        });
        modelBuilder.Entity<Mascota>(entity =>
        {
            entity.Property(m => m.Tipo).HasConversion<string>();
        });

        // --- Pedido ---
        modelBuilder.Entity<Pedido>(entity =>
        {
            entity.Property(p => p.Estado).HasConversion<string>();
            entity.Property(p => p.MetodoEntrega).HasConversion<string>();
            entity.Property(p => p.Origen).HasConversion<string>();
            entity.Property(p => p.MetodoPago).HasConversion<string>(); // <- agregar esta línea
            entity.Property(p => p.Total).HasPrecision(10, 2);

            entity.HasOne(p => p.Direccion)
                .WithMany()
                .HasForeignKey(p => p.DireccionId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // --- DetallePedido ---
        modelBuilder.Entity<DetallePedido>(entity =>
        {
            entity.Property(d => d.PrecioUnitario).HasPrecision(10, 2);

            entity.HasOne(d => d.Variante)
                .WithMany(v => v.DetallesPedido)
                .HasForeignKey(d => d.VarianteId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(d => d.Mascota) // <- nuevo
                .WithMany(m => m.ComprasAsociadas)
                .HasForeignKey(d => d.MascotaId)
                .OnDelete(DeleteBehavior.SetNull);
        });


        // --- VarianteProducto ---
        modelBuilder.Entity<VarianteProducto>(entity =>
        {
            // La propiedad calculada NO es una columna de la base de datos
            entity.Ignore(v => v.StockDisponibleWeb);

            // Precio: precisión decimal explícita (evita warnings de EF Core con SQLite)
            entity.Property(v => v.Precio).HasPrecision(10, 2);

            // Auto-referencia: una variante "origen" apunta a una variante "destino"
            // Sin cascada de borrado, para no arrastrar variantes relacionadas sin querer
            entity.HasOne(v => v.VarianteDestino)
                  .WithMany()
                  .HasForeignKey(v => v.VarianteDestinoId)
                  .OnDelete(DeleteBehavior.Restrict);
        });

        // --- HistorialStock ---
        modelBuilder.Entity<HistorialStock>(entity =>
        {
            // El enum se guarda como texto legible en vez de número (0, 1, 2...)
            entity.Property(h => h.TipoMovimiento)
                  .HasConversion<string>();
        });
    }
}