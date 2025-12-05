namespace VietCommerce.Core.DTOs.Marketing;

/// <summary>
/// DTO for marketing post list view (paginated results)
/// Contains summary information for efficient list display
/// </summary>
public class MarketingPostListDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? ShortDescription { get; set; }
    public string? Image { get; set; }

    public Guid? ProductId { get; set; }
    public string? ProductName { get; set; }

    public string? Platform { get; set; }
    public List<string>? Hashtags { get; set; }

    public int PriorityScore { get; set; }
    public bool IsFeatured { get; set; }

    public string Status { get; set; } = string.Empty;
    public DateTime? ScheduledDate { get; set; }
    public DateTime? PublishedDate { get; set; }

    public int Views { get; set; }
    public int Clicks { get; set; }
    public int Shares { get; set; }

    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
