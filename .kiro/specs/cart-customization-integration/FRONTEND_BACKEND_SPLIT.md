# Frontend-Backend Split: Cart Customization Integration

## Overview

This document clarifies the division of work between Frontend and Backend for the Cart Customization Integration feature.

---

## Backend Responsibilities

The **Backend Team** must implement the following API enhancements:

### 1. Add-to-Cart API Enhancement
- **Endpoint:** `POST /api/v1/Cart/add` and `POST /api/v1/Cart/guest/add`
- **Change:** Accept optional `customizations` array in request
- **Validation:** Validate customization options exist and quantities are within bounds
- **Response:** Return customizations with calculated prices

### 2. Get Cart API Enhancement
- **Endpoint:** `GET /api/v1/Cart` and `GET /api/v1/Cart/guest`
- **Change:** Include customization details in cart item responses
- **Include:** `basePrice`, `customizationPrice`, `finalPrice`, and `customizations` array

### 3. Update Cart Item API Enhancement
- **Endpoint:** `PUT /api/v1/Cart/items/{id}` and `PUT /api/v1/Cart/guest/items/{id}`
- **Change:** Support updating customizations for a cart item
- **Response:** Return updated item with recalculated prices

### 4. Checkout API Enhancement
- **Endpoint:** `POST /api/v1/Checkout/process`
- **Change:** Ensure customizations from cart items are included in the order
- **Validation:** Validate customization data is preserved

**See:** `BACKEND_REQUIREMENTS.md` for detailed specifications

---

## Frontend Responsibilities

The **Frontend Team** will implement the following:

### 1. CartService Updates
- Update `AddToCartRequest` interface to include customizations
- Modify `cartService.addItem()` to pass customizations to backend
- Support both authenticated and guest cart operations

### 2. Component Updates
- **EventSidebar:** Retrieve customization state and pass to cart
- **ProductDetailPage:** Retrieve customization state and pass to cart
- **CartPage:** Display customization details and allow editing
- **CheckoutPage:** Display customization summary

### 3. New Components
- **CustomizationDisplay:** Show customization details for a cart item
- **CustomizationEditor:** Allow editing customization quantities

### 4. Data Handling
- Transform `SavedCustomizationState` to `CartItemCustomizationDto[]` format
- Handle customization data from backend responses
- Display prices correctly (basePrice + customizationPrice = finalPrice)

**See:** `tasks.md` for detailed frontend tasks

---

## Data Flow

```
Frontend UI (CustomizationSelector)
    ↓
CustomizationStateService (localStorage)
    ↓
EventSidebar / ProductDetail (retrieves state)
    ↓
CartService.addItem() (includes customizations)
    ↓
Backend API: POST /api/v1/Cart/add
    ↓
Backend: Validates & stores customizations
    ↓
Backend API Response: Returns customizations with prices
    ↓
useCart Hook (displays customizations)
    ↓
CartPage / CheckoutPage (shows customization details)
```

---

## API Contract

### Request Format (Frontend → Backend)

```json
{
  "productId": "125c1031-6df9-4bd2-b7f2-dfcc5cac55e8",
  "quantity": 1,
  "customizations": [
    {
      "optionId": "opt-xoi",
      "quantity": 10,
      "unitPrice": 45000
    }
  ]
}
```

### Response Format (Backend → Frontend)

```json
{
  "cartItemId": "a0339e7f-4b4f-4bf2-b259-d126e4ec4aec",
  "productId": "125c1031-6df9-4bd2-b7f2-dfcc5cac55e8",
  "quantity": 1,
  "basePrice": 4200000,
  "customizationPrice": 450000,
  "finalPrice": 4650000,
  "customizations": [
    {
      "optionId": "opt-xoi",
      "quantity": 10,
      "unitPrice": 45000,
      "totalPrice": 450000
    }
  ]
}
```

---

## Implementation Order

### Phase 1: Backend Implementation
1. Backend team implements API enhancements (see `BACKEND_REQUIREMENTS.md`)
2. Backend provides test endpoints or mock data
3. Backend team notifies frontend when ready

### Phase 2: Frontend Implementation
1. Frontend team implements CartService updates
2. Frontend team implements component updates
3. Frontend team tests with backend APIs
4. Frontend team implements CustomizationDisplay and CustomizationEditor
5. Frontend team tests complete flow

### Phase 3: Integration Testing
1. Both teams test end-to-end flow
2. Verify data consistency
3. Test error handling
4. Performance testing

---

## Key Points

✅ **Frontend does NOT:**
- Validate customization options (backend does this)
- Calculate prices (backend does this)
- Store customizations in database (backend does this)
- Handle business logic for customizations (backend does this)

✅ **Frontend DOES:**
- Retrieve saved customization state from localStorage
- Transform data to API format
- Pass customizations to backend APIs
- Display customization details from backend responses
- Allow users to edit customizations
- Show prices calculated by backend

✅ **Backend does NOT:**
- Handle UI display
- Manage localStorage
- Handle user interactions

✅ **Backend DOES:**
- Validate customization options
- Calculate prices
- Store customizations with cart items
- Return customizations in responses
- Handle customization updates

---

## Testing Strategy

### Frontend Testing
- Unit tests for CartService customization handling
- Component tests for CustomizationDisplay and CustomizationEditor
- Integration tests with mock backend responses

### Backend Testing
- Unit tests for customization validation
- Unit tests for price calculation
- Integration tests with database
- API contract tests

### End-to-End Testing
- Test complete flow with real backend
- Test error scenarios
- Test edge cases

---

## Timeline

| Phase | Team | Duration | Status |
|-------|------|----------|--------|
| Backend API Implementation | Backend | 2-3 days | Pending |
| Frontend Implementation | Frontend | 2-3 days | Ready to start after backend |
| Integration Testing | Both | 1-2 days | After both phases complete |

---

## Questions & Support

**For Backend Questions:**
- See `BACKEND_REQUIREMENTS.md` for detailed API specifications
- See `design.md` for data model details

**For Frontend Questions:**
- See `tasks.md` for detailed frontend tasks
- See `requirements.md` for feature requirements

