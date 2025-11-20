using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using VietCommerce.Core.Entities.Logistics;

namespace VietCommerce.Data.Repositories.Interfaces
{
    public interface IStockTransferRepository : IGenericRepository<StockTransfer>
    {
        Task<StockTransfer?> GetByIdWithDetailsAsync(Guid id);
        Task<(IEnumerable<StockTransfer> Items, int TotalCount)> GetPagedWithDetailsAsync(
            int pageNumber,
            int pageSize,
            Expression<Func<StockTransfer, bool>>? predicate = null,
            Expression<Func<StockTransfer, object>>? orderBy = null,
            bool ascending = true);
        Task<IEnumerable<StockTransfer>> GetByWarehouseAsync(string warehouse);
        Task<IEnumerable<StockTransfer>> GetByStatusAsync(StockTransferStatus status);
        Task<IEnumerable<StockTransfer>> GetPendingTransfersAsync();
        Task<bool> HasActiveTransfersBetweenWarehousesAsync(string fromWarehouse, string toWarehouse);
    }
}
