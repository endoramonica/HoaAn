// File: VietCommerce.Data/Repositories/PaymentMethodRepository.cs
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using VietCommerce.Core.Entities.Payments;
using VietCommerce.Data.Context;
using VietCommerce.Data.Repositories.Interfaces;

namespace VietCommerce.Data.Repositories
{
    public class PaymentMethodRepository : GenericRepository<PaymentMethod>, IPaymentMethodRepository
    {
        public PaymentMethodRepository(AppDbContext context) : base(context)
        {
        }

        public async Task<PaymentMethod?> GetByCodeAsync(string code)
        {
            if (string.IsNullOrWhiteSpace(code))
                return null;

            return await _dbSet
                .FirstOrDefaultAsync(pm => pm.Code == code.ToUpper() && !pm.IsDeleted);
        }

        public async Task<IEnumerable<PaymentMethod>> GetActiveMethodsSortedByNameAsync()
        {
            return await _dbSet
                .Where(pm => pm.IsActive && !pm.IsDeleted)
                .OrderBy(pm => pm.Name)
                .ToListAsync();
        }

        public async Task<IEnumerable<PaymentMethod>> GetActiveMethodsSortedByCodeAsync()
        {
            return await _dbSet
                .Where(pm => pm.IsActive && !pm.IsDeleted)
                .OrderBy(pm => pm.Code)
                .ToListAsync();
        }

        public async Task<IEnumerable<PaymentMethod>> GetActiveMethodsSortedByCreatedAtAsync()
        {
            return await _dbSet
                .Where(pm => pm.IsActive && !pm.IsDeleted)
                .OrderByDescending(pm => pm.CreatedAt)
                .ToListAsync();
        }

        public async Task<bool> IsMethodActiveAsync(string code)
        {
            if (string.IsNullOrWhiteSpace(code))
                return false;

            return await _dbSet
                .AnyAsync(pm => pm.Code == code.ToUpper() && pm.IsActive && !pm.IsDeleted);
        }
    }
}