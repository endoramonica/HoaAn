using System;
using System.Threading.Tasks;
using VietCommerce.Core.DTOs.Notifications;
using VietCommerce.Core.DTOs.Orders;
using VietCommerce.Core.DTOs.Inventory;
using VietCommerce.Core.DTOs.Tasks;

namespace VietCommerce.Application.Services.Services.Interfaces
{
    public interface IRealtimeService
    {
        Task SendNotificationAsync(NotificationDto notification);
        Task BroadcastOrderCreatedAsync(OrderDetailDto order);
        Task BroadcastOrderStatusUpdatedAsync(Guid orderId, int status);
        Task BroadcastInventoryUpdatedAsync(InventoryDto inventory);
        Task BroadcastTaskUpdatedAsync(TaskDto task);
    }
}
