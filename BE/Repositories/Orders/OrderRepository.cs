using VietCommerce.Core.Entities.Orders;
using VietCommerce.Data;
using VietCommerce.Data.Context;
using VietCommerce.Data.Repositories.Generic;
using VietCommerce.Data.Repositories.Orders;

namespace VietCommerce.Data.Repositories.Orders;

public class OrderRepository : GenericRepository<Order>, IOrderRepository
{
    public OrderRepository(AppDbContext context) : base(context) { }
}