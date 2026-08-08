using System.Text.RegularExpressions;

namespace SindyPetshop.Application.Validaciones;

// Regla de negocio: nombres de cliente y de mascota solo aceptan letras (con acentos y ñ),
// números y espacios. Sin símbolos ni caracteres especiales.
public static class NombreValidator
{
    private static readonly Regex Patron = new(@"^[A-Za-zÁÉÍÓÚáéíóúÑñÜü0-9 ]+$", RegexOptions.Compiled);

    public static bool EsValido(string? nombre)
    {
        if (string.IsNullOrWhiteSpace(nombre)) return false;
        return Patron.IsMatch(nombre.Trim());
    }
}