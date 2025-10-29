using VietCommerce.Core.Enums.Orders;

namespace VietCommerce.Core.DTOs.Orders;

// ==========================================
// 🔄 Nested DTO for tracking status changes
// ==========================================
public class OrderStatusHistoryDTO
{
    public OrderStatus Status { get; set; }
    public string StatusText => Status.ToString();  // ✅ Human-readable
    public DateTime ChangedAt { get; set; }
    public string? ChangedByName { get; set; }
    public string? Notes { get; set; }
}
