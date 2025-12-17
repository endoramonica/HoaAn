# Calendar Booking - Quick Fix Summary

## 🚨 Problem
Service gọi `postApiV1CheckoutProcess` → User vào checkout click "Đặt hàng" → gọi API lần 2 → **400 Error: Cart đã dùng**

## ✅ Solution
Service **KHÔNG gọi Checkout API**, chỉ lưu booking info vào sessionStorage. Checkout API được gọi ở CheckoutPage khi user click "Đặt hàng".

## 📝 Changes

### 1. calendarBookingService.ts
```diff
- const api = getVietCommerceAPI();
- await api.postApiV1CheckoutProcess(checkoutRequest);
+ sessionStorage.setItem('bookingInfo', JSON.stringify(bookingInfo));
```

### 2. EventSidebar.tsx
```diff
- navigate(`/checkout?orderId=${result.orderId}`);
+ navigate('/checkout');
```

### 3. CheckoutPage.tsx
```diff
+ useEffect(() => {
+   const bookingInfo = sessionStorage.getItem('bookingInfo');
+   if (bookingInfo) {
+     setOrderNote(JSON.parse(bookingInfo).bookingNotes);
+   }
+ }, []);
```

## 🔄 New Flow
```
EventSidebar
  ├─ addToCart ✅
  ├─ createBookingFromEvent() → sessionStorage ✅
  └─ navigate('/checkout') ✅
     └─ CheckoutPage
        ├─ Load bookingInfo from sessionStorage ✅
        └─ User click "Đặt hàng"
           └─ POST /Checkout/process ✅ (lần duy nhất)
```

## ✨ Result
- ✅ Không 400 error
- ✅ Checkout API gọi 1 lần
- ✅ Booking info được lưu + hiển thị
