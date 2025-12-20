using Microsoft.AspNetCore.SignalR;
using System;
using System.Threading.Tasks;
using VietCommerce.AdminApi.Hubs;
using VietCommerce.Application.Services.Services.Interfaces;
using VietCommerce.Core.DTOs.Inventory;
using VietCommerce.Core.DTOs.Notifications;
using VietCommerce.Core.DTOs.Orders;
using VietCommerce.Core.DTOs.Tasks;

namespace VietCommerce.AdminApi.Services
{
    public class AdminSignalRRealtimeService : IRealtimeService
    {
        private readonly IHubContext<AdminRealtimeHub> _hub;

        public AdminSignalRRealtimeService(IHubContext<AdminRealtimeHub> hub)
        {
            _hub = hub;
        }

        public Task SendNotificationAsync(NotificationDto notification)
        {
            return _hub.Clients.Group($"user:{notification.UserId}").SendAsync("ReceiveNotification", notification);
        }

        public Task BroadcastOrderCreatedAsync(OrderDetailDto order)
        {
            return _hub.Clients.Groups("domain:orders", $"store:{order.StoreId}", "domain:admin").SendAsync("OrderCreated", order);
        }

        public Task BroadcastOrderStatusUpdatedAsync(Guid orderId, int status)
        {
            return _hub.Clients.Groups("domain:orders", "domain:admin").SendAsync("OrderStatusUpdated", new { orderId, status });
        }

        public Task BroadcastInventoryUpdatedAsync(InventoryDto inventory)
        {
            return _hub.Clients.Groups("domain:inventory", $"store:{inventory.StoreId}", "domain:admin").SendAsync("InventoryUpdated", inventory);
        }

        public Task BroadcastTaskUpdatedAsync(TaskDto task)
        {
            return _hub.Clients.Groups("domain:tasks", $"user:{task.AssignedTo}", "domain:admin").SendAsync("TaskUpdated", task);
        }
    }
}
