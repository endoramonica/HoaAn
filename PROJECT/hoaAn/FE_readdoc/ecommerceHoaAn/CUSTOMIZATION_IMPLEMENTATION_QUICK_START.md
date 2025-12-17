# Customization State Management - Quick Start Guide

## What Was Created

✅ **useCustomizationState Hook** - State management for customizations
✅ **customizationService** - Business logic and utilities
✅ **CustomizationSelector Component** - UI for selecting customizations
✅ **Documentation** - Complete guide and examples

## Quick Integration (5 minutes)

### Step 1: Import Component

```typescript
import { CustomizationSelector } from '@/components/CustomizationSelector';
```

### Step 2: Add to Product Detail Page

```tsx
<CustomizationSelector
  productId={product.id}
  productName={product.name}
  basePrice={product.price}
  options={product.customizableOptions}
  onCustomizationsChange={(options, totalPrice) => {
    setCustomizations(options);
    setCustomizationPrice(totalPrice);
  }}
/>
```

### Step 3: Handle Add to Cart

```typescript
const handleAddToCart = async () => {
  try {
    await customizationService.addToCartWithCustomizations({
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
```

## Features

### 1. Save Temporary State
- User clicks "Lưu tạm thời"
- State saved to localStorage
- User can close browser and come back
- State automatically restored

### 2. Real-time Price Calculation
- Price updates as user changes quantities
- Shows base price + customization price
- Validates min/max bounds

### 3. Validation
- Checks min/max quantities
- Shows error messages
- Prevents invalid selections

### 4. Two Modes
- **Full mode**: For product detail page
- **Compact mode**: For modals/dialogs

## Usage Examples

### Example 1: Product Detail Page

```typescript
import { CustomizationSelector } from '@/components/CustomizationSelector';
import { customizationService } from '@/lib/services/customizationService';

export function ProductDetailPage() {
  const [customizations, setCustomizations] = useState<CustomizationOption[]>([]);
  const [customizationPrice, setCustomizationPrice] = useState(0);

  const handleAddToCart = async () => {
    const response = await customizationService.addToCartWithCustomizations({
      productId: product.id,
      quantity: 1,
      customizations
    });

    if (response.success) {
      toast.success('Đã thêm vào giỏ hàng');
      navigate('/cart');
    }
  };

  return (
    <div className="grid grid-cols-2 gap-8">
      <div>
        {/* Product info */}
      </div>

      <div className="space-y-4">
        <CustomizationSelector
          productId={product.id}
          productName={product.name}
          basePrice={product.price}
          options={product.customizableOptions}
          onCustomizationsChange={(options, totalPrice) => {
            setCustomizations(options);
            setCustomizationPrice(totalPrice);
          }}
        />

        <Button 
          onClick={handleAddToCart}
          className="w-full"
        >
          Thêm vào giỏ hàng ({(product.price + customizationPrice).toLocaleString('vi-VN')}₫)
        </Button>
      </div>
    </div>
  );
}
```

### Example 2: Booking Dialog

```typescript
import { CustomizationSelector } from '@/components/CustomizationSelector';
import { useCustomizationState } from '@/lib/hooks/useCustomizationState';

export function BookingDialog() {
  const { customizationState, saveState } = useCustomizationState();
  const [selectedDate, setSelectedDate] = useState<Date>();
  const [selectedTime, setSelectedTime] = useState<string>();

  const handleBooking = async () => {
    // Save customization state
    saveState();

    // Create booking with customizations
    const response = await bookingService.createBooking({
      serviceId: service.id,
      customizations: customizationState?.options,
      bookingDate: selectedDate,
      bookingTime: selectedTime
    });

    if (response.success) {
      toast.success('Đã đặt lịch thành công');
      navigate('/bookings');
    }
  };

  return (
    <Dialog>
      <DialogContent>
        <DialogHeader>
          <DialogTitle>Đặt lịch {service.name}</DialogTitle>
        </DialogHeader>

        <div className="space-y-4">
          {/* Date/Time picker */}

          <CustomizationSelector
            productId={service.id}
            productName={service.name}
            basePrice={service.price}
            options={service.customizableOptions}
            compact={true}
          />

          <Button onClick={handleBooking} className="w-full">
            Đặt lịch
          </Button>
        </div>
      </DialogContent>
    </Dialog>
  );
}
```

### Example 3: Load Saved State

```typescript
import { useCustomizationState } from '@/lib/hooks/useCustomizationState';

export function ProductDetailPage() {
  const { loadState, customizationState } = useCustomizationState();

  useEffect(() => {
    // Try to load saved state
    const savedState = loadState(product.id);
    
    if (savedState) {
      console.log('Loaded saved customizations:', savedState);
      // State will be automatically restored by useCustomizationState
    }
  }, [product.id]);

  return (
    // ...
  );
}
```

