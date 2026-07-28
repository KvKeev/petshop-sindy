namespace SindyPetshop.Domain.Entities;

public enum RolUsuario
{
    Cliente,
    Admin
}

public class Cliente
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public RolUsuario Rol { get; set; } = RolUsuario.Cliente;
    public DateTime FechaRegistro { get; set; } = DateTime.UtcNow;

    public ICollection<Direccion> Direcciones { get; set; } = new List<Direccion>();
    public ICollection<Pedido> Pedidos { get; set; } = new List<Pedido>();
    public ICollection<Mascota> Mascotas { get; set; } = new List<Mascota>();

}