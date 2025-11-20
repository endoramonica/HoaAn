// File: VietCommerce.Data/Repositories/Interfaces/INotificationRepository.cs
using VietCommerce.Core.Entities.Notifications;

namespace VietCommerce.Data.Repositories.Interfaces;

public interface INotificationRepository : IGenericRepository<Notification>
{
    // Có thể thêm các method đặc thù sau này, ví dụ:
    // Task<IEnumerable<Notification>> GetUnreadByUserIdAsync(Guid userId, int take = 10);
}