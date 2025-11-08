namespace VietCommerce.Core.DTOs.Category;

// DTO cho hierarchy tree
public class CategoryHierarchyDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public Guid? ParentId { get; set; }
    public bool IsActive { get; set; }
    public int Level { get; set; }
    public ICollection<CategoryHierarchyDto> Children { get; set; } = new List<CategoryHierarchyDto>();
}