# 🛒 Luồng Merge Cart - Chi Tiết Phân Tích

## 📊 Tổng Quan Luồng

```
Guest User (Chưa đăng nhập)
    ↓
useCart() hook chạy
    ↓
isGuest = true
    ↓
Tạo guest_cart_session_id (nếu chưa có)
    ↓
Lưu vào localStorage: 'guest_cart_session_id'
    ↓
Gọi CartService.getApiV1CartGuest() với sessionId
    ↓
Backend tạo guest cart session
    ↓
User thêm sản phẩm vào giỏ hàng
    ↓
Tất cả request gửi sessionId qua cookie/header
    ↓
User click "Đăng nhập"
    ↓
Login thành công
    ↓
mergeGuestCart() được gọi
    ↓
Lấy guest_cart_session_id từ localStorage
    ↓
Gọi CartService.postApiV1CartMerge({ sessionId })
    ↓
Backend merge guest cart → user cart
    ↓
Xóa guest_cart_session_id từ localStorage
    ↓
User cart hiển thị tất cả items (guest + user)
```

## 🔍 Chi Tiết Từng Bước

### 1️⃣ **Guest User - Tạo Session**

**File:** `src/lib/hooks/useCart.ts` (line 115-121)

```typescript
if (isGuest) {
  let sessionId = localStorage.getItem('guest_cart_session_id');
  if (!sessionId) {
    // Tạo unique sessionId
    sessionId = `guest_${Date.now()}_${Math.random().toString(36).substring(2, 11)}`;
    localStorage.setItem('guest_cart_session_id', sessionId);
    console.log('[useCart] 🆕 Created guest cart sessionId:', sessionId);
  }
}
```

**Kết quả:**
- ✅ localStorage: `guest_cart_session_id = "guest_1735123456789_abc123def456"`
- ✅ Backend nhận sessionId qua cookie hoặc header

### 2️⃣ **Guest User - Fetch Cart**

**File:** `src/lib/hooks/useCart.ts` (line 130-131)

```typescript
const cartResponse = (isGuest
  ? await CartService.getApiV1CartGuest()  // ← Gửi sessionId
  : await CartService.getApiV1Cart());     // ← Gửi JWT token
```

**Backend xử lý:**
- Nếu `isGuest = true` → Tìm/tạo guest cart session
- Nếu `isGuest = false` → Tìm user cart từ JWT token

### 3️⃣ **Guest User - Thêm Sản Phẩm**

**File:** `src/lib/hooks/useCart.ts` (line 211-220)

```typescript
if (isGuest) {
  const addToCartDto: AddToCartDto = { ... };
  await CartService.postApiV1CartGuestAdd(addToCartDto);  // ← Gửi sessionId
} else {
  await CartService.postApiV1CartAdd(addToCartDto);       // ← Gửi JWT token
}
```

**Backend xử lý:**
- Tìm guest cart session từ sessionId
- Thêm item vào guest cart

### 4️⃣ **User Login - Trigger Merge**

**File:** `src/lib/hooks/useAuth.ts` (line 140-156)

```typescript
const login = useCallback(async (request: LoginRequest) => {
  try {
    setIsLoading(true);
    setError(null);

    console.log('[useAuth] Login started...');
    const response = await authService.login(request);

    console.log('[useAuth] Login successful, user:', response.user.email);

    setUser(response.user);
    saveUserToStorage(response.user);

    // ✅ CRITICAL: Merge guest cart sau khi login thành công
    await mergeGuestCart();

    toast.success('Đăng nhập thành công!');
  } catch (err: any) {
    // Error handling...
  }
});
```

**Khi nào gọi:**
- ✅ Sau `authService.login()` thành công
- ✅ Sau `authService.loginWithGoogle()` thành công
- ✅ Sau `authService.register()` thành công

### 5️⃣ **Merge Guest Cart - Chi Tiết**

**File:** `src/lib/hooks/useAuth.ts` (line 64-98)

```typescript
const mergeGuestCart = async () => {
  try {
    // Bước 1: Lấy guest sessionId từ localStorage
    const guestSessionId = localStorage.getItem('guest_cart_session_id');

    if (!guestSessionId) {
      console.log('[useAuth] ℹ️ No guest cart sessionId found, skipping merge');
      return;  // ← Không có guest cart, skip merge
    }

    console.log('[useAuth] 🔄 Merging guest cart to user cart...', { guestSessionId });

    // Bước 2: Gọi API merge
    await CartService.postApiV1CartMerge({ sessionId: guestSessionId });

    console.log('[useAuth] ✅ Cart merged successfully');

    // Bước 3: Xóa guest sessionId
    localStorage.removeItem('guest_cart_session_id');
    console.log('[useAuth] 🗑️ Cleared guest cart sessionId');
  } catch (error: any) {
    // Error handling
    if (error.status === 404 || error.message?.includes('Not Found')) {
      console.log('[useAuth] ℹ️ No guest cart to merge (404)');
      localStorage.removeItem('guest_cart_session_id');
    } else if (error.status === 415) {
      console.error('[useAuth] ❌ Cart merge 415: Backend expects different Content-Type');
    } else {
      console.log('[useAuth] ⚠️ Cart merge failed (non-critical):', error.message);
    }
  }
};
```

