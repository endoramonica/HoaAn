// VietCommerce.Core/DTOs/Cart/GetCartResponseDto.cs
namespace VietCommerce.Core.DTOs.Cart
{
    public class GetCartResponseDto
    {
        public Guid CartId { get; set; }
        public Guid UserId { get; set; }
        /// <summary>
        /// List of cart items with detailed information including customizations.
        /// Each item includes BasePrice, CustomizationPrice, and FinalPrice breakdown.
        /// </summary>
        public List<CartItemDetailDto> Items { get; set; } = new();
        public int TotalItems { get; set; }
        public decimal SubTotal { get; set; }
        public decimal TaxAmount { get; set; }
        public decimal ShippingFee { get; set; }
        public decimal TotalAmount { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}