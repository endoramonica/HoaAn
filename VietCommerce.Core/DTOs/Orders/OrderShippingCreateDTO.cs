using System.ComponentModel.DataAnnotations;

namespace VietCommerce.Core.DTOs.Orders;

public class OrderShippingCreateDTO
{
    [Required(ErrorMessage = "Shipping address is required")]
    [StringLength(500, ErrorMessage = "Shipping address cannot exceed 500 characters")]
    public string Address { get; set; } = string.Empty;

    [StringLength(100, ErrorMessage = "City cannot exceed 100 characters")]
    public string? City { get; set; }

    [StringLength(10, ErrorMessage = "Zip code cannot exceed 10 characters")]
    public string? ZipCode { get; set; }

    [StringLength(100, ErrorMessage = "Country cannot exceed 100 characters")]
    public string? Country { get; set; }
}