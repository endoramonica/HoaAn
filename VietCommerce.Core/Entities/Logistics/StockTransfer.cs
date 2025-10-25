using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using VietCommerce.Core.Common;
using VietCommerce.Core.Entities.Users;
using VietCommerce.Core.Entities.Products;
using VietCommerce.Core.Entities.Logistics;
namespace VietCommerce.Core.Entities.Logistics
{
    [Table("StockTransfers")]
    [Index(nameof(Status))]
    [Index(nameof(CreatedAt))]
    public class StockTransfer : BaseEntity
    {
        [Required]
        [MaxLength(200)]
        public string FromWarehouse { get; set; } = string.Empty;
        [Required]
        [MaxLength(200)]
        public string ToWarehouse { get; set; } = string.Empty;
        [Required]
        public StockTransferStatus Status { get; set; } = StockTransferStatus.Pending;
        [Required]
        [MaxLength(36)]
        public Guid? RequestedBy { get; set; } 
        [MaxLength(36)]
        public Guid? ApprovedBy { get; set; }
        [MaxLength(36)]
        public Guid? SupplierId { get; set; }
        public DateTime? DeliveryDate { get; set; }
        [MaxLength(1000)]
        public string? Notes { get; set; }
        // Navigation properties
        [ForeignKey(nameof(RequestedBy))]
        public User RequestedByUser { get; set; } = null!;
        [ForeignKey(nameof(ApprovedBy))]
        public User? ApprovedByUser { get; set; }
        [ForeignKey(nameof(SupplierId))]
        public Supplier? Supplier { get; set; }
        [InverseProperty(nameof(TransferItem.StockTransfer))]
        public ICollection<TransferItem> TransferItems { get; set; } = new List<TransferItem>();
        public ICollection<InventoryMovement> InventoryMovements { get; set; } = new List<InventoryMovement>();
    }
    public enum StockTransferStatus
    {
        Pending = 1,
        InTransit = 2,
        Delivered = 3,
        Cancelled = 4
    }
}
