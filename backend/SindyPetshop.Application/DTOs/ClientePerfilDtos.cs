namespace SindyPetshop.Application.DTOs;

public record ClientePerfilDto(int Id, string Nombre, string Email, string? FotoUrl, DateTime FechaRegistro);

public record ActualizarPerfilDto(string Nombre, string Email);

public record CambiarPasswordDto(string PasswordActual, string PasswordNueva);

public record SeleccionarAvatarDto(string AvatarId);

public record AvatarDto(string Id, string Url);

public enum ResultadoActualizarPerfil { Ok, NombreInvalido, EmailInvalido, EmailDuplicado }
public enum ResultadoCambiarPassword { Ok, PasswordActualIncorrecta, PasswordNuevaInvalida }
public enum ResultadoSubirFoto { Ok, ArchivoInvalido }
public enum ResultadoSeleccionarAvatar { Ok, AvatarInvalido }