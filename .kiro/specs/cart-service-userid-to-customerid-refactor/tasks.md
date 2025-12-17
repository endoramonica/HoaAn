# Implementation Plan: CartService userId → customerId Refactor

## Overview
This implementation plan provides a systematic approach to refactor CartService from using `userId` to `customerId`. The refactor is done in phases to ensure no breaking changes and maintain code stability throughout.

---

## Phase 1: Repository Layer Refactor

- [x] 1. Update ICartRepository Interface





  - Rename `GetUserCartWithItemsAsync(Guid userId)` to `GetUserCartWithItemsAsync(Guid customerId)`
  - Rename `GetOrCreateCartByUserIdAsync(Guid userId)` to `GetOrCreateCartByCustomerIdAsync(Guid customerId)`
  - Update `MergeGuestCartToUserCartAsync(string sessionId, Guid userId)` to `MergeGuestCartToUserCartAsync(string sessionId, Guid customerId)`
  - Update XML documentation to reference customerId instead of userId
  - _Requirements: 4.1, 4.2, 4.3_

- [ ]* 1.1 Write unit tests for ICartRepository interface changes
  - Test that interface methods accept customerId parameter
  - Test that old method names are no longer available
  - _Requirements: 4.1, 4.2, 4.3_

- [x] 2. Update CartRepository Implementation





  - Update `GetUserCartWithItemsAsync` to accept `Guid customerId` parameter
  - Update `GetOrCreateCartByUserIdAsync` to `GetOrCreateCartByCustomerIdAsync` accepting `Guid customerId`
  - Update `MergeGuestCartToUserCartAsync` to accept `Guid customerId` parameter
  - Ensure all internal queries still use `c.UserId == customerId` (mapping customerId to UserId field)
  - Update XML documentation
  - _Requirements: 2.5, 4.1, 4.2, 4.3_

- [ ]* 2.1 Write unit tests for CartRepository implementation
  - Test GetUserCartWithItemsAsync retrieves correct cart for customerId
  - Test GetOrCreateCartByCustomerIdAsync creates cart for new customerId
  - Test MergeGuestCartToUserCartAsync merges to correct customer
  - Test that queries correctly map customerId to UserId field
  - _Requirements: 2.5, 4.1, 4.2, 4.3_

- [ ]* 2.2 Write property test for repository cart isolation
  - **Property 5: Repository Method Mapping**
  - **Validates: Requirements 2.5, 4.1, 4.2, 4.3**

---

## Phase 2: CartService Internal Refactor


- [x] 3. Update CartService Helper Methods




  - Rename `GetActiveUserCartWithItemsAsync(Guid userId)` to `GetActiveUserCartWithItemsAsync(Guid customerId)`
  - Update all internal calls to use `customerId` parameter
  - Update all calls to repository methods to use new method names
  - Update cache key generation to use `customerId` instead of `userId`
  - Update XML documentation
  - _Requirements: 2.1, 2.2, 2.3_

- [ ]* 3.1 Write unit tests for CartService helper methods
  - Test GetActiveUserCartWithItemsAsync accepts customerId
  - Test cache keys are generated correctly with customerId
  - Test that helper method calls repository with customerId
  - _Requirements: 2.1, 2.2, 2.3_

- [ ]* 3.2 Write property test for cache key consistency
  - **Property 3: Cache Key Uniqueness**
  - **Validates: Requirements 2.3**

- [x] 4. Update CartService Public Methods - Part 1 (Read Operations)





  - Update `GetCartAsync(Guid userId)` to `GetCartAsync(Guid customerId)`
  - Update `GetCartSummaryAsync(Guid userId)` to `GetCartSummaryAsync(Guid customerId)`
  - Update `GetCartItemCountAsync(Guid userId)` to `GetCartItemCountAsync(Guid customerId)`
  - Update `GetCartItemDetailAsync(Guid userId, Guid cartItemId)` to `GetCartItemDetailAsync(Guid customerId, Guid cartItemId)`
  - Update all internal logic to use `customerId` parameter
  - Update XML documentation
  - _Requirements: 1.1, 1.2, 1.3, 1.9, 1.10_

- [ ]* 4.1 Write unit tests for read operations
  - Test GetCartAsync returns correct cart for customerId
  - Test GetCartSummaryAsync returns correct summary
  - Test GetCartItemCountAsync returns correct count
  - Test GetCartItemDetailAsync returns correct item
  - _Requirements: 1.1, 1.2, 1.3, 1.9, 1.10_

- [ ]* 4.2 Write property test for read operations
  - **Property 1: CustomerId Parameter Acceptance**
  - **Validates: Requirements 1.1, 1.2, 1.3, 1.9, 1.10**

