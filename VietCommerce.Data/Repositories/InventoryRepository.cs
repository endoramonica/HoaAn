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
    public class InventoryRepository : GenericRepository<Inventory>, IInventoryRepository
    {
        public InventoryRepository(AppDbContext context) : base(context) { }

        public async Task<Inventory?> GetByStoreAndProductAsync(Guid storeId, Guid productId)
        {
            return await _dbSet
                .FirstOrDefaultAsync(x => x.StoreId == storeId && x.ProductId == productId && !x.IsDeleted);
        }

        public async Task<List<Inventory>> GetLowStockByStoreAsync(Guid storeId)
        {
            return await _dbSet
                .Where(x => x.StoreId == storeId && !x.IsDeleted && x.IsActive)
                .Where(x => (x.QuantityAvailable - x.QuantityReserved) <= x.ReorderLevel)
                .ToListAsync();
        }

        public async Task<int> GetAvailableQuantityAsync(Guid storeId, Guid productId)
        {
            var inventory = await GetByStoreAndProductAsync(storeId, productId);
            if (inventory == null) return 0;
            return inventory.QuantityAvailable - inventory.QuantityReserved;
        }

        public async Task<List<Inventory>> GetByProductIdAsync(Guid productId)
        {
            return await _dbSet
                .Where(x => x.ProductId == productId && !x.IsDeleted)
                .ToListAsync();
        }
        public async Task<List<Inventory>> GetByStoreAndProductsAsync(Guid storeId, List<Guid> productIds)
        {
            return await _dbSet
                .Where(x => x.StoreId == storeId && productIds.Contains(x.ProductId) && !x.IsDeleted)
                .ToListAsync();
        }
    }
}
