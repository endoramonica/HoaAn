using VietCommerce.Core.Common;
using VietCommerce.Core.Entities.Orders;
using VietCommerce.Core.Entities.Organization;
using VietCommerce.Core.Entities.CRM;
using VietCommerce.Core.Entities.Users;
using System.ComponentModel.DataAnnotations.Schema;

namespace VietCommerce.Core.Entities.Customers;

public class Customer : BaseEntity, ISoftDelete
{
    public Guid TenantId { get; set; }
    public Guid? StoreId { get; set; }
    public Guid? UserId { get; set; }
    public string? Name { get; set; }
    public string? Phone { get; set; }
    public int LoyaltyPoints { get; set; } = 0;
    public string? Tier { get; set; }
    public string? Email { get; set; }
    public bool IsActive { get; set; }

    // ISoftDelete
    public bool IsDeleted { get; set; }
    public DateTime? DeletedAt { get; set; }
    public Guid? DeletedBy { get; set; }

    // ========== NAVIGATION PROPERTIES ==========

    [ForeignKey(nameof(TenantId))]
    public virtual Tenant Tenant { get; set; } = null!;

    [ForeignKey(nameof(StoreId))]
    public virtual Store? Store { get; set; } = null!;

    [ForeignKey(nameof(UserId))]
    public virtual User? User { get; set; }
    public string? CustomerAvatar { get; set; }

    // Collections
    public virtual ICollection<Cart> Carts { get; set; } = new List<Cart>();
    public virtual ICollection<CustomerAddress> Addresses { get; set; } = new List<CustomerAddress>();
    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();

    [InverseProperty(nameof(CRMInteraction.Customer))]
    public virtual ICollection<CRMInteraction> Interactions { get; set; } = new List<CRMInteraction>();
}