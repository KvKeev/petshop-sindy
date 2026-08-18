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
    // Nullable: una cuenta creada automáticamente por una compra de invitado no tiene
    // contraseña todavía - queda pendiente de activación (magic link). Mientras sea
    // null, el login normal debe rechazarse (ver AuthService.LoginAsync).
    public string? PasswordHash { get; set; }
    public RolUsuario Rol { get; set; } = RolUsuario.Cliente;
    public DateTime FechaRegistro { get; set; } = DateTime.UtcNow;
    public string? Telefono { get; set; }

    // Token de un solo uso para activar una cuenta creada por compra de invitado
    // (definir contraseña por primera vez). Se limpia (vuelve a null) apenas se usa.
    public Guid? ActivacionToken { get; set; }
    public DateTime? ActivacionTokenExpira { get; set; }

    public ICollection<Direccion> Direcciones { get; set; } = new List<Direccion>();
    public ICollection<Pedido> Pedidos { get; set; } = new List<Pedido>();
    public ICollection<Mascota> Mascotas { get; set; } = new List<Mascota>();
    public string? FotoUrl { get; set; }
}