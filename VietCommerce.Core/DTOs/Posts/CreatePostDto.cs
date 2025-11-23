using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VietCommerce.Core.DTOs.Comments;
using VietCommerce.Core.DTOs.Customers;
using VietCommerce.Core.Models;

namespace VietCommerce.Core.DTOs.Posts
{
    public class CreatePostDto
    {
        public Guid CustomerId { get; set; }

        [MaxLength(5000)]
        public string? Content { get; set; }

        public IFormFile? PhotoFile { get; set; }

        public DateTime? NotificationOn { get; set; }
    }
    public class UpdatePostDto
    {
        [MaxLength(5000)]
        public string? Content { get; set; }

        public IFormFile? PhotoFile { get; set; }

        public bool RemovePhoto { get; set; } = false;

        public byte[] RowVersion { get; set; }  // For concurrency
    }
    public class PostResponseDto
    {
        public Guid Id { get; set; }
        public Guid CustomerId { get; set; }
        public string? CustomerName { get; set; }
        public string? CustomerAvatar { get; set; }

        public string? Content { get; set; }
        public string? PhotoUrl { get; set; }
        public string? ThumbnailUrl { get; set; }

        public DateTime PostedOn { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        public int LikesCount { get; set; }
        public int CommentsCount { get; set; }
        public int BookmarksCount { get; set; }

        // For current user
        public bool IsLikedByCurrentUser { get; set; }
        public bool IsBookmarkedByCurrentUser { get; set; }
        public bool IsOwnedByCurrentUser { get; set; }
    }
    public class PostFeedDto : PostResponseDto
    {
        public List<CommentPreviewDto>? LatestComments { get; set; }  // Top 3 comments
    }

    public class PostDetailDto : PostResponseDto
    {
        public PaginatedResult<CommentDto>? Comments { get; set; }
        public List<CustomerDto>? RecentLikes { get; set; }  // Top 10 who liked
    }

}
