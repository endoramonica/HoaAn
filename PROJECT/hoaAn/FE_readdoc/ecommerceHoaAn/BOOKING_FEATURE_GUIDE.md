# Tính Năng Đặt Dịch Vụ Theo Lịch Âm/Dương

## 📋 Tổng Quan

Tính năng cho phép khách hàng đặt dịch vụ bằng cách chọn ngày theo lịch dương, hệ thống tự động chuyển đổi sang lịch âm và hiển thị thông tin Can Chi.

## 🏗️ Kiến Trúc

### Frontend (React/TypeScript)

```
src/
├── pages/booking/
│   └── CalendarBookingPage.tsx          # Trang chính đặt dịch vụ
├── lib/services/
│   ├── lunarDateService.ts              # Service chuyển đổi lịch âm/dương
│   └── bookingService.ts                # Service gửi booking
└── components/ui/                       # UI components (Button, Card, Input, etc.)
```

### Backend (.NET)

```
Controllers/
├── BookingController.cs                 # Xử lý booking requests

Models/
├── BookingCreateRequest.cs              # DTO nhận từ FE
├── BookingResponse.cs                   # DTO trả về FE
└── LunarDateInfo.cs                     # Thông tin lịch âm

Services/
├── IBookingService.cs                   # Interface service
└── BookingService.cs                    # Implement booking logic

Data/
└── BookingRepository.cs                 # Lưu trữ booking
```

## 🔄 Flow Diagram

```
┌─────────────────────────────────────────────────────────────────┐
│                    CALENDAR BOOKING FLOW                         │
└─────────────────────────────────────────────────────────────────┘

1. SELECT DATE (Chọn ngày dương lịch)
   ├─ User chọn ngày trên calendar
   └─ FE gọi convertToLunar API

2. LUNAR INFO (Hiển thị lịch âm)
   ├─ Hiển thị ngày dương: 29/01/2025
   ├─ Hiển thị ngày âm: 01/01/2025
   ├─ Hiển thị Can Chi: Ất Tỵ
   └─ User xác nhận tiếp tục

3. CUSTOMER INFO (Thu thập thông tin)
   ├─ Chọn dịch vụ
   ├─ Nhập tên khách
   ├─ Nhập số điện thoại
   ├─ Nhập email
   └─ Ghi chú (tùy chọn)

4. CONFIRM (Xác nhận)
   ├─ Hiển thị toàn bộ thông tin
   └─ User xác nhận đặt

5. SUBMIT
   ├─ FE gửi BookingCreateRequest tới /api/v1/Booking/create
   ├─ Backend lưu vào database
   └─ Trả về BookingResponse với ID

6. SUCCESS
   ├─ Hiển thị thông báo thành công
   └─ Cho phép đặt dịch vụ khác hoặc về trang chủ
```

## 📡 API Endpoints

### 1. Chuyển Đổi Lịch (External API)

**POST** `https://open.oapi.vn/date/convert-to-lunar`

Request:
```json
{
  "day": 29,
  "month": 1,
  "year": 2025
}
```

Response:
```json
{
  "data": {
    "day": 1,
    "month": 1,
    "year": 2025,
    "date": "2025-01-01T00:00:00",
    "heavenlyStems": "Ất",
    "earthlyBranches": "Tỵ",
    "sexagenaryCycle": "Ất Tỵ"
  },
  "code": "success"
}
```

### 2. Tạo Booking

**POST** `/api/v1/Booking/create`

Request:
```json
{
  "customerId": "D93A557C-A41E-4DD9-A0A7-D6B3897BC4A7",
  "serviceId": "svc-001",
  "solarDate": "2025-01-29",
  "lunarDate": {
    "day": 1,
    "month": 1,
    "year": 2025,
    "heavenlyStems": "Ất",
    "earthlyBranches": "Tỵ",
    "sexagenaryCycle": "Ất Tỵ"
  },
  "customerName": "Nguyễn Văn A",
  "customerPhone": "0912345678",
  "customerEmail": "nguyenvana@example.com",
  "notes": "Ghi chú thêm"
}
```

