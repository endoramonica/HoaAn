// VietCommerce.Core/DTOs/Cart/CartSummaryDto.cs
namespace VietCommerce.Core.DTOs.Cart
{
    public class CartSummaryDto
    {
        public int ItemCount { get; set; }
        public decimal SubTotal { get; set; }
        public decimal DiscountAmount { get; set; }
        public decimal TotalAmount { get; set; }
        public string Status { get; set; } = "ACTIVE";
        public string? AppliedVoucherCode { get; set; }
    }
}

