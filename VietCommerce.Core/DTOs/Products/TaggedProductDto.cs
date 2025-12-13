namespace VietCommerce.Core.DTOs.Products
{
    /// <summary>
    /// Lightweight product information for marketing posts.
    /// Contains live product data (price, discount, image) that is fetched at retrieval time.
    /// Used to display current product details alongside marketing content.
    /// </summary>
    public class TaggedProductDto
    {
        /// <summary>
        /// Product unique identifier
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Product name
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Current product price
        /// </summary>
        public decimal Price { get; set; }

        /// <summary>
        /// Currency code (default: VND)
        /// </summary>
        public string Currency { get; set; } = "VND";

        /// <summary>
        /// Formatted price string for display (e.g., "45,000 VND")
        /// </summary>
        public string FormattedPrice { get; set; } = string.Empty;

        /// <summary>
        /// Product thumbnail image URL
        /// </summary>
        public string ThumbnailUrl { get; set; } = string.Empty;

        /// <summary>
        /// Indicates if product has an active discount
        /// </summary>
        public bool HasDiscount { get; set; }

        /// <summary>
        /// Discount percentage (0-100)
        /// </summary>
        public int DiscountPercentage { get; set; }
    }
}
