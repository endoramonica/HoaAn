using System.ComponentModel.DataAnnotations;
using VietCommerce.Core.Common;
using VietCommerce.Core.Entities.Audit;
using VietCommerce.Core.Entities.Marketing;
using VietCommerce.Core.Entities.Notifications;
using VietCommerce.Core.Entities.Orders;
using VietCommerce.Core.Entities.Organization;
using VietCommerce.Core.Entities.Products;
using VietCommerce.Core.Enums.Users;


namespace VietCommerce.Core.Entities.Users;

public class User : AuditableEntity, ISoftDelete
{
    public Guid StoreId { get; set; }
    
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public string? Name { get; set; }
    public string? Phone { get; set; }
    public bool IsActive { get; set; } = true;
    // 🟡 Các trường từ ISoftDelete
    public bool IsDeleted { get; set; } = false;
    public DateTime? DeletedAt { get; set; }
    public Guid? DeletedBy { get; set; }

    public DateTime? LastLogin { get; set; }
    // ⭐ THÊM VÀO:
    [Required]
    public UserStatus Status { get; set; } = UserStatus.ACTIVE;

    // ⭐ THÊM NAVIGATION PROPERTIES:
    public virtual ICollection<UserAddress> UserAddresses { get; set; } = new List<UserAddress>();
    
    public virtual ICollection<ProductPrice> CreatedPrices { get; set; } = new List<ProductPrice>();
   
    // Navigation properties
    public virtual Store Store { get; set; } = null!;
    public virtual ICollection<Campaign> CreatedCampaigns { get; set; } = new List<Campaign>();
    public virtual ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
    public virtual ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();
    public virtual ICollection<UserAddress> Addresses { get; set; } = new List<UserAddress>();
    public virtual ICollection<Notification> Notifications { get; set; } = new List<Notification>();
    public virtual ICollection<Cart> Carts { get; set; } = new List<Cart>();
    public virtual ICollection<Order> CreatedOrders { get; set; } = new List<Order>();
    public virtual ICollection<InventoryMovement> InventoryMovements { get; set; } = new List<InventoryMovement>();
    public virtual ICollection<AuditLog> AuditLogs { get; set; } = new List<AuditLog>();
    
}
