

// ============================================
// FILE: ProductCreateDto.cs
// Mô tả: DTO để tạo sản phẩm mới
// ============================================

// ============================================
// FILE: ProductUpdateDto.cs
// Mô tả: DTO để cập nhật sản phẩm
// ============================================
using System.ComponentModel.DataAnnotations;

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
