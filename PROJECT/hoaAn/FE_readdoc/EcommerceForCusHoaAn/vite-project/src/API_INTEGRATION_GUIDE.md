# Hướng dẫn Tích hợp API với Backend ASP.NET Core

## 📚 Tổng quan

Hệ thống quản lý API này được thiết kế để tích hợp hoàn toàn với backend ASP.NET Core (.NET 8), bao gồm:

- **API Client Layer**: Axios instance với interceptors, JWT authentication, auto refresh token
- **Service Layer**: RESTful API services theo chuẩn ASP.NET Core
- **Custom Hooks Layer**: React hooks để quản lý state, caching, error handling

## 🏗️ Kiến trúc 3 lớp

```
┌─────────────────────────────────────────────────────────┐
│                   React Components                       │
│  (HomePage, ProductsPage, OrdersPage, etc.)             │
└────────────────────┬────────────────────────────────────┘
                     │
                     ▼
┌─────────────────────────────────────────────────────────┐
│              Custom Hooks Layer                          │
│  useAuth(), useProducts(), useOrders()                  │
│  - State management                                      │
│  - Auto-refresh                                          │
│  - Error toasts                                          │
│  - Caching                                              │
└────────────────────┬────────────────────────────────────┘
                     │
                     ▼
┌─────────────────────────────────────────────────────────┐
│              Service Layer                               │
│  authService, productService, orderService              │
│  - RESTful API calls                                     │
│  - DTO/Model mapping                                     │
│  - Mock data fallback                                    │
└────────────────────┬────────────────────────────────────┘
                     │
                     ▼
┌─────────────────────────────────────────────────────────┐
│              API Client Layer                            │
│  apiClient (Axios instance)                             │
│  - Request/Response interceptors                         │
│  - JWT auto-attach                                       │
│  - Auto refresh token                                    │
│  - Error handling                                        │
│  - Logging                                              │
└────────────────────┬────────────────────────────────────┘
                     │
                     ▼
              ASP.NET Core API
         (http://localhost:5000/api/v1)
```

## 🚀 Cài đặt & Cấu hình

### 1. Tạo file `.env`

Copy file `.env.example` thành `.env`:

```bash
cp .env.example .env
```

Cấu hình các biến môi trường:

```env
# URL của backend API
REACT_APP_API_URL=http://localhost:5000/api/v1

# Sử dụng mock data (true/false)
REACT_APP_USE_MOCK=true
```

**Lưu ý:**
- Khi `REACT_APP_USE_MOCK=true`: Sử dụng mock data, không cần backend
- Khi `REACT_APP_USE_MOCK=false`: Kết nối với backend API thực

### 2. Cài đặt dependencies

```bash
npm install axios
```

## 📁 Cấu trúc File

```
/lib
├── api/
│   ├── client.ts          # Axios instance & interceptors
│   ├── types.ts           # TypeScript types (DTOs)
│   └── errors.ts          # Error handling
├── services/
│   ├── authService.ts     # Authentication service
│   ├── productService.ts  # Product service
│   └── orderService.ts    # Order service
└── hooks/
    ├── useAuth.ts         # Authentication hook
    ├── useProducts.ts     # Products hook
    └── useOrders.ts       # Orders hook
```

## 🔐 Authentication Flow

### 1. Login

```typescript
import { useAuth } from './lib/hooks/useAuth';

function LoginPage() {
  const { login, isLoading, error } = useAuth();

  const handleLogin = async () => {
    try {
      await login({
        email: 'user@example.com',
        password: 'password123',
      });
      // Auto redirect hoặc update UI
    } catch (err) {
      // Error đã được toast tự động
    }
  };

  return (
    <button onClick={handleLogin} disabled={isLoading}>
      Đăng nhập
    </button>
  );
}
```

### 2. Auto Refresh Token

Khi access token hết hạn (401), hệ thống tự động:
1. Gọi API `/auth/refresh-token` với refresh token
2. Lưu access token mới
3. Retry request ban đầu
4. Nếu refresh failed → auto logout & redirect to login

### 3. Protected Routes

```typescript
import { useAuth } from './lib/hooks/useAuth';

function ProtectedPage() {
  const { isAuthenticated, user, isLoading } = useAuth();

  if (isLoading) return <div>Loading...</div>;
  
  if (!isAuthenticated) {
    // Redirect to login
    window.location.href = '/login';
    return null;
  }

  return <div>Welcome {user?.fullName}</div>;
}
```

