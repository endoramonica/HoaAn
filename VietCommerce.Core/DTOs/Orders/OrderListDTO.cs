using VietCommerce.Core.Enums;
using VietCommerce.Core.Enums.Orders;
namespace VietCommerce.Core.DTOs.Orders;
public class OrderListDTO
{
    public Guid Id { get; set; }
    public Guid CustomerId { get; set; }
    public Guid StoreId { get; set; }
    public decimal TotalAmount { get; set; }
    public OrderStatus Status { get; set; }
    public DateTime CreatedDate { get; set; }
    public int ItemsCount { get; set; }
}
