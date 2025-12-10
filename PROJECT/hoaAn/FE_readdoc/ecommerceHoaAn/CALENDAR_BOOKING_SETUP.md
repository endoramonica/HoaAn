# Calendar Booking - Setup & Integration

## 1. Thêm Route cho BookingSuccessPage

Mở file router config (thường là `src/App.tsx` hoặc `src/routes.tsx`):

```typescript
import { BookingSuccessPage } from './pages/calendar/BookingSuccessPage';

// Thêm route này vào router config
<Route path="/bookings/:orderId" element={<BookingSuccessPage />} />
```

## 2. Cập nhật EventSidebar trong Calendar

EventSidebar đã được cập nhật tự động. Nó bây giờ có:
- ✅ Nút "Đặt lịch"
- ✅ Dialog nhập thông tin khách hàng
- ✅ Hiển thị kết quả booking
- ✅ Nút "Xem đơn hàng"

## 3. Sử dụng trong ServicePage/ServiceDetailPage

Để tích hợp Calendar vào ServicePage hoặc ServiceDetailPage:

```typescript
// src/pages/service/ServiceDetailPage.tsx
import { Calendar } from '../calendar/Calendar';
import { EventSidebar } from '../calendar/EventSidebar';

export function ServiceDetailPage() {
  const [selectedDate, setSelectedDate] = useState<Date | null>(null);
  const [selectedEvent, setSelectedEvent] = useState<CalendarEvent | null>(null);
  const [events, setEvents] = useState<CalendarEvent[]>([]);
  const [lunarData, setLunarData] = useState<LunarDateResult | null>(null);
  const [isLoadingLunar, setIsLoadingLunar] = useState(false);

  return (
    <div className="grid grid-cols-3 gap-6">
      {/* Service details */}
      <div className="col-span-2">
        <ServiceDetails />
      </div>

      {/* Calendar sidebar */}
      <div>
        <Calendar
          onDateSelect={setSelectedDate}
          onEventSelect={setSelectedEvent}
          events={events}
        />
        <EventSidebar
          events={events}
          selectedDate={selectedDate}
          lunarData={lunarData}
          isLoadingLunar={isLoadingLunar}
          selectedEvent={selectedEvent}
          onEventSelect={setSelectedEvent}
          onCloseEvent={() => setSelectedEvent(null)}
        />
      </div>
    </div>
  );
}
```

## 4. Lấy thông tin khách hàng từ User Profile

Nếu muốn lấy thông tin từ user profile thay vì localStorage:

```typescript
// src/lib/services/calendarBookingService.ts
import { userService } from './userService';

export async function createBookingFromEvent(
  request: CalendarBookingRequest
): Promise<CalendarBookingResponse> {
  try {
    // Lấy thông tin user nếu không có
    let customerName = request.customerName;
    let customerPhone = request.customerPhone;
    let customerEmail = request.customerEmail;

    if (!customerName || !customerPhone || !customerEmail) {
      const user = await userService.getCurrentUser();
      customerName = customerName || user.fullName;
      customerPhone = customerPhone || user.phone;
      customerEmail = customerEmail || user.email;
    }

    // ... rest of the code
  } catch (error) {
    // ...
  }
}
```

## 5. Tùy chỉnh thông báo

Để tùy chỉnh thông báo success/error:

```typescript
// src/pages/calendar/EventSidebar.tsx
const handleBookingConfirm = async (formData: BookingFormData) => {
  // ...
  const result = await calendarBookingService.createBookingFromEvent({
    // ...
  });

  setBookingResult(result);
  setShowBookingDialog(false);

  if (result.success) {
    // Tùy chỉnh thông báo success
    toast.success('🎉 Đặt lịch thành công!', {
      description: 'Nhân viên sẽ gọi lại trong 24 giờ',
      duration: 5000,
    });
  } else {
    // Tùy chỉnh thông báo error
    toast.error('❌ Đặt lịch thất bại', {
      description: result.message,
      duration: 5000,
    });
  }
};
```

## 6. Thêm xác nhận email

Để gửi email xác nhận booking:

```typescript
// src/lib/services/calendarBookingService.ts
import { emailService } from './emailService';

export async function createBookingFromEvent(
  request: CalendarBookingRequest
): Promise<CalendarBookingResponse> {
  try {
    // Tạo đơn hàng
    const result = await calendarBookingService.createBookingFromEvent(request);

    if (result.success) {
      // Gửi email xác nhận
      await emailService.sendBookingConfirmation({
        email: request.customerEmail,
        orderId: result.orderId,
        eventTitle: request.event.title,
        lunarDate: `${request.lunarData.day}/${request.lunarData.month}`,
      });
    }

    return result;
  } catch (error) {
    // ...
  }
}
```

## 7. Lịch sử booking

Để xem lịch sử booking của user:

```typescript
// src/pages/calendar/BookingHistoryPage.tsx
import { orderService } from '../../lib/services/orderService';

export function BookingHistoryPage() {
  const [bookings, setBookings] = useState<any[]>([]);
  const [isLoading, setIsLoading] = useState(true);

  useEffect(() => {
    const fetchBookings = async () => {
      try {
        const result = await orderService.getMyServiceOrders({
          pageSize: 10,
        });
        setBookings(result.items);
      } catch (error) {
        console.error('Error fetching bookings:', error);
      } finally {
        setIsLoading(false);
      }
    };

    fetchBookings();
  }, []);

  return (
    <div>
      {/* Render bookings */}
    </div>
  );
}
```

## 8. Hủy booking

Để cho phép user hủy booking:

