

// ============================================
// FILE: ProductCreateDto.cs
// Mô tả: DTO để tạo sản phẩm mới
// ============================================

// ============================================
// FILE: ProductUpdateDto.cs
// Mô tả: DTO để cập nhật sản phẩm
// ============================================

// ============================================
// FILE: ProductFilterDto.cs
// Mô tả: DTO cho filtering và pagination
// ============================================

// ============================================
// FILE: ProductImageDto.cs
// Mô tả: DTO cho hình ảnh sản phẩm
// ============================================
using System.ComponentModel.DataAnnotations;

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
