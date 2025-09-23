using VietCommerce.Core.Common;
using VietCommerce.Core.Entities.Customers;
using VietCommerce.Core.Entities.Orders;
using VietCommerce.Core.Entities.Organization;
using VietCommerce.Core.Entities.Products;
using VietCommerce.Core.Entities.Users;

namespace VietCommerce.Core.Entities.Orders;

public class Cart : BaseEntity , ISoftDelete
{
    public Guid UserId { get; set; }
    public Guid CustomerId { get; set; }

    // Navigation properties
    public virtual User User { get; set; } = null!;
    public virtual Customer? Customer { get; set; } 
    public virtual ICollection<CartItem> CartItems { get; set; } = new List<CartItem>();
    public bool IsActive { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    public bool IsDeleted { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    public DateTime? DeletedAt { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    public int? DeletedBy { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
}