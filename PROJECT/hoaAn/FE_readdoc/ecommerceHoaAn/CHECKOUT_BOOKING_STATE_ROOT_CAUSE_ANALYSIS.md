# 🔴 ROOT CAUSE ANALYSIS: Checkout Booking State Issue

## I. TRIỆU CHỨNG QUAN SÁT

### 1️⃣ Booking State Không Được Clear
```
[CheckoutPage] 📅 Booking info loaded from sessionStorage: {
  eventId: 'event-2025-12-17T17:00:00.000Z',
  cartId: 'b33aa0e4-3e32-49f9-9b49-e183631fc0d5',
  serviceProductId: '125c1031-6df9-4bd2-b7f2-dfcc5cac55e8',
  ...
}
```
**Vấn đề:** Sau khi checkout xong, `bookingInfo` vẫn còn trong `sessionStorage`

### 2️⃣ Cart Vẫn ACTIVE Sau Checkout
```
GET /Cart/summary Response:
{
  "status": "ACTIVE",  // ❌ Phải là CHECKED_OUT hoặc COMPLETED
  "itemCount": 1,
  "subTotal": 1100000.00,
  "totalAmount": 1100000.00
}
```
**Vấn đề:** Cart không bị đóng/clear sau checkout

### 3️⃣ Order Giá Bị Lệch
```
Cart trước checkout:
- unitPrice: 1100000.00
- totalAmount: 1100000.00

Order sau checkout:
- unitPrice: 0.00  ❌
- subTotal: 0.00   ❌
- totalAmount: 30000.00 (chỉ có shippingFee)
```
**Vấn đề:** Dữ liệu giá không được truyền đúng từ cart sang order

---

## II. NGUYÊN NHÂN GỐC (ROOT CAUSES)

### 🔥 ROOT CAUSE #1: OrderSuccessPage Không Clear Booking State

**Vị trí:** `src/pages/checkout/OrderSuccessPage.tsx`

**Hiện tại:**
```typescript
useEffect(() => {
  loadOrderDetails();
}, []);

const loadOrderDetails = async () => {
  // ... load order details
  
  // ✅ Clear pending order
  sessionStorage.removeItem('pendingOrderId');
  sessionStorage.removeItem('pendingOrderNumber');
  
  // ❌ MISSING: Clear booking state!
  // sessionStorage.removeItem('bookingInfo');
  // sessionStorage.removeItem('selectedBookingSlot');
};
```

**Hậu quả:**
- `bookingInfo` vẫn trong sessionStorage
- Khi user thêm sản phẩm mới → cart vẫn gắn với booking cũ
- Booking state bị "dính" vào cart mới

---

### 🔥 ROOT CAUSE #2: CheckoutPage Không Clear Booking State Sau Checkout

**Vị trí:** `src/pages/checkout/CheckoutPage.tsx` - `handlePlaceOrder()`

**Hiện tại:**
```typescript
const handlePlaceOrder = async () => {
  // ... validate cart, address, payment
  
  const result = await processCheckout(checkoutData);
  
  if (result) {
    // ✅ Navigate to success
    onNavigate('success', { 
      orderId: result.orderId,
      orderNumber: result.orderNumber 
    });
    
    // ❌ MISSING: Clear booking state before navigate!
    // sessionStorage.removeItem('bookingInfo');
    // sessionStorage.removeItem('selectedBookingSlot');
  }
};
```

**Hậu quả:**
- Booking state vẫn tồn tại khi navigate đến OrderSuccessPage
- OrderSuccessPage không biết cần clear nó

---

### 🔥 ROOT CAUSE #3: Backend Không Đóng/Clear Cart Sau Checkout

**Vị trí:** Backend `CheckoutService.Process()` (không có trong workspace)

