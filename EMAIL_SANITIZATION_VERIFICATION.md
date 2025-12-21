# ✅ Email Sanitization Verification

## 🎯 Vấn đề

Khi login sai, email được sử dụng để tạo Redis key phải được **sanitize** (trim + lowercase) để đảm bảo consistency.

## 🔍 Kiểm tra

### Trước Fix

```csharp
// ❌ SAI: Email không được sanitize khi lưu
attempt = new LoginAttemptDto
{
    Email = email,  // ← Có thể là "User555@Example.com"
    FailedCount = 1,
    LockLevel = 0
};
```

**Vấn đề:**
- Email lưu trong Redis: `"User555@Example.com"`
- Email dùng để tạo key: `"login_attempt:user555@example.com"` (sanitized)
- Mismatch → Inconsistency

### Sau Fix

```csharp
// ✅ ĐÚNG: Email được sanitize trước khi lưu
var sanitizedEmail = email.Trim().ToLower();

attempt = new LoginAttemptDto
{
    Email = sanitizedEmail,  // ← Luôn là "user555@example.com"
    FailedCount = 1,
    LockLevel = 0
};
```

**Kết quả:**
- Email lưu trong Redis: `"user555@example.com"` (sanitized)
- Email dùng để tạo key: `"login_attempt:user555@example.com"` (sanitized)
- ✅ Consistent!

---

## 📋 Sanitization Applied

### 1. GetKey() Method
```csharp
private string? GetKey(string email)
{
    if (string.IsNullOrWhiteSpace(email))
    {
        _logger.LogError("❌ GetKey called with null or empty email");
        return null;
    }

    var key = $"login_attempt:{email.Trim().ToLower()}";  // ✅ Sanitized
    _logger.LogDebug("🔑 Generated login key for email: {Email}", email);
    return key;
}
```

### 2. IncreaseFailedCountAsync() Method
```csharp
public async Task<int> IncreaseFailedCountAsync(string email)
{
    // ... validation ...

    try
    {
        // ✅ Sanitize email
        var sanitizedEmail = email.Trim().ToLower();

        var key = GetKey(email);
        // ... get or create attempt ...

        if (value.IsNullOrEmpty)
        {
            attempt = new LoginAttemptDto
            {
                Email = sanitizedEmail,  // ✅ Use sanitized email
                FailedCount = 1,
                LockLevel = 0
            };
            _logger.LogInformation("🆕 First failed attempt for {Email}", sanitizedEmail);
        }
        else
        {
            attempt = JsonSerializer.Deserialize<LoginAttemptDto>(value.ToString())
                ?? new LoginAttemptDto { Email = sanitizedEmail, FailedCount = 1, LockLevel = 0 };

            attempt.FailedCount++;
            _logger.LogWarning("⚠️ Failed attempt #{Count} for {Email}", attempt.FailedCount, sanitizedEmail);
        }

        // ... rest of method ...
    }
}
```

### 3. IsLockedAsync() Method
```csharp
public async Task<(bool isLocked, string message)> IsLockedAsync(string email)
{
    // ... validation ...

    try
    {
        // ✅ Sanitize email
        var sanitizedEmail = email.Trim().ToLower();

        var key = GetKey(email);
        // ... get attempt from Redis ...

        if (attempt.FailedCount >= MaxFailedAttempts)
        {
            _logger.LogWarning("🚫 Account locked: {Email} | Count: {Count} | Level: {Level} | TTL: {TTL}m",
                sanitizedEmail, attempt.FailedCount, attempt.LockLevel, remaining);  // ✅ Use sanitized email
            return (true, message);
        }
    }
}
```

### 4. ResetAsync() Method
```csharp
public async Task ResetAsync(string email)
{
    // ... validation ...

    try
    {
        // ✅ Sanitize email
        var sanitizedEmail = email.Trim().ToLower();

        var key = GetKey(email);
        // ... delete key ...

        if (deleted)
            _logger.LogInformation("✅ Login attempt counter reset for {Email}", sanitizedEmail);  // ✅ Use sanitized email
    }
}
```

---

## 🧪 Test Scenarios

### Scenario 1: Email with Mixed Case
```
Input: "User555@Example.com"
↓
GetKey(): "login_attempt:user555@example.com"
↓
IncreaseFailedCountAsync():
  - sanitizedEmail = "user555@example.com"
  - Stored in Redis: { Email: "user555@example.com", FailedCount: 1 }
↓
IsLockedAsync():
  - Reads from Redis: { Email: "user555@example.com", FailedCount: 1 }
  - Logs: "Account locked: user555@example.com"
↓
ResetAsync():
  - Logs: "Login attempt counter reset for user555@example.com"
```

### Scenario 2: Email with Spaces
```
Input: "  user555@example.com  "
↓
GetKey(): "login_attempt:user555@example.com"
↓
IncreaseFailedCountAsync():
  - sanitizedEmail = "user555@example.com"
  - Stored in Redis: { Email: "user555@example.com", FailedCount: 1 }
```

### Scenario 3: Multiple Failed Attempts
```
Attempt 1: "User555@Example.com"
  → Key: "login_attempt:user555@example.com"
  → Stored: { Email: "user555@example.com", FailedCount: 1 }

Attempt 2: "user555@example.com"
  → Key: "login_attempt:user555@example.com"
  → Retrieved: { Email: "user555@example.com", FailedCount: 1 }
  → Updated: { Email: "user555@example.com", FailedCount: 2 }

Attempt 3: "USER555@EXAMPLE.COM"
  → Key: "login_attempt:user555@example.com"
  → Retrieved: { Email: "user555@example.com", FailedCount: 2 }
  → Updated: { Email: "user555@example.com", FailedCount: 3 }

✅ All attempts tracked correctly!
```

---

## 📊 Sanitization Rules

| Input | Sanitized | Key |
|-------|-----------|-----|
| `user@example.com` | `user@example.com` | `login_attempt:user@example.com` |
| `User@Example.com` | `user@example.com` | `login_attempt:user@example.com` |
| `USER@EXAMPLE.COM` | `user@example.com` | `login_attempt:user@example.com` |
| `  user@example.com  ` | `user@example.com` | `login_attempt:user@example.com` |
| `User555@Example.com` | `user555@example.com` | `login_attempt:user555@example.com` |

---

## ✨ Benefits

✅ **Consistency** - Email always stored in lowercase + trimmed
✅ **Deduplication** - "User@Example.com" and "user@example.com" treated as same
✅ **Predictability** - Key generation always produces same result
✅ **Logging** - Logs always show sanitized email for clarity
✅ **Security** - Prevents case-sensitivity bypass attempts

---

## 🔍 Verification Checklist

- [x] GetKey() sanitizes email with `.Trim().ToLower()`
- [x] IncreaseFailedCountAsync() stores sanitized email in Redis
- [x] IsLockedAsync() uses sanitized email in logs
- [x] ResetAsync() uses sanitized email in logs
- [x] All methods handle mixed case emails correctly
- [x] All methods handle emails with spaces correctly
- [x] Multiple failed attempts with different cases tracked correctly

