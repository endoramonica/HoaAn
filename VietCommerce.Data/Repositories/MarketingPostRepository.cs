using Microsoft.EntityFrameworkCore;
using VietCommerce.Core.DTOs.Marketing;
using VietCommerce.Core.Entities.Marketing;
using VietCommerce.Core.Enums.Marketing;
using VietCommerce.Core.Models;
using VietCommerce.Data.Context;
using VietCommerce.Data.Repositories.Interfaces;

namespace VietCommerce.Data.Repositories;

/// <summary>
/// Repository implementation for Marketing Post operations
/// Extends GenericRepository and provides specialized methods for marketing posts
/// </summary>
public class MarketingPostRepository : GenericRepository<MarketingPost>, IMarketingPostRepository
{
    public MarketingPostRepository(AppDbContext context) : base(context)
    {
    }

    /// <summary>
    /// Get paginated posts with advanced filtering, search, and sorting
    /// Supports status, product, platform, search term, date range, priority, and display location filters
    /// </summary>
    public async Task<PaginatedResult<MarketingPost>> GetPostsAsync(
        int pageNumber,
        int pageSize,
        MarketingPostStatus? status = null,
        Guid? productId = null,
        string? platform = null,
        string? searchTerm = null,
        string? displayLocation = null,
        bool? isFeatured = null,
        int? minPriorityScore = null,
        DateTime? fromDate = null,
        DateTime? toDate = null,
        string sortBy = "priority",
        string sortOrder = "desc",
        bool includeDeleted = false)
    {
        // Start with base query
        var query = includeDeleted
            ? _dbSet.IgnoreQueryFilters().AsQueryable()
            : _dbSet.AsQueryable();

        // Include related Product entity for denormalized data
        query = query.Include(p => p.Product);

        // Apply filters
        if (status.HasValue)
        {
            query = query.Where(p => p.Status == status.Value);
        }

        if (productId.HasValue)
        {
            query = query.Where(p => p.ProductId == productId.Value);
        }

        if (!string.IsNullOrWhiteSpace(platform))
        {
            query = query.Where(p => p.Platform == platform);
        }

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            var search = searchTerm.ToLower();
            query = query.Where(p =>
                p.Title.ToLower().Contains(search) ||
                p.Content.ToLower().Contains(search) ||
                (p.ProductName != null && p.ProductName.ToLower().Contains(search)));
        }

        if (!string.IsNullOrWhiteSpace(displayLocation))
        {
            query = query.Where(p => p.DisplayLocation != null && p.DisplayLocation.Contains(displayLocation));
        }

        if (isFeatured.HasValue)
        {
            query = query.Where(p => p.IsFeatured == isFeatured.Value);
        }

        if (minPriorityScore.HasValue)
        {
            query = query.Where(p => p.PriorityScore >= minPriorityScore.Value);
        }

        if (fromDate.HasValue)
        {
            query = query.Where(p => p.CreatedAt >= fromDate.Value);
        }

        if (toDate.HasValue)
        {
            query = query.Where(p => p.CreatedAt <= toDate.Value);
        }

        // Apply sorting
        var isDescending = sortOrder.ToLower() == "desc";
        query = sortBy.ToLower() switch
        {
            "priority" => isDescending
                ? query.OrderByDescending(p => p.PriorityScore).ThenByDescending(p => p.PublishedDate)
                : query.OrderBy(p => p.PriorityScore).ThenBy(p => p.PublishedDate),
            "publisheddate" => isDescending
                ? query.OrderByDescending(p => p.PublishedDate)
                : query.OrderBy(p => p.PublishedDate),
            "views" => isDescending
                ? query.OrderByDescending(p => p.Views)
                : query.OrderBy(p => p.Views),
            "clicks" => isDescending
                ? query.OrderByDescending(p => p.Clicks)
                : query.OrderBy(p => p.Clicks),
            "shares" => isDescending
                ? query.OrderByDescending(p => p.Shares)
                : query.OrderBy(p => p.Shares),
            "updatedat" => isDescending
                ? query.OrderByDescending(p => p.UpdatedAt)
                : query.OrderBy(p => p.UpdatedAt),
            _ => isDescending
                ? query.OrderByDescending(p => p.PriorityScore).ThenByDescending(p => p.PublishedDate)
                : query.OrderBy(p => p.PriorityScore).ThenBy(p => p.PublishedDate)
        };

        // Get total count
        var totalItems = await query.CountAsync();

