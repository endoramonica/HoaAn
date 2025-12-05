using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using VietCommerce.Application.Services.Services.Interfaces;
using VietCommerce.Application.Services.Services.Interfaces.Identities;
using VietCommerce.Core.DTOs.Posts;
using VietCommerce.Core.Entities.Marketing;
using VietCommerce.Core.Enums.Marketing;
using VietCommerce.Core.Models;
using VietCommerce.Data.Repositories.Interfaces;

namespace VietCommerce.Application.Services.Services;

public class MixedFeedService : IMixedFeedService
{
    private IUnitOfWork _unitOfWork;
    private readonly ICurrentUser _currentUser;
    private readonly ILogger<MixedFeedService> _logger;

    public MixedFeedService(
       IUnitOfWork unitOfWork,
        ICurrentUser currentUser,
        ILogger<MixedFeedService> logger)
    {
        _unitOfWork = unitOfWork;
        _currentUser = currentUser;
        _logger = logger;
    }

    public async Task<ApiResponse<PaginatedResult<MixedFeedDto>>> GetMixedFeedAsync(MixedFeedQuery query)
    {
        try
        {
            // Validate marketing ratio
            if (query.MarketingRatio < 1 || query.MarketingRatio > 10)
            {
                return ApiResponse<PaginatedResult<MixedFeedDto>>.FailureResponse(
                    "Marketing ratio must be between 1 and 10");
            }

            var currentUserId = _currentUser.UserId;
            var mixedFeed = new List<MixedFeedDto>();

            // Calculate how many of each type we need
            var totalItems = query.PageSize;
            var marketingCount = totalItems / (query.MarketingRatio + 1);
            var communityCount = totalItems - marketingCount;

            // Get community posts
            var communityPosts = await GetCommunityPostsAsync(communityCount, currentUserId);

            // Get marketing posts
            var marketingPosts = await GetMarketingPostsAsync(
                marketingCount,
                query.Location,
                query.MinPriorityScore,
                query.ProductId,
                currentUserId);

            // Mix posts using smart algorithm
            mixedFeed = MixPosts(communityPosts, marketingPosts, query.MarketingRatio);

            // Apply pagination
            var skip = (query.PageNumber - 1) * query.PageSize;
            var pagedItems = mixedFeed.Skip(skip).Take(query.PageSize).ToList();

            // Get total count (approximate)
            var totalCommunity = await _unitOfWork.Posts.CountAsync(p => !p.IsDeleted && p.IsActive);
            var totalMarketing = await _unitOfWork.MarketingPosts.CountAsync(m => !m.IsDeleted && m.Status == MarketingPostStatus.Published);
            var totalCount = totalCommunity + totalMarketing;

            var result = new PaginatedResult<MixedFeedDto>
            {
                Items = pagedItems,
                PageNumber = query.PageNumber,
                PageSize = query.PageSize,
                TotalItems = totalCount,
                TotalPages = (int)Math.Ceiling(totalCount / (double)query.PageSize)
            };

            return ApiResponse<PaginatedResult<MixedFeedDto>>.SuccessResponse(
                result,
                "Mixed feed retrieved successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting mixed feed");
            return ApiResponse<PaginatedResult<MixedFeedDto>>.FailureResponse(
                "Failed to retrieve mixed feed");
        }
    }

    public async Task<ApiResponse<LocationPostsDto>> GetPostsByLocationAsync(
        string location,
        int pageNumber = 1,
        int pageSize = 20)
    {
        try
        {
            var currentUserId = _currentUser.UserId;

            var result = await _unitOfWork.MarketingPosts.GetPostsAsync(
                pageNumber: pageNumber,
                pageSize: pageSize,
                status: MarketingPostStatus.Published,
                displayLocation: location,
                sortBy: "priority",
                sortOrder: "desc");

            var posts = result.Items.Select(m => MapMarketingPostToDto(m, currentUserId)).ToList();

            var locationResult = new LocationPostsDto
            {
                Location = location,
                Posts = posts,
                TotalCount = result.TotalItems
            };

            return ApiResponse<LocationPostsDto>.SuccessResponse(
                locationResult,
                $"Posts for location '{location}' retrieved successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting posts by location: {Location}", location);
            return ApiResponse<LocationPostsDto>.FailureResponse(
                "Failed to retrieve posts by location");
        }
    }

