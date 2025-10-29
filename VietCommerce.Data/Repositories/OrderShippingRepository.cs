// ============================================================
// File: VietCommerce.Data/Repositories/OrderShippingRepository.cs
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using VietCommerce.Core.DTOs.Orders;
using VietCommerce.Core.Entities.Orders;
using VietCommerce.Core.Enums.Orders;
using VietCommerce.Data.Context;
using VietCommerce.Data.Repositories.Interfaces;

namespace VietCommerce.Data.Repositories;

public class OrderShippingRepository : GenericRepository<OrderShipping>, IOrderShippingRepository
{
    private readonly AppDbContext _context;
    private readonly ILogger<OrderShippingRepository> _logger;

    public OrderShippingRepository(AppDbContext context, ILogger<OrderShippingRepository> logger) : base(context)
    {
        _context = context;
        _logger = logger;
    }

    /// <summary>
    /// Get shipping info by Order ID
    /// </summary>
    public async Task<OrderShipping?> GetByOrderIdAsync(Guid orderId)
    {
        return await _dbSet
            .AsNoTracking()
            .Where(os => os.OrderId == orderId && !os.IsDeleted)
            .Include(os => os.Order)
            .FirstOrDefaultAsync();
    }

    /// <summary>
    /// Get shipping records by status
    /// </summary>
    public async Task<IEnumerable<OrderShipping>> GetByStatusAsync(ShippingStatus status)
    {
        return await _dbSet
            .AsNoTracking()
            .Where(os => os.Status == status && !os.IsDeleted)
            .Include(os => os.Order)
            .OrderByDescending(os => os.CreatedAt)
            .ToListAsync();
    }

    /// <summary>
    /// Get shipping records by status with pagination
    /// </summary>
    public async Task<IEnumerable<OrderShipping>> GetByStatusAsync(ShippingStatus status, int skip, int take)
    {
        return await _dbSet
            .AsNoTracking()
            .Where(os => os.Status == status && !os.IsDeleted)
            .Include(os => os.Order)
            .OrderByDescending(os => os.CreatedAt)
            .Skip(skip)
            .Take(take)
            .ToListAsync();
    }

    /// <summary>
    /// Get pending shipments (not yet delivered or failed)
    /// </summary>
    public async Task<IEnumerable<OrderShipping>> GetPendingShipmentsAsync()
    {
        return await _dbSet
            .AsNoTracking()
            .Where(os => !os.IsDeleted &&
                        (os.Status == ShippingStatus.PREPARING ||
                         os.Status == ShippingStatus.PICKED_UP ||
                         os.Status == ShippingStatus.IN_TRANSIT))
            .Include(os => os.Order)
            .OrderBy(os => os.CreatedAt)
            .ToListAsync();
    }

    /// <summary>
    /// Get shipped orders within date range
    /// </summary>
    public async Task<IEnumerable<OrderShipping>> GetShippedBetweenDatesAsync(DateTime fromDate, DateTime toDate)
    {
        return await _dbSet
            .AsNoTracking()
            .Where(os => !os.IsDeleted &&
                        os.ShippedAt.HasValue &&
                        os.ShippedAt >= fromDate &&
                        os.ShippedAt <= toDate.AddDays(1))
            .Include(os => os.Order)
            .OrderByDescending(os => os.ShippedAt)
            .ToListAsync();
    }

    /// <summary>
    /// Get delivered orders within date range
    /// </summary>
    public async Task<IEnumerable<OrderShipping>> GetDeliveredBetweenDatesAsync(DateTime fromDate, DateTime toDate)
    {
        return await _dbSet
            .AsNoTracking()
            .Where(os => !os.IsDeleted &&
                        os.DeliveredAt.HasValue &&
                        os.DeliveredAt >= fromDate &&
                        os.DeliveredAt <= toDate.AddDays(1))
            .Include(os => os.Order)
            .OrderByDescending(os => os.DeliveredAt)
            .ToListAsync();
    }

    /// <summary>
    /// Get failed/returned shipments
    /// </summary>
    public async Task<IEnumerable<OrderShipping>> GetFailedShipmentsAsync()
    {
        return await _dbSet
            .AsNoTracking()
            .Where(os => !os.IsDeleted &&
                        (os.Status == ShippingStatus.FAILED ||
                         os.Status == ShippingStatus.RETURNED))
            .Include(os => os.Order)
            .OrderByDescending(os => os.CreatedAt)
            .ToListAsync();
    }

    /// <summary>
    /// Update shipping status
    /// </summary>
    public async Task<bool> UpdateShippingStatusAsync(Guid shippingId, ShippingStatus newStatus)
    {
        var shipping = await _dbSet.FirstOrDefaultAsync(os => os.Id == shippingId && !os.IsDeleted);
        if (shipping == null)
            return false;

        var oldStatus = shipping.Status;
        shipping.Status = newStatus;
        shipping.UpdatedAt = DateTime.UtcNow;

        _logger.LogInformation("Shipping {ShippingId} status changed from {OldStatus} to {NewStatus}",
            shippingId, oldStatus, newStatus);

        await _context.SaveChangesAsync();
        return true;
    }

