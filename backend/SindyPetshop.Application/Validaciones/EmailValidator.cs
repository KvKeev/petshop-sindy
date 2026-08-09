using System.Text.RegularExpressions;

namespace SindyPetshop.Application.Validaciones;

public static class EmailValidator
{
    private static readonly Regex Patron = new(@"^[^\s@]+@[^\s@]+\.[^\s@]+$", RegexOptions.Compiled);

    public static bool EsValido(string? email) => !string.IsNullOrWhiteSpace(email) && Patron.IsMatch(email.Trim());
}