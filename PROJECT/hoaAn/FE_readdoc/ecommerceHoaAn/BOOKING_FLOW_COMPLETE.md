# Complete Service Booking Flow Implementation

## Overview
The service booking flow is now fully implemented, allowing users to:
1. Browse services on ServicesPage or ServiceDetailPage
2. Select a service and navigate to Calendar
3. Choose a date and book the service
4. Complete checkout and receive order confirmation

## Architecture

### 1. Context-Based Service Product ID Management
**File**: `src/lib/contexts/AppContext.tsx`

The AppContext now manages the selected service product ID:
```typescript
interface AppContextType {
  selectedServiceProductId: string | null;
  setSelectedServiceProductId: (productId: string | null) => void;
}
```

This allows any component to:
- Set the service product ID when user selects a service
- Get the service product ID when creating a booking

### 2. Service Selection Flow

#### ServicesPage (`src/components/ServicesPage.tsx`)
- Displays list of all services
- Each service card has a "Đặt dịch vụ" button
- When clicked:
  1. Sets `selectedServiceProductId` in context
  2. Navigates to `/calendar`

```typescript
<Button
  onClick={() => {
    if (service.id) {
      setSelectedServiceProductId(service.id);
      navigate("/calendar");
    }
  }}
  className="flex-1 bg-red-600 hover:bg-red-700 text-white"
>
  Đặt dịch vụ
</Button>
```

#### ServiceDetailPage (`src/components/ServiceDetailPage.tsx`)
- Displays detailed information about a single service
- Has a "Đặt dịch vụ ngay" button
- When clicked:
  1. Sets `selectedServiceProductId` in context
  2. Navigates to `/calendar`

```typescript
<Button
  onClick={() => {
    if (service.id) {
      setSelectedServiceProductId(service.id);
      navigate('/calendar');
    }
  }}
  className="w-full bg-red-600 hover:bg-red-700 text-white py-6 text-lg"
>
  Đặt dịch vụ ngay
</Button>
```

### 3. Calendar Booking Flow

#### CalendarPage (`src/components/CalendarPage.tsx`)
- User selects a date on the calendar
- Lunar date is calculated automatically
- EventSidebar displays booking button

#### EventSidebar (`src/pages/calendar/EventSidebar.tsx`)
- Retrieves `selectedServiceProductId` from context
- Validates that a service is selected before allowing booking
- Shows error if no service is selected: "Vui lòng chọn dịch vụ trước khi đặt lịch"
- Opens BookingDialog when user clicks "Đặt lịch"

```typescript
const { selectedServiceProductId } = useApp();

const handleBookingClick = () => {
  if (!selectedDate || !lunarData) {
    toast.error('Vui lòng chọn ngày');
    return;
  }
  if (!selectedServiceProductId) {
    toast.error('Vui lòng chọn dịch vụ trước khi đặt lịch');
    return;
  }
  setShowBookingDialog(true);
};
```

#### BookingDialog (`src/pages/calendar/BookingDialog.tsx`)
- Collects customer information:
  - Name, phone, email
  - Service date, duration, location
  - Additional notes
- Validates all required fields
- Saves customer info to localStorage for reuse

#### CalendarBookingService (`src/lib/services/calendarBookingService.ts`)
- Implements the booking flow:
  1. Validates `serviceProductId` is provided
  2. Adds service to guest cart using `postApiV1CartGuestAdd()`
  3. Gets cart ID using `getApiV1CartGuest()`
  4. Creates order via `postApiV1CheckoutProcess()` with:
     - cartId
     - shippingInfo (OrderShippingInputDto)
     - notes
  5. Returns orderId on success

```typescript
// Step 1: Add service product to guest cart
const addToCartResponse = await api.postApiV1CartGuestAdd({
  productId: serviceProductId,
  quantity: 1,
});

// Step 2: Get guest cart to get cart ID
const cartResponse = await api.getApiV1CartGuest();
const cartId = cartData?.id;

// Step 3: Prepare checkout request
const checkoutRequest = {
  cartId: cartId,
  shippingInfo: {
    recipientName: customerName,
    phoneNumber: customerPhone,
    address: `${serviceLocation} - Tư vấn dịch vụ`,
    ward: 'Phường 1',
    district: 'Quận 1',
    city: 'Hà Nội',
    postalCode: '100000',
    deliveryNote: `${event.title}\n...`,
    shippingMethod: 'standard',
  },
  notes: `${event.title}\n...`,
};

// Step 4: Call checkout API
const checkoutResponse = await api.postApiV1CheckoutProcess(checkoutRequest);
```

