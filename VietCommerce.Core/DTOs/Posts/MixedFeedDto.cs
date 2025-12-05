using System;
using System.Collections.Generic;

namespace VietCommerce.Core.DTOs.Posts;

/// <summary>
/// DTO cho Mixed Feed - kết hợp Community Posts và Marketing Posts
/// </summary>
public class MixedFeedDto
{
    public Guid Id { get; set; }
    public string PostType { get; set; } = string.Empty; // "community" hoặc "marketing"

    // Common fields
    public string? Content { get; set; }
    public string? ImageUrl { get; set; }
    public List<string>? ImageUrls { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? PublishedDate { get; set; }

    // Community Post specific
    public Guid? CustomerId { get; set; }
    public string? CustomerName { get; set; }
    public string? CustomerAvatar { get; set; }

    // Marketing Post specific
    public string? Title { get; set; }
    public string? ShortDescription { get; set; }
    public Guid? ProductId { get; set; }
    public string? ProductName { get; set; }
    public int? PriorityScore { get; set; }
    public bool? IsFeatured { get; set; }
    public List<string>? DisplayLocation { get; set; }
    public List<string>? Hashtags { get; set; }

    // Engagement metrics
    public int LikesCount { get; set; }
    public int CommentsCount { get; set; }
    public int SharesCount { get; set; }
    public int ViewsCount { get; set; }
    public int ClicksCount { get; set; }

    // Current user interaction
    public bool IsLikedByCurrentUser { get; set; }
    public bool IsBookmarkedByCurrentUser { get; set; }
    public bool IsOwnedByCurrentUser { get; set; }
}

/// <summary>
/// Query parameters cho Mixed Feed
/// </summary>
public class MixedFeedQuery
{
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 20;

    /// <summary>
    /// Tỷ lệ marketing posts trong feed (1-10)
    /// Ví dụ: 3 = 1 marketing post mỗi 3 community posts
    /// </summary>
    public int MarketingRatio { get; set; } = 4; // Default: 1 marketing mỗi 4 community posts

    /// <summary>
    /// Vị trí hiển thị cụ thể (homepage_banner, product_section, featured_section)
    /// </summary>
    public string? Location { get; set; }

    /// <summary>
    /// Chỉ lấy featured posts (priority > 80)
    /// </summary>
    public bool? FeaturedOnly { get; set; }

    /// <summary>
    /// Filter theo productId
    /// </summary>
    public Guid? ProductId { get; set; }

    /// <summary>
    /// Minimum priority score cho marketing posts
    ///   /// </summary>
    public int? MinPriorityScore { get; set; }
}

/// <summary>
/// Result cho location-based posts
/// </summary>
public class LocationPostsDto
{
    public string Location { get; set; } = string.Empty;
    public List<MixedFeedDto> Posts { get; set; } = new();
    public int TotalCount { get; set; }
}

/// <summary>
/// Featured posts response
/// </summary>
public class FeaturedPostsDto
{
    public List<MixedFeedDto> Posts { get; set; } = new();
    public int TotalFeatured { get; set; }
}

/// <summary>
/// Related posts by product
/// </summary>
public class RelatedPostsDto
{
    public Guid ProductId { get; set; }
    public string? ProductName { get; set; }
    public List<MixedFeedDto> MarketingPosts { get; set; } = new();
    public List<MixedFeedDto> CommunityPosts { get; set; } = new();
    public int TotalCount { get; set; }
}

/// <summary>
/// Interaction tracking DTO
/// </summary>
public class PostInteractionDto
{
    public string Action { get; set; } = string.Empty; // "view", "click", "share"
    public string? Source { get; set; } // "feed", "detail", "search"
    public string? DeviceType { get; set; } // "mobile", "desktop", "tablet"
}

/// <summary>
/// Interaction result
/// </summary>
public class PostInteractionResult
{
    public bool Success { get; set; }
    public string Action { get; set; } = string.Empty;
    public int NewCount { get; set; }
}
