using System;
using System.ComponentModel.DataAnnotations;
using VietCommerce.Core.Enums.Orders;

namespace VietCommerce.Core.DTOs.Orders;

/// <summary>
/// DTO for filtering and pagination of orders
/// </summary>
public class OrderFilterDTO
{
    // ========================================
    // PAGINATION
    // ========================================

    [Range(1, int.MaxValue, ErrorMessage = "Page must be at least 1")]
    public int Page { get; set; } = 1;

    [Range(1, 100, ErrorMessage = "PageSize must be between 1 and 100")]
    public int PageSize { get; set; } = 20;

    // ========================================
    // FILTERS
    // ========================================

    /// <summary>
    /// Search by OrderNumber or CustomerName (partial match, case-insensitive)
    /// </summary>
    [MaxLength(100, ErrorMessage = "Keyword cannot exceed 100 characters")]
    public string? Keyword { get; set; }

    /// <summary>
    /// Filter by specific customer
    /// </summary>
    public Guid? CustomerId { get; set; }

    /// <summary>
    /// Filter by specific store
    /// </summary>
    public Guid? StoreId { get; set; }

    /// <summary>
    /// Filter by order status (enum)
    /// Example: 1 (Pending), 2 (Confirmed), etc.
    /// </summary>
    public OrderStatus? Status { get; set; }

    /// <summary>
    /// Filter orders created on or after this date
    /// </summary>
    public DateTime? FromDate { get; set; }

    /// <summary>
    /// Filter orders created on or before this date
    /// </summary>
    public DateTime? ToDate { get; set; }

    /// <summary>
    /// Filter by minimum order amount
    /// </summary>
    [Range(0, double.MaxValue, ErrorMessage = "MinAmount must be non-negative")]
    public decimal? MinAmount { get; set; }

    /// <summary>
    /// Filter by maximum order amount
    /// </summary>
    [Range(0, double.MaxValue, ErrorMessage = "MaxAmount must be non-negative")]
    public decimal? MaxAmount { get; set; }

    // ========================================
    // SORTING
    // ========================================

    /// <summary>
    /// Sort by field: "CreatedAt", "TotalAmount", "OrderNumber", "Status", "CustomerName"
    /// </summary>
    [MaxLength(50)]
    public string SortBy { get; set; } = "CreatedAt";

    /// <summary>
    /// Sort descending (true) or ascending (false)
    /// </summary>
    public bool SortDescending { get; set; } = true;

    // ========================================
    // VALIDATION
    // ========================================

    /// <summary>
    /// Validate date range
    /// </summary>
    public bool IsValid(out string errorMessage)
    {
        if (FromDate.HasValue && ToDate.HasValue && FromDate > ToDate)
        {
            errorMessage = "FromDate must be less than or equal to ToDate";
            return false;
        }

        if (MinAmount.HasValue && MaxAmount.HasValue && MinAmount > MaxAmount)
        {
            errorMessage = "MinAmount must be less than or equal to MaxAmount";
            return false;
        }

        errorMessage = string.Empty;
        return true;
    }
}