    /// <summary>
    /// Mark as shipped
    /// </summary>
    public async Task<bool> MarkAsShippedAsync(Guid shippingId)
    {
        var shipping = await _dbSet.FirstOrDefaultAsync(os => os.Id == shippingId && !os.IsDeleted);
        if (shipping == null)
            return false;

        if (shipping.ShippedAt != null)
            return true; // Already marked

        shipping.ShippedAt = DateTime.UtcNow;
        shipping.Status = ShippingStatus.PICKED_UP;
        shipping.UpdatedAt = DateTime.UtcNow;

        _logger.LogInformation("Shipping {ShippingId} marked as shipped at {ShippedAt}",
            shippingId, shipping.ShippedAt);

        await _context.SaveChangesAsync();
        return true;
    }

    /// <summary>
    /// Mark as delivered
    /// </summary>
    public async Task<bool> MarkAsDeliveredAsync(Guid shippingId)
    {
        var shipping = await _dbSet.FirstOrDefaultAsync(os => os.Id == shippingId && !os.IsDeleted);
        if (shipping == null)
            return false;

        if (shipping.DeliveredAt != null)
            return true; // Already delivered

        shipping.DeliveredAt = DateTime.UtcNow;
        shipping.Status = ShippingStatus.DELIVERED;
        shipping.UpdatedAt = DateTime.UtcNow;

        _logger.LogInformation("Shipping {ShippingId} delivered at {DeliveredAt}",
            shippingId, shipping.DeliveredAt);

        await _context.SaveChangesAsync();
        return true;
    }

    /// <summary>
    /// Get shipping statistics
    /// </summary>
    public async Task<ShippingStatisticsDto> GetShippingStatisticsAsync(
    Guid storeId,
    DateTime? fromDate = null,
    DateTime? toDate = null)
    {
        var query = _dbSet
            .AsNoTracking()
            .Include(os => os.Order)
            .Where(os => !os.IsDeleted && os.Order.StoreId == storeId);

        if (fromDate.HasValue)
            query = query.Where(os => os.CreatedAt >= fromDate.Value);

        if (toDate.HasValue)
            query = query.Where(os => os.CreatedAt <= toDate.Value.AddDays(1));

        var shippings = await query.ToListAsync();

        return new ShippingStatisticsDto
        {
            TotalShipments = shippings.Count,
            PreparedCount = shippings.Count(s => s.Status == ShippingStatus.PREPARING),
            PickedUpCount = shippings.Count(s => s.Status == ShippingStatus.PICKED_UP),
            InTransitCount = shippings.Count(s => s.Status == ShippingStatus.IN_TRANSIT),
            DeliveredCount = shippings.Count(s => s.Status == ShippingStatus.DELIVERED),
            FailedCount = shippings.Count(s => s.Status == ShippingStatus.FAILED),
            ReturnedCount = shippings.Count(s => s.Status == ShippingStatus.RETURNED),
            TotalShippingCost = shippings.Sum(s => s.ShippingCost),
            SuccessfulDeliveryRate = shippings.Count > 0
                ? (double)shippings.Count(s => s.Status == ShippingStatus.DELIVERED) / shippings.Count * 100
                : 0
        };
    }


    /// <summary>
    /// Get average delivery time
    /// </summary>
    public async Task<double> GetAverageDeliveryTimeAsync(Guid storeId, DateTime? fromDate = null)
    {
        var query = _dbSet
            .AsNoTracking()
            .Include(os => os.Order)
            .Where(os => !os.IsDeleted &&
                         os.Order.StoreId == storeId &&
                         os.DeliveredAt.HasValue &&
                         os.ShippedAt.HasValue);

        if (fromDate.HasValue)
            query = query.Where(os => os.ShippedAt >= fromDate.Value);

        var deliveredShippings = await query.ToListAsync();

        if (!deliveredShippings.Any())
            return 0;

        var averageDays = deliveredShippings
            .Average(s => (s.DeliveredAt.Value - s.ShippedAt.Value).TotalDays);

        return averageDays;
    }


    /// <summary>
    /// Get delayed shipments (exceeded SLA)
    /// </summary>
    public async Task<IEnumerable<OrderShipping>> GetDelayedShipmentsAsync(int slaThresholdDays = 7)
    {
        var thresholdDate = DateTime.UtcNow.AddDays(-slaThresholdDays);

        return await _dbSet
            .AsNoTracking()
            .Where(os => !os.IsDeleted &&
                        os.Status != ShippingStatus.DELIVERED &&
                        os.Status != ShippingStatus.RETURNED &&
                        os.CreatedAt <= thresholdDate)
            .Include(os => os.Order)
            .OrderBy(os => os.CreatedAt)
            .ToListAsync();
    }

    /// <summary>
    /// Get shipping with order details
    /// </summary>
    public async Task<OrderShipping?> GetWithOrderDetailsAsync(Guid shippingId)
    {
        return await _dbSet
            .AsNoTracking()
            .Where(os => os.Id == shippingId && !os.IsDeleted)
            .Include(os => os.Order)
            .ThenInclude(o => o.Customer)
            .Include(os => os.Order)
            .ThenInclude(o => o.Store)
            .FirstOrDefaultAsync();
    }
}