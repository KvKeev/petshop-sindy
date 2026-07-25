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

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

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