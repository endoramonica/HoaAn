using System.ComponentModel.DataAnnotations;
using VietCommerce.Core.Enums.Marketing;

namespace VietCommerce.Core.DTOs.Marketing;

/// <summary>
/// DTO for updating an existing marketing post
/// All fields are optional to allow partial updates
/// </summary>
public class UpdateMarketingPostDto
{
    [StringLength(200, MinimumLength = 3, ErrorMessage = "Title must be between 3 and 200 characters")]
    public string? Title { get; set; }

    [StringLength(10000, MinimumLength = 10, ErrorMessage = "Content must be at least 10 characters")]
    public string? Content { get; set; }

    [StringLength(500, ErrorMessage = "Short description cannot exceed 500 characters")]
    public string? ShortDescription { get; set; }

    public string? ImageUrl { get; set; }
    public string? ImageData { get; set; }
    public List<string>? ImageUrls { get; set; }

    public Guid? ProductId { get; set; }

    public string? Topic { get; set; }
    public string? Platform { get; set; }
    public string? Tone { get; set; }
    public List<string>? Hashtags { get; set; }

    [Range(1, 100, ErrorMessage = "Priority score must be between 1 and 100")]
    public int? PriorityScore { get; set; }

    public List<string>? DisplayLocation { get; set; }

    [StringLength(200, ErrorMessage = "Meta title cannot exceed 200 characters")]
    public string? MetaTitle { get; set; }

    [StringLength(500, ErrorMessage = "Meta description cannot exceed 500 characters")]
    public string? MetaDescription { get; set; }

    public List<string>? MetaKeywords { get; set; }

    public SocialMediaPostsDto? SocialPosts { get; set; }

    public MarketingPostStatus? Status { get; set; }

    public DateTime? ScheduledDate { get; set; }

    public byte[]? RowVersion { get; set; } // For concurrency control
}
