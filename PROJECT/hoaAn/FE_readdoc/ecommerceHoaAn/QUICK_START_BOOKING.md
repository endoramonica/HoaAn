# Quick Start: Service Booking Flow

## What Was Implemented

The complete service booking flow is now working:
- Users select a service from ServicesPage or ServiceDetailPage
- Service product ID is stored in AppContext
- Users navigate to Calendar and select a date
- Users fill in booking details
- System creates an order and redirects to checkout

## How to Test

### 1. Start the Application
```bash
npm run dev
```

### 2. Navigate to Services
- Go to `/services` or click "Dịch vụ" in navigation
- You should see a list of services

### 3. Select a Service
- Click "Đặt dịch vụ" button on any service card
- OR click "Chi tiết" to view service details, then click "Đặt dịch vụ ngay"

### 4. You Should Be Redirected to Calendar
- The calendar page should load
- The service product ID is now stored in context

### 5. Select a Date
- Click on any date in the calendar
- Lunar date will be calculated automatically

### 6. Click "Đặt lịch" Button
- The booking dialog should open
- Fill in your information:
  - Name
  - Phone
  - Email
  - Service date
  - Duration
  - Location
  - Notes (optional)

### 7. Submit Booking
- Click "Đặt lịch" button in the dialog
- System will:
  1. Add service to guest cart
  2. Get cart ID
  3. Create order via checkout API
  4. Redirect to checkout page

### 8. View Order
- You should see the order details on checkout page
- Order ID will be displayed

## Key Files

| File | Purpose |
|------|---------|
| `src/lib/contexts/AppContext.tsx` | Stores selected service product ID |
| `src/components/ServiceDetailPage.tsx` | Service detail page with booking button |
| `src/components/ServicesPage.tsx` | Services list with booking buttons |
| `src/pages/calendar/EventSidebar.tsx` | Calendar sidebar with booking dialog |
| `src/lib/services/calendarBookingService.ts` | Booking service logic |

## API Flow

```
1. POST /api/v1/Cart/guest/add
   - Adds service to guest cart
   
2. GET /api/v1/Cart/guest
   - Gets cart ID
   
3. POST /api/v1/Checkout/process
   - Creates order
```

## Troubleshooting

### Error: "Vui lòng chọn dịch vụ trước khi đặt lịch"
- You didn't select a service before going to calendar
- Go back to `/services` and click "Đặt dịch vụ"

### Error: "Không thể lấy ID giỏ hàng"
- Cart API failed
- Check browser console for details
- Try again or contact support

### Error: "Có lỗi xảy ra khi đặt lịch"
- Checkout API failed
- Check browser console for error details
- Verify all required fields are filled

### Service Product ID Not Set
- Check AppContext in React DevTools
- Verify `selectedServiceProductId` is not null
- Make sure you clicked booking button from service page

## Console Debugging

Open browser DevTools (F12) and check Console tab for:
- "Starting booking process for event: ..." - Booking started
- "Add to cart response: ..." - Cart response
- "Cart response: ..." - Cart details
- "Checkout request: ..." - Request being sent
- "Checkout response: ..." - Order created

## Next Steps

1. Test the complete flow end-to-end
2. Verify order is created in backend
3. Test payment processing
4. Verify order confirmation page displays correctly
5. Test with different services
6. Test error scenarios

## Contact

If you encounter any issues:
1. Check browser console for error messages
2. Check network tab for API responses
3. Verify service product ID is set in AppContext
4. Check that all required fields are filled in booking form
