// VietCommerce.Core/DTOs/Cart/CartItemDto.cs
using System;

namespace VietCommerce.Core.DTOs.Cart
{
    public class CartItemDto
    {
        public Guid Id { get; set; }
        public Guid CartId { get; set; }
        public Guid ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public string ProductCode { get; set; } = string.Empty;
        public string? ProductImage { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal TotalPrice => Quantity * UnitPrice;
        public int StockAvailable { get; set; }
        public DateTime AddedAt { get; set; }
    }
}

