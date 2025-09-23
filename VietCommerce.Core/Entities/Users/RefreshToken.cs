using VietCommerce.Core.Common;

namespace VietCommerce.Core.Entities.Users;

public class RefreshToken : BaseEntity
{
    public Guid UserId { get; set; }
    public string Token { get; set; } = string.Empty;
    public DateTime ExpiresAt { get; set; }

    // Navigation properties
    public virtual User User { get; set; } = null!;
}