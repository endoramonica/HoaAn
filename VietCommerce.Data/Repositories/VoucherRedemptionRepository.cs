using Microsoft.EntityFrameworkCore;
using VietCommerce.Core.Entities.Marketing;
using VietCommerce.Data.Context;
using VietCommerce.Data.Repositories.Interfaces;

namespace VietCommerce.Data.Repositories
{
    /// <summary>
    /// Repository implementation for VoucherRedemption entity
    /// Handles all data access operations for voucher redemptions
    /// Requirements: 7.3
    /// </summary>
    public class VoucherRedemptionRepository : GenericRepository<VoucherRedemption>, IVoucherRedemptionRepository
    {
        public VoucherRedemptionRepository(AppDbContext context) : base(context)
        {
        }

        /// <summary>
        /// Get redemptions count for a campaign
        /// </summary>
        public async Task<int> GetRedemptionCountAsync(Guid campaignId)
        {
            return await _context.Set<VoucherRedemption>()
                .Include(x => x.Voucher)
                .Where(x => x.Voucher.Promotion.CampaignId == campaignId)
                .CountAsync();
        }

        /// <summary>
        /// Get redemptions count for a campaign within a date range
        /// </summary>
        public async Task<int> GetRedemptionCountByDateRangeAsync(Guid campaignId, DateTime fromDate, DateTime toDate)
        {
            return await _context.Set<VoucherRedemption>()
                .Include(x => x.Voucher)
                .Where(x => x.Voucher.Promotion.CampaignId == campaignId &&
                           x.RedeemedAt >= fromDate &&
                           x.RedeemedAt <= toDate)
                .CountAsync();
        }

        /// <summary>
        /// Get all redemptions for a campaign
        /// </summary>
        public async Task<IEnumerable<VoucherRedemption>> GetRedemptionsByCampaignAsync(Guid campaignId)
        {
            return await _context.Set<VoucherRedemption>()
                .Include(x => x.Voucher)
                .Where(x => x.Voucher.Promotion.CampaignId == campaignId)
                .OrderByDescending(x => x.RedeemedAt)
                .ToListAsync();
        }

        /// <summary>
        /// Get redemptions for a campaign within a date range
        /// </summary>
        public async Task<IEnumerable<VoucherRedemption>> GetRedemptionsByDateRangeAsync(Guid campaignId, DateTime fromDate, DateTime toDate)
        {
            return await _context.Set<VoucherRedemption>()
                .Include(x => x.Voucher)
                .Where(x => x.Voucher.Promotion.CampaignId == campaignId &&
                           x.RedeemedAt >= fromDate &&
                           x.RedeemedAt <= toDate)
                .OrderByDescending(x => x.RedeemedAt)
                .ToListAsync();
        }

        /// <summary>
        /// Get redemptions for a specific voucher
        /// </summary>
        public async Task<IEnumerable<VoucherRedemption>> GetRedemptionsByVoucherAsync(Guid voucherId)
        {
            return await _context.Set<VoucherRedemption>()
                .Where(x => x.VoucherId == voucherId)
                .OrderByDescending(x => x.RedeemedAt)
                .ToListAsync();
        }
    }
}
