namespace SindyPetshop.Domain.Entities;

public enum TipoMovimientoStock
{
    Venta,
    Fraccionamiento,
    AjusteManual,
    CargaInicial,
    DevolucionCancelacion
}

public class HistorialStock
{
    public int Id { get; set; }

    public int VarianteId { get; set; }
    public VarianteProducto? Variante { get; set; }

    public TipoMovimientoStock TipoMovimiento { get; set; }
    public int Cantidad { get; set; }
    public DateTime Fecha { get; set; } = DateTime.UtcNow;
    public string? Detalle { get; set; } // NUEVO: motivo opcional, se usa sobre todo en AjusteManual
}