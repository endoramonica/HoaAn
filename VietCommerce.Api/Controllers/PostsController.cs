using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VietCommerce.Application.Services.Services.Interfaces;
using VietCommerce.Core.DTOs.Posts;
using VietCommerce.Core.Models;

namespace VietCommerce.Api.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class PostsController : ControllerBase
{
    private readonly IPostService _postService;
    private readonly ILogger<PostsController> _logger;

    public PostsController(
        IPostService postService,
        ILogger<PostsController> logger)
    {
        _postService = postService;
        _logger = logger;
    }

    /// <summary>
    /// Tạo post mới (yêu cầu authentication)
    /// </summary>
    /// <param name="dto">Thông tin post cần tạo</param>
    /// <returns>ApiResponse với PostResponseDto</returns>
    [Authorize]
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<PostResponseDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<PostResponseDto>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreatePost([FromForm] CreatePostDto dto)
    {
        var response = await _postService.CreatePostAsync(dto);

        if (!response.Success)
            return BadRequest(response);

        return CreatedAtAction(
            nameof(GetPostById),
            new { postId = response.Data?.PostId },
            response);
    }


    /// <summary>
    /// Cập nhật post (chỉ owner được phép)
    /// </summary>
    /// <param name="postId">ID của post cần update</param>
    /// <param name="dto">Dữ liệu cập nhật</param>
    /// <returns>ApiResponse với PostResponseDto</returns>
    [Authorize]
    [HttpPut("{postId:guid}")]
    [ProducesResponseType(typeof(ApiResponse<PostResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<PostResponseDto>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdatePost(Guid postId, [FromBody] UpdatePostDto dto)
    {
        var response = await _postService.UpdatePostAsync(postId, dto);

        if (!response.Success)
        {
            // Phân biệt lỗi dựa trên Message hoặc Errors
            if (response.Message?.Contains("not found", StringComparison.OrdinalIgnoreCase) == true)
                return NotFound(response);

            if (response.Message?.Contains("not authorized", StringComparison.OrdinalIgnoreCase) == true ||
                response.Message?.Contains("forbidden", StringComparison.OrdinalIgnoreCase) == true)
                return StatusCode(StatusCodes.Status403Forbidden, response);

            return BadRequest(response);
        }

        return Ok(response);
    }

    /// <summary>
    /// Xóa post (soft delete, chỉ owner được phép)
    /// </summary>
    /// <param name="postId">ID của post cần xóa</param>
    /// <returns>ApiResponse với boolean result</returns>
    [Authorize]
    [HttpDelete("{postId:guid}")]
    [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeletePost(Guid postId)
    {
        var response = await _postService.DeletePostAsync(postId);

        if (!response.Success)
        {
            // Phân biệt lỗi dựa trên Message
            if (response.Message?.Contains("not found", StringComparison.OrdinalIgnoreCase) == true)
                return NotFound(response);

            if (response.Message?.Contains("not authorized", StringComparison.OrdinalIgnoreCase) == true ||
                response.Message?.Contains("forbidden", StringComparison.OrdinalIgnoreCase) == true)
                return StatusCode(StatusCodes.Status403Forbidden, response);

            return BadRequest(response);
        }

        return Ok(response);
    }

    /// <summary>
    /// Lấy feed posts với pagination (public, guest có thể xem)
    /// </summary>
    /// <param name="pageNumber">Số trang (default: 1)</param>
    /// <param name="pageSize">Số items mỗi trang (default: 20)</param>
    /// <returns>ApiResponse với PaginatedResult của PostFeedDto</returns>
    [HttpGet("feed")]
    [ProducesResponseType(typeof(ApiResponse<PaginatedResult<PostFeedDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<PaginatedResult<PostFeedDto>>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetPostFeed(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 20)
    {
        var response = await _postService.GetPostFeedAsync(pageNumber, pageSize);

        if (!response.Success)
            return BadRequest(response);

        return Ok(response);
    }

    /// <summary>
    /// Lấy chi tiết một post (public, guest có thể xem)
    /// </summary>
    /// <param name="postId">ID của post</param>
    /// <returns>ApiResponse với PostDetailDto</returns>
    [HttpGet("{postId:guid}")]
    [ProducesResponseType(typeof(ApiResponse<PostDetailDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<PostDetailDto>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetPostById(Guid postId)
    {
        var response = await _postService.GetPostByIdAsync(postId);

        if (!response.Success)
            return NotFound(response);

        return Ok(response);
    }

    /// <summary>
    /// Lấy danh sách posts của một customer (public profile)
    /// </summary>
    /// <param name="customerId">ID của customer</param>
    /// <param name="pageNumber">Số trang (default: 1)</param>
    /// <param name="pageSize">Số items mỗi trang (default: 20)</param>
    /// <returns>ApiResponse với PaginatedResult của PostResponseDto</returns>
    [HttpGet("customer/{customerId:guid}")]
    [ProducesResponseType(typeof(ApiResponse<PaginatedResult<PostResponseDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<PaginatedResult<PostResponseDto>>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetPostsByCustomerId(
        Guid customerId,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 20)
    {
        var response = await _postService.GetPostsByCustomerIdAsync(customerId, pageNumber, pageSize);

        if (!response.Success)
            return BadRequest(response);

        return Ok(response);
    }

    /// <summary>
    /// Tìm kiếm posts theo keyword và date range
    /// </summary>
    /// <param name="keyword">Từ khóa tìm kiếm</param>
    /// <param name="fromDate">Từ ngày (nullable)</param>
    /// <param name="toDate">Đến ngày (nullable)</param>
    /// <param name="pageNumber">Số trang (default: 1)</param>
    /// <param name="pageSize">Số items mỗi trang (default: 20)</param>
    /// <returns>ApiResponse với PaginatedResult của PostResponseDto</returns>
    [HttpGet("search")]
    [ProducesResponseType(typeof(ApiResponse<PaginatedResult<PostResponseDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<PaginatedResult<PostResponseDto>>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> SearchPosts(
        [FromQuery] string keyword,
        [FromQuery] DateTime? fromDate = null,
        [FromQuery] DateTime? toDate = null,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 20)
    {
        var response = await _postService.SearchPostsAsync(
            keyword,
            fromDate,
            toDate,
            pageNumber,
            pageSize);

        if (!response.Success)
            return BadRequest(response);

        return Ok(response);
    }

    /// <summary>
    /// Toggle like/unlike post (yêu cầu authentication)
    /// </summary>
    /// <param name="postId">ID của post</param>
    /// <returns>ApiResponse với PostLikeResult</returns>
    [Authorize]
    [HttpPost("{postId:guid}/like")]
    [ProducesResponseType(typeof(ApiResponse<PostLikeResult>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<PostLikeResult>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ToggleLike(Guid postId)
    {
        var response = await _postService.ToggleLikeAsync(postId);

        if (!response.Success)
        {
            if (response.Message?.Contains("not found", StringComparison.OrdinalIgnoreCase) == true)
                return NotFound(response);

            return BadRequest(response);
        }

        return Ok(response);
    }

    /// <summary>
    /// Toggle bookmark/unbookmark post (yêu cầu authentication)
    /// </summary>
    /// <param name="postId">ID của post</param>
    /// <returns>ApiResponse với PostBookmarkResult</returns>
    [Authorize]
    [HttpPost("{postId:guid}/bookmark")]
    [ProducesResponseType(typeof(ApiResponse<PostBookmarkResult>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<PostBookmarkResult>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ToggleBookmark(Guid postId)
    {
        var response = await _postService.ToggleBookmarkAsync(postId);

        if (!response.Success)
        {
            if (response.Message?.Contains("not found", StringComparison.OrdinalIgnoreCase) == true)
                return NotFound(response);

            return BadRequest(response);
        }

        return Ok(response);
    }
}