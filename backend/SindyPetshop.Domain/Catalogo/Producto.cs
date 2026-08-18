namespace SindyPetshop.Domain.Entities;

public class Producto
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string? Descripcion { get; set; }
    public string? ImagenUrl { get; set; }
    public bool Activo { get; set; } = true;

    public int CategoriaId { get; set; }
    public Categoria? Categoria { get; set; }

    // Un producto tiene una o más variantes (al menos una, aunque sea "Standard")
    public ICollection<VarianteProducto> Variantes { get; set; } = new List<VarianteProducto>();
}