using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Threading.Tasks;
using VietCommerce.Api.Services.Interfaces;
using VietCommerce.Core.Models;

namespace VietCommerce.Api.Services
{
    /// <summary>
    /// Base class cho tất cả các service trong hệ thống
    /// Cung cấp các chức năng chung: logging, exception handling, caching, validation
    /// Tích hợp với RedisCacheService và ApiResponse pattern
    /// 
    /// FEATURES:
    /// ✅ Automatic exception handling với ApiResponse pattern
    /// ✅ Redis caching với TTL và invalidation strategies
    /// ✅ Standardized logging với emoji icons
    /// ✅ Reusable validation helpers
    /// ✅ Pub/Sub support cho distributed cache invalidation
    /// ❌ NO Monitoring/Metrics (keep it simple - add only when needed)
    /// 
    /// VERSION: 2.0
    /// LAST UPDATED: 2025-01-30
    /// </summary>
    public abstract class BaseService
    {
        protected readonly ILogger _logger;
        protected readonly ICacheService _cacheService;

        protected BaseService(ILogger logger, ICacheService? cacheService = null)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _cacheService = cacheService;
        }

        #region Logging Methods
        protected void LogInfo(string message, params object[] args)
            => _logger.LogInformation(message, args);

        protected void LogWarning(string message, params object[] args)
            => _logger.LogWarning(message, args);

        protected void LogError(string message, Exception? ex = null, params object[] args)
        {
            if (ex != null)
                _logger.LogError(ex, message, args);
            else
                _logger.LogError(message, args);
        }

        protected void LogDebug(string message, params object[] args)
            => _logger.LogDebug(message, args);
        #endregion

