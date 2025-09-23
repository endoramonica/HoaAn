using VietCommerce.Core.Entities.Payments;
using VietCommerce.Data;
using VietCommerce.Data.Context;
using VietCommerce.Data.Repositories.Generic;
using VietCommerce.Data.Repositories.Payments;

namespace VietCommerce.Data.Repositories.Payments;

public class PaymentTransactionRepository : GenericRepository<PaymentTransaction>, IPaymentTransactionRepository
{
    public PaymentTransactionRepository(AppDbContext context) : base(context) { }
}