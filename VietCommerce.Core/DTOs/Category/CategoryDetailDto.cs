namespace VietCommerce.Core.DTOs.Category;

// DTO chi tiết với navigation properties
public class CategoryDetailDto : CategoryDto
{
    public ICollection<CategoryDto> SubCategories { get; set; } = new List<CategoryDto>();
    public ICollection<ProductSummaryDto> Products { get; set; } = new List<ProductSummaryDto>();
}
