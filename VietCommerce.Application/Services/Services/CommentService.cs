using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VietCommerce.Application.Services.Services.Interfaces;
using VietCommerce.Application.Services.Services.Interfaces.Identities;
using VietCommerce.Core.DTOs.Comments;
using VietCommerce.Core.Models;
using VietCommerce.Data.Repositories.Interfaces;

namespace VietCommerce.Application.Services.Services
{
    /// <summary>
    /// ✅ SECURE & OPTIMIZED Implementation của ICommentService
    /// </summary>
    public class CommentService : ICommentService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICommentRepository _commentRepository;
        private readonly IMapper _mapper;
        private readonly ICurrentUser _currentUser;
        private readonly ILogger<CommentService> _logger;
        // TODO: Inject INotificationService
        // private readonly INotificationService _notificationService;

        public CommentService(
            IUnitOfWork unitOfWork,
            ICommentRepository commentRepository,
            IMapper mapper,
            ICurrentUser currentUser,
            ILogger<CommentService> logger)
        {
            _unitOfWork = unitOfWork;
            _commentRepository = commentRepository;
            _mapper = mapper;
            _currentUser = currentUser;
            _logger = logger;
        }

        #region Create Comment

        public async Task<ApiResponse<CommentDto>> CreateCommentAsync(CreateCommentDto dto)
        {
            try
            {
                var customerId = _currentUser.CustomerId;
                if (customerId == Guid.Empty)
                {
                    return ApiResponse<CommentDto>.FailureResponse("Unauthorized");
                }

                // Validate post exists
                var postOwnerId = await _commentRepository.GetPostOwnerIdAsync(dto.PostId);
                if (!postOwnerId.HasValue)
                {
                    return ApiResponse<CommentDto>.FailureResponse("Post not found");
                }

                // Validate parent comment if this is a reply
                if (dto.ParentCommentId.HasValue)
                {
                    var validationError = await ValidateParentCommentAsync(
                        dto.ParentCommentId.Value,
                        dto.PostId);

                    if (validationError != null)
                    {
                        return ApiResponse<CommentDto>.FailureResponse(validationError);
                    }
                }

                // Create comment
                var comment = new Comment
                {
                    Id = Guid.NewGuid(),
                    PostId = dto.PostId,
                    CustomerId = customerId,
                    Content = dto.Content,
                    ParentCommentId = dto.ParentCommentId,
                    AddedOn = DateTime.UtcNow,
                    IsActive = true
                };

                await _commentRepository.AddAsync(comment);
                await _unitOfWork.SaveChangesAsync();

                // Reload with related data
                var createdComment = await LoadCommentWithDetailsAsync(comment.Id);
                var responseDto = MapToCommentDto(createdComment, customerId);

                // TODO: Send notification
                if (postOwnerId.Value != customerId)
                {
                    _logger.LogInformation(
                        "Comment notification should be sent to {PostOwnerId}",
                        postOwnerId.Value);
                }

                return ApiResponse<CommentDto>.SuccessResponse(
                    responseDto,
                    "Comment created successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating comment for post {PostId}", dto.PostId);
                return ApiResponse<CommentDto>.FailureResponse(
                    "An error occurred while creating comment");
            }
        }

        #endregion

        #region Update Comment

        public async Task<ApiResponse<CommentDto>> UpdateCommentAsync(
            Guid commentId,
            UpdateCommentDto dto)
        {
            try
            {
                var customerId = _currentUser.UserId;
                if (customerId == Guid.Empty)
                {
                    return ApiResponse<CommentDto>.FailureResponse("Unauthorized");
                }

                var comment = await _commentRepository.GetByIdAsync(commentId);
                if (comment == null || comment.IsDeleted)
                {
                    return ApiResponse<CommentDto>.FailureResponse("Comment not found");
                }

                // Authorization check
                if (comment.CustomerId != customerId)
                {
                    return ApiResponse<CommentDto>.FailureResponse(
                        "You are not authorized to update this comment");
                }

                // Update comment
                comment.Content = dto.Content;
                //comment.ModifiedOn = DateTime.UtcNow;
                //comment.ModifiedBy = customerId;

                _commentRepository.Update(comment);

                try
                {
                    await _unitOfWork.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    return ApiResponse<CommentDto>.FailureResponse(
                        "The comment has been modified by someone else. Please reload and try again.");
                }

                // Reload with related data
                var updatedComment = await LoadCommentWithDetailsAsync(commentId);
                var responseDto = MapToCommentDto(updatedComment, customerId);

                return ApiResponse<CommentDto>.SuccessResponse(
                    responseDto,
                    "Comment updated successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating comment {CommentId}", commentId);
                return ApiResponse<CommentDto>.FailureResponse(
                    "An error occurred while updating comment");
            }
        }

        #endregion

        #region Delete Comment

        public async Task<ApiResponse<bool>> DeleteCommentAsync(Guid commentId)
        {
            try
            {
                var customerId = _currentUser.UserId;
                if (customerId == Guid.Empty)
                {
                    return ApiResponse<bool>.FailureResponse("Unauthorized");
                }

                var comment = await _commentRepository.GetByIdAsync(commentId);
                if (comment == null || comment.IsDeleted)
                {
                    return ApiResponse<bool>.FailureResponse("Comment not found");
                }

                // Authorization check
                if (comment.CustomerId != customerId)
                {
                    return ApiResponse<bool>.FailureResponse(
                        "You are not authorized to delete this comment");
                }

                // Soft delete
                comment.IsDeleted = true;
                comment.DeletedAt = DateTime.UtcNow;
                comment.DeletedBy = customerId;

                _commentRepository.Update(comment);
                await _unitOfWork.SaveChangesAsync();

                return ApiResponse<bool>.SuccessResponse(
                    true,
                    "Comment deleted successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting comment {CommentId}", commentId);
                return ApiResponse<bool>.FailureResponse(
                    "An error occurred while deleting comment");
            }
        }

