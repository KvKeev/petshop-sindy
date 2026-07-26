using SindyPetshop.Domain.Entities;

namespace SindyPetshop.Infrastructure.Data.Seed;

public static class DataSeeder
{
    public static void Seed(SindyPetshopDbContext context)
    {
        // Si ya hay categorías, asumimos que ya se sembró antes — no duplicar
        if (context.Categorias.Any()) return;

        var alimentos = new Categoria { Nombre = "Alimentos" };
        var accesorios = new Categoria { Nombre = "Accesorios" };
        context.Categorias.AddRange(alimentos, accesorios);
        context.SaveChanges();

        var alimentoPerro = new Producto
        {
            Nombre = "Alimento Perro Adulto",
            Descripcion = "Alimento balanceado para perros adultos",
            CategoriaId = alimentos.Id,
            Activo = true
        };
        context.Productos.Add(alimentoPerro);
        context.SaveChanges();

        // Variante "Bolsa 20kg", fraccionable
        var bolsa20kg = new VarianteProducto
        {
            ProductoId = alimentoPerro.Id,
            Atributo = "Peso",
            Valor = "20kg (bolsa cerrada)",
            Precio = 45000,
            StockFisico = 5,
            StockMinimoWeb = 1,
            EsFraccionable = true,
            CantidadFraccionable = 20
        };
        context.VariantesProducto.Add(bolsa20kg);
        context.SaveChanges();

        // Variante "Suelto 1kg", destino del fraccionamiento
        var suelto1kg = new VarianteProducto
        {
            ProductoId = alimentoPerro.Id,
            Atributo = "Peso",
            Valor = "1kg (suelto)",
            Precio = 2800,
            StockFisico = 8,
            StockMinimoWeb = 0
        };
        context.VariantesProducto.Add(suelto1kg);
        context.SaveChanges();

        bolsa20kg.VarianteDestinoId = suelto1kg.Id;
        context.SaveChanges();

        var collar = new Producto
        {
            Nombre = "Collar de cuero",
            Descripcion = "Collar de cuero genuino, ajustable",
            CategoriaId = accesorios.Id,
            Activo = true
        };
        context.Productos.Add(collar);
        context.SaveChanges();

        context.VariantesProducto.AddRange(
            new VarianteProducto { ProductoId = collar.Id, Atributo = "Medida", Valor = "S", Precio = 4500, StockFisico = 10, StockMinimoWeb = 2 },
            new VarianteProducto { ProductoId = collar.Id, Atributo = "Medida", Valor = "M", Precio = 5000, StockFisico = 7, StockMinimoWeb = 2 }
        );
        context.SaveChanges();
    }
}