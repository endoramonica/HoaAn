# Calendar Booking Design Fix - Giải quyết lỗi "Cart đã dùng"

## 🚨 Vấn đề Gốc

**Flow cũ (SAI):**
```
EventSidebar
  ├─ addToCart (useCart)
  ├─ createBookingFromEvent()
  │  └─ ❌ POST /Checkout/process (tạo order ngay)
  └─ redirect /checkout?orderId=...
     └─ User click "Đặt hàng"
        └─ ❌ POST /Checkout/process (lần 2)
           └─ 🚨 400 Error: Cart đã dùng
```

**Nguyên nhân:**
- Service `calendarBookingService` gọi `postApiV1CheckoutProcess` (tạo order)
- Sau đó user vào `/checkout` và click "Đặt hàng" → gọi API lần 2
- Backend: Cart chỉ có thể dùng 1 lần → **lỗi 400**

---

## ✅ Giải pháp

**Flow mới (ĐÚNG):**
```
EventSidebar
  ├─ addToCart (useCart) ✅
  ├─ createBookingFromEvent()
  │  └─ ✅ Lưu booking info vào sessionStorage (KHÔNG gọi API)
  └─ redirect /checkout
     └─ CheckoutPage
        ├─ Đọc booking info từ sessionStorage
        ├─ User click "Đặt hàng"
        └─ ✅ POST /Checkout/process (lần duy nhất)
           └─ ✅ Tạo order thành công
```

---

## 📝 Thay đổi Chi Tiết

### 1. **calendarBookingService.ts** - Xóa Checkout API call

**Trước:**
```typescript
// ❌ Gọi Checkout API
const checkoutResponse = await api.postApiV1CheckoutProcess(checkoutRequest);
```

**Sau:**
```typescript
// ✅ Chỉ lưu booking info vào sessionStorage
const bookingInfo = {
  eventId: event.id,
  eventTitle: event.title,
  cartId: cartId,
  serviceProductId: serviceProductId,
  customerName: customerName,
  customerPhone: customerPhone,
  customerEmail: customerEmail,
  serviceDate: request.serviceDate,
  serviceDuration: serviceDuration,
  serviceLocation: serviceLocation,
  serviceNotes: serviceNotes,
  lunarData: lunarData,
  customizationState: customizationState,
  bookingNotes: `...`, // Chi tiết booking
  createdAt: new Date().toISOString(),
};

sessionStorage.setItem('bookingInfo', JSON.stringify(bookingInfo));
```

**Thay đổi:**
- ✅ Xóa import `getVietCommerceAPI` (không cần gọi API)
- ✅ Xóa import `ShippingMethodEnum` (không cần)
- ✅ Xóa methods `getBookingOrder()` và `cancelBookingOrder()` (không dùng)
- ✅ Đổi comment từ "Flow: Add to Cart → Get Cart ID → Checkout" thành "Flow: Add to Cart → Save Booking Info → User goes to Checkout"

---

### 2. **EventSidebar.tsx** - Đơn giản hóa redirect

**Trước:**
```typescript
if (result.success && result.orderId) {
  toast.success(result.message);
  // Lưu thông tin booking vào sessionStorage
  sessionStorage.setItem('bookingInfo', JSON.stringify({
    orderId: result.orderId,
    serviceDate: formData.serviceDate,
    // ...
  }));
  // Redirect đến checkout
  setTimeout(() => {
    navigate(`/checkout?orderId=${result.orderId}`);
  }, 1000);
}
```

**Sau:**
```typescript
if (result.success) {
  toast.success(result.message);
  // Redirect đến checkout page
  setTimeout(() => {
    navigate('/checkout');
  }, 1000);
}
```

**Thay đổi:**
- ✅ Xóa logic lưu booking info (đã làm ở service)
- ✅ Xóa `?orderId=` query param (không cần)
- ✅ Xóa "View Order Button" (không có order ngay)

---

