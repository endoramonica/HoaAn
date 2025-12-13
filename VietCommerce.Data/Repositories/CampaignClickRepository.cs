using Microsoft.EntityFrameworkCore;
using VietCommerce.Core.Entities.Marketing;
using VietCommerce.Data.Context;
using VietCommerce.Data.Repositories.Interfaces;

namespace VietCommerce.Data.Repositories
{
    /// <summary>
    /// Repository implementation for CampaignClick entity
    /// Handles all data access operations for campaign clicks
    /// Requirements: 7.2
    /// </summary>
    public class CampaignClickRepository : GenericRepository<CampaignClick>, ICampaignClickRepository
    {
        public CampaignClickRepository(AppDbContext context) : base(context)
        {
        }

        /// <summary>
        /// Get clicks count for a campaign
        /// </summary>
        public async Task<int> GetClickCountAsync(Guid campaignId)
        {
            return await _context.Set<CampaignClick>()
                .Where(x => x.CampaignId == campaignId)
                .CountAsync();
        }

        /// <summary>
        /// Get clicks count for a campaign within a date range
        /// </summary>
        public async Task<int> GetClickCountByDateRangeAsync(Guid campaignId, DateTime fromDate, DateTime toDate)
        {
            return await _context.Set<CampaignClick>()
                .Where(x => x.CampaignId == campaignId &&
                           x.RecordedAt >= fromDate &&
                           x.RecordedAt <= toDate)
                .CountAsync();
        }

        /// <summary>
        /// Get all clicks for a campaign
        /// </summary>
        public async Task<IEnumerable<CampaignClick>> GetClicksByCampaignAsync(Guid campaignId)
        {
            return await _context.Set<CampaignClick>()
                .Where(x => x.CampaignId == campaignId)
                .OrderByDescending(x => x.RecordedAt)
                .ToListAsync();
        }

        /// <summary>
        /// Get clicks for a campaign within a date range
        /// </summary>
        public async Task<IEnumerable<CampaignClick>> GetClicksByDateRangeAsync(Guid campaignId, DateTime fromDate, DateTime toDate)
        {
            return await _context.Set<CampaignClick>()
                .Where(x => x.CampaignId == campaignId &&
                           x.RecordedAt >= fromDate &&
                           x.RecordedAt <= toDate)
                .OrderByDescending(x => x.RecordedAt)
                .ToListAsync();
        }
    }
}
