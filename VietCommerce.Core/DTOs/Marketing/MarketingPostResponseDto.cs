using VietCommerce.Core.DTOs.Products;

namespace VietCommerce.Core.DTOs.Marketing;

/// <summary>
/// DTO for marketing post response with full details
/// </summary>
public class MarketingPostResponseDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public string? ShortDescription { get; set; }

    public string? Image { get; set; } // Primary image
    public List<string>? Images { get; set; }

    public Guid? ProductId { get; set; }
    public string? ProductName { get; set; }
    public TaggedProductDto? TaggedProduct { get; set; } // Live product data

    public string? Topic { get; set; }
    public string? Platform { get; set; }
    public string? Tone { get; set; }
    public List<string>? Hashtags { get; set; }

    public int PriorityScore { get; set; }
    public List<string>? DisplayLocation { get; set; }
    public bool IsFeatured { get; set; }

    public string? MetaTitle { get; set; }
    public string? MetaDescription { get; set; }
    public List<string>? MetaKeywords { get; set; }

    public SocialMediaPostsDto? SocialPosts { get; set; }

    public string Status { get; set; } = string.Empty;
    public DateTime? ScheduledDate { get; set; }
    public DateTime? PublishedDate { get; set; }

    public int Views { get; set; }
    public int Clicks { get; set; }
    public int Shares { get; set; }

    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
