namespace SindyPetshop.Domain.Entities;

public enum TipoMascota
{
    Perro,
    Gato,
    Ave,
    Conejo,
    Hamster,
    Otro
}

public class Mascota
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public TipoMascota Tipo { get; set; }

    public int ClienteId { get; set; }
    public Cliente? Cliente { get; set; }

    // Alimento favorito: elección curada por admin o dueño, NUNCA inferida de compras.
    // No bloquea ni valida ventas — es puramente informativo.
    public int? AlimentoFavoritoProductoId { get; set; }
    public Producto? AlimentoFavoritoProducto { get; set; }
    public string? AlimentoFavoritoDescripcion { get; set; }
    public DateTime? AlimentoFavoritoActualizadoEn { get; set; }
    public string? AlimentoFavoritoActualizadoPor { get; set; } // email de quién lo cargó/editó
    public string? FotoUrl { get; set; }

    public ICollection<DetallePedido> ComprasAsociadas { get; set; } = new List<DetallePedido>();
}