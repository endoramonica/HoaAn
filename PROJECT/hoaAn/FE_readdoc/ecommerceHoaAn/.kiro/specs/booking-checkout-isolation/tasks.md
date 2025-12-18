# Implementation Plan: Booking-Checkout Isolation

## Overview
This plan breaks down the design into discrete, manageable coding tasks. Each task builds incrementally on previous steps, starting with core validation logic, moving through component implementation, and ending with comprehensive testing.

---

## Phase 1: Core Validation Infrastructure

- [ ] 1. Create CheckoutContextValidator module
  - Create `src/lib/validators/CheckoutContextValidator.ts`
  - Implement `validate()` method to detect invalid state combinations
  - Implement `isBookingCheckout()` and `isProductCheckout()` helpers
  - Implement `hasStateContamination()` to catch mixed items
  - Export `ValidationResult` interface
  - _Requirements: 3.3, 4.1_

- [ ]* 1.1 Write property test for context validation completeness
  - **Feature: booking-checkout-isolation, Property 1: Context Validation Completeness**
  - **Validates: Requirements 3.3, 4.1**

- [ ] 2. Create CheckoutContext type definitions and utilities
  - Create `src/lib/types/CheckoutContext.ts`
  - Define `CheckoutContext`, `BookingInfo`, `CartItem` interfaces
  - Create helper functions: `createCheckoutContext()`, `updateCheckoutContext()`
  - Create serialization/deserialization for sessionStorage
  - _Requirements: 3.1, 3.2, 3.3_

- [ ]* 2.1 Write property test for checkout context round-trip
  - **Feature: booking-checkout-isolation, Property 4: Pricing Integrity**
  - **Validates: Requirements 6.1, 6.4**

- [ ] 3. Create CartStateManager for contamination prevention
  - Create `src/lib/services/CartStateManager.ts`
  - Implement `addBookingItem()` - clears product items first
  - Implement `addProductItem()` - clears booking items first
  - Implement `validateCartContents()` - ensures single-type invariant
  - Implement `getCartType()` - returns 'booking' | 'product' | 'invalid'
  - _Requirements: 3.2, 8.0_

- [ ]* 3.1 Write property test for cart contamination prevention
  - **Feature: booking-checkout-isolation, Property 8: Cart Contamination Prevention**
  - **Validates: Requirements 3.2**

---

## Phase 2: Payload Builders

- [ ] 4. Create BookingCheckoutPayloadBuilder
  - Create `src/lib/builders/BookingCheckoutPayloadBuilder.ts`
  - Implement `build()` method to construct booking payload
  - Validate bookingInfo exists and is complete
  - Validate cart contains only booking items
  - Validate pricing: `checkoutTotal === cartTotal`
  - Exclude all product-related fields
  - _Requirements: 5.1, 6.1_

- [ ]* 4.1 Write property test for booking checkout isolation
  - **Feature: booking-checkout-isolation, Property 2: Booking Checkout Isolation**
  - **Validates: Requirements 5.1, 5.2**

- [ ] 5. Create ProductCheckoutPayloadBuilder
  - Create `src/lib/builders/ProductCheckoutPayloadBuilder.ts`
  - Implement `build()` method to construct product payload
  - Validate bookingInfo does NOT exist
  - Validate cart contains only product items
  - Validate pricing: `checkoutTotal === cartTotal`
  - Exclude all booking-related fields
  - _Requirements: 5.2, 6.1_

- [ ]* 5.1 Write property test for product checkout isolation
  - **Feature: booking-checkout-isolation, Property 3: Product Checkout Isolation**
  - **Validates: Requirements 5.2, 6.1**

---

## Phase 3: Checkout Page Guard

- [ ] 6. Create CheckoutPageGuard component
  - Create `src/components/checkout/CheckoutPageGuard.tsx`
  - On mount: Validate CheckoutContext immediately
  - If invalid: Display error message, disable checkout UI
  - If valid: Enable checkout UI
  - Store validation result in component state
  - Do NOT auto-trigger checkout (no useEffect-based checkout)
  - _Requirements: 4.1, 4.2_

- [ ]* 6.1 Write unit tests for checkout page guard
  - Test invalid context blocks checkout UI
  - Test valid context enables checkout UI
  - Test no auto-execution on mount
  - _Requirements: 4.1, 4.2_

- [ ] 7. Create CheckoutContextProvider
  - Create `src/lib/contexts/CheckoutContext.tsx`
  - Implement React Context for CheckoutContext
  - Implement `useCheckoutContext()` hook
  - Validate context on every change
  - Emit validation errors to consumers
  - _Requirements: 3.1, 3.3_

- [ ]* 7.1 Write unit tests for checkout context provider
  - Test context creation and updates
  - Test validation on context changes
  - Test error emission
  - _Requirements: 3.1, 3.3_

---

## Phase 4: Booking Completion Handler

