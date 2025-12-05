using VietCommerce.Core.Enums.Orders;


namespace VietCommerce.Core.DTOs.Orders;

// ========================================
// REQUEST DTOs
// ========================================


/// <summary>
/// Request to bulk update order statuses
/// </summary>
public class BulkUpdateStatusRequest
{
    /// <summary>
    /// List of order IDs to update
    /// </summary>
    public List<Guid> OrderIds { get; set; } = new();

    /// <summary>
    /// New status to apply
    /// </summary>
    public OrderStatus NewStatus { get; set; }

    /// <summary>
    /// Optional reason for the update
    /// </summary>
    public string? Reason { get; set; }
}
