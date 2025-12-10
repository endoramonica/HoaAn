# Calendar Booking Integration Guide

## Tổng quan

Tính năng "Đặt lịch" cho phép người dùng đặt lịch tư vấn dịch vụ trực tiếp từ sự kiện lịch âm. Khi đặt lịch thành công, hệ thống sẽ tạo một đơn hàng dịch vụ và nhân viên sẽ gọi lại để tư vấn chi tiết.

## Luồng hoạt động

```
ServicePage/ServiceDetailPage
    ↓
Calendar (chọn sự kiện)
    ↓
EventSidebar (hiển thị chi tiết sự kiện)
    ↓
Click "Đặt lịch" button
    ↓
BookingDialog (nhập thông tin khách hàng)
    ↓
calendarBookingService.createBookingFromEvent()
    ↓
API: POST /api/v1/Checkout/process
    ↓
✅ Success: Thông báo + Xem đơn hàng
❌ Failed: Thông báo lỗi
```

## Các file được tạo/cập nhật

### 1. **calendarBookingService.ts** (NEW)
- Xử lý tạo đơn hàng từ sự kiện lịch
- Gọi API `postApiV1CheckoutProcess` để tạo đơn hàng
- Xử lý lỗi và trả về kết quả

**Hàm chính:**
```typescript
createBookingFromEvent(request: CalendarBookingRequest): Promise<CalendarBookingResponse>
```

### 2. **BookingDialog.tsx** (NEW)
- Dialog để nhập thông tin khách hàng
- Validation form (tên, số điện thoại, email)
- Lưu thông tin vào localStorage để tái sử dụng

**Props:**
```typescript
interface BookingDialogProps {
  open: boolean;
  onOpenChange: (open: boolean) => void;
  onConfirm: (data: BookingFormData) => Promise<void>;
  isLoading?: boolean;
  eventTitle?: string;
}
```

### 3. **EventSidebar.tsx** (UPDATED)
- Thêm nút "Đặt lịch"
- Hiển thị kết quả booking (success/error)
- Nút "Xem đơn hàng" khi booking thành công
- Tích hợp BookingDialog

**Thay đổi:**
- Thêm state: `isBooking`, `bookingResult`, `showBookingDialog`
- Thêm handler: `handleBookingClick`, `handleBookingConfirm`
- Thêm UI: Booking button, result card, view order button

### 4. **BookingSuccessPage.tsx** (NEW)
- Trang hiển thị chi tiết đơn hàng sau khi đặt lịch thành công
- Hiển thị thông tin khách hàng, đơn hàng, dịch vụ
- Hướng dẫn bước tiếp theo

**Route:** `/bookings/:orderId`

## Cách sử dụng

### 1. Thêm route cho BookingSuccessPage

```typescript
// src/App.tsx hoặc router config
import { BookingSuccessPage } from './pages/calendar/BookingSuccessPage';

// Thêm route
<Route path="/bookings/:orderId" element={<BookingSuccessPage />} />
```

### 2. Sử dụng trong Calendar

EventSidebar đã được cập nhật, chỉ cần:
1. Người dùng chọn sự kiện trong lịch
2. Click nút "Đặt lịch"
3. Nhập thông tin liên hệ
4. Xem kết quả

### 3. Lấy thông tin khách hàng

Thông tin khách hàng được lưu trong localStorage:
```typescript
localStorage.getItem('customerName')
localStorage.getItem('customerPhone')
localStorage.getItem('customerEmail')
```

Có thể lấy từ user profile hoặc form nhập.

## API Endpoints sử dụng

### 1. Tạo đơn hàng
```
POST /api/v1/Checkout/process
Body: CheckoutDto
Response: { id, orderId, ... }
```

### 2. Lấy chi tiết đơn hàng
```
GET /api/v1/Checkout/{orderId}
Response: Order details
```

### 3. Hủy đơn hàng
```
POST /api/v1/Checkout/{orderId}/cancel
Body: { reason }
Response: Success/Error
```

## Dữ liệu được gửi

