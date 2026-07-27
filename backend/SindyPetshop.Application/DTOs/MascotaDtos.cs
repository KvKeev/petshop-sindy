namespace SindyPetshop.Application.DTOs;

public record MascotaDto(int Id, string Nombre, string Tipo);

public record CrearMascotaDto(string Nombre, string Tipo);

// Para responder "¿qué come esta mascota?"
public record CompraMascotaDto(
    string ProductoNombre,
    string VarianteDescripcion, // ej: "Peso: 15kg"
    DateTime Fecha,
    int Cantidad
);

public record MascotaConHistorialDto(
    int Id,
    string Nombre,
    string Tipo,
    IEnumerable<CompraMascotaDto> HistorialCompras
);