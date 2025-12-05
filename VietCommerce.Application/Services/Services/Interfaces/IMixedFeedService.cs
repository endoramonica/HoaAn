using VietCommerce.Core.DTOs.Posts;
using VietCommerce.Core.Models;

namespace VietCommerce.Application.Services.Services.Interfaces;

/// <summary>
/// Service interface cho Mixed Feed - kết hợp Community Posts và Marketing Posts
/// </summary>
public interface IMixedFeedService
{
    /// <summary>
    /// Lấy mixed feed với thuật toán trộn thông minh
    /// Marketing posts xuất hiện mỗi N community posts (configurable)
    /// </summary>
    /// <param name="query">Query parameters</param>
    /// <returns>Paginated mixed feed</returns>
    Task<ApiResponse<PaginatedResult<MixedFeedDto>>> GetMixedFeedAsync(MixedFeedQuery query);

    /// <summary>
    /// Lấy posts theo vị trí hiển thị cụ thể
    /// Ví dụ: homepage_banner, product_section, featured_section
    /// </summary>
    /// <param name="location">Display location</param>
    /// <param name="pageNumber">Page number</param>
    /// <param name="pageSize">Page size</param>
    /// <returns>Posts filtered by location</returns>
    Task<ApiResponse<LocationPostsDto>> GetPostsByLocationAsync(
        string location,
        int pageNumber = 1,
        int pageSize = 20);

    /// <summary>
    /// Lấy featured posts (priority > 80)
    /// </summary>
    /// <param name="pageNumber">Page number</param>
    /// <param name="pageSize">Page size</param>
    /// <returns>Featured posts only</returns>
    Task<ApiResponse<FeaturedPostsDto>> GetFeaturedPostsAsync(
        int pageNumber = 1,
        int pageSize = 10);

    /// <summary>
    /// Lấy posts liên quan đến một product
    /// Bao gồm cả marketing posts và community posts mention product đó
    /// </summary>
    /// <param name="productId">Product ID</param>
    /// <param name="pageNumber">Page number</param>
    /// <param name="pageSize">Page size</param>
    /// <returns>Related posts</returns>
    Task<ApiResponse<RelatedPostsDto>> GetRelatedPostsByProductAsync(
        Guid productId,
        int pageNumber = 1,
        int pageSize = 20);

    /// <summary>
    /// Track interaction với post (view, click, share)
    /// Public endpoint - không cần authentication
    /// </summary>
    /// <param name="postId">Post ID</param>
    /// <param name="postType">Post type (community/marketing)</param>
    /// <param name="interaction">Interaction details</param>
    /// <returns>Interaction result</returns>
    Task<ApiResponse<PostInteractionResult>> TrackInteractionAsync(
        Guid postId,
        string postType,
        PostInteractionDto interaction);
}
