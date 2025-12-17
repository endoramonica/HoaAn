# Checkout Empty Cart Error - Problem Analysis & Solution

## Problem Description

**Error:** `400 Bad Request - "Cart is empty EMPTY_CART"`

**Symptoms:**
- Frontend shows cart has 1 item: `{cartId: 'b33aa0e4-3e32-49f9-9b49-e183631fc0d5', itemCount: 1}`
- Backend logs show cart retrieved successfully: `📦 Cart retrieved for user 48fdbb7b-9d91-4e41-872e-dd8296a0f315: 1 items`
- But checkout API returns "Cart is empty" error
- Checkout request is sent with correct cartId

## Root Cause Analysis

### Issue 1: Cart State Mismatch
The frontend cart state shows items, but the backend's checkout service is checking the cart and finding it empty. This suggests:

1. **Cart Items Not Properly Persisted**: The cart items might not be saved to the database when added
2. **Session/User Mismatch**: The cart belongs to a different session or user context
3. **Cart Cleared Between Requests**: The cart is cleared after being retrieved but before checkout processes it

### Issue 2: Backend Checkout Logic
Looking at the backend logs:
```
CheckoutService: ✅ Using CustomerId from JWT: d93a557c-a41e-4dd9-a0a7-d6b3897bc4a7
CheckoutService: ✅ Using CustomerId: d93a557c-a41e-4dd9-a0a7-d6b3897bc4a7
```

The checkout service is using CustomerId from JWT, but it might be checking the cart with a different context.

## Solution Strategy

### Step 1: Verify Cart Data Before Checkout

**File:** `src/pages/checkout/CheckoutPage.tsx`

Add detailed cart validation before sending checkout request:

```typescript
const handlePlaceOrder = async () => {
  try {
    // ✅ STEP 1: Validate cart exists and has items
    if (!cart) {
      toast.error('Không tìm thấy giỏ hàng');
      navigate('/cart');
      return;
    }

    if (!cart.id) {
      toast.error('Giỏ hàng không có ID. Vui lòng tải lại trang.');
      navigate('/cart');
      return;
    }

    if (!cart.items || cart.items.length === 0) {
      toast.error('Giỏ hàng trống. Vui lòng thêm sản phẩm.');
      navigate('/cart');
      return;
    }

    // ✅ STEP 2: Log cart details for debugging
    console.log('[CheckoutPage] 🔍 Cart validation:', {
      cartId: cart.id,
      itemCount: cart.items.length,
      items: cart.items.map(i => ({
        id: i.id,
        productId: i.productId,
        quantity: i.quantity,
        price: i.price
      })),
      totalAmount: cart.totalAmount
    });

    // ✅ STEP 3: Validate address
    if (!selectedAddressId) {
      toast.error('Vui lòng chọn địa chỉ giao hàng');
      return;
    }

    const selectedAddress = addresses.find(a => a.id === selectedAddressId);
    if (!selectedAddress) {
      toast.error('Địa chỉ không hợp lệ');
      return;
    }

    // ✅ STEP 4: Build checkout payload with validation
    const checkoutData: CheckoutDto = {
      cartId: cart.id,
      shippingInfo: {
        recipientName: selectedAddress.recipientName || 'Người nhận',
        phoneNumber: selectedAddress.phoneNumber,
        address: selectedAddress.streetAddress,
        ward: selectedAddress.state || 'Phường/Xã',
        district: selectedAddress.city || 'Quận/Huyện',
        city: selectedAddress.country || 'Vietnam',
        postalCode: selectedAddress.postalCode || null,
        deliveryNote: orderNote || null,
        shippingMethod: ShippingMethodEnum.standard,
      },
      paymentMethod: selectedPaymentMethod,
      couponCode: cart.couponCode || null,
      notes: orderNote || null,
    };

    // ✅ STEP 5: Log final payload
    console.log('[CheckoutPage] 📦 Final checkout payload:', {
      cartId: checkoutData.cartId,
      itemCount: cart.items.length,
      totalAmount: cart.totalAmount,
      shippingInfo: {
        recipientName: checkoutData.shippingInfo.recipientName,
        phoneNumber: checkoutData.shippingInfo.phoneNumber,
        address: checkoutData.shippingInfo.address
      }
    });

    // ✅ STEP 6: Process checkout
    const result = await processCheckout(checkoutData as any);

    if (!result) {
      console.error('[CheckoutPage] ❌ Checkout failed');
      toast.error('Không thể đặt hàng');
      return;
    }

    // ✅ STEP 7: Handle success
    onNavigate('success', { 
      orderId: result.orderId || '', 
      orderNumber: result.orderNumber || '' 
    });

  } catch (err: any) {
    console.error('[CheckoutPage] ❌ Error:', err);
    toast.error(err.message || 'Không thể đặt hàng');
  }
};
```

