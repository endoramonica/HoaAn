using VietCommerce.Core.Enums.Marketing;

namespace VietCommerce.Core.DTOs.Marketing;

/// <summary>
/// Query parameters for filtering and paginating marketing posts
/// </summary>
public class GetMarketingPostsQuery
{
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 20;

    public MarketingPostStatus? Status { get; set; }
    public Guid? ProductId { get; set; }
    public string? Platform { get; set; }
    public string? SearchTerm { get; set; }
    public string? DisplayLocation { get; set; }
    public bool? IsFeatured { get; set; }
    public int? MinPriorityScore { get; set; }

    public DateTime? FromDate { get; set; }
    public DateTime? ToDate { get; set; }

    public string SortBy { get; set; } = "priority"; // priority, publishedDate, views, clicks, shares, updatedAt
    public string SortOrder { get; set; } = "desc"; // asc, desc

    public bool IncludeDeleted { get; set; } = false;
}
