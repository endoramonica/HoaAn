using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using VietCommerce.Core.Common;
using VietCommerce.Core.Entities.Products;
using VietCommerce.Core.Enums.Marketing;
using static System.Runtime.InteropServices.JavaScript.JSType;
namespace VietCommerce.Core.Entities.Marketing
{
    [Table("Promotions")]
    public class Promotion : AuditableEntity , ISoftDelete
    {
        [Required]
        public string PromotionName { get; set; } = string.Empty;
        [Required]
        public PromotionType PromotionType { get; set; }
        [Column(TypeName = "decimal(15,2)")]
        public decimal DiscountValue { get; set; } = 0;
        [Column(TypeName = "decimal(15,2)")]
        public decimal? MinOrderAmount { get; set; }
        [Column(TypeName = "decimal(15,2)")]
        public decimal? MaxDiscount { get; set; }
        public int? UsageLimit { get; set; }
        public int UsedCount { get; set; } = 0;
        [MaxLength(2000)]
        public string? Conditions { get; set; } // JSON conditions
        [Required]
        public DateTime StartDate { get; set; }
        [Required]
        public DateTime EndDate { get; set; }
        // thừa vì ta có thể tính bằng Status + soft-delete (IsDeleted) là đủ để quản lý vòng đời của Promotion.
        public bool IsActive { get; set; }
        [Required]
        public PromotionStatus Status { get; set; } = PromotionStatus.ACTIVE;
        // Navigation
        public Guid CampaignId { get; set; }
        public virtual Campaign Campaign { get; set; } = null!;
        public virtual ICollection<PromotionProduct> PromotionProducts { get; set; } = new List<PromotionProduct>();
        public virtual ICollection<Voucher> Vouchers { get; set; } = new List<Voucher>();
        // 🔹 Thêm các property từ ISoftDelete
        public bool IsDeleted { get; set; } = false;
        public DateTime? DeletedAt { get; set; }
        public Guid? DeletedBy { get; set; }
    }
}
