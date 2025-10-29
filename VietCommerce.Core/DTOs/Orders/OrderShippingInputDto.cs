using System.ComponentModel.DataAnnotations;
using VietCommerce.Core.Enums.Orders;

[Serializable]
public class OrderShippingInputDto
{
    [Required(ErrorMessage = "Recipient name is required")]
    [MaxLength(100, ErrorMessage = "Recipient name cannot exceed 100 characters")]
    public string RecipientName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Phone number is required")]
    [RegularExpression(@"^[0-9]{10,11}$", ErrorMessage = "Phone number must be 10-11 digits")]
    public string PhoneNumber { get; set; } = string.Empty;

    [Required(ErrorMessage = "Address is required")]
    [MaxLength(255, ErrorMessage = "Address cannot exceed 255 characters")]
    public string Address { get; set; } = string.Empty;

    [Required(ErrorMessage = "Ward is required")]
    [MaxLength(100, ErrorMessage = "Ward cannot exceed 100 characters")]
    public string Ward { get; set; } = string.Empty;

    [Required(ErrorMessage = "District is required")]
    [MaxLength(100, ErrorMessage = "District cannot exceed 100 characters")]
    public string District { get; set; } = string.Empty;

    [Required(ErrorMessage = "City is required")]
    [MaxLength(100, ErrorMessage = "City cannot exceed 100 characters")]
    public string City { get; set; } = string.Empty;

    [MaxLength(20, ErrorMessage = "Postal code cannot exceed 20 characters")]
    public string? PostalCode { get; set; }

    [MaxLength(500, ErrorMessage = "Delivery note cannot exceed 500 characters")]
    public string? DeliveryNote { get; set; }

    [Required(ErrorMessage = "Shipping method is required")]
    public ShippingMethodEnum ShippingMethod { get; set; }
}