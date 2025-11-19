// File: VietCommerce.Data/Repositories/Interfaces/IPaymentMethodRepository.cs
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using VietCommerce.Core.Entities.Orders;
using VietCommerce.Core.Entities.Payments;

namespace VietCommerce.Data.Repositories.Interfaces
{
    public interface IPaymentMethodRepository : IGenericRepository<PaymentMethod>
    {
        /// <summary>
        /// Lấy PaymentMethod theo Code (COD, VNPAY, MOMO, etc.)
        /// </summary>
        Task<PaymentMethod?> GetByCodeAsync(string code);

        /// Lấy tất cả PaymentMethods đang active - Sort by Name (A-Z)
        /// </summary>
        Task<IEnumerable<PaymentMethod>> GetActiveMethodsSortedByNameAsync();

        /// <summary>
        /// Lấy tất cả PaymentMethods đang active - Sort by Code (A-Z)
        /// </summary>
        Task<IEnumerable<PaymentMethod>> GetActiveMethodsSortedByCodeAsync();

        /// <summary>
        /// Lấy tất cả PaymentMethods đang active - Sort by CreatedAt (Newest first)
        /// </summary>
        Task<IEnumerable<PaymentMethod>> GetActiveMethodsSortedByCreatedAtAsync();

        /// <summary>
        /// Kiểm tra PaymentMethod có tồn tại và active không
        /// </summary>
        Task<bool> IsMethodActiveAsync(string code);

        
    }
}