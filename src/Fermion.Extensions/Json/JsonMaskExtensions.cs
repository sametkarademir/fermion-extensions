using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Unicode;
using System.Text.RegularExpressions;

namespace Fermion.Extensions.Json;

/// <summary>
/// Provides extension methods for masking sensitive data in JSON strings.
/// </summary>
public static class JsonMaskExtensions
{
    private static readonly JsonSerializerOptions JsonOptions = new JsonSerializerOptions
    {
        Encoder = JavaScriptEncoder.Create(UnicodeRanges.All),
        WriteIndented = false
    };

    /// <summary>
    /// Masks sensitive data in a JSON string based on the provided options.
    /// </summary>
    /// <param name="data">The JSON string to mask.</param>
    /// <param name="maskPattern">The pattern used to mask sensitive data. Default is "***MASKED***".</param>
    /// <param name="sensitivePropertyNames">An array of property names that should be masked. Default includes common sensitive properties.</param>
    /// <returns>The masked JSON string.</returns>
    /// <remarks>
    /// This method attempts to parse the input string as JSON. If parsing fails, it falls back to regex-based masking.
    /// </remarks>
    public static string? MaskSensitiveData(string? data, string maskPattern = "***MASKED***", string[]? sensitivePropertyNames = null)
    {
        sensitivePropertyNames ??= ["Password", "Token", "Secret", "ApiKey", "RecoveryKey", "Key", "Credential", "Ssn", "Credit", "Card"];
        if (string.IsNullOrEmpty(data))
        {
            return data;
        }

        try
        {
            var dataBytes = Encoding.UTF8.GetBytes(data);
            var reader = new Utf8JsonReader(dataBytes);

            if (JsonSerializer.Deserialize<JsonElement>(ref reader, JsonOptions) is var jsonElement)
            {
                return MaskJsonProperties(jsonElement, sensitivePropertyNames, maskPattern);
            }
        }
        catch
        {
            foreach (var prop in sensitivePropertyNames)
            {
                // Mask property values
                var propertyPattern = $@"(""{prop}""\s*:\s*"")(.*?)("")";
                data = Regex.Replace(data, propertyPattern, $"$1{maskPattern}$3", RegexOptions.IgnoreCase);

                // Mask connection string like values
                var connectionStringPattern = $@"({prop}=)([^;]+)";
                data = Regex.Replace(data, connectionStringPattern, $"$1{maskPattern}", RegexOptions.IgnoreCase);
            }
        }

        return data;
    }

    private static string MaskJsonProperties(JsonElement element, string[] sensitivePropertyNames, string maskPattern = "***MASKED***")
    {
        var sensitiveProps = new HashSet<string>(sensitivePropertyNames, StringComparer.OrdinalIgnoreCase);

        using var ms = new MemoryStream();
        using var writer = new Utf8JsonWriter(ms, new JsonWriterOptions
        {
            Encoder = JavaScriptEncoder.Create(UnicodeRanges.All),
            Indented = false
        });

        MaskJsonElement(element, writer, sensitiveProps, maskPattern);

        writer.Flush();
        return Encoding.UTF8.GetString(ms.ToArray());
    }

    private static void MaskJsonElement(JsonElement element, Utf8JsonWriter writer, HashSet<string> sensitiveProps, string maskPattern)
    {
        switch (element.ValueKind)
        {
            case JsonValueKind.Object:
                writer.WriteStartObject();
                foreach (var property in element.EnumerateObject())
                {
                    writer.WritePropertyName(property.Name);
                    if (sensitiveProps.Contains(property.Name))
                    {
                        writer.WriteStringValue(maskPattern);
                    }
                    else if (property.Value.ValueKind == JsonValueKind.String)
                    {
                        var value = property.Value.GetString();
                        if (value != null)
                        {
                            // Check if the value is a connection string like structure
                            var maskedValue = value;
                            foreach (var prop in sensitiveProps)
                            {
                                var pattern = $@"({prop}=)([^;]+)";
                                maskedValue = Regex.Replace(maskedValue, pattern, $"$1{maskPattern}", RegexOptions.IgnoreCase);
                            }
                            writer.WriteStringValue(maskedValue);
                        }
                        else
                        {
                            writer.WriteStringValue(value);
                        }
                    }
                    else
                    {
                        MaskJsonElement(property.Value, writer, sensitiveProps, maskPattern);
                    }
                }
                writer.WriteEndObject();
                break;

            case JsonValueKind.Array:
                writer.WriteStartArray();
                foreach (var item in element.EnumerateArray())
                {
                    MaskJsonElement(item, writer, sensitiveProps, maskPattern);
                }
                writer.WriteEndArray();
                break;

            case JsonValueKind.String:
                var stringValue = element.GetString();
                if (stringValue != null)
                {
                    // Check if the value is a connection string like structure
                    var maskedValue = stringValue;
                    foreach (var prop in sensitiveProps)
                    {
                        var pattern = $@"({prop}=)([^;]+)";
                        maskedValue = Regex.Replace(maskedValue, pattern, $"$1{maskPattern}", RegexOptions.IgnoreCase);
                    }
                    writer.WriteStringValue(maskedValue);
                }
                else
                {
                    writer.WriteStringValue(stringValue);
                }
                break;

            case JsonValueKind.Number:
                writer.WriteNumberValue(element.GetDecimal());
                break;

            case JsonValueKind.True:
                writer.WriteBooleanValue(true);
                break;

            case JsonValueKind.False:
                writer.WriteBooleanValue(false);
                break;

            case JsonValueKind.Null:
                writer.WriteNullValue();
                break;
        }
    }
}