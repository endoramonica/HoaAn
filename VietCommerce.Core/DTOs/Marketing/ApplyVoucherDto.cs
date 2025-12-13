namespace VietCommerce.Core.DTOs.Marketing
{
    /// <summary>
    /// DTO for applying a voucher code to cart
    /// Requirements: 3.2, 3.3
    /// </summary>
    public class ApplyVoucherDto
    {
        /// <summary>
        /// Voucher code to apply
        /// </summary>
        public string VoucherCode { get; set; } = string.Empty;
    }
}
