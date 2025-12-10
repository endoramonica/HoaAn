# Booking Feature - Flow Diagram

## 🔄 Complete User Flow

```
┌─────────────────────────────────────────────────────────────────────────────┐
│                         CALENDAR BOOKING FLOW                               │
└─────────────────────────────────────────────────────────────────────────────┘

                              USER JOURNEY

┌──────────────────────────────────────────────────────────────────────────────┐
│ STEP 1: SELECT DATE                                                          │
├──────────────────────────────────────────────────────────────────────────────┤
│                                                                              │
│  User navigates to /booking/calendar                                        │
│           ↓                                                                 │
│  CalendarBookingPage renders with calendar grid                            │
│           ↓                                                                 │
│  User clicks on a date (e.g., 29/01/2025)                                 │
│           ↓                                                                 │
│  FE validates: date is not in the past                                     │
│           ↓                                                                 │
│  FE calls: convertToLunar(29, 1, 2025)                                    │
│           ↓                                                                 │
│  API Request: POST https://open.oapi.vn/date/convert-to-lunar             │
│  {                                                                          │
│    "day": 29,                                                              │
│    "month": 1,                                                             │
│    "year": 2025                                                            │
│  }                                                                          │
│           ↓                                                                 │
│  API Response:                                                              │
│  {                                                                          │
│    "data": {                                                               │
│      "day": 1,                                                             │
│      "month": 1,                                                           │
│      "year": 2025,                                                         │
│      "heavenlyStems": "Ất",                                               │
│      "earthlyBranches": "Tỵ",                                             │
│      "sexagenaryCycle": "Ất Tỵ"                                           │
│    },                                                                       │
│    "code": "success"                                                        │
│  }                                                                          │
│           ↓                                                                 │
│  FE stores lunar data in state                                             │
│           ↓                                                                 │
│  Move to STEP 2                                                            │
│                                                                              │
└──────────────────────────────────────────────────────────────────────────────┘

┌──────────────────────────────────────────────────────────────────────────────┐
│ STEP 2: DISPLAY LUNAR INFO                                                   │
├──────────────────────────────────────────────────────────────────────────────┤
│                                                                              │
│  Display lunar date information:                                           │
│  ┌────────────────────────────────────────────────────────────────────┐   │
│  │ Ngày dương lịch: 29/01/2025                                        │   │
│  │ Ngày âm lịch: 01/01/2025                                           │   │
│  │ Can Chi: Ất Tỵ                                                     │   │
│  └────────────────────────────────────────────────────────────────────┘   │
│           ↓                                                                 │
│  User reviews and clicks "Tiếp tục"                                       │
│           ↓                                                                 │
│  Move to STEP 3                                                            │
│                                                                              │
└──────────────────────────────────────────────────────────────────────────────┘

┌──────────────────────────────────────────────────────────────────────────────┐
│ STEP 3: COLLECT CUSTOMER INFO                                                │
├──────────────────────────────────────────────────────────────────────────────┤
│                                                                              │
│  Display form with fields:                                                 │
│  ┌────────────────────────────────────────────────────────────────────┐   │
│  │ Dịch vụ: [Dropdown]                                               │   │
│  │   - Tư vấn phong thủy                                             │   │
│  │   - Cúng tổ tiên                                                  │   │
│  │   - Lễ cầu duyên                                                  │   │
│  │   - Khai trương                                                   │   │
│  │                                                                    │   │
│  │ Tên khách hàng: [Text Input]                                     │   │
│  │ Số điện thoại: [Tel Input]                                       │   │
│  │ Email: [Email Input]                                             │   │
│  │ Ghi chú: [Textarea]                                              │   │
│  └────────────────────────────────────────────────────────────────────┘   │
│           ↓                                                                 │
│  User fills in all required fields                                        │
│           ↓                                                                 │
│  FE validates:                                                             │
│    ✓ Service selected                                                      │
│    ✓ Name not empty                                                        │
│    ✓ Phone is 10-11 digits                                               │
│    ✓ Email is valid format                                               │
│           ↓                                                                 │
│  User clicks "Tiếp tục"                                                   │
│           ↓                                                                 │
│  Move to STEP 4                                                            │
│                                                                              │
└──────────────────────────────────────────────────────────────────────────────┘

┌──────────────────────────────────────────────────────────────────────────────┐
│ STEP 4: CONFIRM BOOKING                                                      │
├──────────────────────────────────────────────────────────────────────────────┤
│                                                                              │
│  Display confirmation summary:                                             │
│  ┌────────────────────────────────────────────────────────────────────┐   │
│  │ Dịch vụ: Tư vấn phong thủy                                        │   │
│  │ Ngày dương lịch: 29/01/2025                                        │   │
│  │ Ngày âm lịch: 01/01/2025                                           │   │
│  │ Can Chi: Ất Tỵ                                                     │   │
│  │ Tên khách: Nguyễn Văn A                                            │   │
│  │ Điện thoại: 0912345678                                             │   │
│  │ Email: nguyenvana@example.com                                      │   │
│  └────────────────────────────────────────────────────────────────────┘   │
│           ↓                                                                 │
│  User clicks "Xác nhận đặt dịch vụ"                                       │
│           ↓                                                                 │
│  Move to STEP 5                                                            │
│                                                                              │
└──────────────────────────────────────────────────────────────────────────────┘

┌──────────────────────────────────────────────────────────────────────────────┐
│ STEP 5: SUBMIT BOOKING                                                       │
├──────────────────────────────────────────────────────────────────────────────┤
│                                                                              │
│  FE prepares payload:                                                      │
│  {                                                                          │
│    "customerId": "D93A557C-A41E-4DD9-A0A7-D6B3897BC4A7",                 │
│    "serviceId": "svc-001",                                                │
│    "solarDate": "2025-01-29",                                             │
│    "lunarDate": {                                                          │
│      "day": 1,                                                            │
│      "month": 1,                                                          │
│      "year": 2025,                                                        │
│      "heavenlyStems": "Ất",                                              │
│      "earthlyBranches": "Tỵ",                                            │
│      "sexagenaryCycle": "Ất Tỵ"                                          │
│    },                                                                      │
│    "customerName": "Nguyễn Văn A",                                        │
│    "customerPhone": "0912345678",                                         │
│    "customerEmail": "nguyenvana@example.com",                             │
│    "notes": ""                                                             │
│  }                                                                          │
│           ↓                                                                 │
│  FE calls: createBooking(payload)                                         │
│           ↓                                                                 │
│  API Request: POST /api/v1/Booking/create                                │
│  Headers: Authorization: Bearer {token}                                   │
│           ↓                                                                 │
│  Backend receives request                                                  │
│           ↓                                                                 │
│  Backend validates:                                                        │
│    ✓ CustomerId matches fixed ID                                         │
│    ✓ ServiceId exists                                                     │
│    ✓ SolarDate is valid                                                  │
│    ✓ LunarDate has all required fields                                   │
│    ✓ Phone format is valid                                               │
│    ✓ Email format is valid                                               │
│           ↓                                                                 │
│  Backend creates Booking entity:                                          │
│  {                                                                          │
│    Id: "booking-uuid",                                                    │
│    CustomerId: "D93A557C-A41E-4DD9-A0A7-D6B3897BC4A7",                  │
│    ServiceId: "svc-001",                                                 │
│    SolarDate: 2025-01-29,                                                │
│    LunarDay: 1,                                                           │
│    LunarMonth: 1,                                                         │
│    LunarYear: 2025,                                                       │
│    HeavenlyStems: "Ất",                                                  │
│    EarthlyBranches: "Tỵ",                                                │
│    SexagenaryCycle: "Ất Tỵ",                                             │
│    CustomerName: "Nguyễn Văn A",                                          │
│    CustomerPhone: "0912345678",                                           │
│    CustomerEmail: "nguyenvana@example.com",                               │
│    Notes: "",                                                              │
│    Status: "pending",                                                      │
│    CreatedAt: 2025-01-29T10:30:00Z,                                      │
│    UpdatedAt: 2025-01-29T10:30:00Z                                       │
│  }                                                                          │
│           ↓                                                                 │
│  Backend saves to database                                                │
│           ↓                                                                 │
│  Backend returns BookingResponse:                                         │
│  {                                                                          │
│    "id": "booking-uuid",                                                  │
│    "customerId": "D93A557C-A41E-4DD9-A0A7-D6B3897BC4A7",                │
│    "serviceId": "svc-001",                                               │
│    "solarDate": "2025-01-29",                                            │
│    "lunarDate": {                                                         │
│      "day": 1,                                                           │
│      "month": 1,                                                         │
│      "year": 2025,                                                       │
│      "heavenlyStems": "Ất",                                             │
│      "earthlyBranches": "Tỵ",                                           │
│      "sexagenaryCycle": "Ất Tỵ"                                         │
│    },                                                                     │
│    "customerName": "Nguyễn Văn A",                                       │
│    "customerPhone": "0912345678",                                        │
│    "customerEmail": "nguyenvana@example.com",                            │
│    "notes": "",                                                           │
│    "status": "pending",                                                   │
│    "createdAt": "2025-01-29T10:30:00Z",                                 │
│    "updatedAt": "2025-01-29T10:30:00Z"                                  │
│  }                                                                         │
│           ↓                                                                 │
│  FE receives response (201 Created)                                       │
│           ↓                                                                 │
│  Move to STEP 6                                                            │
│                                                                              │
└──────────────────────────────────────────────────────────────────────────────┘

┌──────────────────────────────────────────────────────────────────────────────┐
│ STEP 6: SUCCESS                                                              │
├──────────────────────────────────────────────────────────────────────────────┤
│                                                                              │
│  Display success message:                                                  │
│  ┌────────────────────────────────────────────────────────────────────┐   │
│  │ ✓ Đặt dịch vụ thành công!                                         │   │
│  │ Chúng tôi sẽ liên hệ với bạn sớm.                                 │   │
│  │                                                                    │   │
│  │ [Đặt dịch vụ khác] [Về trang chủ]                                │   │
│  └────────────────────────────────────────────────────────────────────┘   │
│           ↓                                                                 │
│  User can:                                                                 │
│    - Click "Đặt dịch vụ khác" to reset and book again                   │
│    - Click "Về trang chủ" to go back to home                            │
│                                                                              │
└──────────────────────────────────────────────────────────────────────────────┘
```

