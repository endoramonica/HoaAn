using VietCommerce.Core.Enums.Orders;


namespace VietCommerce.Core.DTOs.Orders;

// ============================================================
// Helper DTOs

public class OrderStatusDurationDto
{
    public OrderStatus Status { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime? EndTime { get; set; }
    public double DurationHours { get; set; }
    public string? Notes { get; set; }
}
