namespace SindyPetshop.Domain.Entities;

public class DetallePedido
{
    public int Id { get; set; }

    public int PedidoId { get; set; }
    public Pedido? Pedido { get; set; }

    public int VarianteId { get; set; }
    public VarianteProducto? Variante { get; set; }

    public int Cantidad { get; set; }
    public decimal PrecioUnitario { get; set; } // "foto" del precio al momento de la compra
}