# Profile Page API Integration - Complete

## Tóm tắt
Đã hoàn thành cập nhật trang Profile để gọi API `GET/api/v1/CustomerAdmin/{id}` và cho phép chỉnh sửa thông tin cá nhân.

## Các file được tạo/cập nhật

### 1. Service: `src/lib/services/customerAdminService.ts`
Service để gọi API CustomerAdmin:
- `getCustomerById(id)` - GET /api/v1/CustomerAdmin/{id}
- `updateCustomer(id, data)` - PUT /api/v1/CustomerAdmin/{id}

Sử dụng Orval generated API (`getVietCommerceAPI()`)

### 2. Component: `src/components/ProfilePage.tsx`
Cập nhật Profile component:
- Thêm state để quản lý dữ liệu từ API
- Thêm useEffect để tự động load dữ liệu khi component mount
- Cập nhật hàm `handleSaveProfile()` để gọi API cập nhật
- Thêm loading/error UI states
- Hiển thị tier thành viên và điểm loyalty từ API

## Tính năng

✅ Tự động load thông tin khách hàng từ API khi component mount
✅ Hiển thị loading state trong khi tải dữ liệu
✅ Hiển thị error message nếu tải thất bại
✅ Cho phép chỉnh sửa thông tin (Họ tên, Email, Số điện thoại)
✅ Lưu thay đổi lên API
✅ Hiển thị tier thành viên từ API
✅ Hiển thị điểm thành viên (Loyalty Points)
✅ Toast notification cho success/error

## Cách sử dụng

### Gọi API lấy thông tin khách hàng
```typescript
const customerData = await customerAdminService.getCustomerById(userId);
console.log(customerData.name, customerData.email, customerData.loyaltyPoints);
```

### Cập nhật thông tin khách hàng
```typescript
const updated = await customerAdminService.updateCustomer(userId, {
  name: "Tên mới",
  email: "email@example.com",
  phone: "0912345678"
});
```

## API Endpoints

### GET /api/v1/CustomerAdmin/{id}
Lấy thông tin chi tiết khách hàng

**Response:**
```json
{
  "success": true,
  "data": {
    "id": "customer-id",
    "name": "Nguyễn Văn An",
    "email": "nguyenvanan@email.com",
    "phone": "0912345678",
    "loyaltyPoints": 1500,
    "tier": "VIP",
    "isActive": true,
    "totalOrders": 5,
    "totalSpent": 5000000
  },
  "message": "Success"
}
```

### PUT /api/v1/CustomerAdmin/{id}
Cập nhật thông tin khách hàng

**Request Body:**
```json
{
  "name": "Tên mới",
  "email": "email@example.com",
  "phone": "0912345678"
}
```

**Response:** Trả về CustomerDetailDto đã cập nhật

## Lưu ý

- Dữ liệu được load từ API dựa trên `user.id` từ useAuth hook
- Khi chỉnh sửa, chỉ gửi các field: name, email, phone
- Các field khác như loyaltyPoints, tier được quản lý từ backend
- BirthDate không được hỗ trợ bởi API, nên vẫn để trống
- Sử dụng `any` type tạm thời cho các schema từ Orval (có thể cập nhật sau)

## Troubleshooting

### Lỗi 500 khi load trang
- Kiểm tra xem path import có đúng không
- Xóa cache TypeScript và rebuild

### Lỗi "Cannot find module"
- Đảm bảo path import đúng (3 level up từ src/components/)
- Sử dụng `any` type nếu TypeScript không nhận ra module

### API trả về lỗi
- Kiểm tra xem user ID có hợp lệ không
- Kiểm tra xem token authentication có được gửi không
- Xem console log để debug
