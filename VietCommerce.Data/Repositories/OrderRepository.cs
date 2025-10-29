// File: VietCommerce.Data/Repositories/OrderRepository.cs
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using VietCommerce.Core.DTOs.Orders;
using VietCommerce.Core.Entities.Orders;
using VietCommerce.Core.Enums.Orders;
using VietCommerce.Core.Models;
using VietCommerce.Data.Context;
using VietCommerce.Data.Repositories.Interfaces;

namespace VietCommerce.Data.Repositories
{
    public class OrderRepository : GenericRepository<Order>, IOrderRepository
    {
        private readonly AppDbContext _context;
        private readonly ILogger<OrderRepository> _logger;

        public OrderRepository(AppDbContext context, ILogger<OrderRepository> logger) : base(context)
        {
            _context = context;
            _logger = logger;
        }

        

        // ========================================
        // LOOKUP BY BUSINESS KEYS
        // ========================================

        /// <summary>
        /// Get order by order number with all related entities loaded
        /// </summary>
        public async Task<Order?> GetByOrderNumberAsync(string orderNumber)
        {
            return await _dbSet
                .AsNoTracking()
                .Include(o => o.Customer)
                .Include(o => o.Store)
                .Include(o => o.CreatedByUser)
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Product)
                    .ThenInclude(p => p.Images)
                .Include(o => o.OrderShipping)
                .Include(o => o.OrderStatusHistories.OrderByDescending(h => h.CreatedAt))
                .Include(o => o.Payments)
                    .ThenInclude(p => p.PaymentMethod)
                .FirstOrDefaultAsync(o => o.OrderNumber == orderNumber && !o.IsDeleted);
        }

