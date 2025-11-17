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
    /// <summary>
    /// DTO cho set ảnh chính
    /// </summary>
    public class SetMainImageDto
    {
        public Guid ProductId { get; set; }
        public Guid ImageId { get; set; }
    }
}
