namespace VietCommerce.Api.Services.Interfaces;
/// Distributed cache service using Redis for:
/// - Permission caching
/// - Session storage
/// - Pub/Sub invalidation
/// </summary>
public interface ICacheService
{
    // ============ BASIC CACHE OPERATIONS ============
    /// Get cached value by key
    Task<T?> GetAsync<T>(string key) where T : class;
    /// Set cache with expiration
    Task SetAsync<T>(string key, T value, TimeSpan expiration) where T : class;
    /// Remove cache by key
    Task RemoveAsync(string key);
    /// Check if key exists
    Task<bool> ExistsAsync(string key);
    // ============ BATCH OPERATIONS ============
    /// Remove multiple keys matching pattern (e.g., "permissions:user:*")
    Task RemoveByPatternAsync(string pattern);
    // ============ PUB/SUB ============
    /// Publish message to Redis channel
    Task PublishAsync<T>(string channel, T message) where T : class;
    /// Subscribe to Redis channel
    Task SubscribeAsync<T>(string channel, Action<T> handler) where T : class;
}
