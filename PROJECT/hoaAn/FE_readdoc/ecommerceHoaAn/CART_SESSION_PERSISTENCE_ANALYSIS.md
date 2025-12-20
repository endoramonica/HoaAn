# 🛒 Phân Tích Vấn Đề: Giỏ Hàng Không Lưu Theo Phiên Đăng Nhập

## 📋 Tóm Tắt Vấn Đề

**Vấn đề chính:** Giỏ hàng của người dùng đã đăng nhập không được lưu theo phiên (session), chỉ lưu trong localStorage cho khách vô danh.

**Kết quả:** 
- ❌ Người dùng đã đăng nhập: Giỏ hàng không persistent (mất khi refresh/đóng tab)
- ✅ Khách vô danh: Giỏ hàng lưu trong localStorage (persistent)

---

## 🔍 Phân Tích Flow Hiện Tại

### 1. **Guest User (Khách Vô Danh)**

```
User vào trang (chưa login)
    ↓
useCart() hook chạy
    ↓
isGuest = true
    ↓
fetchCart() → Tạo sessionId
    ↓
sessionId = `guest_${Date.now()}_${random}`
    ↓
localStorage.setItem('guest_cart_session_id', sessionId)
    ↓
Gọi CartService.getApiV1CartGuest()
    ↓
Backend nhận request với sessionId (qua cookie)
    ↓
Backend tạo guest cart session
    ↓
✅ Guest cart được lưu trên backend (session-based)
```

**Kết quả:** ✅ Guest cart persistent (lưu trên backend)

---

### 2. **Authenticated User (Người Dùng Đã Đăng Nhập)**

```
User đã login
    ↓
useCart() hook chạy
    ↓
isGuest = false
    ↓
fetchCart() → Gọi CartService.getApiV1Cart()
    ↓
Backend nhận request với JWT token
    ↓
Backend lấy userId từ token
    ↓
Backend trả về user cart từ database
    ↓
Frontend lưu cart vào state (React state)
    ↓
❌ Cart KHÔNG được lưu trong localStorage
    ↓
User refresh page
    ↓
React state bị reset
    ↓
useCart() chạy lại
    ↓
Fetch cart từ backend lại
    ↓
⚠️ Nếu backend không có cart → Cart trống
```

**Kết quả:** ⚠️ User cart phụ thuộc vào backend, không có fallback

---

## 🎯 Root Cause Analysis

### **Vấn đề 1: Không Có Session Persistence cho User Cart**

**File:** `src/lib/hooks/useCart.ts`

```typescript
// ❌ HIỆN TẠI: Cart chỉ lưu trong React state
const [cart, setCart] = useState<Cart | null>(null);

// Khi user refresh page:
// 1. React state bị reset → cart = null
// 2. useEffect chạy lại
// 3. Fetch cart từ backend
// 4. Nếu backend fail → Cart trống
```

**Giải pháp:** Lưu user cart vào localStorage (tương tự guest cart)

---

### **Vấn đề 2: Merge Cart Không Đảm Bảo Persistence**

**File:** `src/lib/hooks/useAuth.ts`

```typescript
const mergeGuestCart = async () => {
  try {
    // ✅ Merge được gọi
    await CartService.postApiV1CartMerge({ sessionId: guestSessionId });
    
    // ❌ Nhưng không có cơ chế lưu merged cart
    // Nếu user refresh ngay sau merge → Cart có thể mất
    
    localStorage.removeItem('guest_cart_session_id');
  } catch (error) {
    // ...
  }
};
```

**Giải pháp:** Lưu merged cart vào localStorage sau khi merge thành công

---

### **Vấn đề 3: Không Có Fallback khi Backend Fail**

```typescript
// ❌ HIỆN TẠI: Nếu backend fail → Cart trống
const fetchCart = useCallback(async () => {
  try {
    const cartResponse = await CartService.getApiV1Cart();
    setCart(cartResponse);
  } catch (err) {
    setCart(null);  // ❌ Cart trống
    setError(err.message);
  }
}, []);
```

**Giải pháp:** Fallback đến localStorage nếu backend fail

---

## 📊 So Sánh: Guest vs User Cart

| Tiêu Chí | Guest Cart | User Cart |
|---------|-----------|----------|
| **Lưu trữ chính** | Backend (session) | Backend (database) |
| **Fallback** | localStorage (sessionId) | ❌ Không có |
| **Persistence** | ✅ Persistent | ⚠️ Phụ thuộc backend |
| **Refresh page** | ✅ Giữ được | ❌ Có thể mất |
| **Đóng tab** | ✅ Giữ được | ❌ Có thể mất |
| **Logout** | ✅ Xóa sạch | ✅ Xóa sạch |

---

## 🔧 Giải Pháp Đề Xuất

### **Bước 1: Tạo User Cart Storage Utility**

