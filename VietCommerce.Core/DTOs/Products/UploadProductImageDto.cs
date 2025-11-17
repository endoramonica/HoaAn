// ============================================
// FILE: ProductDto.cs
// Mô tả: DTO cơ bản cho danh sách sản phẩm
// ============================================
// ============================================
// FILE: ProductImageDto.cs
// Mô tả: DTO cho hình ảnh sản phẩm
// ============================================
namespace VietCommerce.Core.DTOs.Products
{
    public class UploadProductImageDto
    {
        public Guid ProductId { get; set; }
        public bool SetFirstAsMain { get; set; } = false;
    }
}
