using VietCommerce.Core.Enums.Orders;


namespace VietCommerce.Core.DTOs.Orders;

public class AverageStatusDurationDto
{
    public OrderStatus Status { get; set; }
    public double AverageDurationHours { get; set; }
    public double MinDurationHours { get; set; }
    public double MaxDurationHours { get; set; }
    public int Count { get; set; }
}
