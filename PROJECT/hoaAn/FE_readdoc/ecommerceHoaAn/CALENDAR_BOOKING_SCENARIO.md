# Calendar Booking - Kịch bản giải quyết

## 1. Hiểu rõ vấn đề

### Service là gì?
- Service (dịch vụ) là một loại **product đặc biệt**
- Có thể thêm vào cart giống như product thường
- Có thể checkout và tạo order giống như product
- **Khác biệt**: Service cần **đặt lịch** (booking) để xác định ngày/giờ thực hiện

### Booking là gì?
- Booking = **Chốt đơn hàng dịch vụ** với thông tin lịch
- Khi user "Đặt lịch" → Tạo Order với:
  - `type: "service"`
  - `serviceCategory`: Loại dịch vụ (phong-thuy, phat-giao, gia-tien, le-hoi)
  - `serviceDate`: Ngày dự kiến thực hiện dịch vụ
  - `serviceNotes`: Ghi chú thêm
  - `serviceDuration`: Thời lượng dịch vụ (nếu có)
  - `serviceLocation`: Địa điểm thực hiện (nếu có)

## 2. Kịch bản hiện tại (Sai)

```
User click "Đặt lịch"
  ↓
Mở BookingDialog (nhập thông tin khách hàng)
  ↓
Gọi calendarBookingService.createBookingFromEvent()
  ↓
Tạo CheckoutDto và gọi POST /api/v1/Checkout/process
  ↓
Tạo Order
  ↓
Hiển thị BookingSuccessPage
```

**Vấn đề**: 
- Không rõ `CheckoutDto` cần những field gì
- Không biết service có cần thêm vào cart trước không
- Không biết flow checkout có khác không

## 3. Kịch bản đề xuất (Đúng)

### Option A: Service là Product - Thêm vào Cart trước

```
ServiceDetailPage / ServicePage
  ↓
User click "Thêm vào giỏ"
  ↓
Service được thêm vào Cart (giống Product)
  ↓
User vào Cart
  ↓
User click "Thanh toán"
  ↓
Vào CheckoutFlow
  ↓
Ở bước "Xác nhận đơn hàng" → Nếu có Service → Hiển thị "Đặt lịch"
  ↓
User click "Đặt lịch"
  ↓
Mở BookingDialog (nhập thông tin lịch)
  ↓
Gọi POST /api/v1/Order (tạo order với serviceDate, serviceNotes, etc)
  ↓
Order được tạo với status = "pending"
  ↓
Hiển thị OrderSuccessPage
```

### Option B: Service là Product - Đặt lịch trực tiếp từ Calendar

```
CalendarPage (Xem lịch âm)
  ↓
User click "Đặt lịch" trên ngày có sự kiện
  ↓
Mở BookingDialog (nhập thông tin khách hàng + lịch)
  ↓
Gọi POST /api/v1/Order (tạo order trực tiếp)
  ↓
Order được tạo với:
  - type: "service"
  - serviceCategory: event.category
  - serviceDate: selectedDate
  - serviceNotes: event.title + ghi chú
  - items: [{ productId: event.id, quantity: 1, ... }]
  ↓
Hiển thị BookingSuccessPage
```

## 4. Thông tin cần cung cấp

### Từ Backend:
1. **Service Product API**
   - GET /api/v1/Product?type=service
   - Response: Danh sách service với fields:
     - `id`, `name`, `description`, `price`
     - `serviceCategory`: phong-thuy, phat-giao, gia-tien, le-hoi
     - `serviceDuration`: "1 giờ", "2 giờ", etc
     - `serviceLocation`: "Online", "Tại nhà", etc

2. **Order Creation API**
   - POST /api/v1/Order
   - Request body cần:
     ```json
     {
       "items": [
         {
           "productId": "uuid",
           "quantity": 1,
           "unitPrice": 0,
           "type": "service",
           "serviceCategory": "phong-thuy",
           "serviceDuration": "1 giờ"
         }
       ],
       "shippingAddress": {
         "fullName": "string",
         "phoneNumber": "string",
         "address": "string",
         "ward": "string",
         "district": "string",
         "city": "string",
         "postalCode": "string"
       },
       "paymentMethod": "COD",
       "type": "service",
       "serviceCategory": "phong-thuy",
       "serviceDate": "2025-12-07T14:34:11.837Z",
       "serviceNotes": "string",
       "serviceDuration": "1 giờ",
       "serviceLocation": "Online"
     }
     ```

3. **Order Response** (GET /api/v1/Order/{id})
   - Đã có rồi, response có fields:
     - `serviceDate`: Ngày dự kiến
     - `serviceNotes`: Ghi chú
     - `serviceDuration`: Thời lượng
     - `serviceLocation`: Địa điểm
     - `serviceCategory`: Loại dịch vụ

### Từ Frontend:
1. **Service List**
   - Danh sách service từ API
   - Hiển thị trong ServicePage / ServiceDetailPage