### 4. Checkout & Order Confirmation

#### CheckoutFlow (`src/pages/checkout/CheckoutFlow.tsx`)
- Displays booking information
- Shows order summary
- Handles payment processing
- Redirects to success page on completion

#### BookingSuccessPage (`src/pages/calendar/BookingSuccessPage.tsx`)
- Displays order confirmation
- Shows order details and booking information
- Provides next steps for customer

## Complete User Journey

```
1. User browses services
   ↓
2. User clicks "Đặt dịch vụ" on service card
   ↓
3. setSelectedServiceProductId(serviceId) is called
   ↓
4. User is navigated to /calendar
   ↓
5. User selects a date on calendar
   ↓
6. Lunar date is calculated
   ↓
7. User clicks "Đặt lịch" button
   ↓
8. BookingDialog opens
   ↓
9. User fills in booking details
   ↓
10. System validates form
    ↓
11. System calls calendarBookingService.createBookingFromEvent()
    ↓
12. Service adds product to cart (using serviceProductId)
    ↓
13. Service gets cart ID
    ↓
14. Service creates order via checkout API
    ↓
15. Order is created successfully
    ↓
16. User is redirected to /checkout?orderId={orderId}
    ↓
17. CheckoutFlow displays order details
    ↓
18. User completes payment
    ↓
19. User is redirected to /bookings/{orderId}
    ↓
20. BookingSuccessPage displays confirmation
```

## API Endpoints Used

### 1. Add to Guest Cart
```
POST /api/v1/Cart/guest/add
Body: {
  productId: string (GUID),
  quantity: number
}
```

### 2. Get Guest Cart
```
GET /api/v1/Cart/guest
Response: {
  id: string (cartId),
  items: [...],
  ...
}
```

### 3. Checkout Process
```
POST /api/v1/Checkout/process
Body: {
  cartId: string,
  shippingInfo: {
    recipientName: string,
    phoneNumber: string,
    address: string,
    ward: string,
    district: string,
    city: string,
    postalCode: string,
    deliveryNote: string,
    shippingMethod: 'standard' | 'express' | 'sameDay' | 'overnight'
  },
  notes: string
}
Response: {
  orderId: string,
  ...
}
```

## Error Handling

### Service Not Selected
- Error message: "Vui lòng chọn dịch vụ trước khi đặt lịch"
- User must go back to service page and select a service

### Invalid Product ID
- Error message: "Không thể lấy ID giỏ hàng. Vui lòng thử lại."
- User should try again or contact support

### Checkout Failure
- Error message: "Có lỗi xảy ra khi đặt lịch. Vui lòng thử lại sau."
- Console logs show detailed error information

## State Management

### AppContext
- `selectedServiceProductId`: Stores the selected service product ID
- Persists across navigation
- Cleared when user navigates away or selects a different service

### LocalStorage
- Customer information (name, phone, email) is saved for reuse
- Booking information is saved to sessionStorage during checkout

## Testing Checklist

- [ ] User can browse services on ServicesPage
- [ ] User can click "Đặt dịch vụ" and navigate to calendar
- [ ] Service product ID is set in context
- [ ] User can select a date on calendar
- [ ] Lunar date is calculated correctly
- [ ] User can click "Đặt lịch" button
- [ ] BookingDialog opens with form
- [ ] User can fill in booking details
- [ ] Form validation works correctly
- [ ] Customer info is saved to localStorage
- [ ] Booking service adds product to cart
- [ ] Cart ID is retrieved successfully
- [ ] Checkout API is called with correct parameters
- [ ] Order is created successfully
- [ ] User is redirected to checkout page
- [ ] Order details are displayed correctly
- [ ] User can complete payment
- [ ] User is redirected to success page
- [ ] Booking confirmation is displayed

## Files Modified

1. `src/lib/contexts/AppContext.tsx` - Added service product ID state
2. `src/lib/services/calendarBookingService.ts` - Updated to use service product ID
3. `src/pages/calendar/EventSidebar.tsx` - Added context usage and validation
4. `src/components/ServiceDetailPage.tsx` - Added context usage for booking
5. `src/components/ServicesPage.tsx` - Added context usage for booking

## Future Enhancements

1. Add service selection confirmation before navigating to calendar
2. Display selected service name/details on calendar page
3. Add ability to change service selection on calendar page
4. Implement service-specific booking options
5. Add service availability calendar
6. Implement real-time booking status updates
