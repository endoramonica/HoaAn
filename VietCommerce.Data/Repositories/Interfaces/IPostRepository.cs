// VietCommerce.Data/Repositories/Interfaces/IPostRepository.cs
using VietCommerce.Core.Models;

namespace VietCommerce.Data.Repositories.Interfaces;

/// <summary>
/// Repository interface cho Post entity
/// Cung cấp các query methods đặc biệt cho social feed
/// </summary>
public interface IPostRepository : IGenericRepository<Post>
{
    /// <summary>
    /// Lấy post với đầy đủ thông tin (Customer, Likes, Comments, Bookmarks)
    /// </summary>
    Task<Post?> GetPostWithDetailsAsync(Guid postId);

    /// <summary>
    /// Lấy feed posts (active, not deleted) với pagination
    /// </summary>
    /// <param name="pageNumber">Trang hiện tại (1-based)</param>
    /// <param name="pageSize">Số items mỗi trang</param>
    /// <param name="currentCustomerId">CustomerId của user hiện tại (để check liked/bookmarked)</param>
    Task<PaginatedResult<Post>> GetPostFeedAsync(
        int pageNumber,
        int pageSize,
        Guid? currentCustomerId = null);

    /// <summary>
    /// Lấy posts của một customer cụ thể
    /// </summary>
    Task<PaginatedResult<Post>> GetPostsByCustomerIdAsync(
        Guid customerId,
        int pageNumber,
        int pageSize);

    /// <summary>
    /// Tìm kiếm posts theo keyword, date range
    /// </summary>
    Task<PaginatedResult<Post>> SearchPostsAsync(
        string keyword,
        DateTime? fromDate = null,
        DateTime? toDate = null,
        int pageNumber = 1,
        int pageSize = 20);

    /// <summary>
    /// Kiểm tra ảnh trùng lặp qua PhotoHash
    /// </summary>
    Task<bool> IsDuplicateImageAsync(string photoHash);

    /// <summary>
    /// Đếm số likes của post
    /// </summary>
    Task<int> GetLikesCountAsync(Guid postId);

    /// <summary>
    /// Đếm số comments của post (bao gồm replies)
    /// </summary>
    Task<int> GetCommentsCountAsync(Guid postId);

    /// <summary>
    /// Đếm số bookmarks của post
    /// </summary>
    Task<int> GetBookmarksCountAsync(Guid postId);

    /// <summary>
    /// Kiểm tra customer đã like post chưa
    /// </summary>
    Task<bool> IsLikedByCustomerAsync(Guid postId, Guid customerId);

    /// <summary>
    /// Kiểm tra customer đã bookmark post chưa
    /// </summary>
    Task<bool> IsBookmarkedByCustomerAsync(Guid postId, Guid customerId);

    /// <summary>
    /// Lấy danh sách customers đã like post (top N)
    /// </summary>
    Task<List<Guid>> GetPostLikersAsync(Guid postId, int top = 10);

    /// <summary>
    /// Lấy post theo PhotoHash (để check duplicate)
    /// </summary>
    Task<Post?> GetPostByPhotoHashAsync(string photoHash);
    Task<HashSet<Guid>> GetLikedPostIdsByCustomerAsync(List<Guid> postIds, Guid customerId);
    Task<HashSet<Guid>> GetBookmarkedPostIdsByCustomerAsync(List<Guid> postIds, Guid customerId);
}