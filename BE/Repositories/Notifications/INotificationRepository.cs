using VietCommerce.Core.Entities.Notifications;
using VietCommerce.Core.Models;
using VietCommerce.Data.Repositories.Generic;

namespace VietCommerce.Data.Repositories.Notifications;

public interface INotificationRepository : IGenericRepository<Notification>
{
    Task<PaginatedResult<Notification>> GetPaginatedByUserAsync(Guid userId, int pageNumber, int pageSize);
    Task<int> GetTotalCountByUserAsync(Guid userId);
}