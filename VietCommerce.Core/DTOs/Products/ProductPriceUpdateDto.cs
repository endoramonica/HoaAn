// ============================================
// FILE: ProductDto.cs
// Mô tả: DTO cơ bản cho danh sách sản phẩm
// ============================================
using System.ComponentModel.DataAnnotations;
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
