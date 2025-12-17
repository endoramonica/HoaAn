# CartService Refactoring Plan

## Objective
Implement single source of truth for retrieving active user carts to fix inconsistent query logic.

## Changes Made

### 1. ✅ Added Private Helper Method
**Method**: `GetActiveUserCartWithItemsAsync(Guid userId)`
- Single source of truth for retrieving active carts
- Loads Cart + CartItems (filtered for non-deleted)
- Loads Product data for each item
- Throws InvalidOperationException if cart doesn't exist
- Location: After cache constants, before User Cart Methods section

### 2. ✅ Refactored Public Methods

#### GetCartAsync(Guid userId)
- **Before**: Direct call to `cartRepository.GetUserCartWithItemsAsync(userId)`
- **After**: Uses `GetActiveUserCartWithItemsAsync(userId)`
- **Impact**: Consistent cart retrieval

#### GetCartSummaryAsync(Guid userId)
- **Before**: Direct call to `cartRepository.GetUserCartWithItemsAsync(userId)`
- **After**: Uses `GetActiveUserCartWithItemsAsync(userId)`
- **Impact**: Fixes "itemCount = 0" issue

#### GetCartItemCountAsync(Guid userId)
- **Before**: Direct call to `cartRepository.GetUserCartWithItemsAsync(userId)` with null check
- **After**: Uses `GetActiveUserCartWithItemsAsync(userId)` (throws if not found)
- **Impact**: Consistent error handling

#### GetCartItemDetailAsync(Guid userId, Guid cartItemId)
- **Before**: Queried CartItem directly, then verified cart ownership
- **After**: Gets cart via `GetActiveUserCartWithItemsAsync`, finds item in cart
- **Impact**: Fixes "Cart item not found" issue - now verifies ownership first

#### ApplyCouponAsync(Guid userId, string couponCode)
- **Before**: Direct call to `cartRepository.GetUserCartWithItemsAsync(userId)`
- **After**: Uses `GetActiveUserCartWithItemsAsync(userId)`
- **Impact**: Consistent cart retrieval

#### RemoveVoucherAsync(Guid userId)
- **Before**: Direct calls to `cartRepository.GetUserCartWithItemsAsync(userId)` (twice)
- **After**: Uses `GetActiveUserCartWithItemsAsync(userId)` (twice)
- **Impact**: Consistent cart retrieval

### 3. Remaining Methods to Refactor

These methods still need refactoring (lines with direct cartRepository calls):

- Line 150: AddToCartAsync - uses `GetOrCreateCartByUserIdAsync` (different logic, keep as-is)
- Line 345: AddToCartAsync - after adding item, retrieves updated cart
- Line 404: UpdateCartItemQuantityAsync - after updating quantity
- Line 479: RemoveFromCartAsync - retrieves cart
- Line 513: ClearCartAsync - retrieves cart
- Line 883: MergeGuestCartToUserAsync - after merge
- Line 1067: ApplyVoucherAsync (guest) - different method
- Line 1412: RemoveVoucherAsync (guest) - different method
- Line 1466: RemoveFromCartAsync - retrieves updated cart

## Benefits

1. **Consistency**: All cart retrievals use same logic
2. **Reliability**: Single point of failure/debugging
3. **Maintainability**: Changes to cart loading logic only need to be made once
4. **Correctness**: Fixes "0 items" and "not found" issues
5. **Clean Architecture**: Follows DDD principles with clear domain logic

## Testing

After refactoring, verify:
- ✅ GET /cart → returns items
- ✅ GET /cart/summary → returns correct itemCount
- ✅ GET /cart/item-count → returns correct count
- ✅ GET /cart/items/{id} → returns item or 404
- ✅ POST /cart/apply-voucher → works correctly
- ✅ POST /cart/remove-voucher → works correctly
