using VietCommerce.Core.Common;
using VietCommerce.Core.Entities.Customers;
using VietCommerce.Core.Entities.Orders;
using VietCommerce.Core.Entities.Products;
using VietCommerce.Core.Entities.Users;
namespace VietCommerce.Core.Entities.Organization;
public class Tenant : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string? Domain { get; set; }
    public string? Status { get; set; }
    public string? Plan { get; set; }
    // Navigation properties
    public virtual ICollection<Store> Stores { get; set; } = new List<Store>();
    public virtual ICollection<User> Users { get; set; } = new List<User>();
    public virtual ICollection<Customer> Customers { get; set; } = new List<Customer>();
    public virtual ICollection<Category> Categories { get; set; } = new List<Category>();
    public virtual ICollection<Product> Products { get; set; } = new List<Product>();
    public virtual ICollection<Inventory> Inventories { get; set; } = new List<Inventory>();
    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
}
