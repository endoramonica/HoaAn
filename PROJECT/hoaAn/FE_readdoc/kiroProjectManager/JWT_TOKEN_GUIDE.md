# Hướng Dẫn Xử Lý JWT Token

## Cấu Trúc Token

Token của hệ thống có cấu trúc payload như sau:

```json
{
  "nameid": "2aaa2386-e5a8-469e-959b-147adddf15a1",
  "email": "user111@example.com",
  "jti": "b897b6b6-d0da-47c2-8f91-64ce6e2dbb63",
  "iat": 1764749434,
  "role": ["Staff", "Administrator"],
  "permission": [
    "order.view_all",
    "inventory.view",
    "product.create",
    "order.cancel",
    "pos.access",
    "admin.manage_users",
    ...
  ],
  "nbf": 1764749434,
  "exp": 1764756634,
  "iss": "VietCommerce.Api",
  "aud": "VietCommerce.Client"
}
```

### Đặc điểm quan trọng:

1. **`role` là Array** - Một user có thể có nhiều roles: `["Staff", "Administrator"]`
2. **`permission` là Array** - Danh sách các quyền hạn cụ thể
3. **`nameid`** - User ID
4. **`email`** - Email của user
5. **`exp`** - Thời gian hết hạn (Unix timestamp)

---

## JWT Helper Functions

### 1. Decode Token

```typescript
import { decodeJWT } from '../lib/utils/jwtHelper';

const token = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...";
const payload = decodeJWT(token);

console.log(payload);
// {
//   nameid: "2aaa2386-e5a8-469e-959b-147adddf15a1",
//   email: "user111@example.com",
//   role: ["Staff", "Administrator"],
//   ...
// }
```

### 2. Extract Roles

```typescript
import { getRolesFromToken } from '../lib/utils/jwtHelper';

const roles = getRolesFromToken(token);
console.log(roles); // ["Staff", "Administrator"]
```

### 3. Check Role

```typescript
import { hasRole, isAdministrator } from '../lib/utils/jwtHelper';

// Kiểm tra một role
if (hasRole(token, 'Administrator')) {
    console.log('User là Administrator');
}

// Kiểm tra nhiều roles (có một trong số đó là đủ)
if (hasRole(token, ['Admin', 'Administrator', 'Manager'])) {
    console.log('User có quyền quản lý');
}

// Kiểm tra nhanh Administrator
if (isAdministrator(token)) {
    console.log('User là Administrator');
}
```

### 4. Extract Permissions

```typescript
import { getPermissionsFromToken, hasPermission } from '../lib/utils/jwtHelper';

// Lấy tất cả permissions
const permissions = getPermissionsFromToken(token);
console.log(permissions); // ["order.view_all", "inventory.view", ...]

// Kiểm tra permission
if (hasPermission(token, 'product.create')) {
    console.log('User có quyền tạo sản phẩm');
}

// Kiểm tra nhiều permissions
if (hasPermission(token, ['product.create', 'product.update'])) {
    console.log('User có quyền tạo hoặc sửa sản phẩm');
}
```

### 5. Check Token Expiration

```typescript
import { isTokenExpired, getTokenExpiration } from '../lib/utils/jwtHelper';

// Kiểm tra token hết hạn chưa
if (isTokenExpired(token)) {
    console.log('Token đã hết hạn');
}

// Lấy thời gian hết hạn
const expDate = getTokenExpiration(token);
console.log('Token hết hạn lúc:', expDate);
```

---

## Sử Dụng Trong Component

### 1. Kiểm Tra Role

```typescript
import { useAuth } from '../lib/contexts/AuthContext';

function MyComponent() {
    const { roles, hasRole } = useAuth();

    return (
        <div>
            {/* Hiển thị tất cả roles */}
            <div>
                Roles: {roles.join(', ')}
            </div>

            {/* Kiểm tra role cụ thể */}
            {hasRole('Administrator') && (
                <button>Chức năng Admin</button>
            )}

            {/* Kiểm tra nhiều roles */}
            {hasRole(['Administrator', 'Manager']) && (
                <button>Chức năng Quản lý</button>
            )}
        </div>
    );
}
```

### 2. Kiểm Tra Permission

