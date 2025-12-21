# 🔍 Debug Guide - Login Stateful Bug

## 🎯 Vấn đề

Exception: `ArgumentNullException: Value cannot be null. (Parameter 'key')`

Xảy ra khi:
1. Login sai 1-n lần
2. Login đúng lần tiếp theo → Exception

## 🔴 Root Cause (Đã Fix)

### Vị trí lỗi
**File:** `VietCommerce.Application/Services/Services/RedisLoginAttemptService.cs`

**Method:** `GetKey(string email)`

```csharp
// ❌ SAI: Trả về null
private string? GetKey(string email)
{
    if (string.IsNullOrWhiteSpace(email))
    {
        _logger.LogWarning(...);
        return null;  // ← PROBLEM!
    }
    return $"login_attempt:{email.Trim().ToLower()}";
}
```

### Chuỗi sự kiện gây lỗi

```
1. IncreaseFailedCountAsync(email) được gọi
   ↓
2. var key = GetKey(email);  // Nếu email null → key = null
   ↓
3. if (string.IsNullOrEmpty(key)) return 0;  // ← Không check này!
   ↓
4. await _db.StringSetAsync(key, serialized, ttl);
   ↓
5. StackExchange.Redis throws: ArgumentNullException (Parameter 'key')
```

## ✅ Fix Applied

### 1. **GetKey() - Throw Exception Instead of Returning Null**

```csharp
// ✅ ĐÚNG: Throw exception nếu email invalid
private string GetKey(string email)
{
    if (string.IsNullOrWhiteSpace(email))
    {
        _logger.LogError("❌ GetKey called with null or empty email");
        throw new ArgumentException("Email cannot be null or empty", nameof(email));
    }

    var key = $"login_attempt:{email.Trim().ToLower()}";
    _logger.LogDebug("🔑 Generated login key for email: {Email}", email);
    return key;
}
```

### 2. **IsLockedAsync() - Add Null Check**

```csharp
var key = GetKey(email);
if (string.IsNullOrEmpty(key))
{
    _logger.LogError("❌ Failed to generate key for email: {Email}", email);
    return (false, string.Empty);
}
```

### 3. **IncreaseFailedCountAsync() - Add Null Check**

```csharp
var key = GetKey(email);
if (string.IsNullOrEmpty(key))
{
    _logger.LogError("❌ Failed to generate key for email: {Email}", email);
    return 0;
}
```

### 4. **ResetAsync() - Add Null Check**

```csharp
var key = GetKey(email);
if (string.IsNullOrEmpty(key))
{
    _logger.LogError("❌ Failed to generate key for email: {Email}", email);
    return;
}
```

---

## 🧪 Testing Scenarios

### Scenario 1: Normal Login Flow
```
1. Login sai (email/password) → Failed count = 1
2. Login sai (email/password) → Failed count = 2
3. Login sai (email/password) → Failed count = 3
4. Login đúng → ✅ SUCCESS (failed count reset)
```

**Expected Logs:**
```
🆕 First failed attempt for user@example.com
💾 Saved login attempt: user@example.com | Count: 1 | Level: 0 | TTL: 15m
⚠️ Failed attempt #2 for user@example.com
💾 Saved login attempt: user@example.com | Count: 2 | Level: 0 | TTL: 15m
⚠️ Failed attempt #3 for user@example.com
💾 Saved login attempt: user@example.com | Count: 3 | Level: 0 | TTL: 15m
✅ Login attempt counter reset for user@example.com
✅ Login successful | UserId: ...
```

### Scenario 2: Account Locked
```
1-5. Login sai → Failed count = 5 → Account locked
6. Login đúng (nhưng account locked) → ❌ BLOCKED
7. Wait 15 minutes → Account unlocked
8. Login đúng → ✅ SUCCESS
```

**Expected Logs:**
```
🚫 Account locked: user@example.com | Count: 5 | Level: 0 | TTL: 15m
🚫 Login blocked - Account locked: user@example.com
```

---