- [x] 5. Update CartService Public Methods - Part 2 (Write Operations)





  - Update `AddToCartAsync(Guid userId, AddToCartDto dto)` to `AddToCartAsync(Guid customerId, AddToCartDto dto)`
  - Update `UpdateCartItemAsync(Guid userId, UpdateCartItemDto dto)` to `UpdateCartItemAsync(Guid customerId, UpdateCartItemDto dto)`
  - Update `RemoveFromCartAsync(Guid userId, Guid cartItemId)` to `RemoveFromCartAsync(Guid customerId, Guid cartItemId)`
  - Update `ClearCartAsync(Guid userId)` to `ClearCartAsync(Guid customerId)`
  - Update `ValidateCartForCheckoutAsync(Guid userId)` to `ValidateCartForCheckoutAsync(Guid customerId)`
  - Update all internal logic to use `customerId` parameter
  - Update XML documentation
  - _Requirements: 1.1, 1.4, 1.5, 1.6, 1.7, 1.8_

- [ ]* 5.1 Write unit tests for write operations
  - Test AddToCartAsync adds item to correct customer's cart
  - Test UpdateCartItemAsync updates item in correct cart
  - Test RemoveFromCartAsync removes from correct cart
  - Test ClearCartAsync clears correct cart
  - Test ValidateCartForCheckoutAsync validates correct cart
  - _Requirements: 1.1, 1.4, 1.5, 1.6, 1.7, 1.8_

- [ ]* 5.2 Write property test for cart item isolation
  - **Property 8: Cart Item Isolation**
  - **Validates: Requirements 6.1, 6.2**


- [x] 6. Update CartService Public Methods - Part 3 (Customization Operations)




  - Update `AddToCartWithCustomizationsAsync(Guid userId, AddToCartDto dto)` to `AddToCartWithCustomizationsAsync(Guid customerId, AddToCartDto dto)`
  - Update `UpdateCartItemCustomizationsAsync(Guid userId, Guid cartItemId, List<CartItemCustomizationDto> customizations)` to `UpdateCartItemCustomizationsAsync(Guid customerId, Guid cartItemId, List<CartItemCustomizationDto> customizations)`
  - Update all internal logic to use `customerId` parameter
  - Update XML documentation
  - _Requirements: 1.1, 1.13, 1.14_

- [ ]* 6.1 Write unit tests for customization operations
  - Test AddToCartWithCustomizationsAsync adds item with customizations
  - Test UpdateCartItemCustomizationsAsync updates customizations correctly
  - _Requirements: 1.1, 1.13, 1.14_

- [x] 7. Update CartService Public Methods - Part 4 (Voucher Operations)





  - Update `ApplyCouponAsync(Guid userId, string couponCode)` to `ApplyCouponAsync(Guid customerId, string couponCode)`
  - Update `RemoveCouponAsync(Guid userId)` to `RemoveCouponAsync(Guid customerId)`
  - Update `ApplyVoucherAsync(Guid userId, string voucherCode)` to `ApplyVoucherAsync(Guid customerId, string voucherCode)`
  - Update `RemoveVoucherAsync(Guid userId)` to `RemoveVoucherAsync(Guid customerId)`
  - Update `ApplyVoucherToGuestAsync(string sessionId, string voucherCode)` - no change needed
  - Update `RemoveVoucherFromGuestAsync(string sessionId)` - no change needed
  - Update all internal logic to use `customerId` parameter
  - Update XML documentation
  - _Requirements: 1.1, 1.11, 1.12, 1.15, 1.16_

- [ ]* 7.1 Write unit tests for voucher operations
  - Test ApplyCouponAsync applies voucher to correct cart
  - Test RemoveCouponAsync removes voucher from correct cart
  - Test ApplyVoucherAsync applies voucher correctly
  - Test RemoveVoucherAsync removes voucher correctly
  - _Requirements: 1.1, 1.11, 1.12, 1.15, 1.16_

- [ ]* 7.2 Write property test for voucher isolation
  - **Property 11: Voucher Application Isolation**
  - **Validates: Requirements 6.5**

- [x] 8. Update CartService Public Methods - Part 5 (Merge Operation)





  - Update `MergeGuestCartToUserAsync(string sessionId, Guid userId)` to `MergeGuestCartToUserAsync(string sessionId, Guid customerId)`
  - Update all internal logic to use `customerId` parameter
  - Update call to repository method to use new name
  - Update XML documentation
  - _Requirements: 1.1, 1.17_

- [ ]* 8.1 Write unit tests for merge operation
  - Test MergeGuestCartToUserAsync merges to correct customer
  - Test guest cart items are moved to customer cart
  - Test guest cart is cleared after merge
  - _Requirements: 1.1, 1.17_

