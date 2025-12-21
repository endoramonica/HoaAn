namespace VietCommerce.Core.Enums.Payments
{
    /// <summary>
    /// Payment status enum for tracking payment lifecycle
    /// </summary>
    public enum PaymentStatus
    {
        Pending = 0,      // Payment initiated, awaiting gateway response
        Paid = 1,         // Payment successful
        Failed = 2,       // Payment failed
        Cancelled = 3     // Payment cancelled by user
    }
}