        /// <summary>
        /// Get order by ID with all details (no tracking for read-only)
        /// </summary>
        public async Task<Order?> GetByIdWithDetailsAsync(Guid id)
        {
            return await _dbSet
                .AsNoTracking()
                .Include(o => o.Customer)
                .Include(o => o.Store)
                .Include(o => o.CreatedByUser)
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Product)
                    .ThenInclude(p => p.Images)
                .Include(o => o.OrderShipping)
                .Include(o => o.OrderStatusHistories.OrderByDescending(h => h.CreatedAt))
                .Include(o => o.Payments)
                    .ThenInclude(p => p.PaymentMethod)
                .FirstOrDefaultAsync(o => o.Id == id && !o.IsDeleted);
        }

        // ========================================
        // QUERIES BY RELATIONSHIPS
        // ========================================

        /// <summary>
        /// Get all orders for a specific customer
        /// </summary>
        public async Task<IEnumerable<Order>> GetByCustomerIdAsync(Guid customerId)
        {
            return await _dbSet
                .AsNoTracking()
                .Where(o => o.CustomerId == customerId && !o.IsDeleted)
                .Include(o => o.OrderItems)
                .Include(o => o.Store)
                .OrderByDescending(o => o.CreatedAt)
                .ToListAsync();
        }

        /// <summary>
        /// Get all orders for a specific store
        /// </summary>
        public async Task<IEnumerable<Order>> GetByStoreIdAsync(Guid storeId)
        {
            return await _dbSet
                .AsNoTracking()
                .Where(o => o.StoreId == storeId && !o.IsDeleted)
                .Include(o => o.Customer)
                .Include(o => o.OrderItems)
                .OrderByDescending(o => o.CreatedAt)
                .ToListAsync();
        }

        /// <summary>
        /// Get all orders with a specific status
        /// </summary>
        public async Task<IEnumerable<Order>> GetByStatusAsync(OrderStatus status)
        {
            return await _dbSet
                .AsNoTracking()
                .Where(o => o.Status == status && !o.IsDeleted)
                .Include(o => o.Customer)
                .Include(o => o.Store)
                .OrderByDescending(o => o.CreatedAt)
                .ToListAsync();
        }

        // ========================================
        // PAGINATION WITH FILTERS
        // ========================================

        /// <summary>
        /// Get paginated list of orders with filtering and sorting
        /// </summary>
        /// <param name="filter">Filter criteria (status, date range, search keyword)</param>
        /// <param name="restrictToCustomerId">If provided, only return orders for this customer (for customer view)</param>
        public async Task<PaginatedResult<Order>> GetPaginatedAsync(
            OrderFilterDTO filter,
            Guid? restrictToCustomerId = null)
        {
            // Base query with includes
            var query = _dbSet
                .AsNoTracking()
                .Where(o => !o.IsDeleted)
                .Include(o => o.Customer)
                .Include(o => o.Store)
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Product)
                    .ThenInclude(p => p.Images)
                .Include(o => o.OrderShipping)
                .AsQueryable();

            // Restrict to customer (Security)
            if (restrictToCustomerId.HasValue)
            {
                query = query.Where(o => o.CustomerId == restrictToCustomerId.Value);
            }

            // Apply filters
            if (!string.IsNullOrWhiteSpace(filter.Keyword))
            {
                var keyword = filter.Keyword.ToLower().Trim();
                query = query.Where(o =>
                    o.OrderNumber.ToLower().Contains(keyword) ||
                    (o.Customer != null && o.Customer.Name != null && o.Customer.Name.ToLower().Contains(keyword)));
            }

            if (filter.Status.HasValue)
            {
                query = query.Where(o => o.Status == filter.Status.Value);
            }

            if (filter.CustomerId.HasValue && !restrictToCustomerId.HasValue)
            {
                query = query.Where(o => o.CustomerId == filter.CustomerId.Value);
            }

            if (filter.StoreId.HasValue)
            {
                query = query.Where(o => o.StoreId == filter.StoreId.Value);
            }

            if (filter.FromDate.HasValue)
            {
                var fromDate = filter.FromDate.Value.Date;
                query = query.Where(o => o.CreatedAt >= fromDate);
            }

            if (filter.ToDate.HasValue)
            {
                var toDate = filter.ToDate.Value.Date.AddDays(1);
                query = query.Where(o => o.CreatedAt < toDate);
            }

            if (filter.MinAmount.HasValue)
            {
                query = query.Where(o => o.TotalAmount >= filter.MinAmount.Value);
            }

            if (filter.MaxAmount.HasValue)
            {
                query = query.Where(o => o.TotalAmount <= filter.MaxAmount.Value);
            }

            // Get total count before pagination
            var totalCount = await query.CountAsync();

            // Apply sorting
            query = ApplySorting(query, filter.SortBy ?? "CreatedAt", filter.SortDescending);

            // Apply pagination
            var orders = await query
                .Skip((filter.Page - 1) * filter.PageSize)
                .Take(filter.PageSize)
                .ToListAsync();

            return new PaginatedResult<Order>
            {
                Items = orders,
                PageNumber = filter.Page,
                PageSize = filter.PageSize,
                TotalItems = totalCount,
                TotalPages = (int)Math.Ceiling((double)totalCount / filter.PageSize)
            };
        }

        // ========================================
        // BUSINESS QUERIES
        // ========================================

        /// <summary>
        /// Get recent orders (last N orders, optionally filtered by store)
        /// </summary>
        public async Task<IEnumerable<Order>> GetRecentOrdersAsync(int count = 10, Guid? storeId = null)
        {
            var query = _dbSet
                .AsNoTracking()
                .Where(o => !o.IsDeleted);

            if (storeId.HasValue)
            {
                query = query.Where(o => o.StoreId == storeId.Value);
            }

            return await query
                .Include(o => o.Customer)
                .Include(o => o.OrderItems)
                .OrderByDescending(o => o.CreatedAt)
                .Take(count)
                .ToListAsync();
        }

        /// <summary>
        /// Get total count of orders (optionally filtered by store and date range)
        /// </summary>
        public async Task<int> GetTotalOrdersCountAsync(Guid? storeId = null, DateTime? fromDate = null, DateTime? toDate = null)
        {
            var query = _dbSet
                .AsNoTracking()
                .Where(o => !o.IsDeleted);

            if (storeId.HasValue)
            {
                query = query.Where(o => o.StoreId == storeId.Value);
            }

            if (fromDate.HasValue)
            {
                query = query.Where(o => o.CreatedAt >= fromDate.Value);
            }

            if (toDate.HasValue)
            {
                var adjustedToDate = toDate.Value.AddDays(1);
                query = query.Where(o => o.CreatedAt < adjustedToDate);
            }

            return await query.CountAsync();
        }

        /// <summary>
        /// Get total revenue (optionally filtered by store and date range)
        /// Only counts Completed and Shipped orders
        /// </summary>
        public async Task<decimal> GetTotalRevenueAsync(Guid? storeId = null, DateTime? fromDate = null, DateTime? toDate = null)
        {
            var query = _dbSet
                .AsNoTracking()
                .Where(o => !o.IsDeleted && (o.Status == OrderStatus.Completed || o.Status == OrderStatus.Shipped));

            if (storeId.HasValue)
            {
                query = query.Where(o => o.StoreId == storeId.Value);
            }

            if (fromDate.HasValue)
            {
                query = query.Where(o => o.CreatedAt >= fromDate.Value);
            }

            if (toDate.HasValue)
            {
                var adjustedToDate = toDate.Value.AddDays(1);
                query = query.Where(o => o.CreatedAt < adjustedToDate);
            }

            return await query.SumAsync(o => (decimal?)o.TotalAmount) ?? 0m;
        }

        // ========================================
        // STATUS MANAGEMENT
        // ========================================

        /// <summary>
        /// Update order status and create status history entry.
        /// </summary>
        public async Task<bool> UpdateOrderStatusAsync(Guid orderId, OrderStatus newStatus, Guid changedBy, string? notes)
        {
            await using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var order = await _context.Orders
                    .Include(o => o.OrderStatusHistories)
                    .FirstOrDefaultAsync(o => o.Id == orderId && !o.IsDeleted);

                if (order == null)
                    return false;

                var oldStatus = order.Status; // ✅ Giữ lại giá trị cũ

                // Chỉ update nếu có thay đổi thật sự
                if (oldStatus == newStatus)
                    return true;

                // Cập nhật trạng thái
                order.Status = newStatus;
                order.UpdatedAt = DateTime.UtcNow;
                order.UpdatedBy = changedBy;

                // Tạo log lịch sử
                var statusHistory = new OrderStatusHistory
                {
                    OrderId = order.Id,
                    OldStatus = oldStatus,
                    NewStatus = newStatus,
                    ChangedBy = changedBy,
                    Notes = notes,
                    ChangedAt = DateTime.UtcNow
                };

                _context.OrderStatusHistories.Add(statusHistory);
                await _context.SaveChangesAsync();

                await transaction.CommitAsync();
                return true;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "Failed to update order status for OrderId={OrderId}", orderId);
                throw;
            }
        }


        /// <summary>
        /// Check if status transition is valid
        /// Pending → Confirmed → Shipped → Completed
        /// Any → Cancelled (with restrictions)
        /// </summary>
        public async Task<bool> CanChangeStatusAsync(Guid orderId, OrderStatus newStatus)
        {
            var order = await _dbSet
                .AsNoTracking()
                .FirstOrDefaultAsync(o => o.Id == orderId && !o.IsDeleted);

            if (order == null)
                return false;

            var currentStatus = order.Status;

            // Define valid transitions
            var validTransitions = new Dictionary<OrderStatus, List<OrderStatus>>
            {
                { OrderStatus.Pending, new List<OrderStatus> { OrderStatus.Confirmed, OrderStatus.Cancelled } },
                { OrderStatus.Confirmed, new List<OrderStatus> { OrderStatus.Shipped, OrderStatus.Cancelled } },
                { OrderStatus.Shipped, new List<OrderStatus> { OrderStatus.Completed, OrderStatus.Cancelled } },
                { OrderStatus.Completed, new List<OrderStatus>() }, // No transitions from Completed
                { OrderStatus.Cancelled, new List<OrderStatus>() }  // No transitions from Cancelled
            };

            // Special case: can't cancel from Shipped/Completed (business rule)
            if (newStatus == OrderStatus.Cancelled &&
                (currentStatus == OrderStatus.Shipped || currentStatus == OrderStatus.Completed))
            {
                return false;
            }

            return validTransitions.ContainsKey(currentStatus) &&
                   validTransitions[currentStatus].Contains(newStatus);
        }

        // ========================================
        // VALIDATION HELPERS
        // ========================================

        /// <summary>
        /// Check if order number already exists
        /// </summary>
        public async Task<bool> ExistsByOrderNumberAsync(string orderNumber)
        {
            return await _dbSet
                .AsNoTracking()
                .AnyAsync(o => o.OrderNumber == orderNumber && !o.IsDeleted);
        }

        /// <summary>
        /// Check if customer has any orders
        /// </summary>
        public async Task<bool> CustomerHasOrdersAsync(Guid customerId)
        {
            return await _dbSet
                .AsNoTracking()
                .AnyAsync(o => o.CustomerId == customerId && !o.IsDeleted);
        }
        /// <summary>
        /// Get orders for a specific user (customer view - only their orders)
        /// </summary>
        public async Task<PaginatedResult<Order>> GetUserOrdersAsync(
            Guid userId,
            OrderFilterDTO filter)
        {
            // Restrict to user's orders
            return await GetPaginatedAsync(filter, restrictToCustomerId: userId);
        }
        /// <summary>
        /// Get all orders (admin view - no restriction)
        /// </summary>
        public async Task<PaginatedResult<Order>> GetAllOrdersAsync(OrderFilterDTO filter)
        {
            // No restriction - admin sees all
            return await GetPaginatedAsync(filter, restrictToCustomerId: null);
        }
        /// <summary>
        /// Cancel an order (set status to Cancelled)
        /// </summary>
        public async Task<bool> CancelOrderAsync(
    Guid orderId,
    string reason,
    Guid changedBy)  // Accept Guid, not string
        {
            var canCancel = await CanChangeStatusAsync(orderId, OrderStatus.Cancelled);
            if (!canCancel)
                return false;

            return await UpdateOrderStatusAsync(
                orderId,
                OrderStatus.Cancelled,
                changedBy,  // Use the Guid directly
                reason);
        }
        public async Task<List<Order>> GetOrdersByStatusAsync(OrderStatus status)
        {
            // Validate status enum value
            if (!Enum.IsDefined(typeof(OrderStatus), status))
            {
                throw new ArgumentException($"Invalid order status: {status}", nameof(status));
            }

            return await _dbSet
                .AsNoTracking()
                .Where(o => o.Status == status && !o.IsDeleted)
                .Include(o => o.Customer)
                .Include(o => o.Store)
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Product)
                .Include(o => o.OrderShipping)
                .Include(o => o.OrderStatusHistories
                    .OrderByDescending(h => h.CreatedAt))
                .OrderByDescending(o => o.CreatedAt)
                .ToListAsync();
        }
        public async Task<IEnumerable<Order>> SearchOrdersAsync(string searchTerm)
        {
            var lowerTerm = searchTerm.ToLower().Trim();

            return await _dbSet
                .AsNoTracking()
                .Where(o => !o.IsDeleted && (
                    o.OrderNumber.ToLower().Contains(lowerTerm) ||
                    (o.Customer != null && o.Customer.Name.ToLower().Contains(lowerTerm)) ||
                    (o.Customer != null && o.Customer.Email != null && o.Customer.Email.ToLower().Contains(lowerTerm))
                ))
                .Include(o => o.Customer)
                .Include(o => o.OrderItems)
                .OrderByDescending(o => o.CreatedAt)
                .ToListAsync();
        }
        public async Task<int> BulkUpdateStatusAsync(
    List<Guid> orderIds,
    OrderStatus newStatus,
    Guid changedBy,
    string? reason = null)
        {
            var orders = await _context.Orders
                .Where(o => orderIds.Contains(o.Id) && !o.IsDeleted)
                .ToListAsync();

            foreach (var order in orders)
            {
                if (await CanChangeStatusAsync(order.Id, newStatus))
                {
                    order.Status = newStatus;
                    order.UpdatedAt = DateTime.UtcNow;
                    order.UpdatedBy = changedBy;

                    _context.OrderStatusHistories.Add(new OrderStatusHistory
                    {
                        Id = Guid.NewGuid(),
                        OrderId = order.Id,
                        OldStatus = order.Status,
                        NewStatus = newStatus,
                        ChangedBy = changedBy,
                        Notes = reason,
                        ChangedAt = DateTime.UtcNow
                    });
                }
            }

            return await _context.SaveChangesAsync();
        }
        public async Task<bool> UpdateStatusAsync(Guid orderId, OrderStatus newStatus)
        {
            return await UpdateOrderStatusAsync(orderId, newStatus, Guid.Empty, null);
        }

        public async Task<List<OrderStatusHistory>> GetStatusHistoryAsync(Guid orderId)
        {
            return await _context.OrderStatusHistories
                .AsNoTracking()
                .Where(h => h.OrderId == orderId)
                .OrderByDescending(h => h.ChangedAt)
                .ToListAsync();
        }
        // ========================================
        // PRIVATE HELPER METHODS
        // ========================================

        /// <summary>
        /// ✅ ONLY ONE ApplySorting method - NO DUPLICATES
        /// Apply sorting to query based on sortBy field and direction
        /// </summary>
        private IQueryable<Order> ApplySorting(IQueryable<Order> query, string sortBy, bool descending)
        {
            return sortBy.ToLower() switch
            {
                "ordernumber" => descending
                    ? query.OrderByDescending(o => o.OrderNumber)
                    : query.OrderBy(o => o.OrderNumber),

                "totalamount" => descending
                    ? query.OrderByDescending(o => o.TotalAmount)
                    : query.OrderBy(o => o.TotalAmount),

                "status" => descending
                    ? query.OrderByDescending(o => o.Status)
                    : query.OrderBy(o => o.Status),

                "customername" => descending
                    ? query.OrderByDescending(o => o.Customer != null ? o.Customer.Name : string.Empty)
                    : query.OrderBy(o => o.Customer != null ? o.Customer.Name : string.Empty),

                "storename" => descending
                    ? query.OrderByDescending(o => o.Store != null ? o.Store.Name : string.Empty)
                    : query.OrderBy(o => o.Store != null ? o.Store.Name : string.Empty),

                // Default: Sort by CreatedAt
                "createdat" or _ => descending
                    ? query.OrderByDescending(o => o.CreatedAt)
                    : query.OrderBy(o => o.CreatedAt)
            };
        }
    }
}