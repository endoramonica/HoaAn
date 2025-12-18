# Booking-Checkout Isolation Design Document

## Overview

This design ensures complete separation between booking flows and product checkout flows. The system enforces strict isolation at the state management, UI, and API layers to prevent data contamination and ensure pricing integrity. The core principle is that booking and product purchases are fundamentally different transaction types that must never be mixed within a single checkout session.

## Architecture

### High-Level Flow Separation

```
┌─────────────────────────────────────────────────────────────┐
│                    User Action                              │
└────────────────┬────────────────────────────────────────────┘
                 │
        ┌────────┴────────┐
        │                 │
    ┌───▼────┐      ┌────▼────┐
    │ Booking│      │ Product │
    │ Flow   │      │ Checkout│
    └───┬────┘      └────┬────┘
        │                │
    ┌───▼────────────────▼────┐
    │  CheckoutContext Guard   │
    │  (Validation Layer)      │
    └───┬────────────────┬────┘
        │                │
    ┌───▼────┐      ┌────▼────┐
    │Booking │      │ Product │
    │Checkout│      │Checkout │
    │API     │      │API      │
    └────────┘      └─────────┘
```

### State Management Architecture

**CheckoutContext Structure:**
```typescript
{
  cartType: 'booking' | 'product',
  cartId: string,
  bookingInfo?: {
    eventId: string,
    serviceProductId: string,
    customization?: object,
    notes?: string
  },
  isValid: boolean,
  validationErrors?: string[]
}
```

**Session Storage:**
- `bookingInfo` stored in sessionStorage (temporary, cleared on completion)
- `cartId` stored in localStorage (persistent across sessions)
- `cartType` derived from cart contents validation

## Components and Interfaces

### 1. CheckoutContextValidator

**Responsibility:** Validate checkout context before any checkout operation

**Interface:**
```typescript
interface CheckoutContextValidator {
  validate(context: CheckoutContext): ValidationResult
  isBookingCheckout(context: CheckoutContext): boolean
  isProductCheckout(context: CheckoutContext): boolean
  hasStateContamination(context: CheckoutContext): boolean
}

interface ValidationResult {
  isValid: boolean
  errors: string[]
  cartType: 'booking' | 'product' | 'invalid'
}
```

**Validation Rules:**
- If `bookingInfo` exists AND cart contains product items → INVALID
- If `bookingInfo` exists AND cart contains only booking items → VALID (booking)
- If `bookingInfo` does NOT exist AND cart contains only product items → VALID (product)
- If cart is empty → INVALID

### 2. CheckoutPageGuard

**Responsibility:** Protect checkout page from invalid states and auto-execution

**Behavior:**
- On mount: Validate context immediately
- If invalid: Display error message, block checkout UI
- If valid: Enable checkout UI
- Checkout API call ONLY on explicit user action (button click)
- NO useEffect-based auto-checkout

**Key Constraint:** Checkout must be explicitly triggered by user, never automatically

### 3. BookingCheckoutPayloadBuilder

**Responsibility:** Construct booking checkout payload with proper isolation

**Rules:**
- ONLY include booking-specific fields when `bookingInfo` exists
- Include: `bookingInfo`, `cartId`, `bookingServiceItems`
- Exclude: product-related fields, product pricing overrides
- Validate: `checkoutTotal === cartTotal` before submission

### 4. ProductCheckoutPayloadBuilder

**Responsibility:** Construct product checkout payload without booking contamination

**Rules:**
- ONLY include product items from cart
- Exclude: any `bookingInfo` fields
- Include: standard checkout fields (shipping, payment, etc.)
- Validate: `checkoutTotal === cartTotal` before submission

### 5. BookingCompletionHandler

**Responsibility:** Clean up booking state after successful checkout

**Actions on Success:**
1. Remove `bookingInfo` from sessionStorage
2. Clear booking cart
3. Create new empty product cart
4. Reset checkout context
5. Redirect to success page

**Actions on Failure:**
1. Preserve session state
2. Allow retry without data loss
3. Log error for debugging

### 6. CartStateManager

**Responsibility:** Maintain cart integrity and prevent contamination

**Rules:**
- Cart can contain EITHER booking items OR product items, never both
- When adding booking item: Clear product items first
- When adding product item: Clear booking items first
- Validate cart contents on every mutation
- Emit validation errors to UI

