

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