## 🐛 Debugging Breakpoints

### Breakpoint 1: Check Email Validation
**File:** `RedisLoginAttemptService.cs`
**Line:** `GetKey()` method

```csharp
private string GetKey(string email)
{
    if (string.IsNullOrWhiteSpace(email))  // ← BREAKPOINT HERE
    {
        _logger.LogError("❌ GetKey called with null or empty email");
        throw new ArgumentException("Email cannot be null or empty", nameof(email));
    }
    ...
}
```

**Check:**
- Is `email` null or empty?
- What's the actual value?

### Breakpoint 2: Check Key Generation
**File:** `RedisLoginAttemptService.cs`
**Line:** After `GetKey()` call

```csharp
var key = GetKey(email);  // ← BREAKPOINT HERE
if (string.IsNullOrEmpty(key))
{
    _logger.LogError("❌ Failed to generate key for email: {Email}", email);
    return 0;
}
```

**Check:**
- Is `key` null or empty?
- What's the actual key value?

### Breakpoint 3: Check Redis Operation
**File:** `RedisLoginAttemptService.cs`
**Line:** Before `StringSetAsync()`

```csharp
await _db.StringSetAsync(key, serialized, ttl);  // ← BREAKPOINT HERE
```

**Check:**
- Is `key` null?
- Is `serialized` null?
- Is `ttl` valid?

### Breakpoint 4: Check AuthService Flow
**File:** `AuthService.cs`
**Line:** `LoginAsync()` method

```csharp
// STEP 3: Validate credentials
bool isValidCredentials = user != null &&
                         PasswordHelper.VerifyPassword(request.Password, user.PasswordHash);

if (!isValidCredentials)
{
    // ✅ Tăng failed attempts
    var failedCount = await _loginAttemptService.IncreaseFailedCountAsync(request.Email);  // ← BREAKPOINT
    ...
}
```

**Check:**
- Is `request.Email` valid?
- What's the return value of `IncreaseFailedCountAsync()`?

---

## 📊 Log Analysis

### Good Logs (Success)
```
🔐 Login attempt for email: user@example.com
✅ Login attempt counter reset for: user@example.com
✅ Login successful | UserId: ...
```

### Bad Logs (Error)
```
🔐 Login attempt for email: user@example.com
❌ GetKey called with null or empty email
⚠️ Tham số không hợp lệ (Login): Value cannot be null. (Parameter 'key')
```

---

## 🔧 How to Verify Fix

### Step 1: Run Login Test
```
1. POST /api/auth/login
   {
     "email": "user555@example.com",
     "password": "wrongpassword"
   }
   → Expected: 400 Bad Request (invalid credentials)

2. POST /api/auth/login
   {
     "email": "user555@example.com",
     "password": "wrongpassword"
   }
   → Expected: 400 Bad Request (invalid credentials)

3. POST /api/auth/login
   {
     "email": "user555@example.com",
     "password": "correctpassword"
   }
   → Expected: 200 OK (login successful)
```

### Step 2: Check Logs
```
✅ Should see: "✅ Login attempt counter reset for: user555@example.com"
✅ Should see: "✅ Login successful | UserId: ..."
❌ Should NOT see: "Value cannot be null. (Parameter 'key')"
```

### Step 3: Check Redis
```
redis-cli
> KEYS login_attempt:*
(empty list or list)

> GET login_attempt:user555@example.com
(nil)  ← Should be nil after successful login
```

---

## 📝 Summary of Changes

| File | Method | Change |
|------|--------|--------|
| `RedisLoginAttemptService.cs` | `GetKey()` | Throw exception instead of returning null |
| `RedisLoginAttemptService.cs` | `IsLockedAsync()` | Add null check for key |
| `RedisLoginAttemptService.cs` | `IncreaseFailedCountAsync()` | Add null check for key |
| `RedisLoginAttemptService.cs` | `ResetAsync()` | Add null check for key |
| `AuthService.cs` | `LoginAsync()` | Reorder steps + null-safe operations |

