using System;
namespace VietCommerce.Application.Services.Services.Interfaces;
public interface IProductStatsService
{
    /// Track view - gọi khi user xem product detail
    /// Realtime: Insert vào ProductView table
    /// Batch: Background job cập nhật ViewCount
    Task TrackViewAsync(Guid productId, Guid? userId, string sessionId, string? ipAddress = null, string? userAgent = null);
    /// Toggle favorite - Realtime update FavoriteCount
    Task<bool> ToggleFavoriteAsync(Guid productId, Guid userId);
    /// Update purchase count - gọi khi Order.Status = COMPLETED
    Task UpdatePurchaseCountAsync(Guid productId, int quantity);
    /// Update review stats - gọi khi có review mới/update/delete
    Task UpdateReviewStatsAsync(Guid productId);
    /// Calculate trending score - Background job chạy hàng ngày/giờ
    /// Formula: (ViewCount * 0.1) + (FavoriteCount * 0.3) + (PurchaseCount * 0.6)
    /// Chỉ tính trong 7 ngày gần nhất
    Task UpdateTrendingScoresAsync(int days = 7);
    /// Batch update view counts từ ProductView table
    Task BatchUpdateViewCountsAsync();  
}
