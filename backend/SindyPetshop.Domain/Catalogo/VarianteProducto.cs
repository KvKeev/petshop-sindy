namespace SindyPetshop.Domain.Entities;

public class VarianteProducto
{
    public int Id { get; set; }

    public int ProductoId { get; set; }
    public Producto? Producto { get; set; }

    // Ej: atributo = "Peso", valor = "1kg"
    public string Atributo { get; set; } = string.Empty;
    public string Valor { get; set; } = string.Empty;

    public decimal Precio { get; set; }

    // --- Stock físico vs. reservado para mostrador ---
    public int StockFisico { get; set; }
    public int StockMinimoWeb { get; set; }

    // Propiedad calculada: no ocupa columna en la base de datos (se ignora en EF Core)
    public int StockDisponibleWeb => Math.Max(0, StockFisico - StockMinimoWeb);

    // --- Fraccionamiento (venta de alimento a granel) ---
    public bool EsFraccionable { get; set; }
    public int? VarianteDestinoId { get; set; }
    public VarianteProducto? VarianteDestino { get; set; }
    public int? CantidadFraccionable { get; set; }

    public ICollection<HistorialStock> Movimientos { get; set; } = new List<HistorialStock>();
    public ICollection<DetallePedido> DetallesPedido { get; set; } = new List<DetallePedido>();
}