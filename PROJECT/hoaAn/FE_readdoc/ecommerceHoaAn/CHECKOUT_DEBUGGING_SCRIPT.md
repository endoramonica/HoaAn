# Checkout Empty Cart - Debugging & Testing Script

## Problem Summary
```
Frontend: ✓ Cart has 1 item
Backend: ✓ Cart retrieved successfully  
Checkout: ✗ "Cart is empty" error
```

## Debugging Flow

### Phase 1: Frontend Validation

**What to check:**
1. Cart state has items
2. Cart ID is valid
3. All required fields are present

**Browser Console Commands:**
```javascript
// 1. Check if cart has items
const cartState = JSON.parse(sessionStorage.getItem('cart') || '{}');
console.log('Cart State:', cartState);
console.log('Has items:', cartState.items?.length > 0);

// 2. Check auth token
const token = localStorage.getItem('auth_token') || sessionStorage.getItem('auth_token');
console.log('Auth Token exists:', !!token);
console.log('Token preview:', token?.substring(0, 50) + '...');

// 3. Check guest session
const guestSession = localStorage.getItem('guest_cart_session_id');
console.log('Guest Session:', guestSession);

// 4. Decode JWT to check userId
function decodeJWT(token) {
  const base64Url = token.split('.')[1];
  const base64 = base64Url.replace(/-/g, '+').replace(/_/g, '/');
  const jsonPayload = decodeURIComponent(atob(base64).split('').map(c => 
    '%' + ('00' + c.charCodeAt(0).toString(16)).slice(-2)
  ).join(''));
  return JSON.parse(jsonPayload);
}

const decoded = decodeJWT(token);
console.log('JWT Decoded:', {
  userId: decoded.sub,
  email: decoded.email,
  customerId: decoded.customerId
});
```

### Phase 2: Network Request Inspection

**What to check:**
1. Checkout request is sent with correct cartId
2. Request headers include auth token
3. Request body has all required fields

**Steps:**
1. Open DevTools → Network tab
2. Click "Đặt hàng" button
3. Look for `POST /api/v1/Checkout/process`
4. Check Request Headers:
   ```
   Authorization: Bearer <token>
   Content-Type: application/json
   ```
5. Check Request Body:
   ```json
   {
     "cartId": "b33aa0e4-3e32-49f9-9b49-e183631fc0d5",
     "shippingInfo": {
       "recipientName": "...",
       "phoneNumber": "...",
       "address": "...",
       "ward": "...",
       "district": "...",
       "city": "...",
       "shippingMethod": "standard"
     },
     "paymentMethod": "cod"
   }
   ```
6. Check Response:
   ```json
   {
     "success": false,
     "message": "Cart is empty EMPTY_CART",
     "errors": ["Cart is empty EMPTY_CART"]
   }
   ```

### Phase 3: Backend Verification

**What to check:**
1. Backend receives the request
2. Backend retrieves the cart
3. Backend finds items in cart

**Backend Logs to Look For:**
```
[CheckoutController] Processing checkout for cart b33aa0e4-3e32-49f9-9b49-e183631fc0d5
[CheckoutService] 🛒 Checkout started for user 48fdbb7b-9d91-4e41-872e-dd8296a0f315
[CheckoutService] ✅ Using CustomerId from JWT: d93a557c-a41e-4dd9-a0a7-d6b3897bc4a7
[CartService] 📦 Cart retrieved for user 48fdbb7b-9d91-4e41-872e-dd8296a0f315: 1 items
```

**If you see these logs:** Backend is processing correctly ✓

**If you DON'T see cart retrieval log:** Backend is not retrieving cart ✗

## Root Cause Identification

### Scenario 1: Frontend shows items, Backend says empty

**Diagnosis:**
```
Frontend Cart: {
  cartId: 'b33aa0e4-3e32-49f9-9b49-e183631fc0d5',
  items: [{ id: '...', quantity: 1 }]
}

Backend Cart: {
  cartId: 'b33aa0e4-3e32-49f9-9b49-e183631fc0d5',
  items: []  // ← EMPTY!
}
```

**Possible Causes:**
1. Items not saved to database when added to cart
2. Items cleared before checkout
3. Different user context between requests

**Fix:**
- Check CartService.addItem() saves to database
- Verify userId is consistent
- Check if cart is cleared somewhere

### Scenario 2: Frontend shows empty, Backend shows items

**Diagnosis:**
```
Frontend Cart: {
  items: []  // ← EMPTY!
}

Backend Cart: {
  items: [{ id: '...', quantity: 1 }]
}
```

**Possible Causes:**
1. Frontend cart state not updated after adding item
2. useCart hook not refreshing
3. Cart data not being fetched

**Fix:**
- Check useCart.ts fetchCart() is called
- Verify cart response is being processed
- Check for state update issues

### Scenario 3: Both show items, but checkout still fails

**Diagnosis:**
```
Frontend: ✓ Items present
Backend: ✓ Items present
Checkout: ✗ Still fails
```

