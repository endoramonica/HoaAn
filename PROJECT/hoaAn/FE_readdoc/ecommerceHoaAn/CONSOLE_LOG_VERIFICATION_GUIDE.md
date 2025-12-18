# 🔍 CONSOLE LOG VERIFICATION GUIDE

## 📊 EXPECTED CONSOLE LOGS AFTER FIX

### Frontend - CheckoutPage Flow

#### Before Checkout
```
[useCart] 🔍 Fetching cart...
[useCart] 📋 Auth Token: undefined...
[useCart] 👤 Is Guest: false
[useCart] ✅ Cart Response: {cartId: 'b33aa0e4-3e32-49f9-9b49-e183631fc0d5', ...}
[useCart] ✅ Cart loaded successfully
[CheckoutPage] 📅 Booking info loaded from sessionStorage: {eventId: '...', cartId: '...'}
```

#### During Checkout
```
[CheckoutPage] 🔄 Refreshing cart before checkout...
[CheckoutPage] ✅ Cart refreshed: {itemCount: 1, items: 1}
[CheckoutPage] 🔍 Validating cart...
[CheckoutPage] 📦 Cart Details: {cartId: 'b33aa0e4-3e32-49f9-9b49-e183631fc0d5', itemCount: 1, totalAmount: 1100000}
[CheckoutPage] ✅ Cart valid: {cartId: 'b33aa0e4-3e32-49f9-9b49-e183631fc0d5', itemCount: 1, totalAmount: 1100000}
[CheckoutPage] 🔍 Validating address...
[CheckoutPage] ✅ Address valid
[CheckoutPage] 📦 Preparing checkout data...
[CheckoutPage] 🔍 Validating checkout payload...
[CheckoutPage] ✅ Checkout data prepared: {cartId: 'b33aa0e4-3e32-49f9-9b49-e183631fc0d5', itemCount: 1, totalAmount: 1100000}
[CheckoutPage] 🛒 Processing checkout...
[useCheckout] 🛒 Processing checkout...
[useCheckout] 📦 Checkout Payload: {cartId: 'b33aa0e4-3e32-49f9-9b49-e183631fc0d5', paymentMethod: 'cod', ...}
```

#### After Checkout (NEW - CRITICAL FIX)
```
[useCheckout] ✅ Checkout successful: {orderId: '0b539155-8744-4ed1-a48a-93f79e510c6f', orderNumber: '251217144200227SDP', totalAmount: 1130000}
[CheckoutPage] ✅ Checkout successful: {orderId: '0b539155-8744-4ed1-a48a-93f79e510c6f', orderNumber: '251217144200227SDP', totalAmount: 1130000}
[CheckoutPage] 🧹 Clearing booking state...
[CheckoutPage] ✅ Booking state cleared
[CheckoutPage] 💳 Handling payment method: cod
```

### Frontend - OrderSuccessPage Flow

#### Page Load (NEW - BACKUP CLEAR)
```
[OrderSuccessPage] 📋 Loading order details...
[OrderSuccessPage] 🧹 Clearing booking state (backup)...
[OrderSuccessPage] ✅ Booking state cleared
[OrderSuccessPage] 🔍 Fetching order: 0b539155-8744-4ed1-a48a-93f79e510c6f
[useCheckout] 📋 Fetching order details: 0b539155-8744-4ed1-a48a-93f79e510c6f
[OrderSuccessPage] ✅ Order loaded: {orderId: '0b539155-8744-4ed1-a48a-93f79e510c6f', orderNumber: '251217144200227SDP', status: 'pending'}
```

---

## 🧪 VERIFICATION STEPS

### Step 1: Open DevTools
```
1. Open browser
2. Press F12 to open DevTools
3. Go to Console tab
4. Clear console (Ctrl+L or click clear button)
```

### Step 2: Add Product to Cart
```
1. Go to Products page
2. Add product (price: 1,100,000₫) to cart
3. Check console for cart loading logs
```

### Step 3: Go to Calendar Booking
```
1. Click "Đặt lịch tư vấn" or similar
2. Select booking slot
3. Check console for booking info logs
```

### Step 4: Go to Checkout
```
1. Click "Thanh toán" or "Checkout"
2. Check console for:
   - [CheckoutPage] 📅 Booking info loaded from sessionStorage ✅
   - [CheckoutPage] 📦 Cart Details: ... ✅
```

### Step 5: Click "Đặt hàng"
```
1. Click "Đặt hàng" button
2. Wait for checkout to complete
3. Check console for:
   - [useCheckout] ✅ Checkout successful ✅
   - [CheckoutPage] 🧹 Clearing booking state... ✅
   - [CheckoutPage] ✅ Booking state cleared ✅
```

### Step 6: Verify OrderSuccessPage
```
1. Wait for OrderSuccessPage to load
2. Check console for:
   - [OrderSuccessPage] 🧹 Clearing booking state (backup)... ✅
   - [OrderSuccessPage] ✅ Booking state cleared ✅
   - [OrderSuccessPage] ✅ Order loaded ✅
```

### Step 7: Verify Session Storage
```
1. Open DevTools → Application → Session Storage
2. Check for:
   - bookingInfo: ❌ SHOULD NOT EXIST
   - selectedBookingSlot: ❌ SHOULD NOT EXIST
   - bookingInfoClient: ❌ SHOULD NOT EXIST
```

---

## 🔴 TROUBLESHOOTING

### Issue: Booking State Still in Session Storage
```
❌ Problem: bookingInfo still exists after checkout
✅ Solution:
   1. Check console for [CheckoutPage] 🧹 Clearing booking state...
   2. If NOT present → CheckoutPage fix not applied
   3. Check console for [OrderSuccessPage] 🧹 Clearing booking state (backup)...
   4. If NOT present → OrderSuccessPage fix not applied
   5. Verify code changes are deployed
```

