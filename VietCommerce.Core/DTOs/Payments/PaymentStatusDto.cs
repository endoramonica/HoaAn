namespace VietCommerce.Core.DTOs.Payments
{
    /// <summary>
    /// Payment status DTO for retrieving payment information
    /// /// </summary>
    public class PaymentStatusDto
    {
        public string OrderId { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty; // Pending, Paid, Failed, Cancelled
        public string? TransactionId { get; set; }
        public string? ResponseCode { get; set; }
        public string? ErrorMessage { get; set; }
        public DateTime? PaidAt { get; set; }
        public decimal Amount { get; set; }
    }
}