```typescript
import { useAuth } from '../lib/contexts/AuthContext';

function ProductPage() {
    const { permissions, hasPermission } = useAuth();

    return (
        <div>
            {/* Hiển thị số lượng permissions */}
            <p>Bạn có {permissions.length} quyền hạn</p>

            {/* Kiểm tra permission cụ thể */}
            {hasPermission('product.create') && (
                <button>Thêm Sản Phẩm</button>
            )}

            {hasPermission('product.delete') && (
                <button>Xóa Sản Phẩm</button>
            )}
        </div>
    );
}
```

### 3. Sử Dụng RoleGuard

```typescript
import { RoleGuard } from '../components/auth/RoleGuard';

function Dashboard() {
    return (
        <div>
            {/* Chỉ Administrator mới thấy */}
            <RoleGuard requiredRoles="Administrator">
                <AdminPanel />
            </RoleGuard>

            {/* Staff hoặc Administrator đều thấy */}
            <RoleGuard requiredRoles={['Staff', 'Administrator']}>
                <StaffPanel />
            </RoleGuard>
        </div>
    );
}
```

### 4. Sử Dụng PermissionGuard

```typescript
import { PermissionGuard } from '../components/auth/PermissionGuard';

function ProductList() {
    return (
        <div>
            {/* Chỉ hiển thị nếu có quyền tạo sản phẩm */}
            <PermissionGuard requiredPermissions="product.create">
                <button>Thêm Sản Phẩm</button>
            </PermissionGuard>

            {/* Chỉ hiển thị nếu có quyền xóa */}
            <PermissionGuard requiredPermissions="product.delete">
                <button>Xóa Sản Phẩm</button>
            </PermissionGuard>

            {/* Với fallback */}
            <PermissionGuard 
                requiredPermissions="admin.super_admin"
                fallback={<p>Bạn không có quyền xem nội dung này</p>}
            >
                <SuperAdminContent />
            </PermissionGuard>
        </div>
    );
}
```

---

## Flow Xác Thực

### 1. Đăng Nhập

```
User nhập email/password
    ↓
API trả về token
    ↓
Lưu token vào localStorage/sessionStorage
    ↓
Decode token để lấy roles và permissions
    ↓
Kiểm tra có role "Administrator"?
    ↓
Có → Cho phép đăng nhập
Không → Hiển thị lỗi "Không có quyền"
```

### 2. Load User Info

```
App khởi động
    ↓
Kiểm tra token trong storage
    ↓
Có token?
    ↓
Decode token → Extract roles & permissions
    ↓
Gọi API getCurrentUser() → Lấy thông tin user
    ↓
Merge roles từ token với user data
    ↓
Set user, roles, permissions vào AuthContext
```

### 3. Kiểm Tra Quyền

```
Component render
    ↓
Gọi hasRole('Administrator')
    ↓
So sánh với roles array từ token
    ↓
Trả về true/false
    ↓
Hiển thị/ẩn UI tương ứng
```

---

## Ví Dụ Thực Tế

### Ví dụ 1: Trang với nhiều mức quyền

```typescript
function EmployeesPage() {
    const { roles, permissions, hasRole, hasPermission } = useAuth();

    return (
        <div>
            {/* Hiển thị thông tin user */}
            <div className="user-info">
                <p>Roles: {roles.join(', ')}</p>
                <p>Permissions: {permissions.length}</p>
            </div>

            {/* Chỉ Administrator mới thấy */}
            <RoleGuard requiredRoles="Administrator">
                <button>Thêm Nhân Viên</button>
            </RoleGuard>

            {/* Kiểm tra permission cụ thể */}
            <PermissionGuard requiredPermissions="hrm.manage">
                <button>Quản Lý Nhân Sự</button>
            </PermissionGuard>

            {/* Kiểm tra trong code */}
            {hasPermission('hrm.approve_leave_requests') && (
                <button>Duyệt Đơn Nghỉ</button>
            )}
        </div>
    );
}
```

### Ví dụ 2: Protected Route với role check

```typescript
// Trong router
<ProtectedRoute requiredRoles="Administrator">
    <AdminPage />
</ProtectedRoute>

// ProtectedRoute sẽ:
// 1. Kiểm tra isAuthenticated
// 2. Kiểm tra hasRole('Administrator')
// 3. Nếu không có → Hiển thị "Không có quyền truy cập"
```

### Ví dụ 3: Dynamic UI dựa trên permissions

