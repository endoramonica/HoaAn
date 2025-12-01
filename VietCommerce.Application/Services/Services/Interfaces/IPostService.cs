// VietCommerce.Application/Services/Interfaces/IPostService.cs
using VietCommerce.Core.DTOs.Posts;
using VietCommerce.Core.Models;

namespace VietCommerce.Application.Services.Services.Interfaces;

/// <summary>
/// ✅ SECURE Service interface cho Post operations
/// - CustomerId được lấy từ ICurrentUser (JWT token)
/// - KHÔNG nhận CustomerId từ client để tránh security flaw
/// </summary>
public interface IPostService
{
    /// <summary>
    /// Tạo post mới (với hoặc không có ảnh)
    /// CustomerId được lấy tự động từ JWT token
    /// </summary>
    /// <param name="dto">Data transfer object chứa thông tin post</param>
    /// <returns>ApiResponse với PostResponseDto nếu thành công</returns>
    Task<ApiResponse<PostResponseDto>> CreatePostAsync(CreatePostDto dto);

    /// <summary>
    /// Cập nhật post (chỉ owner có quyền)
    /// Authorization được kiểm tra tự động thông qua JWT token
    /// </summary>
    /// <param name="postId">ID của post cần update</param>
    /// <param name="dto">Dữ liệu cập nhật</param>
    /// <returns>ApiResponse với PostResponseDto nếu thành công</returns>
    Task<ApiResponse<PostResponseDto>> UpdatePostAsync(Guid postId, UpdatePostDto dto);

    /// <summary>
    /// Xóa post (soft delete, chỉ owner có quyền)
    /// Authorization được kiểm tra tự động thông qua JWT token
    /// </summary>
    /// <param name="postId">ID của post cần xóa</param>
    /// <returns>ApiResponse với true nếu thành công</returns>
    Task<ApiResponse<bool>> DeletePostAsync(Guid postId);

    /// <summary>
    /// Lấy post feed với pagination (public feed, guest có thể xem)
    /// Nếu user đã login, sẽ hiển thị thêm thông tin liked/bookmarked
    /// </summary>
    /// <param name="pageNumber">Trang hiện tại (1-based)</param>
    /// <param name="pageSize">Số items mỗi trang</param>
    /// <returns>ApiResponse với PaginatedResult của PostFeedDto</returns>
    Task<ApiResponse<PaginatedResult<PostFeedDto>>> GetPostFeedAsync(
        int pageNumber = 1,
        int pageSize = 20);

    /// <summary>
    /// Lấy chi tiết một post (public, guest có thể xem)
    /// </summary>
    /// <param name="postId">ID của post</param>
    /// <returns>ApiResponse với PostDetailDto</returns>
    Task<ApiResponse<PostDetailDto>> GetPostByIdAsync(Guid postId);

    /// <summary>
    /// Lấy danh sách posts của một customer (public profile)
    /// </summary>
    /// <param name="customerId">ID của customer</param>
    /// <param name="pageNumber">Trang hiện tại</param>
    /// <param name="pageSize">Số items mỗi trang</param>
    /// <returns>ApiResponse với PaginatedResult của PostResponseDto</returns>
    Task<ApiResponse<PaginatedResult<PostResponseDto>>> GetPostsByCustomerIdAsync(
        Guid customerId,
        int pageNumber = 1,
        int pageSize = 20);

    /// <summary>
    /// Tìm kiếm posts theo keyword và date range
    /// </summary>
    /// <param name="keyword">Từ khóa tìm kiếm</param>
    /// <param name="fromDate">Từ ngày (nullable)</param>
    /// <param name="toDate">Đến ngày (nullable)</param>
    /// <param name="pageNumber">Trang hiện tại</param>
    /// <param name="pageSize">Số items mỗi trang</param>
    /// <returns>ApiResponse với PaginatedResult của PostResponseDto</returns>
    Task<ApiResponse<PaginatedResult<PostResponseDto>>> SearchPostsAsync(
        string keyword,
        DateTime? fromDate = null,
        DateTime? toDate = null,
        int pageNumber = 1,
        int pageSize = 20);

    /// <summary>
    /// Toggle like/unlike post (requires authentication)
    /// CustomerId được lấy từ JWT token
    /// </summary>
    /// <param name="postId">ID của post</param>
    /// <returns>ApiResponse với true (liked) hoặc false (unliked)</returns>
    Task<ApiResponse<PostLikeResult>> ToggleLikeAsync(Guid postId);

    /// <summary>
    /// Toggle bookmark/unbookmark post (requires authentication)
    /// CustomerId được lấy từ JWT token
    /// </summary>
    /// <param name="postId">ID của post</param>
    /// <returns>ApiResponse với true (bookmarked) hoặc false (unbookmarked)</returns>
    Task<ApiResponse<PostBookmarkResult>> ToggleBookmarkAsync(Guid postId);
}