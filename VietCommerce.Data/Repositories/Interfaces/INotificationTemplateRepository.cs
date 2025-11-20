// File: VietCommerce.Data/Repositories/Interfaces/INotificationTemplateRepository.cs
using VietCommerce.Core.Entities.Notifications;

namespace VietCommerce.Data.Repositories.Interfaces;

public interface INotificationTemplateRepository : IGenericRepository<NotificationTemplate>
{
    Task<NotificationTemplate?> GetActiveByNameAsync(string name);
    Task<NotificationTemplate?> GetActiveByTypeAsync(string type);
}