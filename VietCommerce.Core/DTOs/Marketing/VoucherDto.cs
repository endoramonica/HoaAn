namespace VietCommerce.Core.DTOs.Marketing
{
    /// <summary>
    /// DTO for Voucher entity
    /// Requirements: 3.1
    /// </summary>
    public class VoucherDto
    {
        public Guid Id { get; set; }
        public string Code { get; set; } = string.Empty;
        public Guid PromotionId { get; set; }
        public DateTime ExpiryDate { get; set; }
        public int UsageCount { get; set; }
        public DateTime? LastUsedAt { get; set; }
        public Guid? LastUsedBy { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
    }
}