- [ ]* 8.2 Write property test for guest cart merge
  - **Property 10: Guest Cart Merge Consistency**
  - **Validates: Requirements 6.4**

- [x] 9. Checkpoint - Ensure all CartService tests pass




  - Ensure all tests pass, ask the user if questions arise.

---

## Phase 3: Controller Layer Refactor

- [x] 10. Update CartController




  - Add helper method to extract customerId from current user context
  - Update all endpoint methods to extract customerId and pass to CartService
  - Update all calls to CartService methods to use customerId parameter
  - Update XML documentation for endpoints
  - _Requirements: 3.1, 3.2, 3.3, 3.4, 3.5, 3.6, 3.7, 3.8_

- [ ]* 10.1 Write unit tests for CartController
  - Test GetCart endpoint extracts customerId and calls service
  - Test AddToCart endpoint passes customerId to service
  - Test UpdateCartItem endpoint passes customerId to service
  - Test RemoveFromCart endpoint passes customerId to service
  - Test ClearCart endpoint passes customerId to service
  - Test ApplyVoucher endpoint passes customerId to service
  - Test RemoveVoucher endpoint passes customerId to service
  - _Requirements: 3.1, 3.2, 3.3, 3.4, 3.5, 3.6, 3.7, 3.8_

- [ ]* 10.2 Write property test for controller parameter extraction
  - **Property 6: Controller Parameter Extraction**
  - **Validates: Requirements 3.1, 3.2, 3.3, 3.4, 3.5, 3.6, 3.7, 3.8**

---

## Phase 4: Integration Layer Refactor

- [x] 11. Update CheckoutService





  - Update all calls to CartService methods to pass customerId instead of userId
  - Update ValidateCartForCheckoutAsync call to use customerId
  - Update XML documentation
  - _Requirements: 5.1, 5.2_

- [ ]* 11.1 Write unit tests for CheckoutService integration
  - Test CheckoutService passes customerId to CartService methods
  - Test ValidateCartForCheckoutAsync is called with customerId
  - _Requirements: 5.1, 5.2_

- [ ]* 11.2 Write property test for CheckoutService integration
  - **Property 7: CheckoutService Integration**
  - **Validates: Requirements 5.1, 5.2**


- [x] 12. Search for and Update Other Services




  - Search codebase for other services that call CartService methods
  - Update all calls to use customerId parameter
  - Update XML documentation
  - _Requirements: 5.1, 5.2_

- [ ]* 12.1 Write unit tests for other service integrations
  - Test all services pass customerId to CartService
  - _Requirements: 5.1, 5.2_

---

## Phase 5: Validation and Testing

- [ ] 13. Write Integration Tests


  - Test full flow: GetCart → AddToCart → UpdateCart → RemoveCart → ClearCart with customerId
  - Test guest cart merge flow with customerId
  - Test voucher application flow with customerId
  - Test checkout validation flow with customerId
  - _Requirements: 6.1, 6.2, 6.3, 6.4, 6.5_

- [ ]* 13.1 Write property test for clear operation
  - **Property 9: Cart Clear Operation**
  - **Validates: Requirements 6.3**

- [ ]* 13.2 Write property test for permission preservation
  - **Property 4: Permission Check Preservation**
  - **Validates: Requirements 2.4**

- [ ]* 13.3 Write property test for internal helper method
  - **Property 2: Internal Helper Method Signature**
  - **Validates: Requirements 2.1, 2.2**

- [ ] 14. Checkpoint - Ensure all tests pass


  - Ensure all tests pass, ask the user if questions arise.

- [ ] 15. Code Review and Verification
  - Review all changes to ensure customerId is used consistently
  - Verify no userId references remain in CartService public methods
  - Verify cache keys use customerId
  - Verify repository calls use new method names
  - Verify controller extracts customerId correctly
  - Verify all services pass customerId to CartService
  - _Requirements: 1.1-1.17, 2.1-2.5, 3.1-3.8, 4.1-4.3, 5.1-5.2, 6.1-6.5_

- [ ] 16. Final Checkpoint - Ensure all tests pass
  - Ensure all tests pass, ask the user if questions arise.

---

## Notes

- **Backward Compatibility**: The Cart entity's `UserId` field remains unchanged in the database. We're only changing parameter names and logic flow semantically.
- **Cache Invalidation**: All cache invalidation logic uses customerId instead of userId.
- **Permission Checks**: Permission checks still use userId (from User entity) for authentication validation.
- **Guest Carts**: Guest cart operations remain unchanged (they use sessionId).
- **Testing**: Both unit tests and property-based tests are included to ensure correctness and prevent regressions.
