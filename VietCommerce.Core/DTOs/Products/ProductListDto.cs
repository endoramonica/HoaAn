namespace VietCommerce.Core.DTOs.Products
{
    /// Lightweight product info (for list/grid view)
    public class ProductListDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? ShortDescription { get; set; }
        public string Code { get; set; } = string.Empty;
        public string Slug { get; set; } = string.Empty;

        // Pricing
        public decimal Price { get; set; }
        public decimal? CompareAtPrice { get; set; }

        // Hiển thị giá đã áp khuyến mãi
        public DisplayPriceResult? DisplayPrice { get; set; }

        // Discount percentage
        public decimal? DiscountPercentage => CompareAtPrice.HasValue && CompareAtPrice > Price
            ? Math.Round((CompareAtPrice.Value - Price) / CompareAtPrice.Value * 100, 2)
            : null;

        // Inventory
        public int StockQuantity { get; set; }
        public bool InStock => StockQuantity > 0;

        // Media
        public string? PrimaryImage { get; set; }
        public List<ProductImageDto> Images { get; set; } = new();

        // Status
        public bool IsActive { get; set; }
        public bool IsFeatured { get; set; }

        // Category
        public Guid? CategoryId { get; set; }
        public string? CategoryName { get; set; }

        // Statistics
        public int ViewCount { get; set; }
        public int FavoriteCount { get; set; }
        public decimal AverageRating { get; set; }

        // Audit
        public DateTime CreatedAt { get; set; }

        // ========================================
        // 🔧 SERVICES-PRODUCT UNIFICATION - NEW FIELDS
        // ========================================

        /// <summary>
        /// Type discriminator: "product" or "service"
        /// </summary>
        public string Type { get; set; } = "product";

        /// <summary>
        /// Service category (only for type='service')
        /// </summary>
        public string? ServiceCategory { get; set; }

        /// <summary>
        /// Service duration (only for type='service')
        /// </summary>
        public string? ServiceDuration { get; set; }

        /// <summary>
        /// Service rating (0-5 stars)
        /// </summary>
        public decimal? ServiceRating { get; set; }
    }
}