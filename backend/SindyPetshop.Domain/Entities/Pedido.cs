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
    PagoEnEntrega, // tarjeta/efectivo en el local, o efectivo/QR/transferencia contra entrega
}

// Solo aplica cuando MetodoPago == PagoEnEntrega. Aclara al local/repartidor cómo se
// va a cobrar en el momento.
public enum SubMetodoPagoEntrega
{
    Efectivo,
    CuentaDNI_QR,
    Transferencia,
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
    public SubMetodoPagoEntrega? SubMetodoPagoEntrega { get; set; }
    public DateTime? ExpiraReservaEn { get; set; } // solo aplica si MetodoPago == Online
    public string? MercadoPagoPreferenceId { get; set; }
    public string? MercadoPagoPaymentId { get; set; }
    public string? MercadoPagoInitPoint { get; set; }
    public Direccion? Direccion { get; set; }

    public DateTime Fecha { get; set; } = DateTime.UtcNow;
    public EstadoPedido Estado { get; set; } = EstadoPedido.PendientePago;
    public MetodoEntrega MetodoEntrega { get; set; }
    public OrigenPedido Origen { get; set; } = OrigenPedido.Web;
    public decimal CostoEnvio { get; set; }
    public decimal Total { get; set; }

    // Llave de acceso público para consultar el pedido sin login (GET /pedidos/seguimiento/{token}).
    // Se genera siempre, para todos los pedidos, no solo invitados.
    public Guid TrackingToken { get; set; } = Guid.NewGuid();

    public ICollection<DetallePedido> Detalles { get; set; } = new List<DetallePedido>();
}