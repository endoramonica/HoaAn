using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using VietCommerce.Core.Entities.Logistics;
using VietCommerce.Data.Context;
using VietCommerce.Data.Repositories.Interfaces;

namespace VietCommerce.Data.Repositories
{
    public class StockTransferRepository : GenericRepository<StockTransfer>, IStockTransferRepository
    {
        public StockTransferRepository(AppDbContext context) : base(context)
        {
        }

        public async Task<StockTransfer?> GetByIdWithDetailsAsync(Guid id)
        {
            return await _dbSet
                .Include(st => st.RequestedByUser)
                .Include(st => st.ApprovedByUser)
                .Include(st => st.Supplier)
                .Include(st => st.TransferItems)
                    .ThenInclude(ti => ti.Product)
                .FirstOrDefaultAsync(st => st.Id == id);
        }

        public async Task<(IEnumerable<StockTransfer> Items, int TotalCount)> GetPagedWithDetailsAsync(
            int pageNumber,
            int pageSize,
            Expression<Func<StockTransfer, bool>>? predicate = null,
            Expression<Func<StockTransfer, object>>? orderBy = null,
            bool ascending = true)
        {
            IQueryable<StockTransfer> query = _dbSet
                .Include(st => st.RequestedByUser)
                .Include(st => st.ApprovedByUser)
                .Include(st => st.Supplier)
                .Include(st => st.TransferItems)
                    .ThenInclude(ti => ti.Product);

            if (predicate != null)
            {
                query = query.Where(predicate);
            }

            var totalCount = await query.CountAsync();

            if (orderBy != null)
            {
                query = ascending
                    ? query.OrderBy(orderBy)
                    : query.OrderByDescending(orderBy);
            }
            else
            {
                query = query.OrderByDescending(st => st.CreatedAt);
            }

            var items = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (items, totalCount);
        }

        public async Task<IEnumerable<StockTransfer>> GetByWarehouseAsync(string warehouse)
        {
            return await _dbSet
                .Include(st => st.TransferItems)
                .Where(st => st.FromWarehouse == warehouse || st.ToWarehouse == warehouse)
                .OrderByDescending(st => st.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<StockTransfer>> GetByStatusAsync(StockTransferStatus status)
        {
            return await _dbSet
                .Include(st => st.TransferItems)
                .Where(st => st.Status == status)
                .OrderByDescending(st => st.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<StockTransfer>> GetPendingTransfersAsync()
        {
            return await _dbSet
                .Include(st => st.TransferItems)
                    .ThenInclude(ti => ti.Product)
                .Include(st => st.RequestedByUser)
                .Where(st => st.Status == StockTransferStatus.Pending)
                .OrderBy(st => st.CreatedAt)
                .ToListAsync();
        }

        public async Task<bool> HasActiveTransfersBetweenWarehousesAsync(string fromWarehouse, string toWarehouse)
        {
            return await _dbSet.AnyAsync(st =>
                st.FromWarehouse == fromWarehouse &&
                st.ToWarehouse == toWarehouse &&
                (st.Status == StockTransferStatus.Pending || st.Status == StockTransferStatus.InTransit));
        }
    }
}