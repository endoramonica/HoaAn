namespace VietCommerce.Core.DTOs.Payments
{
    /// <summary>
    /// Result of processing a payment callback
    /// </summary>
    public class PaymentCallbackResult
    {
        public bool Success { get; set; }
        public string OrderId { get; set; } = string.Empty;
        public string? ResponseCode { get; set; }
        public string? TransactionId { get; set; }
        public string Message { get; set; } = string.Empty;
    }
}
