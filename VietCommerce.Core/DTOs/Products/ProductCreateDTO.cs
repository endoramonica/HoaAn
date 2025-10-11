// ============================================
// FILE: ProductDto.cs
// Mô tả: DTO cơ bản cho danh sách sản phẩm
// ============================================
using System.ComponentModel.DataAnnotations;

namespace VietCommerce.Core.DTOs.Products
{
    public class ProductDto
    {
        public Guid Id { get; set; }
        
        public Guid StoreId { get; set; }
        
        public string StoreName { get; set; } = string.Empty;
        
        [Required]
        [StringLength(200)]
        public string Name { get; set; } = string.Empty;
        
        [StringLength(50)]
        public string Code { get; set; } = string.Empty;
        
        [StringLength(150)]
        public string Slug { get; set; } = string.Empty;
        
        [StringLength(50)]
        public string SKU { get; set; } = string.Empty;
        
        public Guid? CategoryId { get; set; }
        
        public string? CategoryName { get; set; }
        
        [Range(0, int.MaxValue)]
        public int Stock { get; set; }
        
        [Range(0, double.MaxValue)]
        public decimal? CurrentPrice { get; set; }
        
        public bool IsActive { get; set; }
        
        public string? ThumbnailUrl { get; set; }
        
        public DateTime CreatedAt { get; set; }
        
        public DateTime? UpdatedAt { get; set; }
    }
}

// ============================================
// FILE: ProductDetailDto.cs
// Mô tả: DTO chi tiết cho một sản phẩm cụ thể
// ============================================
namespace VietCommerce.Core.DTOs.Products
{
    public class ProductDetailDto
    {
        public Guid Id { get; set; }
        
        public Guid StoreId { get; set; }
        
        public string StoreName { get; set; } = string.Empty;
        
        public string Name { get; set; } = string.Empty;
        
        public string Code { get; set; } = string.Empty;
        
        public string Slug { get; set; } = string.Empty;
        
        public string SKU { get; set; } = string.Empty;
        
        public Guid? CategoryId { get; set; }
        
        public string? CategoryName { get; set; }
        
        public int Stock { get; set; }
        
        public bool IsActive { get; set; }
        
        public List<ProductImageDto> Images { get; set; } = new();
        
        public List<ProductPriceDto> Prices { get; set; } = new();
        
        public decimal? CurrentPrice { get; set; }
        
        public List<ProductInventoryDto> Inventories { get; set; } = new();
        
        public List<SupplierDto> Suppliers { get; set; } = new();
        
        public ProductStatisticsDto? Statistics { get; set; }
        
        public DateTime CreatedAt { get; set; }
        
        public DateTime? UpdatedAt { get; set; }
        
        public Guid? CreatedBy { get; set; }
        
        public Guid? UpdatedBy { get; set; }
    }
}

// ============================================
// FILE: ProductCreateDto.cs
// Mô tả: DTO để tạo sản phẩm mới
// ============================================
namespace VietCommerce.Core.DTOs.Products
{
    public class ProductCreateDto
    {
        [Required(ErrorMessage = "Store ID is required")]
        public Guid StoreId { get; set; }
        
        [Required(ErrorMessage = "Product name is required")]
        [StringLength(200, MinimumLength = 3, ErrorMessage = "Name must be between 3 and 200 characters")]
        public string Name { get; set; } = string.Empty;
        
        [StringLength(50, ErrorMessage = "SKU cannot exceed 50 characters")]
        public string? SKU { get; set; }
        
        public Guid? CategoryId { get; set; }
        
        [Required(ErrorMessage = "Price is required")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than 0")]
        public decimal Price { get; set; }
        
        [Range(0, int.MaxValue, ErrorMessage = "Stock cannot be negative")]
        public int Stock { get; set; } = 0;
        
        public bool IsActive { get; set; } = true;
        
        public List<string>? ImageUrls { get; set; }
        
        public List<Guid>? SupplierIds { get; set; }
    }
}

// ============================================
// FILE: ProductUpdateDto.cs
// Mô tả: DTO để cập nhật sản phẩm
// ============================================
namespace VietCommerce.Core.DTOs.Products
{
    public class ProductUpdateDto
    {
        [StringLength(200, MinimumLength = 3, ErrorMessage = "Name must be between 3 and 200 characters")]
        public string? Name { get; set; }
        
