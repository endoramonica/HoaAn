using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VietCommerce.Core.DTOs.Orders;
using VietCommerce.Core.DTOs.Payments;
using VietCommerce.Core.Models;

namespace VietCommerce.Application.Services.Payments
{
    public interface IPaymentService
    {
        // Processing
        Task<PaymentResponse> ProcessPaymentAsync(PaymentRequest request);
        Task<PaymentResponse> ProcessCashPaymentAsync(CashPaymentRequest request);
        Task<PaymentResponse> ProcessCardPaymentAsync(CardPaymentRequest request);
        Task<RefundResponse> ProcessRefundAsync(Guid paymentId, RefundRequest request);
        Task VoidPaymentAsync(Guid paymentId, string reason);

        // Calculation
        Task<PaymentCalculationDto> CalculateOrderTotalsAsync(CalculateOrderRequest request);

        // Query
        Task<PaginatedResult<PaymentDto>> GetPaymentHistoryAsync(PaginationParams pagination, PaymentFilters filters);

        // VNPay Callback Processing
        Task<PaymentCallbackResult> ProcessVNPayCallbackAsync(IDictionary<string, string> callbackData);
        Task<PaymentStatusDto> GetPaymentStatusAsync(string orderId);
        Task UpdateOrderPaymentStatusAsync(string orderId, string status);
    }
}
