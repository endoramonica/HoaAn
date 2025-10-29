using System.ComponentModel.DataAnnotations;
namespace VietCommerce.Core.DTOs.Orders;

public class OrderItemCreateDTO
{
    [Required(ErrorMessage = "Product ID is required")]
    public Guid ProductId { get; set; }
    [Required(ErrorMessage = "Quantity is required")]
    [Range(1, int.MaxValue, ErrorMessage = "Quantity must be at least 1")]
    public int Quantity { get; set; }


}
