// ================================================================
// FILE: VietCommerce.Api/Services/RedisLoginAttemptService.cs
// VERSION: v2 Hardened (Auto-Reconnect Safe, Fail-Open Mode)
// ================================================================

using Microsoft.Extensions.Logging;
using StackExchange.Redis;
using System.Text.Json;
using VietCommerce.Application.Services.Services.Interfaces;
using VietCommerce.Core.DTOs.Auth;

namespace VietCommerce.Application.Services.Services
{
    public class RedisLoginAttemptService : ILoginAttemptService
    {
        private readonly IDatabase _db;
        private readonly ILogger<RedisLoginAttemptService> _logger;

        private const int MaxFailedAttempts = 5;
        private readonly TimeSpan[] LockDurations = new[]
        {
            TimeSpan.FromMinutes(15),
            TimeSpan.FromMinutes(30),
            TimeSpan.FromHours(1)
        };

        public RedisLoginAttemptService(
            IConnectionMultiplexer redis,
            ILogger<RedisLoginAttemptService> logger)
        {
            _db = redis?.GetDatabase() ?? throw new ArgumentNullException(nameof(redis));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        // ================================
        // Helper: Check Redis connection
        // ================================
        private bool IsRedisConnected()
        {
            try
            {
                return _db.Multiplexer.IsConnected;
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "⚠️ Redis connectivity check failed");
                return false;
            }
        }

        // ================================
        // Helper: Generate safe key
        // ================================
        private string? GetKey(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                _logger.LogWarning("⚠️ GetKey called with null or empty email");
                return null;
            }

            return $"login_attempt:{email.Trim().ToLower()}";
        }

        // ================================
        // Check lock status
        // ================================
        public async Task<(bool isLocked, string message)> IsLockedAsync(string email)
        {
            if (!IsRedisConnected())
            {
                _logger.LogWarning("⚠️ Redis disconnected — skip IsLockedAsync for {Email}", email);
                return (false, string.Empty);
            }

            var key = GetKey(email);
            if (string.IsNullOrEmpty(key))
                return (false, string.Empty);

            try
            {
                var value = await _db.StringGetAsync(key);
                if (value.IsNullOrEmpty)
                {
                    _logger.LogDebug("🔓 No lock record found for {Email}", email);
                    return (false, string.Empty);
                }

                var attempt = JsonSerializer.Deserialize<LoginAttemptDto>(value.ToString());
                if (attempt == null)
                {
                    _logger.LogWarning("⚠️ Invalid attempt data for {Email}", email);
                    return (false, string.Empty);
                }

                if (attempt.FailedCount >= MaxFailedAttempts)
                {
                    var ttl = await _db.KeyTimeToLiveAsync(key) ?? TimeSpan.Zero;
                    var lockLevel = Math.Min(attempt.LockLevel, LockDurations.Length - 1);
                    var remaining = Math.Ceiling(ttl.TotalMinutes);
                    var message = $"Tài khoản tạm thời bị khóa, thử lại sau {remaining} phút.";

                    _logger.LogWarning("🚫 Account locked: {Email} | Count: {Count} | Level: {Level} | TTL: {TTL}m",
                        email, attempt.FailedCount, attempt.LockLevel, remaining);

                    return (true, message);
                }

                return (false, string.Empty);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error checking lock status for {Email}", email);
                return (false, string.Empty);
            }
        }

        // ================================
        // Increase failed count
        // ================================
        public async Task<int> IncreaseFailedCountAsync(string email)
        {
            if (!IsRedisConnected())
            {
                _logger.LogWarning("⚠️ Redis disconnected — skip IncreaseFailedCountAsync for {Email}", email);
                return 0;
            }

            var key = GetKey(email);
            if (string.IsNullOrEmpty(key))
                return 0;

            try
            {
                LoginAttemptDto attempt;
                var value = await _db.StringGetAsync(key);

                if (value.IsNullOrEmpty)
                {
                    attempt = new LoginAttemptDto
                    {
                        Email = email,
                        FailedCount = 1,
                        LockLevel = 0
                    };
                    _logger.LogInformation("🆕 First failed attempt for {Email}", email);
                }
                else
                {
                    attempt = JsonSerializer.Deserialize<LoginAttemptDto>(value.ToString())
                        ?? new LoginAttemptDto { Email = email, FailedCount = 1, LockLevel = 0 };

                    attempt.FailedCount++;
                    _logger.LogWarning("⚠️ Failed attempt #{Count} for {Email}", attempt.FailedCount, email);
                }

                if (attempt.FailedCount >= MaxFailedAttempts && attempt.FailedCount % MaxFailedAttempts == 0)
                {
                    attempt.LockLevel = Math.Min(attempt.LockLevel + 1, LockDurations.Length - 1);
                    _logger.LogWarning("🔒 Lock level increased to {Level} for {Email}", attempt.LockLevel, email);
                }

                var ttl = LockDurations[Math.Min(attempt.LockLevel, LockDurations.Length - 1)];
                var serialized = JsonSerializer.Serialize(attempt);

                await _db.StringSetAsync(key, serialized, ttl);

                _logger.LogInformation("💾 Saved login attempt: {Email} | Count: {Count} | Level: {Level} | TTL: {TTL}m",
                    email, attempt.FailedCount, attempt.LockLevel, ttl.TotalMinutes);

                return attempt.FailedCount;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error increasing failed count for {Email}", email);
                return 0;
            }
        }

        // ================================
        // Reset counter on successful login
        // ================================
        public async Task ResetAsync(string email)
        {
            if (!IsRedisConnected())
            {
                _logger.LogWarning("⚠️ Redis disconnected — skip ResetAsync for {Email}", email);
                return;
            }

            var key = GetKey(email);
            if (string.IsNullOrEmpty(key))
            {
                _logger.LogWarning("⚠️ ResetAsync skipped due to null key for {Email}", email);
                return;
            }

            try
            {
                var deleted = await _db.KeyDeleteAsync(key);

                if (deleted)
                    _logger.LogInformation("✅ Login attempt counter reset for {Email}", email);
                else
                    _logger.LogDebug("ℹ️ No counter to reset for {Email}", email);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error resetting counter for {Email}", email);
            }
        }
    }
}
