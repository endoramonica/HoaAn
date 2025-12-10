# Calendar Booking Service Refactor - Using CartService

## Problem Identified

The original implementation had several issues:

1. **Wrong API Endpoint**: Called `/api/v1/Cart/guest/add` directly instead of using the authenticated cart endpoint
2. **No Auth Sync**: Didn't sync with the app's cart state management
3. **Duplicate Logic**: Duplicated cart handling logic instead of using existing `cartService`
4. **User Detection**: Didn't properly detect if user was authenticated or guest

### Console Evidence
```
POST https://localhost:7131/api/v1/Cart/guest/add 400 (Bad Request)
Error: "Validation failed"
errors: {
  "dto": ["The dto field is required."],
  "$.productId": ["The JSON value could not be converted to System.Guid..."]
}
```

The user was logged in (`user555@example.com`) but the service was calling the guest cart API, causing a 400 error.

## Solution Implemented

### 1. Refactored CalendarBookingService
**File**: `src/lib/services/calendarBookingService.ts`

**Changes**:
- Removed direct API calls to cart endpoints
- Added `cartId` parameter to `CalendarBookingRequest` interface
- Service now only handles the checkout API call
- Cart operations are handled by the component using `cartService`

```typescript
export interface CalendarBookingRequest {
    // ... other fields
    cartId: string; // Cart ID from cartService (already has product added)
}
```

### 2. Updated EventSidebar Component
**File**: `src/pages/calendar/EventSidebar.tsx`

**Changes**:
- Imported `cartService` instead of calling API directly
- Added product to cart using `cartService.addItem()` before booking
- Gets cart ID from `cartService.getCart()` response
- Passes cart ID to booking service

**New Flow**:
```typescript
// Step 1: Add service product to cart
await cartService.addItem({
  productId: selectedServiceProductId,
  quantity: 1,
});

// Step 2: Get updated cart to get cart ID
const cartResponse = await cartService.getCart();
const cartId = cartResponse?.data?.id || cartResponse?.data?.cartId;

// Step 3: Create booking with cart ID
const result = await calendarBookingService.createBookingFromEvent({
  // ... other fields
  cartId: cartId,
});
```

## Benefits

✅ **Correct Endpoint**: Uses authenticated cart endpoint when user is logged in
✅ **Auth Sync**: Automatically uses correct endpoint based on user authentication status
✅ **No Duplication**: Reuses existing `cartService` logic
✅ **Proper User Detection**: `cartService` handles auth detection internally
✅ **Cleaner Separation**: Service only handles checkout, component handles cart

## How CartService Works

The `cartService` automatically detects user authentication:

```typescript
// cartService.addItem() internally:
// - Checks if user is authenticated
// - Calls POST /api/v1/Cart/add for authenticated users
// - Calls POST /api/v1/Cart/guest/add for guest users
// - Backend handles sessionId via HTTP-only cookie
```

## API Flow (Updated)

```
1. Component: cartService.addItem()
   ↓
   POST /api/v1/Cart/add (if authenticated)
   OR
   POST /api/v1/Cart/guest/add (if guest)

2. Component: cartService.getCart()
   ↓
   GET /api/v1/Cart (if authenticated)
   OR
   GET /api/v1/Cart/guest (if guest)

3. Service: postApiV1CheckoutProcess()
   ↓
   POST /api/v1/Checkout/process
   (with cartId and shipping info)
```

## Testing

### For Authenticated Users
1. Login with user account
2. Go to Services page
3. Click "Đặt dịch vụ"
4. Select date on calendar
5. Fill booking form
6. System should:
   - Add product to authenticated cart
   - Get cart ID
   - Create order
   - Redirect to checkout

### For Guest Users
1. Don't login
2. Go to Services page
3. Click "Đặt dịch vụ"
4. Select date on calendar
5. Fill booking form
6. System should:
   - Add product to guest cart
   - Get cart ID
   - Create order
   - Redirect to checkout

## Console Logs

When booking, you should see:
```
[EventSidebar] Adding product to cart: 11d63acd-24f3-4e85-b061-6494b62902bb
[EventSidebar] Product added to cart successfully
[EventSidebar] Cart ID: e77528fc-c538-470c-a0de-332cfd84dbf8
[calendarBookingService] Starting booking process for event: ... with product: ... and cart: ...
[calendarBookingService] Checkout request: {...}
[calendarBookingService] Checkout response: {...}
```

## Files Modified

1. `src/lib/services/calendarBookingService.ts`
   - Removed cart API calls
   - Added cartId parameter
   - Simplified to only handle checkout

2. `src/pages/calendar/EventSidebar.tsx`
   - Added cartService import
   - Added product to cart before booking
   - Gets cart ID and passes to service

## Migration Notes

If you have other components using the old booking service:
- Update them to add product to cart first using `cartService.addItem()`
- Get cart ID using `cartService.getCart()`
- Pass cartId to booking service

Example:
```typescript
// OLD (don't use)
await calendarBookingService.createBookingFromEvent({
  serviceProductId: productId,
  // ... other fields
});

// NEW (correct way)
await cartService.addItem({ productId, quantity: 1 });
const cart = await cartService.getCart();
await calendarBookingService.createBookingFromEvent({
  serviceProductId: productId,
  cartId: cart.data.id,
  // ... other fields
});
```
