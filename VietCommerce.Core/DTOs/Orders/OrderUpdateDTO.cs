using System.ComponentModel.DataAnnotations;
using VietCommerce.Core.Enums;
using VietCommerce.Core.Enums.Orders;
namespace VietCommerce.Core.DTOs.Orders;
public class OrderUpdateDTO
{
    public decimal? Subtotal { get; set; }
    public decimal? TaxAmount { get; set; }
    public decimal? ShippingAmount { get; set; }
    public decimal? TotalAmount { get; set; }
    public OrderStatus? Status { get; set; }
    public string? Notes { get; set; }
}