    public async Task<ApiResponse<FeaturedPostsDto>> GetFeaturedPostsAsync(
        int pageNumber = 1,
        int pageSize = 10)
    {
        try
        {
            var currentUserId = _currentUser.UserId;

            var result = await _unitOfWork.MarketingPosts.GetPostsAsync(
                pageNumber: pageNumber,
                pageSize: pageSize,
                status: MarketingPostStatus.Published,
                minPriorityScore: 80,
                sortBy: "priority",
                sortOrder: "desc");

            var posts = result.Items.Select(m => MapMarketingPostToDto(m, currentUserId)).ToList();

            var featuredResult = new FeaturedPostsDto
            {
                Posts = posts,
                TotalFeatured = result.TotalItems
            };

            return ApiResponse<FeaturedPostsDto>.SuccessResponse(
                featuredResult,
                "Featured posts retrieved successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting featured posts");
            return ApiResponse<FeaturedPostsDto>.FailureResponse(
                "Failed to retrieve featured posts");
        }
    }

    public async Task<ApiResponse<RelatedPostsDto>> GetRelatedPostsByProductAsync(
        Guid productId,
        int pageNumber = 1,
        int pageSize = 20)
    {
        try
        {
            var currentUserId = _currentUser.UserId;

            // Get marketing posts for this product
            var marketingPosts = await _unitOfWork.MarketingPosts.GetByProductIdAsync(productId);
            marketingPosts = marketingPosts
                .Where(m => !m.IsDeleted && m.Status == MarketingPostStatus.Published)
                .OrderByDescending(m => m.PriorityScore)
                .ThenByDescending(m => m.PublishedDate)
                .Take(pageSize / 2)
                .ToList();

            // TODO: Get community posts that mention this product
            // This would require full-text search or product tagging in community posts
            var communityPosts = new List<Post>();

            var result = new RelatedPostsDto
            {
                ProductId = productId,
                ProductName = marketingPosts.FirstOrDefault()?.ProductName,
                MarketingPosts = marketingPosts.Select(m => MapMarketingPostToDto(m, currentUserId)).ToList(),
                CommunityPosts = communityPosts.Select(p => MapCommunityPostToDto(p, currentUserId)).ToList(),
                TotalCount = marketingPosts.Count + communityPosts.Count
            };

            return ApiResponse<RelatedPostsDto>.SuccessResponse(
                result,
                "Related posts retrieved successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting related posts for product: {ProductId}", productId);
            return ApiResponse<RelatedPostsDto>.FailureResponse(
                "Failed to retrieve related posts");
        }
    }

    public async Task<ApiResponse<PostInteractionResult>> TrackInteractionAsync(
        Guid postId,
        string postType,
        PostInteractionDto interaction)
    {
        try
        {
            if (postType.ToLower() == "marketing")
            {
                return await TrackMarketingInteractionAsync(postId, interaction);
            }
            else if (postType.ToLower() == "community")
            {
                return await TrackCommunityInteractionAsync(postId, interaction);
            }

            return ApiResponse<PostInteractionResult>.FailureResponse(
                "Invalid post type. Must be 'marketing' or 'community'");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error tracking interaction for post: {PostId}", postId);
            return ApiResponse<PostInteractionResult>.FailureResponse(
                "Failed to track interaction");
        }
    }

    #region Private Helper Methods

    private async Task<List<MixedFeedDto>> GetCommunityPostsAsync(int count, Guid? currentUserId)
    {
        var result = await _unitOfWork.Posts.GetPostFeedAsync(
            pageNumber: 1,
            pageSize: count * 2, // Get more to ensure we have enough after filtering
            currentCustomerId: currentUserId);

        return result.Items.Select(p => MapCommunityPostToDto(p, currentUserId)).ToList();
    }

    private async Task<List<MixedFeedDto>> GetMarketingPostsAsync(
        int count,
        string? location,
        int? minPriority,
        Guid? productId,
        Guid? currentUserId)
    {
        var result = await _unitOfWork.MarketingPosts.GetPostsAsync(
            pageNumber: 1,
            pageSize: count * 2,
            status: MarketingPostStatus.Published,
            productId: productId,
            displayLocation: location,
            minPriorityScore: minPriority,
            sortBy: "priority",
            sortOrder: "desc");

        return result.Items.Select(m => MapMarketingPostToDto(m, currentUserId)).ToList();
    }

