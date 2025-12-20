using Microsoft.AspNetCore.SignalR;
using System;
using System.Threading.Tasks;
using VietCommerce.Api.Hubs;
using VietCommerce.Application.Services.Services.Interfaces;
using VietCommerce.Core.DTOs.Inventory;
using VietCommerce.Core.DTOs.Notifications;
using VietCommerce.Core.DTOs.Orders;
using VietCommerce.Core.DTOs.Tasks;

namespace VietCommerce.Api.Services
{
    public class SignalRRealtimeService : IRealtimeService
    {
        private readonly IHubContext<RealtimeHub> _hub;

        public SignalRRealtimeService(IHubContext<RealtimeHub> hub)
        {
            _hub = hub;
        }

        public Task SendNotificationAsync(NotificationDto notification)
        {
            return _hub.Clients.Group($"user:{notification.UserId}").SendAsync("ReceiveNotification", notification);
        }

        public Task BroadcastOrderCreatedAsync(OrderDetailDto order)
        {
            return _hub.Clients.Groups("domain:orders", $"store:{order.StoreId}").SendAsync("OrderCreated", order);
        }

        public Task BroadcastOrderStatusUpdatedAsync(Guid orderId, int status)
        {
            return _hub.Clients.Group("domain:orders").SendAsync("OrderStatusUpdated", new { orderId, status });
        }

        public Task BroadcastInventoryUpdatedAsync(InventoryDto inventory)
        {
            return _hub.Clients.Groups("domain:inventory", $"store:{inventory.StoreId}").SendAsync("InventoryUpdated", inventory);
        }

        public Task BroadcastTaskUpdatedAsync(TaskDto task)
        {
            return _hub.Clients.Groups("domain:tasks", $"user:{task.AssignedTo}").SendAsync("TaskUpdated", task);
        }
    }
}
