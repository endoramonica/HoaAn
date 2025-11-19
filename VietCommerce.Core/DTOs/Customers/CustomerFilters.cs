using System.ComponentModel.DataAnnotations;

namespace VietCommerce.Core.DTOs.Customers;

/// <summary>
/// Filters for customer queries
/// </summary>
public class CustomerFilters
{
    /// <summary>
    /// Search by Name, Email or Phone (partial match, case-insensitive)
    /// </summary>
    [MaxLength(100, ErrorMessage = "Search term cannot exceed 100 characters")]
    public string? SearchTerm { get; set; }

    /// <summary>
    /// Filter by customer tier
    /// </summary>
    [MaxLength(50)]
    public string? Tier { get; set; }

    /// <summary>
    /// Filter by active status
    /// </summary>
    public bool? IsActive { get; set; }

    /// <summary>
    /// Filter customers created on or after this date
    /// </summary>
    public DateTime? FromDate { get; set; }

    /// <summary>
    /// Filter customers created on or before this date
    /// </summary>
    public DateTime? ToDate { get; set; }

    /// <summary>
    /// Filter by minimum loyalty points
    /// </summary>
    [Range(0, int.MaxValue, ErrorMessage = "MinLoyaltyPoints must be non-negative")]
    public int? MinLoyaltyPoints { get; set; }

    /// <summary>
    /// Filter by maximum loyalty points
    /// </summary>
    [Range(0, int.MaxValue, ErrorMessage = "MaxLoyaltyPoints must be non-negative")]
    public int? MaxLoyaltyPoints { get; set; }

    /// <summary>
    /// Filter by email existence
    /// </summary>
    public bool? HasEmail { get; set; }

    /// <summary>
    /// Filter by phone existence
    /// </summary>
    public bool? HasPhone { get; set; }

    /// <summary>
    /// Validate filters
    /// </summary>
    public bool IsValid(out string errorMessage)
    {
        if (FromDate.HasValue && ToDate.HasValue && FromDate > ToDate)
        {
            errorMessage = "FromDate must be less than or equal to ToDate";
            return false;
        }

        if (MinLoyaltyPoints.HasValue && MaxLoyaltyPoints.HasValue && MinLoyaltyPoints > MaxLoyaltyPoints)
        {
            errorMessage = "MinLoyaltyPoints must be less than or equal to MaxLoyaltyPoints";
            return false;
        }

        errorMessage = string.Empty;
        return true;
    }
}
