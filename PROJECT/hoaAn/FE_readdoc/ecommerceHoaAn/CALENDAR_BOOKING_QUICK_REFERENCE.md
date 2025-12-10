# Calendar Booking - Quick Reference

## Files Created/Updated

| File | Status | Purpose |
|------|--------|---------|
| `src/lib/services/calendarBookingService.ts` | ✅ NEW | Service xử lý booking |
| `src/pages/calendar/EventSidebar.tsx` | ✅ UPDATED | Thêm nút "Đặt lịch" |
| `src/pages/calendar/BookingDialog.tsx` | ✅ NEW | Dialog nhập thông tin |
| `src/pages/calendar/BookingSuccessPage.tsx` | ✅ NEW | Trang chi tiết booking |
| `src/pages/calendar/BookingHistoryPage.tsx` | ✅ NEW | Trang lịch sử booking |
| `src/lib/hooks/useCalendarBooking.ts` | ✅ NEW | Hook xử lý booking |
| `src/lib/hooks/useBookingOrder.ts` | ✅ NEW | Hook lấy booking |
| `src/App.tsx` | ✅ UPDATED | Thêm routes |

## Routes

```
GET  /calendar                    - Trang lịch
GET  /bookings                    - Lịch sử booking
GET  /bookings/:orderId           - Chi tiết booking
```

## API Endpoints

```
POST /api/v1/Checkout/process              - Tạo đơn hàng
GET  /api/v1/Checkout/{orderId}            - Lấy chi tiết
POST /api/v1/Checkout/{orderId}/cancel     - Hủy đơn hàng
GET  /api/v1/Order/my-orders               - Lịch sử đơn hàng
GET  /api/v1/Order/{orderId}/status-history - Lịch sử trạng thái
```

## Usage Examples

### 1. Sử dụng useCalendarBooking hook
```typescript
import { useCalendarBooking } from '@/lib/hooks/useCalendarBooking';

function MyComponent() {
  const { createBooking, isLoading, isSuccess, error } = useCalendarBooking();

  const handleBooking = async () => {
    const result = await createBooking({
      event: selectedEvent,
      lunarData: lunarData,
      selectedDate: selectedDate,
      customerName: 'John Doe',
      customerPhone: '0123456789',
      customerEmail: 'john@example.com',
    });
  };

  return (
    <button onClick={handleBooking} disabled={isLoading}>
      {isLoading ? 'Đang xử lý...' : 'Đặt lịch'}
    </button>
  );
}
```

### 2. Sử dụng useBookingOrder hook
```typescript
import { useBookingOrder } from '@/lib/hooks/useBookingOrder';

function BookingDetail({ orderId }) {
  const { data: order, isLoading, error } = useBookingOrder(orderId);

  if (isLoading) return <div>Loading...</div>;
  if (error) return <div>Error: {error.message}</div>;

  return <div>{order.id}</div>;
}
```

### 3. Gọi service trực tiếp
```typescript
import { calendarBookingService } from '@/lib/services/calendarBookingService';

const result = await calendarBookingService.createBookingFromEvent({
  event: selectedEvent,
  lunarData: lunarData,
  selectedDate: selectedDate,
  customerName: 'John Doe',
  customerPhone: '0123456789',
  customerEmail: 'john@example.com',
});

if (result.success) {
  console.log('Booking ID:', result.orderId);
} else {
  console.error('Error:', result.message);
}
```

## Form Validation Rules

| Field | Rules |
|-------|-------|
| Tên | Bắt buộc, không trống |
| SĐT | Bắt buộc, 10-11 chữ số |
| Email | Bắt buộc, định dạng email |

## Status Codes

| Status | Label | Color |
|--------|-------|-------|
| pending | Chờ xác nhận | Yellow |
| confirmed | Đã xác nhận | Blue |
| processing | Đang xử lý | Purple |
| completed | Hoàn thành | Green |
| cancelled | Đã hủy | Red |

## localStorage Keys

```
customerName      - Tên khách hàng
customerPhone     - Số điện thoại
customerEmail     - Email
```

## Error Messages

| Error | Message |
|-------|---------|
| Empty name | "Vui lòng nhập tên" |
| Invalid phone | "Số điện thoại không hợp lệ" |
| Invalid email | "Email không hợp lệ" |
| API error | "Không thể tạo đơn hàng. Vui lòng thử lại." |
| Network error | "Có lỗi xảy ra khi đặt lịch. Vui lòng thử lại sau." |

## Success Messages

```
"Đặt lịch thành công! Nhân viên sẽ gọi lại để tư vấn chi tiết."
```

## Component Props

### BookingDialog
```typescript
interface BookingDialogProps {
  open: boolean;
  onOpenChange: (open: boolean) => void;
  onConfirm: (data: BookingFormData) => Promise<void>;
  isLoading?: boolean;
  eventTitle?: string;
}
```

### EventSidebar
```typescript
interface EventSidebarProps {
  events: CalendarEvent[];
  selectedDate: Date | null;
  lunarData: LunarDateResult | null;
  isLoadingLunar: boolean;
  selectedEvent: CalendarEvent | null;
  onEventSelect: (event: CalendarEvent) => void;
  onCloseEvent: () => void;
}
```

## Data Flow

```
EventSidebar
  ↓ (click "Đặt lịch")
BookingDialog
  ↓ (submit form)
useCalendarBooking hook
  ↓ (call service)
calendarBookingService
  ↓ (call API)
POST /api/v1/Checkout/process
  ↓ (response)
BookingSuccessPage
  ↓ (navigate)
/bookings/:orderId
```

## Testing Checklist

- [ ] Booking thành công
- [ ] Validation lỗi
- [ ] API lỗi
- [ ] Xem chi tiết booking
- [ ] Xem lịch sử booking
- [ ] localStorage hoạt động
- [ ] Responsive design
- [ ] Accessibility

## Common Issues & Solutions

| Issue | Solution |
|-------|----------|
| Dialog không hiển thị | Kiểm tra `showBookingDialog` state |
| Thông tin không lưu | Kiểm tra localStorage |
| API error | Kiểm tra backend API |
| Route not found | Kiểm tra App.tsx routes |

## Performance Tips

1. Lazy load BookingSuccessPage
2. Cache booking data (5 minutes)
3. Debounce form input
4. Optimize images
5. Minimize bundle size

## Browser Support

- Chrome 90+
- Firefox 88+
- Safari 14+
- Edge 90+

## Dependencies

- react-router-dom
- @tanstack/react-query
- sonner (toast)
- lucide-react (icons)
- tailwindcss (styling)

## Environment Variables

Không cần environment variables khác. Sử dụng API base URL từ `Api/generated-orval`.

## Deployment

1. Build: `npm run build`
2. Test: `npm run dev`
3. Deploy: Push to production

## Support

Xem `CALENDAR_BOOKING_INTEGRATION.md` để biết thêm chi tiết.

---

**Last Updated**: December 7, 2025
**Version**: 1.0.0