2. **Calendar Events**
   - Mapping từ Service hoặc từ API riêng
   - Mỗi event có:
     - `id`: Service ID
     - `title`: Service name
     - `category`: serviceCategory
     - `solarDate`: Ngày dương lịch
     - `lunarDate`: Ngày âm lịch

3. **Booking Form**
   - Thông tin khách hàng: name, phone, email
   - Thông tin lịch: serviceDate, serviceDuration, serviceLocation, serviceNotes

## 5. Cấu trúc dữ liệu cần thiết

### CalendarEvent (từ Service)
```typescript
interface CalendarEvent {
  id: string;                    // Service ID
  title: string;                 // Service name
  lunarDate: string;             // "1/1"
  solarDate: string;             // "2025-01-29"
  type: 'service';               // Loại là service
  category: 'phong-thuy' | 'phat-giao' | 'gia-tien' | 'le-hoi';
  description: string;           // Service description
  price: number;                 // Service price
  serviceDuration: string;       // "1 giờ", "2 giờ"
  serviceLocation: string;       // "Online", "Tại nhà"
  color: string;                 // Màu hiển thị
}
```

### BookingRequest
```typescript
interface BookingRequest {
  // Thông tin khách hàng
  customerName: string;
  customerPhone: string;
  customerEmail: string;
  
  // Thông tin lịch
  serviceDate: Date;             // Ngày dự kiến
  serviceDuration: string;       // "1 giờ", "2 giờ"
  serviceLocation: string;       // "Online", "Tại nhà"
  serviceNotes: string;          // Ghi chú thêm
  
  // Thông tin service
  serviceId: string;             // Product ID
  serviceCategory: string;       // phong-thuy, phat-giao, etc
  quantity: number;              // Thường là 1
}
```

### OrderRequest (POST /api/v1/Order)
```typescript
interface OrderRequest {
  items: [
    {
      productId: string;
      quantity: number;
      unitPrice: number;
      type: "service";
      serviceCategory: string;
      serviceDuration: string;
    }
  ];
  shippingAddress: {
    fullName: string;
    phoneNumber: string;
    address: string;
    ward: string;
    district: string;
    city: string;
    postalCode: string;
  };
  paymentMethod: "COD" | "BankTransfer" | "VNPay" | "Momo";
  type: "service";
  serviceCategory: string;
  serviceDate: string;           // ISO format
  serviceNotes: string;
  serviceDuration: string;
  serviceLocation: string;
}
```

## 6. Các câu hỏi cần trả lời

### Về Service:
1. **Service có được lưu trong Product table không?**
   - Nếu có → Có thể dùng chung flow với Product [có]
   - Nếu không → Cần API riêng cho Service

2. **Service có price không?**
   - Nếu có → Có thể tính tiền khi checkout
   - Nếu không → Có thể là "Tư vấn miễn phí" [không]

3. **Service có cần thêm vào Cart không?**
   - Nếu có → Dùng flow checkout thường [có]
   - Nếu không → Đặt lịch trực tiếp tạo order

### Về Booking:
1. **Booking có cần thanh toán không?**[không]
   - Nếu có → Cần payment gateway
   - Nếu không → Chỉ cần COD

2. **Booking có cần xác nhận từ admin không?**[có]
   - Nếu có → Status = "pending" → Admin xác nhận → Status = "confirmed"
   - Nếu không → Status = "confirmed" ngay

3. **Booking có cần gửi email xác nhận không?**[có]
   - Nếu có → Cần email service
   - Nếu không → Chỉ hiển thị trên UI

### Về Calendar:
1. **Calendar events từ đâu?**
   - Từ Service API?
   - Từ API riêng?
   - Hardcoded?

2. **Có bao nhiêu loại service?**
   - phong-thuy, phat-giao, gia-tien, le-hoi?
   - Có thêm loại khác không?

## 7. Đề xuất Implementation

### Bước 1: Xác định Service Structure
- Cung cấp Service API endpoint
- Cung cấp Service response format

### Bước 2: Xác định Booking Flow
- Chọn Option A (thêm vào cart) hay Option B (đặt lịch trực tiếp)
- Cung cấp POST /api/v1/Order request/response format

### Bước 3: Cập nhật Calendar
- Lấy Service list từ API
- Mapping thành Calendar events
- Hiển thị trong Calendar

### Bước 4: Cập nhật Booking
- Cập nhật BookingDialog để nhập serviceDate, serviceDuration, serviceLocation
- Gọi POST /api/v1/Order thay vì POST /api/v1/Checkout/process
- Hiển thị OrderSuccessPage

### Bước 5: Cập nhật Success Page
- Hiển thị Order details (từ GET /api/v1/Order/{id})
- Hiển thị serviceDate, serviceDuration, serviceLocation
- Hiển thị status và hướng dẫn tiếp theo

## 8. Tóm tắt

**Hiện tại**: Booking = Tạo Order với type="service"
**Cần làm**: 
1. Cung cấp Service API
2. Cung cấp POST /api/v1/Order format
3. Xác định Booking flow (Option A hay B)
4. Cập nhật code theo flow đã chọn

---

**Chờ bạn cung cấp thông tin để tiếp tục implementation.**
