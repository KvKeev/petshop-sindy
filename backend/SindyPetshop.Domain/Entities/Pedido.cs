namespace SindyPetshop.Domain.Entities;

public enum EstadoPedido
{
    PendientePago, // SOLO para pago Online: stock reservado (no descontado), esperando confirmación de MercadoPago
    Confirmado, // SOLO para PagoEnEntrega: stock ya descontado en firme, pago pendiente de cobrar físicamente
    Pagado,
    ListoParaRetirar,
    Enviado,
    Completado,
    Cancelado,
}

public enum MetodoPago
{
    Online, // MercadoPago vía web
    PagoEnEntrega, // tarjeta/efectivo en el local, o efectivo contra entrega
}

public enum MetodoEntrega
{
    Retiro,
    Envio,
}

public enum OrigenPedido
{
    Web,
    Mostrador,
}

public class Pedido
{
    public int Id { get; set; }

    public int ClienteId { get; set; }
    public Cliente? Cliente { get; set; }

    public int? DireccionId { get; set; }
    public MetodoPago MetodoPago { get; set; }
    public DateTime? ExpiraReservaEn { get; set; } // solo aplica si MetodoPago == Online
    public string? MercadoPagoPreferenceId { get; set; }
    public string? MercadoPagoPaymentId { get; set; }
    public Direccion? Direccion { get; set; }

    public DateTime Fecha { get; set; } = DateTime.UtcNow;
    public EstadoPedido Estado { get; set; } = EstadoPedido.PendientePago;
    public MetodoEntrega MetodoEntrega { get; set; }
    public OrigenPedido Origen { get; set; } = OrigenPedido.Web;
    public decimal Total { get; set; }

    public ICollection<DetallePedido> Detalles { get; set; } = new List<DetallePedido>();
}
