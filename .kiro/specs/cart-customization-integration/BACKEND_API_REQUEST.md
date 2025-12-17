# Backend API Request: Cart Customization Support

**Status:** ⏳ Waiting for Backend Implementation

---

## Request Summary

Frontend team needs Backend to implement customization support in the Cart API. This document specifies exactly what APIs need to be enhanced and what data structures are required.

---

## Required API Enhancements

### 1. Add-to-Cart API - Accept Customizations

**Endpoints:**
- `POST /api/v1/Cart/add` (authenticated users)
- `POST /api/v1/Cart/guest/add` (guest users)

**Current Behavior:**
```
Request:  { productId, quantity }
Response: { cartItemId, productId, quantity, unitPrice }
```

**Required Enhancement:**
Accept optional `customizations` array in request and return it in response.

**New Request Format:**
```json
{
  "productId": "125c1031-6df9-4bd2-b7f2-dfcc5cac55e8",
  "quantity": 1,
  "customizations": [
    {
      "optionId": "opt-xoi",
      "quantity": 10,
      "unitPrice": 45000
    },
    {
      "optionId": "opt-che",
      "quantity": 5,
      "unitPrice": 35000
    }
  ]
}
```

**New Response Format:**
```json
{
  "success": true,
  "data": {
    "cartItemId": "a0339e7f-4b4f-4bf2-b259-d126e4ec4aec",
    "productId": "125c1031-6df9-4bd2-b7f2-dfcc5cac55e8",
    "quantity": 1,
    "unitPrice": 4200000,
    "basePrice": 4200000,
    "customizationPrice": 800000,
    "finalPrice": 5000000,
    "customizations": [
      {
        "optionId": "opt-xoi",
        "quantity": 10,
        "unitPrice": 45000,
        "totalPrice": 450000
      },
      {
        "optionId": "opt-che",
        "quantity": 5,
        "unitPrice": 35000,
        "totalPrice": 350000
      }
    ]
  },
  "message": "Item added to cart successfully"
}
```

**Backend Responsibilities:**
- ✅ Accept `customizations` array (optional, can be null or empty)
- ✅ Validate each customization option exists for the product
- ✅ Validate quantities are within min/max bounds
- ✅ Calculate `totalPrice = quantity × unitPrice` for each customization
- ✅ Calculate `customizationPrice = sum of all customization totalPrices`
- ✅ Calculate `finalPrice = basePrice + customizationPrice`
- ✅ Store customizations with the cart item
- ✅ Return all customization details in response

**Error Handling:**
If customization validation fails, return 400 Bad Request:
```json
{
  "success": false,
  "message": "Invalid customization options",
  "errors": [
    {
      "optionId": "opt-invalid",
      "message": "Option does not exist for this product"
    },
    {
      "optionId": "opt-xoi",
      "message": "Quantity 25 exceeds maximum of 20"
    }
  ]
}
```

---

### 2. Get Cart API - Return Customizations

**Endpoints:**
- `GET /api/v1/Cart` (authenticated users)
- `GET /api/v1/Cart/guest` (guest users)

**Current Behavior:**
Returns cart items without customization details.

**Required Enhancement:**
Include customization details in each cart item.

**New Response Format:**
```json
{
  "success": true,
  "data": {
    "cartId": "b33aa0e4-3e32-49f9-9b49-e183631fc0d5",
    "userId": "d93a557c-a41e-4dd9-a0a7-d6b3897bc4a7",
    "items": [
      {
        "cartItemId": "a0339e7f-4b4f-4bf2-b259-d126e4ec4aec",
        "productId": "125c1031-6df9-4bd2-b7f2-dfcc5cac55e8",
        "productName": "Tư Vấn Phong Thủy Chuyên Sâu",
        "productImage": "https://...",
        "quantity": 1,
        "unitPrice": 4200000,
        "basePrice": 4200000,
        "customizationPrice": 800000,
        "finalPrice": 5000000,
        "availableStock": 100,
        "customizations": [
          {
            "optionId": "opt-xoi",
            "quantity": 10,
            "unitPrice": 45000,
            "totalPrice": 450000
          },
          {
            "optionId": "opt-che",
            "quantity": 5,
            "unitPrice": 35000,
            "totalPrice": 350000
          }
        ]
      }
    ],
    "subTotal": 5000000,
    "taxAmount": 0,
    "shippingFee": 0,
    "totalAmount": 5000000
  }
}
```

**Backend Responsibilities:**
- ✅ Include `basePrice` for each item
- ✅ Include `customizationPrice` for each item
- ✅ Include `finalPrice` for each item
- ✅ Include full `customizations` array with all details
- ✅ Ensure prices are calculated correctly

---

### 3. Update Cart Item API - Support Customization Updates

**Endpoints:**
- `PUT /api/v1/Cart/items/{cartItemId}` (authenticated users)
- `PUT /api/v1/Cart/guest/items/{cartItemId}` (guest users)

