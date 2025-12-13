using VietCommerce.Core.Entities.Marketing;

namespace VietCommerce.Data.Repositories.Interfaces
{
    /// <summary>
    /// Repository interface for CampaignImpression entity
    /// Handles all data access operations for campaign impressions
    /// Requirements: 7.1
    /// </summary>
    public interface ICampaignImpressionRepository : IGenericRepository<CampaignImpression>
    {
        /// <summary>
        /// Get impressions count for a campaign
        /// </summary>
        Task<int> GetImpressionCountAsync(Guid campaignId);

        /// <summary>
        /// Get impressions count for a campaign within a date range
        /// </summary>
        Task<int> GetImpressionCountByDateRangeAsync(Guid campaignId, DateTime fromDate, DateTime toDate);

        /// <summary>
        /// Get all impressions for a campaign
        /// </summary>
        Task<IEnumerable<CampaignImpression>> GetImpressionsByCampaignAsync(Guid campaignId);

        /// <summary>
        /// Get impressions for a campaign within a date range
        /// </summary>
        Task<IEnumerable<CampaignImpression>> GetImpressionsByDateRangeAsync(Guid campaignId, DateTime fromDate, DateTime toDate);
    }
}
