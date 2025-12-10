# Calendar Module

## Overview

Calendar module cung cấp tính năng xem lịch âm, sự kiện, và đặt lịch tư vấn dịch vụ.

## Components

### CalendarGrid.tsx
Hiển thị lịch dạng grid với các sự kiện.

### EventSidebar.tsx
Hiển thị chi tiết sự kiện và nút "Đặt lịch".

**Features:**
- Hiển thị thông tin âm lịch
- Danh sách sự kiện trong tháng
- Nút "Đặt lịch"
- Hiển thị kết quả booking

### BookingDialog.tsx
Dialog để nhập thông tin khách hàng.

**Features:**
- Form nhập tên, SĐT, email
- Validation form
- Lưu vào localStorage
- Loading state

### BookingSuccessPage.tsx
Trang hiển thị chi tiết booking sau khi đặt lịch thành công.

**Features:**
- Hiển thị thông tin khách hàng
- Hiển thị thông tin đơn hàng
- Hiển thị dịch vụ
- Hướng dẫn bước tiếp theo
- Nút quay lại / lịch sử

### BookingHistoryPage.tsx
Trang hiển thị lịch sử booking của user.

**Features:**
- Danh sách tất cả booking
- Trạng thái booking
- Thông tin liên hệ
- Xem chi tiết từng booking

## Services

### calendarBookingService.ts
Service xử lý booking.

**Methods:**
- `createBookingFromEvent()` - Tạo đơn hàng từ sự kiện
- `getBookingOrder()` - Lấy chi tiết đơn hàng
- `cancelBookingOrder()` - Hủy đơn hàng

## Hooks

### useCalendarBooking.ts
Hook xử lý booking.

**Usage:**
```typescript
const { createBooking, isLoading, isSuccess, error } = useCalendarBooking();
```

### useBookingOrder.ts
Hook lấy thông tin booking.

**Usage:**
```typescript
const { data: order, isLoading, error } = useBookingOrder(orderId);
```

## Routes

```
/calendar                    - Trang lịch
/bookings                    - Lịch sử booking
/bookings/:orderId           - Chi tiết booking
```

## Usage

### 1. Mở Calendar
```typescript
import CalendarPage from './components/CalendarPage';

// Route: /calendar
```

### 2. Đặt lịch
1. Chọn sự kiện
2. Click "Đặt lịch"
3. Nhập thông tin
4. Click "Đặt lịch"

### 3. Xem chi tiết booking
```typescript
navigate(`/bookings/${orderId}`);
```

### 4. Xem lịch sử booking
```typescript
navigate('/bookings');
```

## API Endpoints

```
POST /api/v1/Checkout/process
GET  /api/v1/Checkout/{orderId}
POST /api/v1/Checkout/{orderId}/cancel
GET  /api/v1/Order/my-orders
GET  /api/v1/Order/{orderId}/status-history
```

## localStorage Keys

```
customerName      - Tên khách hàng
customerPhone     - Số điện thoại
customerEmail     - Email
```

## Validation Rules

| Field | Rules |
|-------|-------|
| Tên | Bắt buộc, không trống |
| SĐT | Bắt buộc, 10-11 chữ số |
| Email | Bắt buộc, định dạng email |

## Error Handling

- Form validation errors
- API errors
- Network errors
- localStorage errors

## Features

- ✅ Xem lịch âm
- ✅ Xem sự kiện
- ✅ Đặt lịch
- ✅ Xem chi tiết booking
- ✅ Xem lịch sử booking
- ✅ Validation form
- ✅ Error handling
- ✅ localStorage support
- ✅ Toast notifications

## Testing

See `CALENDAR_BOOKING_CHECKLIST.md` for testing checklist.

## Documentation

- `CALENDAR_BOOKING_INTEGRATION.md` - Hướng dẫn chi tiết
- `CALENDAR_BOOKING_SETUP.md` - Hướng dẫn setup
- `CALENDAR_BOOKING_QUICK_REFERENCE.md` - Quick reference

## Status

✅ Complete and ready for production

---

**Last Updated**: December 7, 2025
**Version**: 1.0.0
