using VietCommerce.Core.Entities.Customers;
using VietCommerce.Data;
using VietCommerce.Data.Context;
using VietCommerce.Data.Repositories.Customers;
using VietCommerce.Data.Repositories.Generic;

namespace VietCommerce.Data.Repositories.Customers;

public class CustomerRepository : GenericRepository<Customer>, ICustomerRepository
{
    public CustomerRepository(AppDbContext context) : base(context) { }
}