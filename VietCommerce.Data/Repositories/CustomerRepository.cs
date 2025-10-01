using Microsoft.EntityFrameworkCore;
using VietCommerce.Core.DTOs.Customers;
using VietCommerce.Core.Entities.Customers;
using VietCommerce.Data.Context;
using VietCommerce.Data.Repositories.Interfaces;

namespace VietCommerce.Data.Repositories;

public class CustomerRepository : GenericRepository<Customer> , ICustomerRepository
{
    public CustomerRepository(AppDbContext context) : base(context)
    {
    }

    public async Task<Customer?> GetCustomerWithDetailsAsync(Guid id)
    {
        return await _dbSet
            .Include(c => c.User)
            .Include(c => c.Store)
            .Include(c => c.Orders)
            .FirstOrDefaultAsync(c => c.Id == id && !c.IsDeleted);
    }

    public async Task<Customer?> GetCustomerByUserIdAsync(Guid userId)
    {
        return await _dbSet
            .Include(c => c.User)
            .Include(c => c.Store)
            .FirstOrDefaultAsync(c => c.UserId == userId && !c.IsDeleted);
    }

    public async Task<bool> EmailExistsAsync(string email)
    {
        return await _dbSet.AnyAsync(c => c.Email == email.ToLower() && !c.IsDeleted);
    }

    public async Task<(IEnumerable<CustomerListDTO> customers, int totalCount)> GetCustomersPagedAsync(
        int pageNumber, int pageSize, string? searchTerm = null)
    {
        var query = _dbSet
            .Where(c => !c.IsDeleted)
            .Include(c => c.User)
            .AsQueryable();

        if (!string.IsNullOrEmpty(searchTerm))
        {
            var lowerSearchTerm = searchTerm.ToLower();
            query = query.Where(c =>
                (c.Name != null && c.Name.ToLower().Contains(lowerSearchTerm)) ||
                (c.Email != null && c.Email.ToLower().Contains(lowerSearchTerm)) ||
                (c.Phone != null && c.Phone.Contains(searchTerm)));
        }

        var totalCount = await query.CountAsync();

        var customers = await query
            .OrderBy(c => c.CreatedAt)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .Select(c => new CustomerListDTO
            {
                Id = c.Id,
                FullName = c.Name ?? "",
                Email = c.Email,
                PhoneNumber = c.Phone,
                UserId = c.UserId,
                CreatedDate = c.CreatedAt
            })
            .ToListAsync();

        return (customers, totalCount);
    }
}
