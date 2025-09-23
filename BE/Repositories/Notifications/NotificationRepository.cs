using Microsoft.EntityFrameworkCore;
using VietCommerce.Core.Entities.Notifications;
using VietCommerce.Core.Models;
using VietCommerce.Data;
using VietCommerce.Data.Context;
using VietCommerce.Data.Repositories.Generic;
using VietCommerce.Data.Repositories.Notifications;

namespace VietCommerce.Data.Repositories.Notifications;

public class NotificationRepository : GenericRepository<Notification>, INotificationRepository
{
    public NotificationRepository(AppDbContext context) : base(context) { }

    public async Task<PaginatedResult<Notification>> GetPaginatedByUserAsync(Guid userId, int pageNumber, int pageSize)
    {
        var skip = (pageNumber - 1) * pageSize;
        var items = await _context.Notifications
            .Where(n => n.UserId == userId)
            .OrderByDescending(n => n.CreatedDate)
            .Skip(skip)
            .Take(pageSize)
            .ToListAsync();
        var total = await _context.Notifications.CountAsync(n => n.UserId == userId);
        return new PaginatedResult<Notification>(items, pageNumber, pageSize, total);
    }

    public async Task<int> GetTotalCountByUserAsync(Guid userId)
    {
        return await _context.Notifications.CountAsync(n => n.UserId == userId);
    }
}