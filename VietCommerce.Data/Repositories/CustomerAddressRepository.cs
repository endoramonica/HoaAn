using Microsoft.EntityFrameworkCore;
using VietCommerce.Core.Entities.Customers;
using VietCommerce.Core.Enums.Common;
using VietCommerce.Data.Context;
using VietCommerce.Data.Repositories.Interfaces;

namespace VietCommerce.Data.Repositories
{
    public class CustomerAddressRepository : GenericRepository<CustomerAddress>, ICustomerAddressRepository
    {
        public CustomerAddressRepository(AppDbContext context) : base(context)
        {
        }

        /// <summary>
        /// Lấy tất cả địa chỉ của customer (không bao gồm soft-deleted)
        /// </summary>
        public async Task<List<CustomerAddress>> GetByCustomerIdAsync(Guid customerId)
        {
            return await _dbSet
                .Where(a => a.CustomerId == customerId && !a.IsDeleted)
                .OrderByDescending(a => a.IsDefault)
                .ThenByDescending(a => a.IsPrimary)
                .ThenByDescending(a => a.CreatedAt)
                .ToListAsync();
        }

        /// <summary>
        /// Lấy địa chỉ mặc định của customer
        /// </summary>
        public async Task<CustomerAddress?> GetDefaultAddressAsync(Guid customerId)
        {
            return await _dbSet
                .Where(a => a.CustomerId == customerId && !a.IsDeleted && a.IsDefault)
                .FirstOrDefaultAsync();
        }

        /// <summary>
        /// Xóa flag IsDefault của tất cả địa chỉ của customer
        /// (dùng khi set địa chỉ mới làm default)
        /// </summary>
        public async Task ClearDefaultAsync(Guid customerId)
        {
            var addresses = await _dbSet
                .Where(a => a.CustomerId == customerId && !a.IsDeleted && a.IsDefault)
                .ToListAsync();

            foreach (var address in addresses)
            {
                address.IsDefault = false;
                address.UpdatedAt = DateTime.UtcNow;
            }

            // Context sẽ track changes, SaveChanges sẽ được gọi từ UnitOfWork
        }

        /// <summary>
        /// Lấy địa chỉ theo ID và kiểm tra ownership
        /// </summary>
        public async Task<CustomerAddress?> GetByIdAndCustomerIdAsync(Guid id, Guid customerId)
        {
            return await _dbSet
                .Where(a => a.Id == id && a.CustomerId == customerId && !a.IsDeleted)
                .FirstOrDefaultAsync();
        }

        /// <summary>
        /// Kiểm tra địa chỉ có thuộc customer không
        /// </summary>
        public async Task<bool> IsOwnerAsync(Guid addressId, Guid customerId)
        {
            return await _dbSet
                .AnyAsync(a => a.Id == addressId && a.CustomerId == customerId && !a.IsDeleted);
        }

        /// <summary>
        /// Lấy tất cả địa chỉ (Admin only) - có filter
        /// </summary>
        public async Task<(List<CustomerAddress> Items, int TotalCount)> GetAllPagedAsync(
            Guid? customerId,
            AddressType? addressType,
            bool? isActive,
            bool? isDefault,
            int pageNumber,
            int pageSize,
            string sortBy = "CreatedAt",
            bool ascending = false)
        {
            var query = _dbSet.Where(a => !a.IsDeleted);

            // Apply filters
            if (customerId.HasValue)
                query = query.Where(a => a.CustomerId == customerId.Value);

            if (addressType.HasValue)
                query = query.Where(a => a.AddressType == addressType.Value);

            if (isActive.HasValue)
                query = query.Where(a => a.IsActive == isActive.Value);

            if (isDefault.HasValue)
                query = query.Where(a => a.IsDefault == isDefault.Value);

            // Count total
            var totalCount = await query.CountAsync();

            // Apply sorting
            query = sortBy?.ToLower() switch
            {
                "city" => ascending ? query.OrderBy(a => a.City) : query.OrderByDescending(a => a.City),
                "addresstype" => ascending ? query.OrderBy(a => a.AddressType) : query.OrderByDescending(a => a.AddressType),
                "isdefault" => ascending ? query.OrderBy(a => a.IsDefault) : query.OrderByDescending(a => a.IsDefault),
                _ => ascending ? query.OrderBy(a => a.CreatedAt) : query.OrderByDescending(a => a.CreatedAt)
            };

            // Apply pagination
            var items = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (items, totalCount);
        }

    }
}