        #region Exception Handling - ApiResponse Pattern
        protected async Task<ApiResponse<T>> ExecuteAsApiResponseAsync<T>(
            Func<Task<T>> action,
            string operationName,
            string successMessage = "Operation successful")
        {
            try
            {
                LogDebug($"🔹 Bắt đầu: {operationName}");
                var result = await action();
                LogDebug($"✅ Thành công: {operationName}");
                return ApiResponse<T>.SuccessResponse(result, successMessage);
            }
            catch (ArgumentException ex)
            {
                LogWarning($"⚠️ Tham số không hợp lệ ({operationName}): {ex.Message}");
                return ApiResponse<T>.FailureResponse(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                LogWarning($"⚠️ Lỗi logic ({operationName}): {ex.Message}");
                return ApiResponse<T>.FailureResponse(ex.Message);
            }
            catch (KeyNotFoundException ex)
            {
                LogWarning($"⚠️ Không tìm thấy ({operationName}): {ex.Message}");
                return ApiResponse<T>.FailureResponse(ex.Message);
            }
            catch (Exception ex)
            {
                LogError($"❌ Lỗi hệ thống ({operationName}): {ex.Message}", ex);
                return ApiResponse<T>.FailureResponse($"An error occurred during {operationName}");
            }
        }

        protected async Task<T> ExecuteAsync<T>(Func<Task<T>> action, string operationName)
        {
            try
            {
                LogDebug($"🔹 Bắt đầu: {operationName}");
                var result = await action();
                LogDebug($"✅ Thành công: {operationName}");
                return result;
            }
            catch (Exception ex)
            {
                LogError($"❌ Lỗi khi thực hiện {operationName}: {ex.Message}", ex);
                throw;
            }
        }

        protected async Task ExecuteAsync(Func<Task> action, string operationName)
        {
            try
            {
                LogDebug($"🔹 Bắt đầu: {operationName}");
                await action();
                LogDebug($"✅ Thành công: {operationName}");
            }
            catch (Exception ex)
            {
                LogError($"❌ Lỗi khi thực hiện {operationName}: {ex.Message}", ex);
                throw;
            }
        }

        protected T Execute<T>(Func<T> action, string operationName)
        {
            try
            {
                LogDebug($"🔹 Bắt đầu: {operationName}");
                var result = action();
                LogDebug($"✅ Thành công: {operationName}");
                return result;
            }
            catch (Exception ex)
            {
                LogError($"❌ Lỗi khi thực hiện {operationName}: {ex.Message}", ex);
                throw;
            }
        }
        #endregion

        #region Cache Methods - Redis Integration
        protected string CreateCacheKey(string prefix, params object[] parts)
            => $"{prefix}:{string.Join(":", parts)}";

        protected async Task<T> GetFromCacheOrExecuteAsync<T>(
    string cacheKey,
    Func<Task<T>> action,
    TimeSpan? cacheDuration = null)
        {
            if (_cacheService == null)
                return await action();

            try
            {
                // ✅ 1. Cache hit
                var cached = await _cacheService.GetAsync<T>(cacheKey);
                if (!EqualityComparer<T>.Default.Equals(cached, default))
                {
                    LogDebug($"📦 Cache HIT: {cacheKey}");
                    return cached!;
                }

                // ✅ 2. Cache miss → execute
                LogDebug($"🔄 Cache MISS: {cacheKey}");
                var data = await action();

                // ✅ 3. Save to cache if valid
                if (!EqualityComparer<T>.Default.Equals(data, default))
                {
                    var ttl = cacheDuration ?? TimeSpan.FromMinutes(30);
                    await _cacheService.SetAsync(cacheKey, data, ttl);
                    LogDebug($"💾 Cached {cacheKey} (TTL: {ttl.TotalSeconds}s)");
                }

                return data;
            }
            catch (Exception ex)
            {
                LogWarning($"⚠️ Cache lỗi {cacheKey}, fallback: {ex.Message}");
                return await action();
            }
        }



        protected async Task InvalidateCacheAsync(string key)
        {
            if (_cacheService == null) return;
            try
            {
                await _cacheService.RemoveAsync(key);
                LogDebug($"🗑️ Cache removed: {key}");
            }
            catch (Exception ex)
            {
                LogWarning($"⚠️ Lỗi khi xóa cache {key}: {ex.Message}");
            }
        }

        protected async Task InvalidateMultipleCachesAsync(params string[] keys)
        {
            if (keys == null || !keys.Any()) return;
            await Task.WhenAll(keys.Select(InvalidateCacheAsync));
            LogDebug($"🗑️ Đã xóa {keys.Length} cache keys");
        }

        protected async Task InvalidateCacheByPrefixAsync(string pattern)
        {
            if (_cacheService == null) return;
            try
            {
                LogDebug($"🗑️ Xóa cache pattern: {pattern}");
                await _cacheService.RemoveByPatternAsync(pattern);
            }
            catch (Exception ex)
            {
                LogWarning($"⚠️ Lỗi khi xóa cache pattern {pattern}: {ex.Message}");
            }
        }
        #endregion

        #region Pub/Sub Methods
        protected async Task PublishCacheInvalidationAsync<T>(string channel, T message)
            where T : class
        {
            if (_cacheService == null) return;
            try
            {
                await _cacheService.PublishAsync(channel, message);
                LogDebug($"📡 Published to {channel}");
            }
            catch (Exception ex)
            {
                LogWarning($"⚠️ Publish lỗi {channel}: {ex.Message}");
            }
        }

        protected async Task SubscribeCacheInvalidationAsync<T>(string channel, Action<T> handler)
            where T : class
        {
            if (_cacheService == null) return;
            try
            {
                await _cacheService.SubscribeAsync(channel, handler);
                LogInfo($"🔔 Subscribed to {channel}");
            }
            catch (Exception ex)
            {
                LogWarning($"⚠️ Subscribe lỗi {channel}: {ex.Message}");
            }
        }
        #endregion

        #region Validation Methods
        protected void ValidateNotNull<T>(T obj, string name) where T : class
        {
            if (obj == null) throw new ArgumentNullException(name, $"{name} không được null");
        }

        protected void ValidateNotEmpty(string value, string name)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException($"{name} không được rỗng", name);
        }

        protected void ValidateId(int id, string name = "id")
        {
            if (id <= 0)
                throw new ArgumentException($"{name} phải > 0", name);
        }

        protected void ValidateId(Guid id, string name = "id")
        {
            if (id == Guid.Empty)
                throw new ArgumentException($"{name} không được rỗng", name);
        }

        protected void ValidateEmail(string email)
        {
            ValidateNotEmpty(email, nameof(email));
            var addr = new MailAddress(email);
            if (addr.Address != email)
                throw new ArgumentException("Email không hợp lệ", nameof(email));
        }

        protected void ValidateNotEmpty<T>(IEnumerable<T> list, string name)
        {
            if (list == null || !list.Any())
                throw new ArgumentException($"{name} không được rỗng", name);
        }
        #endregion

        #region Utility Methods
        protected void ThrowIf(bool condition, string message)
        {
            if (condition)
                throw new InvalidOperationException(message);
        }

        protected void ThrowIfNot(bool condition, string message)
        {
            if (!condition)
                throw new InvalidOperationException(message);
        }
        #endregion
    }
}
