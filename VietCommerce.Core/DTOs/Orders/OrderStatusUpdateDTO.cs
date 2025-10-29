using System;
using VietCommerce.Core.Enums.Orders;

namespace VietCommerce.Core.DTOs.Orders
{
    public class OrderStatusUpdateDTO
    {
        public OrderStatus Status { get; set; }
        public string? Notes { get; set; }
    }
}
