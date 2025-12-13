using VietCommerce.Core.DTOs.Marketing;
using VietCommerce.Core.Models;

namespace VietCommerce.Application.Services.Services.Interfaces
{
    /// <summary>
    /// Service interface for Campaign Analytics
    /// Handles tracking and reporting of campaign performance metrics
    /// Requirements: 7.1, 7.2, 7.3, 7.4, 7.5
    /// </summary>
    public interface IAnalyticsService
    {
        /// <summary>
        /// Track a campaign impression (when campaign is shown to user)
        /// Requirements: 7.1
        /// </summary>
        Task<ApiResponse<bool>> TrackImpressionAsync(Guid campaignId, TrackImpressionDto dto);

        /// <summary>
        /// Track a campaign click (when user clicks on campaign)
        /// Requirements: 7.2
        /// </summary>
        Task<ApiResponse<bool>> TrackClickAsync(Guid campaignId, TrackClickDto dto);

        /// <summary>
        /// Track a voucher redemption (when voucher is applied to cart)
        /// Requirements: 7.3
        /// </summary>
        Task<ApiResponse<bool>> TrackRedemptionAsync(Guid voucherId, TrackRedemptionDto dto);

        /// <summary>
        /// Get campaign statistics including impressions, clicks, and redemptions
        /// Requirements: 7.4, 7.5
        /// </summary>
        Task<ApiResponse<CampaignStatsDto>> GetCampaignStatsAsync(Guid campaignId, GetCampaignStatsQueryDto query);
    }
}
