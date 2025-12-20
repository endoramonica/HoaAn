namespace VietCommerce.Core.DTOs.Rituals
{
    /// <summary>
    /// DTO for tracking user actions in the ritual recommendation system
    /// </summary>
    public class ActionDto
    {
        /// <summary>
        /// Unique identifier for the action
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// User ID who performed the action
        /// </summary>
        public Guid UserId { get; set; }

        /// <summary>
        /// Session ID for tracking actions within a session
        /// </summary>
        public string SessionId { get; set; } = string.Empty;

        /// <summary>
        /// Type of action performed
        /// </summary>
        public string Type { get; set; } = string.Empty;

        /// <summary>
        /// Timestamp when the action was performed
        /// </summary>
        public DateTime Timestamp { get; set; }

        /// <summary>
        /// Optional product ID associated with the action
        /// </summary>
        public Guid? ProductId { get; set; }

        /// <summary>
        /// Optional category ID associated with the action
        /// </summary>
        public Guid? CategoryId { get; set; }

        /// <summary>
        /// Additional metadata for the action
        /// </summary>
        public Dictionary<string, object>? Metadata { get; set; }
    }
}
