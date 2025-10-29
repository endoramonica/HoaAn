// VietCommerce.Core/DTOs/Cart/UpdateCartItemDto.cs
namespace VietCommerce.Core.DTOs.Cart
{
    public class UpdateCartItemDto
    {
        public Guid CartItemId { get; set; }
        public int Quantity { get; set; }
    }
}