## 🔀 Error Handling Flow

```
┌──────────────────────────────────────────────────────────────────────────────┐
│ ERROR SCENARIOS                                                              │
├──────────────────────────────────────────────────────────────────────────────┤
│                                                                              │
│ 1. LUNAR DATE CONVERSION ERROR                                             │
│    ├─ Network error                                                        │
│    ├─ Invalid date format                                                  │
│    └─ API timeout                                                          │
│         ↓                                                                   │
│    Display: "Không thể chuyển đổi ngày. Vui lòng thử lại."               │
│         ↓                                                                   │
│    User can click "Chọn lại ngày"                                         │
│                                                                              │
│ 2. VALIDATION ERROR                                                        │
│    ├─ Missing required field                                               │
│    ├─ Invalid phone format                                                 │
│    ├─ Invalid email format                                                 │
│    └─ Service not selected                                                 │
│         ↓                                                                   │
│    Display: Specific error message                                         │
│         ↓                                                                   │
│    User corrects and resubmits                                             │
│                                                                              │
│ 3. BACKEND ERROR                                                           │
│    ├─ Invalid customer ID                                                  │
│    ├─ Service not found                                                    │
│    ├─ Database error                                                       │
│    └─ Authentication failed                                                │
│         ↓                                                                   │
│    Display: "Lỗi khi đặt dịch vụ"                                         │
│         ↓                                                                   │
│    User can click "Thử lại" or "Về trang chủ"                            │
│                                                                              │
└──────────────────────────────────────────────────────────────────────────────┘
```

