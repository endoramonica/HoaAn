# Profile Page - Response Parse Fix

## Vấn đề
API trả về response với cấu trúc nested:
```json
{
  "success": true,
  "data": {
    "customer": { "id", "loyaltyPoints", "isActive", "addresses", ... },
    "user": { "id", "email", "name", "phone", ... }
  },
  "message": "Operation successful"
}
```

Nhưng service đang parse sai, lấy `response.data` trực tiếp thay vì `response.data.customer`.

## Giải pháp

### 1. Cập nhật customerAdminService
**File:** `src/lib/services/customerAdminService.ts`

```typescript
async getCustomerById(id: string): Promise<CustomerDetailDto> {
  try {
    const response = await api.getApiV1CustomerAdminId(id);
    
    // Extract customer data from nested structure
    // Response: { data: { customer: {...}, user: {...} } }
    const customerData = response.data?.customer || response.data;
    
    if (!customerData) {
      throw new Error('No customer data returned');
    }
    
    return customerData;
  } catch (error) {
    console.error('[customerAdminService] Failed to get customer:', error);
    throw error;
  }
}
```

### 2. Cập nhật ProfilePage để xử lý response
**File:** `src/components/ProfilePage.tsx`

```typescript
const data = await customerAdminService.getCustomerById(user.customerId);
setCustomerData(data);

// Response contains both customer and user data
const userData = (data as any).user || {};

// Populate userInfo with API data
setUserInfo((prev) => ({
  ...prev,
  name: userData.name || user.fullName || "",
  email: userData.email || user.email || "",
  phone: userData.phone || user.phoneNumber || "",
  birthDate: "",
}));
```

### 3. Cập nhật cách truy cập dữ liệu
**Hiển thị tier:**
```typescript
// ❌ Sai (cũ)
{customerData?.tier || "VIP"}

// ✅ Đúng (mới)
{(customerData as any)?.customer?.tier || "VIP"}
```

**Hiển thị loyalty points:**
```typescript
// ❌ Sai (cũ)
{customerData?.loyaltyPoints || 0}

// ✅ Đúng (mới)
{(customerData as any)?.customer?.loyaltyPoints || 0}
```

## Response Structure

```
Response từ API:
{
  success: true,
  data: {
    customer: {
      id: "d93a557c-a41e-4dd9-a0a7-d6b3897bc4a7",
      loyaltyPoints: 0,
      isActive: false,
      userId: "48fdbb7b-9d91-4e41-872e-dd8296a0f315",
      storeId: "47aa5519-c503-4cfa-8101-2edb36fd9d8c",
      totalOrders: 12,
      totalSpent: 45999000.00,
      addresses: [...]
    },
    user: {
      id: "48fdbb7b-9d91-4e41-872e-dd8296a0f315",
      email: "user555@example.com",
      name: "user555",
      phone: "1234567890",
      isActive: true,
      status: "active"
    }
  },
  message: "Operation successful"
}

Service trả về:
- customerData = response.data.customer (hoặc response.data nếu không có nested)
- userData = customerData.user (nếu có)
```

## Dữ liệu được sử dụng

| Trường | Nguồn | Mục đích |
|--------|-------|---------|
| name | userData.name | Hiển thị tên người dùng |
| email | userData.email | Hiển thị email |
| phone | userData.phone | Hiển thị số điện thoại |
| loyaltyPoints | customerData.loyaltyPoints | Hiển thị điểm thành viên |
| tier | customerData.tier | Hiển thị hạng thành viên |
| isActive | customerData.isActive | Kiểm tra trạng thái |
| addresses | customerData.addresses | Danh sách địa chỉ |

## Các file được cập nhật

1. ✅ `src/lib/services/customerAdminService.ts` - Parse response đúng
2. ✅ `src/components/ProfilePage.tsx` - Xử lý dữ liệu từ response

## Test

```typescript
// Kiểm tra trong console
const response = await customerAdminService.getCustomerById(customerId);
console.log('Customer data:', response.customer);
console.log('User data:', response.user);
console.log('Loyalty points:', response.customer.loyaltyPoints);
```
