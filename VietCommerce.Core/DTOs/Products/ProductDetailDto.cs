using System.ComponentModel.DataAnnotations;

namespace VietCommerce.Core.DTOs.Products
{


    /// Detailed product information (for single product view)

    public class ProductDetailDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? ShortDescription { get; set; }
        public string? Description { get; set; }
        public string Code { get; set; } = string.Empty;
        public string Slug { get; set; } = string.Empty;

        // Pricing
        public decimal Price { get; set; }
        public decimal? CompareAtPrice { get; set; }
        public decimal? Cost { get; set; }

        // Discount percentage (calculated)
        public decimal? DiscountPercentage => CompareAtPrice.HasValue && CompareAtPrice > Price
            ? Math.Round((CompareAtPrice.Value - Price) / CompareAtPrice.Value * 100, 2)
            : null;

        // Inventory
        public int StockQuantity { get; set; }
        public string? Sku { get; set; }
        public string? Barcode { get; set; }

        // Categorization
        public Guid? CategoryId { get; set; }
        public string? CategoryName { get; set; }
        public Guid? BrandId { get; set; }
        public string? BrandName { get; set; }

        // Media
        public List<string> Images { get; set; } = new();
        public string? PrimaryImage => Images.FirstOrDefault();

        // Tags & SEO
        public List<string> Tags { get; set; } = new();
        public string? MetaTitle { get; set; }
        public string? MetaDescription { get; set; }
        public string? MetaKeywords { get; set; }

        // Status
        public bool IsActive { get; set; }
        public bool IsFeatured { get; set; }

        // Statistics (from ProductStats if available)
        public int ViewCount { get; set; }
        public int FavoriteCount { get; set; }
        public decimal AverageRating { get; set; }
        public int ReviewCount { get; set; }

        // Store info
        public Guid StoreId { get; set; }
        public string? StoreName { get; set; }

        // Audit
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public Guid CreatedBy { get; set; }
        public string? CreatedByName { get; set; }
    }
}