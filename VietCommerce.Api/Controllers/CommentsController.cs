using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VietCommerce.Application.Extensions;
using VietCommerce.Application.Services.Services.Interfaces;
using VietCommerce.Core.DTOs.Comments;
using VietCommerce.Core.Models;

namespace VietCommerce.Api.Controllers
{
    /// <summary>
    /// Controller xử lý các operations liên quan đến Comments
    /// </summary>
    [ApiController]
    [Route("api/v1/[controller]")]
    [Produces("application/json")]
    public class CommentsController : ControllerBase
    {
        private readonly ICommentService _commentService;

        public CommentsController(ICommentService commentService)
        {
            _commentService = commentService;
        }

        /// <summary>
        /// Tạo comment mới (root comment hoặc reply)
        /// </summary>
        /// <param name="dto">Thông tin comment cần tạo</param>
        /// <returns>Comment vừa được tạo</returns>
        [HttpPost]
        [Authorize]
        [ProducesResponseType(typeof(ApiResponse<CommentDto>), 200)]
        [ProducesResponseType(typeof(ApiResponse<CommentDto>), 400)]
        [ProducesResponseType(401)]
        public async Task<IActionResult> CreateComment([FromBody] CreateCommentDto dto)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToArray();

                return BadRequest(ApiResponse<CommentDto>.FailureResponse(
                    message: "Invalid input data",
                    errors: errors
                ));
            }

            var result = await _commentService.CreateCommentAsync(dto);

