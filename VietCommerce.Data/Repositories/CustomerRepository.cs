using Microsoft.EntityFrameworkCore;
using VietCommerce.Core.Entities.Customers;
using VietCommerce.Data.Context;
using VietCommerce.Data.Repositories.Interfaces;

namespace VietCommerce.Data.Repositories
{
    public class CustomerRepository : GenericRepository<Customer>, ICustomerRepository
    {
        public CustomerRepository(AppDbContext context) : base(context){}

        // 🔹 Lấy Customer theo UserId (toàn hệ thống)
        public async Task<Customer?> GetByUserIdAsync(Guid userId)
        {
            return await _context.Customers
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.UserId == userId && !c.IsDeleted);
        }

        // 🔹 Lấy Customer theo UserId + StoreId (quan trọng khi đa cửa hàng)
        public async Task<Customer?> GetByUserAndStoreAsync(Guid userId, Guid storeId)
        {
            return await _context.Customers
                .FirstOrDefaultAsync(c => c.UserId == userId && c.StoreId == storeId && !c.IsDeleted);
        }

        // 🔹 Kiểm tra trùng email
        public async Task<bool> EmailExistsAsync(string email)
        {
            return await _context.Customers
                .AnyAsync(c => c.Email == email && !c.IsDeleted);
        }

        // 🔹 Đảm bảo Customer tồn tại (nếu chưa thì tạo mới)
        public async Task<Customer> EnsureCustomerExistsAsync(Guid userId, Guid storeId)
        {
            var existing = await _context.Customers
                .FirstOrDefaultAsync(c => c.UserId == userId && c.StoreId == storeId && !c.IsDeleted);

            if (existing != null)
                return existing;

            // Tạo mới nếu chưa có
            var newCustomer = new Customer
            {
                Id = Guid.NewGuid(),
                StoreId = storeId,
                UserId = userId,
                Name = null, // Có thể cập nhật sau
                Email = null,
                Phone = null,
                LoyaltyPoints = 0,
                Tier = null,
                TenantId = Guid.Parse("F40EC7E0-FC21-4E67-831C-07D14D0B304A"),
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            await _context.Customers.AddAsync(newCustomer);
            await _context.SaveChangesAsync();

            return newCustomer;
        }

    }
}