```typescript
// src/lib/utils/userCartStorage.ts

const USER_CART_CACHE_KEY = 'user_cart_cache';
const USER_CART_TIMESTAMP_KEY = 'user_cart_timestamp';
const CACHE_DURATION = 24 * 60 * 60 * 1000; // 24 hours

export const userCartStorage = {
  // Lưu user cart vào localStorage
  saveCart(cart: Cart): void {
    try {
      localStorage.setItem(USER_CART_CACHE_KEY, JSON.stringify(cart));
      localStorage.setItem(USER_CART_TIMESTAMP_KEY, Date.now().toString());
      console.log('[UserCart] 💾 Saved cart to localStorage');
    } catch (error) {
      console.error('Failed to save user cart:', error);
    }
  },

  // Lấy user cart từ localStorage
  getCart(): Cart | null {
    try {
      const cartJson = localStorage.getItem(USER_CART_CACHE_KEY);
      const timestamp = localStorage.getItem(USER_CART_TIMESTAMP_KEY);
      
      if (!cartJson || !timestamp) return null;
      
      // Kiểm tra cache expiry
      const age = Date.now() - parseInt(timestamp);
      if (age > CACHE_DURATION) {
        this.clearCart();
        return null;
      }
      
      return JSON.parse(cartJson);
    } catch (error) {
      console.error('Failed to get user cart:', error);
      return null;
    }
  },

  // Xóa user cart từ localStorage
  clearCart(): void {
    try {
      localStorage.removeItem(USER_CART_CACHE_KEY);
      localStorage.removeItem(USER_CART_TIMESTAMP_KEY);
      console.log('[UserCart] 🗑️ Cleared cart from localStorage');
    } catch (error) {
      console.error('Failed to clear user cart:', error);
    }
  },

  // Kiểm tra cache còn hợp lệ không
  isCacheValid(): boolean {
    try {
      const timestamp = localStorage.getItem(USER_CART_TIMESTAMP_KEY);
      if (!timestamp) return false;
      
      const age = Date.now() - parseInt(timestamp);
      return age < CACHE_DURATION;
    } catch (error) {
      return false;
    }
  }
};
```

---

### **Bước 2: Cập Nhật useCart Hook**

```typescript
// src/lib/hooks/useCart.ts

const fetchCart = useCallback(async () => {
  try {
    const cartResponse = isGuest
      ? await CartService.getApiV1CartGuest()
      : await CartService.getApiV1Cart();

    const transformedItems = /* ... */;
    const newCart = /* ... */;

    setCart(newCart);
    
    // ✅ NEW: Lưu user cart vào localStorage
    if (!isGuest) {
      userCartStorage.saveCart(newCart);
    }

    setError(null);
  } catch (err: any) {
    // ✅ NEW: Fallback đến localStorage nếu backend fail
    if (!isGuest && err?.status !== 404) {
      const cachedCart = userCartStorage.getCart();
      if (cachedCart) {
        console.log('[useCart] ⚠️ Backend failed, using cached cart');
        setCart(cachedCart);
        setError(null);
        return;
      }
    }

    // Xử lý error như bình thường
    if (err?.status === 404) {
      setCart(null);
    } else {
      setError(err.message);
    }
  }
}, [isGuest]);

// ✅ NEW: Load cart từ cache khi component mount
useEffect(() => {
  if (authLoading) return;

  let isMounted = true;

  const loadCart = async () => {
    setIsLoading(true);

    // ✅ NEW: Nếu là user, thử load từ cache trước
    if (!isGuest) {
      const cachedCart = userCartStorage.getCart();
      if (cachedCart && isMounted) {
        console.log('[useCart] 📦 Loaded cart from cache');
        setCart(cachedCart);
        setCartItemCount(cachedCart.itemCount);
      }
    }

    // Fetch từ backend (update cache)
    try {
      await Promise.all([fetchCart(), fetchCartCount()]);
    } catch (err) {
      console.error('[useCart] Error loading cart:', err);
    } finally {
      if (isMounted) {
        setIsLoading(false);
      }
    }
  };

  loadCart();

  return () => {
    isMounted = false;
  };
}, [authLoading, isAuthenticated, isGuest]);
```

---

### **Bước 3: Cập Nhật useAuth Hook**

