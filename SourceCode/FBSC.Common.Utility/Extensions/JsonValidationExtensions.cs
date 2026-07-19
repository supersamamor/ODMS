using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Text.Json;
namespace FBSC.Common.Utility.Extensions
{
    public static class JsonValidationExtensions
    {
        /// <summary>
        /// Validates if a string is well-formed JSON without deserializing to a specific object.
        /// </summary>
        public static void ValidateJson(this string? jsonInput, ModelStateDictionary modelState)
        {
            if (string.IsNullOrWhiteSpace(jsonInput))
            {
                return;
            }
            try
            {
                // Parses the JSON purely to check syntax. 'using' ensures memory is freed immediately.
                using var document = JsonDocument.Parse(jsonInput);
                return;
            }
            catch (JsonException)
            {
                modelState.AddModelError($"{nameof(jsonInput)}", "Invalid JSON format. Please check for syntax errors.");
                return;
            }
        }
    }
}
