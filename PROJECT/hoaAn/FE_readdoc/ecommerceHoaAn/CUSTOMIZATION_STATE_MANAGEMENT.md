# Customization State Management - Complete Guide

## Overview

Hệ thống quản lý state tạm thời cho customizable options, cho phép user:
- ✅ Chọn số lượng cho mỗi tùy chọn
- ✅ Xem giá cập nhật tự động
- ✅ Lưu lựa chọn tạm thời (localStorage)
- ✅ Quay lại sau và tiếp tục
- ✅ Thêm vào giỏ hàng hoặc booking

## Architecture

```
ProductDetailPage
  ↓
CustomizationSelector (UI Component)
  ↓
useCustomizationState (State Management)
  ↓
customizationService (Business Logic)
  ↓
localStorage (Persistence)
```

## Components & Hooks

### 1. useCustomizationState Hook

**File:** `src/lib/hooks/useCustomizationState.ts`

**Purpose:** Quản lý state customizations

**Usage:**
```typescript
const {
  customizationState,      // Current state
  selectedOptions,         // Map of selected quantities
  totalPrice,             // Total customization price
  initializeCustomization, // Initialize with options
  updateOptionQuantity,    // Update quantity for option
  getSelectedOptions,      // Get selected options
  getTotalPrice,          // Get total price
  saveState,              // Save to localStorage
  loadState,              // Load from localStorage
  clearState,             // Clear saved state
  resetOptions            // Reset to defaults
} = useCustomizationState();
```

**Example:**
```typescript
useEffect(() => {
  initializeCustomization(productId, productName, options);
}, [productId, productName, options]);

const handleQuantityChange = (optionId: string, quantity: number) => {
  updateOptionQuantity(optionId, quantity);
};

const handleSave = () => {
  saveState();
  toast.success('Đã lưu lựa chọn');
};
```

### 2. CustomizationSelector Component

**File:** `src/components/CustomizationSelector.tsx`

**Purpose:** UI để chọn customizations

**Props:**
```typescript
interface CustomizationSelectorProps {
  productId: string;
  productName: string;
  basePrice: number;
  options: CustomizationOption[];
  onCustomizationsChange?: (options: CustomizationOption[], totalPrice: number) => void;
  onSave?: (options: CustomizationOption[]) => void;
  compact?: boolean; // Compact mode for modal
}
```

**Usage:**
```tsx
<CustomizationSelector
  productId={product.id}
  productName={product.name}
  basePrice={product.price}
  options={product.customizableOptions}
  onCustomizationsChange={(options, totalPrice) => {
    console.log('Customizations changed:', options, totalPrice);
  }}
  onSave={(options) => {
    console.log('Customizations saved:', options);
  }}
  compact={false}
/>
```

### 3. customizationService

**File:** `src/lib/services/customizationService.ts`

**Purpose:** Business logic cho customizations

**Methods:**
```typescript
// Add to cart with customizations
addToCartWithCustomizations(request: AddToCartWithCustomizationRequest)

// Save state to sessionStorage
saveCustomizationState(productId: string, customizations: CustomizationOption[])

// Load state from sessionStorage
loadCustomizationState(productId: string)

// Clear saved state
clearCustomizationState(productId: string)

// Calculate customization price
calculateCustomizationPrice(customizations: CustomizationOption[])

// Calculate final price
calculateFinalPrice(basePrice: number, customizations: CustomizationOption[])

// Validate customizations
validateCustomizations(customizations: CustomizationOption[])
```

## Data Flow

### 1. Initialize Customization

```
User opens product detail page
  ↓
ProductDetailPage loads customizable options
  ↓
CustomizationSelector initializes
  ↓
useCustomizationState.initializeCustomization()
  ↓
Try to load saved state from localStorage
  ↓
If exists: Restore saved selections
If not: Initialize with default (base quantity)
```

### 2. Update Selection

```
User changes quantity
  ↓
CustomizationSelector.handleQuantityChange()
  ↓
useCustomizationState.updateOptionQuantity()
  ↓
Update state in memory
  ↓
Calculate new total price
  ↓
Call onCustomizationsChange callback
  ↓
Parent component updates UI
```

### 3. Save State

```
User clicks "Lưu tạm thời"
  ↓
CustomizationSelector.handleSaveState()
  ↓
Validate customizations
  ↓
useCustomizationState.saveState()
  ↓
Save to localStorage
  ↓
Show success toast
  ↓
Call onSave callback
```

### 4. Add to Cart

```
User clicks "Thêm vào giỏ hàng"
  ↓
ProductDetailPage.handleAddToCart()
  ↓
Get customizations from useCustomizationState
  ↓
customizationService.addToCartWithCustomizations()
  ↓
Send request to backend with customizations
  ↓
Backend creates cart item with customizations
  ↓
Return response with final price
  ↓
Clear customization state
  ↓
Show success toast
```

## Storage Structure

### localStorage (useCustomizationState)

```json
{
  "customization_state": {
    "product-id-1": {
      "productId": "product-id-1",
      "productName": "Lễ Tân Gia Trọn Gói",
      "options": [
        {
          "optionId": "opt-fruit-tray",
          "name": "Mâm trái cây",
          "baseQuantity": 1,
          "unitPrice": 600000,
          "minQuantity": 1,
          "maxQuantity": 5,
          "unit": "mâm",
          "selectedQuantity": 2
        }
      ],
      "totalCustomizationPrice": 600000,
      "savedAt": "2025-12-16T10:30:00Z"
    }
  }
}
```

