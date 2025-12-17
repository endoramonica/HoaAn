# Design Document: Cart Customization Integration

## Overview

This design establishes the complete flow for capturing, transmitting, and persisting customization options when customers add products to their cart. The system ensures that customization data flows seamlessly from the frontend UI through the cart service to the backend API, and is properly displayed throughout the checkout process.

The key insight is that customization options are already defined in the backend API schema (`AddToCartDto.customizations` and `CartItemCustomizationDto`), but the frontend is not currently utilizing this field when making add-to-cart requests.

## Architecture

### Data Flow

```
Frontend UI (CustomizationSelector)
    ↓
CustomizationStateService (localStorage)
    ↓
EventSidebar / ProductDetail (retrieves saved state)
    ↓
CartService.addItem() (includes customizations in request)
    ↓
Backend API: POST /api/v1/Cart/add
    ↓
Backend Cart Service (persists customizations)
    ↓
CartItemDetailDto (returned with customizations)
    ↓
useCart Hook (displays customizations)
    ↓
CartPage / CheckoutPage (shows customization details)
```

### Key Components

1. **Frontend State Management**
   - `CustomizationStateService`: Manages local storage of customization selections
   - `SavedCustomizationState`: Type definition for stored customization data

2. **Cart Service Layer**
   - `cartService.addItem()`: Enhanced to accept and transmit customization options
   - `AddToCartRequest`: Extended to include customizations array
   - `AddToCartDto`: Already supports customizations field (from backend schema)

3. **API Integration**
   - `CartItemCustomizationDto`: Represents individual customization selections
   - Backend validates customization options against product definition
   - Backend calculates customization prices and updates cart totals

4. **Display Components**
   - `CartPage`: Shows customization details for each item
   - `CheckoutPage`: Includes customization info in order summary
   - `CartItemCustomizations`: Component to display and edit customizations

## Components and Interfaces

### 1. Enhanced AddToCartRequest Interface

```typescript
interface AddToCartRequest {
  productId: string;
  quantity: number;
  customizations?: Array<{
    optionId: string;
    quantity: number;
  }>;
}
```

### 2. CartService.addItem() Enhancement

The method signature remains the same, but the implementation now:
- Accepts `AddToCartRequest` with optional customizations
- Transforms the request to `AddToCartDto` including customizations
- Sends the complete payload to the backend

### 3. Component Integration Points

**EventSidebar.tsx**
- Retrieves saved customization state before adding to cart
- Passes customization data to `cartService.addItem()`

**ProductDetailPage.tsx**
- Retrieves customization state when adding to cart
- Includes customizations in the add-to-cart call

**CartPage.tsx**
- Displays customization options for each cart item
- Allows editing customizations (calls update endpoint)

**CheckoutPage.tsx**
- Shows customization details in order summary
- Includes customizations in checkout request

## Data Models

### SavedCustomizationState (Frontend)
```typescript
interface SavedCustomizationState {
  productId: string;
  productName: string;
  productType?: string;
  customizations: Array<{
    optionId: string;
    optionName: string;
    quantity: number;
    unitPrice: number;
    unit: string;
  }>;
  savedAt: string;
}
```

### CartItemCustomizationDto (Backend)
```typescript
interface CartItemCustomizationDto {
  optionId?: string | null;
  quantity?: number;
  unitPrice?: number;
  totalPrice?: number;
}
```

### CartItemDetailDto (Backend Response)
```typescript
interface CartItemDetailDto {
  cartItemId?: string;
  productId?: string;
  productName?: string | null;
  basePrice?: number;
  customizationPrice?: number;
  finalPrice?: number;
  customizations?: CartItemCustomizationDto[] | null;
  // ... other fields
}
```

## Correctness Properties

A property is a characteristic or behavior that should hold true across all valid executions of a system—essentially, a formal statement about what the system should do. Properties serve as the bridge between human-readable specifications and machine-verifiable correctness guarantees.

### Property 1: Customization State Storage
*For any* product with customization options and a valid customization selection, saving the customization state SHALL result in the state being retrievable from storage with all fields intact (optionId, quantity, unitPrice, unit).

**Validates: Requirements 1.1**

### Property 2: Customization Transmission
*For any* product with customization options and a valid customization state, when the product is added to cart via cartService.addItem(), the customization options SHALL be included in the add-to-cart API request payload.