Response:
```json
{
  "id": "booking-uuid",
  "customerId": "D93A557C-A41E-4DD9-A0A7-D6B3897BC4A7",
  "serviceId": "svc-001",
  "solarDate": "2025-01-29",
  "lunarDate": {
    "day": 1,
    "month": 1,
    "year": 2025,
    "heavenlyStems": "Ất",
    "earthlyBranches": "Tỵ",
    "sexagenaryCycle": "Ất Tỵ"
  },
  "customerName": "Nguyễn Văn A",
  "customerPhone": "0912345678",
  "customerEmail": "nguyenvana@example.com",
  "notes": "Ghi chú thêm",
  "status": "pending",
  "createdAt": "2025-01-29T10:30:00Z"
}
```

### 3. Lấy Booking

**GET** `/api/v1/Booking/{bookingId}`

### 4. Lấy Bookings của Khách

**GET** `/api/v1/Booking/customer/{customerId}`

### 5. Hủy Booking

**POST** `/api/v1/Booking/{bookingId}/cancel`

## 📝 DTOs

### BookingCreateRequest

```csharp
public class BookingCreateRequest
{
    public string CustomerId { get; set; }
    public string ServiceId { get; set; }
    public string SolarDate { get; set; } // ISO format: YYYY-MM-DD
    public LunarDateInfo LunarDate { get; set; }
    public string CustomerName { get; set; }
    public string CustomerPhone { get; set; }
    public string CustomerEmail { get; set; }
    public string Notes { get; set; }
}
```

### LunarDateInfo

```csharp
public class LunarDateInfo
{
    public int Day { get; set; }
    public int Month { get; set; }
    public int Year { get; set; }
    public string HeavenlyStems { get; set; }
    public string EarthlyBranches { get; set; }
    public string SexagenaryCycle { get; set; }
}
```

### BookingResponse

```csharp
public class BookingResponse
{
    public string Id { get; set; }
    public string CustomerId { get; set; }
    public string ServiceId { get; set; }
    public string SolarDate { get; set; }
    public LunarDateInfo LunarDate { get; set; }
    public string CustomerName { get; set; }
    public string CustomerPhone { get; set; }
    public string CustomerEmail { get; set; }
    public string Notes { get; set; }
    public string Status { get; set; }
    public DateTime CreatedAt { get; set; }
}
```

## ✅ Validation

### Frontend Validation

1. **Ngày**: Không được chọn ngày trong quá khứ
2. **Dịch vụ**: Bắt buộc chọn
3. **Tên khách**: Bắt buộc, không để trống
4. **Số điện thoại**: 
   - Bắt buộc
   - Phải là 10-11 chữ số
   - Regex: `/^\d{10,11}$/`
5. **Email**:
   - Bắt buộc
   - Phải hợp lệ
   - Regex: `/^[^\s@]+@[^\s@]+\.[^\s@]+$/`

### Backend Validation

1. **CustomerId**: Phải khớp với fixed customer ID
2. **ServiceId**: Phải tồn tại trong database
3. **SolarDate**: Phải là ngày hợp lệ
4. **LunarDate**: Phải có đầy đủ thông tin
5. **CustomerPhone**: Phải là số điện thoại hợp lệ
6. **CustomerEmail**: Phải là email hợp lệ

## 🔐 Security

1. **CustomerId cố định**: `D93A557C-A41E-4DD9-A0A7-D6B3897BC4A7`
   - Không cho phép thay đổi từ FE
   - Backend luôn ghi đè giá trị này

2. **Authentication**: Yêu cầu Bearer token
   - Tất cả requests phải có Authorization header

3. **Input Sanitization**: 
   - Trim whitespace
   - Escape special characters
   - Validate data types

4. **Rate Limiting**: 
   - Giới hạn số requests per minute
   - Prevent spam bookings

## 🚀 Cách Sử Dụng

### 1. Import Components

```typescript
import { CalendarBookingPage } from '@/pages/booking/CalendarBookingPage';
```

### 2. Thêm Route

```typescript
// src/App.tsx
import { CalendarBookingPage } from './pages/booking/CalendarBookingPage';

<Route path="/booking/calendar" element={<CalendarBookingPage />} />
```

### 3. Sử Dụng Services