```typescript
// src/lib/hooks/useAuth.ts

const mergeGuestCart = async () => {
  try {
    const guestSessionId = localStorage.getItem('guest_cart_session_id');

    if (!guestSessionId) {
      console.log('[useAuth] ℹ️ No guest cart to merge');
      return;
    }

    console.log('[useAuth] 🔄 Merging guest cart...');

    // Merge cart
    const mergedCartResponse = await CartService.postApiV1CartMerge({ 
      sessionId: guestSessionId 
    });

    console.log('[useAuth] ✅ Cart merged successfully');

    // ✅ NEW: Lưu merged cart vào localStorage
    if (mergedCartResponse?.data) {
      userCartStorage.saveCart(mergedCartResponse.data);
      console.log('[useAuth] 💾 Saved merged cart to localStorage');
    }

    // Clear guest cart sessionId
    localStorage.removeItem('guest_cart_session_id');
    console.log('[useAuth] 🗑️ Cleared guest cart sessionId');
  } catch (error: any) {
    // Error handling như bình thường
    if (error.status === 404) {
      console.log('[useAuth] ℹ️ No guest cart to merge (404)');
      localStorage.removeItem('guest_cart_session_id');
    } else {
      console.warn('[useAuth] ⚠️ Cart merge failed:', error.message);
      localStorage.removeItem('guest_cart_session_id');
    }
  }
};

const logout = useCallback(async () => {
  try {
    setIsLoading(true);
    await authService.logout();

    setUser(null);
    clearUserFromStorage();
    
    // ✅ NEW: Xóa user cart cache khi logout
    userCartStorage.clearCart();

    toast.success('Đăng xuất thành công!');
  } catch (err) {
    console.error('[useAuth] Logout error:', err);
    setUser(null);
    clearUserFromStorage();
    userCartStorage.clearCart();
  } finally {
    setIsLoading(false);
  }
}, []);
```

---

### **Bước 4: Cập Nhật Logout Flow**

```typescript
// src/lib/hooks/useCart.ts

const clearCart = useCallback(async () => {
  try {
    if (isGuest) {
      await CartService.deleteApiV1CartGuestClear();
    } else {
      await CartService.deleteApiV1CartClear();
      // ✅ NEW: Xóa cache khi clear cart
      userCartStorage.clearCart();
    }

    setCart(null);
    setCartItemCount(0);
  } catch (err: any) {
    setError(err.message || "Không thể xóa giỏ hàng");
    throw err;
  }
}, [isGuest]);
```

---

## 📈 Kết Quả Sau Khi Fix

| Tiêu Chí | Trước | Sau |
|---------|------|-----|
| **User cart persistence** | ❌ Không | ✅ Có (localStorage) |
| **Refresh page** | ❌ Mất cart | ✅ Giữ được (cache) |
| **Backend fail** | ❌ Cart trống | ✅ Fallback cache |
| **Merge cart** | ⚠️ Không persistent | ✅ Lưu cache |
| **Logout** | ✅ Xóa | ✅ Xóa (+ cache) |

---

## 🔐 Bảo Mật & Lưu Ý

### **Lưu Ý 1: Cache Expiry**
- Cache hết hạn sau 24 giờ
- Tránh lưu thông tin cũ quá lâu

### **Lưu Ý 2: Sensitive Data**
- ❌ Không lưu giá tiền nhạy cảm trong cache
- ✅ Lưu chỉ cartId, itemIds, quantities
- ✅ Luôn verify với backend trước checkout

### **Lưu Ý 3: Storage Quota**
- localStorage có giới hạn ~5-10MB
- Cart data thường nhỏ (<100KB)
- Không vấn đề

### **Lưu Ý 4: Logout Security**
- ✅ Xóa cache khi logout
- ✅ Xóa token khi logout
- ✅ Xóa user data khi logout

---

## 🧪 Test Cases

### **Test 1: User Cart Persistence**
```
1. Login
2. Add item to cart
3. Refresh page
4. ✅ Cart vẫn có item
5. Check localStorage → ✅ Cart data có
```

### **Test 2: Backend Fail Fallback**
```
1. Login
2. Add item to cart
3. Disconnect network
4. Refresh page
5. ✅ Cart load từ cache
6. Reconnect network
7. ✅ Cart update từ backend
```

### **Test 3: Merge Cart Persistence**
```
1. Guest add item
2. Login
3. Merge cart
4. Refresh page
5. ✅ Merged items vẫn có
6. Check localStorage → ✅ Merged cart data có
```

### **Test 4: Logout Clear Cache**
```
1. Login
2. Add item
3. Logout
4. Check localStorage → ❌ Cart data xóa
5. Login lại
6. ✅ Cart trống (hoặc từ backend)
```

---

## 📝 Implementation Checklist

- [ ] Tạo `src/lib/utils/userCartStorage.ts`
- [ ] Cập nhật `src/lib/hooks/useCart.ts` (save cache)
- [ ] Cập nhật `src/lib/hooks/useCart.ts` (load cache)
- [ ] Cập nhật `src/lib/hooks/useAuth.ts` (save merged cart)
- [ ] Cập nhật `src/lib/hooks/useAuth.ts` (clear cache on logout)
- [ ] Test user cart persistence
- [ ] Test backend fail fallback
- [ ] Test merge cart persistence
- [ ] Test logout clear cache

---

## 🎯 Ưu Tiên

**Cao:** Lưu user cart vào localStorage (persistence)
**Cao:** Fallback cache khi backend fail
**Trung:** Lưu merged cart vào cache
**Trung:** Clear cache on logout

