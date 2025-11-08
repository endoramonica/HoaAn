namespace VietCommerce.Core.DTOs.Category;

// DTO tóm tắt sản phẩm (để dùng trong CategoryDetailDto)
public class ProductSummaryDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public bool IsActive { get; set; }
}
