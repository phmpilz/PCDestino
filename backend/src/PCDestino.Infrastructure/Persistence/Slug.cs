using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace PCDestino.Infrastructure.Persistence;

internal static partial class Slug
{
    public static string From(string value)
    {
        var decomposed = value.Trim().ToLowerInvariant().Normalize(NormalizationForm.FormD);
        var builder = new StringBuilder(decomposed.Length);
        foreach (var character in decomposed)
        {
            if (CharUnicodeInfo.GetUnicodeCategory(character) != UnicodeCategory.NonSpacingMark)
            {
                builder.Append(character);
            }
        }

        var normalized = InvalidCharacters().Replace(builder.ToString().Normalize(NormalizationForm.FormC), "-");
        return MultipleDashes().Replace(normalized, "-").Trim('-');
    }

    [GeneratedRegex("[^a-z0-9]+", RegexOptions.CultureInvariant)]
    private static partial Regex InvalidCharacters();

    [GeneratedRegex("-+", RegexOptions.CultureInvariant)]
    private static partial Regex MultipleDashes();
}
