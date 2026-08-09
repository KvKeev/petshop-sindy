using SindyPetshop.Application.DTOs;

namespace SindyPetshop.Application.Avatares;

// Catálogo fijo. Los archivos de imagen reales deben existir en:
// wwwroot/avatares/clientes/{1..6}.png
// wwwroot/avatares/mascotas/{Tipo}/{1..3}.png  (Tipo: Perro, Gato, Ave, Conejo, Hamster, Otro)
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