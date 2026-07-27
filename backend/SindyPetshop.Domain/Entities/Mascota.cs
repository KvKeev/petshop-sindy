namespace SindyPetshop.Domain.Entities;

public enum TipoMascota
{
    Perro,
    Gato,
    Ave,
    Otro
}

public class Mascota
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public TipoMascota Tipo { get; set; }

    public int ClienteId { get; set; }
    public Cliente? Cliente { get; set; }

    public ICollection<DetallePedido> ComprasAsociadas { get; set; } = new List<DetallePedido>();
}