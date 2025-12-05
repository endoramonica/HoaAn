using VietCommerce.Core.DTOs.Marketing;
using VietCommerce.Core.Entities.Marketing;
using VietCommerce.Core.Enums.Marketing;
using VietCommerce.Core.Models;

namespace VietCommerce.Data.Repositories.Interfaces;

/// <summary>
/// Repository interface for Marketing Post operations
/// Handles CRUD, filtering, pagination, analytics, and statistics
/// </summary>
public interface IMarketingPostRepository : IGenericRepository<MarketingPost>
{
    /// <summary>
    /// Get paginated posts with advanced filtering and search
    /// </summary>
    Task<PaginatedResult<MarketingPost>> GetPostsAsync(
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
        bool includeDeleted = false);

    /// <summary>
    /// Get post by ID with option to include soft-deleted posts
    /// </summary>
    Task<MarketingPost?> GetByIdAsync(Guid id, bool includeDeleted = false);

    /// <summary>
    /// Create a new marketing post
    /// </summary>
    Task<MarketingPost> CreateAsync(MarketingPost post);

    /// <summary>
    /// Update an existing marketing post
    /// </summary>
    Task<MarketingPost> UpdateAsync(MarketingPost post);

    /// <summary>
    /// Soft delete a marketing post
    /// </summary>
    Task<bool> DeleteAsync(Guid id, Guid deletedBy);

    /// <summary>
    /// Restore a soft-deleted marketing post
    /// </summary>
    Task<bool> RestoreAsync(Guid id);

    /// <summary>
    /// Get all posts linked to a specific product
    /// </summary>
    Task<List<MarketingPost>> GetByProductIdAsync(Guid productId);

    /// <summary>
    /// Get aggregate statistics for all marketing posts
    /// </summary>
    Task<MarketingPostStatisticsDto> GetStatisticsAsync();

    /// <summary>
    /// Atomically increment the views counter for a post
    /// </summary>
    Task<bool> IncrementViewsAsync(Guid id);

    /// <summary>
    /// Atomically increment the clicks counter for a post
    /// </summary>
    Task<bool> IncrementClicksAsync(Guid id);

    /// <summary>
    /// Atomically increment the shares counter for a post
    /// </summary>
    Task<bool> IncrementSharesAsync(Guid id);

    /// <summary>
    /// Get all scheduled posts that are due for publishing
    /// </summary>
    Task<List<MarketingPost>> GetScheduledPostsDueAsync(DateTime currentTime);
}
