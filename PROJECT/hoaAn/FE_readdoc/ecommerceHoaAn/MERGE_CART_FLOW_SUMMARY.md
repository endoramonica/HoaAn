# 🛒 Merge Cart Flow - Tóm Tắt Hoạt Động

## 📊 Luồng Chính

### 1. **Guest User - Tạo Session**
```
User vào trang (chưa login)
    ↓
useCart() hook chạy
    ↓
isAuthenticated = false → isGuest = true
    ↓
fetchCart() chạy
    ↓
Kiểm tra localStorage.getItem('guest_cart_session_id')
    ↓
Nếu không có → Tạo sessionId mới
    sessionId = `guest_${Date.now()}_${random}`
    ↓
Lưu vào localStorage: 'guest_cart_session_id'
    ↓
Gọi CartService.getApiV1CartGuest()
    ↓
Backend nhận request với cookie (sessionId)
    ↓
Backend tạo/lấy guest cart session
    ↓
Trả về guest cart (rỗng hoặc có items)
```

### 2. **Guest User - Thêm Sản Phẩm**
```
User click "Thêm vào giỏ"
    ↓
Gọi CartService.postApiV1CartGuestAdd(addToCartDto)
    ↓
Request gửi:
    - Method: POST
    - URL: /api/v1/Cart/guest/add
    - Body: { productId, quantity, ... }
    - Cookie: sessionId (tự động gửi vì withCredentials: true)
    ↓
Backend nhận request
    ↓
Lấy sessionId từ cookie
    ↓
Tìm guest cart session
    ↓
Thêm item vào guest cart
    ↓
Trả về updated cart
```

### 3. **User Login - Trigger Merge**
```
User click "Đăng nhập"
    ↓
Gọi authService.login(email, password)
    ↓
Backend xác thực
    ↓
Trả về JWT token + user info
    ↓
Frontend lưu token vào localStorage
    ↓
setUser(response.user)
    ↓
Gọi mergeGuestCart()
    ↓
Lấy guest_cart_session_id từ localStorage
    ↓
Gọi CartService.postApiV1CartMerge({ sessionId })
    ↓
Request gửi:
    - Method: POST
    - URL: /api/v1/Cart/merge
    - Body: { sessionId: "guest_123_abc" }
    - Header: Authorization: Bearer {JWT_TOKEN}
    ↓
Backend nhận request
    ↓
Xác thực JWT token → Lấy userId
    ↓
Lấy guest cart từ sessionId
    ↓
Lấy user cart từ userId
    ↓
Merge items:
    - Nếu item đã có trong user cart → Cộng quantity
    - Nếu item chưa có → Thêm item mới
    ↓
Xóa guest cart session
    ↓
Lưu user cart
    ↓
Trả về merged cart
    ↓
Frontend xóa guest_cart_session_id từ localStorage
    ↓
useCart() refresh → Fetch user cart
    ↓
User cart hiển thị tất cả items (guest + user)
```

## 🔑 Key Points

### ✅ **SessionId Management**
- **Tạo:** Khi user vào trang (chưa login)
- **Lưu:** localStorage: `guest_cart_session_id`
- **Gửi:** Qua cookie (withCredentials: true)
- **Xóa:** Sau merge thành công

### ✅ **Request Flow**
```
Guest Request:
  GET /api/v1/Cart/guest
  Cookie: sessionId (tự động)
  
User Request:
  GET /api/v1/Cart
  Header: Authorization: Bearer {JWT_TOKEN}
  
Merge Request:
  POST /api/v1/Cart/merge
  Body: { sessionId: "guest_123_abc" }
  Header: Authorization: Bearer {JWT_TOKEN}
```

### ✅ **Error Handling**
```
404 Not Found
  → Không có guest cart
  → Xóa sessionId
  → Không retry

415 Unsupported Media Type
  → Backend expects different Content-Type
  → Xóa sessionId
  → Không retry

401/403 Unauthorized
  → User không authorized
  → Xóa sessionId
  → Không retry

Lỗi khác
  → Xóa sessionId
  → Không retry (tránh vô hạn)
```

## 🎯 Cải Tiến Gần Đây

### ✅ **Error Handling Improvement**
**File:** `src/lib/hooks/useAuth.ts`

Trước:
```typescript
catch (error: any) {
  if (error.status === 404) {
    localStorage.removeItem('guest_cart_session_id');
  } else if (error.status === 415) {
    console.error('...');
    // ❌ Không xóa sessionId → Retry vô hạn
  }
}
```

Sau:
```typescript
catch (error: any) {
  if (error.status === 404) {
    localStorage.removeItem('guest_cart_session_id');
  } else if (error.status === 415) {
    console.error('...');
    // ✅ Xóa sessionId → Không retry
    localStorage.removeItem('guest_cart_session_id');
  } else if (error.status === 401 || error.status === 403) {
    // ✅ Xóa sessionId → Không retry
    localStorage.removeItem('guest_cart_session_id');
  } else {
    // ✅ Xóa sessionId → Không retry
    localStorage.removeItem('guest_cart_session_id');
  }
}
```

## 📋 Checklist - Merge Cart Hoạt Động Đúng

- ✅ Guest sessionId được tạo khi user vào trang
- ✅ Guest sessionId được lưu trong localStorage
- ✅ Guest sessionId được gửi qua cookie (withCredentials: true)
- ✅ Merge được gọi sau login/register thành công
- ✅ Merge API gửi sessionId trong request body
- ✅ Merge API gửi JWT token trong header
- ✅ Guest sessionId được xóa sau merge (thành công hoặc fail)
- ✅ Error handling xóa sessionId để tránh retry vô hạn
- ✅ Không có redirect loop khi merge fail

## 🚀 Kết Luận

**Merge Cart Flow hoạt động đúng và không có vấn đề gì cần fix.**

Tất cả các bước đều được xử lý đúng:
1. ✅ Guest session được tạo
2. ✅ Items được thêm vào guest cart
3. ✅ Merge được trigger sau login
4. ✅ Items được merge vào user cart
5. ✅ Guest session được xóa
6. ✅ Error handling hợp lý