**Dấu hiệu từ API Response:**
```
POST /Checkout/process Response:
{
  "orderId": "0b539155-8744-4ed1-a48a-93f79e510c6f",
  "totalAmount": 30000.000,  // ❌ Chỉ có shippingFee
  ...
}

GET /Cart/summary Response (sau checkout):
{
  "status": "ACTIVE",  // ❌ Vẫn ACTIVE
  "itemCount": 1,
  "subTotal": 1100000.00,
  "totalAmount": 1100000.00
}
```

**Vấn đề:**
- Backend không set `cart.Status = CHECKED_OUT`
- Backend không clear `cart.Items`
- Backend không save cart vào database
- Order được tạo nhưng cart vẫn ACTIVE

---

### 🔥 ROOT CAUSE #4: Order Items Giá Bị Lệch

**Vị trí:** Backend `CheckoutService.Process()` - Mapping cart items → order items

**Dấu hiệu:**
```
Cart Item:
{
  "unitPrice": 1100000.00,
  "quantity": 1,
  "totalPrice": 1100000.00
}

Order Item:
{
  "unitPrice": 0.00,  // ❌ Mất giá
  "quantity": 1,
  "totalPrice": 0.00
}

Order Summary:
{
  "subTotal": 0.00,  // ❌ Tính từ items
  "shippingFee": 30000.00,
  "totalAmount": 30000.00  // ❌ Chỉ có shipping
}
```

**Vấn đề:**
- Backend không copy `unitPrice` từ cart item sang order item
- Order items được tạo với `unitPrice = 0`
- Order `subTotal` được tính từ items → kết quả = 0

---

## III. FLOW HIỆN TẠI (BROKEN)

```
1. User tại Calendar Booking Page
   └─ sessionStorage.setItem('bookingInfo', {...})
   └─ sessionStorage.setItem('selectedBookingSlot', {...})

2. User navigate đến CheckoutPage
   └─ useEffect: Load bookingInfo từ sessionStorage ✅
   └─ Pre-fill order note với booking details ✅

3. User click "Đặt hàng"
   └─ POST /Checkout/process
      ├─ Backend: Tạo Order (nhưng items.unitPrice = 0) ❌
      ├─ Backend: Cart vẫn ACTIVE ❌
      └─ Response: orderId, orderNumber

4. Frontend: onNavigate('success', { orderId, orderNumber })
   └─ Navigate đến OrderSuccessPage
   └─ ❌ bookingInfo vẫn trong sessionStorage
   └─ ❌ selectedBookingSlot vẫn trong sessionStorage

5. OrderSuccessPage: loadOrderDetails()
   └─ GET /Checkout/{orderId}
   └─ ❌ Không clear bookingInfo
   └─ ❌ Không clear selectedBookingSlot

6. User click "Tiếp tục mua sắm" hoặc thêm sản phẩm mới
   └─ useCart hook load cart
   └─ ❌ bookingInfo vẫn trong sessionStorage
   └─ ❌ Booking state bị "dính" vào cart mới
   └─ ❌ Khi checkout lần 2 → booking cũ vẫn được gửi
```

---

## IV. FLOW MONG MUỐN (FIXED)

```
1. User tại Calendar Booking Page
   └─ sessionStorage.setItem('bookingInfo', {...})
   └─ sessionStorage.setItem('selectedBookingSlot', {...})

2. User navigate đến CheckoutPage
   └─ useEffect: Load bookingInfo từ sessionStorage ✅
   └─ Pre-fill order note với booking details ✅

3. User click "Đặt hàng"
   └─ POST /Checkout/process
      ├─ Backend: Tạo Order với items.unitPrice = cart.unitPrice ✅
      ├─ Backend: Set cart.Status = CHECKED_OUT ✅
      ├─ Backend: Clear cart.Items ✅
      ├─ Backend: Save cart ✅
      └─ Response: orderId, orderNumber

4. Frontend: handlePlaceOrder() - BEFORE navigate
   ├─ ✅ sessionStorage.removeItem('bookingInfo')
   ├─ ✅ sessionStorage.removeItem('selectedBookingSlot')
   └─ onNavigate('success', { orderId, orderNumber })

5. Navigate đến OrderSuccessPage
   └─ bookingInfo ✅ CLEARED
   └─ selectedBookingSlot ✅ CLEARED

6. OrderSuccessPage: loadOrderDetails()
   └─ GET /Checkout/{orderId}
   └─ Order có đúng giá ✅
   └─ Không cần clear (đã clear ở step 4)

7. User click "Tiếp tục mua sắm" hoặc thêm sản phẩm mới
   └─ useCart hook load cart
   └─ ✅ bookingInfo KHÔNG còn
   └─ ✅ selectedBookingSlot KHÔNG còn
   └─ ✅ Cart mới là clean, không bị "dính" booking cũ
```

