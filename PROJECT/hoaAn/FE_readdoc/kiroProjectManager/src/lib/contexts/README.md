# Authentication Context

Hệ thống xác thực và phân quyền cho ứng dụng Admin POS.

## Tính năng

- ✅ Kiểm tra đăng nhập tự động
- ✅ Kiểm tra role/quyền hạn (admin, administrator)
- ✅ Redirect về trang trước đó sau khi đăng nhập
- ✅ Tự động redirect đến login khi chưa đăng nhập
- ✅ Tự động refresh token khi hết hạn
- ✅ Hiển thị thông tin user và role

## Cách sử dụng

### 1. Sử dụng AuthContext trong component

```tsx
import { useAuth } from '../lib/contexts/AuthContext';

function MyComponent() {
    const { user, isAuthenticated, hasRole, logout } = useAuth();

    if (!isAuthenticated) {
        return <div>Chưa đăng nhập</div>;
    }

    return (
        <div>
            <p>Xin chào, {user?.fullName}</p>
            <p>Role: {user?.role}</p>
            
            {hasRole('admin') && (
                <button>Chức năng chỉ dành cho Admin</button>
            )}
            
            <button onClick={logout}>Đăng xuất</button>
        </div>
    );
}
```

### 2. Sử dụng ProtectedRoute trong router

```tsx
import { ProtectedRoute } from '../layout/ProtectedRoute';

// Yêu cầu đăng nhập và role admin/administrator (mặc định)
<Route
    path="/admin"
    element={
        <ProtectedRoute>
            <AdminPage />
        </ProtectedRoute>
    }
/>

// Yêu cầu role cụ thể
<Route
    path="/super-admin"
    element={
        <ProtectedRoute requiredRoles={['super-admin']}>
            <SuperAdminPage />
        </ProtectedRoute>
    }
/>

// Yêu cầu một trong nhiều role
<Route
    path="/manager"
    element={
        <ProtectedRoute requiredRoles={['admin', 'manager', 'supervisor']}>
            <ManagerPage />
        </ProtectedRoute>
    }
/>
```

### 3. Sử dụng useRequireAuth hook

```tsx
import { useRequireAuth } from '../lib/hooks/useRequireAuth';

function ProtectedPage() {
    // Tự động redirect nếu chưa đăng nhập hoặc không có quyền
    const { user, hasRole } = useRequireAuth(['admin', 'administrator']);

    return (
        <div>
            <h1>Trang chỉ dành cho Admin</h1>
            <p>Xin chào, {user?.fullName}</p>
        </div>
    );
}
```

## API

### AuthContext

```typescript
interface AuthContextType {
    user: User | null;              // Thông tin user hiện tại
    isLoading: boolean;             // Đang load user info
    isAuthenticated: boolean;       // Đã đăng nhập hay chưa
    hasRole: (roles: string | string[]) => boolean;  // Kiểm tra role
    login: (email: string, password: string, rememberMe?: boolean) => Promise<void>;
    logout: () => Promise<void>;
    refreshUser: () => Promise<void>;  // Reload user info
}
```

### User Interface

```typescript
interface User {
    id: string;
    email: string;
    fullName?: string;
    role?: string;  // 'admin', 'administrator', 'manager', etc.
    [key: string]: any;
}
```

## Flow hoạt động

1. **Khi user truy cập trang được bảo vệ:**
   - ProtectedRoute kiểm tra `isAuthenticated`
   - Nếu chưa đăng nhập → redirect đến `/login` và lưu URL hiện tại
   - Nếu đã đăng nhập → kiểm tra `role`
   - Nếu không có quyền → hiển thị trang "Không có quyền truy cập"

2. **Khi user đăng nhập:**
   - Gọi API login
   - Lưu token vào localStorage/sessionStorage
   - Load thông tin user
   - Kiểm tra role (admin/administrator)
   - Redirect về trang trước đó (hoặc `/` nếu không có)

3. **Khi API trả về 401:**
   - Axios interceptor tự động thử refresh token
   - Nếu refresh thành công → retry request
   - Nếu refresh thất bại → clear token và redirect đến login

4. **Khi user đăng xuất:**
   - Gọi API logout
   - Clear token
   - Clear user info
   - Redirect đến `/login`