## 🛍️ Products Usage

### 1. Danh sách sản phẩm với filter & pagination

```typescript
import { useProducts } from './lib/hooks/useProducts';

function ProductsPage() {
  const {
    products,
    pagedData,
    categories,
    isLoading,
    params,
    setParams,
    goToPage,
    nextPage,
    previousPage,
  } = useProducts({
    autoLoad: true,
    initialParams: {
      pageNumber: 1,
      pageSize: 12,
      sortBy: 'createdAt',
      sortOrder: 'desc',
    },
  });

  // Filter by category
  const handleCategoryChange = (categoryId: string) => {
    setParams({ ...params, categoryId, pageNumber: 1 });
  };

  // Search
  const handleSearch = (search: string) => {
    setParams({ ...params, search, pageNumber: 1 });
  };

  return (
    <div>
      {/* Category filter */}
      <select onChange={(e) => handleCategoryChange(e.target.value)}>
        <option value="">All</option>
        {categories.map(cat => (
          <option key={cat.id} value={cat.id}>{cat.name}</option>
        ))}
      </select>

      {/* Products grid */}
      <div className="grid grid-cols-4 gap-4">
        {products.map(product => (
          <div key={product.id}>{product.name}</div>
        ))}
      </div>

      {/* Pagination */}
      <div>
        <button onClick={previousPage} disabled={!pagedData?.hasPreviousPage}>
          Previous
        </button>
        <span>Page {pagedData?.pageNumber} / {pagedData?.totalPages}</span>
        <button onClick={nextPage} disabled={!pagedData?.hasNextPage}>
          Next
        </button>
      </div>
    </div>
  );
}
```

### 2. Chi tiết sản phẩm

```typescript
import { useProducts } from './lib/hooks/useProducts';
import { useEffect, useState } from 'react';

function ProductDetailPage({ productId }: { productId: string }) {
  const { getProductById } = useProducts({ autoLoad: false });
  const [product, setProduct] = useState(null);

  useEffect(() => {
    const loadProduct = async () => {
      const data = await getProductById(productId);
      setProduct(data);
    };
    loadProduct();
  }, [productId]);

  if (!product) return <div>Loading...</div>;

  return (
    <div>
      <h1>{product.name}</h1>
      <p>{product.description}</p>
      <p>Price: {product.price.toLocaleString('vi-VN')}đ</p>
    </div>
  );
}
```

## 🛒 Orders Usage

### 1. Tạo đơn hàng

```typescript
import { useOrders } from './lib/hooks/useOrders';

function CheckoutPage() {
  const { createOrder, isLoading } = useOrders({ autoLoad: false });

  const handleCheckout = async () => {
    const order = await createOrder({
      items: [
        { productId: 'product-1', quantity: 2 },
        { productId: 'product-2', quantity: 1 },
      ],
      shippingAddress: {
        fullName: 'Nguyễn Văn A',
        phoneNumber: '0901234567',
        addressLine1: '123 Nguyễn Huệ',
        ward: 'Phường Bến Nghé',
        district: 'Quận 1',
        province: 'TP. Hồ Chí Minh',
      },
      paymentMethod: 'COD',
      note: 'Giao giờ hành chính',
    });

    if (order) {
      // Redirect to order detail or success page
      console.log('Order created:', order.orderNumber);
    }
  };

  return (
    <button onClick={handleCheckout} disabled={isLoading}>
      Đặt hàng
    </button>
  );
}
```

### 2. Lịch sử đơn hàng

```typescript
import { useOrders } from './lib/hooks/useOrders';

function MyOrdersPage() {
  const {
    orders,
    pagedData,
    isLoading,
    params,
    setParams,
    cancelOrder,
  } = useOrders({
    autoLoad: true,
    myOrdersOnly: true,
    initialParams: {
      pageNumber: 1,
      pageSize: 10,
    },
  });

  const handleCancel = async (orderId: string) => {
    const success = await cancelOrder(orderId, 'Đổi ý không mua nữa');
    if (success) {
      // Orders tự động refresh
    }
  };

  return (
    <div>
      {orders.map(order => (
        <div key={order.id}>
          <h3>Đơn hàng #{order.orderNumber}</h3>
          <p>Trạng thái: {order.status}</p>
          <p>Tổng tiền: {order.total.toLocaleString('vi-VN')}đ</p>
          {order.status === 'Pending' && (
            <button onClick={() => handleCancel(order.id)}>
              Hủy đơn
            </button>
          )}
        </div>
      ))}
    </div>
  );
}
```

