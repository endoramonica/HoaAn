# Service Product ID for Calendar Booking

## Overview
The calendar booking feature now requires a valid service product ID (GUID) to be set before users can book. This ID should come from the ServicePage or ServiceDetail page when a user selects a service.

## How It Works

### 1. Set Service Product ID (from ServicePage/ServiceDetail)
When a user selects a service on the ServicePage or ServiceDetail page, set the product ID in the AppContext:

```typescript
import { useApp } from '@/lib/contexts/AppContext';

function ServiceDetail() {
  const { setSelectedServiceProductId } = useApp();
  
  // When user selects a service
  const handleSelectService = (productId: string) => {
    setSelectedServiceProductId(productId);
    // Navigate to calendar or show booking option
  };
}
```

### 2. Use Service Product ID (in Calendar Booking)
The EventSidebar will automatically use the selected service product ID when creating a booking:

```typescript
// In EventSidebar.tsx
const { selectedServiceProductId } = useApp();

// The booking service will use this ID to add the service to cart
const result = await calendarBookingService.createBookingFromEvent({
  // ... other fields
  serviceProductId: selectedServiceProductId,
});
```

### 3. Booking Flow
1. User selects a service on ServicePage/ServiceDetail
2. `setSelectedServiceProductId(productId)` is called
3. User navigates to Calendar page
4. User selects a date and clicks "Đặt lịch"
5. Booking dialog opens
6. User fills in booking details
7. System adds service to guest cart using the product ID
8. System creates order via checkout API
9. User is redirected to checkout page

## API Flow

```
1. POST /api/v1/Cart/guest/add
   - productId: string (GUID from service)
   - quantity: 1

2. GET /api/v1/Cart/guest
   - Get cart ID

3. POST /api/v1/Checkout/process
   - cartId: string
   - shippingInfo: OrderShippingInputDto
   - notes: string
```

## Error Handling

If no service product ID is set:
- Booking button will show error: "Vui lòng chọn dịch vụ trước khi đặt lịch"
- User must go back to ServicePage and select a service first

## Implementation Checklist

- [x] AppContext updated with `selectedServiceProductId` state
- [x] EventSidebar checks for `selectedServiceProductId` before allowing booking
- [x] CalendarBookingService validates `serviceProductId` in request
- [x] Error messages guide user to select service first
- [ ] ServicePage/ServiceDetail needs to call `setSelectedServiceProductId()` when service is selected
- [ ] Navigation flow from ServicePage → Calendar should be implemented

## Next Steps

1. Update ServicePage/ServiceDetail to set the product ID when user selects a service
2. Add navigation from service selection to calendar page
3. Test the complete flow: Service Selection → Calendar → Booking → Checkout