### CheckoutDto
```typescript
{
  items: [
    {
      productId: event.id,
      quantity: 1,
      price: 0,
      name: event.title,
      description: "Sự kiện - Ngày âm lịch"
    }
  ],
  shippingAddress: {
    fullName: customerName,
    phone: customerPhone,
    email: customerEmail,
    addressLine1: "Tư vấn qua điện thoại",
    city: "Online",
    district: "Online",
    ward: "Online",
    postalCode: "00000"
  },
  paymentMethod: "COD",
  notes: "Sự kiện: ... Ngày âm lịch: ... Can Chi: ...",
  type: "service",
  serviceCategory: event.category
}
```

## Xử lý lỗi

### Lỗi validation
- Tên trống → "Vui lòng nhập tên"
- Số điện thoại không hợp lệ → "Số điện thoại không hợp lệ"
- Email không hợp lệ → "Email không hợp lệ"

### Lỗi API
- Tạo đơn hàng thất bại → "Không thể tạo đơn hàng. Vui lòng thử lại."
- Lỗi server → "Có lỗi xảy ra khi đặt lịch. Vui lòng thử lại sau."

## Thông báo (Toast)

- ✅ Success: "Đặt lịch thành công! Nhân viên sẽ gọi lại để tư vấn chi tiết."
- ❌ Error: Hiển thị message từ API hoặc error message

## Tính năng bổ sung

### 1. Lưu thông tin khách hàng
Thông tin được lưu vào localStorage, lần sau người dùng không cần nhập lại.

### 2. Xem lịch sử booking
Có thể lấy từ `orderService.getMyServiceOrders()` với filter `serviceCategory`.

### 3. Hủy booking
Sử dụng `calendarBookingService.cancelBookingOrder(orderId)`.

### 4. Theo dõi trạng thái
Sử dụng `orderService.getOrderStatusHistory(orderId)` để xem lịch sử trạng thái.

## Testing

### Test case 1: Booking thành công
1. Chọn sự kiện trong lịch
2. Click "Đặt lịch"
3. Nhập thông tin hợp lệ
4. Xác nhận
5. Kết quả: Thông báo success + Xem đơn hàng

### Test case 2: Validation lỗi
1. Chọn sự kiện
2. Click "Đặt lịch"
3. Để trống các field
4. Click "Đặt lịch"
5. Kết quả: Hiển thị error message

### Test case 3: API lỗi
1. Mock API trả về error
2. Chọn sự kiện + Click "Đặt lịch"
3. Nhập thông tin + Xác nhận
4. Kết quả: Hiển thị error message

## Tích hợp với ServicePage/ServiceDetailPage

Để tích hợp với ServicePage hoặc ServiceDetailPage:

```typescript
// ServiceDetailPage.tsx
import { Calendar } from './pages/calendar/Calendar';

export function ServiceDetailPage() {
  return (
    <div>
      {/* Service details */}
      <ServiceDetails />
      
      {/* Calendar for booking */}
      <Calendar />
    </div>
  );
}
```

## Cải tiến trong tương lai

1. **Chọn thời gian cụ thể** - Thêm time picker để chọn giờ tư vấn
2. **Ghi chú thêm** - Cho phép nhập ghi chú thêm về nhu cầu
3. **Chọn loại dịch vụ** - Nếu có nhiều dịch vụ
4. **Xác nhận email** - Gửi email xác nhận booking
5. **Lịch sử booking** - Trang xem lịch sử booking của user
6. **Đánh giá dịch vụ** - Sau khi hoàn thành dịch vụ

## Troubleshooting

### Lỗi: "Không thể tạo đơn hàng"
- Kiểm tra API endpoint `/api/v1/Checkout/process`
- Kiểm tra dữ liệu gửi đi (CheckoutDto)
- Kiểm tra authentication token

### Lỗi: "Không thể tải thông tin đơn hàng"
- Kiểm tra API endpoint `/api/v1/Checkout/{orderId}`
- Kiểm tra orderId có hợp lệ không

### Dialog không hiển thị
- Kiểm tra AlertDialog component có được import đúng không
- Kiểm tra state `showBookingDialog`

### Thông tin không được lưu
- Kiểm tra localStorage có bị disable không
- Kiểm tra browser console có error không
