# Hướng Dẫn Sử Dụng Hệ Thống Authentication

## Tổng Quan

Hệ thống authentication đã được cài đặt với các tính năng:

✅ **Kiểm tra đăng nhập tự động** - Tự động kiểm tra token khi load app
✅ **Kiểm tra role/quyền hạn** - Chỉ cho phép admin/administrator truy cập
✅ **Redirect thông minh** - Tự động quay lại trang trước đó sau khi đăng nhập
✅ **Auto refresh token** - Tự động làm mới token khi hết hạn
✅ **Hiển thị thông tin user** - Hiển thị tên và role trên TopBar

---

## 1. Cấu Trúc Files

```
src/
├── lib/
│   ├── contexts/
│   │   ├── AuthContext.tsx          # Context quản lý authentication
│   │   └── README.md                # Tài liệu chi tiết
│   ├── hooks/
│   │   └── useRequireAuth.ts        # Hook tiện ích
│   └── api/
│       └── client.ts                # Axios client với interceptors
├── layout/
│   ├── ProtectedRoute.tsx           # Component bảo vệ routes
│   └── TopBar.tsx                   # TopBar với user info
├── pages/
│   └── LoginPage.tsx                # Trang đăng nhập
└── components/
    └── auth/
        └── RoleGuard.tsx            # Component kiểm tra role
```

---

## 2. Cách Sử Dụng

### A. Bảo vệ Routes (Đã cài đặt sẵn)

Tất cả routes trong `src/router/index.tsx` đã được bảo vệ với `ProtectedRoute`:

```tsx
<Route
    path="/employees"
    element={
        <ProtectedRoute>
            <Layout>
                <EmployeesPage />
            </Layout>
        </ProtectedRoute>
    }
/>
```

**Mặc định:** Yêu cầu role `admin` hoặc `administrator`

**Tùy chỉnh role:**
```tsx
<ProtectedRoute requiredRoles={['manager', 'admin']}>
    <ManagerPage />
</ProtectedRoute>
```

---

### B. Sử dụng AuthContext trong Component

```tsx
import { useAuth } from '../lib/contexts/AuthContext';

function MyComponent() {
    const { user, isAuthenticated, hasRole, logout } = useAuth();

    return (
        <div>
            {/* Hiển thị thông tin user */}
            <p>Xin chào, {user?.fullName}</p>
            <p>Email: {user?.email}</p>
            <p>Role: {user?.role}</p>

            {/* Kiểm tra role */}
            {hasRole('admin') && (
                <button>Chức năng Admin</button>
            )}

            {/* Đăng xuất */}
            <button onClick={logout}>Đăng xuất</button>
        </div>
    );
}
```

---

### C. Ẩn/Hiện UI Elements theo Role

Sử dụng `RoleGuard` component:

```tsx
import { RoleGuard } from '../components/auth/RoleGuard';

function EmployeesPage() {
    return (
        <div>
            {/* Nút này chỉ hiển thị cho admin */}
            <RoleGuard requiredRoles={['admin', 'administrator']}>
                <Button>Thêm Nhân Viên</Button>
            </RoleGuard>

            {/* Với fallback */}
            <RoleGuard 
                requiredRoles="admin"
                fallback={<p>Bạn không có quyền xem nội dung này</p>}
            >
                <SecretContent />
            </RoleGuard>
        </div>
    );
}
```

**Ví dụ thực tế:** Xem `src/pages/EmployeesPage.tsx`

---

### D. Sử dụng Hook useRequireAuth

Tự động redirect nếu không có quyền:

```tsx
import { useRequireAuth } from '../lib/hooks/useRequireAuth';

function AdminOnlyPage() {
    // Tự động redirect nếu không phải admin
    const { user } = useRequireAuth(['admin', 'administrator']);

    return (
        <div>
            <h1>Trang chỉ dành cho Admin</h1>
            <p>Xin chào, {user?.fullName}</p>
        </div>
    );
}
```

---

## 3. Flow Hoạt Động

