using VietCommerce.Core.DTOs.Marketing;
using VietCommerce.Core.Enums.Marketing;
using VietCommerce.Core.Models;

namespace VietCommerce.Application.Services.Services.Interfaces;

/// <summary>
/// Service interface for Admin Marketing Post Management
/// Handles CRUD operations, publishing, analytics, and bulk operations for marketing posts
/// Separate from Social Community Posts (Customer posts)
/// </summary>
public interface IMarketingPostService
{
    #region CRUD Operations

    /// <summary>
    /// Get paginated list of marketing posts with filtering and search
    /// </summary>
    /// <param name="query">Query parameters for filtering, pagination, and sorting</param>
    /// <returns>ApiResponse with paginated result of marketing posts</returns>
    Task<ApiResponse<PaginatedResult<MarketingPostListDto>>> GetPostsAsync(
        GetMarketingPostsQuery query);

    /// <summary>
    /// Get detailed information of a specific marketing post by ID
    /// </summary>
    /// <param name="id">Marketing post ID</param>
    /// <returns>ApiResponse with detailed marketing post information</returns>
    Task<ApiResponse<MarketingPostDetailDto>> GetPostByIdAsync(Guid id);

    /// <summary>
    /// Create a new marketing post
    /// Admin authentication required - CreatedBy will be set from JWT token
    /// </summary>
    /// <param name="dto">Marketing post creation data</param>
    /// <returns>ApiResponse with created marketing post</returns>
    Task<ApiResponse<MarketingPostResponseDto>> CreatePostAsync(
        CreateMarketingPostDto dto);

    /// <summary>
    /// Update an existing marketing post
    /// Admin authentication required - UpdatedBy will be set from JWT token
    /// </summary>
    /// <param name="id">Marketing post ID</param>
    /// <param name="dto">Marketing post update data</param>
    /// <returns>ApiResponse with updated marketing post</returns>
    Task<ApiResponse<MarketingPostResponseDto>> UpdatePostAsync(
        Guid id,
        UpdateMarketingPostDto dto);

    /// <summary>
    /// Soft delete a marketing post
    /// Admin authentication required - DeletedBy will be set from JWT token
    /// </summary>
    /// <param name="id">Marketing post ID</param>
    /// <returns>ApiResponse with success status</returns>
    Task<ApiResponse<bool>> DeletePostAsync(Guid id);

    /// <summary>
    /// Restore a soft-deleted marketing post
    /// Admin authentication required
    /// </summary>
    /// <param name="id">Marketing post ID</param>
    /// <returns>ApiResponse with success status</returns>
    Task<ApiResponse<bool>> RestorePostAsync(Guid id);

    #endregion

    #region Publishing Operations

    /// <summary>
    /// Publish a draft or scheduled post immediately
    /// Changes status to Published and sets PublishedDate
    /// </summary>
    /// <param name="id">Marketing post ID</param>
    /// <returns>ApiResponse with updated marketing post</returns>
    Task<ApiResponse<MarketingPostResponseDto>> PublishPostAsync(Guid id);

    /// <summary>
    /// Schedule a post for future publication
    /// Changes status to Scheduled and sets ScheduledDate
    /// </summary>
    /// <param name="id">Marketing post ID</param>
    /// <param name="scheduledDate">Future date/time for publication</param>
    /// <returns>ApiResponse with updated marketing post</returns>
    Task<ApiResponse<MarketingPostResponseDto>> SchedulePostAsync(
        Guid id,
        DateTime scheduledDate);

    /// <summary>
    /// Unpublish a published post (revert to draft)
    /// Changes status to Draft and clears PublishedDate
    /// </summary>
    /// <param name="id">Marketing post ID</param>
    /// <returns>ApiResponse with updated marketing post</returns>
    Task<ApiResponse<MarketingPostResponseDto>> UnpublishPostAsync(Guid id);

    #endregion

    #region Analytics Operations

    /// <summary>
    /// Increment view counter for a marketing post
    /// Public endpoint - no authentication required
    /// </summary>
    /// <param name="id">Marketing post ID</param>
    /// <returns>ApiResponse with success status</returns>
    Task<ApiResponse<bool>> IncrementViewsAsync(Guid id);

    /// <summary>
    /// Increment click counter for a marketing post
    /// Public endpoint - no authentication required
    /// </summary>
    /// <param name="id">Marketing post ID</param>
    /// <returns>ApiResponse with success status</returns>
    Task<ApiResponse<bool>> IncrementClicksAsync(Guid id);

    /// <summary>
    /// Increment share counter for a marketing post
    /// Public endpoint - no authentication required
    /// </summary>
    /// <param name="id">Marketing post ID</param>
    /// <returns>ApiResponse with success status</returns>
    Task<ApiResponse<bool>> IncrementSharesAsync(Guid id);

    #endregion

    #region Utility Operations

    /// <summary>
    /// Duplicate an existing marketing post
    /// Creates a new post with copied content, appends "(Copy)" to title,
    /// sets status to Draft, and resets analytics counters
    /// </summary>
    /// <param name="id">Source marketing post ID</param>
    /// <returns>ApiResponse with newly created duplicate post</returns>
    Task<ApiResponse<MarketingPostResponseDto>> DuplicatePostAsync(Guid id);

    /// <summary>
    /// Get aggregate statistics for all marketing posts
    /// Returns counts by status and total analytics metrics
    /// </summary>
    /// <returns>ApiResponse with marketing post statistics</returns>
    Task<ApiResponse<MarketingPostStatisticsDto>> GetStatisticsAsync();

    /// <summary>
    /// Get all marketing posts linked to a specific product
    /// </summary>
    /// <param name="productId">Product ID</param>
    /// <returns>ApiResponse with list of marketing posts</returns>
    Task<ApiResponse<List<MarketingPostListDto>>> GetPostsByProductAsync(
        Guid productId);

    #endregion

    #region Bulk Operations

    /// <summary>
    /// Soft delete multiple marketing posts in a single transaction
    /// Returns detailed results indicating success/failure for each post
    /// </summary>
    /// <param name="postIds">List of marketing post IDs to delete</param>
    /// <returns>ApiResponse with bulk operation results</returns>
    Task<ApiResponse<BulkOperationResult>> BulkDeleteAsync(List<Guid> postIds);

    /// <summary>
    /// Publish multiple draft posts in a single transaction
    /// Returns detailed results indicating success/failure for each post
    /// </summary>
    /// <param name="postIds">List of marketing post IDs to publish</param>
    /// <returns>ApiResponse with bulk operation results</returns>
    Task<ApiResponse<BulkOperationResult>> BulkPublishAsync(List<Guid> postIds);

    /// <summary>
    /// Update status for multiple marketing posts in a single transaction
    /// Returns detailed results indicating success/failure for each post
    /// </summary>
    /// <param name="postIds">List of marketing post IDs to update</param>
    /// <param name="status">New status to apply</param>
    /// <returns>ApiResponse with bulk operation results</returns>
    Task<ApiResponse<BulkOperationResult>> BulkUpdateStatusAsync(
        List<Guid> postIds,
        MarketingPostStatus status);

    #endregion
}
