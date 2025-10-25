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
    // Log chi ti?t các ho?t d?ng làm thay d?i t?n kho, giúp audit và truy v?t
    public class InventoryMovement : AuditableEntity
    {
        public Guid InventoryId { get; set; }
        [ForeignKey(nameof(InventoryId))]
        public virtual Inventory Inventory { get; set; } = null!;
        // Ðon hàng liên quan (n?u có)
        public Guid? OrderId { get; set; }
        [ForeignKey(nameof(OrderId))]
        public Order? Order { get; set; }
        // Phi?u chuy?n kho liên quan (n?u có)
        public Guid? TransferId { get; set; }
        [ForeignKey(nameof(TransferId))]
        public StockTransfer? Transfer { get; set; }
        // S? lu?ng thay d?i (có th? là duong ho?c âm)
        public int ChangeAmount { get; set; }
        public InventoryMovementType MovementType { get; set; }
        // Thông tin b? sung
        public int? QuantityBefore { get; set; }
        public int? QuantityAfter { get; set; }
        public string? Reason { get; set; }
        // Ngu?i th?c hi?n
        public Guid? PerformedById { get; set; }
        [ForeignKey(nameof(PerformedById))]
        public virtual User? PerformedBy { get; set; }
    }
}
