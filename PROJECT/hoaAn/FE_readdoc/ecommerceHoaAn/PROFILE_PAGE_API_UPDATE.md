# Profile Page API Integration - Cập nhật

## Tổng quan
Đã cập nhật trang Profile để gọi API `GET/api/v1/CustomerAdmin/{id}` và đổ dữ liệu vào form chỉnh sửa thông tin cá nhân.

## Các thay đổi

### 1. Tạo Service mới: `customerAdminService.ts`
**Đường dẫn:** `src/lib/services/customerAdminService.ts`

Service này cung cấp 2 hàm chính:
- `getCustomerById(id: string)` - Lấy thông tin khách hàng từ API
- `updateCustomer(id: string, data: UpdateCustomerRequest)` - Cập nhật thông tin khách hàng

```typescript
// Ví dụ sử dụng
const customerData = await customerAdminService.getCustomerById(userId);
const updated = await customerAdminService.updateCustomer(userId, {
  name: "Tên mới",
  email: "email@example.com",
  phone: "0912345678"
});
```

### 2. Cập nhật ProfilePage Component
**Đường dẫn:** `src/components/ProfilePage.tsx`

#### Thêm State mới:
```typescript
const [customerData, setCustomerData] = useState<CustomerDetailDto | null>(null);
const [customerLoading, setCustomerLoading] = useState(true);
const [customerError, setCustomerError] = useState<string | null>(null);
const [isSaving, setIsSaving] = useState(false);
```

#### Thêm useEffect để load dữ liệu:
```typescript
useEffect(() => {
  const loadCustomerData = async () => {
    if (!user?.id) return;
    
    try {
      const data = await customerAdminService.getCustomerById(user.id);
      setCustomerData(data);
      setUserInfo(prev => ({
        ...prev,
        name: data.name || "",
        email: data.email || "",
        phone: data.phone || "",
      }));
    } catch (error) {
      setCustomerError("Không thể tải thông tin cá nhân");
      toast.error("Không thể tải thông tin cá nhân");
    } finally {
      setCustomerLoading(false);
    }
  };
  
  loadCustomerData();
}, [user?.id]);
```

#### Cập nhật hàm handleSaveProfile:
```typescript
const handleSaveProfile = async () => {
  if (!user?.id) {
    toast.error("Không thể xác định người dùng");
    return;
  }

  try {
    setIsSaving(true);
    const updateData: UpdateCustomerRequest = {
      name: userInfo.name,
      email: userInfo.email,
      phone: userInfo.phone,
    };

    const updatedData = await customerAdminService.updateCustomer(
      user.id,
      updateData
    );

    setCustomerData(updatedData);
    setIsEditing(false);
    toast.success("Cập nhật thông tin thành công!");
  } catch (error) {
    toast.error("Cập nhật thông tin thất bại");
  } finally {
    setIsSaving(false);
  }
};
```

#### Cập nhật UI:
- Thêm loading state khi tải dữ liệu
- Thêm error state nếu tải thất bại
- Hiển thị tier thành viên từ API: `{customerData?.tier || "VIP"}`
- Hiển thị điểm thành viên: `{customerData?.loyaltyPoints || 0}`
- Thêm loading indicator khi lưu dữ liệu

## Schema được sử dụng

### CustomerDetailDto
```typescript
interface CustomerDetailDto {
  id?: string;
  name?: string | null;
  email?: string | null;
  phone?: string | null;
  loyaltyPoints?: number;
  tier?: string | null;
  isActive?: boolean;
  userId?: string | null;
  storeId?: string | null;
  storeName?: string | null;
  tenantId?: string;
  createdAt?: string;
  updatedAt?: string;
  createdBy?: string;
  updatedBy?: string | null;
  totalOrders?: number;
  totalSpent?: number;
  totalInteractions?: number;
  addresses?: CustomerAddressDto[] | null;
}
```

### UpdateCustomerRequest
```typescript
interface UpdateCustomerRequest {
  name?: string | null;
  email?: string | null;
  phone?: string | null;
  loyaltyPoints?: number | null;
  tier?: string | null;
  isActive?: boolean | null;
  storeId?: string | null;
}
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

## Tính năng

✅ Tự động load thông tin khách hàng khi component mount
✅ Hiển thị loading state trong khi tải dữ liệu
✅ Hiển thị error message nếu tải thất bại
✅ Cho phép chỉnh sửa thông tin (Họ tên, Email, Số điện thoại)
✅ Lưu thay đổi lên API
✅ Hiển thị tier thành viên từ API
✅ Hiển thị điểm thành viên (Loyalty Points)
✅ Toast notification cho success/error

## Lưu ý

- Dữ liệu được load từ API dựa trên `user.id` từ useAuth hook
- Khi chỉnh sửa, chỉ gửi các field: name, email, phone
- Các field khác như loyaltyPoints, tier được quản lý từ backend
- BirthDate không được hỗ trợ bởi API, nên vẫn để trống
