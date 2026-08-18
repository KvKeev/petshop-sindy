using SindyPetshop.Domain.Interfaces;

namespace SindyPetshop.Infrastructure.Services;

public class FileStorageService : IFileStorageService
{
    private readonly string _wwwRootPath;
    private static readonly string[] ExtensionesPermitidas = { ".jpg", ".jpeg", ".png", ".webp" };
    private const long TamanioMaximoBytes = 5 * 1024 * 1024; // 5MB

    public FileStorageService(string wwwRootPath)
    {
        _wwwRootPath = wwwRootPath;
    }

    public async Task<string?> GuardarAsync(Stream contenido, long tamanioBytes, string nombreOriginal, string subcarpeta, string prefijoNombre)
    {
        var extension = Path.GetExtension(nombreOriginal).ToLowerInvariant();
        if (!ExtensionesPermitidas.Contains(extension)) return null;
        if (tamanioBytes <= 0 || tamanioBytes > TamanioMaximoBytes) return null;

        var carpetaFisica = Path.Combine(_wwwRootPath, "uploads", subcarpeta);
        Directory.CreateDirectory(carpetaFisica);

        var nombreArchivo = $"{prefijoNombre}_{Guid.NewGuid():N}{extension}";
        var rutaFisica = Path.Combine(carpetaFisica, nombreArchivo);

        using (var destino = new FileStream(rutaFisica, FileMode.Create))
        {
            await contenido.CopyToAsync(destino);
        }

        return $"/uploads/{subcarpeta}/{nombreArchivo}";
    }

    public void EliminarSiEsSubida(string? urlRelativa)
    {
        if (string.IsNullOrWhiteSpace(urlRelativa)) return;
        if (!urlRelativa.StartsWith("/uploads/")) return; // nunca borra avatares de galería

        var rutaFisica = Path.Combine(_wwwRootPath, urlRelativa.TrimStart('/').Replace('/', Path.DirectorySeparatorChar));
        if (File.Exists(rutaFisica))
            File.Delete(rutaFisica);
    }
}