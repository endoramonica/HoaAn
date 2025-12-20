using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;
using System.Threading.Tasks;
using System;
using VietCommerce.Core.DTOs.Orders;
using VietCommerce.Core.DTOs.Inventory;
using VietCommerce.Core.DTOs.Tasks;
using VietCommerce.Core.DTOs.Notifications;

namespace VietCommerce.AdminApi.Hubs
{
    [Authorize(Policy = "AdminOnly")]
    public class AdminRealtimeHub : Hub
    {
        public override async Task OnConnectedAsync()
        {
            var userId = Context.User?.FindFirstValue(ClaimTypes.NameIdentifier);
            var role = Context.User?.FindFirstValue(ClaimTypes.Role);
            var storeId = Context.User?.FindFirst("storeId")?.Value;
            if (!string.IsNullOrEmpty(userId))
                await Groups.AddToGroupAsync(Context.ConnectionId, $"user:{userId}");
            if (!string.IsNullOrEmpty(role))
                await Groups.AddToGroupAsync(Context.ConnectionId, $"role:{role}");
            if (!string.IsNullOrEmpty(storeId))
                await Groups.AddToGroupAsync(Context.ConnectionId, $"store:{storeId}");
            await Groups.AddToGroupAsync(Context.ConnectionId, "domain:admin");
            await base.OnConnectedAsync();
        }

        public Task JoinDomain(string domain)
        {
            return Groups.AddToGroupAsync(Context.ConnectionId, $"domain:{domain}");
        }

        public Task LeaveDomain(string domain)
        {
            return Groups.RemoveFromGroupAsync(Context.ConnectionId, $"domain:{domain}");
        }

        public Task SendNotification(NotificationDto notification)
        {
            return Clients.Group($"user:{notification.UserId}").SendAsync("ReceiveNotification", notification);
        }

        public Task OrderCreated(OrderDetailDto order)
        {
            return Clients.Groups($"domain:orders", $"store:{order.StoreId}", "domain:admin").SendAsync("OrderCreated", order);
        }

        public Task OrderStatusUpdated(Guid orderId, int status)
        {
            return Clients.Groups("domain:orders", "domain:admin").SendAsync("OrderStatusUpdated", new { orderId, status });
        }

        public Task InventoryUpdated(InventoryDto inventory)
        {
            return Clients.Groups("domain:inventory", $"store:{inventory.StoreId}", "domain:admin").SendAsync("InventoryUpdated", inventory);
        }

        public Task TaskUpdated(TaskDto task)
        {
            return Clients.Groups("domain:tasks", $"user:{task.AssignedTo}", "domain:admin").SendAsync("TaskUpdated", task);
        }
    }
}
