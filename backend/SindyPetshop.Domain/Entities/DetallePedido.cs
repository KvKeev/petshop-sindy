namespace SindyPetshop.Domain.Entities;

public class DetallePedido
{
    public int Id { get; set; }

    public int PedidoId { get; set; }
    public Pedido? Pedido { get; set; }

    public int VarianteId { get; set; }
    public VarianteProducto? Variante { get; set; }

    // Opcional: para qué mascota fue esta línea de compra (trazabilidad de "qué come cada mascota")
    public int? MascotaId { get; set; }
    public Mascota? Mascota { get; set; }

    public int Cantidad { get; set; }
    public decimal PrecioUnitario { get; set; }
}