using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using VietCommerce.Core.Common;
using VietCommerce.Core.Entities.Organization;
using VietCommerce.Core.Entities.Users;
using VietCommerce.Core.Enums.Marketing;
namespace VietCommerce.Core.Entities.Marketing
{
    [Table("Campaigns")]
    public class Campaign : AuditableEntity
    {
        public Guid StoreId { get; set; }
        [Required, MaxLength(255)]
        public string CampaignName { get; set; } = string.Empty;
        [MaxLength(1000)]
        public string Description { get; set; } = string.Empty;
        [Required]
        public CampaignType CampaignType { get; set; }
        [Required]
        public DateTime StartDate { get; set; }
        [Required]
        public DateTime EndDate { get; set; }
        [Column(TypeName = "decimal(15,2)")]
        [Range(0, double.MaxValue, ErrorMessage = "Budget must be non-negative")]
        public decimal Budget { get; set; } = 0;
        [Column(TypeName = "decimal(15,2)")]
        [Range(0, double.MaxValue, ErrorMessage = "ActualCost must be non-negative")]
        public decimal ActualCost { get; set; } = 0;
        [Required]
        public CampaignStatus Status { get; set; } = CampaignStatus.DRAFT;
        
        // Targeting rules (stored as JSON)
        [MaxLength(2000)]
        public string TargetingRules { get; set; } = string.Empty;
        
        // Navigation properties
        public virtual Store Store { get; set; } = null!;
        [ForeignKey(nameof(CreatedBy))]
        public virtual User Creator { get; set; } = null!;
        public virtual ICollection<Promotion> Promotions { get; set; } = new List<Promotion>();
        public virtual ICollection<CampaignImpression> Impressions { get; set; } = new List<CampaignImpression>();
        public virtual ICollection<CampaignClick> Clicks { get; set; } = new List<CampaignClick>();
        // Computed properties
        [NotMapped]
        public bool IsActive => Status == CampaignStatus.ACTIVE &&
                               StartDate <= DateTime.UtcNow &&
                               EndDate >= DateTime.UtcNow;
        [NotMapped]
        public bool IsValidDateRange => EndDate > StartDate;
        [NotMapped]
        public decimal RemainingBudget => Budget - ActualCost;
        [NotMapped]
        public double BudgetUtilizationPercentage => Budget > 0 ? (double)(ActualCost / Budget * 100) : 0;
    }
}
