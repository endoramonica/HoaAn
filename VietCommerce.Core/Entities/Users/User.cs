using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using VietCommerce.Core.Common;
using VietCommerce.Core.Entities.Audit;
using VietCommerce.Core.Entities.Marketing;
using VietCommerce.Core.Entities.Notifications;
using VietCommerce.Core.Entities.Orders;
using VietCommerce.Core.Entities.Organization;
using VietCommerce.Core.Entities.Products;
using VietCommerce.Core.Enums.Users;
using VietCommerce.Core.Entities.HRM;
using VietCommerce.Core.Entities.CRM;
using VietCommerce.Core.Entities.Logistics;
using VietCommerce.Core.Entities.Tasks;

namespace VietCommerce.Core.Entities.Users;

public class User : AuditableEntity, ISoftDelete
{
    public Guid StoreId { get; set; }
    
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public string? Name { get; set; }
    public string? Phone { get; set; }
    public bool IsActive { get; set; } = true;
    
    // Các trường từ ISoftDelete
    public bool IsDeleted { get; set; } = false;
    public DateTime? DeletedAt { get; set; }
    public Guid? DeletedBy { get; set; }
    
    // MANAGER HIERARCHY - GIỮ LẠI
    public Guid? ManagerId { get; set; }
    
    [ForeignKey(nameof(ManagerId))]
    public virtual User? Manager { get; set; }
    
    [InverseProperty(nameof(Manager))]
    public virtual ICollection<User> Subordinates { get; set; } = new List<User>();
    
    // Link to HR data - 1-1 relationship
    public Guid? EmployeeId { get; set; }
    
    [ForeignKey(nameof(EmployeeId))]
    public virtual Employee? EmployeeProfile { get; set; }
    
    public DateTime? LastLogin { get; set; }
    
    [Required]
    public UserStatus Status { get; set; } = UserStatus.ACTIVE;
    
    // Navigation properties - Store
    [ForeignKey(nameof(StoreId))]
    public virtual Store Store { get; set; } = null!;
    

    public virtual ICollection<UserAddress> UserAddresses { get; set; } = new List<UserAddress>();
    public virtual ICollection<ProductPrice> CreatedPrices { get; set; } = new List<ProductPrice>();
    public virtual ICollection<Campaign> CreatedCampaigns { get; set; } = new List<Campaign>();
    public virtual ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
    public virtual ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();
    public virtual ICollection<Notification> Notifications { get; set; } = new List<Notification>();
    public virtual ICollection<Cart> Carts { get; set; } = new List<Cart>();
    public virtual ICollection<Order> CreatedOrders { get; set; } = new List<Order>();
    public virtual ICollection<InventoryMovement> InventoryMovements { get; set; } = new List<InventoryMovement>();
    public virtual ICollection<AuditLog> AuditLogs { get; set; } = new List<AuditLog>();
    [InverseProperty(nameof(Shift.Staff))]
    public virtual ICollection<Shift> Shifts { get; set; } = new List<Shift>();
    [InverseProperty(nameof(WorkTask.AssignedToUser))]
    public virtual ICollection<WorkTask> AssignedTasks { get; set; } = new List<WorkTask>();
    
    [InverseProperty(nameof(WorkTask.AssignedByUser))]
    public virtual ICollection<WorkTask> CreatedTasks { get; set; } = new List<WorkTask>();
    
    // CRM
    [InverseProperty(nameof(CRMInteraction.CreatedByUser))]
    public virtual ICollection<CRMInteraction> CRMInteractions { get; set; } = new List<CRMInteraction>();
    
    // Logistics
    [InverseProperty(nameof(StockTransfer.RequestedByUser))]
    public virtual ICollection<StockTransfer> RequestedTransfers { get; set; } = new List<StockTransfer>();
    
    [InverseProperty(nameof(StockTransfer.ApprovedByUser))]
    public virtual ICollection<StockTransfer> ApprovedTransfers { get; set; } = new List<StockTransfer>();
}