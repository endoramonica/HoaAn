using System.ComponentModel.DataAnnotations;

namespace VietCommerce.Core.DTOs.Category;

// DTO tạo mới
public class CreateCategoryDto
{
    [Required(ErrorMessage = "Tên danh mục là bắt buộc")]
    [StringLength(200, MinimumLength = 2, ErrorMessage = "Tên danh mục phải từ 2-200 ký tự")]
    public string Name { get; set; } = string.Empty;

    [Required(ErrorMessage = "StoreId là bắt buộc")]
    public Guid StoreId { get; set; }

    public Guid? ParentId { get; set; }

    public bool IsActive { get; set; } = true;
}
