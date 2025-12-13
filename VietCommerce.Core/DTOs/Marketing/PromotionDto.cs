using VietCommerce.Core.Enums.Marketing;

namespace VietCommerce.Core.DTOs.Marketing
{
    /// <summary>
    /// DTO for Promotion response
    /// </summary>
    public class PromotionDto
    {
        public Guid Id { get; set; }
        public Guid CampaignId { get; set; }
        public string PromotionName { get; set; } = string.Empty;
        public PromotionType PromotionType { get; set; }
        public decimal DiscountValue { get; set; }
        public decimal? MinOrderAmount { get; set; }
        public decimal? MaxDiscount { get; set; }
        public int? UsageLimit { get; set; }
        public int UsedCount { get; set; }
        public string? Conditions { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public bool IsActive { get; set; }
        public PromotionStatus Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public Guid? CreatedBy { get; set; }
        public Guid? UpdatedBy { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime? DeletedAt { get; set; }
        public Guid? DeletedBy { get; set; }
    }

    /// <summary>
    /// DTO for creating a new promotion
    /// </summary>
    public class CreatePromotionDto
    {
        public string PromotionName { get; set; } = string.Empty;
        public PromotionType PromotionType { get; set; }
        public decimal DiscountValue { get; set; }
        public decimal? MinOrderAmount { get; set; }
        public decimal? MaxDiscount { get; set; }
        public int? UsageLimit { get; set; }
        public string? Conditions { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }

    /// <summary>
    /// DTO for updating a promotion
    /// </summary>
    public class UpdatePromotionDto
    {
        public string? PromotionName { get; set; }
        public PromotionType? PromotionType { get; set; }
        public decimal? DiscountValue { get; set; }
        public decimal? MinOrderAmount { get; set; }
        public decimal? MaxDiscount { get; set; }
        public int? UsageLimit { get; set; }
        public string? Conditions { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public bool? IsActive { get; set; }
    }

    /// <summary>
    /// DTO for promotion query parameters
    /// </summary>
    public class GetPromotionsQueryDto
    {
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public PromotionStatus? Status { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public string? SearchTerm { get; set; }
        public string? SortBy { get; set; } = "CreatedAt";
        public bool IsDescending { get; set; } = true;
    }

    /// <summary>
    /// DTO for linking products to a promotion
    /// </summary>
    public class LinkProductsDto
    {
        public List<Guid> ProductIds { get; set; } = new();
        public decimal? DiscountOverride { get; set; }
    }
}
