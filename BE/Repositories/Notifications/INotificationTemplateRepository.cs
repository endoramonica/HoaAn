using VietCommerce.Core.Entities.Notifications;
using VietCommerce.Core.Enums;
using VietCommerce.Core.Enums.Notifications;
using VietCommerce.Data.Repositories.Generic;

namespace VietCommerce.Data.Repositories.Notifications;

public interface INotificationTemplateRepository : IGenericRepository<NotificationTemplate>
{
    Task<NotificationTemplate?> GetByTypeAsync(NotificationType type);
}