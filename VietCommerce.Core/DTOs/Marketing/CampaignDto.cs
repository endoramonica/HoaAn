using VietCommerce.Core.Enums.Marketing;

namespace VietCommerce.Core.DTOs.Marketing
{
    /// <summary>
    /// DTO for Campaign response
    /// </summary>
    public class CampaignDto
    {
        public Guid Id { get; set; }
        public Guid StoreId { get; set; }
        public string CampaignName { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public CampaignType CampaignType { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public decimal Budget { get; set; }
        public decimal ActualCost { get; set; }
        public CampaignStatus Status { get; set; }
        public string TargetingRules { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public Guid? CreatedBy { get; set; }
        public Guid? UpdatedBy { get; set; }
        
        // Computed properties
        public bool IsActive { get; set; }
        public bool IsValidDateRange { get; set; }
        public decimal RemainingBudget { get; set; }
        public double BudgetUtilizationPercentage { get; set; }
    }

    /// <summary>
    /// DTO for creating a new campaign
    /// </summary>
    public class CreateCampaignDto
    {
        public Guid StoreId { get; set; }
        public string CampaignName { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public CampaignType CampaignType { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public decimal Budget { get; set; }
        public string? TargetingRules { get; set; }
    }

    /// <summary>
    /// DTO for updating a campaign
    /// </summary>
    public class UpdateCampaignDto
    {
        public string? CampaignName { get; set; }
        public string? Description { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public decimal? Budget { get; set; }
        public string? TargetingRules { get; set; }
    }

    /// <summary>
    /// DTO for campaign query parameters
    /// </summary>
    public class GetCampaignsQueryDto
    {
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public CampaignStatus? Status { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public string? SearchTerm { get; set; }
        public string? SortBy { get; set; } = "CreatedAt";
        public bool IsDescending { get; set; } = true;
    }

    /// <summary>
    /// DTO for changing campaign status
    /// </summary>
    public class ChangeCampaignStatusDto
    {
        public int NewStatus { get; set; }
    }
}