### Khi User Truy Cập Trang Được Bảo Vệ

```
1. User truy cập /employees
   ↓
2. ProtectedRoute kiểm tra isAuthenticated
   ↓
3a. Chưa đăng nhập?
    → Redirect đến /login
    → Lưu URL hiện tại (/employees)
   ↓
3b. Đã đăng nhập?
    → Kiểm tra role
    ↓
4a. Không có quyền?
    → Hiển thị "Không có quyền truy cập"
   ↓
4b. Có quyền?
    → Hiển thị trang
```

### Khi User Đăng Nhập

```
1. User nhập email/password
   ↓
2. Gọi API login
   ↓
3. Lưu token vào localStorage/sessionStorage
   ↓
4. Load thông tin user từ API
   ↓
5. Kiểm tra role (admin/administrator)
   ↓
6a. Không phải admin?
    → Hiển thị lỗi "Không có quyền"
   ↓
6b. Là admin?
    → Redirect về trang trước đó (/employees)
    → Hoặc về trang chủ (/) nếu không có
```

### Khi API Trả Về 401 (Unauthorized)

```
1. API trả về 401
   ↓
2. Axios interceptor bắt lỗi
   ↓
3. Thử refresh token
   ↓
4a. Refresh thành công?
    → Cập nhật token mới
    → Retry request ban đầu
   ↓
4b. Refresh thất bại?
    → Clear token
    → Redirect đến /login
    → Lưu URL hiện tại để quay lại sau
```

---

## 4. API AuthContext

### Properties

