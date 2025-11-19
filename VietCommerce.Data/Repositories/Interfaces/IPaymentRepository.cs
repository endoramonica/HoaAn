// File: VietCommerce.Data/Repositories/Interfaces/IPaymentRepository.cs
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using VietCommerce.Core.Entities.Orders;
using VietCommerce.Core.Entities.Payments;
using VietCommerce.Core.Enums.Payments;

namespace VietCommerce.Data.Repositories.Interfaces
{
    public interface IPaymentRepository : IGenericRepository<Payment>
    {
        /// <summary>
        /// Lấy Payment theo OrderId với đầy đủ thông tin
        /// </summary>
        Task<Payment?> GetByOrderIdAsync(Guid orderId);

        /// <summary>
        /// Lấy danh sách Payments theo OrderId
        /// </summary>
        Task<IEnumerable<Payment>> GetPaymentsByOrderIdAsync(Guid orderId);

        /// <summary>
        /// Lấy Payment theo TransactionId (tìm qua PaymentTransactions)
        /// </summary>
        Task<Payment?> GetByTransactionIdAsync(string transactionId);

        /// <summary>
        /// Cập nhật trạng thái Payment và tạo PaymentTransaction mới
        /// </summary>
        Task<bool> UpdatePaymentStatusAsync(Guid paymentId, PaymentMethodType status, string? transactionId = null, string? gatewayResponse = null);

        /// <summary>
        /// Lấy danh sách Payments theo Status
        /// </summary>
        Task<IEnumerable<Payment>> GetPaymentsByStatusAsync(PaymentMethodType status);

        /// <summary>
        /// Lấy Payment với thông tin đầy đủ (Order, PaymentMethod, Transactions)
        /// </summary>
        Task<Payment?> GetPaymentWithDetailsAsync(Guid paymentId);

        /// <summary>
        /// Kiểm tra xem Order đã có Payment thành công chưa
        /// </summary>
        Task<bool> HasSuccessfulPaymentAsync(Guid orderId);

        /// <summary>
        /// Lấy Payment transactions history của một Order
        /// </summary>
        Task<IEnumerable<PaymentTransaction>> GetPaymentTransactionsByOrderIdAsync(Guid orderId);
    }
}
