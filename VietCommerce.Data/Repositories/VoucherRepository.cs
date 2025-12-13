using Microsoft.EntityFrameworkCore;
using VietCommerce.Core.Entities.Marketing;
using VietCommerce.Core.Models;
using VietCommerce.Data.Context;
using VietCommerce.Data.Repositories.Interfaces;

namespace VietCommerce.Data.Repositories
{
    /// <summary>
    /// Repository implementation for Voucher entity
    /// Handles all data access operations for vouchers
    /// Requirements: 3.1, 3.2, 3.4, 3.5
    /// </summary>
    public class VoucherRepository : GenericRepository<Voucher>, IVoucherRepository
    {
        public VoucherRepository(AppDbContext context) : base(context) { }

        /// <summary>
        /// Get voucher by code
        /// Requirements: 3.2
        /// </summary>
        public async Task<Voucher?> GetByCodeAsync(string code)
        {
            return await _dbSet
                .Include(v => v.Promotion)
                .FirstOrDefaultAsync(v => v.Code == code && !v.IsDeleted);
        }

        /// <summary>
        /// Get vouchers by promotion ID with pagination
        /// Requirements: 3.1
        /// </summary>
        public async Task<PaginatedResult<Voucher>> GetByPromotionIdAsync(
            Guid promotionId,
            int pageNumber,
            int pageSize)
        {
            var query = _dbSet
                .Where(v => v.PromotionId == promotionId && !v.IsDeleted)
                .Include(v => v.Promotion)
                .OrderByDescending(v => v.CreatedAt);

            var totalCount = await query.CountAsync();

            var vouchers = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PaginatedResult<Voucher>
            {
                Items = vouchers,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalItems = totalCount,
                TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize)
            };
        }

        /// <summary>
        /// Check if voucher code exists
        /// Requirements: 3.1
        /// </summary>
        public async Task<bool> CodeExistsAsync(string code)
        {
            return await _dbSet.AnyAsync(v => v.Code == code && !v.IsDeleted);
        }

        /// <summary>
        /// Get active vouchers for a promotion
        /// Requirements: 3.2
        /// </summary>
        public async Task<IEnumerable<Voucher>> GetActiveVouchersForPromotionAsync(Guid promotionId)
        {
            var now = DateTime.UtcNow;
            return await _dbSet
                .Where(v => v.PromotionId == promotionId &&
                           !v.IsDeleted &&
                           v.IsActive &&
                           v.ExpiryDate > now)
                .Include(v => v.Promotion)
                .OrderByDescending(v => v.CreatedAt)
                .ToListAsync();
        }

        /// <summary>
        /// Get expired vouchers
        /// Requirements: 3.4
        /// </summary>
        public async Task<IEnumerable<Voucher>> GetExpiredVouchersAsync()
        {
            var now = DateTime.UtcNow;
            return await _dbSet
                .Where(v => v.ExpiryDate <= now && !v.IsDeleted)
                .OrderByDescending(v => v.ExpiryDate)
                .ToListAsync();
        }

        /// <summary>
        /// Check if voucher is valid (not expired, not deleted, usage limit not exceeded, promotion is active)
        /// Requirements: 3.2, 3.4, 3.5
        /// </summary>
        public async Task<bool> IsVoucherValidAsync(string code)
        {
            var now = DateTime.UtcNow;
            return await _dbSet.AnyAsync(v =>
                v.Code == code &&
                !v.IsDeleted &&
                v.IsActive &&
                v.ExpiryDate > now &&
                (v.Promotion.UsageLimit == null || v.UsageCount < v.Promotion.UsageLimit) &&
                v.Promotion.Status == VietCommerce.Core.Enums.Marketing.PromotionStatus.ACTIVE);
        }

        /// <summary>
        /// Get vouchers by promotion ID (non-deleted)
        /// Requirements: 3.1
        /// </summary>
        public async Task<IEnumerable<Voucher>> GetVouchersForPromotionAsync(Guid promotionId)
        {
            return await _dbSet
                .Where(v => v.PromotionId == promotionId && !v.IsDeleted)
                .Include(v => v.Promotion)
                .OrderByDescending(v => v.CreatedAt)
                .ToListAsync();
        }

        /// <summary>
        /// Bulk insert vouchers
        /// Requirements: 3.1
        /// </summary>
        public async Task<IEnumerable<Voucher>> BulkInsertAsync(IEnumerable<Voucher> vouchers)
        {
            await _dbSet.AddRangeAsync(vouchers);
            return vouchers;
        }

        /// <summary>
        /// Validate voucher with detailed error information
        /// Requirements: 3.2, 3.4, 3.5
        /// </summary>
        public async Task<(bool IsValid, string? ErrorMessage)> ValidateVoucherDetailedAsync(string code)
        {
            var now = DateTime.UtcNow;
            var voucher = await _dbSet
                .Include(v => v.Promotion)
                .FirstOrDefaultAsync(v => v.Code == code);

            // Check if voucher exists
            if (voucher == null)
                return (false, "Voucher code not found");

            // Check if voucher is deleted
            if (voucher.IsDeleted)
                return (false, "Voucher has been deleted");

            // Check if voucher is active
            if (!voucher.IsActive)
                return (false, "Voucher is not active");

            // Check if voucher is expired
            if (voucher.ExpiryDate <= now)
                return (false, "Voucher has expired");

            // Check if promotion is active
            if (voucher.Promotion.Status != VietCommerce.Core.Enums.Marketing.PromotionStatus.ACTIVE)
                return (false, "Associated promotion is not active");

            // Check if usage limit is reached
            if (voucher.Promotion.UsageLimit.HasValue && voucher.UsageCount >= voucher.Promotion.UsageLimit.Value)
                return (false, "Voucher usage limit has been reached");

            return (true, null);
        }
    }
}
