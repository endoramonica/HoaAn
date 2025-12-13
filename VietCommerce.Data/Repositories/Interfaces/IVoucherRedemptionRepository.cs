using VietCommerce.Core.Entities.Marketing;

namespace VietCommerce.Data.Repositories.Interfaces
{
    /// <summary>
    /// Repository interface for VoucherRedemption entity
    /// Handles all data access operations for voucher redemptions
    /// Requirements: 7.3
    /// </summary>
    public interface IVoucherRedemptionRepository : IGenericRepository<VoucherRedemption>
    {
        /// <summary>
        /// Get redemptions count for a campaign
        /// </summary>
        Task<int> GetRedemptionCountAsync(Guid campaignId);

        /// <summary>
        /// Get redemptions count for a campaign within a date range
        /// </summary>
        Task<int> GetRedemptionCountByDateRangeAsync(Guid campaignId, DateTime fromDate, DateTime toDate);

        /// <summary>
        /// Get all redemptions for a campaign
        /// </summary>
        Task<IEnumerable<VoucherRedemption>> GetRedemptionsByCampaignAsync(Guid campaignId);

        /// <summary>
        /// Get redemptions for a campaign within a date range
        /// </summary>
        Task<IEnumerable<VoucherRedemption>> GetRedemptionsByDateRangeAsync(Guid campaignId, DateTime fromDate, DateTime toDate);

        /// <summary>
        /// Get redemptions for a specific voucher
        /// </summary>
        Task<IEnumerable<VoucherRedemption>> GetRedemptionsByVoucherAsync(Guid voucherId);
    }
}
