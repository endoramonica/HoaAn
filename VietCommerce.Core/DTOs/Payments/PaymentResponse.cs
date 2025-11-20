namespace VietCommerce.Core.DTOs.Payments;

public class PaymentResponse
    {
        public Guid PaymentId { get; set; }
        public bool IsSuccess { get; set; }
        public string Message { get; set; } = string.Empty;
        public string TransactionId { get; set; } = string.Empty;
    }
