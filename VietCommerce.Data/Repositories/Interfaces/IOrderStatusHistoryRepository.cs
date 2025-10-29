// File: VietCommerce.Data/Repositories/Interfaces/IOrderStatusHistoryRepository.cs
using VietCommerce.Core.DTOs.Orders;
using VietCommerce.Core.Entities.Orders;
using VietCommerce.Core.Enums.Orders;

namespace VietCommerce.Data.Repositories.Interfaces;

public interface IOrderStatusHistoryRepository : IGenericRepository<OrderStatusHistory>
{
    /// <summary>
    /// Get all status history for an order (ordered by date DESC)
    /// </summary>
    Task<IEnumerable<OrderStatusHistory>> GetByOrderIdAsync(Guid orderId);

    /// <summary>
    /// Get status history with user details (who changed status)
    /// </summary>
    Task<IEnumerable<OrderStatusHistory>> GetByOrderIdWithUserAsync(Guid orderId);

    /// <summary>
    /// Get latest status change for an order
    /// </summary>
    Task<OrderStatusHistory?> GetLatestStatusAsync(Guid orderId);

    /// <summary>
    /// Get status history between dates
    /// </summary>
    Task<IEnumerable<OrderStatusHistory>> GetByOrderIdBetweenDatesAsync(
        Guid orderId,
        DateTime fromDate,
        DateTime toDate);

    /// <summary>
    /// Get all status transitions for specific status
    /// </summary>
    Task<IEnumerable<OrderStatusHistory>> GetByStatusAsync(OrderStatus status);

    /// <summary>
    /// Get status history by changed user
    /// </summary>
    Task<IEnumerable<OrderStatusHistory>> GetByChangedByAsync(Guid userId);

    /// <summary>
    /// Get orders that changed to specific status in date range
    /// </summary>
    Task<IEnumerable<OrderStatusHistory>> GetStatusChangesBetweenDatesAsync(
        OrderStatus status,
        DateTime fromDate,
        DateTime toDate);

    /// <summary>
    /// Get time spent in each status for an order
    /// </summary>
    Task<IEnumerable<OrderStatusDurationDto>> GetStatusDurationsAsync(Guid orderId);

    /// <summary>
    /// Get average time per status across orders
    /// </summary>
    Task<IEnumerable<AverageStatusDurationDto>> GetAverageStatusDurationsAsync(
        DateTime? fromDate = null,
        DateTime? toDate = null);

    /// <summary>
    /// Get orders that are stuck in a status (not progressed)
    /// </summary>
    Task<IEnumerable<OrderStatusHistory>> GetStuckOrdersAsync(OrderStatus status, int stuckHours = 24);

    /// <summary>
    /// Get status history count by status
    /// </summary>
    Task<Dictionary<OrderStatus, int>> GetStatusCountAsync();

    /// <summary>
    /// Check if order has transitioned through specific status
    /// </summary>
    Task<bool> HasPassedStatusAsync(Guid orderId, OrderStatus status);

    /// <summary>
    /// Get time when order reached specific status
    /// </summary>
    Task<DateTime?> GetStatusReachedTimeAsync(Guid orderId, OrderStatus status);
}