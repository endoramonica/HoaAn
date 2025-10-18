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
