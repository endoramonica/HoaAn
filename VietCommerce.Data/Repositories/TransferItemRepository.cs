using Microsoft.EntityFrameworkCore;
using VietCommerce.Core.Entities.Logistics;
using VietCommerce.Data.Context;
using VietCommerce.Data.Repositories.Interfaces;

namespace VietCommerce.Data.Repositories
{
    public class TransferItemRepository : GenericRepository<TransferItem>, ITransferItemRepository
    {
        public TransferItemRepository(AppDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<TransferItem>> GetByStockTransferIdAsync(Guid stockTransferId)
        {
            return await _dbSet
                .Include(ti => ti.Product)
                .Where(ti => ti.StockTransferId == stockTransferId)
                .ToListAsync();
        }

        public async Task<IEnumerable<TransferItem>> GetByProductIdAsync(Guid productId)
        {
            return await _dbSet
                .Include(ti => ti.StockTransfer)
                .Where(ti => ti.ProductId == productId)
                .OrderByDescending(ti => ti.CreatedAt)
                .ToListAsync();
        }

        public async Task<TransferItem?> GetByIdWithDetailsAsync(Guid id)
        {
            return await _dbSet
                .Include(ti => ti.StockTransfer)
                .Include(ti => ti.Product)
                .FirstOrDefaultAsync(ti => ti.Id == id);
        }
    }
}