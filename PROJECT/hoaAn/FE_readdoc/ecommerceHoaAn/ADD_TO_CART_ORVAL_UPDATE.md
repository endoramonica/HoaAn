# Add to Cart - Orval API Integration

## Summary
Updated ProductCard, ProductsPage, and QuickViewModal to use Orval-generated API for add to cart functionality.

## Changes Made

### 1. QuickViewModal.tsx ✅
- **Updated imports**: Replaced `CartService` with `getVietCommerceAPI()` from Orval
- **Added type imports**: `AddToCartDto` from Orval schemas
- **Updated handleAddToCart**: Now uses `api.postApiV1CartAdd()` and `api.postApiV1CartGuestAdd()`
- **Auto-detection**: Automatically detects guest/user status using `isAuthenticated`
- **Cart refresh**: Calls `refreshCart()` after successful add
- **Error handling**: Proper error messages for stock issues and auth errors

### 2. ProductCard.tsx ✅
- **Updated imports**: Added Orval API and necessary hooks
- **Removed onAddToCart prop**: Now handles add to cart internally
- **Added state**: `isAddingToCart` for loading state
- **Implemented handleAddToCart**: Complete add to cart logic with Orval API
- **Loading UI**: Shows spinner and "Đang thêm..." during add operation
- **Toast notifications**: Success/error messages using sonner
- **Fixed wishlist**: Updated to use correct hook signature

### 3. ProductsPage.tsx ✅
- **Updated imports**: Added Orval API, useCart, useAuth hooks
- **Added state**: `cartLoading` Set to track loading per product
- **Implemented handleAddToCart**: New function for add to cart with Orval API
- **Updated button**: Add to cart button now calls handleAddToCart with loading state
- **Loading UI**: Shows spinner per product during add operation
- **Toast notifications**: Success/error messages

## API Methods Used

```typescript
// From Api/generated-orval/index.ts
const api = getVietCommerceAPI();

// For authenticated users
await api.postApiV1CartAdd(dto: AddToCartDto);

// For guest users
await api.postApiV1CartGuestAdd(dto: AddToCartDto);
```

## AddToCartDto Interface

```typescript
interface AddToCartDto {
  productId: string;
  quantity: number;
}
```

## Features

### ✅ Guest & User Support
- Automatically detects authentication status
- Uses appropriate API endpoint (guest vs user)
- Guest cart uses sessionId stored in localStorage

### ✅ Loading States
- Per-product loading state in ProductsPage
- Disabled buttons during operation
- Loading spinner with "Đang thêm..." text

### ✅ Error Handling
- Insufficient stock detection
- Authentication errors
- Generic error fallback
- User-friendly Vietnamese error messages

### ✅ Success Feedback
- Toast notification with product name
- Cart badge updates automatically
- Quantity resets to 1 in QuickViewModal

### ✅ Cart Refresh
- Calls `refreshCart()` after successful add
- Updates cart count badge in header
- Syncs cart state across components

## Testing Checklist

- [ ] Add to cart from ProductsPage (guest)
- [ ] Add to cart from ProductsPage (authenticated)
- [ ] Add to cart from QuickViewModal (guest)
- [ ] Add to cart from QuickViewModal (authenticated)
- [ ] Add to cart from ProductCard (if used elsewhere)
- [ ] Verify loading states show correctly
- [ ] Verify error messages for out of stock
- [ ] Verify cart badge updates
- [ ] Verify toast notifications
- [ ] Verify guest cart persists with sessionId
- [ ] Verify cart merge on login

## Related Files

- `src/components/QuickViewModal.tsx`
- `src/components/ProductCard.tsx`
- `src/components/ProductsPage.tsx`
- `src/lib/hooks/useCart.ts` (uses old CartService, still works)
- `Api/generated-orval/index.ts` (Orval API)

## Notes

- useCart hook still uses old CartService from generated-client, but this is fine as the methods are compatible
- All add to cart buttons now use Orval API directly for consistency
- Guest cart sessionId is managed in localStorage with key `guest_cart_session_id`
- Cart merge happens automatically on login (handled by backend)