### sessionStorage (customizationService)

```json
{
  "customization_product-id-1": {
    "productId": "product-id-1",
    "customizations": [
      {
        "optionId": "opt-fruit-tray",
        "selectedQuantity": 2,
        "unitPrice": 600000
      }
    ],
    "savedAt": "2025-12-16T10:30:00Z"
  }
}
```

## Integration Examples

### Example 1: Product Detail Page

```typescript
import { CustomizationSelector } from '@/components/CustomizationSelector';

export function ProductDetailPage() {
  const [customizations, setCustomizations] = useState<CustomizationOption[]>([]);
  const [customizationPrice, setCustomizationPrice] = useState(0);

  const handleAddToCart = async () => {
    try {
      const response = await customizationService.addToCartWithCustomizations({
        productId: product.id,
        quantity: 1,
        customizations
      });

      toast.success('Đã thêm vào giỏ hàng');
      navigate('/cart');
    } catch (err) {
      toast.error('Không thể thêm vào giỏ hàng');
    }
  };

  return (
    <div>
      <CustomizationSelector
        productId={product.id}
        productName={product.name}
        basePrice={product.price}
        options={product.customizableOptions}
        onCustomizationsChange={(options, totalPrice) => {
          setCustomizations(options);
          setCustomizationPrice(totalPrice);
        }}
        onSave={(options) => {
          console.log('Saved:', options);
        }}
      />

      <Button onClick={handleAddToCart}>
        Thêm vào giỏ hàng ({(product.price + customizationPrice).toLocaleString('vi-VN')}₫)
      </Button>
    </div>
  );
}
```

### Example 2: Booking with Customizations

```typescript
import { useCustomizationState } from '@/lib/hooks/useCustomizationState';

export function BookingDialog() {
  const { customizationState, saveState } = useCustomizationState();

  const handleBooking = async () => {
    // Save customization state
    saveState();

    // Send booking request with customizations
    const bookingRequest = {
      serviceId: service.id,
      customizations: customizationState?.options,
      bookingDate: selectedDate,
      bookingTime: selectedTime
    };

    const response = await bookingService.createBooking(bookingRequest);
    
    if (response.success) {
      toast.success('Đã đặt lịch thành công');
      navigate('/bookings');
    }
  };

  return (
    <Dialog>
      <DialogContent>
        <CustomizationSelector
          productId={service.id}
          productName={service.name}
          basePrice={service.price}
          options={service.customizableOptions}
          compact={true}
        />

        <Button onClick={handleBooking}>
          Đặt lịch
        </Button>
      </DialogContent>
    </Dialog>
  );
}
```

## Price Calculation

### Formula

```
Final Price = Base Price + Customization Price

Customization Price = Σ (Selected Quantity - Base Quantity) × Unit Price

Example:
- Base Price: 5,500,000₫
- Option 1: Base 1, Selected 2, Unit Price 600,000₫
  → Additional: (2 - 1) × 600,000 = 600,000₫
- Option 2: Base 1, Selected 1, Unit Price 0₫
  → Additional: (1 - 1) × 0 = 0₫

Final Price = 5,500,000 + 600,000 = 6,100,000₫
```

## Validation Rules

```typescript
// Min/Max Quantity
selectedQuantity >= minQuantity
selectedQuantity <= maxQuantity

// Price Calculation
customizationPrice >= 0
finalPrice = basePrice + customizationPrice

// State Persistence
savedAt must be valid ISO string
productId must not be empty
```

## Error Handling

```typescript
// Validation Error
{
  valid: false,
  errors: [
    "Mâm trái cây: Minimum quantity is 1",
    "Mâm trái cây: Maximum quantity is 5"
  ]
}

// Storage Error
try {
  saveState();
} catch (err) {
  console.error('Failed to save state:', err);
  toast.error('Không thể lưu lựa chọn');
}
```

## Best Practices

1. **Always validate** before saving or adding to cart
2. **Clear state** after successful add to cart
3. **Use compact mode** for modals/dialogs
4. **Provide feedback** with toast notifications
5. **Log important actions** for debugging
6. **Handle errors gracefully** with user-friendly messages

## Testing Checklist

- [ ] Initialize customization with options
- [ ] Update quantity for each option
- [ ] Validate min/max bounds
- [ ] Calculate price correctly
- [ ] Save state to localStorage
- [ ] Load state from localStorage
- [ ] Clear state properly
- [ ] Reset to defaults
- [ ] Add to cart with customizations
- [ ] Handle validation errors
- [ ] Handle storage errors

## Performance Considerations

- State updates are optimized with useCallback
- localStorage operations are wrapped in try-catch
- Validation happens before save/add
- Price calculation is memoized
- Component re-renders only when necessary

## Future Enhancements

- [ ] Sync customization state across tabs
- [ ] Add undo/redo functionality
- [ ] Support for customization presets
- [ ] Analytics tracking for customizations
- [ ] A/B testing for customization options
