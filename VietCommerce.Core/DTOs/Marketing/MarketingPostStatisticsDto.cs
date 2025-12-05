namespace VietCommerce.Core.DTOs.Marketing;

/// <summary>
/// DTO for marketing post statistics and aggregate data
/// </summary>
public class MarketingPostStatisticsDto
{
    public int Total { get; set; }
    public int Draft { get; set; }
    public int Published { get; set; }
    public int Scheduled { get; set; }

    public int TotalViews { get; set; }
    public int TotalClicks { get; set; }
    public int TotalShares { get; set; }

    public double AverageViews { get; set; }
    public double AverageClicks { get; set; }
    public double AverageShares { get; set; }
}
