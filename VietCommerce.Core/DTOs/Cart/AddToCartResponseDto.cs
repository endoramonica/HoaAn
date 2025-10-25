// VietCommerce.Core/DTOs/Cart/AddToCartResponseDto.cs
namespace VietCommerce.Core.DTOs.Cart
{
    public class AddToCartResponseDto
    {
        public Guid CartItemId { get; set; }
        public Guid ProductId { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public int CartItemCount { get; set; }
        public decimal CartTotalAmount { get; set; }
    }
}