```typescript
interface AuthContextType {
    // Thông tin user hiện tại
    user: User | null;
    
    // Đang load user info
    isLoading: boolean;
    
    // Đã đăng nhập hay chưa
    isAuthenticated: boolean;
    
    // Kiểm tra role
    hasRole: (roles: string | string[]) => boolean;
    
    // Đăng nhập
    login: (email: string, password: string, rememberMe?: boolean) => Promise<void>;
    
    // Đăng xuất
    logout: () => Promise<void>;
    
    // Reload user info
    refreshUser: () => Promise<void>;
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

---

## 5. Ví Dụ Thực Tế

### Ví dụ 1: Trang với nhiều mức quyền

```tsx
function DashboardPage() {
    const { user, hasRole } = useAuth();

    return (
        <div>
            {/* Tất cả user đã đăng nhập đều thấy */}
            <WelcomeCard user={user} />

            {/* Chỉ admin thấy */}
            <RoleGuard requiredRoles="admin">
                <AdminStats />
            </RoleGuard>

            {/* Manager và admin đều thấy */}
            <RoleGuard requiredRoles={['admin', 'manager']}>
                <TeamStats />
            </RoleGuard>

            {/* Kiểm tra trong code */}
            {hasRole('admin') && (
                <button onClick={deleteAllData}>
                    Xóa Tất Cả Dữ Liệu
                </button>
            )}
        </div>
    );
}
```

### Ví dụ 2: Form với quyền hạn

```tsx
function ProductForm() {
    const { hasRole } = useAuth();
    const canDelete = hasRole(['admin', 'manager']);

    return (
        <form>
            <Input name="name" />
            <Input name="price" />
            
            <div className="flex gap-2">
                <Button type="submit">Lưu</Button>
                
                {canDelete && (
                    <Button variant="danger" onClick={handleDelete}>
                        Xóa
                    </Button>
                )}
            </div>
        </form>
    );
}
```

### Ví dụ 3: API call với error handling

```tsx
function ProductsPage() {
    const { logout } = useAuth();
    const [products, setProducts] = useState([]);

    useEffect(() => {
        fetchProducts();
    }, []);

    const fetchProducts = async () => {
        try {
            const response = await apiGet('/products');
            setProducts(response.data);
        } catch (error: any) {
            if (error.response?.status === 401) {
                // Axios interceptor đã xử lý, nhưng có thể thêm logic
                toast.error('Phiên đăng nhập hết hạn');
                logout();
            } else {
                toast.error('Lỗi tải dữ liệu');
            }
        }
    };

    return <ProductList products={products} />;
}
```

---

## 6. Testing

### Test đăng nhập

1. Mở app → Tự động redirect đến `/login`
2. Nhập email/password
3. Chọn "Ghi nhớ đăng nhập" (optional)
4. Click "Đăng nhập"
5. Kiểm tra:
   - ✅ Redirect về trang chủ `/`
   - ✅ TopBar hiển thị tên user và role
   - ✅ Có thể truy cập các trang được bảo vệ

### Test redirect về trang trước đó

1. Đăng xuất
2. Truy cập trực tiếp `/employees`
3. Tự động redirect đến `/login`
4. Đăng nhập thành công
5. Kiểm tra:
   - ✅ Tự động quay lại `/employees`

### Test role-based access

1. Đăng nhập với user không phải admin
2. Truy cập trang được bảo vệ
3. Kiểm tra:
   - ✅ Hiển thị "Không có quyền truy cập"
   - ✅ Có nút "Quay lại"

### Test token refresh

1. Đăng nhập
2. Đợi token hết hạn (hoặc xóa token trong DevTools)
3. Thực hiện API call
4. Kiểm tra:
   - ✅ Tự động refresh token
   - ✅ Request thành công
   - ✅ Không bị logout

---

## 7. Troubleshooting

### Lỗi: "useAuth must be used within an AuthProvider"

**Nguyên nhân:** Component không nằm trong AuthProvider

**Giải pháp:** Đảm bảo AuthProvider wrap toàn bộ app trong `src/router/index.tsx`

### Lỗi: Redirect loop (login → home → login)

**Nguyên nhân:** User không có role admin/administrator

**Giải pháp:** 
- Kiểm tra API trả về đúng role
- Hoặc thay đổi `requiredRoles` trong ProtectedRoute

### Lỗi: Token không được lưu

**Nguyên nhân:** API không trả về đúng format token

**Giải pháp:** Kiểm tra `authService.ts` và đảm bảo API trả về:
```json
{
  "data": {
    "token": "...",
    "refreshToken": "..."
  }
}
```

---

## 8. Tùy Chỉnh

### Thay đổi role mặc định

Trong `src/layout/ProtectedRoute.tsx`:

```tsx
export const ProtectedRoute: React.FC<ProtectedRouteProps> = ({ 
    children, 
    requiredRoles = ['manager', 'admin']  // Thay đổi ở đây
}) => {
    // ...
}
```

### Thêm role mới

Chỉ cần backend trả về role mới, frontend tự động hỗ trợ:

```tsx
<RoleGuard requiredRoles="super-admin">
    <SuperAdminPanel />
</RoleGuard>
```

### Tùy chỉnh trang "Không có quyền"

Trong `src/layout/ProtectedRoute.tsx`, tìm phần:

```tsx
return (
    <div className="min-h-screen flex items-center justify-center">
        {/* Tùy chỉnh UI ở đây */}
    </div>
);
```

---

## 9. Best Practices

1. **Luôn kiểm tra role ở backend** - Frontend chỉ là UI, backend mới là security
2. **Sử dụng RoleGuard cho UI elements** - Dễ đọc và maintain hơn
3. **Sử dụng ProtectedRoute cho pages** - Bảo vệ toàn bộ trang
4. **Không hardcode role strings** - Tạo constants nếu cần:
   ```tsx
   export const ROLES = {
       ADMIN: 'admin',
       MANAGER: 'manager',
       CASHIER: 'cashier',
   } as const;
   ```
5. **Log errors trong development** - Axios interceptor đã có sẵn logging

---

## 10. Tài Liệu Tham Khảo

- **AuthContext:** `src/lib/contexts/README.md`
- **API Client:** `src/lib/api/client.ts`
- **Ví dụ thực tế:** `src/pages/EmployeesPage.tsx`

---

**Lưu ý:** Hệ thống đã được cài đặt và test sẵn. Bạn chỉ cần sử dụng theo hướng dẫn trên! 🎉
