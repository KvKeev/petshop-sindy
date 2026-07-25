namespace SindyPetshop.Domain.Entities;

public class Categoria
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;

    // Una categoría contiene muchos productos
    public ICollection<Producto> Productos { get; set; } = new List<Producto>();
}