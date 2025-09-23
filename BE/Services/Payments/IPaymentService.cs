using VietCommerce.Core.DTOs.Payments;
using VietCommerce.Core.Enums;
using VietCommerce.Core.Enums.Payments;
using VietCommerce.Core.Models;

namespace VietCommerce.Core.Services.Payments;

public interface IPaymentService
{
    Task<PaginatedResult<PaymentListDTO>> GetPaginatedAsync(int pageNumber = 1, int pageSize = 10);
    Task<PaymentListDTO> GetByIdAsync(Guid id);
    Task<Guid> CreateAsync(PaymentCreateDTO dto);
    Task UpdateAsync(Guid id, PaymentUpdateDTO dto);
    Task DeleteAsync(Guid id);
    Task ProcessPaymentAsync(Guid paymentId, PaymentMethodType status, string transactionId);
}