        [StringLength(50, ErrorMessage = "SKU cannot exceed 50 characters")]
        public string? SKU { get; set; }
        
        public Guid? CategoryId { get; set; }
        
        [Range(0, int.MaxValue, ErrorMessage = "Stock cannot be negative")]
        public int? Stock { get; set; }
        
        public bool? IsActive { get; set; }
    }
}

// ============================================
// FILE: ProductFilterDto.cs
// Mô tả: DTO cho filtering và pagination
// ============================================
namespace VietCommerce.Core.DTOs.Products
{
    public class ProductFilterDto
    {
        [Range(1, int.MaxValue, ErrorMessage = "Page number must be at least 1")]
        public int PageNumber { get; set; } = 1;

        [Range(1, 100, ErrorMessage = "Page size must be between 1 and 100")]
        public int PageSize { get; set; } = 10;

        [StringLength(200, ErrorMessage = "Search term cannot exceed 200 characters")]
        public string? SearchTerm { get; set; }
        
        public Guid? CategoryId { get; set; }
        
        public Guid? StoreId { get; set; }
        
        public bool? IsActive { get; set; }
        
        [Range(0, double.MaxValue, ErrorMessage = "Min price must be non-negative")]
        public decimal? MinPrice { get; set; }
        
        [Range(0, double.MaxValue, ErrorMessage = "Max price must be non-negative")]
        public decimal? MaxPrice { get; set; }
        
        [Range(0, int.MaxValue, ErrorMessage = "Min stock must be non-negative")]
        public int? MinStock { get; set; }
        
        [Range(0, int.MaxValue, ErrorMessage = "Max stock must be non-negative")]
        public int? MaxStock { get; set; }
        
        /// <summary>
        /// Sort by: name, price, stock, createdat, updatedat
        /// </summary>
        [StringLength(50)]
        public string? SortBy { get; set; }
        
        public bool IsDescending { get; set; } = false;
        
        public bool? HasPromotion { get; set; }
        
        public bool? InStock { get; set; }
    }
}

// ============================================
// FILE: ProductImageDto.cs
// Mô tả: DTO cho hình ảnh sản phẩm
// ============================================
namespace VietCommerce.Core.DTOs.Products
{
    public class ProductImageDto
    {
        public Guid Id { get; set; }
        
        public Guid ProductId { get; set; }
        
        [Required]
        [Url(ErrorMessage = "Invalid URL format")]
        [StringLength(500)]
        public string Url { get; set; } = string.Empty;
        
        public int DisplayOrder { get; set; }
        
        public bool IsPrimary { get; set; }
        
        public DateTime CreatedAt { get; set; }
    }
}

// ============================================
// FILE: ProductImageCreateDto.cs
// Mô tả: DTO để thêm hình ảnh cho sản phẩm
// ============================================
namespace VietCommerce.Core.DTOs.Products
{
    public class ProductImageCreateDto
    {
        [Required(ErrorMessage = "Product ID is required")]
        public Guid ProductId { get; set; }
        
        [Required(ErrorMessage = "Image URL is required")]
        [Url(ErrorMessage = "Invalid URL format")]
        [StringLength(500)]
        public string Url { get; set; } = string.Empty;
        
        public int DisplayOrder { get; set; } = 0;
        
        public bool IsPrimary { get; set; } = false;
    }
}

// ============================================
// FILE: ProductPriceDto.cs
// Mô tả: DTO cho giá sản phẩm
// ============================================
using VietCommerce.Core.Enums.Products;

namespace VietCommerce.Core.DTOs.Products
{
    public class ProductPriceDto
    {
        public Guid Id { get; set; }
        
        public Guid ProductId { get; set; }
        
        public PriceType PriceType { get; set; }
        
        [Range(0, double.MaxValue)]
        public decimal Price { get; set; }
        
        public DateTime EffectiveFrom { get; set; }
        
        public DateTime? EffectiveTo { get; set; }
        
        public bool IsActive { get; set; }
        
        public DateTime CreatedAt { get; set; }
        