### 3. Theo dõi đơn hàng

```typescript
import { useOrders } from './lib/hooks/useOrders';
import { useState } from 'react';

function TrackOrderPage() {
  const { trackOrder } = useOrders({ autoLoad: false });
  const [tracking, setTracking] = useState(null);

  const handleTrack = async (trackingNumber: string) => {
    const result = await trackOrder(trackingNumber);
    setTracking(result);
  };

  return (
    <div>
      <input 
        placeholder="Nhập mã vận đơn"
        onBlur={(e) => handleTrack(e.target.value)}
      />
      
      {tracking && (
        <div>
          <h3>Trạng thái: {tracking.order.status}</h3>
          <div>
            {tracking.timeline.map((event, index) => (
              <div key={index}>
                <p>{event.status}</p>
                <p>{event.description}</p>
                <p>{new Date(event.timestamp).toLocaleString('vi-VN')}</p>
              </div>
            ))}
          </div>
        </div>
      )}
    </div>
  );
}
```

## 🔧 Backend API Requirements

### ASP.NET Core API Format

Backend API cần follow các chuẩn sau:

#### 1. Base URL Structure
```
https://api.example.com/api/v1/{controller}/{action}
```

#### 2. Authentication Endpoints

```csharp
// POST /api/v1/auth/login
[HttpPost("login")]
public async Task<ActionResult<LoginResponse>> Login([FromBody] LoginRequest request)
{
    // Return LoginResponse with accessToken, refreshToken, user
}

// POST /api/v1/auth/register
[HttpPost("register")]
public async Task<ActionResult<LoginResponse>> Register([FromBody] RegisterRequest request)
{
    // Return LoginResponse
}

// POST /api/v1/auth/refresh-token
[HttpPost("refresh-token")]
public async Task<ActionResult<RefreshTokenResponse>> RefreshToken([FromBody] RefreshTokenRequest request)
{
    // Return new tokens
}

// GET /api/v1/auth/me
[HttpGet("me")]
[Authorize]
public async Task<ActionResult<UserDto>> GetCurrentUser()
{
    // Return current user info
}
```

#### 3. Products Endpoints

```csharp
// GET /api/v1/products?pageNumber=1&pageSize=12&categoryId=xxx&search=xxx
[HttpGet]
public async Task<ActionResult<PagedResponse<ProductDto>>> GetProducts(
    [FromQuery] ProductFilterParams params)
{
    // Return paged products
}

// GET /api/v1/products/{id}
[HttpGet("{id}")]
public async Task<ActionResult<ProductDto>> GetProduct(string id)
{
    // Return product detail
}

// POST /api/v1/products
[HttpPost]
[Authorize(Roles = "Admin")]
public async Task<ActionResult<ProductDto>> CreateProduct([FromBody] CreateProductRequest request)
{
    // Create and return product
}
```

#### 4. Orders Endpoints

```csharp
// GET /api/v1/orders
[HttpGet]
[Authorize]
public async Task<ActionResult<PagedResponse<OrderDto>>> GetOrders(
    [FromQuery] OrderFilterParams params)
{
    // Return paged orders
}

// POST /api/v1/orders
[HttpPost]
[Authorize]
public async Task<ActionResult<OrderDto>> CreateOrder([FromBody] CreateOrderRequest request)
{
    // Create and return order
}

// GET /api/v1/orders/track/{trackingNumber}
[HttpGet("track/{trackingNumber}")]
public async Task<ActionResult<OrderTrackingResponse>> TrackOrder(string trackingNumber)
{
    // Return order tracking info
}
```

### Error Response Format (ProblemDetails)

#### Validation Error (400)
```json
{
  "type": "https://tools.ietf.org/html/rfc7231#section-6.5.1",
  "title": "One or more validation errors occurred.",
  "status": 400,
  "errors": {
    "Email": ["Email is required", "Email is not valid"],
    "Password": ["Password must be at least 6 characters"]
  }
}
```

#### Unauthorized (401)
```json
{
  "type": "https://tools.ietf.org/html/rfc7235#section-3.1",
  "title": "Unauthorized",
  "status": 401,
  "detail": "Invalid credentials"
}
```

#### Not Found (404)
```json
{
  "type": "https://tools.ietf.org/html/rfc7231#section-6.5.4",
  "title": "Not Found",
  "status": 404,
  "detail": "Product with id 'xxx' not found"
}
```

