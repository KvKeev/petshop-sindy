namespace SindyPetshop.Application.DTOs;

public record MascotaDto(
    int Id,
    string Nombre,
    string Tipo,
    string? FotoUrl,
    string? AlimentoFavoritoNombre,        // nombre del producto, si está en catálogo
    string? AlimentoFavoritoDescripcion,   // texto libre, si no está en catálogo
    DateTime? AlimentoFavoritoActualizadoEn,
    string? AlimentoFavoritoActualizadoPor
);

public record CrearMascotaDto(string Nombre, string Tipo);

// NUEVO: para cargar/editar el alimento favorito
public record ActualizarAlimentoFavoritoDto(int? ProductoId, string? Descripcion);

// Para responder "¿qué come esta mascota?" (historial REAL de compras, sin tocar)
public record CompraMascotaDto(
    string ProductoNombre,
    string VarianteDescripcion,
    DateTime Fecha,
    int Cantidad
);

public record MascotaConHistorialDto(
    int Id,
    string Nombre,
    string Tipo,
    string? AlimentoFavoritoNombre,
    string? AlimentoFavoritoDescripcion,
    DateTime? AlimentoFavoritoActualizadoEn,
    string? AlimentoFavoritoActualizadoPor,
    IEnumerable<CompraMascotaDto> HistorialCompras
);

// NUEVO: foto/avatar de mascota
public record SeleccionarAvatarMascotaDto(string AvatarId);

public enum ResultadoSubirFotoMascota { Ok, NoEncontrada, NoAutorizado, ArchivoInvalido }
public enum ResultadoSeleccionarAvatarMascota { Ok, NoEncontrada, NoAutorizado, AvatarInvalido }

// NUEVO: distingue el motivo de fallo al crear mascota (antes solo existía tipo inválido)
public enum ResultadoCrearMascota
{
    Ok,
    TipoInvalido,
    NombreInvalido
}