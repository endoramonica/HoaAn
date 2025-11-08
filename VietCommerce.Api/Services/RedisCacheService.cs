using StackExchange.Redis;
using System.Text.Json;
using System.Text.Json.Serialization;
using VietCommerce.Api.Services.Interfaces;

namespace VietCommerce.Api.Services
{
    /// <summary>
    /// Redis-based distributed cache implementation.
    /// Uses StackExchange.Redis for high-performance caching.
    /// </summary>
    /// <remarks>
    /// Key design decisions:
    /// - Prefix all keys (vietcommerce:...) to avoid collisions
    /// - Log cache HIT/MISS for monitoring
    /// - Graceful error handling (doesn't crash if Redis is down)
    /// - Use SCAN instead of KEYS (production-safe)
    /// </remarks>
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
        private static readonly JsonSerializerOptions _jsonOptions = new()
        {
            ReferenceHandler = ReferenceHandler.IgnoreCycles, // 👈 Bỏ qua vòng lặp
            WriteIndented = false,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        // ============ BASIC OPERATIONS ============

        public async Task<T?> GetAsync<T>(string key)
        {
            try
            {
                var fullKey = GetFullKey(key);
                var value = await _db.StringGetAsync(fullKey);

                if (value.IsNullOrEmpty)
                {
                    _logger.LogDebug("Cache MISS: {Key}", fullKey);
                    return default;
                }

                _logger.LogDebug("Cache HIT: {Key}", fullKey);

                // Handle primitive/value types
                if (typeof(T).IsPrimitive || typeof(T) == typeof(string) || typeof(T) == typeof(decimal))
                {
                    return (T)Convert.ChangeType(value.ToString(), typeof(T));
                }
                return JsonSerializer.Deserialize<T>(value!, _jsonOptions);


            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting cache for key {Key}", key);
                return default;
            }
        }

        public async Task SetAsync<T>(string key, T value, TimeSpan expiration)
        {
            try
            {
                var fullKey = GetFullKey(key);
                string serialized;

                if (value is string || value?.GetType().IsPrimitive == true || value is decimal)
                    serialized = value?.ToString() ?? string.Empty;
                else
                    serialized = JsonSerializer.Serialize(value, _jsonOptions);

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

        public async Task<(bool Found, T? Value)> TryGetValueAsync<T>(string key)
        {
            try
            {
                var fullKey = GetFullKey(key);
                var value = await _db.StringGetAsync(fullKey);

                if (value.IsNullOrEmpty)
                {
                    _logger.LogDebug("Cache MISS: {Key}", fullKey);
                    return (false, default);
                }

                _logger.LogDebug("Cache HIT: {Key}", fullKey);

                T? deserialized;
                if (typeof(T).IsPrimitive || typeof(T) == typeof(string) || typeof(T) == typeof(decimal))
                {
                    deserialized = (T)Convert.ChangeType(value.ToString(), typeof(T));
                }
                else
                {
                    deserialized = JsonSerializer.Deserialize<T>(value!, _jsonOptions);
                }

                return (true, deserialized);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error trying to get cache for key {Key}", key);
                return (false, default);
            }
        }

        // ============ BATCH OPERATIONS ============

        public async Task RemoveByPatternAsync(string pattern)
        {
            try
            {
                var fullPattern = GetFullKey(pattern);
                var server = _redis.GetServer(_redis.GetEndPoints().First());

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

        public async Task PublishAsync<T>(string channel, T message)
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

        public async Task SubscribeAsync<T>(string channel, Action<T> handler)
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
}
