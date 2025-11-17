using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VietCommerce.Core.Entities.Customers;

namespace VietCommerce.Data.Repositories.Interfaces
{
    // ICustomerAddressRepository.cs
    public interface ICustomerAddressRepository : IGenericRepository<CustomerAddress>
    {
        Task<List<CustomerAddress>> GetByCustomerIdAsync(Guid customerId);
        Task<CustomerAddress?> GetDefaultAddressAsync(Guid customerId);
        Task ClearDefaultAsync(Guid customerId);
        Task<CustomerAddress?> GetByIdAndCustomerIdAsync(Guid addressId, Guid customerId);

    }
}
