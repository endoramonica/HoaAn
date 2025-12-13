using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using VietCommerce.Core.Common;

namespace VietCommerce.Core.Entities.Marketing
{
    [Table("VoucherRedemptions")]
    public class VoucherRedemption : BaseEntity
    {
        public Guid VoucherId { get; set; }

        public Guid? OrderId { get; set; }

        [Column(TypeName = "decimal(15,2)")]
        public decimal DiscountAmount { get; set; }

        public DateTime RedeemedAt { get; set; } = DateTime.UtcNow;

        public Guid? RedeemedBy { get; set; }

        // Navigation
        public virtual Voucher Voucher { get; set; } = null!;
    }
}
