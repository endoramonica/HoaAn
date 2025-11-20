namespace VietCommerce.Core.DTOs.Payments;

public class CashPaymentRequest : PaymentRequest
    {
        // Cash specific fields if any (e.g. AmountReceived, ChangeReturned)
        public decimal AmountReceived { get; set; }
    }
