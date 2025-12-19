using VietCommerce.Core.DTOs.Rituals;

namespace VietCommerce.Application.Services.Services.Interfaces
{
    /// <summary>
    /// Interface for the Gemini explanation service
    /// Handles natural language explanation generation for recommendations using Gemini API
    /// </summary>
    public interface IGeminiExplanationService
    {
        /// <summary>
        /// Generates a natural language explanation for a recommendation using Gemini API
        /// </summary>
        /// <param name="payload">The recommendation payload from BE-AI</param>
        /// <returns>ExplanationPayloadDto with human-readable explanation, or fallback if API fails</returns>
        Task<ExplanationPayloadDto> GenerateExplanationAsync(RecommendationPayloadDto payload);

        /// <summary>
        /// Gets a fallback template explanation when Gemini API is unavailable
        /// </summary>
        /// <param name="payload">The recommendation payload from BE-AI</param>
        /// <returns>ExplanationPayloadDto with template-based explanation</returns>
        ExplanationPayloadDto GetFallbackExplanation(RecommendationPayloadDto payload);

        /// <summary>
        /// Checks if the Gemini API is properly configured and available
        /// </summary>
        /// <returns>True if API key is configured, false otherwise</returns>
        bool IsConfigured();
    }
}