        public Guid CreatedBy { get; set; }
        
        public string? CreatedByName { get; set; }
    }
}

// ============================================
// FILE: ProductPriceCreateDto.cs
// Mô tả: DTO để tạo giá mới cho sản phẩm
// ============================================
using VietCommerce.Core.Enums.Products;

namespace VietCommerce.Core.DTOs.Products
{
    public class ProductPriceCreateDto
    {
        [Required(ErrorMessage = "Product ID is required")]
        public Guid ProductId { get; set; }
        
        [Required(ErrorMessage = "Price type is required")]
        public PriceType PriceType { get; set; }
        
        [Required(ErrorMessage = "Price is required")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than 0")]
        public decimal Price { get; set; }
        
        [Required(ErrorMessage = "Effective from date is required")]
        public DateTime EffectiveFrom { get; set; }
        
        public DateTime? EffectiveTo { get; set; }
        
        public bool IsActive { get; set; } = true;
    }
}

// ============================================
// FILE: ProductPriceUpdateDto.cs
// Mô tả: DTO để cập nhật giá sản phẩm
// ============================================
namespace VietCommerce.Core.DTOs.Products
{
    public class ProductPriceUpdateDto
    {
        [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than 0")]
        public decimal? Price { get; set; }
        
        public DateTime? EffectiveTo { get; set; }
        
        public bool? IsActive { get; set; }
    }
}

// ============================================
// FILE: ProductInventoryDto.cs
// Mô tả: DTO cho inventory của sản phẩm
// ============================================
namespace VietCommerce.Core.DTOs.Products
{
    public class ProductInventoryDto
    {
        public Guid Id { get; set; }
        
        public Guid ProductId { get; set; }
        
        public Guid WarehouseId { get; set; }
        
        public string WarehouseName { get; set; } = string.Empty;
        
        public int Quantity { get; set; }
        
        public int ReservedQuantity { get; set; }
        
        public int AvailableQuantity => Quantity - ReservedQuantity;
        
        public int ReorderLevel { get; set; }
        
        public int ReorderQuantity { get; set; }
        
        public bool NeedsReorder => AvailableQuantity <= ReorderLevel;
        
        public DateTime? LastRestocked { get; set; }
        
        public DateTime UpdatedAt { get; set; }
    }
}

// ============================================
// FILE: ProductReviewDto.cs
// Mô tả: DTO cho đánh giá sản phẩm
// ============================================
namespace VietCommerce.Core.DTOs.Products
{
    public class ProductReviewDto
    {
        public Guid Id { get; set; }
        
        public Guid ProductId { get; set; }
        
        public Guid UserId { get; set; }
        
        public string UserName { get; set; } = string.Empty;
        
        public string? UserAvatar { get; set; }
        
        [Range(1, 5, ErrorMessage = "Rating must be between 1 and 5")]
        public int Rating { get; set; }
        
        [StringLength(1000, ErrorMessage = "Comment cannot exceed 1000 characters")]
        public string? Comment { get; set; }
        
        public bool IsVerifiedPurchase { get; set; }
        
        public int HelpfulCount { get; set; }
        
        public List<string>? Images { get; set; }
        
        public DateTime CreatedAt { get; set; }
        
        public DateTime? UpdatedAt { get; set; }
    }
}

// ============================================
// FILE: ProductReviewCreateDto.cs
// Mô tả: DTO để tạo đánh giá sản phẩm
// ============================================
namespace VietCommerce.Core.DTOs.Products
{
    public class ProductReviewCreateDto
    {
        [Required(ErrorMessage = "Product ID is required")]
        public Guid ProductId { get; set; }
        
        [Required(ErrorMessage = "Rating is required")]
        [Range(1, 5, ErrorMessage = "Rating must be between 1 and 5")]
        public int Rating { get; set; }
        
        [StringLength(1000, ErrorMessage = "Comment cannot exceed 1000 characters")]
        public string? Comment { get; set; }
        
        public List<string>? ImageUrls { get; set; }
    }
}

// ============================================
// FILE: ProductStatisticsDto.cs
// Mô tả: DTO cho thống kê sản phẩm
// ============================================
namespace VietCommerce.Core.DTOs.Products
{
    public class ProductStatisticsDto
    {
        public Guid ProductId { get; set; }
        
