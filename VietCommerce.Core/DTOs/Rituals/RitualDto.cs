namespace VietCommerce.Core.DTOs.Rituals
{
    /// <summary>
    /// DTO for ritual pattern structure used in the recommendation system
    /// </summary>
    public class RitualDto
    {
        /// <summary>
        /// Unique identifier for the ritual
        /// </summary>
        public string Id { get; set; } = string.Empty;

        /// <summary>
        /// Name of the ritual (e.g., "Đầy Tháng", "Tết", "Lễ Cúng Tổ Tiên")
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Sequence of action types that indicate this ritual
        /// </summary>
        public List<ActionTypeDto> ActionSequencePattern { get; set; } = new();

        /// <summary>
        /// Required items for the ritual, organized by category
        /// </summary>
        public List<RitualRequiredItemDto> RequiredItems { get; set; } = new();

        /// <summary>
        /// Confidence threshold (0-1) for pattern matching
        /// </summary>
        public decimal ConfidenceThreshold { get; set; }

        /// <summary>
        /// Cultural significance and description of the ritual
        /// </summary>
        public string CulturalSignificance { get; set; } = string.Empty;

        /// <summary>
        /// Sources or references for the ritual information
        /// </summary>
        public List<string> Sources { get; set; } = new();
    }

    /// <summary>
    /// DTO for required items in a ritual
    /// </summary>
    public class RitualRequiredItemDto
    {
        /// <summary>
        /// Product category ID for the required item
        /// </summary>
        public Guid CategoryId { get; set; }

        /// <summary>
        /// Specific product IDs that fulfill this requirement
        /// </summary>
        public List<Guid> ProductIds { get; set; } = new();
    }
}
