using Microsoft.EntityFrameworkCore;
using VietCommerce.Core.Entities.Marketing;
using VietCommerce.Core.Enums.Marketing;
using VietCommerce.Core.Models;
using VietCommerce.Data.Context;
using VietCommerce.Data.Repositories.Interfaces;

namespace VietCommerce.Data.Repositories
{
    /// <summary>
    /// Repository implementation for Campaign entity
    /// Handles all data access operations for campaigns
    /// </summary>
    public class CampaignRepository : GenericRepository<Campaign>, ICampaignRepository
    {
        public CampaignRepository(AppDbContext context) : base(context) { }

        /// <summary>
        /// Get campaigns by store ID with pagination and filtering
        /// Requirements: 1.5
        /// </summary>
        public async Task<PaginatedResult<Campaign>> GetCampaignsByStoreAsync(
            Guid storeId,
            int pageNumber,
            int pageSize,
            CampaignStatus? status = null,
            DateTime? fromDate = null,
            DateTime? toDate = null,
            string? searchTerm = null,
            string? sortBy = null,
            bool isDescending = false)
        {
            var query = _dbSet
                .Where(c => c.StoreId == storeId)
                .Include(c => c.Promotions)
                .Include(c => c.Impressions)
                .Include(c => c.Clicks)
                .AsQueryable();

            // Filter by status
            if (status.HasValue)
            {
                query = query.Where(c => c.Status == status.Value);
            }

            // Filter by date range
            if (fromDate.HasValue)
            {
                query = query.Where(c => c.StartDate >= fromDate.Value);
            }

            if (toDate.HasValue)
            {
                query = query.Where(c => c.EndDate <= toDate.Value);
            }

            // Search by campaign name or description
            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                query = query.Where(c =>
                    c.CampaignName.Contains(searchTerm) ||
                    c.Description.Contains(searchTerm));
            }

            // Get total count before pagination
            var totalCount = await query.CountAsync();

            // Apply sorting
            query = ApplySorting(query, sortBy, isDescending);

            // Apply pagination
            var campaigns = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PaginatedResult<Campaign>
            {
                Items = campaigns,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalItems = totalCount,
                TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize)
            };
        }

        /// <summary>
        /// Get active campaigns for a store
        /// </summary>
        public async Task<IEnumerable<Campaign>> GetActiveCampaignsAsync(Guid storeId)
        {
            var now = DateTime.UtcNow;
            return await _dbSet
                .Where(c => c.StoreId == storeId &&
                           c.Status == CampaignStatus.ACTIVE &&
                           c.StartDate <= now &&
                           c.EndDate >= now)
                .Include(c => c.Promotions)
                .OrderByDescending(c => c.CreatedAt)
                .ToListAsync();
        }

        /// <summary>
        /// Get campaigns by status
        /// </summary>
        public async Task<IEnumerable<Campaign>> GetCampaignsByStatusAsync(Guid storeId, CampaignStatus status)
        {
            return await _dbSet
                .Where(c => c.StoreId == storeId && c.Status == status)
                .Include(c => c.Promotions)
                .OrderByDescending(c => c.CreatedAt)
                .ToListAsync();
        }

        /// <summary>
        /// Check if campaign exists by ID
        /// </summary>
        public async Task<bool> CampaignExistsAsync(Guid id)
        {
            return await _dbSet.AnyAsync(c => c.Id == id);
        }

        /// <summary>
        /// Get campaign with all related data (promotions, impressions, clicks)
        /// </summary>
        public async Task<Campaign?> GetCampaignWithDetailsAsync(Guid id)
        {
            return await _dbSet
                .Include(c => c.Promotions)
                    .ThenInclude(p => p.PromotionProducts)
                .Include(c => c.Impressions)
                .Include(c => c.Clicks)
                .FirstOrDefaultAsync(c => c.Id == id);
        }

        /// <summary>
        /// Helper method to apply sorting
        /// </summary>
        private IQueryable<Campaign> ApplySorting(IQueryable<Campaign> query, string? sortBy, bool isDescending)
        {
            return (sortBy?.ToLower()) switch
            {
                "name" => isDescending
                    ? query.OrderByDescending(c => c.CampaignName)
                    : query.OrderBy(c => c.CampaignName),
                "startdate" => isDescending
                    ? query.OrderByDescending(c => c.StartDate)
                    : query.OrderBy(c => c.StartDate),
                "enddate" => isDescending
                    ? query.OrderByDescending(c => c.EndDate)
                    : query.OrderBy(c => c.EndDate),
                "budget" => isDescending
                    ? query.OrderByDescending(c => c.Budget)
                    : query.OrderBy(c => c.Budget),
                "status" => isDescending
                    ? query.OrderByDescending(c => c.Status)
                    : query.OrderBy(c => c.Status),
                _ => isDescending
                    ? query.OrderByDescending(c => c.CreatedAt)
                    : query.OrderBy(c => c.CreatedAt)
            };
        }
    }
}