        public int TotalViews { get; set; }
        
        public int TotalSales { get; set; }
        
        public int TotalReviews { get; set; }
        
        public decimal AverageRating { get; set; }
        
        public int FavoriteCount { get; set; }
        
        public decimal TotalRevenue { get; set; }
        
        public int ReturnCount { get; set; }
        
        public decimal ReturnRate => TotalSales > 0 ? (decimal)ReturnCount / TotalSales * 100 : 0;
        
        public DateTime? LastSoldAt { get; set; }
        
        public DateTime? LastViewedAt { get; set; }
    }
}

// ============================================
// FILE: SupplierDto.cs
// Mô tả: DTO cho nhà cung cấp (dùng trong ProductDetailDto)
// ============================================
namespace VietCommerce.Core.DTOs.Products
{
    public class SupplierDto
    {
        public Guid Id { get; set; }
        
        public string Name { get; set; } = string.Empty;
        
        public string Code { get; set; } = string.Empty;
        
        public string? ContactPerson { get; set; }
        
        public string? Email { get; set; }
        
        public string? Phone { get; set; }
        
        public bool IsActive { get; set; }
    }
}

// ============================================
// FILE: ProductBulkUpdateDto.cs
// Mô tả: DTO để cập nhật nhiều sản phẩm cùng lúc
// ============================================
namespace VietCommerce.Core.DTOs.Products
{
    public class ProductBulkUpdateDto
    {
        [Required(ErrorMessage = "Product IDs are required")]
        [MinLength(1, ErrorMessage = "At least one product ID is required")]
        public List<Guid> ProductIds { get; set; } = new();
        
        public bool? IsActive { get; set; }
        
        public Guid? CategoryId { get; set; }
        
        public decimal? PriceAdjustmentPercent { get; set; }
        
        public int? StockAdjustment { get; set; }
    }
}

// ============================================
// FILE: ProductListDto.cs (Updated version)
// Mô tả: DTO đơn giản cho list (compatibility với code cũ)
// ============================================
namespace VietCommerce.Core.DTOs.Products
{
    public class ProductListDTO
    {
        public Guid Id { get; set; }
        
        public string Name { get; set; } = string.Empty;
        
        public string? Description { get; set; }
        
        public decimal Price { get; set; }
        
        public Guid CategoryId { get; set; }
        
        public int StockQuantity { get; set; }
        
        public DateTime CreatedDate { get; set; }
    }
}

// ============================================
// FILE: ProductExportDto.cs
// Mô tả: DTO để export dữ liệu sản phẩm
// ============================================
namespace VietCommerce.Core.DTOs.Products
{
    public class ProductExportDto
    {
        public string Code { get; set; } = string.Empty;
        
        public string Name { get; set; } = string.Empty;
        
        public string SKU { get; set; } = string.Empty;
        
        public string CategoryName { get; set; } = string.Empty;
        
        public string StoreName { get; set; } = string.Empty;
        
        public decimal CurrentPrice { get; set; }
        
        public int Stock { get; set; }
        
        public string Status { get; set; } = string.Empty;
        
        public int TotalSales { get; set; }
        
        public decimal TotalRevenue { get; set; }
        
        public DateTime CreatedAt { get; set; }
    }
}

// ============================================
// FILE: ProductImportDto.cs
// Mô tả: DTO để import sản phẩm từ Excel/CSV
// ============================================
namespace VietCommerce.Core.DTOs.Products
{
    public class ProductImportDto
    {
        [Required]
        public string Name { get; set; } = string.Empty;
        
        public string? SKU { get; set; }
        
        [Required]
        public string CategoryCode { get; set; } = string.Empty;
        
        [Required]
        public string StoreCode { get; set; } = string.Empty;
        
        [Required]
        [Range(0.01, double.MaxValue)]
        public decimal Price { get; set; }
        
        [Range(0, int.MaxValue)]
        public int Stock { get; set; }
        
        public string? SupplierCodes { get; set; } // Comma-separated
        
        public string? ImageUrls { get; set; } // Comma-separated
    }
}