        // Apply pagination
        var items = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return new PaginatedResult<MarketingPost>(items, pageNumber, pageSize, totalItems);
    }

    /// <summary>
    /// Get post by ID with option to include soft-deleted posts
    /// </summary>
    public async Task<MarketingPost?> GetByIdAsync(Guid id, bool includeDeleted = false)
    {
        var query = includeDeleted
            ? _dbSet.IgnoreQueryFilters()
            : _dbSet;

        return await query
            .Include(p => p.Product)
            .FirstOrDefaultAsync(p => p.Id == id);
    }

    /// <summary>
    /// Create a new marketing post
    /// </summary>
    public async Task<MarketingPost> CreateAsync(MarketingPost post)
    {
        var result = await _dbSet.AddAsync(post);
        await _context.SaveChangesAsync();
        return result.Entity;
    }

    /// <summary>
    /// Update an existing marketing post
    /// </summary>
    public async Task<MarketingPost> UpdateAsync(MarketingPost post)
    {
        _dbSet.Update(post);
        await _context.SaveChangesAsync();
        return post;
    }

    /// <summary>
    /// Soft delete a marketing post
    /// Sets IsDeleted flag and preserves all data
    /// </summary>
    public async Task<bool> DeleteAsync(Guid id, Guid deletedBy)
    {
        var post = await _dbSet.IgnoreQueryFilters()
            .FirstOrDefaultAsync(p => p.Id == id);

        if (post == null)
            return false;

        post.IsDeleted = true;
        post.DeletedAt = DateTime.UtcNow;
        post.DeletedBy = deletedBy;
        post.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        return true;
    }

    /// <summary>
    /// Restore a soft-deleted marketing post
    /// Clears IsDeleted flag and related fields
    /// </summary>
    public async Task<bool> RestoreAsync(Guid id)
    {
        var post = await _dbSet.IgnoreQueryFilters()
            .FirstOrDefaultAsync(p => p.Id == id && p.IsDeleted);

        if (post == null)
            return false;

        post.IsDeleted = false;
        post.DeletedAt = null;
        post.DeletedBy = null;
        post.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        return true;
    }

    /// <summary>
    /// Get all posts linked to a specific product
    /// Excludes soft-deleted posts
    /// </summary>
    public async Task<List<MarketingPost>> GetByProductIdAsync(Guid productId)
    {
        return await _dbSet
            .Where(p => p.ProductId == productId)
            .OrderByDescending(p => p.CreatedAt)
            .ToListAsync();
    }

    /// <summary>
    /// Get aggregate statistics for all marketing posts
    /// Excludes soft-deleted posts
    /// </summary>
    public async Task<MarketingPostStatisticsDto> GetStatisticsAsync()
    {
        var posts = await _dbSet.ToListAsync();

        var total = posts.Count;
        var draft = posts.Count(p => p.Status == MarketingPostStatus.Draft);
        var published = posts.Count(p => p.Status == MarketingPostStatus.Published);
        var scheduled = posts.Count(p => p.Status == MarketingPostStatus.Scheduled);

        var totalViews = posts.Sum(p => p.Views);
        var totalClicks = posts.Sum(p => p.Clicks);
        var totalShares = posts.Sum(p => p.Shares);

        return new MarketingPostStatisticsDto
        {
            Total = total,
            Draft = draft,
            Published = published,
            Scheduled = scheduled,
            TotalViews = totalViews,
            TotalClicks = totalClicks,
            TotalShares = totalShares,
            AverageViews = total > 0 ? (double)totalViews / total : 0,
            AverageClicks = total > 0 ? (double)totalClicks / total : 0,
            AverageShares = total > 0 ? (double)totalShares / total : 0
        };
    }

    /// <summary>
    /// Atomically increment the views counter for a post
    /// Uses raw SQL for atomic operation to prevent race conditions
    /// </summary>
    public async Task<bool> IncrementViewsAsync(Guid id)
    {
        var rowsAffected = await _context.Database.ExecuteSqlRawAsync(
            "UPDATE MarketingPosts SET Views = Views + 1, UpdatedAt = {0} WHERE Id = {1} AND IsDeleted = 0",
            DateTime.UtcNow, id);

        return rowsAffected > 0;
    }

    /// <summary>
    /// Atomically increment the clicks counter for a post
    /// Uses raw SQL for atomic operation to prevent race conditions
    /// </summary>
    public async Task<bool> IncrementClicksAsync(Guid id)
    {
        var rowsAffected = await _context.Database.ExecuteSqlRawAsync(
            "UPDATE MarketingPosts SET Clicks = Clicks + 1, UpdatedAt = {0} WHERE Id = {1} AND IsDeleted = 0",
            DateTime.UtcNow, id);

        return rowsAffected > 0;
    }

    /// <summary>
    /// Atomically increment the shares counter for a post
    /// Uses raw SQL for atomic operation to prevent race conditions
    /// </summary>
    public async Task<bool> IncrementSharesAsync(Guid id)
    {
        var rowsAffected = await _context.Database.ExecuteSqlRawAsync(
            "UPDATE MarketingPosts SET Shares = Shares + 1, UpdatedAt = {0} WHERE Id = {1} AND IsDeleted = 0",
            DateTime.UtcNow, id);

        return rowsAffected > 0;
    }

    /// <summary>
    /// Get all scheduled posts that are due for publishing
    /// Returns posts with Status = Scheduled and ScheduledDate <= currentTime
    /// </summary>
    public async Task<List<MarketingPost>> GetScheduledPostsDueAsync(DateTime currentTime)
    {
        return await _dbSet
            .Where(p => p.Status == MarketingPostStatus.Scheduled &&
                       p.ScheduledDate.HasValue &&
                       p.ScheduledDate.Value <= currentTime)
            .ToListAsync();
    }
}
