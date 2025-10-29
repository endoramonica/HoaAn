// File: VietCommerce.Data/Repositories/OrderStatusHistoryRepository.cs
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using VietCommerce.Core.DTOs.Orders;
using VietCommerce.Core.Entities.Orders;
using VietCommerce.Core.Enums.Orders;
using VietCommerce.Data.Context;
using VietCommerce.Data.Repositories.Interfaces;

namespace VietCommerce.Data.Repositories;

public class OrderStatusHistoryRepository : GenericRepository<OrderStatusHistory>, IOrderStatusHistoryRepository
{
    private readonly AppDbContext _context;
    private readonly ILogger<OrderStatusHistoryRepository> _logger;

    public OrderStatusHistoryRepository(AppDbContext context, ILogger<OrderStatusHistoryRepository> logger) : base(context)
    {
        _context = context;
        _logger = logger;
    }

    /// <summary>
    /// Get all status history for an order
    /// </summary>
    public async Task<IEnumerable<OrderStatusHistory>> GetByOrderIdAsync(Guid orderId)
    {
        return await _dbSet
            .AsNoTracking()
            .Where(h => h.OrderId == orderId)
            .OrderByDescending(h => h.ChangedAt)
            .ToListAsync();
    }

    /// <summary>
    /// Get status history with user details
    /// </summary>
    public async Task<IEnumerable<OrderStatusHistory>> GetByOrderIdWithUserAsync(Guid orderId)
    {
        return await _dbSet
            .AsNoTracking()
            .Where(h => h.OrderId == orderId)
            .Include(h => h.ChangedByUser)
            .Include(h => h.Order)
            .OrderByDescending(h => h.ChangedAt)
            .ToListAsync();
    }

    /// <summary>
    /// Get latest status change
    /// </summary>
    public async Task<OrderStatusHistory?> GetLatestStatusAsync(Guid orderId)
    {
        return await _dbSet
            .AsNoTracking()
            .Where(h => h.OrderId == orderId)
            .OrderByDescending(h => h.ChangedAt)
            .FirstOrDefaultAsync();
    }

    /// <summary>
    /// Get status history between dates
    /// </summary>
    public async Task<IEnumerable<OrderStatusHistory>> GetByOrderIdBetweenDatesAsync(
        Guid orderId,
        DateTime fromDate,
        DateTime toDate)
    {
        return await _dbSet
            .AsNoTracking()
            .Where(h => h.OrderId == orderId &&
                       h.ChangedAt >= fromDate &&
                       h.ChangedAt <= toDate.AddDays(1))
            .OrderByDescending(h => h.ChangedAt)
            .ToListAsync();
    }

    /// <summary>
    /// Get all status transitions for specific status
    /// </summary>
    public async Task<IEnumerable<OrderStatusHistory>> GetByStatusAsync(OrderStatus status)
    {
        return await _dbSet
            .AsNoTracking()
            .Where(h => h.NewStatus == status)
            .Include(h => h.Order)
            .OrderByDescending(h => h.ChangedAt)
            .ToListAsync();
    }

    /// <summary>
    /// Get status history by user
    /// </summary>
    public async Task<IEnumerable<OrderStatusHistory>> GetByChangedByAsync(Guid userId)
    {
        return await _dbSet
            .AsNoTracking()
            .Where(h => h.ChangedBy == userId)
            .Include(h => h.Order)
            .OrderByDescending(h => h.ChangedAt)
            .ToListAsync();
    }

    /// <summary>
    /// Get status changes in date range
    /// </summary>
    public async Task<IEnumerable<OrderStatusHistory>> GetStatusChangesBetweenDatesAsync(
        OrderStatus status,
        DateTime fromDate,
        DateTime toDate)
    {
        return await _dbSet
            .AsNoTracking()
            .Where(h => h.NewStatus == status &&
                       h.ChangedAt >= fromDate &&
                       h.ChangedAt <= toDate.AddDays(1))
            .Include(h => h.Order)
            .OrderByDescending(h => h.ChangedAt)
            .ToListAsync();
    }

