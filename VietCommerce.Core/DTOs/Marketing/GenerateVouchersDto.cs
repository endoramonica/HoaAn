namespace VietCommerce.Core.DTOs.Marketing
{
    /// <summary>
    /// DTO for generating vouchers
    /// Requirements: 3.1
    /// </summary>
    public class GenerateVouchersDto
    {
        /// <summary>
        /// Number of vouchers to generate
        /// </summary>
        public int Quantity { get; set; }

        /// <summary>
        /// Prefix for voucher codes (e.g., "PROMO")
        /// </summary>
        public string? Prefix { get; set; }

        /// <summary>
        /// Expiry date for generated vouchers
        /// </summary>
        public DateTime ExpiryDate { get; set; }
    }
}
