# Cart Customizations Display Implementation

## Overview
Updated the cart system to display customizations information from the backend response, showing all customization options selected by users with their prices.

## Changes Made

### 1. **src/lib/api/types.ts**
- Added `CartItemCustomization` interface to represent customization data:
  ```typescript
  interface CartItemCustomization {
    optionId?: string;
    quantity?: number;
    unitPrice?: number;
    totalPrice?: number;
  }
  ```
- Updated `CartItemDto` to include `customizations` field:
  ```typescript
  customizations?: CartItemCustomization[];
  ```

### 2. **src/lib/hooks/useCart.ts**
- Extended `CartItem` interface to include customization fields:
  - `basePrice`: Base price before customizations
  - `customizationPrice`: Total price of all customizations
  - `finalPrice`: Final total (basePrice + customizationPrice)
  - `customizations`: Array of customization objects
- Updated the transformation logic in `fetchCart()` to map customization data from API response

### 3. **src/components/CartPage.tsx**
- Imported `CartItemCustomizations` component
- Added customizations display section after quantity controls:
  ```tsx
  {item.customizations && item.customizations.length > 0 && (
    <CartItemCustomizations
      customizations={item.customizations}
      productName={item.name}
    />
  )}
  ```

### 4. **src/components/CartItemCustomizations.tsx**
- Refactored to work with the new data structure
- Displays each customization option with:
  - Option ID (e.g., "opt-fruit-tray")
  - Quantity selected
  - Unit price
  - Total price for that customization
- Shows total customization price at the bottom
- Handles formatting with Vietnamese locale

## Data Flow

```
Backend Response (CartItemDto)
  ↓
useCart Hook (transforms to CartItem)
  ↓
CartPage (displays items)
  ↓
CartItemCustomizations (renders customization details)
```

## Example Display

For a cart item with customizations:
```
Tùy chọn thêm:
  opt-fruit-tray × 1          600,000₫
  
Tổng tùy chọn:               600,000₫
```

## Response Structure
The backend now returns customizations in this format:
```json
{
  "customizations": [
    {
      "optionId": "opt-fruit-tray",
      "quantity": 1,
      "unitPrice": 600000,
      "totalPrice": 600000
    }
  ],
  "basePrice": 5500000,
  "customizationPrice": 600000,
  "finalPrice": 6100000
}
```

## Testing
- Customizations display only when present in the response
- Handles multiple customization options
- Correctly calculates and displays total customization price
- Properly formats prices in Vietnamese locale
