using VietCommerce.Core.Entities;
using VietCommerce.Core.Models;
using VietCommerce.Core.Entities.Orders;
using VietCommerce.Core.DTOs.Orders;
using VietCommerce.Core.Enums.Orders;

namespace VietCommerce.Data.Repositories.Interfaces;

public interface IOrderRepository : IGenericRepository<Order>
{
    // Custom query methods
    Task<IEnumerable<Order>> GetByCustomerIdAsync(Guid customerId);
    Task<IEnumerable<Order>> GetByStoreIdAsync(Guid storeId);
    Task<IEnumerable<Order>> GetByStatusAsync(OrderStatus status);
    Task<IEnumerable<Order>> GetRecentOrdersAsync(int count = 10, Guid? storeId = null);
    Task<PaginatedResult<Order>> GetPaginatedAsync(OrderFilterDTO filter, Guid? restrictToCustomerId = null);

    Task<List<Order>> GetOrdersByStatusAsync(OrderStatus status);

    // Methods from IOrderService (mapped to repository)
    Task<PaginatedResult<Order>> GetUserOrdersAsync(Guid userId, OrderFilterDTO filter); // For user-specific orders
    Task<bool> UpdateOrderStatusAsync(Guid orderId, OrderStatus newStatus, Guid changedBy, string? notes ); // Status update
    Task<bool> CancelOrderAsync(Guid orderId, string reason, Guid changedBy); // Cancel order
    Task<PaginatedResult<Order>> GetAllOrdersAsync(OrderFilterDTO filter); // Admin order list
    Task<Order> GetByIdWithDetailsAsync(Guid orderId);
    Task<bool> UpdateStatusAsync(Guid orderId, OrderStatus newStatus);
    Task<List<OrderStatusHistory>> GetStatusHistoryAsync(Guid orderId);
    Task<IEnumerable<Order>> SearchOrdersAsync(string searchTerm);
    Task<int> BulkUpdateStatusAsync(List<Guid> orderIds,OrderStatus newStatus,Guid changedBy,string? reason = null);
    Task<bool> CanChangeStatusAsync(Guid orderId, OrderStatus newStatus);
    Task<decimal> GetTotalRevenueAsync(Guid? storeId = null, DateTime? fromDate = null, DateTime? toDate = null);
    Task<int> GetTotalOrdersCountAsync(Guid? storeId = null, DateTime? fromDate = null, DateTime? toDate = null);
    Task<bool> ExistsByOrderNumberAsync(string orderNumber);    


}