### Issue: Order Price Still Wrong
```
❌ Problem: Order subTotal = 0, only shippingFee visible
✅ Solution:
   1. Check console for [useCheckout] 📦 Checkout Payload
   2. Verify cartId is correct
   3. Check backend logs for order creation
   4. Verify backend fix is deployed
   5. Check POST /Checkout/process response
```

### Issue: Cart Still ACTIVE
```
❌ Problem: GET /Cart/summary returns status: ACTIVE
✅ Solution:
   1. Check backend logs for [Checkout] Cart ... closed after checkout
   2. If NOT present → backend fix not applied
   3. Verify CartStatus.CHECKED_OUT enum exists
   4. Verify backend code changes are deployed
```

---

## 📊 EXPECTED CONSOLE OUTPUT (FULL FLOW)

### Complete Successful Checkout Flow
```
=== BEFORE CHECKOUT ===
[useCart] 🔍 Fetching cart...
[useCart] ✅ Cart Response: {cartId: 'b33aa0e4-3e32-49f9-9b49-e183631fc0d5', itemCount: 1}
[useCart] ✅ Cart loaded successfully
[CheckoutPage] 📅 Booking info loaded from sessionStorage: {eventId: 'event-2025-12-17T17:00:00.000Z', cartId: 'b33aa0e4-3e32-49f9-9b49-e183631fc0d5'}

=== DURING CHECKOUT ===
[CheckoutPage] 🔄 Refreshing cart before checkout...
[CheckoutPage] ✅ Cart refreshed: {itemCount: 1, items: 1}
[CheckoutPage] 🔍 Validating cart...
[CheckoutPage] 📦 Cart Details: {cartId: 'b33aa0e4-3e32-49f9-9b49-e183631fc0d5', itemCount: 1, totalAmount: 1100000}
[CheckoutPage] ✅ Cart valid: {cartId: 'b33aa0e4-3e32-49f9-9b49-e183631fc0d5', itemCount: 1, totalAmount: 1100000}
[CheckoutPage] 🔍 Validating address...
[CheckoutPage] ✅ Address valid
[CheckoutPage] 📦 Preparing checkout data...
[CheckoutPage] ✅ Checkout data prepared: {cartId: 'b33aa0e4-3e32-49f9-9b49-e183631fc0d5', itemCount: 1, totalAmount: 1100000}
[CheckoutPage] 🛒 Processing checkout...
[useCheckout] 🛒 Processing checkout...
[useCheckout] 📦 Checkout Payload: {cartId: 'b33aa0e4-3e32-49f9-9b49-e183631fc0d5', paymentMethod: 'cod', shippingInfo: {...}}

=== AFTER CHECKOUT (NEW - CRITICAL FIX) ===
[useCheckout] ✅ Checkout successful: {orderId: '0b539155-8744-4ed1-a48a-93f79e510c6f', orderNumber: '251217144200227SDP', totalAmount: 1130000}
[CheckoutPage] ✅ Checkout successful: {orderId: '0b539155-8744-4ed1-a48a-93f79e510c6f', orderNumber: '251217144200227SDP', totalAmount: 1130000}
[CheckoutPage] 🧹 Clearing booking state...
[CheckoutPage] ✅ Booking state cleared
[CheckoutPage] 💳 Handling payment method: cod

=== ORDER SUCCESS PAGE (NEW - BACKUP CLEAR) ===
[OrderSuccessPage] 📋 Loading order details...
[OrderSuccessPage] 🧹 Clearing booking state (backup)...
[OrderSuccessPage] ✅ Booking state cleared
[OrderSuccessPage] 🔍 Fetching order: 0b539155-8744-4ed1-a48a-93f79e510c6f
[useCheckout] 📋 Fetching order details: 0b539155-8744-4ed1-a48a-93f79e510c6f
[OrderSuccessPage] ✅ Order loaded: {orderId: '0b539155-8744-4ed1-a48a-93f79e510c6f', orderNumber: '251217144200227SDP', status: 'pending'}
```

---

## ✅ VERIFICATION CHECKLIST

### Console Logs
- [ ] [CheckoutPage] 🧹 Clearing booking state... appears
- [ ] [CheckoutPage] ✅ Booking state cleared appears
- [ ] [OrderSuccessPage] 🧹 Clearing booking state (backup)... appears
- [ ] [OrderSuccessPage] ✅ Booking state cleared appears
- [ ] No errors in console

### Session Storage
- [ ] bookingInfo NOT in sessionStorage after checkout
- [ ] selectedBookingSlot NOT in sessionStorage after checkout
- [ ] bookingInfoClient NOT in sessionStorage after checkout

### API Responses
- [ ] POST /Checkout/process returns correct totalAmount
- [ ] GET /Cart/summary returns status: CHECKED_OUT (after backend fix)
- [ ] GET /Checkout/{orderId} returns correct prices

### User Experience
- [ ] Checkout completes successfully
- [ ] OrderSuccessPage loads without errors
- [ ] Can add new product after checkout
- [ ] New product checkout doesn't include old booking info

---

## 🚀 PRODUCTION VERIFICATION

### Day 1 After Deployment
```
✅ Monitor console logs for errors
✅ Check for [CheckoutPage] 🧹 Clearing booking state...
✅ Check for [OrderSuccessPage] 🧹 Clearing booking state (backup)...
✅ Monitor customer complaints
```

### Week 1 After Deployment
```
✅ Verify no booking state sticking issues
✅ Verify order prices are correct
✅ Verify cart closes properly
✅ Check backend logs for cart closing
```

### Ongoing
```
✅ Monitor production logs
✅ Watch for any regressions
✅ Verify fix is working as expected
```

