using VietCommerce.Core.DTOs.Payments;

namespace VietCommerce.Application.Services.Services.Interfaces
{
    /// <summary>
    /// Payment service interface for handling payment operations
    /// </summary>
    public interface IPaymentService
    {
        /// <summary>
        /// /// Process VNPay callback from payment gateway
        /// </summary>
        /// <param name="callbackData">Callback data from VNPay</param>
        /// <returns>Callback processing result</returns>
        Task<PaymentCallbackResult> ProcessVNPayCallbackAsync(IDictionary<string, string> callbackData);

        /// <summary>
        /// Get payment status for an order
        /// </summary>
        /// <param name="orderId">Order ID</param>
        /// <returns>Payment status information</returns>
        Task<PaymentStatusDto> GetPaymentStatusAsync(string orderId);

        /// <summary>
        /// Update order payment status
        /// </summary>
        /// <param name="orderId">Order ID</param>
        /// <param name="status">New payment status</param>
        /// <returns>Success indicator</returns>
        Task<bool> UpdateOrderPaymentStatusAsync(string orderId, string status);
    }
}
