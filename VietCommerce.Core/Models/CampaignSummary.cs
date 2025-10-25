using System;
using VietCommerce.Core.Enums.Marketing;
namespace VietCommerce.Core.Models
{
    public class CampaignSummary
    {
        public int CampaignId { get; set; }
        public string? CampaignName { get; set; }
        public CampaignStatus Status { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public decimal Budget { get; set; }
        public decimal ActualCost { get; set; }
        public decimal CostPercentage => Budget > 0 ? (ActualCost / Budget) * 100 : 0;
        public int TotalPromotions { get; set; }
        public int ActivePromotions { get; set; }
        public int TotalOrders { get; set; }
        public decimal TotalRevenue { get; set; }
        public decimal TotalDiscount { get; set; }
        public decimal ROI => ActualCost > 0 ? ((TotalRevenue - ActualCost) / ActualCost) * 100 : 0;
    }
}
