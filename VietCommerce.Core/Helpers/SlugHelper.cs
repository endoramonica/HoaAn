using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace VietCommerce.Core.Helpers;

public static class SlugHelper
{
    /// <summary>
    /// Generate SEO-friendly slug from Vietnamese text
    /// Example: "Áo thun nam" → "ao-thun-nam"
    /// </summary>
    public static string GenerateSlug(string text)
    {
        if (string.IsNullOrWhiteSpace(text))
            return string.Empty;

        // Convert to lowercase
        text = text.ToLowerInvariant();

        // Remove Vietnamese diacritics
        text = RemoveVietnameseDiacritics(text);

        // Remove invalid characters
        text = Regex.Replace(text, @"[^a-z0-9\s-]", "");

        // Convert multiple spaces/hyphens to single hyphen
        text = Regex.Replace(text, @"[\s-]+", " ").Trim();

        // Replace spaces with hyphens
        text = Regex.Replace(text, @"\s", "-");

        return text;
    }

    private static string RemoveVietnameseDiacritics(string text)
    {
        var normalized = text.Normalize(NormalizationForm.FormD);
        var stringBuilder = new StringBuilder();

        foreach (var c in normalized)
        {
            var unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(c);
            if (unicodeCategory != UnicodeCategory.NonSpacingMark)
            {
                stringBuilder.Append(c);
            }
        }

        return stringBuilder.ToString().Normalize(NormalizationForm.FormC)
            .Replace("đ", "d").Replace("Đ", "d");
    }
}