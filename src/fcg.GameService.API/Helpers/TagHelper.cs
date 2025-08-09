using System.Globalization;
using System.Text.RegularExpressions;

namespace fcg.GameService.API.Helpers;

public class TagHelper
{
    private static readonly Regex _spaceRegex = new(@"\s+");

    public static string[] NormalizeTags(string[] tags)
    {
        if (tags == null || tags.Length == 0)
            return Array.Empty<string>();

        return tags
            .Where(tag => !string.IsNullOrWhiteSpace(tag))
            .Select(NormalizeTag)
            .Distinct()
            .ToArray();
    }

    private static string NormalizeTag(string tag)
    {
        var trimmed = tag.Trim();

        var noAccentuation = new string(
            trimmed
                .Normalize(System.Text.NormalizationForm.FormD)
                .Where(ch => char.GetUnicodeCategory(ch) != UnicodeCategory.NonSpacingMark)
                .ToArray()
        );

        string replaced = _spaceRegex.Replace(noAccentuation, "-");
        
        return replaced.ToLower();
    }
}
