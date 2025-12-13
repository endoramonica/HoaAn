using VietCommerce.Core.Entities.Marketing;

namespace VietCommerce.Data.Repositories.Interfaces
{
    /// <summary>
    /// Repository interface for CampaignClick entity
    /// Handles all data access operations for campaign clicks
    /// Requirements: 7.2
    /// </summary>
    public interface ICampaignClickRepository : IGenericRepository<CampaignClick>
    {
        /// <summary>
        /// Get clicks count for a campaign
        /// </summary>
        Task<int> GetClickCountAsync(Guid campaignId);

        /// <summary>
        /// Get clicks count for a campaign within a date range
        /// </summary>
        Task<int> GetClickCountByDateRangeAsync(Guid campaignId, DateTime fromDate, DateTime toDate);

        /// <summary>
        /// Get all clicks for a campaign
        /// </summary>
        Task<IEnumerable<CampaignClick>> GetClicksByCampaignAsync(Guid campaignId);

        /// <summary>
        /// Get clicks for a campaign within a date range
        /// </summary>
        Task<IEnumerable<CampaignClick>> GetClicksByDateRangeAsync(Guid campaignId, DateTime fromDate, DateTime toDate);
    }
}
