// File: VietCommerce.Data/Repositories/OrderItemRepository.cs
using Microsoft.EntityFrameworkCore;
using VietCommerce.Core.Entities.Orders;
using VietCommerce.Data.Context;
using VietCommerce.Data.Repositories.Interfaces;
using VietCommerce.Core.Enums.Orders;

namespace VietCommerce.Data.Repositories
{
    public class OrderItemRepository : GenericRepository<OrderItem>, IOrderItemRepository
    {
        private readonly AppDbContext _context;

        public OrderItemRepository(AppDbContext context) : base(context)
        {
            _context = context;
        }

        /// <summary>
        /// Get all items for a specific order with product details
        /// </summary>
        public async Task<IEnumerable<OrderItem>> GetByOrderIdAsync(Guid orderId)
        {
            return await _dbSet
                .AsNoTracking()
                .Where(oi => oi.OrderId == orderId)
                .Include(oi => oi.Product)
                    .ThenInclude(p => p.Images)
                .OrderBy(oi => oi.CreatedAt)
                .ToListAsync();
        }

        /// <summary>
        /// Get all order items for a specific product
        /// </summary>
        public async Task<IEnumerable<OrderItem>> GetByProductIdAsync(Guid productId)
        {
            return await _dbSet
                .AsNoTracking()
                .Where(oi => oi.ProductId == productId)
                .Include(oi => oi.Order)
                .OrderByDescending(oi => oi.CreatedAt)
                .ToListAsync();
        }

        /// <summary>
        /// Get total quantity sold for a product (optionally filtered by date range)
        /// </summary>
        public async Task<int> GetTotalQuantitySoldAsync(Guid productId, DateTime? fromDate = null, DateTime? toDate = null)
        {
            var query = _dbSet
                .AsNoTracking()
                .Where(oi => oi.ProductId == productId)
                .Include(oi => oi.Order)
                .Where(oi => oi.Order != null && !oi.Order.IsDeleted &&
                            (oi.Order.Status == OrderStatus.Completed ||
                             oi.Order.Status == OrderStatus.Shipped));

            if (fromDate.HasValue)
            {
                query = query.Where(oi => oi.CreatedAt >= fromDate.Value);
            }

            if (toDate.HasValue)
            {
                var adjustedToDate = toDate.Value.AddDays(1);
                query = query.Where(oi => oi.CreatedAt < adjustedToDate);
            }

            return await query.SumAsync(oi => oi.Quantity);
        }

        /// <summary>
        /// Get top selling products by quantity sold
        /// </summary>
        public async Task<Dictionary<Guid, int>> GetTopSellingProductsAsync(
            int top = 10,
            Guid? storeId = null,
            DateTime? fromDate = null)
        {
            var query = _dbSet
                .AsNoTracking()
                .Include(oi => oi.Order)
                .Where(oi => oi.Order != null && !oi.Order.IsDeleted &&
                            (oi.Order.Status == OrderStatus.Completed ||
                             oi.Order.Status == OrderStatus.Shipped));

            if (storeId.HasValue)
            {
                query = query.Where(oi => oi.Order.StoreId == storeId.Value);
            }

            if (fromDate.HasValue)
            {
                query = query.Where(oi => oi.CreatedAt >= fromDate.Value);
            }

            var result = await query
                .GroupBy(oi => oi.ProductId)
                .Select(g => new
                {
                    ProductId = g.Key,
                    TotalQuantity = g.Sum(oi => oi.Quantity)
                })
                .OrderByDescending(x => x.TotalQuantity)
                .Take(top)
                .ToListAsync();

            return result.ToDictionary(x => x.ProductId, x => x.TotalQuantity);
        }
    }
}