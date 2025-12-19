using Microsoft.Extensions.Logging;
using System.Net.Http.Json;
using System.Text.Json;
using VietCommerce.Application.Services.Services.Interfaces;
using VietCommerce.Core.DTOs.Rituals;

namespace VietCommerce.Application.Services.Services
{
    /// <summary>
    /// Service for generating natural language explanations using Google's Gemini API
    /// Provides fallback template explanations when API is unavailable
    /// </summary>
    public class GeminiExplanationService : IGeminiExplanationService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<GeminiExplanationService> _logger;
        private readonly string? _geminiApiKey;
        private const string GeminiApiUrl = "https://generativelanguage.googleapis.com/v1beta/models/gemini-1.5-flash:generateContent";
        private const int MaxRetries = 3;
        private const int RetryDelayMs = 1000;

        public GeminiExplanationService(
            HttpClient httpClient,
            ILogger<GeminiExplanationService> logger)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _geminiApiKey = Environment.GetEnvironmentVariable("VITE_GEMINI_API_KEY");
        }

        /// <summary>
        /// Generates a natural language explanation for a recommendation using Gemini API
        /// Falls back to template explanation if API fails
        /// </summary>
        /// <param name="payload">The recommendation payload from BE-AI</param>
        /// <returns>ExplanationPayloadDto with human-readable explanation</returns>
        public async Task<ExplanationPayloadDto> GenerateExplanationAsync(RecommendationPayloadDto payload)
        {
            try
            {
                // Validate input
                if (payload == null)
                {
                    _logger.LogWarning("Null payload provided to GenerateExplanationAsync");
                    return GetFallbackExplanation(payload!);
                }

                // Check if API is configured
                if (!IsConfigured())
                {
                    _logger.LogInformation("Gemini API not configured, using fallback explanation");
                    return GetFallbackExplanation(payload);
                }

                // Attempt to call Gemini API with retry logic
                for (int attempt = 0; attempt < MaxRetries; attempt++)
                {
                    try
                    {
                        var explanation = await CallGeminiApiAsync(payload);
                        if (explanation != null)
                        {
                            _logger.LogInformation($"Successfully generated explanation for ritual {payload.RitualName}");
                            return explanation;
                        }
                    }
                    catch (HttpRequestException ex) when (attempt < MaxRetries - 1)
                    {
                        _logger.LogWarning($"Gemini API call failed (attempt {attempt + 1}/{MaxRetries}): {ex.Message}");
                        await Task.Delay(RetryDelayMs * (attempt + 1)); // Exponential backoff
                    }
                    catch (Exception ex) when (attempt < MaxRetries - 1)
                    {
                        _logger.LogWarning($"Error calling Gemini API (attempt {attempt + 1}/{MaxRetries}): {ex.Message}");
                        await Task.Delay(RetryDelayMs * (attempt + 1));
                    }
                }

                // All retries failed, use fallback
                _logger.LogWarning($"All Gemini API retries failed for ritual {payload.RitualName}, using fallback");
                return GetFallbackExplanation(payload);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error in GenerateExplanationAsync");
                return GetFallbackExplanation(payload);
            }
        }

        /// <summary>
        /// Gets a fallback template explanation when Gemini API is unavailable
        /// </summary>
        /// <param name="payload">The recommendation payload from BE-AI</param>
        /// <returns>ExplanationPayloadDto with template-based explanation</returns>
        public ExplanationPayloadDto GetFallbackExplanation(RecommendationPayloadDto payload)
        {
            try
            {
                if (payload == null)
                {
                    return new ExplanationPayloadDto
                    {
                        RitualName = "Unknown Ritual",
                        CulturalContext = "Unable to generate explanation at this time.",
                        ItemExplanations = new List<ItemExplanationDto>(),
                        Sources = new List<string>(),
                        GeneratedBy = "fallback"
                    };
                }

                var itemExplanations = new List<ItemExplanationDto>();

                // Generate template explanations for each missing item
                foreach (var missingItemId in payload.MissingItems)
                {
                    var reason = payload.SystemReport?.ReasonsForMissingItems
                        .FirstOrDefault(r => r.Key == missingItemId).Value ?? "Required for this ritual";

                    itemExplanations.Add(new ItemExplanationDto
                    {
                        ProductId = missingItemId,
                        ProductName = $"Item {missingItemId:N}",
                        WhyNeeded = reason,
                        TraditionalUsage = $"This item is traditionally used in the {payload.RitualName} ritual."
                    });
                }

                var explanation = new ExplanationPayloadDto
                {
                    RitualName = payload.RitualName,
                    CulturalContext = $"The {payload.RitualName} is an important Vietnamese cultural ritual. " +
                                     $"Based on your shopping behavior, we detected that you may be preparing for this ritual. " +
                                     $"The following items are traditionally required for this ceremony.",
                    ItemExplanations = itemExplanations,
                    Sources = new List<string>
                    {
                        "Vietnamese Cultural Traditions",
                        "Traditional Ritual Practices"
                    },
                    GeneratedBy = "fallback"
                };

                _logger.LogInformation($"Generated fallback explanation for ritual {payload.RitualName}");
                return explanation;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating fallback explanation");
                return new ExplanationPayloadDto
                {
                    RitualName = payload?.RitualName ?? "Unknown",
                    CulturalContext = "We recommend these items based on your shopping behavior.",
                    ItemExplanations = new List<ItemExplanationDto>(),
                    Sources = new List<string>(),
                    GeneratedBy = "fallback"
                };
            }
        }

        /// <summary>
        /// Checks if the Gemini API is properly configured
        /// </summary>
        /// <returns>True if API key is configured, false otherwise</returns>
        public bool IsConfigured()
        {
            return !string.IsNullOrWhiteSpace(_geminiApiKey);
        }

        /// <summary>
        /// Calls the Gemini API to generate an explanation
        /// </summary>
        /// <param name="payload">The recommendation payload</param>
        /// <returns>ExplanationPayloadDto with Gemini-generated explanation, or null if failed</returns>
        private async Task<ExplanationPayloadDto?> CallGeminiApiAsync(RecommendationPayloadDto payload)
        {
            try
            {
                // Build the prompt for Gemini
                var prompt = BuildGeminiPrompt(payload);

                // Create the request body
                var requestBody = new
                {
                    contents = new[]
                    {
                        new
                        {
                            parts = new[]
                            {
                                new { text = prompt }
                            }
                        }
                    },
                    generationConfig = new
                    {
                        temperature = 0.7,
                        topK = 40,
                        topP = 0.95,
                        maxOutputTokens = 1024
                    }
                };

                // Make the API call
                var url = $"{GeminiApiUrl}?key={_geminiApiKey}";
                var response = await _httpClient.PostAsJsonAsync(url, requestBody);

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogWarning($"Gemini API returned status code {response.StatusCode}");
                    return null;
                }

                // Parse the response
                var responseContent = await response.Content.ReadAsStringAsync();
                var jsonDoc = JsonDocument.Parse(responseContent);
                var root = jsonDoc.RootElement;

                // Extract the generated text
                if (root.TryGetProperty("candidates", out var candidates) &&
                    candidates.GetArrayLength() > 0 &&
                    candidates[0].TryGetProperty("content", out var content) &&
                    content.TryGetProperty("parts", out var parts) &&
                    parts.GetArrayLength() > 0 &&
                    parts[0].TryGetProperty("text", out var text))
                {
                    var generatedText = text.GetString();
                    return ParseGeminiResponse(payload, generatedText);
                }

                _logger.LogWarning("Unexpected Gemini API response format");
                return null;
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "HTTP error calling Gemini API");
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error calling Gemini API");
                throw;
            }
        }

        /// <summary>
        /// Builds the prompt to send to Gemini API
        /// </summary>
        /// <param name="payload">The recommendation payload</param>
        /// <returns>The formatted prompt string</returns>
        private string BuildGeminiPrompt(RecommendationPayloadDto payload)
        {
            var missingItemsText = string.Join(", ", payload.MissingItems.Select(id => $"Product {id:N}"));

            var prompt = $@"You are a cultural advisor for Vietnamese traditions and rituals. 
A customer is preparing for the '{payload.RitualName}' ritual with a confidence score of {payload.ConfidenceScore:P}.

The following items are missing from their cart and are traditionally required for this ritual:
{missingItemsText}

Please provide:
1. A brief explanation of the cultural significance of the {payload.RitualName} ritual
2. For each missing item, explain why it is traditionally required for this ritual
3. Any relevant cultural context or traditional practices

Format your response as JSON with the following structure:
{{
  ""culturalContext"": ""explanation of the ritual"",
  ""itemExplanations"": [
    {{
      ""productId"": ""product-id"",
      ""whyNeeded"": ""why this item is needed"",
      ""traditionalUsage"": ""how it's traditionally used""
    }}
  ],
  ""sources"": [""source1"", ""source2""]
}}

Respond ONLY with valid JSON, no additional text.";

            return prompt;
        }

        /// <summary>
        /// Parses the Gemini API response and creates an ExplanationPayloadDto
        /// </summary>
        /// <param name="payload">The original recommendation payload</param>
        /// <param name="geminiResponse">The text response from Gemini</param>
        /// <returns>ExplanationPayloadDto with parsed explanation</returns>
        private ExplanationPayloadDto ParseGeminiResponse(RecommendationPayloadDto payload, string? geminiResponse)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(geminiResponse))
                {
                    return GetFallbackExplanation(payload);
                }

                // Try to extract JSON from the response
                var jsonStart = geminiResponse.IndexOf('{');
                var jsonEnd = geminiResponse.LastIndexOf('}');

                if (jsonStart < 0 || jsonEnd < 0 || jsonStart >= jsonEnd)
                {
                    _logger.LogWarning("Could not find JSON in Gemini response");
                    return GetFallbackExplanation(payload);
                }

                var jsonString = geminiResponse.Substring(jsonStart, jsonEnd - jsonStart + 1);
                var jsonDoc = JsonDocument.Parse(jsonString);
                var root = jsonDoc.RootElement;

                // Parse cultural context
                var culturalContext = root.TryGetProperty("culturalContext", out var cc)
                    ? cc.GetString() ?? ""
                    : "";

                // Parse item explanations
                var itemExplanations = new List<ItemExplanationDto>();
                if (root.TryGetProperty("itemExplanations", out var itemsArray))
                {
                    foreach (var item in itemsArray.EnumerateArray())
                    {
                        var productIdStr = item.TryGetProperty("productId", out var pid)
                            ? pid.GetString() ?? ""
                            : "";

                        if (Guid.TryParse(productIdStr, out var productId) || 
                            payload.MissingItems.Contains(productId))
                        {
                            itemExplanations.Add(new ItemExplanationDto
                            {
                                ProductId = productId,
                                ProductName = $"Item {productId:N}",
                                WhyNeeded = item.TryGetProperty("whyNeeded", out var wn)
                                    ? wn.GetString() ?? ""
                                    : "",
                                TraditionalUsage = item.TryGetProperty("traditionalUsage", out var tu)
                                    ? tu.GetString() ?? ""
                                    : ""
                            });
                        }
                    }
                }

                // Parse sources
                var sources = new List<string>();
                if (root.TryGetProperty("sources", out var sourcesArray))
                {
                    foreach (var source in sourcesArray.EnumerateArray())
                    {
                        if (source.ValueKind == JsonValueKind.String)
                        {
                            sources.Add(source.GetString() ?? "");
                        }
                    }
                }

                var explanation = new ExplanationPayloadDto
                {
                    RitualName = payload.RitualName,
                    CulturalContext = culturalContext,
                    ItemExplanations = itemExplanations,
                    Sources = sources,
                    GeneratedBy = "gemini"
                };

                _logger.LogInformation($"Successfully parsed Gemini response for ritual {payload.RitualName}");
                return explanation;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error parsing Gemini response");
                return GetFallbackExplanation(payload);
            }
        }
    }
}