---

## V. VẤN ĐỀ CHI TIẾT

### Vấn đề #1: Booking State Không Được Clear
- **Nơi xảy ra:** Frontend - OrderSuccessPage
- **Nguyên nhân:** Không có code clear `bookingInfo` từ sessionStorage
- **Hậu quả:** Booking state bị "dính" vào cart mới

### Vấn đề #2: Cart Không Được Đóng
- **Nơi xảy ra:** Backend - CheckoutService.Process()
- **Nguyên nhân:** Không set `cart.Status = CHECKED_OUT` hoặc `COMPLETED`
- **Hậu quả:** Cart vẫn ACTIVE, có thể reuse lại

### Vấn đề #3: Order Items Giá Bị Lệch
- **Nơi xảy ra:** Backend - CheckoutService.Process() - Mapping items
- **Nguyên nhân:** Không copy `unitPrice` từ cart item sang order item
- **Hậu quả:** Order có `subTotal = 0`, chỉ có `shippingFee`

### Vấn đề #4: Timing Issue
- **Nơi xảy ra:** Frontend - CheckoutPage.handlePlaceOrder()
- **Nguyên nhân:** Clear booking state sau navigate (quá muộn)
- **Hậu quả:** OrderSuccessPage có thể load với booking state cũ

---

## VI. SOLUTION ROADMAP

### Phase 1: Frontend - Clear Booking State (CRITICAL)
**File:** `src/pages/checkout/CheckoutPage.tsx`
- Thêm code clear `bookingInfo` + `selectedBookingSlot` TRƯỚC khi navigate
- Thêm code clear trong OrderSuccessPage (backup)

### Phase 2: Backend - Close Cart After Checkout (CRITICAL)
**File:** Backend `CheckoutService.cs` - `Process()` method
- Set `cart.Status = "CHECKED_OUT"` hoặc `"COMPLETED"`
- Clear `cart.Items` hoặc archive cart
- Save cart vào database

### Phase 3: Backend - Fix Order Items Price (CRITICAL)
**File:** Backend `CheckoutService.cs` - Mapping logic
- Copy `unitPrice` từ cart item sang order item
- Verify `order.subTotal` được tính đúng từ items

### Phase 4: Frontend - Verify Cart Status (OPTIONAL)
**File:** `src/lib/hooks/useCart.ts`
- Thêm check: Nếu cart status = CHECKED_OUT → tạo cart mới
- Prevent reuse cart đã closed

---

## VII. VERIFICATION CHECKLIST

### Sau khi fix, verify:
- [ ] Checkout thành công → Order có đúng giá
- [ ] Checkout thành công → `bookingInfo` KHÔNG còn trong sessionStorage
- [ ] Checkout thành công → `selectedBookingSlot` KHÔNG còn trong sessionStorage
- [ ] Checkout thành công → Cart status = CHECKED_OUT (hoặc COMPLETED)
- [ ] Thêm sản phẩm mới → Booking state KHÔNG bị "dính"
- [ ] Checkout lần 2 → Booking cũ KHÔNG được gửi
- [ ] GET /Cart/summary → Trả về cart mới (empty hoặc new items)

