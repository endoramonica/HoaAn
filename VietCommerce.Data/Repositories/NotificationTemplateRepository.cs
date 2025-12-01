// File: VietCommerce.Data/Repositories/NotificationTemplateRepository.cs
using Microsoft.EntityFrameworkCore;
using VietCommerce.Core.Entities.Notifications;
using VietCommerce.Core.Enums.Notifications;
using VietCommerce.Data.Context;
using VietCommerce.Data.Repositories.Interfaces;

namespace VietCommerce.Data.Repositories;

public class NotificationTemplateRepository : GenericRepository<NotificationTemplate>, INotificationTemplateRepository
{
    public NotificationTemplateRepository(AppDbContext context) : base(context)
    {
    }

    public async Task<NotificationTemplate?> GetActiveByNameAsync(string name)
    {
        return await _dbSet
            .FirstOrDefaultAsync(t => t.Name == name && t.IsActive && !t.IsDeleted);
    }

    public async Task<NotificationTemplate?> GetActiveByTypeAsync(NotificationType type)
    {
        return await _dbSet
            .FirstOrDefaultAsync(t => t.Type == type
                                   && t.IsActive
                                   && !t.IsDeleted);
    }

}