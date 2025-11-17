// ============================================
// FILE: ProductDto.cs
// Mô tả: DTO cơ bản cho danh sách sản phẩm
// ============================================
using System.ComponentModel.DataAnnotations;
using VietCommerce.Core.Enums.Products;
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
        public string Url { get; set; } = string.Empty;
        public string? ThumbnailUrl { get; set; }
        public int DisplayOrder { get; set; }
        public MediaTypeEnum MediaType { get; set; }
        public bool IsMain { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
    }
}
