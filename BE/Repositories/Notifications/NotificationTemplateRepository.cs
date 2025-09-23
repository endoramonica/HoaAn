using Microsoft.EntityFrameworkCore;
using VietCommerce.Core.Entities.Notifications;
using VietCommerce.Core.Enums;
using VietCommerce.Core.Enums.Notifications;
using VietCommerce.Data;
using VietCommerce.Data.Context;
using VietCommerce.Data.Repositories.Generic;
using VietCommerce.Data.Repositories.Notifications;

namespace VietCommerce.Data.Repositories.Notifications;

public class NotificationTemplateRepository : GenericRepository<NotificationTemplate>, INotificationTemplateRepository
{
    public NotificationTemplateRepository(AppDbContext context) : base(context) { }

    public async Task<NotificationTemplate?> GetByTypeAsync(NotificationType type)
    {
        return await _context.NotificationTemplates.FirstOrDefaultAsync(t => t.Type == type);
    }
}