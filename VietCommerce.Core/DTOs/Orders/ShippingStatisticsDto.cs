namespace VietCommerce.Core.DTOs.Orders;

public class ShippingStatisticsDto
{
    public int TotalShipments { get; set; }
    public int PreparedCount { get; set; }
    public int PickedUpCount { get; set; }
    public int InTransitCount { get; set; }
    public int DeliveredCount { get; set; }
    public int FailedCount { get; set; }
    public int ReturnedCount { get; set; }
    public decimal TotalShippingCost { get; set; }
    public double SuccessfulDeliveryRate { get; set; } // Percentage 0-100
}