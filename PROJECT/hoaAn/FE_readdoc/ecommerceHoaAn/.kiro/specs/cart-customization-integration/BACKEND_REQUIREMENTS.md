# Backend Requirements: Cart Customization Integration

## Overview

This document outlines what the **Backend** needs to implement to support customization options in the cart system. The frontend will only consume these APIs and display the data.

---

## Backend Requirements

### 1. Cart Add API - Support Customizations

**Endpoint:** `POST /api/v1/Cart/add` (authenticated) and `POST /api/v1/Cart/guest/add` (guest)

**Current Request:**
```json
{
  "productId": "125c1031-6df9-4bd2-b7f2-dfcc5cac55e8",
  "quantity": 1
}
```

**Required Enhancement:**
The API should accept an optional `customizations` array:

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

**Backend Responsibilities:**
- ✅ Accept the `customizations` array in `AddToCartDto`
- ✅ Validate that each customization option exists for the product
- ✅ Validate that quantities are within min/max bounds defined for each option
- ✅ Calculate customization prices: `totalPrice = quantity × unitPrice`
- ✅ Store customizations with the cart item
- ✅ Return customizations in the response

**Response Should Include:**
```json
{
  "success": true,
  "data": {
    "cartItemId": "a0339e7f-4b4f-4bf2-b259-d126e4ec4aec",
    "productId": "125c1031-6df9-4bd2-b7f2-dfcc5cac55e8",
    "quantity": 1,
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
        "totalPrice": 175000
      }
    ]
  }
}
```

---

### 2. Cart Get API - Return Customizations

**Endpoint:** `GET /api/v1/Cart` (authenticated) and `GET /api/v1/Cart/guest` (guest)

**Current Response:** Cart items without customization details

**Required Enhancement:**
Each cart item should include customization details:

```json
{
  "success": true,
  "data": {
    "cartId": "b33aa0e4-3e32-49f9-9b49-e183631fc0d5",
    "items": [
      {
        "cartItemId": "a0339e7f-4b4f-4bf2-b259-d126e4ec4aec",
        "productId": "125c1031-6df9-4bd2-b7f2-dfcc5cac55e8",
        "productName": "Tư Vấn Phong Thủy Chuyên Sâu",
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
            "totalPrice": 175000
          }
        ]
      }
    ],
    "subTotal": 5000000,
    "totalAmount": 5000000
  }
}
```

**Backend Responsibilities:**
- ✅ Include `basePrice`, `customizationPrice`, and `finalPrice` for each item
- ✅ Include full `customizations` array with all details
- ✅ Ensure prices are calculated correctly

---

### 3. Cart Update API - Support Customization Updates

**Endpoint:** `PUT /api/v1/Cart/items/{cartItemId}` (authenticated) and `PUT /api/v1/Cart/guest/items/{cartItemId}` (guest)

**Current Request:**
```json
{
  "quantity": 2
}
```

**Required Enhancement:**
Support updating customizations for a cart item:

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

**Backend Responsibilities:**
- ✅ Accept optional `customizations` array in update request
- ✅ Validate new customization options
- ✅ Recalculate prices based on new customizations
- ✅ Return updated cart item with new prices and customizations

**Response Should Include:**
```json
{
  "success": true,
  "data": {
    "cartItemId": "a0339e7f-4b4f-4bf2-b259-d126e4ec4aec",
    "productId": "125c1031-6df9-4bd2-b7f2-dfcc5cac55e8",
    "quantity": 1,
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
  }
}
```

---

### 4. Checkout API - Include Customizations

**Endpoint:** `POST /api/v1/Checkout/process`

**Current Request:** Includes cart items but not customization details

**Required Enhancement:**
Ensure checkout includes customization data from cart items:

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
- ✅ When processing checkout, include all customization details from cart items in the order
- ✅ Ensure order totals include customization prices
- ✅ Store customization details with the order for reference

---

## Data Structures

### CartItemCustomizationDto
```typescript
{
  optionId: string;           // e.g., "opt-xoi"
  quantity: number;           // e.g., 10
  unitPrice: number;          // e.g., 45000
  totalPrice: number;         // quantity × unitPrice
}
```

### CartItemDetailDto (Enhanced)
```typescript
{
  cartItemId: string;
  productId: string;
  productName: string;
  quantity: number;
  unitPrice: number;
  basePrice: number;                    // Price without customizations
  customizationPrice: number;           // Sum of all customization prices
  finalPrice: number;                   // basePrice + customizationPrice
  customizations: CartItemCustomizationDto[];
}
```

---

## Validation Rules

### Customization Option Validation
- ✅ Option ID must exist for the product
- ✅ Quantity must be >= minQuantity
- ✅ Quantity must be <= maxQuantity (if defined)
- ✅ Return 400 Bad Request if validation fails

### Error Response Format
```json
{
  "success": false,
  "message": "Invalid customization option",
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

## Summary of Changes Needed

| Endpoint | Change | Priority |
|----------|--------|----------|
| POST /api/v1/Cart/add | Accept & store customizations | High |
| POST /api/v1/Cart/guest/add | Accept & store customizations | High |
| GET /api/v1/Cart | Return customizations in items | High |
| GET /api/v1/Cart/guest | Return customizations in items | High |
| PUT /api/v1/Cart/items/{id} | Support customization updates | Medium |
| PUT /api/v1/Cart/guest/items/{id} | Support customization updates | Medium |
| POST /api/v1/Checkout/process | Include customizations in order | High |

---

## Frontend Will Handle

Once backend implements the above, frontend will:
- ✅ Retrieve saved customization state from localStorage
- ✅ Transform customization state to API format
- ✅ Pass customizations in add-to-cart requests
- ✅ Display customization details in cart page
- ✅ Allow editing customizations
- ✅ Show customization prices and totals
- ✅ Include customizations in checkout

