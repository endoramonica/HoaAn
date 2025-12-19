namespace VietCommerce.Core.DTOs.Rituals
{
    /// <summary>
    /// DTO for pattern matching results from the sequential pattern matcher
    /// </summary>
    public class MatchResultDto
    {
        /// <summary>
        /// Indicates whether a pattern was matched
        /// </summary>
        public bool Matched { get; set; }

        /// <summary>
        /// ID of the matched ritual
        /// </summary>
        public string RitualId { get; set; } = string.Empty;

        /// <summary>
        /// Name of the matched ritual
        /// </summary>
        public string RitualName { get; set; } = string.Empty;

        /// <summary>
        /// Confidence score (0-1) indicating probability of match
        /// </summary>
        public decimal ConfidenceScore { get; set; }

        /// <summary>
        /// Actions from the sequence that matched the pattern
        /// </summary>
        public List<ActionDto> MatchedActions { get; set; } = new();

        /// <summary>
        /// Products that are missing from the user's cart for this ritual
        /// </summary>
        public List<Guid> MissingItemIds { get; set; } = new();

        /// <summary>
        /// Metadata about the matching process
        /// </summary>
        public MatchingMetadataDto MatchingMetadata { get; set; } = new();
    }

    /// <summary>
    /// DTO for metadata about the pattern matching process
    /// </summary>
    public class MatchingMetadataDto
    {
        /// <summary>
        /// Number of actions that matched the pattern
        /// </summary>
        public int MatchedSequenceLength { get; set; }

        /// <summary>
        /// Total length of the action sequence analyzed
        /// </summary>
        public int TotalSequenceLength { get; set; }

        /// <summary>
        /// Indices of matched actions in the original sequence
        /// </summary>
        public List<int> MatchedActionIndices { get; set; } = new();
    }
}
