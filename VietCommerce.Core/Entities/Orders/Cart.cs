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
    public Guid? CustomerId { get; set; }
    public string? SessionId { get; set; }
    // Navigation properties
    public virtual User User { get; set; } = null!;
    public virtual Customer? Customer { get; set; } 
    public virtual ICollection<CartItem> CartItems { get; set; } = new List<CartItem>();
    // Soft delete properties (implements ISoftDelete)
    public bool IsActive { get; set; } = true;
    public bool IsDeleted { get; set; } = false;
    public DateTime? DeletedAt { get; set; }
    public Guid? DeletedBy { get; set; }
}
