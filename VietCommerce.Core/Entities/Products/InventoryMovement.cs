using System;
using System.ComponentModel.DataAnnotations.Schema;
using VietCommerce.Core.Common;
using VietCommerce.Core.Entities.Users;
using VietCommerce.Core.Enums;
using VietCommerce.Core.Enums.Products;
using VietCommerce.Core.Entities.Logistics;
using VietCommerce.Core.Entities.Orders;

namespace VietCommerce.Core.Entities.Products
{
    // Log chi tiết các hoạt động làm thay đổi tồn kho, giúp audit và truy vết
    public class InventoryMovement : AuditableEntity
    {
        public Guid InventoryId { get; set; }

        [ForeignKey(nameof(InventoryId))]
        public virtual Inventory Inventory { get; set; } = null!;

        // Đơn hàng liên quan (nếu có)
        public Guid? OrderId { get; set; }

        [ForeignKey(nameof(OrderId))]
        public Order? Order { get; set; }

        // Phiếu chuyển kho liên quan (nếu có)
        public Guid? TransferId { get; set; }

        [ForeignKey(nameof(TransferId))]
        public StockTransfer? Transfer { get; set; }

        // Số lượng thay đổi (có thể là dương hoặc âm)
        public int ChangeAmount { get; set; }

        public InventoryMovementType MovementType { get; set; }

        // Thông tin bổ sung
        public int? QuantityBefore { get; set; }
        public int? QuantityAfter { get; set; }
        public string? Reason { get; set; }

        // Người thực hiện
        public Guid? PerformedById { get; set; }

        [ForeignKey(nameof(PerformedById))]
        public virtual User? PerformedBy { get; set; }
    }
}