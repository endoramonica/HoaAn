using System.ComponentModel.DataAnnotations;
using VietCommerce.Core.Enums.Marketing;

namespace VietCommerce.Core.DTOs.Marketing;

/// <summary>
/// DTO for creating a new marketing post
/// </summary>
public class CreateMarketingPostDto
{
    [Required(ErrorMessage = "Title is required")]
    [StringLength(200, MinimumLength = 3, ErrorMessage = "Title must be between 3 and 200 characters")]
    public string Title { get; set; } = string.Empty;

    [Required(ErrorMessage = "Content is required")]
    [StringLength(10000, MinimumLength = 10, ErrorMessage = "Content must be at least 10 characters")]
    public string Content { get; set; } = string.Empty;

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
    public int PriorityScore { get; set; } = 50;

    public List<string>? DisplayLocation { get; set; }

    [StringLength(200, ErrorMessage = "Meta title cannot exceed 200 characters")]
    public string? MetaTitle { get; set; }

    [StringLength(500, ErrorMessage = "Meta description cannot exceed 500 characters")]
    public string? MetaDescription { get; set; }

    public List<string>? MetaKeywords { get; set; }

    public SocialMediaPostsDto? SocialPosts { get; set; }

    public MarketingPostStatus Status { get; set; } = MarketingPostStatus.Draft;

    public DateTime? ScheduledDate { get; set; }
}
