using System.Text.Json;
using System.Text.Json.Serialization;

namespace FBSC.GenAI.Services.AI.Providers
{
    public interface IAiResponseParser
    {
        string ProviderName { get; }
        string ParseResponse(string rawJson);
    }

    // 2. Common Utility for Sanitization (Shared across all AI providers)
    public static class AiContentSanitizer
    {
        public static string Sanitize(string? content)
        {
            if (string.IsNullOrWhiteSpace(content)) return string.Empty;
            return content.Replace("```sql", "")
                          .Replace("```json", "")
                          .Replace("```csharp", "")
                          .Replace("```", "")
                          .Trim();
        }
    }

    // 3. Gemini Implementation
    public class GeminiResponseParser : IAiResponseParser
    {
        public string ProviderName => "Gemini";

        public string ParseResponse(string rawJson)
        {
            var response = JsonSerializer.Deserialize<GeminiResponse>(rawJson);
            var text = response?.Candidates?.SelectMany(c => c.Content?.Parts ?? [])
                                            .FirstOrDefault()?.Text;
            return AiContentSanitizer.Sanitize(text);
        }

        private record GeminiResponse([property: JsonPropertyName("candidates")] List<GeminiCandidate>? Candidates);
        private record GeminiCandidate([property: JsonPropertyName("content")] GeminiContent? Content);
        private record GeminiContent([property: JsonPropertyName("parts")] List<GeminiPart> Parts);
        private record GeminiPart([property: JsonPropertyName("text")] string Text);
    }

    // 4. OpenAI / ChatGPT Implementation
    public class OpenAiResponseParser : IAiResponseParser
    {
        public string ProviderName => "OpenAI";

        public string ParseResponse(string rawJson)
        {
            var response = JsonSerializer.Deserialize<OpenAiResponse>(rawJson);
            var text = response?.Choices?.FirstOrDefault()?.Message?.Content;
            return AiContentSanitizer.Sanitize(text);
        }

        private record OpenAiResponse([property: JsonPropertyName("choices")] List<OpenAiChoice>? Choices);
        private record OpenAiChoice([property: JsonPropertyName("message")] OpenAiMessage? Message);
        private record OpenAiMessage([property: JsonPropertyName("content")] string? Content);
    }

    // 5. Claude / Anthropic Implementation
    public class ClaudeResponseParser : IAiResponseParser
    {
        public string ProviderName => "Claude";

        public string ParseResponse(string rawJson)
        {
            var response = JsonSerializer.Deserialize<ClaudeResponse>(rawJson);
            var text = response?.Content?.FirstOrDefault(c => c.Type == "text")?.Text;
            return AiContentSanitizer.Sanitize(text);
        }

        private record ClaudeResponse([property: JsonPropertyName("content")] List<ClaudeContent>? Content);
        private record ClaudeContent(
            [property: JsonPropertyName("type")] string Type,
            [property: JsonPropertyName("text")] string? Text);
    }

    // 6. Microsoft Copilot / Azure OpenAI Implementation
    public class MicrosoftCopilotResponseParser : IAiResponseParser
    {
        public string ProviderName => "MicrosoftCopilot";

        public string ParseResponse(string rawJson)
        {
            // Microsoft Copilot / Azure OpenAI uses a structure fundamentally identical to OpenAI,
            // but having a dedicated class allows future extraction of Copilot-specific fields (like Citations).
            var response = JsonSerializer.Deserialize<CopilotResponse>(rawJson);
            var text = response?.Choices?.FirstOrDefault()?.Message?.Content;
            return AiContentSanitizer.Sanitize(text);
        }

        private record CopilotResponse([property: JsonPropertyName("choices")] List<CopilotChoice>? Choices);
        private record CopilotChoice([property: JsonPropertyName("message")] CopilotMessage? Message);
        private record CopilotMessage([property: JsonPropertyName("content")] string? Content);
    }

    // 7. The Factory that routes the response to the correct parser
    public class AiResponseParserFactory
    {
        private readonly IEnumerable<IAiResponseParser> _parsers;

        public AiResponseParserFactory()
        {
            // In a real .NET Core app, you inject these via DI (IEnumerable<IAiResponseParser>)
            _parsers =
            [
                new GeminiResponseParser(),
                new OpenAiResponseParser(),
                new ClaudeResponseParser(),
                new MicrosoftCopilotResponseParser()
            ];
        }

        /// <summary>
        /// Parses the AI response by auto-detecting the provider from the JSON structure.
        /// This is highly resilient if you don't pass the provider name explicitly.
        /// </summary>
        public string ParseAutoDetect(string rawJson)
        {
            if (string.IsNullOrWhiteSpace(rawJson)) return string.Empty;

            try
            {
                using var document = JsonDocument.Parse(rawJson);
                var root = document.RootElement;

                if (root.TryGetProperty("candidates", out _))
                {
                    return GetParser("Gemini").ParseResponse(rawJson);
                }
                if (root.TryGetProperty("choices", out _))
                {
                    // Note: Both OpenAI and Microsoft Copilot use the "choices" array.
                    // If auto-detecting, standard OpenAI extraction will successfully parse both.
                    return GetParser("OpenAI").ParseResponse(rawJson);
                }
                if (root.TryGetProperty("content", out _) && root.TryGetProperty("role", out _))
                {
                    return GetParser("Claude").ParseResponse(rawJson);
                }

                throw new InvalidOperationException("Unrecognized AI response format.");
            }
            catch (JsonException ex)
            {
                throw new InvalidOperationException($"Failed to parse AI JSON response: {ex.Message}");
            }
        }

        /// <summary>
        /// Explicit parsing if you extract the Provider name from WebhookApiState.AdditionalConfigurationJson
        /// </summary>
        public string ParseByProvider(string providerName, string rawJson)
        {
            return GetParser(providerName).ParseResponse(rawJson);
        }

        private IAiResponseParser GetParser(string providerName)
        {
            var parser = _parsers.FirstOrDefault(p => p.ProviderName.Equals(providerName, StringComparison.OrdinalIgnoreCase)) ?? throw new NotSupportedException($"AI Provider '{providerName}' is not supported.");
            return parser;
        }
    }
}
