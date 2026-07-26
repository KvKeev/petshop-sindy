namespace SindyPetshop.Domain.Entities;

public enum EstadoPedido
{
    PendientePago,
    Pagado,
    ListoParaRetirar,
    Enviado,
    Completado,
    Cancelado
}

public enum MetodoEntrega
{
    Retiro,
    Envio
}

public class Pedido
{
    public int Id { get; set; }

    public int ClienteId { get; set; }
    public Cliente? Cliente { get; set; }

    // Nullable: si el método de entrega es "Retiro", no hace falta dirección
    public int? DireccionId { get; set; }
    public Direccion? Direccion { get; set; }

    public DateTime Fecha { get; set; } = DateTime.UtcNow;
    public EstadoPedido Estado { get; set; } = EstadoPedido.PendientePago;
    public MetodoEntrega MetodoEntrega { get; set; }
    public decimal Total { get; set; }

    public ICollection<DetallePedido> Detalles { get; set; } = new List<DetallePedido>();
}