# 🔴 Stateful Login Bug - Root Cause & Fix

## 📋 Vấn đề (Symptoms)

**Hiện tượng:** Login thất bại khi:
1. Người dùng đăng nhập sai 1-n lần (email/mật khẩu không đúng)
2. Sau đó đăng nhập **đúng** → Exception xảy ra → Login thất bại

**Nhưng:** Nếu đăng nhập đúng ngay từ đầu → ✅ Không bị lỗi

**Kết luận:** Đây là **stateful bug** - lỗi phụ thuộc vào trạng thái trước đó

---

## 🔍 Root Cause Analysis

### Exception Log
```
VietCommerce.Application.Services.Services.AuthService: Warning: 
⚠️ Tham số không hợp lệ (Login): Value cannot be null. (Parameter 'key')
```

### Nguyên nhân chính

**File:** `VietCommerce.Application/Services/Services/AuthService.cs`

**Vấn đề:** Thứ tự xử lý sai trong `LoginAsync()`:

```csharp
// ❌ SAI: Reset failed attempts TRƯỚC khi validate token
await _loginAttemptService.ResetAsync(request.Email);  // Line 127

// ... sau đó ...

// Nếu exception xảy ra ở đây, failed attempts đã bị reset
var jti = _jwtHelper.GetJtiFromToken(accessToken);
ThrowIf(string.IsNullOrEmpty(jti), "Failed to extract JTI from token");
```

### Chuỗi sự kiện gây lỗi

1. **Lần 1-n:** Đăng nhập sai → `IncreaseFailedCountAsync()` → Redis lưu failed count
2. **Lần n+1:** Đăng nhập đúng:
   - ✅ Credentials valid
   - ✅ `ResetAsync()` được gọi → **Redis key bị xóa**
   - ❌ Nhưng sau đó, exception xảy ra khi extract JTI hoặc generate token
   - ❌ Exception không được catch đúng cách
   - ❌ Transaction rollback → Database không lưu được session
   - ❌ Nhưng Redis key đã bị xóa → Không thể retry

### Thêm vấn đề: Null Reference

```csharp
// ❌ Không check null
var permissions = user.UserRoles
    .SelectMany(ur => ur.Role.RolePermissions)  // Nếu ur.Role = null → NullReferenceException
    .Select(rp => rp.Permission.Name)
    .Distinct()
    .ToList();
```

---

## ✅ Fix Applied

### 1. **Reorder Steps - Reset AFTER Success**

```csharp
// ✅ ĐÚNG: Reset failed attempts CUỐI CÙNG, sau khi mọi thứ thành công

// STEP 14: Save all changes to database
await _unitOfWork.SaveChangesAsync();

// ✅ STEP 15: Reset failed attempts AFTER successful login & DB save
// This ensures we only reset if everything succeeded
await _loginAttemptService.ResetAsync(request.Email);
LogInfo($"✅ Login attempt counter reset for: {request.Email}");
```

### 2. **Safe Null Handling**

```csharp
// ✅ TRƯỚC: Không check null
var permissions = user.UserRoles
    .SelectMany(ur => ur.Role.RolePermissions)
    .Select(rp => rp.Permission.Name)
    .Distinct()
    .ToList();

// ✅ SAU: Null-safe
var permissions = user.UserRoles?
    .SelectMany(ur => ur.Role?.RolePermissions ?? new List<RolePermission>())
    .Select(rp => rp.Permission?.Name)
    .Where(p => !string.IsNullOrEmpty(p))
    .Distinct()
    .ToList() ?? new List<string>();
```

### 3. **Validate Token Generation**

```csharp
// ✅ Validate access token
var accessToken = _jwtHelper.GenerateToken(user, customer?.Id, permissions);
ThrowIf(string.IsNullOrEmpty(accessToken), "Failed to generate access token");

// ✅ Validate refresh token
var refreshToken = GenerateRefreshToken();
ThrowIf(string.IsNullOrEmpty(refreshToken), "Failed to generate refresh token");
```

### 4. **Validate JTI Extraction**

```csharp
// ✅ Extract JTI from token
var jti = _jwtHelper.GetJtiFromToken(accessToken);
ThrowIf(string.IsNullOrEmpty(jti), "Failed to extract JTI from token");
```

---

## 📊 Execution Flow (After Fix)

```
1. Check account lock
   ↓
2. Get user by email
   ↓
3. Validate credentials
   ↓
4. Check account status
   ↓
5. Extract guest session
   ↓
6. Update last login
   ↓
7. Extract permissions (NULL-SAFE)
   ↓
8. Generate access token (VALIDATED)
   ↓
9. Extract JTI (VALIDATED)
   ↓
10. Generate refresh token (VALIDATED)
   ↓
11. Save refresh token to DB
   ↓
12. Save session to Redis
   ↓
13. Merge guest cart (if exists)
   ↓
14. Save all changes to DB ← CRITICAL POINT
   ↓
15. ✅ RESET FAILED ATTEMPTS ← ONLY IF ALL ABOVE SUCCEEDED
   ↓
16. Prepare response
   ↓
17. Return success
```

---

## 🛡️ Why This Fix Works

### Before (Buggy)
- Reset happens **early** → If exception later, Redis is already cleaned
- No null checks → NullReferenceException possible
- No token validation → Silent failures

### After (Fixed)
- Reset happens **last** → Only if everything succeeded
- Null-safe operations → No NullReferenceException
- Token validation → Fail fast with clear error
- Atomic operation → Either all succeeds or all fails

---

## 🧪 Testing Scenarios

### Scenario 1: Login sai 3 lần, rồi login đúng
```
1. Login sai (email/password) → Failed count = 1
2. Login sai (email/password) → Failed count = 2
3. Login sai (email/password) → Failed count = 3
4. Login đúng → ✅ SUCCESS (failed count reset)
```

### Scenario 2: Login đúng ngay từ đầu
```
1. Login đúng → ✅ SUCCESS (no failed count)
```

### Scenario 3: Login sai 5 lần (account locked)
```
1-5. Login sai → Failed count = 5 → Account locked
6. Login đúng (nhưng account locked) → ❌ BLOCKED (correct behavior)
7. Wait 15 minutes → Account unlocked
8. Login đúng → ✅ SUCCESS
```

---

## 📝 Summary

| Aspect | Before | After |
|--------|--------|-------|
| Reset timing | Early (risky) | Late (safe) |
| Null handling | Unsafe | Safe |
| Token validation | None | Complete |
| Atomicity | Broken | Atomic |
| Stateful bug | ❌ YES | ✅ FIXED |

