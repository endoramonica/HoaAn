namespace VietCommerce.Core.DTOs.Marketing
{
    /// <summary>
    /// DTO for tracking campaign impression
    /// Requirements: 7.1
    /// </summary>
    public class TrackImpressionDto
    {
        /// <summary>
        /// Session ID of the user viewing the campaign
        /// </summary>
        public string SessionId { get; set; } = string.Empty;

        /// <summary>
        /// Page where the campaign was shown
        /// </summary>
        public string Page { get; set; } = string.Empty;

        /// <summary>
        /// Timestamp when the impression was recorded
        /// </summary>
        public DateTime? RecordedAt { get; set; }
    }

    /// <summary>
    /// DTO for tracking campaign click
    /// Requirements: 7.2
    /// </summary>
    public class TrackClickDto
    {
        /// <summary>
        /// Session ID of the user clicking the campaign
        /// </summary>
        public string SessionId { get; set; } = string.Empty;

        /// <summary>
        /// Page where the campaign was clicked
        /// </summary>
        public string Page { get; set; } = string.Empty;

        /// <summary>
        /// Timestamp when the click was recorded
        /// </summary>
        public DateTime? RecordedAt { get; set; }
    }

    /// <summary>
    /// DTO for tracking voucher redemption
    /// Requirements: 7.3
    /// </summary>
    public class TrackRedemptionDto
    {
        /// <summary>
        /// Discount amount applied
        /// </summary>
        public decimal DiscountAmount { get; set; }

        /// <summary>
        /// Order ID if redemption is linked to an order
        /// </summary>
        public Guid? OrderId { get; set; }

        /// <summary>
        /// User ID who redeemed the voucher
        /// </summary>
        public Guid? RedeemedBy { get; set; }

        /// <summary>
        /// Timestamp when the redemption was recorded
        /// </summary>
        public DateTime? RedeemedAt { get; set; }
    }

    /// <summary>
    /// DTO for campaign statistics query parameters
    /// Requirements: 7.4, 7.5
    /// </summary>
    public class GetCampaignStatsQueryDto
    {
        /// <summary>
        /// Start date for filtering statistics
        /// </summary>
        public DateTime? FromDate { get; set; }

        /// <summary>
        /// End date for filtering statistics
        /// </summary>
        public DateTime? ToDate { get; set; }
    }

    /// <summary>
    /// DTO for campaign statistics response
    /// Requirements: 7.4, 7.5
    /// </summary>
    public class CampaignStatsDto
    {
        /// <summary>
        /// Campaign ID
        /// </summary>
        public Guid CampaignId { get; set; }

        /// <summary>
        /// Total number of impressions
        /// </summary>
        public int ImpressionCount { get; set; }

        /// <summary>
        /// Total number of clicks
        /// </summary>
        public int ClickCount { get; set; }

        /// <summary>
        /// Click-through rate (clicks / impressions * 100)
        /// </summary>
        public double ClickThroughRate { get; set; }

        /// <summary>
        /// Total number of redemptions
        /// </summary>
        public int RedemptionCount { get; set; }

        /// <summary>
        /// Redemption rate (redemptions / clicks * 100)
        /// </summary>
        public double RedemptionRate { get; set; }

        /// <summary>
        /// Total revenue from redemptions
        /// </summary>
        public decimal TotalRevenue { get; set; }

        /// <summary>
        /// Total discount amount
        /// </summary>
        public decimal TotalDiscount { get; set; }

        /// <summary>
        /// Conversion rate (redemptions / impressions * 100)
        /// </summary>
        public double ConversionRate { get; set; }

        /// <summary>
        /// Date range start for the statistics
        /// </summary>
        public DateTime? FromDate { get; set; }

        /// <summary>
        /// Date range end for the statistics
        /// </summary>
        public DateTime? ToDate { get; set; }
    }
}