```typescript
// src/pages/calendar/BookingDetailPage.tsx
const handleCancelBooking = async () => {
  if (!confirm('Bạn có chắc muốn hủy booking này?')) return;

  try {
    const success = await calendarBookingService.cancelBookingOrder(
      orderId,
      'Hủy do yêu cầu của khách hàng'
    );

    if (success) {
      toast.success('Hủy booking thành công');
      navigate('/bookings');
    } else {
      toast.error('Không thể hủy booking');
    }
  } catch (error) {
    toast.error('Có lỗi xảy ra');
  }
};
```

## 9. Theo dõi trạng thái booking

Để theo dõi trạng thái booking:

```typescript
// src/pages/calendar/BookingDetailPage.tsx
import { orderService } from '../../lib/services/orderService';

export function BookingDetailPage() {
  const { orderId } = useParams<{ orderId: string }>();
  const [statusHistory, setStatusHistory] = useState<any[]>([]);

  useEffect(() => {
    const fetchStatusHistory = async () => {
      try {
        const history = await orderService.getOrderStatusHistory(orderId!);
        setStatusHistory(history);
      } catch (error) {
        console.error('Error fetching status history:', error);
      }
    };

    fetchStatusHistory();
  }, [orderId]);

  return (
    <div>
      <h2>Lịch sử trạng thái</h2>
      {statusHistory.map((status) => (
        <div key={status.id}>
          <p>{status.status}</p>
          <p>{new Date(status.createdAt).toLocaleString('vi-VN')}</p>
        </div>
      ))}
    </div>
  );
}
```

## 10. Kiểm tra API endpoints

Đảm bảo các API endpoints sau hoạt động:

```bash
# Tạo đơn hàng
POST /api/v1/Checkout/process

# Lấy chi tiết đơn hàng
GET /api/v1/Checkout/{orderId}

# Hủy đơn hàng
POST /api/v1/Checkout/{orderId}/cancel

# Lấy lịch sử đơn hàng
GET /api/v1/Order/my-orders

# Lấy lịch sử trạng thái
GET /api/v1/Order/{orderId}/status-history
```

## 11. Testing

### Test case 1: Booking thành công
```bash
1. Mở Calendar
2. Chọn sự kiện
3. Click "Đặt lịch"
4. Nhập thông tin: Tên, SĐT, Email
5. Click "Đặt lịch"
6. Kết quả: Success toast + Xem đơn hàng button
```

### Test case 2: Validation lỗi
```bash
1. Mở Calendar
2. Chọn sự kiện
3. Click "Đặt lịch"
4. Để trống các field
5. Click "Đặt lịch"
6. Kết quả: Error message cho từng field
```

### Test case 3: API lỗi
```bash
1. Mock API trả về error
2. Chọn sự kiện + Click "Đặt lịch"
3. Nhập thông tin + Xác nhận
4. Kết quả: Error toast
```

## 12. Troubleshooting

### Lỗi: "Cannot find module 'BookingDialog'"
- Kiểm tra file `src/pages/calendar/BookingDialog.tsx` có tồn tại không
- Kiểm tra import path có đúng không

### Lỗi: "API endpoint not found"
- Kiểm tra backend API có endpoint `/api/v1/Checkout/process` không
- Kiểm tra API documentation

### Dialog không hiển thị
- Kiểm tra `showBookingDialog` state
- Kiểm tra AlertDialog component có được import đúng không
- Kiểm tra browser console có error không

### Thông tin không được lưu
- Kiểm tra localStorage có bị disable không
- Kiểm tra browser console có error không
- Kiểm tra `handleBookingConfirm` có được gọi không

## 13. Cấu hình tùy chỉnh

### Thay đổi màu sắc
```typescript
// src/pages/calendar/EventSidebar.tsx
// Thay đổi class names
className="bg-amber-500 hover:bg-amber-600" // Booking button
className="bg-green-50 text-green-700" // Success message
className="bg-red-50 text-red-700" // Error message
```

### Thay đổi text
```typescript
// src/pages/calendar/BookingDialog.tsx
<AlertDialogTitle>Đặt lịch - {eventTitle}</AlertDialogTitle>
// Thay đổi text này
```

### Thay đổi validation rules
```typescript
// src/pages/calendar/BookingDialog.tsx
// Thay đổi regex pattern
if (!/^[0-9]{10,11}$/.test(formData.customerPhone.replace(/\D/g, ''))) {
  // Thay đổi pattern này
}
```

## 14. Performance optimization

### Lazy load BookingSuccessPage
```typescript
import { lazy, Suspense } from 'react';

const BookingSuccessPage = lazy(() =>
  import('./pages/calendar/BookingSuccessPage').then(m => ({
    default: m.BookingSuccessPage
  }))
);

// Sử dụng
<Suspense fallback={<Loader />}>
  <BookingSuccessPage />
</Suspense>
```

### Cache booking data
```typescript
// src/lib/hooks/useBooking.ts
import { useQuery } from '@tanstack/react-query';
import { calendarBookingService } from '../services/calendarBookingService';

export function useBooking(orderId: string) {
  return useQuery({
    queryKey: ['booking', orderId],
    queryFn: () => calendarBookingService.getBookingOrder(orderId),
    staleTime: 5 * 60 * 1000, // 5 minutes
  });
}
```

## 15. Deployment checklist

- [ ] Thêm route `/bookings/:orderId`
- [ ] Kiểm tra API endpoints hoạt động
- [ ] Test booking flow
- [ ] Test validation
- [ ] Test error handling
- [ ] Kiểm tra localStorage hoạt động
- [ ] Kiểm tra email notification (nếu có)
- [ ] Kiểm tra responsive design
- [ ] Kiểm tra accessibility
- [ ] Deploy lên production
