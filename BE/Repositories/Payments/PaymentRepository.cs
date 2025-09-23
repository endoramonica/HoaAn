using VietCommerce.Core.Entities.Payments;
using VietCommerce.Data;
using VietCommerce.Data.Context;
using VietCommerce.Data.Repositories.Generic;
using VietCommerce.Data.Repositories.Payments;

namespace VietCommerce.Data.Repositories.Payments;

public class PaymentRepository : GenericRepository<Payment>, IPaymentRepository
{
    public PaymentRepository(AppDbContext context) : base(context) { }
}