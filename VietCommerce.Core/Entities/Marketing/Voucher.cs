using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using VietCommerce.Core.Common;

namespace VietCommerce.Core.Entities.Marketing
{
    [Table("Vouchers")]
    public class Voucher : BaseEntity, ISoftDelete
    {
        [Required, MaxLength(100)]
        public string Code { get; set; } = string.Empty;

        public Guid PromotionId { get; set; }

        [Required]
        public DateTime ExpiryDate { get; set; }

        public int UsageCount { get; set; } = 0;

        public DateTime? LastUsedAt { get; set; }

        public Guid? LastUsedBy { get; set; }

        // ISoftDelete implementation
        public bool IsActive { get; set; } = true;
        public bool IsDeleted { get; set; } = false;
        public DateTime? DeletedAt { get; set; }
        public Guid? DeletedBy { get; set; }

        // Navigation
        public virtual Promotion Promotion { get; set; } = null!;
        public virtual ICollection<VoucherRedemption> Redemptions { get; set; } = new List<VoucherRedemption>();
    }
}
