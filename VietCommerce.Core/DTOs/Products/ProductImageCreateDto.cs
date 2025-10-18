

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

// ============================================
// FILE: ProductImageCreateDto.cs
// Mô tả: DTO để thêm hình ảnh cho sản phẩm
// ============================================
using System.ComponentModel.DataAnnotations;

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

