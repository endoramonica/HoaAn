using Microsoft.EntityFrameworkCore.Metadata.Internal;
using StackExchange.Redis;
using System;
using System.Text.Json;
using VietCommerce.Api.Services.Interfaces;
namespace VietCommerce.Api.Services;
/// Redis-based distributed cache implementation
/// Uses StackExchange.Redis for high-performance caching
//KEY DESIGN DECISIONS:
//? Prefix all keys(vietcommerce:...) d? tránh collision
//? Log cache HIT/MISS cho monitoring
//? Graceful error handling(không crash app n?u Redis down)
//? SCAN thay vì KEYS(production-safe)
public class RedisCacheService : ICacheService
{
    private readonly IConnectionMultiplexer _redis;
    private readonly IDatabase _db;
    private readonly ILogger<RedisCacheService> _logger;
    private readonly string _keyPrefix;
    public RedisCacheService(
        IConnectionMultiplexer redis,
        IConfiguration configuration,
        ILogger<RedisCacheService> logger)
    {
        _redis = redis;
        _db = redis.GetDatabase();
        _logger = logger;
        _keyPrefix = configuration["CacheSettings:KeyPrefix"] ?? "vietcommerce";
    }
    // ============ BASIC OPERATIONS ============
    public async Task<T?> GetAsync<T>(string key) where T : class
    {
        try
        {
            var fullKey = GetFullKey(key);
            var value = await _db.StringGetAsync(fullKey);
            if (value.IsNullOrEmpty)
            {
                _logger.LogDebug("Cache MISS: {Key}", fullKey);
                return null;
            }
            _logger.LogDebug("Cache HIT: {Key}", fullKey);
            return JsonSerializer.Deserialize<T>(value!);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting cache for key {Key}", key);
            return null;
        }
    }
    public async Task SetAsync<T>(string key, T value, TimeSpan expiration) where T : class
    {
        try
        {
            var fullKey = GetFullKey(key);
            var serialized = JsonSerializer.Serialize(value);
            await _db.StringSetAsync(fullKey, serialized, expiration);
            _logger.LogDebug("Cache SET: {Key}, TTL: {Expiration}s", fullKey, expiration.TotalSeconds);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error setting cache for key {Key}", key);
        }
    }
    public async Task RemoveAsync(string key)
    {
        try
        {
            var fullKey = GetFullKey(key);
            await _db.KeyDeleteAsync(fullKey);
            _logger.LogDebug("Cache REMOVE: {Key}", fullKey);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error removing cache for key {Key}", key);
        }
    }
    public async Task<bool> ExistsAsync(string key)
    {
        try
        {
            var fullKey = GetFullKey(key);
            return await _db.KeyExistsAsync(fullKey);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking existence for key {Key}", key);
            return false;
        }
    }
    // ============ BATCH OPERATIONS ============
    public async Task RemoveByPatternAsync(string pattern)
    {
        try
        {
            var fullPattern = GetFullKey(pattern);
            var server = _redis.GetServer(_redis.GetEndPoints().First());
            // SCAN all keys matching pattern (production-safe, non-blocking)
            var keys = server.Keys(pattern: fullPattern).ToArray();
            if (keys.Length > 0)
            {
                await _db.KeyDeleteAsync(keys);
                _logger.LogInformation("Cache REMOVE PATTERN: {Pattern}, Count: {Count}", fullPattern, keys.Length);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error removing cache by pattern {Pattern}", pattern);
        }
    }
    // ============ PUB/SUB ============
    public async Task PublishAsync<T>(string channel, T message) where T : class
    {
        try
        {
            var subscriber = _redis.GetSubscriber();
            var serialized = JsonSerializer.Serialize(message);
            await subscriber.PublishAsync(channel, serialized);
            _logger.LogDebug("Published to channel {Channel}: {Message}", channel, serialized);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error publishing to channel {Channel}", channel);
        }
    }
    public async Task SubscribeAsync<T>(string channel, Action<T> handler) where T : class
    {
        try
        {
            var subscriber = _redis.GetSubscriber();
            await subscriber.SubscribeAsync(channel, (ch, message) =>
            {
                try
                {
                    var deserialized = JsonSerializer.Deserialize<T>(message!);
                    if (deserialized != null)
                    {
                        handler(deserialized);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error handling message from channel {Channel}", channel);
                }
            });
            _logger.LogInformation("Subscribed to channel: {Channel}", channel);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error subscribing to channel {Channel}", channel);
        }
    }
    // ============ HELPERS ============
    private string GetFullKey(string key)
    {
        return $"{_keyPrefix}:{key}";
    }
}
