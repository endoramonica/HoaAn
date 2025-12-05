using Microsoft.AspNetCore.Mvc;
using VietCommerce.Application.Services.Services.Interfaces;
using VietCommerce.Core.DTOs.Posts;
using VietCommerce.Core.Models;

namespace VietCommerce.Api.Controllers;

/// <summary>
/// Mixed Feed API - Kết hợp Community Posts và Marketing Posts
/// Public API cho end-users
/// </summary>
[ApiController]
[Route("api/v1/[controller]")]
public class MixedFeedController : ControllerBase
{
    private readonly IMixedFeedService _mixedFeedService;
    private readonly ILogger<MixedFeedController> _logger;

    public MixedFeedController(
        IMixedFeedService mixedFeedService,
        ILogger<MixedFeedController> logger)
    {
        _mixedFeedService = mixedFeedService;
        _logger = logger;
    }

    /// <summary>
    /// Lấy mixed feed với thuật toán trộn thông minh
    /// Marketing posts xuất hiện mỗi N community posts (default: 1 marketing mỗi 4 community posts)
    /// </summary>
    /// <param name="query">Query parameters</param>
    /// <returns>Paginated mixed feed</returns>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<PaginatedResult<MixedFeedDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<PaginatedResult<MixedFeedDto>>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetMixedFeed([FromQuery] MixedFeedQuery query)
    {
        var response = await _mixedFeedService.GetMixedFeedAsync(query);

        if (!response.Success)
            return BadRequest(response);

        return Ok(response);
    }

    /// <summary>
    /// Lấy posts theo vị trí hiển thị cụ thể
    /// Ví dụ: homepage_banner, product_section, featured_section, sidebar
    /// </summary>
    /// <param name="location">Display location</param>
    /// <param name="pageNumber">Page number (default: 1)</param>
    /// <param name="pageSize">Page size (default: 20)</param>
    /// <returns>Posts filtered by location</returns>
    [HttpGet("location/{location}")]
    [ProducesResponseType(typeof(ApiResponse<LocationPostsDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<LocationPostsDto>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetPostsByLocation(
        string location,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 20)
    {
        var response = await _mixedFeedService.GetPostsByLocationAsync(location, pageNumber, pageSize);

        if (!response.Success)
            return BadRequest(response);

        return Ok(response);
    }

    /// <summary>
    /// Lấy featured posts (priority score > 80)
    /// Dành cho banner, highlight sections
    /// </summary>
    /// <param name="pageNumber">Page number (default: 1)</param>
    /// <param name="pageSize">Page size (default: 10)</param>
    /// <returns>Featured posts only</returns>
    [HttpGet("featured")]
    [ProducesResponseType(typeof(ApiResponse<FeaturedPostsDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<FeaturedPostsDto>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetFeaturedPosts(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10)
    {
        var response = await _mixedFeedService.GetFeaturedPostsAsync(pageNumber, pageSize);

        if (!response.Success)
            return BadRequest(response);

        return Ok(response);
    }

    /// <summary>
    /// Lấy posts liên quan đến một product
    /// Bao gồm cả marketing posts và community posts mention product đó
    /// </summary>
    /// <param name="productId">Product ID</param>
    /// <param name="pageNumber">Page number (default: 1)</param>
    /// <param name="pageSize">Page size (default: 20)</param>
    /// <returns>Related posts</returns>
    [HttpGet("product/{productId:guid}")]
    [ProducesResponseType(typeof(ApiResponse<RelatedPostsDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<RelatedPostsDto>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetRelatedPostsByProduct(
        Guid productId,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 20)
    {
        var response = await _mixedFeedService.GetRelatedPostsByProductAsync(productId, pageNumber, pageSize);

        if (!response.Success)
            return BadRequest(response);

        return Ok(response);
    }

    /// <summary>
    /// Track interaction với post (view, click, share)
    /// Public endpoint - không cần authentication
    /// </summary>
    /// <param name="postId">Post ID</param>
    /// <param name="postType">Post type: "community" hoặc "marketing"</param>
    /// <param name="interaction">Interaction details</param>
    /// <returns>Interaction result</returns>
    [HttpPost("{postId:guid}/track")]
    [ProducesResponseType(typeof(ApiResponse<PostInteractionResult>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<PostInteractionResult>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> TrackInteraction(
        Guid postId,
        [FromQuery] string postType,
        [FromBody] PostInteractionDto interaction)
    {
        var response = await _mixedFeedService.TrackInteractionAsync(postId, postType, interaction);

        if (!response.Success)
            return BadRequest(response);

        return Ok(response);
    }
}
