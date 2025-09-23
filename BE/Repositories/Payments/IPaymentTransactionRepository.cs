using VietCommerce.Core.Entities.Payments;
using VietCommerce.Data.Repositories.Generic;

namespace VietCommerce.Data.Repositories.Payments;

public interface IPaymentTransactionRepository : IGenericRepository<PaymentTransaction>
{
}