```typescript
import { convertToLunar, convertToSolar } from '@/lib/services/lunarDateService';
import { createBooking, getBooking } from '@/lib/services/bookingService';

// Chuyển đổi ngày
const lunarData = await convertToLunar(29, 1, 2025);

// Tạo booking
const booking = await createBooking({
  serviceId: 'svc-001',
  solarDate: '2025-01-29',
  lunarDate: lunarData,
  customerName: 'Nguyễn Văn A',
  customerPhone: '0912345678',
  customerEmail: 'nguyenvana@example.com',
});
```

## 📊 Database Schema

```sql
CREATE TABLE Bookings (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    CustomerId UNIQUEIDENTIFIER NOT NULL,
    ServiceId NVARCHAR(50) NOT NULL,
    SolarDate DATE NOT NULL,
    LunarDay INT NOT NULL,
    LunarMonth INT NOT NULL,
    LunarYear INT NOT NULL,
    HeavenlyStems NVARCHAR(10),
    EarthlyBranches NVARCHAR(10),
    SexagenaryCycle NVARCHAR(20),
    CustomerName NVARCHAR(255) NOT NULL,
    CustomerPhone NVARCHAR(20) NOT NULL,
    CustomerEmail NVARCHAR(255) NOT NULL,
    Notes NVARCHAR(MAX),
    Status NVARCHAR(50) DEFAULT 'pending',
    CreatedAt DATETIME DEFAULT GETUTCDATE(),
    UpdatedAt DATETIME DEFAULT GETUTCDATE(),
    FOREIGN KEY (CustomerId) REFERENCES Customers(Id),
    FOREIGN KEY (ServiceId) REFERENCES Services(Id)
);

CREATE INDEX IX_Bookings_CustomerId ON Bookings(CustomerId);
CREATE INDEX IX_Bookings_SolarDate ON Bookings(SolarDate);
CREATE INDEX IX_Bookings_Status ON Bookings(Status);
```

## 🧪 Testing

### Unit Tests

```typescript
// Test lunar date conversion
describe('lunarDateService', () => {
  it('should convert solar to lunar correctly', async () => {
    const result = await convertToLunar(29, 1, 2025);
    expect(result.day).toBe(1);
    expect(result.month).toBe(1);
    expect(result.year).toBe(2025);
  });
});

// Test booking creation
describe('bookingService', () => {
  it('should create booking successfully', async () => {
    const booking = await createBooking({
      serviceId: 'svc-001',
      solarDate: '2025-01-29',
      lunarDate: { /* ... */ },
      customerName: 'Test User',
      customerPhone: '0912345678',
      customerEmail: 'test@example.com',
    });
    expect(booking.id).toBeDefined();
  });
});
```

## 🐛 Troubleshooting

### Lỗi: "Không thể chuyển đổi ngày"

- Kiểm tra kết nối internet
- Kiểm tra API endpoint `https://open.oapi.vn/date/convert-to-lunar`
- Kiểm tra format ngày (day, month, year)

### Lỗi: "Đặt dịch vụ thất bại"

- Kiểm tra authentication token
- Kiểm tra validation errors
- Kiểm tra backend logs

### Lỗi: "Email không hợp lệ"

- Đảm bảo email có format: `user@domain.com`
- Không có khoảng trắng

### Lỗi: "Số điện thoại không hợp lệ"

- Phải là 10-11 chữ số
- Không có ký tự đặc biệt (ngoại trừ dấu gạch ngang)

## 📱 Mobile Support

Trang được thiết kế responsive cho mobile:
- Calendar grid tự động điều chỉnh
- Form fields full-width trên mobile
- Touch-friendly buttons (min 44x44px)

## 🔄 Integration Checklist

- [ ] Thêm route `/booking/calendar` vào App.tsx
- [ ] Cập nhật swagger.json với BookingController endpoints
- [ ] Chạy `npm run api:generate` để regenerate client
- [ ] Implement BookingController trong backend
- [ ] Implement BookingService trong backend
- [ ] Tạo Booking table trong database
- [ ] Test flow end-to-end
- [ ] Deploy FE và BE

## 📞 Support

Liên hệ team development nếu có vấn đề.
