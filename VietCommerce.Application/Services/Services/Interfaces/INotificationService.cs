// File: VietCommerce.Application/Services/Services/Interfaces/INotificationService.cs
using VietCommerce.Core.DTOs;
using VietCommerce.Core.DTOs.Notifications;
using VietCommerce.Core.Models;

namespace VietCommerce.Application.Services.Services.Interfaces;

public interface INotificationService
{
    /// <summary>
    /// Lấy danh sách thông báo của user (phân trang, sắp xếp)
    /// </summary>
    Task<PaginatedResult<NotificationDto>> GetNotificationsAsync(
        Guid userId,
        PaginationParams pagination);

    /// <summary>
    /// Lấy chi tiết 1 thông báo theo Id
    /// </summary>
    Task<NotificationDto?> GetNotificationByIdAsync(Guid id);

    /// <summary>
    /// Đánh dấu 1 thông báo đã đọc
    /// </summary>
    Task MarkNotificationAsReadAsync(Guid id);

    /// <summary>
    /// Đánh dấu tất cả thông báo của user đã đọc
    /// </summary>
    Task MarkAllNotificationsAsReadAsync(Guid userId);

    /// <summary>
    /// Xóa thông báo (hard delete hoặc soft delete tùy config sau)
    /// </summary>
    Task DeleteNotificationAsync(Guid id);

    /// <summary>
    /// Đếm số thông báo chưa đọc của user (có cache)
    /// </summary>
    Task<int> GetUnreadNotificationCountAsync(Guid userId);
}