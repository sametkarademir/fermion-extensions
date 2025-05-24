using System.Globalization;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace Fermion.Extensions.Objects;

/// <summary>
/// Provides extension methods for string objects.
/// </summary>
public static class StringExtensions
{
    #region Helpers

    /// <summary>
    /// Replaces Turkish characters with their corresponding Latin characters.
    /// </summary>
    /// <param name="text">The input string to replace characters in.</param>
    /// <returns>The input string with Turkish characters replaced.</returns>
    public static string Replace(string text)
    {
        text = text.Replace("İ", "I");
        text = text.Replace("ı", "i");
        text = text.Replace("Ğ", "G");
        text = text.Replace("ğ", "g");
        text = text.Replace("Ö", "O");
        text = text.Replace("ö", "o");
        text = text.Replace("Ü", "U");
        text = text.Replace("ü", "u");
        text = text.Replace("Ş", "S");
        text = text.Replace("ş", "s");
        text = text.Replace("Ç", "C");
        text = text.Replace("ç", "c");
        text = text.Replace(" ", "_");
        return text;
    }

    /// <summary>
    /// Generates a base64 encoded random ID.
    /// </summary>
    /// <returns>A base64 encoded random ID.</returns>
    public static string GenerateBase64RandomId()
    {
        const string chars = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz";
        var base64 = string.Empty;
        var id = Guid.NewGuid().ToString("N");
        var parts = id.SplitInParts(4);

        foreach (var part in parts)
        {
            var base10 = Convert.ToInt32("0x" + part, 16);
            var val = base10 % 64;
            if (chars.Length > val)
            {
                base64 += chars[val];
            }
            else
            {
                base64 += chars[chars.Length - 1];
            }
        }

        return base64;
    }

    #endregion

    #region Extensions

    /// <summary>
    /// Converts a string to a slug.
    /// </summary>
    /// <param name="input">The input string to convert.</param>
    /// <returns>The slug.</returns>
    public static string ToSlug(this string input)
    {
        if (string.IsNullOrWhiteSpace(input))
        {
            return string.Empty;
        }

        input = Replace(input);
        input = input.Normalize(NormalizationForm.FormD);

        var chars = input.Where(c => CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark).ToArray();
        input = new string(chars).Normalize(NormalizationForm.FormC);

        input = Regex.Replace(input, @"[^A-Za-z0-9\s-]", " ");
        input = Regex.Replace(input, @"\s+", " ").Trim();
        input = Regex.Replace(input, @"\s", "-");
        input = Regex.Replace(input, @"-+", "-").Trim('-');

        return input.ToLower();
    }

    /// <summary>
    /// Truncates a string to a specified maximum length.
    /// </summary>
    /// <param name="input">The input string to truncate.</param>
    /// <param name="maxLength">The maximum length of the truncated string.</param>
    /// <param name="suffix">The suffix to append to the truncated string. Defaults to "...".</param>
    /// <returns>The truncated string.</returns>
    public static string Truncate(this string input, int maxLength, string suffix = "...")
    {
        if (string.IsNullOrEmpty(input) || input.Length <= maxLength)
        {
            return input;
        }

        return input.Substring(0, maxLength) + suffix;
    }

    /// <summary>
    /// Removes all HTML tags from the input string and replaces them with the specified string.
    /// </summary>
    /// <param name="input">The input string from which HTML tags will be removed.</param>
    /// <param name="replaceWith">The string to replace HTML tags with. Defaults to an empty string.</param>
    /// <returns>A string with HTML tags removed and replaced by the specified string.</returns>
    /// <remarks>
    /// Note: The trimming behavior has been removed in this overload.
    /// </remarks>
    public static string RemoveHtmlTags(this string input, string replaceWith = "")
    {
        if (string.IsNullOrEmpty(input))
        {
            return string.Empty;
        }

        return Regex.Replace(input, @"<[^>]+>|&nbsp;", replaceWith);
    }

