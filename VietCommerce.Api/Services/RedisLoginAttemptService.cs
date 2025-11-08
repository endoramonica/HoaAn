// ================================================================
// FILE: VietCommerce.Api/Services/RedisLoginAttemptService.cs
// FIXED: Validation + Error Handling
// ================================================================

using StackExchange.Redis;
using System.Text.Json;
using VietCommerce.Api.Services.Interfaces;
using VietCommerce.Core.DTOs.Auth;

namespace VietCommerce.Api.Services
{
    public class RedisLoginAttemptService : ILoginAttemptService
    {
        private readonly IDatabase _db;
        private readonly ILogger<RedisLoginAttemptService> _logger;

        // Cấu hình
        private const int MaxFailedAttempts = 5;  // lần fail để khóa
        private readonly TimeSpan[] LockDurations = new[]
        {
            TimeSpan.FromMinutes(15), // lần 5-9 (LockLevel 0)
            TimeSpan.FromMinutes(30), // lần 10-14 (LockLevel 1)
            TimeSpan.FromHours(1),    // lần 15+ (LockLevel 2)
        };

        public RedisLoginAttemptService(
            IConnectionMultiplexer redis,
            ILogger<RedisLoginAttemptService> logger)
        {
            _db = redis?.GetDatabase() ?? throw new ArgumentNullException(nameof(redis));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        private string GetKey(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                throw new ArgumentException("Email cannot be null or empty", nameof(email));
            }
            return $"login_attempt:{email.ToLower().Trim()}";
        }

        /// <summary>
        /// Kiểm tra xem tài khoản có đang bị khóa không
        /// </summary>
        public async Task<(bool isLocked, string message)> IsLockedAsync(string email)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(email))
                {
                    _logger.LogWarning("IsLockedAsync: Email is null or empty");
                    return (false, string.Empty);
                }

                var key = GetKey(email);
                var value = await _db.StringGetAsync(key);

                if (value.IsNullOrEmpty)
                {
                    _logger.LogDebug("🔓 No lock record found for: {Email}", email);
                    return (false, string.Empty);
                }

                var attempt = JsonSerializer.Deserialize<LoginAttemptDto>(value.ToString());
                if (attempt == null)
                {
                    _logger.LogWarning("⚠️ Failed to deserialize login attempt for: {Email}", email);
                    return (false, string.Empty);
                }

                // Kiểm tra có bị khóa không
                if (attempt.FailedCount >= MaxFailedAttempts)
                {
                    var ttl = await _db.KeyTimeToLiveAsync(key) ?? TimeSpan.Zero;
                    var lockLevel = Math.Min(attempt.LockLevel, LockDurations.Length - 1);
                    var lockDuration = LockDurations[lockLevel];

                    var remainingMinutes = Math.Ceiling(ttl.TotalMinutes);

                    _logger.LogWarning("🚫 Account locked: {Email} | FailedCount: {Count} | LockLevel: {Level} | Remaining: {Minutes}m",
                        email, attempt.FailedCount, attempt.LockLevel, remainingMinutes);

                    var message = $"Tài khoản tạm thời bị khóa, thử lại sau {remainingMinutes} phút.";
                    return (true, message);
                }

                _logger.LogDebug("🔓 Account not locked: {Email} | FailedCount: {Count}", email, attempt.FailedCount);
                return (false, string.Empty);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error checking lock status for: {Email}", email);
                // Trong trường hợp lỗi, không khóa user (fail open)
                return (false, string.Empty);
            }
        }

        /// <summary>
        /// Tăng số lần đăng nhập thất bại
        /// </summary>
        public async Task<int> IncreaseFailedCountAsync(string email)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(email))
                {
                    _logger.LogWarning("IncreaseFailedCountAsync: Email is null or empty");
                    return 0;
                }

                var key = GetKey(email);
                LoginAttemptDto attempt;

                var value = await _db.StringGetAsync(key);

                if (value.IsNullOrEmpty)
                {
                    // Lần đầu tiên fail
                    attempt = new LoginAttemptDto
                    {
                        Email = email,
                        FailedCount = 1,
                        LockLevel = 0
                    };
                    _logger.LogInformation("🆕 First failed attempt for: {Email}", email);
                }
                else
                {
                    // Tăng failed count
                    attempt = JsonSerializer.Deserialize<LoginAttemptDto>(value.ToString())
                        ?? new LoginAttemptDto { Email = email, FailedCount = 1, LockLevel = 0 };

                    attempt.FailedCount++;
                    _logger.LogWarning("⚠️ Failed attempt #{Count} for: {Email}", attempt.FailedCount, email);
                }

                // Nếu vượt MaxFailedAttempts -> tăng LockLevel
                if (attempt.FailedCount >= MaxFailedAttempts && attempt.FailedCount % MaxFailedAttempts == 0)
                {
                    attempt.LockLevel = Math.Min(attempt.LockLevel + 1, LockDurations.Length - 1);
                    _logger.LogWarning("🔒 Lock level increased to {Level} for: {Email}", attempt.LockLevel, email);
                }

                // TTL dựa theo LockLevel
                var ttlIndex = Math.Min(attempt.LockLevel, LockDurations.Length - 1);
                var ttl = LockDurations[ttlIndex];

                var serialized = JsonSerializer.Serialize(attempt);
                await _db.StringSetAsync(key, serialized, ttl);

                _logger.LogInformation("💾 Saved login attempt: {Email} | Count: {Count} | Level: {Level} | TTL: {TTL}m",
                    email, attempt.FailedCount, attempt.LockLevel, ttl.TotalMinutes);

                return attempt.FailedCount;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error increasing failed count for: {Email}", email);
                return 0;
            }
        }

        /// <summary>
        /// Reset counter khi login thành công
        /// </summary>
        public async Task ResetAsync(string email)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(email))
                {
                    _logger.LogWarning("ResetAsync: Email is null or empty");
                    return;
                }

                var key = GetKey(email);
                var deleted = await _db.KeyDeleteAsync(key);

                if (deleted)
                {
                    _logger.LogInformation("✅ Login attempt counter reset for: {Email}", email);
                }
                else
                {
                    _logger.LogDebug("ℹ️ No counter to reset for: {Email}", email);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error resetting counter for: {Email}", email);
            }
        }
    }

}