**Possible Causes:**
1. Missing required fields in checkout payload
2. Invalid field values
3. Backend validation error

**Fix:**
- Verify all required fields are present
- Check field values are valid
- Review backend validation logic

## Testing Script

### Test 1: Add Item to Cart
```typescript
// In ProductDetailPage or similar
const handleAddToCart = async () => {
  console.log('[TEST] Adding item to cart...');
  
  try {
    await addToCart({
      productId: 'product-id',
      quantity: 1
    });
    
    console.log('[TEST] ✓ Item added');
    
    // Verify cart was updated
    const cart = await cartService.getCart();
    console.log('[TEST] Cart after add:', {
      itemCount: cart.data.totalItems,
      items: cart.data.items.length
    });
    
  } catch (err) {
    console.error('[TEST] ✗ Failed to add item:', err);
  }
};
```

### Test 2: Verify Cart Before Checkout
```typescript
// In CheckoutPage
const handlePlaceOrder = async () => {
  console.log('[TEST] Starting checkout...');
  
  // Log cart state
  console.log('[TEST] Cart state:', {
    exists: !!cart,
    hasId: !!cart?.id,
    itemCount: cart?.items?.length,
    totalAmount: cart?.totalAmount
  });
  
  // Log each item
  if (cart?.items) {
    cart.items.forEach((item, idx) => {
      console.log(`[TEST] Item ${idx}:`, {
        id: item.id,
        productId: item.productId,
        quantity: item.quantity,
        price: item.price
      });
    });
  }
  
  // Log checkout payload
  const payload = {
    cartId: cart?.id,
    shippingInfo: { /* ... */ },
    paymentMethod: 'cod'
  };
  console.log('[TEST] Checkout payload:', payload);
  
  // Proceed with checkout
  const result = await processCheckout(payload);
  console.log('[TEST] Checkout result:', result);
};
```

### Test 3: Verify Backend Cart Retrieval
```csharp
// In CheckoutService.cs
public async Task<OrderDetailDto> ProcessCheckoutAsync(CheckoutDto checkoutDto)
{
    _logger.LogInformation($"[TEST] Checkout started");
    _logger.LogInformation($"[TEST] CartId: {checkoutDto.CartId}");
    _logger.LogInformation($"[TEST] UserId: {_currentUser.Id}");
    
    // Retrieve cart
    var cart = await _cartService.GetCartAsync(checkoutDto.CartId);
    
    _logger.LogInformation($"[TEST] Cart found: {cart != null}");
    _logger.LogInformation($"[TEST] Cart items: {cart?.Items?.Count ?? 0}");
    
    if (cart?.Items?.Any() == true)
    {
        foreach (var item in cart.Items)
        {
            _logger.LogInformation($"[TEST] Item: {item.ProductId}, Qty: {item.Quantity}");
        }
    }
    
    // Verify cart is not empty
    if (!cart?.Items?.Any() == true)
    {
        _logger.LogError("[TEST] Cart is empty!");
        throw new InvalidOperationException("Cart is empty");
    }
    
    // Continue with checkout...
}
```

## Expected Console Output

### Successful Flow
```
[CheckoutPage] 🔍 Validating cart...
[CheckoutPage] 📦 Cart Details: {
  cartId: 'b33aa0e4-3e32-49f9-9b49-e183631fc0d5',
  itemCount: 1,
  items: [{
    id: 'c14f2c29-3b74-476e-9d8f-b57b1b811bae',
    productId: '11d63acd-24f3-4e85-b061-6494b62902bb',
    quantity: 1,
    price: 5500000
  }]
}
[CheckoutPage] ✅ Cart valid
[CheckoutPage] 📦 Preparing checkout data...
[CheckoutPage] ✅ Checkout data prepared
[useCheckout] 🛒 Processing checkout...
[useCheckout] ✅ Checkout successful
```

### Failed Flow
```
[CheckoutPage] 🔍 Validating cart...
[CheckoutPage] 📦 Cart Details: {
  cartId: 'b33aa0e4-3e32-49f9-9b49-e183631fc0d5',
  itemCount: 1,
  items: [...]
}
[CheckoutPage] ✅ Cart valid
[useCheckout] 🛒 Processing checkout...
[useCheckout] ❌ Process checkout failed: {
  message: 'Request failed with status code 400',
  status: 400,
  data: {
    success: false,
    message: 'Cart is empty EMPTY_CART'
  }
}
[useCheckout] 🔴 EMPTY_CART Error
[useCheckout] 💡 Possible causes:
  1. Cart items were not saved to database
  2. Cart belongs to different user/session
  3. Cart was cleared between requests
```

## Next Steps

1. **Run Test 1:** Verify item is added to cart
2. **Run Test 2:** Verify cart state before checkout
3. **Run Test 3:** Verify backend receives cart with items
4. **Compare outputs** with expected results
5. **Identify** which phase is failing
6. **Fix** the identified issue
7. **Retest** the entire flow
