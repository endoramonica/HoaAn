using VietCommerce.Core.Common;
using VietCommerce.Core.Entities.Customers;
using VietCommerce.Core.Entities.Marketing;
using VietCommerce.Core.Entities.Orders;
using VietCommerce.Core.Entities.Products;
using VietCommerce.Core.Entities.Users;
using System.ComponentModel.DataAnnotations.Schema;
using VietCommerce.Core.Entities.HRM;


namespace VietCommerce.Core.Entities.Organization;

public class Store : BaseEntity, ISoftDelete
{
    public Guid TenantId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Address { get; set; }
    public string? Phone { get; set; }
    public bool IsActive { get; set; } = true;

    // Navigation properties
    public virtual Tenant Tenant { get; set; } = null!;
    public virtual ICollection<Campaign> Campaigns { get; set; } = new List<Campaign>();
    public virtual ICollection<User> Users { get; set; } = new List<User>();
    public virtual ICollection<Customer> Customers { get; set; } = new List<Customer>();
    public virtual ICollection<Category> Categories { get; set; } = new List<Category>();
    public virtual ICollection<Product> Products { get; set; } = new List<Product>();
    public virtual ICollection<Inventory> Inventories { get; set; } = new List<Inventory>();
    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
    public virtual ICollection<UserAddress> Addresses { get; set; } = new List<UserAddress>();
    // Thêm vào Store.cs
[InverseProperty(nameof(Shift.Store))]
public ICollection<Shift> Shifts { get; set; } = new List<Shift>();

[InverseProperty(nameof(Employee.Store))]
public ICollection<Employee> Employees { get; set; } = new List<Employee>();
    public bool IsDeleted { get; set; }
    public DateTime? DeletedAt {get;set; }
    public Guid? DeletedBy {get;set; }
}