# VietCommerce API Integration Guide

## 📋 Tổng quan

Document này hướng dẫn cách tích hợp Frontend React với VietCommerce .NET 8 Web API backend.

## 🏗️ Kiến trúc

```
┌─────────────────────────────────────────┐
│  Frontend (React + Vite + TypeScript)   │
│  - ProductsPageV2 component             │
│  - vietCommerceProductService           │
├─────────────────────────────────────────┤
│  API Client Layer                        │
│  - axios instance với JWT interceptor   │
│  - Error handling & retry logic         │
│  - Token refresh mechanism              │
└─────────────────────────────────────────┘
              ↕ HTTP/HTTPS
┌─────────────────────────────────────────┐
│  VietCommerce .NET 8 Web API            │
│  Base URL: /api/v1                      │
│  - ProductController                    │
│  - Auth với JWT Bearer Token            │
│  - ApiResponse<T> wrapper               │
└─────────────────────────────────────────┘
```

## 🔧 Cài đặt

### 1. Copy file .env

```bash
cp .env.example .env
```

### 2. Cấu hình .env

```env
# API URL của backend
VITE_API_URL=https://localhost:7131/api/v1

# Mock mode (development)
VITE_USE_MOCK_DATA=false
```

### 3. Install dependencies

```bash
npm install
```

## 📡 API Endpoints

### Product Module

Base route: `/api/v1/product`

| Method | Endpoint | Auth | Mô tả | Response |
|--------|----------|------|-------|----------|
| GET | `/product` | ❌ | Danh sách sản phẩm (paginated) | `ApiResponse<PaginatedResult<ProductListDto>>` |
| GET | `/product/{id}` | ❌ | Chi tiết sản phẩm | `ApiResponse<ProductDetailDto>` |
| GET | `/product/slug/{slug}` | ❌ | Lấy theo slug | `ApiResponse<ProductDetailDto>` |
| GET | `/product/category/{categoryId}` | ❌ | Lọc theo danh mục | `ApiResponse<ProductListDto[]>` |
| GET | `/product/store/{storeId}` | ❌ | Lọc theo cửa hàng | `ApiResponse<ProductListDto[]>` |
| POST | `/product` | ✅ | Tạo sản phẩm | `ApiResponse<ProductDetailDto>` |
| PUT | `/product/{id}` | ✅ | Cập nhật sản phẩm | `ApiResponse<ProductDetailDto>` |
| DELETE | `/product/{id}` | ✅ | Xóa mềm | `ApiResponse<bool>` |
| PATCH | `/product/{id}/stock` | ✅ | Cập nhật tồn kho | `ApiResponse<bool>` |
| GET | `/product/{id}/stock` | ❌ | Lấy tồn kho | `ApiResponse<int>` |
| PATCH | `/product/{id}/active` | ✅ | Kích hoạt/tắt | `ApiResponse<bool>` |
| PATCH | `/product/{id}/featured` | ✅ | Đặt nổi bật | `ApiResponse<bool>` |
| POST | `/product/{id}/view` | ❌ | Tăng lượt xem | `ApiResponse<bool>` |
| POST | `/product/{id}/favorite` | ✅ | Toggle yêu thích | `ApiResponse<bool>` |

## 📝 DTOs

### ProductFilterDto (Query params)

```typescript
interface ProductFilterDto {
  page?: number;          // Default: 1
  pageSize?: number;      // Default: 12
  searchTerm?: string;    // Tìm kiếm theo tên
  categoryId?: string;    // Lọc theo danh mục
  storeId?: string;       // Lọc theo cửa hàng
  isActive?: boolean;     // Lọc sản phẩm active
  minPrice?: number;      // Giá tối thiểu
  maxPrice?: number;      // Giá tối đa
  sortBy?: string;        // 'name' | 'price' | 'createdAt'
  isDescending?: boolean; // true = DESC, false = ASC
}
```

### ProductListDto (Response)

```typescript
interface ProductListDto {
  id: string;
  name: string;
  slug: string;
  price: number;
  stock: number;
  thumbnailUrl?: string;
  categoryName?: string;
  storeName?: string;
}
```

### ProductDetailDto (Response)

```typescript
interface ProductDetailDto {
  id: string;
  name: string;
  slug: string;
  code: string;
  categoryId: string;
  storeId: string;
  sku: string;
  price: number;
  stock: number;
  isActive: boolean;
  description?: string;
  thumbnailUrl?: string;
  images?: string[];
  createdAt: string;
  updatedAt: string;
}
```

### PaginatedResult<T> (Response wrapper)

```typescript
interface PaginatedResult<T> {
  items: T[];
  totalCount: number;
  page: number;
  pageSize: number;
  totalPages: number;
  hasPreviousPage: boolean;
  hasNextPage: boolean;
}
```

### ApiResponse<T> (Standard response)

```typescript
interface ApiResponse<T> {
  success: boolean;
  data: T;
  message?: string;
  errors?: ValidationError[];
}
```

## 🔐 Authentication

### JWT Bearer Token

API sử dụng JWT Bearer token cho authentication:

```typescript
// Token được tự động attach vào headers bởi axios interceptor
Authorization: Bearer <access_token>
```

### Token Storage

Tokens được lưu trong localStorage:

```typescript
// Access Token
localStorage.getItem('access_token')

// Refresh Token
localStorage.getItem('refresh_token')
```

### Auto Token Refresh

Axios interceptor tự động refresh token khi nhận 401 Unauthorized:

