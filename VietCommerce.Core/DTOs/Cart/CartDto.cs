// VietCommerce.Core/DTOs/Cart/CartDto.cs
namespace VietCommerce.Core.DTOs.Cart
{
    public class CartDto
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public Guid CustomerId { get; set; }
        public List<CartItemDto> Items { get; set; } = new();

        // Calculated properties
        public int TotalItems => Items.Sum(i => i.Quantity);
        public decimal SubTotal => Items.Sum(i => i.TotalPrice);
        public decimal TaxAmount { get; set; } = 0;
        public decimal ShippingFee { get; set; } = 0;
        public decimal TotalAmount => SubTotal + TaxAmount + ShippingFee;

        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public bool IsActive { get; set; } = true;
    }
}