            if (!result.Success)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }

        /// <summary>
        /// Cập nhật comment (chỉ owner có quyền)
        /// </summary>
        /// <param name="commentId">ID của comment cần update</param>
        /// <param name="dto">Thông tin cập nhật</param>
        /// <returns>Comment đã được cập nhật</returns>
        [HttpPut("{commentId}")]
        [Authorize]
        [ProducesResponseType(typeof(ApiResponse<CommentDto>), 200)]
        [ProducesResponseType(typeof(ApiResponse<CommentDto>), 400)]
        [ProducesResponseType(typeof(ApiResponse<CommentDto>), 403)]
        [ProducesResponseType(typeof(ApiResponse<CommentDto>), 404)]
        [ProducesResponseType(401)]
        public async Task<IActionResult> UpdateComment(Guid commentId, [FromBody] UpdateCommentDto dto)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToArray();

                return BadRequest(ApiResponse<CommentDto>.FailureResponse(
                    message: "Invalid input data",
                    errors: errors
                ));
            }

            var result = await _commentService.UpdateCommentAsync(commentId, dto);

            if (!result.Success)
            {
                return result.Message.Contains("not found", StringComparison.OrdinalIgnoreCase)
                    ? NotFound(result)
                    : result.Message.Contains("permission", StringComparison.OrdinalIgnoreCase) ||
                      result.Message.Contains("owner", StringComparison.OrdinalIgnoreCase)
                        ? StatusCode(403, result)
                        : BadRequest(result);
            }

            return Ok(result);
        }

        /// <summary>
        /// Xóa comment (soft delete, chỉ owner có quyền)
        /// </summary>
        /// <param name="commentId">ID của comment cần xóa</param>
        /// <returns>Kết quả xóa</returns>
        [HttpDelete("{commentId}")]
        [Authorize]
        [ProducesResponseType(typeof(ApiResponse<bool>), 200)]
        [ProducesResponseType(typeof(ApiResponse<bool>), 403)]
        [ProducesResponseType(typeof(ApiResponse<bool>), 404)]
        [ProducesResponseType(401)]
        public async Task<IActionResult> DeleteComment(Guid commentId)
        {
            var result = await _commentService.DeleteCommentAsync(commentId);

            if (!result.Success)
            {
                return result.Message.Contains("not found", StringComparison.OrdinalIgnoreCase)
                    ? NotFound(result)
                    : result.Message.Contains("permission", StringComparison.OrdinalIgnoreCase) ||
                      result.Message.Contains("owner", StringComparison.OrdinalIgnoreCase)
                        ? StatusCode(403, result)
                        : BadRequest(result);
            }

            return Ok(result);
        }

        /// <summary>
        /// Lấy danh sách comments của một post với pagination
        /// </summary>
        /// <param name="postId">ID của post</param>
        /// <param name="pageNumber">Số trang (default: 1)</param>
        /// <param name="pageSize">Số lượng items trên mỗi trang (default: 20)</param>
        /// <returns>Danh sách comments với pagination</returns>
        [HttpGet("post/{postId}")]
        [ProducesResponseType(typeof(ApiResponse<PaginatedResponse<CommentDto>>), 200)]
        [ProducesResponseType(typeof(ApiResponse<PaginatedResponse<CommentDto>>), 400)]
        public async Task<IActionResult> GetCommentsByPostId(
            Guid postId,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 20)
        {
            if (pageNumber < 1)
            {
                return BadRequest(ApiResponse<PaginatedResponse<CommentDto>>.FailureResponse(
                    message: "Page number must be greater than 0"
                ));
            }

            if (pageSize < 1 || pageSize > 100)
            {
                return BadRequest(ApiResponse<PaginatedResponse<CommentDto>>.FailureResponse(
                    message: "Page size must be between 1 and 100"
                ));
            }

            var result = await _commentService.GetCommentsByPostIdAsync(postId, pageNumber, pageSize);

            if (!result.Success)
            {
                return BadRequest(ApiResponse<PaginatedResponse<CommentDto>>.FailureResponse(
                    message: result.Message ?? "Failed to get comments",
                    errors: result.Errors
                ));
            }

            // Convert PaginatedResult to PaginatedResponse
            var paginatedResponse = result.Data?.ToResponse();

            return Ok(ApiResponse<PaginatedResponse<CommentDto>>.SuccessResponse(
                data: paginatedResponse!,
                message: result.Message ?? "Comments retrieved successfully"
            ));
        }

        /// <summary>
        /// Lấy chi tiết một comment kèm replies
        /// </summary>
        /// <param name="commentId">ID của comment</param>
        /// <returns>Chi tiết comment</returns>
        [HttpGet("{commentId}")]
        [ProducesResponseType(typeof(ApiResponse<CommentDetailDto>), 200)]
        [ProducesResponseType(typeof(ApiResponse<CommentDetailDto>), 404)]
        public async Task<IActionResult> GetCommentById(Guid commentId)
        {
            var result = await _commentService.GetCommentByIdAsync(commentId);

            if (!result.Success)
            {
                return NotFound(result);
            }

            return Ok(result);
        }

        /// <summary>
        /// Lấy danh sách replies của một comment
        /// </summary>
        /// <param name="commentId">ID của comment cha</param>
        /// <returns>Danh sách replies</returns>
        [HttpGet("{commentId}/replies")]
        [ProducesResponseType(typeof(ApiResponse<List<CommentDto>>), 200)]
        [ProducesResponseType(typeof(ApiResponse<List<CommentDto>>), 404)]
        public async Task<IActionResult> GetRepliesByCommentId(Guid commentId)
        {
            var result = await _commentService.GetRepliesByCommentIdAsync(commentId);

            if (!result.Success)
            {
                return NotFound(result);
            }

            return Ok(result);
        }

        /// <summary>
        /// Save comment (Create hoặc Update dựa trên CommentId)
        /// </summary>
        /// <param name="dto">Thông tin comment</param>
        /// <returns>Comment đã được save</returns>
        [HttpPost("save")]
        [Authorize]
        [ProducesResponseType(typeof(ApiResponse<CommentDto>), 200)]
        [ProducesResponseType(typeof(ApiResponse<CommentDto>), 400)]
        [ProducesResponseType(401)]
        public async Task<IActionResult> SaveComment([FromBody] SaveCommentDto dto)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToArray();

                return BadRequest(ApiResponse<CommentDto>.FailureResponse(
                    message: "Invalid input data",
                    errors: errors
                ));
            }

            var result = await _commentService.SaveCommentAsync(dto);

            if (!result.Success)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }
    }
}