## 🔐 Security Flow

```
┌──────────────────────────────────────────────────────────────────────────────┐
│ SECURITY CHECKS                                                              │
├──────────────────────────────────────────────────────────────────────────────┤
│                                                                              │
│ 1. FRONTEND SECURITY                                                       │
│    ├─ Input validation (type, length, format)                             │
│    ├─ XSS prevention (sanitize user input)                                │
│    ├─ CSRF token (if applicable)                                          │
│    └─ Secure storage of auth token                                        │
│                                                                              │
│ 2. BACKEND SECURITY                                                        │
│    ├─ Authentication check (Bearer token)                                 │
│    ├─ Authorization check (user permissions)                              │
│    ├─ CustomerId validation (must match fixed ID)                         │
│    ├─ Input validation (all fields)                                       │
│    ├─ SQL injection prevention (parameterized queries)                    │
│    ├─ Rate limiting (prevent spam)                                        │
│    └─ Logging (audit trail)                                               │
│                                                                              │
│ 3. DATA PROTECTION                                                         │
│    ├─ HTTPS/TLS encryption in transit                                     │
│    ├─ Database encryption at rest                                         │
│    ├─ PII protection (phone, email)                                       │
│    └─ Secure password hashing (if applicable)                             │
│                                                                              │
└──────────────────────────────────────────────────────────────────────────────┘
```

## 📊 State Management

```
CalendarBookingPage State:
├─ currentDate: Date (current month for calendar)
├─ selectedDate: Date | null (user selected date)
├─ lunarData: LunarDateResponse | null (converted lunar info)
├─ step: BookingStep (select-date | lunar-info | customer-info | confirm | success | error)
├─ loading: boolean (API call in progress)
├─ error: string | null (error message)
├─ successMessage: string | null (success message)
└─ formData: BookingFormData
   ├─ serviceId: string
   ├─ customerName: string
   ├─ customerPhone: string
   ├─ customerEmail: string
   └─ notes: string
```

## 🔄 Data Flow

```
User Input
    ↓
Frontend Validation
    ↓
API Call (Lunar Conversion)
    ↓
Display Lunar Info
    ↓
Collect Customer Info
    ↓
Frontend Validation
    ↓
Confirm Booking
    ↓
API Call (Create Booking)
    ↓
Backend Validation
    ↓
Database Save
    ↓
Response to Frontend
    ↓
Display Success/Error
```

## 📱 Mobile Responsive Flow

```
Desktop (≥1024px):
├─ Calendar grid: 7 columns
├─ Form: 2 columns (label + input)
└─ Confirmation: 2 columns

Tablet (768px - 1023px):
├─ Calendar grid: 7 columns
├─ Form: 1 column
└─ Confirmation: 1 column

Mobile (<768px):
├─ Calendar grid: 7 columns (smaller)
├─ Form: 1 column (full width)
└─ Confirmation: 1 column (full width)
```