**Xử lý lỗi:**
- 404: Không có guest cart → Xóa sessionId
- 415: Content-Type sai → Log error
- Lỗi khác: Log warning (non-critical)

### 6️⃣ **Backend Merge Logic**

**Backend xử lý (ASP.NET Core):**

```csharp
[HttpPost("merge")]
public async Task<IActionResult> MergeCart([FromBody] MergeCartDto request)
{
    // 1. Lấy guest cart từ sessionId
    var guestCart = await _cartService.GetGuestCart(request.SessionId);
    
    if (guestCart == null)
        return NotFound("Guest cart not found");
    
    // 2. Lấy user cart từ JWT token
    var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
    var userCart = await _cartService.GetUserCart(userId);
    
    // 3. Merge items
    foreach (var item in guestCart.Items)
    {
        var existingItem = userCart.Items.FirstOrDefault(x => x.ProductId == item.ProductId);
        
        if (existingItem != null)
            existingItem.Quantity += item.Quantity;  // Cộng số lượng
        else
            userCart.Items.Add(item);  // Thêm item mới
    }
    
    // 4. Xóa guest cart
    await _cartService.DeleteGuestCart(request.SessionId);
    
    // 5. Lưu user cart
    await _cartService.SaveCart(userCart);
    
    return Ok(userCart);
}
```

## ⚠️ Vấn Đề Tiềm Ẩn

### 1. **Merge không được gọi nếu login thất bại**
- ✅ Hiện tại: Chỉ gọi `mergeGuestCart()` khi login thành công
- ✅ Điều này đúng vì guest cart vẫn còn nếu login thất bại

### 2. **Merge không được gọi nếu user đã authenticated**
- ✅ Hiện tại: `mergeGuestCart()` chỉ gọi trong login/register
- ✅ Nếu user refresh page khi đã authenticated, merge không chạy lại
- ⚠️ Nhưng điều này OK vì merge chỉ cần chạy 1 lần

### 3. **Nếu merge API fail, guest cart vẫn còn**
- ✅ Hiện tại: Nếu merge fail, guest sessionId vẫn lưu
- ⚠️ Lần login tiếp theo sẽ cố merge lại
- ✅ Điều này OK vì merge là idempotent (chạy nhiều lần không sao)

### 4. **Nếu user logout rồi login lại**
- ✅ Hiện tại: Guest sessionId được xóa sau merge thành công
- ✅ Nếu user logout, sessionId không còn
- ✅ Nếu user login lại, sẽ là guest mới (sessionId mới)
- ✅ Điều này đúng vì guest cart là per-session

## 🎯 Luồng Hoàn Chỉnh - Ví Dụ

### Scenario: User thêm sản phẩm, logout, login lại

```
1. User vào trang (chưa login)
   → isGuest = true
   → Tạo guest_cart_session_id = "guest_123_abc"
   → Fetch guest cart

2. User thêm sản phẩm A vào giỏ
   → POST /api/v1/cart/guest/add (sessionId: "guest_123_abc")
   → Backend: Thêm A vào guest cart

3. User thêm sản phẩm B vào giỏ
   → POST /api/v1/cart/guest/add (sessionId: "guest_123_abc")
   → Backend: Thêm B vào guest cart

4. User click "Đăng nhập"
   → Login thành công
   → mergeGuestCart() chạy
   → POST /api/v1/cart/merge (sessionId: "guest_123_abc")
   → Backend: Merge A, B vào user cart
   → localStorage.removeItem('guest_cart_session_id')

5. User cart hiển thị: [A, B]

6. User logout
   → localStorage.removeItem('authToken')
   → isGuest = true
   → Tạo guest_cart_session_id = "guest_456_def" (NEW)
   → Fetch guest cart (rỗng)

7. User thêm sản phẩm C vào giỏ
   → POST /api/v1/cart/guest/add (sessionId: "guest_456_def")
   → Backend: Thêm C vào guest cart (mới)

8. User login lại
   → Login thành công
   → mergeGuestCart() chạy
   → POST /api/v1/cart/merge (sessionId: "guest_456_def")
   → Backend: Merge C vào user cart
   → User cart hiển thị: [A, B, C]
```

## ✅ Kết Luận

**Luồng mergeCart hoạt động đúng:**
- ✅ Guest cart được tạo khi user chưa login
- ✅ Guest sessionId được lưu trong localStorage
- ✅ Merge được gọi sau login/register thành công
- ✅ Guest sessionId được xóa sau merge thành công
- ✅ Error handling hợp lý (404, 415, etc.)
- ✅ Merge là idempotent (chạy nhiều lần OK)

**Không có vấn đề gì cần fix trong mergeCart logic.**