1. Request gặp 401
2. Interceptor gọi `/auth/refresh-token`
3. Lưu token mới
4. Retry request ban đầu

## 🎯 Sử dụng Service

### Import Service

```typescript
import { vietCommerceProductService } from '../lib/services/vietCommerceProductService';
```

### Fetch Products với Filter & Pagination

```typescript
const fetchProducts = async () => {
  const filter: ProductFilterDto = {
    page: currentPage,
    pageSize: 12,
    searchTerm: searchQuery || undefined,
    categoryId: selectedCategory !== 'all' ? selectedCategory : undefined,
    sortBy: 'price',
    isDescending: true,
  };

  const result = await vietCommerceProductService.getProducts(filter);
  
  setProducts(result.items);
  setTotalCount(result.totalCount);
  setTotalPages(result.totalPages);
};
```

### Get Product Detail

```typescript
const product = await vietCommerceProductService.getProductById(productId);
```

### Create Product (Admin)

```typescript
const newProduct = await vietCommerceProductService.createProduct({
  name: 'Hương Trầm Cao Cấp',
  code: 'HUONG-001',
  categoryId: 'cat-1',
  sku: 'SKU-001',
  price: 450000,
  stockQuantity: 100,
  description: 'Hương trầm thiên nhiên',
  isActive: true,
});
```

## 🚨 Error Handling

### API Error Format

Backend trả về errors theo format ASP.NET Core ProblemDetails:

```typescript
interface ProblemDetails {
  type: string;
  title: string;
  status: number;
  detail?: string;
  instance?: string;
  errors?: Record<string, string[]>;
}
```

### Frontend Error Handling

```typescript
try {
  const result = await vietCommerceProductService.getProducts(filter);
  // Handle success
} catch (error: any) {
  console.error('Error:', error);
  
  // Display error message
  if (error.status === 404) {
    setError('Không tìm thấy sản phẩm');
  } else if (error.status === 401) {
    setError('Vui lòng đăng nhập');
    navigate('/login');
  } else {
    setError(error.message || 'Đã có lỗi xảy ra');
  }
}
```

## 🔄 Mock Mode

Để phát triển offline, set:

```env
VITE_USE_MOCK_DATA=true
```

Service sẽ tự động sử dụng mock data thay vì gọi API thật.

## 📦 Component Example: ProductsPageV2

ProductsPageV2 là component mẫu tích hợp đầy đủ với VietCommerce API:

```typescript
import { ProductsPageV2 } from './components/ProductsPageV2';

// Sử dụng
<ProductsPageV2 onNavigate={(page) => navigate(page)} />
```

Features:
- ✅ Fetch products từ API với filter/pagination
- ✅ Search products
- ✅ Sort products (name, price, newest)
- ✅ Category filter
- ✅ Grid/List view
- ✅ Loading skeleton
- ✅ Error handling
- ✅ Wishlist integration
- ✅ useHybridNavigate pattern

## 🧪 Testing

### Test với Mock Data

```bash
# Set mock mode
VITE_USE_MOCK_DATA=true npm run dev
```

### Test với API thật

```bash
# Đảm bảo backend đang chạy tại localhost:7131
# Set real API mode
VITE_USE_MOCK_DATA=false npm run dev
```

### Test Authentication

```typescript
// Login trước khi gọi protected endpoints
await authService.login({
  email: 'admin@example.com',
  password: 'password123'
});

// Sau đó có thể gọi các protected endpoints
await vietCommerceProductService.createProduct(productData);
```

## 📊 Performance Tips

### 1. Debounce Search

```typescript
const debouncedSearch = useDebounce(searchQuery, 500);

useEffect(() => {
  fetchProducts();
}, [debouncedSearch]);
```

### 2. Cache Results

```typescript
const [cache, setCache] = useState<Map<string, ProductListDto[]>>(new Map());

const fetchProducts = async () => {
  const cacheKey = JSON.stringify(filter);
  
  if (cache.has(cacheKey)) {
    setProducts(cache.get(cacheKey)!);
    return;
  }
  
  const result = await service.getProducts(filter);
  cache.set(cacheKey, result.items);
  setProducts(result.items);
};
```

### 3. Lazy Load Images

```typescript
<ImageWithFallback
  src={product.thumbnailUrl}
  alt={product.name}
  loading="lazy"
/>
```

## 🐛 Troubleshooting

### CORS Error

Nếu gặp CORS error, thêm vào backend startup:

```csharp
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend",
        builder => builder
            .WithOrigins("http://localhost:5173")
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowCredentials());
});
```

### 401 Unauthorized

1. Check token còn hạn không
2. Verify token format: `Bearer <token>`
3. Check quyền của endpoint (product.create, etc.)

### Network Error

1. Verify backend đang chạy
2. Check `VITE_API_URL` trong .env
3. Test endpoint trực tiếp với Postman

## 📚 Related Files

- `/lib/api/client.ts` - Axios configuration
- `/lib/api/types.ts` - TypeScript DTOs
- `/lib/api/errors.ts` - Error handling
- `/lib/services/vietCommerceProductService.ts` - Product service
- `/components/ProductsPageV2.tsx` - Example component

## 🎉 Kết luận

Integration đã sẵn sàng! Bạn có thể:

1. ✅ Fetch products từ API với pagination
2. ✅ Filter và search products
3. ✅ Manage authentication với JWT
4. ✅ Handle errors gracefully
5. ✅ Switch giữa mock/real API dễ dàng

Tham khảo `ProductsPageV2` component để xem implementation đầy đủ.