    private List<MixedFeedDto> MixPosts(
        List<MixedFeedDto> communityPosts,
        List<MixedFeedDto> marketingPosts,
        int ratio)
    {
        var result = new List<MixedFeedDto>();
        int communityIndex = 0;
        int marketingIndex = 0;

        // Algorithm: Insert 1 marketing post every N community posts
        while (communityIndex < communityPosts.Count || marketingIndex < marketingPosts.Count)
        {
            // Add N community posts
            for (int i = 0; i < ratio && communityIndex < communityPosts.Count; i++)
            {
                result.Add(communityPosts[communityIndex++]);
            }

            // Add 1 marketing post
            if (marketingIndex < marketingPosts.Count)
            {
                result.Add(marketingPosts[marketingIndex++]);
            }
        }

        return result;
    }

    private MixedFeedDto MapCommunityPostToDto(Post post, Guid? currentUserId)
    {
        return new MixedFeedDto
        {
            Id = post.Id,
            PostType = "community",
            Content = post.Content,
            ImageUrl = post.PhotoUrl,
            CreatedAt = post.CreatedAt,
            CustomerId = post.CustomerId,
            CustomerName = post.Customer?.Name,
            CustomerAvatar = post.Customer?.CustomerAvatar,
            LikesCount = post.Likes?.Count ?? 0,
            CommentsCount = post.Comments?.Count ?? 0,
            IsLikedByCurrentUser = currentUserId.HasValue &&
                (post.Likes?.Any(l => l.CustomerId == currentUserId.Value) ?? false),
            IsBookmarkedByCurrentUser = currentUserId.HasValue &&
                (post.Bookmarks?.Any(b => b.CustomerId == currentUserId.Value) ?? false),
            IsOwnedByCurrentUser = currentUserId.HasValue && post.CustomerId == currentUserId.Value
        };
    }

    private MixedFeedDto MapMarketingPostToDto(MarketingPost post, Guid? currentUserId)
    {
        return new MixedFeedDto
        {
            Id = post.Id,
            PostType = "marketing",
            Title = post.Title,
            Content = post.Content,
            ShortDescription = post.ShortDescription,
            ImageUrl = post.ImageUrl,
            ImageUrls = ParseJsonArray(post.ImageUrls),
            CreatedAt = post.CreatedAt,
            PublishedDate = post.PublishedDate,
            ProductId = post.ProductId,
            ProductName = post.ProductName,
            PriorityScore = post.PriorityScore,
            IsFeatured = post.IsFeatured,
            DisplayLocation = ParseJsonArray(post.DisplayLocation),
            Hashtags = ParseJsonArray(post.Hashtags),
            ViewsCount = post.Views,
            ClicksCount = post.Clicks,
            SharesCount = post.Shares,
            IsOwnedByCurrentUser = false // Marketing posts are owned by admin
        };
    }

    private List<string>? ParseJsonArray(string? jsonArray)
    {
        if (string.IsNullOrWhiteSpace(jsonArray))
            return null;

        try
        {
            return System.Text.Json.JsonSerializer.Deserialize<List<string>>(jsonArray);
        }
        catch
        {
            return null;
        }
    }

    private async Task<ApiResponse<PostInteractionResult>> TrackMarketingInteractionAsync(
        Guid postId,
        PostInteractionDto interaction)
    {
        var post = await _unitOfWork.MarketingPosts.GetByIdAsync(postId);
        if (post == null)
        {
            return ApiResponse<PostInteractionResult>.FailureResponse("Marketing post not found");
        }

        int newCount = 0;
        switch (interaction.Action.ToLower())
        {
            case "view":
                post.Views++;
                newCount = post.Views;
                break;
            case "click":
                post.Clicks++;
                newCount = post.Clicks;
                break;
            case "share":
                post.Shares++;
                newCount = post.Shares;
                break;
            default:
                return ApiResponse<PostInteractionResult>.FailureResponse(
                    "Invalid action. Must be 'view', 'click', or 'share'");
        }

        await _unitOfWork.MarketingPosts.UpdateAsync(post);

        var result = new PostInteractionResult
        {
            Success = true,
            Action = interaction.Action,
            NewCount = newCount
        };

        return ApiResponse<PostInteractionResult>.SuccessResponse(
            result,
            $"{interaction.Action} tracked successfully");
    }

    private async Task<ApiResponse<PostInteractionResult>> TrackCommunityInteractionAsync(
        Guid postId,
        PostInteractionDto interaction)
    {
        // Community posts don't have view/click/share counters yet
        // This could be extended in the future
        return ApiResponse<PostInteractionResult>.SuccessResponse(
            new PostInteractionResult { Success = true, Action = interaction.Action },
            "Interaction tracked");
    }

    #endregion
}
