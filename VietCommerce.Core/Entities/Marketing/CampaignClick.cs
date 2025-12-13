using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using VietCommerce.Core.Common;

namespace VietCommerce.Core.Entities.Marketing
{
    [Table("CampaignClicks")]
    public class CampaignClick : BaseEntity
    {
        public Guid CampaignId { get; set; }

        [MaxLength(100)]
        public string SessionId { get; set; } = string.Empty;

        [MaxLength(100)]
        public string Page { get; set; } = string.Empty;

        public DateTime RecordedAt { get; set; } = DateTime.UtcNow;

        // Navigation
        public virtual Campaign Campaign { get; set; } = null!;
    }
}
