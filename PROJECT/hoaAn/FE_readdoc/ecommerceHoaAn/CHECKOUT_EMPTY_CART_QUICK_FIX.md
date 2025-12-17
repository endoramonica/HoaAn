# Checkout Empty Cart Error - Quick Fix Guide

## Problem
```
❌ 400 Bad Request: "Cart is empty EMPTY_CART"
Frontend: Cart shows 1 item ✓
Backend: Cart retrieved successfully ✓
Checkout: "Cart is empty" error ✗
```

## Root Cause
Backend's checkout service is checking the cart and finding it empty, even though:
- Frontend cart state shows items
- Backend cart retrieval shows items
- This suggests cart items are not properly persisted or there's a user/session mismatch

## Quick Diagnosis

### Step 1: Check Browser Console
After clicking "Đặt hàng", look for this log:
```
[CheckoutPage] 📦 Cart Details: {
  cartId: 'b33aa0e4-3e32-49f9-9b49-e183631fc0d5',
  itemCount: 1,
  items: [...]
}
```

**If you see this:** Cart data is correct on frontend ✓

### Step 2: Check Backend Logs
Look for:
```
CheckoutService: 🛒 Checkout started for user ...
CheckoutService: ✅ Using CustomerId from JWT: ...
```

**If you see this:** Backend received the request ✓

### Step 3: Identify the Issue

**If cart shows items on frontend but backend says empty:**
- Issue is in backend cart retrieval during checkout
- Backend is checking a different cart or user context

**If cart is empty on frontend:**
- Issue is in cart persistence
- Items are not being saved to database

## Solution Steps

### For Frontend Team

1. **Verify cart items are saved:**
   ```typescript
   // In CartPage.tsx, after adding item
   console.log('Cart items:', cartItems);
   console.log('Cart total:', summary?.total);
   ```

2. **Check cart before checkout:**
   ```typescript
   // In CheckoutPage.tsx
   if (!cart?.items?.length) {
     console.error('Cart is empty!');
     navigate('/cart');
     return;
   }
   ```

3. **Verify all required fields:**
   ```typescript
   const checkoutData = {
     cartId: cart.id,           // ✓ Must exist
     shippingInfo: {
       recipientName: '...',    // ✓ Must exist
       phoneNumber: '...',      // ✓ Must exist
       address: '...',          // ✓ Must exist
       ward: '...',             // ✓ Must exist
       district: '...',         // ✓ Must exist
       city: '...',             // ✓ Must exist
       shippingMethod: 'standard'
     },
     paymentMethod: 'cod'       // ✓ Must exist
   };
   ```

### For Backend Team

1. **Verify cart exists:**
   ```csharp
   var cart = await _cartService.GetCartAsync(checkoutDto.CartId);
   if (cart == null) {
     throw new InvalidOperationException("Cart not found");
   }
   ```

2. **Verify cart belongs to user:**
   ```csharp
   if (cart.UserId != currentUserId) {
     throw new UnauthorizedAccessException("Cart does not belong to user");
   }
   ```

3. **Verify cart has items:**
   ```csharp
   if (!cart.Items.Any()) {
     throw new InvalidOperationException("Cart has no items");
   }
   ```

4. **Log detailed info:**
   ```csharp
   _logger.LogInformation($"Cart {cartId}: {cart.Items.Count} items, UserId: {cart.UserId}");
   ```

## Testing Checklist

- [ ] Add product to cart
- [ ] Verify cart shows item count
- [ ] Go to checkout
- [ ] Check browser console for cart details
- [ ] Verify all fields are populated
- [ ] Click "Đặt hàng"
- [ ] Check backend logs for cart info
- [ ] Verify order is created

## Common Issues & Solutions

| Issue | Cause | Solution |
|-------|-------|----------|
| Cart empty on frontend | Items not saved | Check CartService.addItem() |
| Cart empty on backend | Wrong user context | Verify JWT token and userId |
| Cart empty after refresh | Session lost | Check sessionStorage/localStorage |
| Wrong cart retrieved | CartId mismatch | Verify cartId is passed correctly |

## Debug Commands

### Browser Console
```javascript
// Check cart state
console.log('Cart:', window.__CART_STATE__);

// Check auth token
console.log('Token:', localStorage.getItem('auth_token'));

// Check session
console.log('Session:', localStorage.getItem('guest_cart_session_id'));
```

### Backend (C#)
```csharp
// In CheckoutService
_logger.LogInformation($"Checkout Debug: CartId={checkoutDto.CartId}, UserId={userId}");
var cart = await _cartService.GetCartAsync(checkoutDto.CartId);
_logger.LogInformation($"Cart found: {cart != null}, Items: {cart?.Items?.Count ?? 0}");
```

## Next Steps

1. **Implement the fixes** in CheckoutPage.tsx and useCheckout.ts
2. **Add detailed logging** in backend checkout service
3. **Test the flow** with a fresh cart
4. **Monitor logs** during checkout process
5. **Verify order creation** succeeds

## Expected Result After Fix

✅ Cart validation passes on frontend
✅ All required fields are present
✅ Checkout request sent with correct cartId
✅ Backend finds cart with items
✅ Order created successfully
✅ User redirected to success page
