using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VietCommerce.Core.Entities.Products;
using VietCommerce.Data.Context;
using VietCommerce.Data.Repositories.Interfaces;

namespace VietCommerce.Data.Repositories
{
    public class InventoryMovementRepository : GenericRepository<InventoryMovement>, IInventoryMovementRepository
    {
        public InventoryMovementRepository(AppDbContext context) : base(context) { }

        public async Task<List<InventoryMovement>> GetByInventoryIdAsync(Guid inventoryId)
        {
            return await _dbSet
                .Where(x => x.InventoryId == inventoryId)
                .Include(x => x.PerformedBy)
                .OrderByDescending(x => x.CreatedAt)
                .ToListAsync();
        }

        public async Task<List<InventoryMovement>> GetByOrderIdAsync(Guid orderId)
        {
            return await _dbSet
                .Where(x => x.OrderId == orderId)
                .Include(x => x.PerformedBy)
                .OrderByDescending(x => x.CreatedAt)
                .ToListAsync();
        }

        public async Task<List<InventoryMovement>> GetByTransferIdAsync(Guid transferId)
        {
            return await _dbSet
                .Where(x => x.TransferId == transferId)
                .Include(x => x.PerformedBy)
                .OrderByDescending(x => x.CreatedAt)
                .ToListAsync();
        }

        public async Task<List<InventoryMovement>> GetMovementsInDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            return await _dbSet
                .Where(x => x.CreatedAt >= startDate && x.CreatedAt <= endDate)
                .Include(x => x.PerformedBy)
                .OrderByDescending(x => x.CreatedAt)
                .ToListAsync();
        }
    }
}
