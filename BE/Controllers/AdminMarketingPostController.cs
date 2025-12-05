using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VietCommerce.Application.Services.Services.Interfaces;
using VietCommerce.Core.DTOs.Marketing;
using VietCommerce.Core.Enums.Marketing;
using VietCommerce.Core.Models;

namespace VietCommerce.AdminApi.Controllers
{
    /// <summary>
    /// Admin Marketing Post Management Controller
    /// Handles CRUD operations, publishing, analytics, and bulk operations for marketing posts
    /// Separate from Social Community Posts (Customer posts)
    /// </summary>
    [ApiController]
    [Route("api/admin/marketing-posts")]
    [Authorize]
    public class AdminMarketingPostController : ControllerBase
    {
        private readonly IMarketingPostService _marketingPostService;
        private readonly ILogger<AdminMarketingPostController> _logger;

        public AdminMarketingPostController(
            IMarketingPostService marketingPostService,
            ILogger<AdminMarketingPostController> logger)
        {
            _marketingPostService = marketingPostService;
            _logger = logger;
        }

        #region CRUD Operations

        /// <summary>
        /// Get paginated list of marketing posts with filtering and search
        /// </summary>
        /// <param name="query">Query parameters for filtering, pagination, and sorting</param>
        /// <returns>Paginated list of marketing posts</returns>
        /// <response code="200">Returns the paginated list of marketing posts</response>
        /// <response code="400">If the query parameters are invalid</response>
        [HttpGet]
        [ProducesResponseType(typeof(ApiResponse<PaginatedResult<MarketingPostListDto>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<PaginatedResult<MarketingPostListDto>>), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ApiResponse<PaginatedResult<MarketingPostListDto>>>> GetPosts(
            [FromQuery] GetMarketingPostsQuery query)
        {
            _logger.LogInformation("GET marketing posts - Page: {PageNumber}, Size: {PageSize}",
                query.PageNumber, query.PageSize);

            var result = await _marketingPostService.GetPostsAsync(query);

            if (!result.Success)
                return BadRequest(result);

            return Ok(result);
        }

        /// <summary>
        /// Get detailed information of a specific marketing post by ID
        /// </summary>
        /// <param name="id">Marketing post ID</param>
        /// <returns>Detailed marketing post information</returns>
        /// <response code="200">Returns the marketing post details</response>
        /// <response code="404">If the marketing post is not found</response>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ApiResponse<MarketingPostDetailDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<MarketingPostDetailDto>), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ApiResponse<MarketingPostDetailDto>>> GetPostById(Guid id)
        {
            _logger.LogInformation("GET marketing post by ID: {PostId}", id);

            var result = await _marketingPostService.GetPostByIdAsync(id);

            if (!result.Success)
            {
                if (result.Message?.Contains("not found", StringComparison.OrdinalIgnoreCase) == true)
                    return NotFound(result);

                return BadRequest(result);
            }

            return Ok(result);
        }

        /// <summary>
        /// Create a new marketing post
        /// </summary>
        /// <param name="dto">Marketing post creation data</param>
        /// <returns>Created marketing post</returns>
        /// <response code="200">Returns the newly created marketing post</response>
        /// <response code="400">If the input data is invalid</response>
        /// <response code="401">If the user is not authenticated</response>
        /// <response code="403">If the user does not have admin role</response>
        [HttpPost]
        [ProducesResponseType(typeof(ApiResponse<MarketingPostResponseDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<MarketingPostResponseDto>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<ApiResponse<MarketingPostResponseDto>>> CreatePost(
            [FromBody] CreateMarketingPostDto dto)
        {
            _logger.LogInformation("CREATE marketing post - Title: {Title}", dto.Title);

            var result = await _marketingPostService.CreatePostAsync(dto);

            if (!result.Success)
            {
                if (result.Message?.Contains("denied", StringComparison.OrdinalIgnoreCase) == true ||
                    result.Message?.Contains("Unauthorized", StringComparison.OrdinalIgnoreCase) == true)
                    return StatusCode(StatusCodes.Status403Forbidden, result);

                return BadRequest(result);
            }

            return Ok(result);
        }

        /// <summary>
        /// Update an existing marketing post
        /// </summary>
        /// <param name="id">Marketing post ID</param>
        /// <param name="dto">Marketing post update data</param>
        /// <returns>Updated marketing post</returns>
        /// <response code="200">Returns the updated marketing post</response>
        /// <response code="400">If the input data is invalid</response>
        /// <response code="401">If the user is not authenticated</response>
        /// <response code="403">If the user does not have admin role</response>
        /// <response code="404">If the marketing post is not found</response>
        /// <response code="409">If there is a concurrency conflict</response>
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(ApiResponse<MarketingPostResponseDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<MarketingPostResponseDto>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ApiResponse<MarketingPostResponseDto>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiResponse<MarketingPostResponseDto>), StatusCodes.Status409Conflict)]
        public async Task<ActionResult<ApiResponse<MarketingPostResponseDto>>> UpdatePost(
            Guid id,
            [FromBody] UpdateMarketingPostDto dto)
        {
            _logger.LogInformation("UPDATE marketing post - ID: {PostId}", id);

            var result = await _marketingPostService.UpdatePostAsync(id, dto);

            if (!result.Success)
            {
                if (result.Message?.Contains("denied", StringComparison.OrdinalIgnoreCase) == true ||
                    result.Message?.Contains("Unauthorized", StringComparison.OrdinalIgnoreCase) == true)
                    return StatusCode(StatusCodes.Status403Forbidden, result);

                if (result.Message?.Contains("not found", StringComparison.OrdinalIgnoreCase) == true)
                    return NotFound(result);

                if (result.Message?.Contains("conflict", StringComparison.OrdinalIgnoreCase) == true ||
                    result.Message?.Contains("modified", StringComparison.OrdinalIgnoreCase) == true)
                    return Conflict(result);

                return BadRequest(result);
            }

            return Ok(result);
        }

        /// <summary>
        /// Soft delete a marketing post
        /// </summary>
        /// <param name="id">Marketing post ID</param>
        /// <returns>Success status</returns>
        /// <response code="200">If the marketing post was successfully deleted</response>
        /// <response code="401">If the user is not authenticated</response>
        /// <response code="403">If the user does not have admin role</response>
        /// <response code="404">If the marketing post is not found</response>
        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ApiResponse<bool>>> DeletePost(Guid id)
        {
            _logger.LogInformation("DELETE marketing post - ID: {PostId}", id);

            var result = await _marketingPostService.DeletePostAsync(id);

            if (!result.Success)
            {
                if (result.Message?.Contains("denied", StringComparison.OrdinalIgnoreCase) == true ||
                    result.Message?.Contains("Unauthorized", StringComparison.OrdinalIgnoreCase) == true)
                    return StatusCode(StatusCodes.Status403Forbidden, result);

                if (result.Message?.Contains("not found", StringComparison.OrdinalIgnoreCase) == true)
                    return NotFound(result);

                return BadRequest(result);
            }

            return Ok(result);
        }

        /// <summary>
        /// Restore a soft-deleted marketing post
        /// </summary>
        /// <param name="id">Marketing post ID</param>
        /// <returns>Success status</returns>
        /// <response code="200">If the marketing post was successfully restored</response>
        /// <response code="401">If the user is not authenticated</response>
        /// <response code="403">If the user does not have admin role</response>
        /// <response code="404">If the marketing post is not found</response>
        [HttpPost("{id}/restore")]
        [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ApiResponse<bool>>> RestorePost(Guid id)
        {
            _logger.LogInformation("RESTORE marketing post - ID: {PostId}", id);

            var result = await _marketingPostService.RestorePostAsync(id);

            if (!result.Success)
            {
                if (result.Message?.Contains("denied", StringComparison.OrdinalIgnoreCase) == true ||
                    result.Message?.Contains("Unauthorized", StringComparison.OrdinalIgnoreCase) == true)
                    return StatusCode(StatusCodes.Status403Forbidden, result);

                if (result.Message?.Contains("not found", StringComparison.OrdinalIgnoreCase) == true)
                    return NotFound(result);

                return BadRequest(result);
            }

            return Ok(result);
        }

        #endregion

        #region Publishing Operations

        /// <summary>
        /// Publish a draft or scheduled post immediately
        /// </summary>
        /// <param name="id">Marketing post ID</param>
        /// <returns>Updated marketing post with Published status</returns>
        /// <response code="200">If the marketing post was successfully published</response>
        /// <response code="400">If the post cannot be published (invalid status)</response>
        /// <response code="401">If the user is not authenticated</response>
        /// <response code="403">If the user does not have admin role</response>
        /// <response code="404">If the marketing post is not found</response>
        [HttpPost("{id}/publish")]
        [ProducesResponseType(typeof(ApiResponse<MarketingPostResponseDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<MarketingPostResponseDto>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ApiResponse<MarketingPostResponseDto>), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ApiResponse<MarketingPostResponseDto>>> PublishPost(Guid id)
        {
            _logger.LogInformation("PUBLISH marketing post - ID: {PostId}", id);

            var result = await _marketingPostService.PublishPostAsync(id);

            if (!result.Success)
            {
                if (result.Message?.Contains("denied", StringComparison.OrdinalIgnoreCase) == true ||
                    result.Message?.Contains("Unauthorized", StringComparison.OrdinalIgnoreCase) == true)
                    return StatusCode(StatusCodes.Status403Forbidden, result);

                if (result.Message?.Contains("not found", StringComparison.OrdinalIgnoreCase) == true)
                    return NotFound(result);

                return BadRequest(result);
            }

            return Ok(result);
        }

        /// <summary>
        /// Schedule a post for future publication
        /// </summary>
        /// <param name="id">Marketing post ID</param>
        /// <param name="dto">Schedule data with future date/time</param>
        /// <returns>Updated marketing post with Scheduled status</returns>
        /// <response code="200">If the marketing post was successfully scheduled</response>
        /// <response code="400">If the scheduled date is invalid (not in future)</response>
        /// <response code="401">If the user is not authenticated</response>
        /// <response code="403">If the user does not have admin role</response>
        /// <response code="404">If the marketing post is not found</response>
        [HttpPost("{id}/schedule")]
        [ProducesResponseType(typeof(ApiResponse<MarketingPostResponseDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<MarketingPostResponseDto>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ApiResponse<MarketingPostResponseDto>), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ApiResponse<MarketingPostResponseDto>>> SchedulePost(
            Guid id,
            [FromBody] SchedulePostDto dto)
        {
            _logger.LogInformation("SCHEDULE marketing post - ID: {PostId}, Date: {ScheduledDate}",
                id, dto.ScheduledDate);

            var result = await _marketingPostService.SchedulePostAsync(id, dto.ScheduledDate);

            if (!result.Success)
            {
                if (result.Message?.Contains("denied", StringComparison.OrdinalIgnoreCase) == true ||
                    result.Message?.Contains("Unauthorized", StringComparison.OrdinalIgnoreCase) == true)
                    return StatusCode(StatusCodes.Status403Forbidden, result);

                if (result.Message?.Contains("not found", StringComparison.OrdinalIgnoreCase) == true)
                    return NotFound(result);

                return BadRequest(result);
            }

            return Ok(result);
        }

        /// <summary>
        /// Unpublish a published post (revert to draft)
        /// </summary>
        /// <param name="id">Marketing post ID</param>
        /// <returns>Updated marketing post with Draft status</returns>
        /// <response code="200">If the marketing post was successfully unpublished</response>
        /// <response code="400">If the post cannot be unpublished (not published)</response>
        /// <response code="401">If the user is not authenticated</response>
        /// <response code="403">If the user does not have admin role</response>
        /// <response code="404">If the marketing post is not found</response>
        [HttpPost("{id}/unpublish")]
        [ProducesResponseType(typeof(ApiResponse<MarketingPostResponseDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<MarketingPostResponseDto>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ApiResponse<MarketingPostResponseDto>), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ApiResponse<MarketingPostResponseDto>>> UnpublishPost(Guid id)
        {
            _logger.LogInformation("UNPUBLISH marketing post - ID: {PostId}", id);

            var result = await _marketingPostService.UnpublishPostAsync(id);

            if (!result.Success)
            {
                if (result.Message?.Contains("denied", StringComparison.OrdinalIgnoreCase) == true ||
                    result.Message?.Contains("Unauthorized", StringComparison.OrdinalIgnoreCase) == true)
                    return StatusCode(StatusCodes.Status403Forbidden, result);

                if (result.Message?.Contains("not found", StringComparison.OrdinalIgnoreCase) == true)
                    return NotFound(result);

                return BadRequest(result);
            }

            return Ok(result);
        }

        #endregion

        #region Analytics Operations

        /// <summary>
        /// Increment view counter for a marketing post
        /// Public endpoint - no authentication required
        /// </summary>
        /// <param name="id">Marketing post ID</param>
        /// <returns>Success status</returns>
        /// <response code="200">If the view counter was successfully incremented</response>
        /// <response code="404">If the marketing post is not found</response>
        [HttpPost("{id}/analytics/views")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ApiResponse<bool>>> IncrementViews(Guid id)
        {
            _logger.LogInformation("INCREMENT views for marketing post - ID: {PostId}", id);

            var result = await _marketingPostService.IncrementViewsAsync(id);

            if (!result.Success)
            {
                if (result.Message?.Contains("not found", StringComparison.OrdinalIgnoreCase) == true)
                    return NotFound(result);

                return BadRequest(result);
            }

            return Ok(result);
        }

        /// <summary>
        /// Increment click counter for a marketing post
        /// Public endpoint - no authentication required
        /// </summary>
        /// <param name="id">Marketing post ID</param>
        /// <returns>Success status</returns>
        /// <response code="200">If the click counter was successfully incremented</response>
        /// <response code="404">If the marketing post is not found</response>
        [HttpPost("{id}/analytics/clicks")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ApiResponse<bool>>> IncrementClicks(Guid id)
        {
            _logger.LogInformation("INCREMENT clicks for marketing post - ID: {PostId}", id);

            var result = await _marketingPostService.IncrementClicksAsync(id);

            if (!result.Success)
            {
                if (result.Message?.Contains("not found", StringComparison.OrdinalIgnoreCase) == true)
                    return NotFound(result);

                return BadRequest(result);
            }

            return Ok(result);
        }

        /// <summary>
        /// Increment share counter for a marketing post
        /// Public endpoint - no authentication required
        /// </summary>
        /// <param name="id">Marketing post ID</param>
        /// <returns>Success status</returns>
        /// <response code="200">If the share counter was successfully incremented</response>
        /// <response code="404">If the marketing post is not found</response>
        [HttpPost("{id}/analytics/shares")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ApiResponse<bool>>> IncrementShares(Guid id)
        {
            _logger.LogInformation("INCREMENT shares for marketing post - ID: {PostId}", id);

            var result = await _marketingPostService.IncrementSharesAsync(id);

            if (!result.Success)
            {
                if (result.Message?.Contains("not found", StringComparison.OrdinalIgnoreCase) == true)
                    return NotFound(result);

                return BadRequest(result);
            }

            return Ok(result);
        }

        #endregion

        #region Utility Operations

        /// <summary>
        /// Duplicate an existing marketing post
        /// Creates a new post with copied content, appends "(Copy)" to title,
        /// sets status to Draft, and resets analytics counters
        /// </summary>
        /// <param name="id">Source marketing post ID</param>
        /// <returns>Newly created duplicate post</returns>
        /// <response code="200">Returns the newly created duplicate post</response>
        /// <response code="401">If the user is not authenticated</response>
        /// <response code="403">If the user does not have admin role</response>
        /// <response code="404">If the source marketing post is not found</response>
        [HttpPost("{id}/duplicate")]
        [ProducesResponseType(typeof(ApiResponse<MarketingPostResponseDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ApiResponse<MarketingPostResponseDto>), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ApiResponse<MarketingPostResponseDto>>> DuplicatePost(Guid id)
        {
            _logger.LogInformation("DUPLICATE marketing post - ID: {PostId}", id);

            var result = await _marketingPostService.DuplicatePostAsync(id);

            if (!result.Success)
            {
                if (result.Message?.Contains("denied", StringComparison.OrdinalIgnoreCase) == true ||
                    result.Message?.Contains("Unauthorized", StringComparison.OrdinalIgnoreCase) == true)
                    return StatusCode(StatusCodes.Status403Forbidden, result);

                if (result.Message?.Contains("not found", StringComparison.OrdinalIgnoreCase) == true)
                    return NotFound(result);

                return BadRequest(result);
            }

            return Ok(result);
        }

        /// <summary>
        /// Get aggregate statistics for all marketing posts
        /// Returns counts by status and total analytics metrics
        /// </summary>
        /// <returns>Marketing post statistics</returns>
        /// <response code="200">Returns the marketing post statistics</response>
        /// <response code="401">If the user is not authenticated</response>
        /// <response code="403">If the user does not have admin role</response>
        [HttpGet("statistics")]
        [ProducesResponseType(typeof(ApiResponse<MarketingPostStatisticsDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<ApiResponse<MarketingPostStatisticsDto>>> GetStatistics()
        {
            _logger.LogInformation("GET marketing post statistics");

            var result = await _marketingPostService.GetStatisticsAsync();

            if (!result.Success)
            {
                if (result.Message?.Contains("denied", StringComparison.OrdinalIgnoreCase) == true ||
                    result.Message?.Contains("Unauthorized", StringComparison.OrdinalIgnoreCase) == true)
                    return StatusCode(StatusCodes.Status403Forbidden, result);

                return BadRequest(result);
            }

            return Ok(result);
        }

        /// <summary>
        /// Get all marketing posts linked to a specific product
        /// </summary>
        /// <param name="productId">Product ID</param>
        /// <returns>List of marketing posts linked to the product</returns>
        /// <response code="200">Returns the list of marketing posts</response>
        /// <response code="401">If the user is not authenticated</response>
        /// <response code="403">If the user does not have admin role</response>
        [HttpGet("by-product/{productId}")]
        [ProducesResponseType(typeof(ApiResponse<List<MarketingPostListDto>>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<ApiResponse<List<MarketingPostListDto>>>> GetPostsByProduct(Guid productId)
        {
            _logger.LogInformation("GET marketing posts by product - ProductId: {ProductId}", productId);

            var result = await _marketingPostService.GetPostsByProductAsync(productId);

            if (!result.Success)
            {
                if (result.Message?.Contains("denied", StringComparison.OrdinalIgnoreCase) == true ||
                    result.Message?.Contains("Unauthorized", StringComparison.OrdinalIgnoreCase) == true)
                    return StatusCode(StatusCodes.Status403Forbidden, result);

                return BadRequest(result);
            }

            return Ok(result);
        }

        #endregion

        #region Bulk Operations

        /// <summary>
        /// Soft delete multiple marketing posts in a single transaction
        /// Returns detailed results indicating success/failure for each post
        /// </summary>
        /// <param name="dto">Bulk delete request with list of post IDs</param>
        /// <returns>Bulk operation results</returns>
        /// <response code="200">Returns the bulk operation results</response>
        /// <response code="400">If the request is invalid</response>
        /// <response code="401">If the user is not authenticated</response>
        /// <response code="403">If the user does not have admin role</response>
        [HttpPost("bulk/delete")]
        [ProducesResponseType(typeof(ApiResponse<BulkOperationResult>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<BulkOperationResult>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<ApiResponse<BulkOperationResult>>> BulkDelete(
            [FromBody] BulkDeleteDto dto)
        {
            _logger.LogInformation("BULK DELETE marketing posts - Count: {Count}", dto.PostIds.Count);

            if (dto.PostIds == null || !dto.PostIds.Any())
            {
                return BadRequest(ApiResponse<BulkOperationResult>.FailureResponse(
                    "Post IDs list cannot be empty"));
            }

            var result = await _marketingPostService.BulkDeleteAsync(dto.PostIds);

            if (!result.Success)
            {
                if (result.Message?.Contains("denied", StringComparison.OrdinalIgnoreCase) == true ||
                    result.Message?.Contains("Unauthorized", StringComparison.OrdinalIgnoreCase) == true)
                    return StatusCode(StatusCodes.Status403Forbidden, result);

                return BadRequest(result);
            }

            return Ok(result);
        }

        /// <summary>
        /// Publish multiple draft posts in a single transaction
        /// Returns detailed results indicating success/failure for each post
        /// </summary>
        /// <param name="dto">Bulk publish request with list of post IDs</param>
        /// <returns>Bulk operation results</returns>
        /// <response code="200">Returns the bulk operation results</response>
        /// <response code="400">If the request is invalid</response>
        /// <response code="401">If the user is not authenticated</response>
        /// <response code="403">If the user does not have admin role</response>
        [HttpPost("bulk/publish")]
        [ProducesResponseType(typeof(ApiResponse<BulkOperationResult>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<BulkOperationResult>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<ApiResponse<BulkOperationResult>>> BulkPublish(
            [FromBody] BulkPublishDto dto)
        {
            _logger.LogInformation("BULK PUBLISH marketing posts - Count: {Count}", dto.PostIds.Count);

            if (dto.PostIds == null || !dto.PostIds.Any())
            {
                return BadRequest(ApiResponse<BulkOperationResult>.FailureResponse(
                    "Post IDs list cannot be empty"));
            }

            var result = await _marketingPostService.BulkPublishAsync(dto.PostIds);

            if (!result.Success)
            {
                if (result.Message?.Contains("denied", StringComparison.OrdinalIgnoreCase) == true ||
                    result.Message?.Contains("Unauthorized", StringComparison.OrdinalIgnoreCase) == true)
                    return StatusCode(StatusCodes.Status403Forbidden, result);

                return BadRequest(result);
            }

            return Ok(result);
        }

        /// <summary>
        /// Update status for multiple marketing posts in a single transaction
        /// Returns detailed results indicating success/failure for each post
        /// </summary>
        /// <param name="dto">Bulk status update request with list of post IDs and new status</param>
        /// <returns>Bulk operation results</returns>
        /// <response code="200">Returns the bulk operation results</response>
        /// <response code="400">If the request is invalid</response>
        /// <response code="401">If the user is not authenticated</response>
        /// <response code="403">If the user does not have admin role</response>
        [HttpPost("bulk/status")]
        [ProducesResponseType(typeof(ApiResponse<BulkOperationResult>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<BulkOperationResult>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<ApiResponse<BulkOperationResult>>> BulkUpdateStatus(
            [FromBody] BulkUpdateStatusDto dto)
        {
            _logger.LogInformation("BULK UPDATE STATUS marketing posts - Count: {Count}, Status: {Status}",
                dto.PostIds.Count, dto.Status);

            if (dto.PostIds == null || !dto.PostIds.Any())
            {
                return BadRequest(ApiResponse<BulkOperationResult>.FailureResponse(
                    "Post IDs list cannot be empty"));
            }

            var result = await _marketingPostService.BulkUpdateStatusAsync(dto.PostIds, dto.Status);

            if (!result.Success)
            {
                if (result.Message?.Contains("denied", StringComparison.OrdinalIgnoreCase) == true ||
                    result.Message?.Contains("Unauthorized", StringComparison.OrdinalIgnoreCase) == true)
                    return StatusCode(StatusCodes.Status403Forbidden, result);

                return BadRequest(result);
            }

            return Ok(result);
        }

        #endregion
    }
}
