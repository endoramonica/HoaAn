namespace VietCommerce.Core.DTOs.Rituals
{
    /// <summary>
    /// DTO for action type definitions used in ritual pattern matching
    /// </summary>
    public class ActionTypeDto
    {
        /// <summary>
        /// Type of action (e.g., "ViewProduct", "AddToCart", "BrowseCategory")
        /// </summary>
        public string Type { get; set; } = string.Empty;

        /// <summary>
        /// Optional product category ID associated with the action
        /// </summary>
        public Guid? ProductCategoryId { get; set; }

        /// <summary>
        /// Additional metadata for the action type
        /// </summary>
        public Dictionary<string, object>? Metadata { get; set; }
    }
}