### Step 2: Ensure Cart Items Are Properly Saved

**File:** `src/lib/hooks/useCart.ts`

Add verification that cart items are actually in the response:

```typescript
const fetchCart = useCallback(async () => {
  try {
    const cartResponse = (isGuest
      ? await CartService.getApiV1CartGuest()
      : await CartService.getApiV1Cart()) as CartResponseDto;

    // ✅ Verify cart has items
    if (!cartResponse?.data?.items || cartResponse.data.items.length === 0) {
      console.warn('[useCart] ⚠️ Cart response has no items:', cartResponse);
      setCart(null);
      return;
    }

    // ✅ Log each item for verification
    console.log('[useCart] 📦 Cart items:', cartResponse.data.items.map(item => ({
      cartItemId: item.cartItemId,
      productId: item.productId,
      quantity: item.quantity,
      basePrice: item.basePrice,
      customizationPrice: item.customizationPrice,
      finalPrice: item.finalPrice
    })));

    // ... rest of transformation logic
  } catch (err: any) {
    // ... error handling
  }
}, [isGuest]);
```

### Step 3: Backend Verification (Backend Team)

The backend checkout service should:

1. **Verify cart exists** with the provided cartId
2. **Verify cart belongs to current user** (from JWT)
3. **Verify cart has items** before processing
4. **Log detailed error** if cart is empty

Example backend check:
```csharp
var cart = await _cartService.GetCartAsync(checkoutDto.CartId);

if (cart == null) {
  throw new InvalidOperationException("Cart not found");
}

if (cart.UserId != currentUserId) {
  throw new UnauthorizedAccessException("Cart does not belong to current user");
}

if (!cart.Items.Any()) {
  throw new InvalidOperationException("Cart is empty");
}
```

## Implementation Steps

### Frontend Fix (Immediate)

1. **Update CheckoutPage.tsx:**
   - Add comprehensive cart validation before checkout
   - Log cart details for debugging
   - Verify all required fields are present

2. **Update useCart.ts:**
   - Add verification that cart items exist in response
   - Log item details for debugging

3. **Update useCheckout.ts:**
   - Add more detailed error logging
   - Include cart details in error messages

### Testing Checklist

- [ ] Add product to cart
- [ ] Verify cart shows 1 item
- [ ] Go to checkout
- [ ] Verify cart data is logged correctly
- [ ] Click "Đặt hàng"
- [ ] Check browser console for cart details
- [ ] Check backend logs for cart retrieval
- [ ] Verify checkout succeeds

## Quick Fix Script

If the issue persists, try this debugging script in browser console:

```javascript
// Check cart state
const cartState = document.querySelector('[data-cart-id]');
console.log('Cart ID:', cartState?.dataset.cartId);

// Check localStorage
console.log('Auth Token:', localStorage.getItem('auth_token')?.substring(0, 20) + '...');
console.log('Guest Session:', localStorage.getItem('guest_cart_session_id'));

// Check if cart items are in DOM
const cartItems = document.querySelectorAll('[data-cart-item]');
console.log('Cart items in DOM:', cartItems.length);
```

## Expected Behavior After Fix

1. ✅ Cart validation passes
2. ✅ All required fields are present
3. ✅ Checkout request is sent with correct cartId
4. ✅ Backend finds cart with items
5. ✅ Order is created successfully
6. ✅ User is redirected to success page