- [ ] 8. Create BookingCompletionHandler
  - Create `src/lib/handlers/BookingCompletionHandler.ts`
  - Implement `handleSuccess()` method:
    - Remove bookingInfo from sessionStorage
    - Clear booking cart
    - Create new empty product cart
    - Reset checkout context
  - Implement `handleFailure()` method:
    - Preserve session state
    - Log error for debugging
  - _Requirements: 3.1, 7.0, 9.0_

- [ ]* 8.1 Write property test for booking completion cleanup
  - **Feature: booking-checkout-isolation, Property 5: Booking Completion Cleanup**
  - **Validates: Requirements 3.1**

- [ ]* 8.2 Write property test for state preservation on failure
  - **Feature: booking-checkout-isolation, Property 7: State Preservation on Failure**
  - **Validates: Requirements 7.0, 9.0**

---

## Phase 5: Checkout Trigger Control

- [ ] 9. Create CheckoutTrigger component
  - Create `src/components/checkout/CheckoutTrigger.tsx`
  - Implement explicit button click handler
  - Call checkout API ONLY on button click
  - Validate context before API call
  - Handle API response with BookingCompletionHandler
  - Show loading state during API call
  - _Requirements: 4.2, 6.0_

- [ ]* 9.1 Write property test for explicit checkout trigger
  - **Feature: booking-checkout-isolation, Property 6: Explicit Checkout Trigger**
  - **Validates: Requirements 4.2**

- [ ]* 9.2 Write unit tests for checkout trigger
  - Test API called only on button click
  - Test validation before API call
  - Test loading state management
  - Test error handling
  - _Requirements: 4.2, 6.0_

---

## Phase 6: Error Handling and User Feedback

- [ ] 10. Create error message components
  - Create `src/components/checkout/CheckoutErrorBanner.tsx`
  - Display "Booking và mua sản phẩm không thể thực hiện chung" for contamination
  - Display "Giá không khớp. Vui lòng tải lại trang." for pricing mismatch
  - Display "Giỏ hàng chứa các mục không tương thích..." for cart issues
  - Provide action buttons (clear, refresh, retry)
  - _Requirements: 3.3, 6.0, 8.0_

- [ ]* 10.1 Write unit tests for error messages
  - Test correct message displayed for each error type
  - Test action buttons work correctly
  - _Requirements: 3.3, 6.0, 8.0_

---

## Phase 7: Integration and Validation

- [ ] 11. Integrate CheckoutPageGuard into CheckoutPage
  - Update `src/pages/checkout/CheckoutPage.tsx`
  - Wrap checkout content with CheckoutPageGuard
  - Use CheckoutContextProvider at page level
  - Integrate CheckoutTrigger for submit button
  - Integrate CheckoutErrorBanner for error display
  - _Requirements: 4.1, 4.2, 3.3_

- [ ]* 11.1 Write integration tests for checkout flow
  - Test complete booking checkout flow
  - Test complete product checkout flow
  - Test contamination prevention
  - Test error handling
  - _Requirements: 3.1, 3.2, 3.3, 4.1, 4.2_

- [ ] 12. Update cart operations to use CartStateManager
  - Update `src/lib/services/CartService.ts` (or equivalent)
  - Use CartStateManager for all add/remove operations
  - Validate cart after every mutation
  - Emit validation errors to UI
  - _Requirements: 3.2, 8.0_

- [ ]* 12.1 Write unit tests for cart operations
  - Test booking item addition clears products
  - Test product item addition clears booking
  - Test validation on every mutation
  - _Requirements: 3.2, 8.0_

---

## Phase 8: Backend Validation Setup

- [ ] 13. Document backend validation requirements
  - Create `BACKEND_VALIDATION_SPEC.md` in spec directory
  - Document required backend checks:
    - Reject if bookingInfo + product items detected
    - Reject if payload contains conflicting pricing sources
    - Reject if cart contents don't match checkout context
  - Document required logging:
    - Log checkoutType (booking | product)
    - Log cartId
    - Log bookingInfo presence
  - _Requirements: 8.0_

---

## Phase 9: Checkpoint and Testing

- [ ] 14. Checkpoint - Ensure all tests pass
  - Run all unit tests: `npm run test`
  - Run all property-based tests: `npm run test:properties`
  - Verify 100% pass rate
  - Fix any failing tests
  - Ask the user if questions arise.

- [ ] 15. Manual testing checklist
  - Test booking checkout flow end-to-end
  - Test product checkout flow end-to-end
  - Test contamination prevention (try to mix booking + products)
  - Test pricing validation
  - Test error messages display correctly
  - Test state preservation on failure
  - Test retry after failure
  - _Requirements: All_

---

## Notes

- All validators and builders should be pure functions (no side effects)
- All context changes should trigger validation
- All API calls should be preceded by validation
- All errors should be logged for debugging
- All tests should run with minimum 100 iterations for property-based tests
- Vietnamese error messages must be exact as specified in requirements
