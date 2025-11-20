namespace VietCommerce.Core.DTOs.Payments;

public class RefundResponse
    {
        public bool IsSuccess { get; set; }
        public string RefundTransactionId { get; set; } = string.Empty;
        public decimal RefundedAmount { get; set; }
    }
