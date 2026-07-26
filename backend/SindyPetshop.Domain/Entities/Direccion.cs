namespace SindyPetshop.Domain.Entities;

public class Direccion
{
    public int Id { get; set; }
    public int ClienteId { get; set; }
    public Cliente? Cliente { get; set; }

    public string Calle { get; set; } = string.Empty;
    public string Ciudad { get; set; } = string.Empty;
}