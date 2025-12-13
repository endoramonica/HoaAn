using VietCommerce.Core.Entities.Marketing;
using VietCommerce.Core.Models;

namespace VietCommerce.Data.Repositories.Interfaces
{
    /// <summary>
    /// Repository interface for Voucher entity
    /// Handles all data access operations for vouchers
    /// Requirements: 3.1, 3.2, 3.4, 3.5
    /// </summary>
    public interface IVoucherRepository : IGenericRepository<Voucher>
    {
        /// <summary>
        /// Get voucher by code
        /// Requirements: 3.2
        /// </summary>
        Task<Voucher?> GetByCodeAsync(string code);

        /// <summary>
        /// Get vouchers by promotion ID with pagination
        /// Requirements: 3.1
        /// </summary>
        Task<PaginatedResult<Voucher>> GetByPromotionIdAsync(
            Guid promotionId,
            int pageNumber,
            int pageSize);

        /// <summary>
        /// Check if voucher code exists
        /// Requirements: 3.1
        /// </summary>
        Task<bool> CodeExistsAsync(string code);

        /// <summary>
        /// Get active vouchers for a promotion
        /// Requirements: 3.2
        /// </summary>
        Task<IEnumerable<Voucher>> GetActiveVouchersForPromotionAsync(Guid promotionId);

        /// <summary>
        /// Get expired vouchers
        /// Requirements: 3.4
        /// </summary>
        Task<IEnumerable<Voucher>> GetExpiredVouchersAsync();

        /// <summary>
        /// Check if voucher is valid (not expired, not deleted)
        /// Requirements: 3.2, 3.4
        /// </summary>
        Task<bool> IsVoucherValidAsync(string code);

        /// <summary>
        /// Get vouchers by promotion ID (non-deleted)
        /// Requirements: 3.1
        /// </summary>
        Task<IEnumerable<Voucher>> GetVouchersForPromotionAsync(Guid promotionId);

        /// <summary>
        /// Bulk insert vouchers
        /// Requirements: 3.1
        /// </summary>
        Task<IEnumerable<Voucher>> BulkInsertAsync(IEnumerable<Voucher> vouchers);

        /// <summary>
        /// Validate voucher with detailed error information
        /// Requirements: 3.2, 3.4, 3.5
        /// </summary>
        Task<(bool IsValid, string? ErrorMessage)> ValidateVoucherDetailedAsync(string code);
    }
}
