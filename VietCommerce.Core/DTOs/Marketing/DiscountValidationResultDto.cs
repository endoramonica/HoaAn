namespace VietCommerce.Core.DTOs.Marketing
{
    /// <summary>
    /// DTO for discount validation result
    /// Contains validation status and error details
    /// </summary>
    public class DiscountValidationResultDto
    {
        /// <summary>
        /// Whether the promotion can be applied
        /// </summary>
        public bool IsValid { get; set; }

        /// <summary>
        /// Error message if validation failed
        /// </summary>
        public string? ErrorMessage { get; set; }

        /// <summary>
        /// Error code for programmatic handling
        /// </summary>
        public string? ErrorCode { get; set; }

        /// <summary>
        /// Reason for validation failure
        /// </summary>
        public string? Reason { get; set; }
    }
}
