using VietCommerce.Core.Common;
using VietCommerce.Core.Entities.Organization;
namespace VietCommerce.Core.Entities.Products;
public class Inventory : BaseEntity , ISoftDelete
{
    public Guid StoreId { get; set; }
    public Guid ProductId { get; set; }
    public Guid? TenantId { get; set; }
    // ?? T?ng s? lu?ng trong kho
    public int QuantityAvailable { get; set; } = 0;
    // ?? S? lu?ng dã du?c gi? (don hàng ch? x? lý, chua xu?t kho)
    public int QuantityReserved { get; set; } = 0;
    // ?? Ngu?ng c?nh báo t?n kho th?p
    public int ReorderLevel { get; set; } = 0;
    // Navigation properties
    public virtual Store Store { get; set; } = null!;
    public virtual Tenant Tenant { get; set; } = null!;
    public virtual Product Product { get; set; } = null!;
    public virtual ICollection<InventoryMovement> InventoryMovements { get; set; } = new List<InventoryMovement>();
    public bool IsActive {get;set; }
    public bool IsDeleted {get;set; }
    public DateTime? DeletedAt {get;set; }
    public Guid? DeletedBy {get;set; }
    public byte[] RowVersion { get; set; } = Array.Empty<byte>();
}
