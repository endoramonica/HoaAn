namespace VietCommerce.Core.DTOs.Marketing
{
    /// <summary>
    /// DTO for voucher generation result
    /// Requirements: 3.1
    /// </summary>
    public class GenerateVouchersResultDto
    {
        /// <summary>
        /// Number of vouchers generated
        /// </summary>
        public int GeneratedCount { get; set; }

        /// <summary>
        /// List of generated voucher codes
        /// </summary>
        public List<string> VoucherCodes { get; set; } = new();

        /// <summary>
        /// Promotion ID the vouchers are linked to
        /// </summary>
        public Guid PromotionId { get; set; }
    }
}
