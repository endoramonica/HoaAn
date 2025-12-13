using Microsoft.EntityFrameworkCore;
using VietCommerce.Core.Entities.Marketing;
using VietCommerce.Core.Enums.Marketing;
using VietCommerce.Core.Models;
using VietCommerce.Data.Context;
using VietCommerce.Data.Repositories.Interfaces;

namespace VietCommerce.Data.Repositories
{
    /// <summary>
    /// Repository implementation for Promotion entity
    /// Handles all data access operations for promotions
    /// Requirements: 2.1, 2.2, 2.3, 2.4, 2.5
    /// </summary>
    public class PromotionRepository : GenericRepository<Promotion>, IPromotionRepository
    {
        public PromotionRepository(AppDbContext context) : base(context) { }

        /// <summary>
        /// Get promotions by campaign ID with pagination and filtering
        /// Requirements: 2.5
        /// </summary>
        public async Task<PaginatedResult<Promotion>> GetPromotionsByCampaignAsync(
            Guid campaignId,
            int pageNumber,
            int pageSize,
            PromotionStatus? status = null,
            DateTime? fromDate = null,
            DateTime? toDate = null,
            string? searchTerm = null,
            string? sortBy = null,
            bool isDescending = false)
        {
            var query = _dbSet
                .Where(p => p.CampaignId == campaignId && !p.IsDeleted)
                .Include(p => p.PromotionProducts)
                .Include(p => p.Vouchers)
                .AsQueryable();

            // Filter by status
            if (status.HasValue)
            {
                query = query.Where(p => p.Status == status.Value);
            }

            // Filter by date range
            if (fromDate.HasValue)
            {
                query = query.Where(p => p.StartDate >= fromDate.Value);
            }

            if (toDate.HasValue)
            {
                query = query.Where(p => p.EndDate <= toDate.Value);
            }

            // Search by promotion name
            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                query = query.Where(p => p.PromotionName.Contains(searchTerm));
            }

            // Get total count before pagination
            var totalCount = await query.CountAsync();

            // Apply sorting
            query = ApplySorting(query, sortBy, isDescending);

            // Apply pagination
            var promotions = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PaginatedResult<Promotion>
            {
                Items = promotions,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalItems = totalCount,
                TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize)
            };
        }

        /// <summary>
        /// Get active promotions for a campaign
        /// </summary>
        public async Task<IEnumerable<Promotion>> GetActivePromotionsByCampaignAsync(Guid campaignId)
        {
            var now = DateTime.UtcNow;
            return await _dbSet
                .Where(p => p.CampaignId == campaignId &&
                           !p.IsDeleted &&
                           p.Status == PromotionStatus.ACTIVE &&
                           p.StartDate <= now &&
                           p.EndDate >= now)
                .Include(p => p.PromotionProducts)
                .OrderByDescending(p => p.CreatedAt)
                .ToListAsync();
        }

        /// <summary>
        /// Get promotions by status
        /// </summary>
        public async Task<IEnumerable<Promotion>> GetPromotionsByStatusAsync(Guid campaignId, PromotionStatus status)
        {
            return await _dbSet
                .Where(p => p.CampaignId == campaignId && p.Status == status && !p.IsDeleted)
                .Include(p => p.PromotionProducts)
                .OrderByDescending(p => p.CreatedAt)
                .ToListAsync();
        }

        /// <summary>
        /// Check if promotion exists by ID
        /// </summary>
        public async Task<bool> PromotionExistsAsync(Guid id)
        {
            return await _dbSet.AnyAsync(p => p.Id == id && !p.IsDeleted);
        }

        /// <summary>
        /// Get promotion with all related data (products, vouchers)
        /// </summary>
        public async Task<Promotion?> GetPromotionWithDetailsAsync(Guid id)
        {
            return await _dbSet
                .Include(p => p.PromotionProducts)
                    .ThenInclude(pp => pp.Product)
                .Include(p => p.Vouchers)
                .FirstOrDefaultAsync(p => p.Id == id && !p.IsDeleted);
        }

        /// <summary>
        /// Get promotions for a campaign (non-deleted)
        /// </summary>
        public async Task<IEnumerable<Promotion>> GetPromotionsForCampaignAsync(Guid campaignId)
        {
            return await _dbSet
                .Where(p => p.CampaignId == campaignId && !p.IsDeleted)
                .Include(p => p.PromotionProducts)
                .OrderByDescending(p => p.CreatedAt)
                .ToListAsync();
        }

        /// <summary>
        /// Check if promotion exists for a campaign
        /// </summary>
        public async Task<bool> PromotionExistsForCampaignAsync(Guid campaignId, Guid promotionId)
        {
            return await _dbSet.AnyAsync(p => p.CampaignId == campaignId && p.Id == promotionId && !p.IsDeleted);
        }

        /// <summary>
        /// Helper method to apply sorting
        /// </summary>
        private IQueryable<Promotion> ApplySorting(IQueryable<Promotion> query, string? sortBy, bool isDescending)
        {
            return (sortBy?.ToLower()) switch
            {
                "name" => isDescending
                    ? query.OrderByDescending(p => p.PromotionName)
                    : query.OrderBy(p => p.PromotionName),
                "startdate" => isDescending
                    ? query.OrderByDescending(p => p.StartDate)
                    : query.OrderBy(p => p.StartDate),
                "enddate" => isDescending
                    ? query.OrderByDescending(p => p.EndDate)
                    : query.OrderBy(p => p.EndDate),
                "discountvalue" => isDescending
                    ? query.OrderByDescending(p => p.DiscountValue)
                    : query.OrderBy(p => p.DiscountValue),
                "status" => isDescending
                    ? query.OrderByDescending(p => p.Status)
                    : query.OrderBy(p => p.Status),
                _ => isDescending
                    ? query.OrderByDescending(p => p.CreatedAt)
                    : query.OrderBy(p => p.CreatedAt)
            };
        }
    }
}