    /// <summary>
    /// Checks if a string is a valid email address.
    /// </summary>
    /// <param name="email">The email address to validate.</param>
    /// <returns>True if the email address is valid, false otherwise.</returns>
    public static bool IsValidEmail(this string email)
    {
        if (string.IsNullOrWhiteSpace(email))
        {
            return false;
        }

        try
        {
            var regex = new Regex(@"^[a-zA-Z0-9.!#$%&'*+/=?^_`{|}~-]+@[a-zA-Z0-9](?:[a-zA-Z0-9-]{0,61}[a-zA-Z0-9])?(?:\.[a-zA-Z0-9](?:[a-zA-Z0-9-]{0,61}[a-zA-Z0-9])?)*$");
            return regex.IsMatch(email);
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    /// Checks if a string is a valid URL.
    /// </summary>
    /// <param name="url">The URL to validate.</param>
    /// <returns>True if the URL is valid, false otherwise.</returns>
    public static bool IsValidUrl(this string url)
    {
        if (string.IsNullOrWhiteSpace(url))
        {
            return false;
        }

        return Uri.TryCreate(url, UriKind.Absolute, out var uriResult)
               && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);
    }

    /// <summary>
    /// Converts a string to a base64 encoded string.
    /// </summary>
    /// <param name="input">The input string to convert.</param>
    /// <returns>The base64 encoded string.</returns>
    public static string ToBase64(this string input)
    {
        if (string.IsNullOrEmpty(input))
        {
            return string.Empty;
        }

        var bytes = Encoding.UTF8.GetBytes(input);
        return Convert.ToBase64String(bytes);
    }

    /// <summary>
    /// Converts a base64 encoded string to a string.
    /// </summary>
    /// <param name="base64String">The base64 encoded string to convert.</param>
    /// <returns>The decoded string.</returns>
    public static string FromBase64(this string base64String)
    {
        if (string.IsNullOrEmpty(base64String))
        {
            return string.Empty;
        }

        try
        {
            var bytes = Convert.FromBase64String(base64String);
            return Encoding.UTF8.GetString(bytes);
        }
        catch
        {
            return string.Empty;
        }
    }

    /// <summary>
    /// Converts a string to an MD5 hash.
    /// </summary>
    /// <param name="input">The input string to convert.</param>
    /// <returns>The MD5 hash.</returns>
    public static string ToMd5Hash(this string input)
    {
        if (string.IsNullOrEmpty(input))
        {
            return string.Empty;
        }

        using var md5 = MD5.Create();
        var inputBytes = Encoding.UTF8.GetBytes(input);
        var hashBytes = md5.ComputeHash(inputBytes);

        var sb = new StringBuilder();
        foreach (var hashByte in hashBytes)
        {
            sb.Append(hashByte.ToString("x2"));
        }

        return sb.ToString();
    }

    /// <summary>
    /// Converts a string to a SHA256 hash.
    /// </summary>
    /// <param name="input">The input string to convert.</param>
    /// <returns>The SHA256 hash.</returns>
    public static string ToSha256Hash(this string input)
    {
        if (string.IsNullOrEmpty(input))
        {
            return string.Empty;
        }

        using var sha256 = SHA256.Create();
        var inputBytes = Encoding.UTF8.GetBytes(input);
        var hashBytes = sha256.ComputeHash(inputBytes);

        var sb = new StringBuilder();
        foreach (var hashByte in hashBytes)
        {
            sb.Append(hashByte.ToString("x2"));
        }

        return sb.ToString();
    }

    /// <summary>
    /// Checks if a string contains a substring, ignoring case.
    /// </summary>
    /// <param name="source">The source string to search within.</param>
    /// <param name="value">The substring to search for.</param>
    /// <returns>True if the substring is found within the source string, false otherwise.</returns>
    public static bool ContainsIgnoreCase(this string source, string value)
    {
        if (string.IsNullOrEmpty(source) || string.IsNullOrEmpty(value))
        {
            return false;
        }

        return source.IndexOf(value, StringComparison.OrdinalIgnoreCase) >= 0;
    }

    /// <summary>
    /// Converts a string to title case.
    /// </summary>
    /// <param name="input">The input string to convert.</param>
    /// <param name="culture">The culture to use for the conversion. Defaults to "en-US".</param>
    /// <returns>The title case string.</returns>
    public static string ToTitleCase(this string input, string culture = "en-US")
    {
        if (string.IsNullOrEmpty(input))
        {
            return string.Empty;
        }

        var textInfo = new CultureInfo(culture, false).TextInfo;
        return textInfo.ToTitleCase(input.ToLower());
    }

    /// <summary>
    /// Strips non-alphanumeric characters from a string.
    /// </summary>
    /// <param name="input">The input string to strip characters from.</param>
    /// <param name="allowSpace">Whether to allow spaces in the output string.</param>
    /// <returns>The string with non-alphanumeric characters removed.</returns>
    public static string StripNonAlphanumeric(this string input, bool allowSpace = false)
    {
        if (string.IsNullOrEmpty(input))
        {
            return string.Empty;
        }

        var pattern = allowSpace ? @"[^a-zA-Z0-9\s]" : @"[^a-zA-Z0-9]";
        return Regex.Replace(input, pattern, "");
    }

    /// <summary>
    /// Splits a string into parts of a specified length.
    /// </summary>
    /// <param name="s">The string to split.</param>
    /// <param name="partLength">The length of each part.</param>
    /// <returns>An enumerable collection of string parts.</returns>
    public static IEnumerable<string> SplitInParts(this string s, int partLength)
    {
        if (s == null)
        {
            throw new AggregateException("null string");
        }

        if (partLength <= 0)
        {
            throw new ArgumentException("Part length has to be positive");
        }

        for (var i = 0; i < s.Length; i += partLength)
        {
            yield return s.Substring(i, Math.Min(partLength, s.Length - i));
        }
    }

    #endregion
}