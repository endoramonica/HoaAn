using VietCommerce.Core.DTOs.Notifications;
using VietCommerce.Core.Enums.Orders;
using VietCommerce.Core.Models;

namespace VietCommerce.Core.Services.Notifications;

public interface INotificationService
{
    Task<PaginatedResult<NotificationListDTO>> GetPaginatedAsync(Guid userId, int pageNumber = 1, int pageSize = 10);
    Task<NotificationListDTO> GetByIdAsync(Guid id);
    Task<Guid> CreateAsync(NotificationCreateDTO dto);
    Task UpdateAsync(Guid id, NotificationUpdateDTO dto);
    Task DeleteAsync(Guid id);
    Task SendOrderStatusNotificationAsync(Guid userId, OrderStatus status, string notes);
}