## Data Flow Diagram

```
┌─────────────────────────────────────────────────────────────┐
│                    Product Detail Page                       │
└─────────────────────────────────────────────────────────────┘
                            ↓
┌─────────────────────────────────────────────────────────────┐
│              CustomizationSelector Component                 │
│  - Display options                                           │
│  - Handle quantity changes                                   │
│  - Show price updates                                        │
│  - "Lưu tạm thời" button                                    │
└─────────────────────────────────────────────────────────────┘
                            ↓
┌─────────────────────────────────────────────────────────────┐
│            useCustomizationState Hook                        │
│  - Manage state in memory                                    │
│  - Calculate prices                                          │
│  - Validate selections                                       │
│  - Persist to localStorage                                   │
└─────────────────────────────────────────────────────────────┘
                            ↓
┌─────────────────────────────────────────────────────────────┐
│          customizationService                                │
│  - Add to cart with customizations                           │
│  - Calculate final price                                     │
│  - Validate customizations                                   │
│  - Manage sessionStorage                                     │
└─────────────────────────────────────────────────────────────┘
                            ↓
┌─────────────────────────────────────────────────────────────┐
│              Backend API                                     │
│  POST /api/v1/Cart/add                                       │
│  - Create cart item with customizations                      │
│  - Calculate final price                                     │
│  - Return order details                                      │
└─────────────────────────────────────────────────────────────┘
```

## Key Features

### 1. Automatic State Persistence
```typescript
// Save state
saveState(); // Saves to localStorage

// Load state
const savedState = loadState(productId); // Loads from localStorage

// Clear state
clearState(productId); // Removes from localStorage
```

### 2. Real-time Price Updates
```typescript
// Price updates automatically as user changes quantities
const totalPrice = getTotalPrice(); // Get current total

// Final price = base + customization
const finalPrice = basePrice + totalPrice;
```

### 3. Validation
```typescript
// Validate before saving
const validation = customizationService.validateCustomizations(options);

if (!validation.valid) {
  console.log('Errors:', validation.errors);
  // Show errors to user
}
```

### 4. Two UI Modes
```typescript
// Full mode (default)
<CustomizationSelector compact={false} />

// Compact mode (for modals)
<CustomizationSelector compact={true} />
```

## Common Tasks

### Task 1: Get Current Customizations
```typescript
const { customizationState } = useCustomizationState();
const options = customizationState?.options;
```

### Task 2: Calculate Total Price
```typescript
const { getTotalPrice } = useCustomizationState();
const totalPrice = getTotalPrice();
```

### Task 3: Reset to Defaults
```typescript
const { resetOptions } = useCustomizationState();
resetOptions(); // Reset all to base quantities
```

### Task 4: Save and Navigate
```typescript
const { saveState } = useCustomizationState();

const handleSave = () => {
  saveState();
  navigate('/booking');
};
```

## Testing

### Test 1: Save and Load State
```typescript
// 1. Initialize customization
initializeCustomization(productId, productName, options);

// 2. Update quantities
updateOptionQuantity('opt-1', 2);
updateOptionQuantity('opt-2', 3);

// 3. Save state
saveState();

// 4. Reload page
window.location.reload();

// 5. Verify state is restored
// Should see same quantities as before
```

### Test 2: Price Calculation
```typescript
// 1. Base price: 5,500,000₫
// 2. Option 1: +1 quantity × 600,000₫ = 600,000₫
// 3. Option 2: +0 quantity × 0₫ = 0₫
// 4. Total: 5,500,000 + 600,000 = 6,100,000₫

// Verify in UI
```

### Test 3: Validation
```typescript
// 1. Try to set quantity below min
// Should show error: "Minimum quantity is X"

// 2. Try to set quantity above max
// Should show error: "Maximum quantity is X"

// 3. Try to save with invalid selections
// Should show validation errors
```

## Troubleshooting

### Issue: State not persisting
**Solution:** Check localStorage is enabled in browser

### Issue: Price not updating
**Solution:** Verify `onCustomizationsChange` callback is called

### Issue: Validation not working
**Solution:** Check `validateCustomizations` is called before save

### Issue: Component not rendering
**Solution:** Verify `customizableOptions` is not empty

## Next Steps

1. ✅ Copy files to your project
2. ✅ Import CustomizationSelector in ProductDetailPage
3. ✅ Add customizable options to product data
4. ✅ Test save/load functionality
5. ✅ Test add to cart with customizations
6. ✅ Test booking with customizations

## Support

For detailed documentation, see: `CUSTOMIZATION_STATE_MANAGEMENT.md`

For API integration, see: `CUSTOMIZATION_IMPLEMENTATION_COMPLETE.md`