    /// <summary>
    /// Get time spent in each status for an order
    /// </summary>
    public async Task<IEnumerable<OrderStatusDurationDto>> GetStatusDurationsAsync(Guid orderId)
    {
        var histories = await _dbSet
            .AsNoTracking()
            .Where(h => h.OrderId == orderId)
            .OrderBy(h => h.ChangedAt)
            .ToListAsync();

        var durations = new List<OrderStatusDurationDto>();

        for (int i = 0; i < histories.Count; i++)
        {
            var current = histories[i];
            var nextChange = i + 1 < histories.Count ? histories[i + 1] : null;

            var duration = nextChange != null
                ? (nextChange.ChangedAt - current.ChangedAt).TotalHours
                : (DateTime.UtcNow - current.ChangedAt).TotalHours;

            durations.Add(new OrderStatusDurationDto
            {
                Status = current.NewStatus,
                StartTime = current.ChangedAt,
                EndTime = nextChange?.ChangedAt,
                DurationHours = duration,
                Notes = current.Notes
            });
        }

        return durations;
    }

    /// <summary>
    /// Get average time per status
    /// </summary>
    public async Task<IEnumerable<AverageStatusDurationDto>> GetAverageStatusDurationsAsync(
        DateTime? fromDate = null,
        DateTime? toDate = null)
    {
        var query = _dbSet.AsNoTracking();

        if (fromDate.HasValue)
            query = query.Where(h => h.ChangedAt >= fromDate.Value);

        if (toDate.HasValue)
            query = query.Where(h => h.ChangedAt <= toDate.Value.AddDays(1));

        var histories = await query
            .OrderBy(h => h.OrderId)
            .ThenBy(h => h.ChangedAt)
            .ToListAsync();

        var groupedByOrder = histories.GroupBy(h => h.OrderId).ToList();
        var statusDurations = new Dictionary<OrderStatus, List<double>>();

        foreach (var orderHistories in groupedByOrder)
        {
            var sortedHistories = orderHistories.OrderBy(h => h.ChangedAt).ToList();

            for (int i = 0; i < sortedHistories.Count; i++)
            {
                var current = sortedHistories[i];
                var next = i + 1 < sortedHistories.Count ? sortedHistories[i + 1] : null;

                if (next != null)
                {
                    var duration = (next.ChangedAt - current.ChangedAt).TotalHours;

                    if (!statusDurations.ContainsKey(current.NewStatus))
                        statusDurations[current.NewStatus] = new List<double>();

                    statusDurations[current.NewStatus].Add(duration);
                }
            }
        }

        return statusDurations
            .Select(kvp => new AverageStatusDurationDto
            {
                Status = kvp.Key,
                AverageDurationHours = kvp.Value.Average(),
                MinDurationHours = kvp.Value.Min(),
                MaxDurationHours = kvp.Value.Max(),
                Count = kvp.Value.Count
            })
            .OrderBy(x => x.Status)
            .ToList();
    }

    /// <summary>
    /// Get stuck orders
    /// </summary>
    public async Task<IEnumerable<OrderStatusHistory>> GetStuckOrdersAsync(OrderStatus status, int stuckHours = 24)
    {
        var stuckTime = DateTime.UtcNow.AddHours(-stuckHours);

        return await _dbSet
            .AsNoTracking()
            .Where(h => h.NewStatus == status && h.ChangedAt <= stuckTime)
            .Include(h => h.Order)
            .OrderBy(h => h.ChangedAt)
            .ToListAsync();
    }

    /// <summary>
    /// Get status count
    /// </summary>
    public async Task<Dictionary<OrderStatus, int>> GetStatusCountAsync()
    {
        var histories = await _dbSet
            .AsNoTracking()
            .ToListAsync();

        // Get latest status for each order
        var latestStatuses = histories
            .GroupBy(h => h.OrderId)
            .Select(g => g.OrderByDescending(h => h.ChangedAt).First())
            .GroupBy(h => h.NewStatus)
            .ToDictionary(g => g.Key, g => g.Count());

        return latestStatuses;
    }

    /// <summary>
    /// Check if order passed through status
    /// </summary>
    public async Task<bool> HasPassedStatusAsync(Guid orderId, OrderStatus status)
    {
        return await _dbSet
            .AsNoTracking()
            .AnyAsync(h => h.OrderId == orderId && h.NewStatus == status);
    }

    /// <summary>
    /// Get time when reached status
    /// </summary>
    public async Task<DateTime?> GetStatusReachedTimeAsync(Guid orderId, OrderStatus status)
    {
        var history = await _dbSet
            .AsNoTracking()
            .Where(h => h.OrderId == orderId && h.NewStatus == status)
            .OrderBy(h => h.ChangedAt)
            .FirstOrDefaultAsync();

        return history?.ChangedAt;
    }
}

