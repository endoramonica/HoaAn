using System.ComponentModel.DataAnnotations;
using VietCommerce.Core.Common;
using VietCommerce.Core.Entities.Customers;
using VietCommerce.Core.Entities.Organization;
using VietCommerce.Core.Entities.Payments;
using VietCommerce.Core.Entities.Products;
using VietCommerce.Core.Entities.Users;
using VietCommerce.Core.Enums.Orders;
using System.ComponentModel.DataAnnotations.Schema;

namespace VietCommerce.Core.Entities.Orders;

public class Order : AuditableEntity , ISoftDelete
{
    public Guid StoreId { get; set; }
    public Guid? CustomerId { get; set; }
   
    public OrderStatus Status { get; set; } 
    public decimal TotalAmount { get; set; }
    [Required]
    public string OrderNumber { get; set; } = string.Empty; // th�m OrderNumber


    // Navigation properties
    public virtual Store Store { get; set; } = null!;
    public virtual Customer Customer { get; set; } = null!;
    public virtual User? CreatedByUser { get; set; }
    public Guid? CreatedById { get; set; }

[ForeignKey(nameof(CreatedById))]
    

    public virtual ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
    public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();
    public virtual ICollection<OrderStatusHistory> OrderStatusHistories { get; set; } = new List<OrderStatusHistory>();
    public virtual ICollection<InventoryMovement> InventoryMovements { get; set; } = new List<InventoryMovement>();     

    public virtual OrderShipping? OrderShipping { get; set; }
    public bool IsActive {get;set; }
    public bool IsDeleted {get;set; }
    public DateTime? DeletedAt {get;set; }
    public Guid? DeletedBy {get;set; }
}