### 3. **CheckoutPage.tsx** - Đọc booking info

**Thêm:**
```typescript
// State để lưu booking info
const [bookingInfo, setBookingInfo] = useState<any>(null);

// Load booking info từ sessionStorage
useEffect(() => {
  const savedBookingInfo = sessionStorage.getItem('bookingInfo');
  if (savedBookingInfo) {
    try {
      const booking = JSON.parse(savedBookingInfo);
      setBookingInfo(booking);
      console.log('[CheckoutPage] 📅 Booking info loaded:', booking);
      
      // Pre-fill order note với booking details
      if (booking.bookingNotes && !orderNote) {
        setOrderNote(booking.bookingNotes);
      }
    } catch (err) {
      console.error('[CheckoutPage] Error parsing booking info:', err);
    }
  }
}, []);
```

**Thay đổi:**
- ✅ Thêm state `bookingInfo`
- ✅ Thêm useEffect để load booking info từ sessionStorage
- ✅ Pre-fill order note với booking details (nếu có)

---

## 🔄 Flow Hoàn Chỉnh

### Bước 1: User chọn ngày + dịch vụ
```
CalendarPage
  └─ EventSidebar
     └─ Click "Đặt lịch"
        └─ BookingDialog (nhập thông tin)
```

### Bước 2: Thêm sản phẩm vào cart + Lưu booking info
```
EventSidebar.handleBookingConfirm()
  ├─ cartService.addItem() ✅ (thêm sản phẩm vào cart)
  ├─ cartService.getCart() ✅ (lấy cart ID)
  └─ calendarBookingService.createBookingFromEvent()
     └─ sessionStorage.setItem('bookingInfo', ...) ✅ (lưu booking info)
```

### Bước 3: Redirect đến checkout
```
EventSidebar
  └─ navigate('/checkout') ✅
```

### Bước 4: CheckoutPage đọc booking info + Checkout
```
CheckoutPage
  ├─ useEffect: Load bookingInfo từ sessionStorage ✅
  ├─ Pre-fill order note với booking details ✅
  └─ User click "Đặt hàng"
     └─ useCheckout.processCheckout()
        └─ POST /Checkout/process ✅ (lần duy nhất)
           └─ ✅ Tạo order thành công
```

---

## 🧪 Test Cases

### ✅ Test 1: Calendar Booking Flow
1. Vào Calendar Page
2. Chọn ngày + sự kiện
3. Click "Đặt lịch"
4. Nhập thông tin booking
5. Click "Đặt lịch" → Redirect /checkout
6. Verify: Order note pre-filled với booking details
7. Click "Đặt hàng" → ✅ Order created (không 400 error)

### ✅ Test 2: Regular Checkout (không booking)
1. Vào Products Page
2. Add product to cart
3. Vào /checkout
4. Verify: Order note trống (không có booking info)
5. Click "Đặt hàng" → ✅ Order created

### ✅ Test 3: Multiple Bookings
1. Booking 1 → /checkout → Đặt hàng ✅
2. Booking 2 → /checkout → Verify booking info updated ✅
3. Đặt hàng ✅

---

## 📊 Tóm Tắt Thay Đổi

| File | Thay Đổi |
|------|---------|
| `calendarBookingService.ts` | ✅ Xóa Checkout API call, chỉ lưu booking info |
| `EventSidebar.tsx` | ✅ Đơn giản hóa redirect, xóa "View Order Button" |
| `CheckoutPage.tsx` | ✅ Thêm logic load booking info từ sessionStorage |

---

## 🎯 Kết Quả

- ✅ **Không còn lỗi 400** "Cart đã dùng"
- ✅ **Flow rõ ràng**: Booking → Cart → Checkout → Order
- ✅ **Booking info được lưu** và hiển thị ở checkout page
- ✅ **Checkout API chỉ gọi 1 lần** (khi user click "Đặt hàng")
- ✅ **Tương thích** với regular checkout flow (không booking)
