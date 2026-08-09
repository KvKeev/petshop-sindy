namespace SindyPetshop.Domain.Interfaces;

public interface IFileStorageService
{
    // Valida extensión y tamaño, guarda el archivo y devuelve la URL relativa (ej. "/uploads/clientes/xxx.jpg").
    // Devuelve null si el archivo no es válido.
    Task<string?> GuardarAsync(Stream contenido, long tamanioBytes, string nombreOriginal, string subcarpeta, string prefijoNombre);

    // Borra un archivo previamente subido. Nunca borra avatares de la galería (esos son fijos, no se tocan).
    void EliminarSiEsSubida(string? urlRelativa);
}