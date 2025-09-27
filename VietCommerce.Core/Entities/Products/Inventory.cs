
using VietCommerce.Core.Common;
using VietCommerce.Core.Entities.Organization;

namespace VietCommerce.Core.Entities.Products;

public class Inventory : BaseEntity , ISoftDelete
{
    public Guid StoreId { get; set; }
    public Guid ProductId { get; set; }
    // 👉 Tổng số lượng trong kho
    public int QuantityAvailable { get; set; } = 0;
    // 👉 Số lượng đã được giữ (đơn hàng chờ xử lý, chưa xuất kho)
    public int QuantityReserved { get; set; } = 0;

    // 👉 Ngưỡng cảnh báo tồn kho thấp
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
}
