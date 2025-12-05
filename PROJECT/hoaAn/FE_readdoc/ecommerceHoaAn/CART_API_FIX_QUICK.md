# Quick Fix for EMPTY_CART Error

## 🎯 Problem
useCart uses OLD API, useCheckout uses NEW API → Different sessions → EMPTY_CART

## ⚡ Quick Fix: Add Debug Logging

### Step 1: Add Logging to useCart

```typescript
// src/lib/hooks/useCart.ts

const fetchCart = useCallback(async () => {
  // ... existing code ...
  
  try {
    // ✅ ADD THIS: Log before API call
    console.log('[useCart] 🔍 Fetching cart...');
    console.log('[useCart] 📋 Auth Token:', localStorage.getItem('auth_token')?.substring(0, 20) + '...');
    console.log('[useCart] 👤 Is Guest:', isGuest);
    
    const cartResponse = isGuest
      ? await CartService.getApiV1CartGuest()
      : await CartService.getApiV1Cart();

    // ✅ ADD THIS: Log response
    console.log('[useCart] ✅ Cart Response:', {
      cartId: cartResponse?.data?.cartId,
      userId: cartResponse?.data?.userId,
      itemCount: cartResponse?.data?.totalItems
    });
    
    // ... rest of code ...
  } catch (err) {
    // ... error handling ...
  }
}, [isGuest]);
```

### Step 2: Add Logging to CheckoutPage

```typescript
// src/pages/checkout/CheckoutPage.tsx

const handlePlaceOrder = async () => {
  try {
    // ... existing validation ...
    
    // ✅ ADD THIS: Log before checkout
    console.log('[CheckoutPage] 🔍 Pre-checkout verification:');
    console.log('[CheckoutPage] 📋 Auth Token:', localStorage.getItem('auth_token')?.substring(0, 20) + '...');
    console.log('[CheckoutPage] 🛒 Cart from useCart:', {
      cartId: cart?.id,
      itemCount: cart?.items?.length,
      totalAmount: cart?.totalAmount
    });
    
    // ✅ ADD THIS: Verify cart one more time before checkout
    console.log('[CheckoutPage] 🔄 Refreshing cart before checkout...');
    await refreshCart();
    
    // Small delay to ensure state update
    await new Promise(resolve => setTimeout(resolve, 200));
    
    // Re-check cart after refresh
    if (!cart || !cart.items || cart.items.length === 0) {
      console.error('[CheckoutPage] ❌ Cart empty after refresh!');
      toast.error('Giỏ hàng trống sau khi làm mới. Vui lòng thử lại.');
      navigate('/cart');
      return;
    }
    
    console.log('[CheckoutPage] ✅ Cart verified after refresh:', {
      cartId: cart.id,
      itemCount: cart.items.length
    });
    
    // ... proceed with checkout ...
  } catch (err) {
    // ... error handling ...
  }
};
```

### Step 3: Compare Network Requests

In DevTools Network tab, compare these requests:

**Request 1: GET /Cart** (from useCart)
```
Headers:
  Authorization: Bearer xxx...
  Cookie: session_id=yyy...
```

**Request 2: POST /Checkout/process** (from useCheckout)
```
Headers:
  Authorization: Bearer xxx...
  Cookie: session_id=yyy...
```

**Check if:**
- ✅ Authorization tokens are SAME
- ✅ Cookie session_id are SAME
- ❌ If different → That's the problem!

## 🔧 If Tokens/Sessions are Different

### Fix: Ensure Same Axios Instance

Create shared API client:

```typescript
// src/lib/api/shared-client.ts
import axios from 'axios';
import { tokenStorage } from './client';

export const sharedApiClient = axios.create({
  baseURL: import.meta.env.VITE_API_BASE_URL || 'https://localhost:7777/api/v1',
  headers: {
    'Content-Type': 'application/json',
  },
  withCredentials: true, // ✅ Important for cookies
});

// Add token interceptor
sharedApiClient.interceptors.request.use((config) => {
  const token = tokenStorage.getAccessToken();
  if (token) {
    config.headers.Authorization = `Bearer ${token}`;
  }
  return config;
});

// Add response interceptor for token refresh
sharedApiClient.interceptors.response.use(
  (response) => response,
  async (error) => {
    if (error.response?.status === 401) {
      // Token expired, try refresh
      // ... refresh logic ...
    }
    return Promise.reject(error);
  }
);
```

Then use this in both APIs.

## 📊 Expected Console Output

### Successful Flow:
```
[useCart] 🔍 Fetching cart...
[useCart] 📋 Auth Token: eyJhbGciOiJIUzI1NiIs...
[useCart] 👤 Is Guest: false
[useCart] ✅ Cart Response: {
  cartId: "b33aa0e4-3e32-49f9-9b49-e183631fc0d5",
  userId: "48fdbb7b-9d91-4e41-872e-dd8296a0f315",
  itemCount: 1
}

[CheckoutPage] 🔍 Pre-checkout verification:
[CheckoutPage] 📋 Auth Token: eyJhbGciOiJIUzI1NiIs...  ← SAME TOKEN
[CheckoutPage] 🛒 Cart from useCart: {
  cartId: "b33aa0e4-3e32-49f9-9b49-e183631fc0d5",  ← SAME CART ID
  itemCount: 1,
  totalAmount: 760000
}

[CheckoutPage] 🔄 Refreshing cart before checkout...
[CheckoutPage] ✅ Cart verified after refresh: {
  cartId: "b33aa0e4-3e32-49f9-9b49-e183631fc0d5",
  itemCount: 1
}

[useCheckout] 🛒 Processing checkout...
[useCheckout] ✅ Checkout successful
```

### Failed Flow (Different Sessions):
```
[useCart] 📋 Auth Token: eyJhbGciOiJIUzI1NiIs...
[useCart] ✅ Cart Response: { cartId: "xxx", itemCount: 1 }

[CheckoutPage] 📋 Auth Token: eyJhbGciOiJIUzI1Nab...  ← DIFFERENT TOKEN!
[CheckoutPage] 🛒 Cart from useCart: { cartId: "xxx", itemCount: 1 }

[useCheckout] ❌ Process checkout failed: {
  message: "Cart is empty EMPTY_CART"
}
```

## 🎯 Action Items

1. **Add logging** to useCart and CheckoutPage
2. **Test checkout** and check console logs
3. **Compare tokens** in Network tab
4. **Report findings**:
   - Are tokens same or different?
   - Are session cookies same or different?
   - What does console show?

Then we can determine the exact fix needed!
