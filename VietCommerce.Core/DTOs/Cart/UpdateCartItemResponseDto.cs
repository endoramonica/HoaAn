// VietCommerce.Core/DTOs/Cart/UpdateCartItemResponseDto.cs
namespace VietCommerce.Core.DTOs.Cart
{
    public class UpdateCartItemResponseDto
    {
        public Guid CartItemId { get; set; }
        public int NewQuantity { get; set; }
        public decimal NewLineTotal { get; set; }
        public decimal CartTotalAmount { get; set; }
    }
}