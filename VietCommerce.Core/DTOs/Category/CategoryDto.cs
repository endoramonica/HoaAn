namespace VietCommerce.Core.DTOs.Category;

// DTO hiển thị cơ bản
public class CategoryDto
{
    public Guid Id { get; set; }
    public Guid StoreId { get; set; }
    public string Name { get; set; } = string.Empty;
    public Guid? ParentId { get; set; }
    public string? ParentName { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public int SubCategoriesCount { get; set; }
    public int ProductsCount { get; set; }
}