        #endregion

        #region Get Comments

        public async Task<ApiResponse<PaginatedResult<CommentDto>>> GetCommentsByPostIdAsync(
            Guid postId,
            int pageNumber = 1,
            int pageSize = 20)
        {
            try
            {
                // Validate post exists
                var postOwnerId = await _commentRepository.GetPostOwnerIdAsync(postId);
                if (!postOwnerId.HasValue)
                {
                    return ApiResponse<PaginatedResult<CommentDto>>.FailureResponse("Post not found");
                }

                var startIndex = (pageNumber - 1) * pageSize;
                var (comments, totalCount) = await _commentRepository
                    .GetCommentsByPostIdAsync(postId, startIndex, pageSize);

                var currentUserId = _currentUser.UserId;
                var commentDtos = comments
                    .Select(c => MapToCommentDto(c, currentUserId))
                    .ToList();

                var paginatedResult = new PaginatedResult<CommentDto>(
                    commentDtos,
                    pageNumber,
                    pageSize,
                    totalCount);

                return ApiResponse<PaginatedResult<CommentDto>>.SuccessResponse(
                    paginatedResult,
                    "Comments retrieved successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting comments for post {PostId}", postId);
                return ApiResponse<PaginatedResult<CommentDto>>.FailureResponse(
                    "An error occurred while retrieving comments");
            }
        }

        public async Task<ApiResponse<CommentDetailDto>> GetCommentByIdAsync(Guid commentId)
        {
            try
            {
                var comment = await _unitOfWork.Comments.GetCommentWithDetailsAsync(commentId);

                if (comment == null)
                    return ApiResponse<CommentDetailDto>.FailureResponse("Comment not found");

                var currentUserId = _currentUser.UserId;

                var detailDto = _mapper.Map<CommentDetailDto>(comment);
                detailDto.IsOwnedByCurrentUser = comment.CustomerId == currentUserId;

                foreach (var reply in detailDto.Replies)
                {
                    var replyEntity = comment.Replies.First(r => r.Id == reply.CommentId);
                    reply.IsOwnedByCurrentUser = replyEntity.CustomerId == currentUserId;
                }

                return ApiResponse<CommentDetailDto>.SuccessResponse(detailDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading comment {CommentId}", commentId);
                return ApiResponse<CommentDetailDto>.FailureResponse("An error occurred");
            }
        }


        public async Task<ApiResponse<List<CommentDto>>> GetRepliesByCommentIdAsync(Guid commentId)
        {
            try
            {
                var parentComment = await _unitOfWork.Comments.GetByIdAsync(commentId);
                if (parentComment == null || parentComment.IsDeleted)
                {
                    return ApiResponse<List<CommentDto>>.FailureResponse("Comment not found");
                }

                var replies = await _commentRepository.GetRepliesByCommentIdAsync(commentId);

                var currentUserId = _currentUser.UserId;
                var replyDtos = replies
                    .Select(r => MapToCommentDto(r, currentUserId))
                    .ToList();

                return ApiResponse<List<CommentDto>>.SuccessResponse(
                    replyDtos,
                    "Replies retrieved successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting replies for comment {CommentId}", commentId);
                return ApiResponse<List<CommentDto>>.FailureResponse(
                    "An error occurred while retrieving replies");
            }
        }

        #endregion

        #region Legacy Support (Deprecated)

        /// <summary>
        /// ⚠️ DEPRECATED - Use CreateCommentAsync hoặc UpdateCommentAsync
        /// Kept for backward compatibility
        /// </summary>
        [Obsolete("Use CreateCommentAsync or UpdateCommentAsync instead")]
        public async Task<ApiResponse<CommentDto>> SaveCommentAsync(SaveCommentDto dto)
        {
            // Redirect to appropriate method
            if (dto.CommentId == Guid.Empty)
            {
                var createDto = new CreateCommentDto
                {
                    PostId = dto.PostId,
                    Content = dto.Content,
                    ParentCommentId = dto.ParentCommentId
                };
                return await CreateCommentAsync(createDto);
            }
            else
            {
                var updateDto = new UpdateCommentDto
                {
                    Content = dto.Content
                };
                return await UpdateCommentAsync(dto.CommentId, updateDto);
            }
        }

       

        #endregion

        #region Private Helper Methods

        /// <summary>
        /// Validate parent comment exists và thuộc cùng post
        /// </summary>
        private async Task<string?> ValidateParentCommentAsync(Guid parentCommentId, Guid postId)
        {
            var parentComment = await _commentRepository.GetByIdAsync(parentCommentId);

            if (parentComment == null || parentComment.IsDeleted)
            {
                return "Parent comment not found";
            }

            if (parentComment.PostId != postId)
            {
                return "Parent comment does not belong to this post";
            }

            return null;
        }

        /// <summary>
        /// Load comment với Customer và Replies
        /// </summary>
        private async Task<Comment> LoadCommentWithDetailsAsync(Guid commentId)
        {
            return await _unitOfWork.Comments.GetCommentWithDetailsAsync(commentId)
                ?? throw new InvalidOperationException($"Comment {commentId} not found");
        }


        /// <summary>
        /// Map Comment entity sang CommentDto với ownership check
        /// </summary>
        private CommentDto MapToCommentDto(Comment comment, Guid currentUserId)
        {
            var dto = _mapper.Map<CommentDto>(comment);
            dto.IsOwnedByCurrentUser = comment.CustomerId == currentUserId;
            return dto;
        }

        #endregion
    }
}