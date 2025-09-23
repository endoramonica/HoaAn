namespace VietCommerce.Core.Common;

public interface ISoftDelete
{
    bool IsActive { get; set; }
    bool IsDeleted { get; set; }
    DateTime? DeletedAt { get; set; }
    // ⭐ THÊM VÀO:
    int? DeletedBy { get; set; }  // Người xóa
}