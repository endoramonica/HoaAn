// File: VietCommerce.Application/Services/Services/NotificationService.cs
using AutoMapper;
using Microsoft.Extensions.Logging;
using System.Linq.Expressions;
using VietCommerce.Application.Extensions;
using VietCommerce.Application.Services.Services.Interfaces;
using VietCommerce.Core.DTOs;
using VietCommerce.Core.DTOs.Notifications;
using VietCommerce.Core.Entities.Notifications;
using VietCommerce.Core.Models;
using VietCommerce.Data.Repositories.Interfaces;

namespace VietCommerce.Application.Services.Services;

public class NotificationService : BaseService, INotificationService
{
    private readonly IGenericRepository<Notification> _notificationRepo;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public NotificationService(
        ILogger<NotificationService> logger,
        IUnitOfWork unitOfWork,
        IMapper mapper,
        ICacheService? cacheService = null) : base(logger, cacheService)
    {
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        _notificationRepo = unitOfWork.Notifications;
    }

    public async Task<PaginatedResult<NotificationDto>> GetNotificationsAsync(Guid userId, PaginationParams pagination)
    {
        return await ExecuteAsync(async () =>
        {
            ValidateId(userId, nameof(userId));

            var cacheKey = CreateCacheKey(
                "notifications",
                userId,
                pagination.Page,
                pagination.PageSize,
                pagination.SortBy ?? "createdat",
                pagination.SortDescending ? "desc" : "asc");

            return await GetFromCacheOrExecuteAsync(cacheKey, async () =>
            {
                Expression<Func<Notification, bool>> predicate = n => n.UserId == userId;

                Expression<Func<Notification, object>> orderBy = pagination.SortBy?.ToLower() switch
                {
                    "read" => n => n.IsRead,
                    "type" => n => n.Type!,
                    "title" => n => n.Title!,
                    _ => n => n.CreatedAt
                };

                var (entities, total) = await _notificationRepo.GetPagedAsync(
                    pageNumber: pagination.Page,
                    pageSize: pagination.PageSize,
                    predicate: predicate,
                    orderBy: orderBy,
                    ascending: !pagination.SortDescending);

                var dtos = _mapper.Map<IEnumerable<NotificationDto>>(entities);

                return new PaginatedResult<NotificationDto>(
                    items: dtos,
                    pageNumber: pagination.Page,
                    pageSize: pagination.PageSize,
                    totalItems: total);

            }, TimeSpan.FromMinutes(5));

        }, $"GetNotificationsAsync - UserId: {userId}, Page: {pagination.Page}");
    }

    public async Task<NotificationDto?> GetNotificationByIdAsync(Guid id)
    {
        return await ExecuteAsync(async () =>
        {
            ValidateId(id);
            var entity = await _notificationRepo.GetByIdAsync(id);
            return entity == null ? null : _mapper.Map<NotificationDto>(entity);
        }, $"GetNotificationByIdAsync - Id: {id}");
    }

    public async Task MarkNotificationAsReadAsync(Guid id)
    {
        await ExecuteAsync(async () =>
        {
            ValidateId(id);

            var notification = await _notificationRepo.GetByIdAsync(id)
                ?? throw new KeyNotFoundException($"Notification {id} not found");

            // Optional: Check ownership
            // if (notification.UserId != currentUserId) throw new UnauthorizedAccessException();

            if (!notification.IsRead)
            {
                notification.IsRead = true;
                _notificationRepo.Update(notification);
                await _unitOfWork.SaveChangesAsync();
            }

            // Lấy userId an toàn
            var userId = notification.UserId
                ?? throw new InvalidOperationException("Notification does not have a UserId");

            // Chỉ gọi 1 lần
            await InvalidateUserNotificationCacheAsync(userId);

        }, $"MarkNotificationAsReadAsync - Id: {id}");
    }


    public async Task MarkAllNotificationsAsReadAsync(Guid userId)
    {
        await ExecuteAsync(async () =>
        {
            ValidateId(userId);

            var unread = await _notificationRepo.FindAsync(n => n.UserId == userId && !n.IsRead);
            if (unread.Any())
            {
                foreach (var n in unread)
                {
                    n.IsRead = true;
                    _notificationRepo.Update(n);
                }
                await _unitOfWork.SaveChangesAsync();
            }

            await InvalidateUserNotificationCacheAsync(userId);

        }, $"MarkAllNotificationsAsReadAsync - UserId: {userId}");
    }

    public async Task DeleteNotificationAsync(Guid id)
    {
        await ExecuteAsync(async () =>
        {
            ValidateId(id);

            var notification = await _notificationRepo.GetByIdAsync(id)
                ?? throw new KeyNotFoundException($"Notification {id} not found");

            _notificationRepo.Delete(notification);
            await _unitOfWork.SaveChangesAsync();
            // Lấy userId an toàn
            var userId = notification.UserId
                ?? throw new InvalidOperationException("Notification does not have a UserId");
            await InvalidateUserNotificationCacheAsync(notification.UserId.Value);

        }, $"DeleteNotificationAsync - Id: {id}");
    }

    public async Task<int> GetUnreadNotificationCountAsync(Guid userId)
    {
        return await ExecuteAsync(async () =>
        {
            ValidateId(userId);

            var cacheKey = CreateCacheKey("unread-count", userId);

            return await GetFromCacheOrExecuteAsync(cacheKey, async () =>
            {
                return await _notificationRepo.CountAsync(n => n.UserId == userId && !n.IsRead);
            }, TimeSpan.FromMinutes(2)); // cache 2 phút

        }, $"GetUnreadNotificationCountAsync - UserId: {userId}");
    }

    // Helper: Xóa toàn bộ cache liên quan đến user
    private async Task InvalidateUserNotificationCacheAsync(Guid userId)
    {
        await InvalidateCacheByPrefixAsync($"notifications:{userId}:");
        await InvalidateCacheByPrefixAsync($"unread-count:{userId}");
        LogInfo($"Invalidated notification cache for UserId: {userId}");
    }
}