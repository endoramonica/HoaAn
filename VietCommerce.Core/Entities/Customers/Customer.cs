using VietCommerce.Core.Common;
using VietCommerce.Core.Entities.Customers;
using VietCommerce.Core.Entities.Orders;
using VietCommerce.Core.Entities.Organization;
using VietCommerce.Core.Entities.Products;
using VietCommerce.Core.Entities.Users;


namespace VietCommerce.Core.Entities.Customers;

public class Customer : BaseEntity,ISoftDelete
{
    public Guid StoreId { get; set; }
    public Guid? UserId { get; set; }
    public string? Name { get; set; }
    public string? Phone { get; set; }
    public int LoyaltyPoints { get; set; } = 0;
    public string? Tier { get; set; }
    public string? Email { get; set; }           // thêm Email


    // Navigation properties
    public virtual ICollection<Cart> Carts { get; set; } = new List<Cart>(); // thêm Carts
    public virtual Store Store { get; set; } = null!;
    public virtual User? User { get; set; }
    public virtual Tenant Tenant { get; set; } = null!;
    public virtual ICollection<CustomerAddress> Addresses { get; set; } = new List<CustomerAddress>();
    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
    public bool IsActive { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    public bool IsDeleted { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    public DateTime? DeletedAt { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    public int? DeletedBy { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
}