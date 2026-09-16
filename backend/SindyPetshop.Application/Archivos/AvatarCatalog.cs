using SindyPetshop.Application.DTOs;

namespace SindyPetshop.Application.Avatares;

// Catálogo de avatares predefinidos estático en código (no requiere persistencia en BD ni gestión por panel Admin).
// Mapea a archivos físicos en wwwroot/avatares/clientes/{1..6}.png y wwwroot/avatares/mascotas/{Tipo}/{1..3}.png.
// Los métodos EsValidoClienteAvatar y EsValidoMascotaAvatar previenen que se almacene un avatarId arbitrario o inexistente.
public static class AvatarCatalog
{
    public static readonly List<AvatarDto> Clientes = Enumerable.Range(1, 6)
        .Select(i => new AvatarDto($"cliente-{i}", $"/avatares/clientes/{i}.png"))
        .ToList();

    private static readonly string[] TiposConAvatares = { "Perro", "Gato", "Ave", "Conejo", "Hamster", "Otro" };
    private const int CantidadPorTipo = 3;

    public static List<AvatarDto> GetMascotaAvatares(string tipo)
    {
        if (!TiposConAvatares.Contains(tipo)) return new List<AvatarDto>();

        return Enumerable.Range(1, CantidadPorTipo)
            .Select(i => new AvatarDto($"{tipo.ToLowerInvariant()}-{i}", $"/avatares/mascotas/{tipo}/{i}.png"))
            .ToList();
    }

    public static bool EsValidoClienteAvatar(string avatarId) => Clientes.Any(a => a.Id == avatarId);

    public static bool EsValidoMascotaAvatar(string tipo, string avatarId) =>
        GetMascotaAvatares(tipo).Any(a => a.Id == avatarId);
}