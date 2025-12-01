using AutoMapper;
using VietCommerce.Core.DTOs.Posts;
using VietCommerce.Core.DTOs.Comments;
using VietCommerce.Core.DTOs.Customers;
using VietCommerce.Core.Entities.Customers;

namespace VietCommerce.Application.Mappings;

/// <summary>
/// AutoMapper profile cho Post entity
/// Mapping từ Entity → DTOs (không mapping ngược lại để tránh over-posting)
/// </summary>
public class PostMappingProfile : Profile
{
    public PostMappingProfile()
    {
        // ========== POST MAPPINGS ==========

        // Post → PostResponseDto
        CreateMap<Post, PostResponseDto>()
            .ForMember(dest => dest.PostId, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.CustomerName, opt => opt.MapFrom(src => src.Customer.Name))
            .ForMember(dest => dest.CustomerAvatar, opt => opt.MapFrom(src => src.Customer.CustomerAvatar))
            .ForMember(dest => dest.PhotoUrl, opt => opt.MapFrom(src => src.PhotoUrl))
            .ForMember(dest => dest.LikesCount, opt => opt.MapFrom(src => src.Likes.Count))
            .ForMember(dest => dest.CommentsCount, opt => opt.MapFrom(src => src.Comments.Count(c => !c.IsDeleted)))
            .ForMember(dest => dest.BookmarksCount, opt => opt.MapFrom(src => src.Bookmarks.Count))
            .ForMember(dest => dest.IsLikedByCurrentUser, opt => opt.Ignore())
            .ForMember(dest => dest.IsBookmarkedByCurrentUser, opt => opt.Ignore())
            .ForMember(dest => dest.IsOwnedByCurrentUser, opt => opt.Ignore());

        // Post → PostFeedDto (dùng LatestComments)
        CreateMap<Post, PostFeedDto>()
            .IncludeBase<Post, PostResponseDto>()
            .ForMember(dest => dest.LatestComments, opt => opt.MapFrom(src =>
                src.Comments
                    .Where(c => !c.IsDeleted)
                    .OrderByDescending(c => c.AddedOn)
                    .Take(3)
                    .ToList()));

        // Post → PostDetailDto (chi tiết, set Comments + RecentLikes manually)
        CreateMap<Post, PostDetailDto>()
            .IncludeBase<Post, PostResponseDto>()
            .ForMember(dest => dest.Comments, opt => opt.Ignore())
            .ForMember(dest => dest.RecentLikes, opt => opt.Ignore());

        // ========== COMMENT MAPPINGS ==========
        // ✅ Map Comment entity to CommentDto
        CreateMap<Comment, CommentDto>()
            .ForMember(dest => dest.CommentId, opt => opt.MapFrom(src => src.Id))  // ← ADD THIS!
            .ForMember(dest => dest.CustomerName, opt => opt.MapFrom(src => src.Customer != null ? src.Customer.Name : "Unknown"))
            .ForMember(dest => dest.CustomerAvatar, opt => opt.MapFrom(src => src.Customer != null ? src.Customer.CustomerAvatar : string.Empty))
            .ForMember(dest => dest.RepliesCount, opt => opt.MapFrom(src => src.Replies != null ? src.Replies.Count : 0));

        // ✅ Map Comment entity to CommentDetailDto
        CreateMap<Comment, CommentDetailDto>()
            .ForMember(dest => dest.CommentId, opt => opt.MapFrom(src => src.Id))  // ← ADD THIS!
            .ForMember(dest => dest.CustomerName, opt => opt.MapFrom(src => src.Customer != null ? src.Customer.Name : "Unknown"))
            .ForMember(dest => dest.CustomerAvatar, opt => opt.MapFrom(src => src.Customer != null ? src.Customer.CustomerAvatar : string.Empty))
            .ForMember(dest => dest.Replies, opt => opt.MapFrom(src => src.Replies));

        // ✅ Map Comment entity to CommentPreviewDto
        CreateMap<Comment, CommentPreviewDto>()
            .ForMember(dest => dest.CommentId, opt => opt.MapFrom(src => src.Id))  // ← ADD THIS!
            .ForMember(dest => dest.CustomerName, opt => opt.MapFrom(src => src.Customer != null ? src.Customer.Name : "Unknown"))
            .ForMember(dest => dest.CustomerAvatar, opt => opt.MapFrom(src => src.Customer != null ? src.Customer.CustomerAvatar : string.Empty));

        // ========== CUSTOMER MAPPINGS ==========

        CreateMap<Customer, CustomerDto>()
            .ForMember(dest => dest.CustomerAvatar, opt => opt.MapFrom(src => src.CustomerAvatar));
    }
}