**Validates: Requirements 1.2, 2.3**

### Property 3: Customization Persistence
*For any* cart item with customizations sent in an add-to-cart request, when the cart is retrieved from the backend, the returned cart item SHALL include all customization options with matching optionId, quantity, unitPrice, and totalPrice values.

**Validates: Requirements 1.3, 1.4, 4.2**

### Property 4: Customization Object Structure
*For any* customization object created for transmission, the object SHALL contain the required fields: optionId, quantity, and unitPrice, with quantity being a positive integer and unitPrice being a non-negative number.

**Validates: Requirements 2.2**

### Property 5: Customization Price Calculation
*For any* cart item with customizations, the final price SHALL equal the base price plus the sum of all customization option prices (calculated as quantity × unitPrice for each customization).

**Validates: Requirements 3.4, 4.3**

### Property 6: Customization Validation
*For any* customization option sent in an add-to-cart request, the backend SHALL validate that the option ID exists for the product and the quantity is within the min/max bounds defined for that option, rejecting invalid customizations with a 400 error.

**Validates: Requirements 2.4, 4.1**

### Property 7: Customization Update Round-Trip
*For any* cart item with customizations, updating the customization quantities via the update endpoint and retrieving the cart SHALL return the updated quantities and recalculated prices matching the new values.

**Validates: Requirements 3.2, 3.3**

### Property 8: Customization Serialization Round-Trip
*For any* cart item with customizations, serializing the cart item to JSON and deserializing it SHALL preserve all customization details (optionId, quantity, unitPrice, totalPrice) exactly as they were before serialization.

**Validates: Requirements 4.4**

## Error Handling

### Invalid Customization Options
- **Scenario**: Customer sends customization for an option that doesn't exist for the product
- **Handling**: Backend returns 400 Bad Request with error message
- **Frontend**: Display error toast and prevent checkout

### Out-of-Range Quantities
- **Scenario**: Customer sends quantity outside min/max bounds
- **Handling**: Backend returns 400 Bad Request with validation error
- **Frontend**: Display error and suggest valid range

### Missing Required Customizations
- **Scenario**: Product requires certain customizations but none are provided
- **Handling**: Backend returns 400 Bad Request
- **Frontend**: Display error and prompt user to select customizations

### Price Mismatch
- **Scenario**: Frontend calculates different price than backend
- **Handling**: Backend price is authoritative; frontend recalculates after response
- **Frontend**: Display backend-calculated price to user

## Testing Strategy

### Unit Testing

Unit tests verify specific examples and edge cases:

1. **CartService.addItem() with customizations**
   - Test that customizations are included in the request payload
   - Test with empty customizations array
   - Test with multiple customization options

2. **CustomizationStateService**
   - Test saving and retrieving customization state
   - Test clearing individual and all states
   - Test state-to-quantities conversion

3. **Price Calculation**
   - Test customization price calculation with various quantities
   - Test final price = base price + customization price
   - Test with zero customization price

### Property-Based Testing

Property-based tests verify universal properties using a testing framework like Vitest with fast-check:

1. **Property 1: Customization Transmission**
   - Generate random products with customization options
   - Generate random customization selections
   - Verify customizations appear in API request payload

2. **Property 2: Customization Persistence**
   - Generate random cart items with customizations
   - Mock backend response with same customizations
   - Verify retrieved cart contains all customizations

3. **Property 3: Customization Price Calculation**
   - Generate random customization quantities and unit prices
   - Verify: finalPrice = basePrice + sum(quantity × unitPrice)

4. **Property 4: Customization Validation**
   - Generate random customization options
   - Verify backend validation accepts valid options and rejects invalid ones

5. **Property 5: Customization Update Round-Trip**
   - Generate random initial customizations
   - Generate random updated quantities
   - Verify update request and response contain correct values

6. **Property 6: Customization Serialization**
   - Generate random cart items with customizations
   - Serialize to JSON and deserialize
   - Verify all fields are preserved exactly

### Testing Configuration

- **Framework**: Vitest with fast-check for property-based testing
- **Minimum Iterations**: 100 per property test
- **Coverage Target**: 80% for cart service layer
- **Test Location**: Co-located with source files using `.test.ts` suffix

