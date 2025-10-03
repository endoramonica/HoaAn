using VietCommerce.Core.Common;
using VietCommerce.Core.Entities.Users;
using VietCommerce.Core.Enums;
using VietCommerce.Core.Enums.Products;
using System.ComponentModel.DataAnnotations.Schema;
using VietCommerce.Core.Entities.Logistics;
//log chi tiết các hoạt động làm thay đổi tồn kho, giúp audit và truy vết
namespace VietCommerce.Core.Entities.Products;

public class InventoryMovement : AuditableEntity
{
    public Guid InventoryId { get; set; }
    public int ChangeAmount { get; set; }
    public InventoryMovementType MovementType { get; set; }

    // Optional improvements
    public int? QuantityBefore { get; set; }
    public int? QuantityAfter { get; set; }
    public string? Reason { get; set; }

    public Guid? PerformedById { get; set; }

    // Navigation properties
    public virtual Inventory Inventory { get; set; } = null!;
    public virtual User? PerformedBy { get; set; }
    // Thêm vào InventoryMovement.cs

public string? TransferId { get; set; }

[ForeignKey(nameof(TransferId))]
public StockTransfer? Transfer { get; set; }
}
