# Tóm Tắt Hệ Thống Authentication

## ✅ Đã Cài Đặt

### 1. Core Components
- **AuthContext** (`src/lib/contexts/AuthContext.tsx`) - Quản lý state authentication, roles, permissions
- **ProtectedRoute** (`src/layout/ProtectedRoute.tsx`) - Bảo vệ routes, kiểm tra role
- **RoleGuard** (`src/components/auth/RoleGuard.tsx`) - Ẩn/hiện UI theo role
- **PermissionGuard** (`src/components/auth/PermissionGuard.tsx`) - Ẩn/hiện UI theo permission

### 2. JWT Utilities
- **jwtHelper** (`src/lib/utils/jwtHelper.ts`) - Decode token, extract roles/permissions
- Hỗ trợ **roles dạng array**: `["Staff", "Administrator"]`
- Hỗ trợ **permissions dạng array**: `["product.create", "order.view", ...]`

### 3. Updated Components
- **LoginPage** - Kiểm tra role "Administrator", redirect về trang trước đó
- **TopBar** - Hiển thị tất cả roles của user
- **EmployeesPage** - Ví dụ hiển thị roles và permissions
- **Router** - Wrap với AuthProvider
- **API Client** - Auto refresh token, redirect khi 401

---

## 🚀 Cách Sử Dụng Nhanh

### Trong Component:
```tsx
import { useAuth } from '../lib/contexts/AuthContext';

const { user, roles, permissions, hasRole, hasPermission } = useAuth();

// Kiểm tra role
if (hasRole('Administrator')) {
    // User là Administrator
}

// Kiểm tra permission
if (hasPermission('product.create')) {
    // User có quyền tạo sản phẩm
}
```

### Ẩn/Hiện UI theo Role:
```tsx
import { RoleGuard } from '../components/auth/RoleGuard';

<RoleGuard requiredRoles="Administrator">
    <AdminButton />
</RoleGuard>
```

### Ẩn/Hiện UI theo Permission:
```tsx
import { PermissionGuard } from '../components/auth/PermissionGuard';

<PermissionGuard requiredPermissions="product.delete">
    <DeleteButton />
</PermissionGuard>
```

### Bảo vệ Route:
```tsx
<ProtectedRoute requiredRoles="Administrator">
    <AdminPage />
</ProtectedRoute>
```

---

## 📋 Tính Năng

✅ Kiểm tra đăng nhập tự động
✅ Kiểm tra role "Administrator" từ JWT token
✅ Hỗ trợ **multiple roles** (array): `["Staff", "Administrator"]`
✅ Hỗ trợ **permissions** (array): `["product.create", "order.view", ...]`
✅ Redirect về trang trước đó sau login
✅ Auto refresh token khi hết hạn
✅ Hiển thị tất cả roles trên TopBar
✅ Ẩn/hiện UI theo role hoặc permission
✅ JWT Helper utilities để decode và extract thông tin

---

## 📚 Tài Liệu Chi Tiết

- **JWT Token Guide:** `JWT_TOKEN_GUIDE.md` - Hướng dẫn xử lý JWT với roles/permissions array
- **Hướng dẫn đầy đủ:** `AUTH_USAGE_EXAMPLES.md` - Các ví dụ sử dụng chi tiết
- **API Reference:** `src/lib/contexts/README.md` - API documentation
- **Ví dụ thực tế:** `src/pages/EmployeesPage.tsx` - Code example

## 🔑 Token Structure

Token payload của bạn:
```json
{
  "nameid": "user-id",
  "email": "user@example.com",
  "role": ["Staff", "Administrator"],  // ← Array of roles
  "permission": ["product.create", "order.view", ...],  // ← Array of permissions
  "exp": 1764756634
}
```

Hệ thống tự động:
- Extract roles từ token → Lưu vào `AuthContext.roles`
- Extract permissions từ token → Lưu vào `AuthContext.permissions`
- Kiểm tra "Administrator" trong array roles → Cho phép đăng nhập