```typescript
function OrderActions({ orderId }: { orderId: string }) {
    const { hasPermission } = useAuth();

    return (
        <div className="actions">
            {hasPermission('order.view') && (
                <button>Xem</button>
            )}
            
            {hasPermission('order.update') && (
                <button>Sửa</button>
            )}
            
            {hasPermission('order.cancel') && (
                <button>Hủy</button>
            )}
            
            {hasPermission('order.update_status') && (
                <select>
                    <option>Đang xử lý</option>
                    <option>Đã giao</option>
                </select>
            )}
        </div>
    );
}
```

---

## Best Practices

### 1. Luôn kiểm tra ở Backend
Frontend chỉ là UI/UX, backend mới là security thật sự.

### 2. Sử dụng Guards cho UI
```typescript
// ✅ Good - Dễ đọc
<RoleGuard requiredRoles="Administrator">
    <AdminButton />
</RoleGuard>

// ❌ Bad - Khó maintain
{user?.role?.includes('Administrator') && <AdminButton />}
```

### 3. Tách biệt Role và Permission
- **Role**: Vai trò tổng quát (Administrator, Staff, Manager)
- **Permission**: Quyền hạn cụ thể (product.create, order.cancel)

```typescript
// Kiểm tra role cho các tính năng lớn
<RoleGuard requiredRoles="Administrator">
    <AdminDashboard />
</RoleGuard>

// Kiểm tra permission cho các hành động cụ thể
<PermissionGuard requiredPermissions="product.delete">
    <DeleteButton />
</PermissionGuard>
```

### 4. Cache roles và permissions
AuthContext đã tự động cache, không cần decode token mỗi lần:

```typescript
// ✅ Good - Sử dụng từ context
const { roles, permissions } = useAuth();

// ❌ Bad - Decode lại mỗi lần
const token = tokenStorage.getAccessToken();
const roles = getRolesFromToken(token);
```

### 5. Handle token expiration
Axios interceptor đã tự động xử lý, nhưng có thể thêm logic:

```typescript
import { isTokenExpired } from '../lib/utils/jwtHelper';

function useTokenCheck() {
    const { logout } = useAuth();
    
    useEffect(() => {
        const interval = setInterval(() => {
            const token = tokenStorage.getAccessToken();
            if (token && isTokenExpired(token)) {
                logout();
            }
        }, 60000); // Check mỗi phút
        
        return () => clearInterval(interval);
    }, [logout]);
}
```

---

## Troubleshooting

### Lỗi: roles luôn là empty array

**Nguyên nhân:** Token không có field `role` hoặc format không đúng

**Giải pháp:**
```typescript
// Debug token
const token = tokenStorage.getAccessToken();
const payload = decodeJWT(token);
console.log('Token payload:', payload);
console.log('Roles:', payload?.role);
```

### Lỗi: hasRole không hoạt động

**Nguyên nhân:** So sánh case-sensitive

**Giải pháp:** JWT helper đã xử lý case-insensitive:
```typescript
// Cả hai đều work
hasRole('administrator')
hasRole('Administrator')
```

### Lỗi: User có role nhưng vẫn bị từ chối

**Nguyên nhân:** Role trong token khác với role check

**Giải pháp:**
```typescript
// Debug
const { roles } = useAuth();
console.log('User roles:', roles);
console.log('Required roles:', ['Administrator']);
console.log('Has role?', hasRole('Administrator'));
```

---

## API Reference

### JWT Helper Functions

```typescript
// Decode token
decodeJWT(token: string): JWTPayload | null

// Extract info
getUserIdFromToken(token: string): string | null
getEmailFromToken(token: string): string | null
getRolesFromToken(token: string): string[]
getPermissionsFromToken(token: string): string[]

// Check role
hasRole(token: string, roles: string | string[]): boolean
isAdministrator(token: string): boolean

// Check permission
hasPermission(token: string, permissions: string | string[]): boolean

// Check expiration
isTokenExpired(token: string): boolean
getTokenExpiration(token: string): Date | null
```

### AuthContext

```typescript
interface AuthContextType {
    user: User | null;
    isLoading: boolean;
    isAuthenticated: boolean;
    roles: string[];                    // Array of roles from token
    permissions: string[];              // Array of permissions from token
    hasRole: (roles: string | string[]) => boolean;
    hasPermission: (permissions: string | string[]) => boolean;
    login: (email: string, password: string, rememberMe?: boolean) => Promise<void>;
    logout: () => Promise<void>;
    refreshUser: () => Promise<void>;
}
```

---

**Lưu ý:** Hệ thống đã được cấu hình để xử lý token với roles và permissions dạng array. Chỉ cần sử dụng theo hướng dẫn trên! 🎉