## Data Models

### CheckoutContext
```typescript
interface CheckoutContext {
  cartType: 'booking' | 'product' | 'invalid'
  cartId: string
  bookingInfo?: BookingInfo
  isValid: boolean
  validationErrors: string[]
  lastValidatedAt: timestamp
}

interface BookingInfo {
  eventId: string
  serviceProductId: string
  customization?: CustomizationData
  notes?: string
  quantity: number
  price: decimal
}

interface CartItem {
  id: string
  type: 'booking' | 'product'
  quantity: number
  price: decimal
  // ... other fields
}
```

### Checkout Payloads

**Booking Checkout Payload:**
```typescript
{
  cartId: string
  cartType: 'booking'
  bookingInfo: {
    eventId: string
    serviceProductId: string
    customization?: object
    notes?: string
  }
  items: BookingCartItem[]
  total: decimal
  // Standard checkout fields
  shippingAddress?: Address
  paymentMethod?: string
}
```

**Product Checkout Payload:**
```typescript
{
  cartId: string
  cartType: 'product'
  items: ProductCartItem[]
  total: decimal
  // Standard checkout fields
  shippingAddress: Address
  paymentMethod: string
  // NO bookingInfo field
}
```

## Correctness Properties

A property is a characteristic or behavior that should hold true across all valid executions of a system—essentially, a formal statement about what the system should do. Properties serve as the bridge between human-readable specifications and machine-verifiable correctness guarantees.

### Property 1: Context Validation Completeness
*For any* checkout context, the validator MUST detect all invalid state combinations (bookingInfo + product items, or vice versa) and prevent checkout execution.

**Validates: Requirements 3.3, 4.1**

### Property 2: Booking Checkout Isolation
*For any* booking checkout, the payload MUST contain ONLY booking-related fields and MUST NOT contain product-related fields or product pricing data.

**Validates: Requirements 5.1, 5.2**

### Property 3: Product Checkout Isolation
*For any* product checkout, the payload MUST NOT contain any bookingInfo fields and MUST contain ONLY product items from the cart.

**Validates: Requirements 5.2, 6.1**

### Property 4: Pricing Integrity
*For any* checkout (booking or product), the checkout total MUST equal the cart total before submission, ensuring no pricing source mixing.

**Validates: Requirements 6.1, 6.4**

### Property 5: Booking Completion Cleanup
*For any* successful booking checkout, bookingInfo MUST be removed from sessionStorage and the booking cart MUST be cleared, leaving only an empty product cart.

**Validates: Requirements 3.1**

### Property 6: Explicit Checkout Trigger
*For any* checkout page, the checkout API MUST only be called when the user explicitly clicks the confirm button, never through automatic effects or page initialization.

**Validates: Requirements 4.2**

### Property 7: State Preservation on Failure
*For any* failed checkout attempt, the session state (bookingInfo, cart contents, context) MUST be preserved to allow retry without data loss.

**Validates: Requirements 7.0, 9.0**

### Property 8: Cart Contamination Prevention
*For any* cart operation, adding a booking item MUST clear product items, and adding a product item MUST clear booking items, maintaining single-type cart invariant.

**Validates: Requirements 3.2**

## Error Handling

### Invalid Context Detection

**Error Message:** "Booking và mua sản phẩm không thể thực hiện chung" (Booking and product purchase cannot be done together)

**Trigger:** When `bookingInfo` exists AND cart contains product items

**User Action:** 
- Display error banner on checkout page
- Disable checkout button
- Provide option to clear booking info or remove products
- Suggest starting fresh

### Pricing Mismatch

**Error Message:** "Giá không khớp. Vui lòng tải lại trang." (Price mismatch. Please refresh the page.)

**Trigger:** When `checkoutTotal !== cartTotal`

**User Action:**
- Block checkout submission
- Suggest page refresh
- Log discrepancy for debugging

### Cart Contamination

**Error Message:** "Giỏ hàng chứa các mục không tương thích. Vui lòng xóa và thử lại." (Cart contains incompatible items. Please clear and try again.)

**Trigger:** When cart validation fails

**User Action:**
- Prevent cart mutation
- Show which items are incompatible
- Offer to clear cart

### Backend Rejection

**Trigger:** Backend detects invalid checkout request

**Backend Response:** 400 Bad Request with specific error code

**Frontend Action:**
- Preserve session state
- Display user-friendly error
- Allow retry

