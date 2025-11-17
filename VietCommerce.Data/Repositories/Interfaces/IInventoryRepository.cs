using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VietCommerce.Core.Entities.Products;

namespace VietCommerce.Data.Repositories.Interfaces
{
    public interface IInventoryRepository : IGenericRepository<Inventory>
    {
        Task<Inventory?> GetByStoreAndProductAsync(Guid storeId, Guid productId);
        Task<List<Inventory>> GetLowStockByStoreAsync(Guid storeId);
        Task<int> GetAvailableQuantityAsync(Guid storeId, Guid productId);
        Task<List<Inventory>> GetByProductIdAsync(Guid productId);
        Task<List<Inventory>> GetByStoreAndProductsAsync(Guid storeId, List<Guid> productIds);
    }
    public interface IInventoryMovementRepository : IGenericRepository<InventoryMovement>
    {
        Task<List<InventoryMovement>> GetByInventoryIdAsync(Guid inventoryId);
        Task<List<InventoryMovement>> GetByOrderIdAsync(Guid orderId);
        Task<List<InventoryMovement>> GetByTransferIdAsync(Guid transferId);
        Task<List<InventoryMovement>> GetMovementsInDateRangeAsync(DateTime startDate, DateTime endDate);
    }
}
