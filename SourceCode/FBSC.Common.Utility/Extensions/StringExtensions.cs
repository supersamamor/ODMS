using System.Text.Json;

namespace FBSC.Common.Utility.Extensions;

/// <summary>
/// Extension methods for <see cref="string"/>.
/// </summary>
public static class StringExtensions
{
    private static readonly JsonSerializerOptions s_writeOptions = new()
    {
        WriteIndented = true
    };
    /// <summary>
    /// Returns formatted version of the JSON.
    /// </summary>
    /// <param name="json"></param>
    /// <returns></returns>
    public static string JsonPrettify(this string json)
    {
        try
        {
            var jDoc = JsonDocument.Parse(json);
            return JsonSerializer.Serialize(jDoc, options: s_writeOptions);
        }
        catch (Exception)
        {
            return json;
        }
    }
}