## Testing Strategy

### Unit Testing

Unit tests verify specific examples and edge cases:

- **Context Validator Tests:**
  - Valid booking context with bookingInfo + booking items
  - Valid product context without bookingInfo + product items
  - Invalid context with bookingInfo + product items
  - Invalid context with empty cart
  - Edge case: bookingInfo exists but cart is empty

- **Payload Builder Tests:**
  - Booking payload excludes product fields
  - Product payload excludes bookingInfo
  - Pricing validation catches mismatches
  - Required fields are present

- **Cart Manager Tests:**
  - Adding booking item clears product items
  - Adding product item clears booking items
  - Cart validation rejects mixed items
  - Cart mutations emit validation events

- **Checkout Guard Tests:**
  - Invalid context blocks checkout UI
  - Valid context enables checkout UI
  - Checkout only triggers on button click
  - No auto-execution on mount

### Property-Based Testing

Property-based tests verify universal properties across all inputs:

- **Property 1: Context Validation Completeness**
  - Generate random context combinations
  - Verify validator catches all invalid states
  - Verify validator allows all valid states
  - Minimum 100 iterations

- **Property 2: Booking Checkout Isolation**
  - Generate random booking checkouts
  - Verify payload contains only booking fields
  - Verify no product fields present
  - Minimum 100 iterations

- **Property 3: Product Checkout Isolation**
  - Generate random product checkouts
  - Verify payload contains only product items
  - Verify bookingInfo absent
  - Minimum 100 iterations

- **Property 4: Pricing Integrity**
  - Generate random cart states
  - Verify checkout total equals cart total
  - Test with various item combinations
  - Minimum 100 iterations

- **Property 5: Booking Completion Cleanup**
  - Generate random booking checkout scenarios
  - Verify bookingInfo removed after success
  - Verify booking cart cleared
  - Verify product cart created
  - Minimum 100 iterations

- **Property 6: Explicit Checkout Trigger**
  - Generate random checkout page states
  - Verify API not called on mount
  - Verify API called only on button click
  - Minimum 100 iterations

- **Property 7: State Preservation on Failure**
  - Generate random checkout scenarios
  - Simulate checkout failure
  - Verify state preserved
  - Verify retry possible
  - Minimum 100 iterations

- **Property 8: Cart Contamination Prevention**
  - Generate random cart operations
  - Verify single-type invariant maintained
  - Verify automatic cleanup on type change
  - Minimum 100 iterations

**Testing Framework:** Vitest with fast-check for property-based testing

**Test Organization:**
- Unit tests co-located with source files (`.test.ts` suffix)
- Property tests in dedicated `__tests__/properties` directory
- Each property test tagged with requirement reference
- Minimum 100 iterations per property test

## Implementation Considerations

### Frontend State Management

- Use React Context for CheckoutContext
- Use sessionStorage for temporary bookingInfo
- Use localStorage for persistent cartId
- Validate on every context change

### API Integration

- Separate endpoints for booking vs product checkout
- Backend validates payload structure
- Backend rejects mixed checkout requests
- Backend logs checkout type for analytics

### User Experience

- Clear error messages in Vietnamese
- Prevent accidental mixing of checkout types
- Smooth transitions between booking and product flows
- Preserve user data on errors

### Performance

- Validation runs synchronously (fast)
- No unnecessary re-renders on context changes
- Lazy load checkout components
- Cache validation results

## Design Decisions and Rationales

1. **Strict Separation at Multiple Layers**
   - *Rationale:* Prevents bugs from slipping through; defense in depth approach
   - *Trade-off:* Slightly more code, but much safer

2. **Explicit User Action for Checkout**
   - *Rationale:* Prevents accidental double-submissions and ensures user intent
   - *Trade-off:* Requires explicit button click, but prevents silent failures

3. **SessionStorage for BookingInfo**
   - *Rationale:* Temporary state that should not persist across browser sessions
   - *Trade-off:* Lost on browser close, but prevents stale booking data

4. **Validation on Every Context Change**
   - *Rationale:* Catches contamination immediately
   - *Trade-off:* Slight performance cost, but ensures correctness

5. **Backend Validation as Hard Stop**
   - *Rationale:* Prevents malicious or buggy frontend from corrupting data
   - *Trade-off:* Requires backend changes, but essential for data integrity
