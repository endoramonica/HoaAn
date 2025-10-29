// File: VietCommerce.Data/Repositories/Interfaces/IOrderShippingRepository.cs
using VietCommerce.Core.DTOs.Orders;
using VietCommerce.Core.Entities.Orders;
using VietCommerce.Core.Enums.Orders;

namespace VietCommerce.Data.Repositories.Interfaces;

public interface IOrderShippingRepository : IGenericRepository<OrderShipping>
{
    
    /// Get shipping info by Order ID
    
    Task<OrderShipping?> GetByOrderIdAsync(Guid orderId);

    
    /// Get all shipping records by status (PREPARING, PICKED_UP, IN_TRANSIT, DELIVERED, FAILED, RETURNED)
    
    Task<IEnumerable<OrderShipping>> GetByStatusAsync(ShippingStatus status);

    
    /// Get shipping records by status with pagination
    
    Task<IEnumerable<OrderShipping>> GetByStatusAsync(ShippingStatus status, int skip, int take);

    
    /// Get pending shipments (PREPARING, PICKED_UP, IN_TRANSIT)
    
    Task<IEnumerable<OrderShipping>> GetPendingShipmentsAsync();

    
    /// Get shipped orders within date range
    
    Task<IEnumerable<OrderShipping>> GetShippedBetweenDatesAsync(DateTime fromDate, DateTime toDate);

    
    /// Get delivered orders within date range
    
    Task<IEnumerable<OrderShipping>> GetDeliveredBetweenDatesAsync(DateTime fromDate, DateTime toDate);

    
    /// Get failed/returned shipments
    
    Task<IEnumerable<OrderShipping>> GetFailedShipmentsAsync();

    
    /// Update shipping status and track timestamps
    
    Task<bool> UpdateShippingStatusAsync(Guid shippingId, ShippingStatus newStatus);

    
    /// Mark order as shipped (set ShippedAt timestamp)
    
    Task<bool> MarkAsShippedAsync(Guid shippingId);

    
    /// Mark order as delivered (set DeliveredAt timestamp)
    
    Task<bool> MarkAsDeliveredAsync(Guid shippingId);

    
    /// Get shipping statistics for a store
    
    Task<ShippingStatisticsDto> GetShippingStatisticsAsync(Guid storeId, DateTime? fromDate = null, DateTime? toDate = null);

    
    /// Get average delivery time (days)
    
    Task<double> GetAverageDeliveryTimeAsync(Guid storeId, DateTime? fromDate = null);

    
    /// Get shipping records that exceeded delivery SLA
    
    Task<IEnumerable<OrderShipping>> GetDelayedShipmentsAsync(int slaThresholdDays = 7);

    
    /// Get shipping info with order details
    
    Task<OrderShipping?> GetWithOrderDetailsAsync(Guid shippingId);
}