#### Server Error (500)
```json
{
  "type": "https://tools.ietf.org/html/rfc7231#section-6.6.1",
  "title": "An error occurred while processing your request.",
  "status": 500,
  "detail": "Internal server error"
}
```

## 📊 Response Examples

### Success Response với Data

```json
{
  "success": true,
  "data": {
    "id": "product-123",
    "name": "Hương Trầm Cao Cấp",
    "price": 150000
  },
  "message": "Success"
}
```

### Paged Response

```json
{
  "items": [...],
  "pageNumber": 1,
  "pageSize": 12,
  "totalPages": 5,
  "totalCount": 60,
  "hasPreviousPage": false,
  "hasNextPage": true
}
```

## 🔒 JWT Token Format

### Access Token Header
```
Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...
```

### Token Payload Example
```json
{
  "sub": "user-123",
  "email": "user@example.com",
  "role": "Customer",
  "exp": 1672531200
}
```

## 🧪 Testing với Mock Data

Khi `REACT_APP_USE_MOCK=true`, toàn bộ API calls sẽ sử dụng mock data trong services:

- **authService.ts**: Mock login/register responses
- **productService.ts**: Mock 50 products với categories
- **orderService.ts**: Mock 20 orders với tracking

**Lợi ích:**
- Develop frontend độc lập không cần backend
- Test UI/UX flows
- Demo cho stakeholders

## 📝 TypeScript Types

Tất cả types được định nghĩa trong `/lib/api/types.ts` tương ứng với C# models:

- `UserDto` ↔ C# `UserDto`
- `ProductDto` ↔ C# `ProductDto`
- `OrderDto` ↔ C# `OrderDto`
- `LoginRequest` ↔ C# `LoginRequest`
- `PagedResponse<T>` ↔ C# `PagedResponse<T>`

**Quan trọng:** Phải đồng bộ types giữa frontend và backend!

## 🐛 Error Handling

### Tự động Toast Errors

```typescript
const { login } = useAuth();

// Error tự động hiển thị toast
await login({ email: 'invalid', password: 'wrong' });
// → Toast: "Email hoặc mật khẩu không đúng"
```

### Custom Error Handling

```typescript
import { ApiError } from './lib/api/errors';

try {
  await productService.getProductById('invalid-id');
} catch (err) {
  if (err instanceof ApiError) {
    if (err.isNotFound()) {
      console.log('Product not found');
    } else if (err.isValidationError()) {
      console.log('Validation errors:', err.errors);
    }
  }
}
```

## 📤 File Upload

```typescript
import { productService } from './lib/services/productService';

const handleUpload = async (file: File) => {
  try {
    const response = await productService.uploadProductImage('product-123', file);
    console.log('Uploaded:', response.url);
  } catch (err) {
    console.error('Upload failed:', err);
  }
};
```

## 🔄 Auto Refresh & Caching

Custom hooks tự động:
- Refresh data khi params thay đổi
- Cache data trong memory
- Retry failed requests
- Debounce search inputs

## 🎯 Best Practices

1. **Luôn sử dụng Custom Hooks** thay vì gọi services trực tiếp
2. **Kiểm tra `isLoading`** trước khi hiển thị data
3. **Handle errors gracefully** - đã có auto toast nhưng có thể custom thêm
4. **Sync TypeScript types** với backend models
5. **Use environment variables** cho API URLs
6. **Test với mock data** trước khi integrate backend
7. **Implement proper loading states** để UX tốt hơn
8. **Add retry logic** cho critical operations

## 🚦 Deployment Checklist

- [ ] Update `REACT_APP_API_URL` trong production `.env`
- [ ] Set `REACT_APP_USE_MOCK=false`
- [ ] Verify CORS configuration trên backend
- [ ] Test authentication flow end-to-end
- [ ] Verify all error responses match ProblemDetails format
- [ ] Test file uploads với real backend
- [ ] Monitor JWT token expiration và refresh flow
- [ ] Setup proper logging cho production

## 📞 Support

Nếu gặp vấn đề khi tích hợp:

1. Kiểm tra console logs (development mode có logging chi tiết)
2. Verify API URL trong `.env`
3. Check network tab trong DevTools
4. Xác nhận backend response format match với docs
5. Test với mock data để isolate vấn đề

---

**Tác giả:** VietCeremony E-commerce Platform  
**Version:** 1.0.0  
**Last Updated:** October 2025
