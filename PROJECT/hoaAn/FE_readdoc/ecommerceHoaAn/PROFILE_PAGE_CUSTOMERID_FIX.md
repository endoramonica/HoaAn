# Profile Page - Customer ID Fix

## Vấn đề
Backend báo lỗi "Customer not found" vì frontend đang gửi `userId` thay vì `customerId`.

### Nguyên nhân
- API endpoint: `GET /api/v1/CustomerAdmin/{id}` tìm Customer theo `Customer.Id` (customerId)
- Frontend đang gửi: `user.id` (userId) - sai!
- Token chứa: `customerId` (đúng) nhưng frontend không lấy

### Sự khác biệt
```
userId:     48fdbb7b-9d91-4e41-872e-dd8296a0f315  ← Tài khoản người dùng
customerId: d93a557c-a41e-4dd9-a0a7-d6b3897bc4a7  ← Hồ sơ khách hàng
```

## Giải pháp

### 1. Thêm `customerId` vào UserDto
**File:** `src/lib/api/types.ts`

```typescript
export interface UserDto {
  id: string;
  email: string;
  fullName: string;
  phoneNumber?: string;
  avatar?: string;
  role: UserRole;
  isEmailConfirmed: boolean;
  createdAt: string;
  updatedAt: string;
  customerId?: string; // ← Thêm dòng này
}
```

### 2. Thêm hàm decode token
**File:** `src/lib/api/client.ts`

```typescript
export const tokenStorage = {
  // ... existing code ...

  /**
   * Decode JWT token to get claims
   */
  decodeToken: (token: string): any => {
    try {
      const parts = token.split('.');
      if (parts.length !== 3) return null;
      return JSON.parse(atob(parts[1]));
    } catch (error) {
      console.error('[TokenStorage] Failed to decode token:', error);
      return null;
    }
  },

  /**
   * Get customerId from access token
   */
  getCustomerId: (): string | null => {
    const token = tokenStorage.getAccessToken();
    if (!token) return null;
    const decoded = tokenStorage.decodeToken(token);
    return decoded?.customerId || null;
  },
};
```

### 3. Cập nhật useAuth để lấy customerId
**File:** `src/lib/hooks/useAuth.ts`

```typescript
useEffect(() => {
  const loadUser = () => {
    try {
      const hasToken = authService.isAuthenticated();
      if (hasToken) {
        const savedUser = getUserFromStorage();
        if (savedUser) {
          // Get customerId from token
          const customerId = tokenStorage.getCustomerId();
          if (customerId) {
            savedUser.customerId = customerId;
          }
          setUser(savedUser);
        }
      }
    } catch (err) {
      // ...
    }
  };
  loadUser();
}, []);
```

### 4. Cập nhật ProfilePage để sử dụng customerId
**File:** `src/components/ProfilePage.tsx`

```typescript
// ❌ Sai (cũ)
const data = await customerAdminService.getCustomerById(user.id);

// ✅ Đúng (mới)
const data = await customerAdminService.getCustomerById(user.customerId);
```

## Các file được cập nhật

1. ✅ `src/lib/api/types.ts` - Thêm `customerId` vào UserDto
2. ✅ `src/lib/api/client.ts` - Thêm `decodeToken()` và `getCustomerId()`
3. ✅ `src/lib/hooks/useAuth.ts` - Lấy customerId từ token
4. ✅ `src/components/ProfilePage.tsx` - Sử dụng `user.customerId` thay vì `user.id`

## Cách hoạt động

1. **Login:** Backend trả về token chứa `customerId`
2. **Token Storage:** Lưu token vào localStorage/sessionStorage
3. **useAuth:** Decode token để lấy `customerId` và thêm vào user object
4. **ProfilePage:** Sử dụng `user.customerId` để gọi API
5. **API:** Backend tìm Customer theo `customerId` ✅

## Test

```typescript
// Kiểm tra trong console
const user = useAuth().user;
console.log('userId:', user.id);           // 48fdbb7b-9d91-4e41-872e-dd8296a0f315
console.log('customerId:', user.customerId); // d93a557c-a41e-4dd9-a0a7-d6b3897bc4a7
```

## Lưu ý

- `customerId` được lấy từ JWT token, không phải từ response
- Token được decode mỗi lần load user từ storage
- Nếu token không có `customerId`, sẽ trả về `null`
- ProfilePage sẽ không load dữ liệu nếu `customerId` không có
