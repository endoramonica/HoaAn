namespace VietCommerce.Core.DTOs.Payments;

public class CalculateOrderRequest
    {
        public Guid OrderId { get; set; }
        public string? CouponCode { get; set; }
        public Guid? ShippingMethodId { get; set; }
    }
