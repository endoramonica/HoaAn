using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using VietCommerce.Core.Common;
using VietCommerce.Core.Entities.Products;
using VietCommerce.Core.Entities.Logistics;
namespace VietCommerce.Core.Entities.Logistics
{
    [Table("TransferItems")]
    [Index(nameof(StockTransferId))]
    [Index(nameof(ProductId))]
    public class TransferItem : BaseEntity
    {
        [Required]
        [MaxLength(36)]
        public Guid StockTransferId { get; set; } 
        [Required]
        [MaxLength(36)]
        public Guid ProductId { get; set; } 
        [Required]
        public int Quantity { get; set; }
        public int? Received { get; set; }
        [MaxLength(1000)]
        public string? Notes { get; set; }
        // Navigation properties
        [ForeignKey(nameof(StockTransferId))]
        public StockTransfer StockTransfer { get; set; } = null!;
        [ForeignKey(nameof(ProductId))]
        public Product Product { get; set; } = null!;
    }
}
