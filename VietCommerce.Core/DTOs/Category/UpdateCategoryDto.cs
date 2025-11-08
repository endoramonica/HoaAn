using System.ComponentModel.DataAnnotations;

namespace VietCommerce.Core.DTOs.Category;

// DTO cập nhật
public class UpdateCategoryDto
{
    [Required(ErrorMessage = "Tên danh mục là bắt buộc")]
    [StringLength(200, MinimumLength = 2, ErrorMessage = "Tên danh mục phải từ 2-200 ký tự")]
    public string Name { get; set; } = string.Empty;

    public Guid? ParentId { get; set; }

    public bool IsActive { get; set; }
}
