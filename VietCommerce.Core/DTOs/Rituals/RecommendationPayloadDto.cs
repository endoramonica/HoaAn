namespace VietCommerce.Core.DTOs.Rituals
{
    /// <summary>
    /// DTO for BE-AI recommendation output containing matched ritual and missing items
    /// </summary>
    public class RecommendationPayloadDto
    {
        /// <summary>
        /// ID of the detected ritual
        /// </summary>
        public string RitualId { get; set; } = string.Empty;

        /// <summary>
        /// Name of the detected ritual
        /// </summary>
        public string RitualName { get; set; } = string.Empty;

        /// <summary>
        /// Confidence score (0-1) for the recommendation
        /// </summary>
        public decimal ConfidenceScore { get; set; }

        /// <summary>
        /// Product IDs that are missing from the user's cart
        /// </summary>
        public List<Guid> MissingItems { get; set; } = new();

        /// <summary>
        /// Metadata about the pattern matching process
        /// </summary>
        public MatchingMetadataDto MatchingMetadata { get; set; } = new();

        /// <summary>
        /// System report with detailed matching logic and reasoning
        /// </summary>
        public SystemReportDto SystemReport { get; set; } = new();
    }

    /// <summary>
    /// DTO for system report containing detailed matching logic
    /// </summary>
    public class SystemReportDto
    {
        /// <summary>
        /// The action sequence pattern that was matched
        /// </summary>
        public List<ActionTypeDto> MatchedPattern { get; set; } = new();

        /// <summary>
        /// Step-by-step explanation of the matching process
        /// </summary>
        public List<string> MatchingSteps { get; set; } = new();

        /// <summary>
        /// Reasons why each item is missing/recommended
        /// </summary>
        public Dictionary<Guid, string> ReasonsForMissingItems { get; set; } = new();
    }
}