**Current Behavior:**
Only updates quantity.

**Required Enhancement:**
Support updating customizations for a cart item.

**New Request Format:**
```json
{
  "quantity": 1,
  "customizations": [
    {
      "optionId": "opt-xoi",
      "quantity": 15,
      "unitPrice": 45000
    },
    {
      "optionId": "opt-che",
      "quantity": 8,
      "unitPrice": 35000
    }
  ]
}
```

**New Response Format:**
```json
{
  "success": true,
  "data": {
    "cartItemId": "a0339e7f-4b4f-4bf2-b259-d126e4ec4aec",
    "productId": "125c1031-6df9-4bd2-b7f2-dfcc5cac55e8",
    "quantity": 1,
    "unitPrice": 4200000,
    "basePrice": 4200000,
    "customizationPrice": 1035000,
    "finalPrice": 5235000,
    "customizations": [
      {
        "optionId": "opt-xoi",
        "quantity": 15,
        "unitPrice": 45000,
        "totalPrice": 675000
      },
      {
        "optionId": "opt-che",
        "quantity": 8,
        "unitPrice": 35000,
        "totalPrice": 280000
      }
    ]
  },
  "message": "Cart item updated successfully"
}
```

**Backend Responsibilities:**
- ✅ Accept optional `customizations` array
- ✅ Validate new customization options
- ✅ Recalculate prices based on new customizations
- ✅ Update cart item with new customizations
- ✅ Return updated item with recalculated prices

---

### 4. Checkout API - Include Customizations

**Endpoint:**
- `POST /api/v1/Checkout/process`

**Current Behavior:**
Processes checkout with cart items but may not include customization details.

**Required Enhancement:**
Ensure customizations from cart items are included in the order.

**Request Format (No Change Needed):**
```json
{
  "cartId": "b33aa0e4-3e32-49f9-9b49-e183631fc0d5",
  "shippingInfo": {
    "recipientName": "Nguyễn Văn A",
    "phoneNumber": "0123456789",
    "address": "123 Đường ABC",
    "ward": "Phường 1",
    "district": "Quận 1",
    "city": "Hà Nội",
    "postalCode": "100000",
    "shippingMethod": "standard"
  },
  "notes": "Đặt lịch tư vấn..."
}
```

**Backend Responsibilities:**
- ✅ When processing checkout, retrieve all cart items including customizations
- ✅ Include customization details in the order
- ✅ Ensure order totals include customization prices
- ✅ Store customization details with the order for reference

---

## Data Structure Specifications

### CartItemCustomizationDto
```typescript
{
  optionId: string;           // Unique identifier for the customization option
  quantity: number;           // Quantity selected by customer
  unitPrice: number;          // Price per unit at time of addition
  totalPrice: number;         // quantity × unitPrice (calculated by backend)
}
```

### CartItemDetailDto (Enhanced)
```typescript
{
  cartItemId: string;
  productId: string;
  productName: string;
  productImage: string;
  quantity: number;
  unitPrice: number;
  basePrice: number;                    // Price without customizations
  customizationPrice: number;           // Sum of all customization totalPrices
  finalPrice: number;                   // basePrice + customizationPrice
  availableStock: number;
  customizations: CartItemCustomizationDto[];  // Array of customizations
}
```

---

## Validation Rules

### Customization Option Validation
- ✅ Option ID must exist for the product
- ✅ Quantity must be >= minQuantity defined for the option
- ✅ Quantity must be <= maxQuantity defined for the option (if maxQuantity is set)
- ✅ Return 400 Bad Request with detailed error if validation fails

### Price Calculation
- ✅ `totalPrice = quantity × unitPrice` for each customization
- ✅ `customizationPrice = sum of all customization totalPrices`
- ✅ `finalPrice = basePrice + customizationPrice`

---

## Implementation Checklist

- [ ] Add `customizations` field to `AddToCartDto`
- [ ] Add `customizations` field to `UpdateCartItemDto`
- [ ] Add `basePrice`, `customizationPrice`, `finalPrice` to `CartItemDetailDto`
- [ ] Add `customizations` field to `CartItemDetailDto`
- [ ] Implement customization validation in add-to-cart endpoint
- [ ] Implement customization validation in update endpoint
- [ ] Implement price calculation logic
- [ ] Update cart retrieval to include customizations
- [ ] Update checkout to include customizations in order
- [ ] Add error handling for invalid customizations
- [ ] Test all endpoints with customizations
- [ ] Update API documentation/Swagger

---

## Timeline

**Requested Completion:** ASAP

Once Backend implements these APIs, Frontend can proceed with:
1. Passing customizations to cart APIs
2. Displaying customization details
3. Allowing customization editing
4. Showing customization prices

---

## Questions?

If Backend team has questions about:
- Data structures: See `design.md`
- Requirements: See `requirements.md`
- Frontend usage: See `FRONTEND_BACKEND_SPLIT.md`

