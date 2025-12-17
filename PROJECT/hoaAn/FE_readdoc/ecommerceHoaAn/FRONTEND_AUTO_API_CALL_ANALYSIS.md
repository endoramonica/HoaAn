# Frontend Auto API Call Analysis - `/cart/process`

## 📋 Tóm Tắt
Frontend **KHÔNG** có logic tự động gọi API `/cart/process` khi vào trang `/cart` hoặc `/checkout`.

---

## 🔍 Chi Tiết Phân Tích

### 1. **Trang Cart** (`src/components/CartPage.tsx`)
- **Khi mount**: Gọi `useCart()` hook
- **useCart hook** tự động:
  - Gọi `CartService.getApiV1Cart()` hoặc `CartService.getApiV1CartGuest()` để lấy dữ liệu giỏ hàng
  - Gọi `CartService.getApiV1CartSummary()` để lấy tóm tắt giỏ hàng
  - **KHÔNG** gọi `/cart/process`

**Endpoint được gọi:**
```
GET /api/v1/cart (hoặc /api/v1/cart/guest)
GET /api/v1/cart/summary (hoặc /api/v1/cart/guest/summary)
```

**Code:**
```typescript
// src/lib/hooks/useCart.ts - useEffect
useEffect(() => {
  if (authLoading) return;
  
  const loadCart = async () => {
    setIsLoading(true);
    await Promise.all([
      fetchCart(),      // Gọi GET /api/v1/cart
      fetchCartCount()  // Gọi GET /api/v1/cart/item-count
    ]);
    setIsLoading(false);
  };
  
  loadCart();
}, [authLoading, isAuthenticated, isGuest]);
```

---

### 2. **Trang Checkout** (`src/pages/checkout/CheckoutPage.tsx`)
- **Khi mount**: Gọi `useCart()` hook (giống như CartPage)
- **Khi user click "Tiến hành thanh toán"**: Gọi `processCheckout()`
- **KHÔNG** có auto-call `/cart/process` khi vào trang

**Endpoint được gọi khi click thanh toán:**
```
POST /api/v1/checkout/process
```

**Code:**
```typescript
// src/pages/checkout/CheckoutPage.tsx - handlePlaceOrder()
const handlePlaceOrder = async () => {
  // ... validation ...
  
  const result = await processCheckout(checkoutData);
  // processCheckout gọi: POST /api/v1/checkout/process
};
```

---

### 3. **useCheckout Hook** (`src/lib/hooks/useCheckout.ts`)
- **Hàm `processCheckout()`**: Gọi `api.postApiV1CheckoutProcess(checkoutData)`
- **Endpoint**: `POST /Checkout/process` (từ Orval-generated API)
- **Khi được gọi**: Chỉ khi user click nút "Tiến hành thanh toán"

**Code:**
```typescript
const processCheckout = useCallback(async (
  checkoutData: CheckoutDto
): Promise<OrderDetailDto | null> => {
  try {
    setIsProcessing(true);
    
    const response = await api.postApiV1CheckoutProcess(checkoutData);
    // Gọi POST /Checkout/process
    
    toast.success('Đặt hàng thành công!');
    return response.data;
  } catch (err) {
    // Error handling
  } finally {
    setIsProcessing(false);
  }
}, []);
```

---

## 🎯 Kết Luận

| Trang | Khi Mount | Khi Click Thanh Toán |
|-------|----------|-------------------|
| `/cart` | Gọi `GET /api/v1/cart` | N/A |
| `/checkout` | Gọi `GET /api/v1/cart` | Gọi `POST /Checkout/process` |

**Không có auto-call `/cart/process` ở bất kỳ đâu.**

---

## 📌 Các Endpoint Thực Tế Được Gọi

### Khi vào trang Cart/Checkout:
```
GET /api/v1/cart (hoặc /api/v1/cart/guest)
GET /api/v1/cart/summary (hoặc /api/v1/cart/guest/summary)
GET /api/v1/cart/item-count (hoặc /api/v1/cart/guest/item-count)
```

### Khi click "Tiến hành thanh toán":
```
POST /Checkout/process
```

---

## 🔧 Nếu Cần Thêm Auto-Call `/cart/process`

Nếu backend yêu cầu tự động gọi `/cart/process` khi vào trang checkout, có thể thêm:

```typescript
// src/pages/checkout/CheckoutPage.tsx
useEffect(() => {
  const autoProcessCart = async () => {
    try {
      console.log('[CheckoutPage] 🔄 Auto-processing cart...');
      const result = await CartService.postApiV1CartProcess();
      console.log('[CheckoutPage] ✅ Cart processed:', result);
    } catch (err) {
      console.error('[CheckoutPage] ❌ Auto-process failed:', err);
    }
  };
  
  autoProcessCart();
}, []);
```

Nhưng hiện tại **KHÔNG CÓ** logic này.
