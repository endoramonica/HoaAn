using Microsoft.EntityFrameworkCore;
using VietCommerce.Core.Entities.Marketing;
using VietCommerce.Data.Context;
using VietCommerce.Data.Repositories.Interfaces;

namespace VietCommerce.Data.Repositories
{
    /// <summary>
    /// Repository implementation for CampaignImpression entity
    /// Handles all data access operations for campaign impressions
    /// Requirements: 7.1
    /// </summary>
    public class CampaignImpressionRepository : GenericRepository<CampaignImpression>, ICampaignImpressionRepository
    {
        public CampaignImpressionRepository(AppDbContext context) : base(context)
        {
        }

        /// <summary>
        /// Get impressions count for a campaign
        /// </summary>
        public async Task<int> GetImpressionCountAsync(Guid campaignId)
        {
            return await _context.Set<CampaignImpression>()
                .Where(x => x.CampaignId == campaignId)
                .CountAsync();
        }

        /// <summary>
        /// Get impressions count for a campaign within a date range
        /// </summary>
        public async Task<int> GetImpressionCountByDateRangeAsync(Guid campaignId, DateTime fromDate, DateTime toDate)
        {
            return await _context.Set<CampaignImpression>()
                .Where(x => x.CampaignId == campaignId &&
                           x.RecordedAt >= fromDate &&
                           x.RecordedAt <= toDate)
                .CountAsync();
        }

        /// <summary>
        /// Get all impressions for a campaign
        /// </summary>
        public async Task<IEnumerable<CampaignImpression>> GetImpressionsByCampaignAsync(Guid campaignId)
        {
            return await _context.Set<CampaignImpression>()
                .Where(x => x.CampaignId == campaignId)
                .OrderByDescending(x => x.RecordedAt)
                .ToListAsync();
        }

        /// <summary>
        /// Get impressions for a campaign within a date range
        /// </summary>
        public async Task<IEnumerable<CampaignImpression>> GetImpressionsByDateRangeAsync(Guid campaignId, DateTime fromDate, DateTime toDate)
        {
            return await _context.Set<CampaignImpression>()
                .Where(x => x.CampaignId == campaignId &&
                           x.RecordedAt >= fromDate &&
                           x.RecordedAt <= toDate)
                .OrderByDescending(x => x.RecordedAt)
                .ToListAsync();
        }
    }
}
