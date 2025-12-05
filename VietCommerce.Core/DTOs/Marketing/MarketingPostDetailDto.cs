namespace VietCommerce.Core.DTOs.Marketing;

/// <summary>
/// DTO for marketing post detail view with full information including audit fields
/// </summary>
public class MarketingPostDetailDto : MarketingPostResponseDto
{
    public Guid CreatedBy { get; set; }
    public Guid? UpdatedBy { get; set; }
}
