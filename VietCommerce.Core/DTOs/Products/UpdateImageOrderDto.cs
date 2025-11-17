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
    /// DTO cho update thứ tự ảnh
    /// </summary>
    public class UpdateImageOrderDto
    {
        public Guid ImageId { get; set; }
        public int NewDisplayOrder { get; set; }
    }
}
