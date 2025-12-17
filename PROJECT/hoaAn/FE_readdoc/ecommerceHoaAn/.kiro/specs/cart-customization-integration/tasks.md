# Implementation Plan: Cart Customization Integration

## Overview

This implementation plan focuses on **Frontend Only** tasks. The backend will implement the API enhancements separately (see `BACKEND_REQUIREMENTS.md`).

Frontend tasks focus on:
1. Retrieving saved customization state
2. Passing customizations to existing cart APIs
3. Displaying customization details from cart responses
4. Allowing users to edit customizations

---

## Prerequisites

**Backend must implement first:**
- ✅ Accept `customizations` array in add-to-cart API
- ✅ Return `customizations` in cart item responses
- ✅ Support customization updates
- ✅ Include customizations in checkout

See `BACKEND_REQUIREMENTS.md` for details.

---

## Tasks

- [x] 1. Update CartService to pass customizations to backend




  - [ ] 1.1 Update AddToCartRequest interface to include customizations
    - Modify `src/lib/services/cartService.ts`
    - Add `customizations?: CartItemCustomizationDto[]` to AddToCartRequest
    - _Requirements: 2.1, 2.2_


  - [ ] 1.2 Update cartService.addItem() to transmit customizations
    - Modify `src/lib/services/cartService.ts`
    - Pass customizations array to api.postApiV1CartAdd()


    - Ensure both authenticated and guest cart services support customizations

    - _Requirements: 1.2, 2.3_

- [ ] 2. Update EventSidebar to include customizations when adding to cart

  - [x] 2.1 Retrieve customization state before adding product to cart

    - Modify `src/pages/calendar/EventSidebar.tsx`
    - Import customizationStateService
    - Before calling cartService.addItem(), retrieve saved customization state for selectedServiceProductId
    - _Requirements: 1.1, 1.2_




  - [ ] 2.2 Transform and pass customizations to cartService.addItem()
    - Modify `src/pages/calendar/EventSidebar.tsx`
    - Transform SavedCustomizationState to CartItemCustomizationDto[] format
    - Pass customizations in the addItem request
    - _Requirements: 1.2_


- [ ] 3. Update ProductDetailPage to include customizations when adding to cart

  - [x] 3.1 Retrieve customization state before adding product to cart

    - Modify `src/pages/ProductDetailPage.tsx`
    - Import customizationStateService

    - Before calling cartService.addItem(), retrieve saved customization state for the product
    - _Requirements: 1.1, 1.2_

  - [x] 3.2 Transform and pass customizations to cartService.addItem()

    - Modify `src/pages/ProductDetailPage.tsx`
    - Transform SavedCustomizationState to CartItemCustomizationDto[] format
    - Pass customizations in the addItem request
    - _Requirements: 1.2_


- [x] 4. Update useCart hook to handle customization data from backend


  - [ ] 4.1 Verify CartItem interface includes customizations field
    - Review `src/lib/hooks/useCart.ts`
    - Ensure CartItem interface has `customizations?: CartItemCustomizationDto[]`
    - _Requirements: 1.4, 4.2_


  - [ ] 4.2 Verify customizations are preserved in cart transformation
    - Review `src/lib/hooks/useCart.ts` fetchCart() method
    - Ensure customizations from API response are mapped to CartItem
    - Ensure basePrice, customizationPrice, finalPrice are included
    - _Requirements: 1.4, 4.2_


- [ ] 5. Display customization options in CartPage

  - [x] 5.1 Create CustomizationDisplay component

    - Create `src/components/CustomizationDisplay.tsx`
    - Display customization options for a cart item

    - Show: option name, quantity, unit, unit price, total price
    - Show customization subtotal
    - _Requirements: 1.4_

  - [ ] 5.2 Update CartPage to display customizations for each item
    - Modify `src/components/CartPage.tsx`
    - For each cart item with customizations, render CustomizationDisplay

    - Show price breakdown: basePrice + customizationPrice = finalPrice
    - _Requirements: 1.4_

  - [ ] 5.3 Add edit customizations button to CartPage
    - Modify `src/components/CartPage.tsx`

    - Add button/link to edit customizations for each item
    - Button should only appear for items with customizations
    - _Requirements: 3.1_

- [x] 6. Implement customization editor component


  - [x] 6.1 Create CustomizationEditor component

    - Create `src/components/CustomizationEditor.tsx`
    - Display current customization options with quantity inputs
    - Show min/max bounds for each option
    - Validate quantities on input change
    - Calculate and display updated prices

    - _Requirements: 3.2, 3.3_

  - [ ] 6.2 Add save and cancel buttons to CustomizationEditor
    - Save button calls cartService to update customizations

    - Cancel button closes editor without changes
    - Show loading state while updating
    - _Requirements: 3.2_

  - [ ] 6.3 Integrate CustomizationEditor into CartPage
    - Modify `src/components/CartPage.tsx`
    - Show CustomizationEditor in modal when edit is clicked
    - Refresh cart after customizations are updated
    - Handle errors and show error messages
    - _Requirements: 3.2, 3.3_

- [ ] 7. Update checkout to display customization summary

  - [ ] 7.1 Update CheckoutPage to show customization details
    - Modify checkout page component
    - Display customization options in order summary
    - Show customization prices in total calculation
    - _Requirements: 1.5_

  - [ ] 7.2 Verify customizations are included in checkout request
    - Review checkout flow
    - Ensure customizations from cart items are preserved
    - Verify checkout API receives customization data
    - _Requirements: 1.5_



- [ ] 8. Checkpoint - Verify integration with backend

  - Ensure all frontend code is ready for backend API
  - Test with mock data to verify data structures
  - Ask the user if questions arise

- [ ] 9. Integration testing (after backend is ready)

  - [ ] 9.1 Test complete flow: select customizations → add to cart → view cart
    - Test that customizations are sent in add-to-cart request
    - Test that customizations are displayed in cart
    - Test that prices are calculated correctly
    - _Requirements: 1.1, 1.2, 1.3, 1.4_

  - [ ] 9.2 Test customization update flow: edit customizations → update cart
    - Test that update request includes new customizations
    - Test that cart is refreshed with updated values
    - Test that prices are recalculated
    - _Requirements: 3.1, 3.2, 3.3, 3.4_

  - [ ] 9.3 Test checkout with customizations
    - Test that checkout includes customization details
    - Test that order totals include customization prices
    - _Requirements: 1.5_

- [ ] 10. Final Checkpoint - Ensure all tests pass
  - Ensure all frontend tests pass
  - Verify integration with backend APIs
  - Ask the user if questions arise

