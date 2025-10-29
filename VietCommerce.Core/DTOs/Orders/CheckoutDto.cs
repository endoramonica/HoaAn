using System.ComponentModel.DataAnnotations;


namespace VietCommerce.Core.DTOs.Orders;

public class CheckoutDto
{
    [Required(ErrorMessage = "Cart ID is required")]
    public Guid CartId { get; set; }

    [Required(ErrorMessage = "Shipping information is required")]
    public OrderShippingInputDto ShippingInfo { get; set; } = null!;

    [MaxLength(50, ErrorMessage = "Coupon code cannot exceed 50 characters")]
    public string? CouponCode { get; set; }

    [MaxLength(500, ErrorMessage = "Notes cannot exceed 500 characters")]
    public string? Notes { get; set; }
}
