# Cart API Conflict Analysis - EMPTY_CART Error

## 🔴 Problem Identified

Backend returns: `{"success": false, "message": "Cart is empty EMPTY_CART"}`

## 🔍 Root Cause Analysis

### Issue 1: Mixed API Usage ❌

**useCart Hook** uses **OLD CartService** (generated-client):
```typescript
// src/lib/hooks/useCart.ts
import { CartService } from "@/api/services/CartService";  // ❌ OLD

await CartService.getApiV1Cart();
await CartService.getApiV1CartGuest();
```

**useCheckout Hook** uses **NEW Orval API**:
```typescript
// src/lib/hooks/useCheckout.ts
import { getVietCommerceAPI } from '../../../Api/generated-orval';  // ✅ NEW

const api = getVietCommerceAPI();
await api.postApiV1CheckoutProcess(checkoutData);
```

### Issue 2: Different API Clients = Different Sessions

**Old CartService** (OpenAPI Generator):
- Uses its own axios instance
- May have different interceptors
- Different token/session handling

**New Orval API** (Orval Generator):
- Uses different axios instance
- Different interceptors
- Different token/session handling

**Result**: Backend sees TWO different sessions!
- Cart created with OLD API session
- Checkout called with NEW API session
- Backend: "This session has no cart!" → EMPTY_CART

## 📊 Current Architecture

```
┌─────────────────────────────────────────────────────────┐
│                     Frontend                             │
├─────────────────────────────────────────────────────────┤
│                                                          │
│  CartPage                                                │
│     └─> useCart (OLD CartService)                       │
│           └─> GET /Cart                                  │
│           └─> POST /Cart/add                             │
│                                                          │
│  CheckoutPage                                            │
│     └─> useCart (OLD CartService)  ← Get cart data      │
│     └─> useCheckout (NEW Orval API) ← Process checkout  │
│           └─> POST /Checkout/process                     │
│                                                          │
└─────────────────────────────────────────────────────────┘
                    │                    │
                    │                    │
            OLD Session           NEW Session
                    │                    │
                    ▼                    ▼
┌─────────────────────────────────────────────────────────┐
│                     Backend                              │
├─────────────────────────────────────────────────────────┤
│                                                          │
│  Session A (OLD API)                                     │
│    └─> Cart: { cartId: "xxx", items: [...] }           │
│                                                          │
│  Session B (NEW API)                                     │
│    └─> Cart: NULL  ← EMPTY_CART!                       │
│                                                          │
└─────────────────────────────────────────────────────────┘
```

## 🎯 Solution Options

### Option 1: Migrate useCart to Orval (Recommended) ✅

**Pros:**
- Consistent API usage across app
- Single axios instance = single session
- Type-safe with Orval types
- Future-proof

**Cons:**
- Need to refactor useCart hook
- Need to test all cart operations

**Implementation:**
```typescript
// src/lib/hooks/useCart.ts
import { getVietCommerceAPI } from '../../../Api/generated-orval';
import type { 
  CartResponseDto,
  UpdateCartItemDto 
} from '../../../Api/generated-orval/schemas';

const api = getVietCommerceAPI();

// Replace all CartService calls with api calls
const cartResponse = isGuest
  ? await api.getApiV1CartGuest()
  : await api.getApiV1Cart();
```

### Option 2: Use Same API Client in Both

**Pros:**
- Quick fix
- Minimal changes

**Cons:**
- Still using old generated-client
- Not future-proof

**Implementation:**
```typescript
// src/lib/hooks/useCheckout.ts
import { CartService } from '@/api/services/CartService';
import { CheckoutService } from '@/api/services/CheckoutService';

// Use old services instead of Orval
await CheckoutService.postApiV1CheckoutProcess(checkoutData);
```

### Option 3: Ensure Same Session Token

**Pros:**
- Keep both APIs
- Fix session issue

**Cons:**
- Complex to implement
- Need to sync axios instances

**Implementation:**
```typescript
// Share token between both API clients
import { tokenStorage } from '@/lib/api/client';
import { apiClient } from '../../../Api/generated-orval';

// Sync tokens
apiClient.interceptors.request.use((config) => {
  const token = tokenStorage.getAccessToken();
  if (token) {
    config.headers.Authorization = `Bearer ${token}`;
  }
  return config;
});
```

## 🚀 Recommended Action Plan

### Phase 1: Quick Fix (Immediate)
1. Check if both APIs use same token
2. Add logging to verify session/token
3. Ensure axios interceptors are synced

### Phase 2: Long-term Fix (Recommended)
1. Migrate useCart to Orval API
2. Remove old CartService usage
3. Test all cart operations
4. Update CartPage to use new API

## 🔧 Debug Steps

### Step 1: Check Token Usage

Add to both API clients:
```typescript
// OLD API
console.log('[CartService] Token:', localStorage.getItem('auth_token'));

// NEW API  
console.log('[Orval API] Token:', localStorage.getItem('auth_token'));
```

### Step 2: Check Session ID

Add logging:
```typescript
// In useCart
console.log('[useCart] Session ID:', document.cookie);

// In useCheckout
console.log('[useCheckout] Session ID:', document.cookie);
```

### Step 3: Compare Request Headers

In DevTools Network:
1. Find `GET /Cart` request (from useCart)
2. Find `POST /Checkout/process` request (from useCheckout)
3. Compare headers:
   - `Authorization`
   - `Cookie`
   - `Session-Id`

### Step 4: Backend Logs

Check backend logs for:
```
[Cart API] User: xxx, Session: yyy
[Checkout API] User: xxx, Session: zzz  ← Different session!
```

## 📝 Files to Check

### Frontend
- `src/lib/hooks/useCart.ts` - Uses OLD CartService
- `src/lib/hooks/useCheckout.ts` - Uses NEW Orval API
- `src/api/services/CartService.ts` - OLD API client
- `Api/generated-orval/index.ts` - NEW API client
- `src/lib/api/client.ts` - Token storage

### Backend
- Cart controller - Check session validation
- Checkout controller - Check cart lookup logic
- Session middleware - Check session handling

## 🎯 Next Steps

1. **Verify Token/Session Sync**
   - Add logging to both APIs
   - Compare request headers
   - Check if tokens match

2. **Choose Solution**
   - Option 1 (Migrate to Orval) - Best long-term
   - Option 2 (Use same API) - Quick fix
   - Option 3 (Sync sessions) - Complex

3. **Implement Fix**
   - Based on chosen solution
   - Test thoroughly
   - Verify EMPTY_CART is resolved

4. **Test Scenarios**
   - Guest cart → Checkout
   - User cart → Checkout
   - Cart merge on login → Checkout
   - Multiple items → Checkout

## 🔗 Related Issues

- Mixed API usage (Orval vs OpenAPI Generator)
- Session/token synchronization
- Cart ownership validation
- API client configuration

## 📚 Related Documentation

- `HOOKS_RESTRUCTURE_COMPLETE.md` - useCheckout migration
- `CHECKOUT_400_ERROR_FIX.md` - Checkout validation
- `CART_MERGE_FLOW.md` - Cart merge logic
