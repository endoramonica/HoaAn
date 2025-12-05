# Kiến Trúc Tích Hợp API - VietCommerce Project

## 📋 Tổng Quan

Dự án này sử dụng **Orval** và **OpenAPI TypeScript Codegen** để tự động sinh TypeScript client từ OpenAPI/Swagger specification. Cách tiếp cận này đảm bảo type-safety hoàn toàn và đồng bộ giữa Frontend và Backend.

---

## 🏗️ Kiến Trúc Tổng Thể

```
Backend (ASP.NET Core)
    ↓ (generates)
swagger.json (OpenAPI Spec)
    ↓ (input for)
Orval + OpenAPI Codegen
    ↓ (generates)
TypeScript API Client
    ↓ (used by)
React Components/Hooks
```

---

## 🔧 Công Cụ & Dependencies

### 1. **Orval** - Primary API Client Generator
```json
{
  "orval": "^7.17.0"
}
```
- Sinh TypeScript client từ OpenAPI spec
- Tích hợp với Axios
- Hỗ trợ custom mutator (interceptors)
- Type-safe response handling

### 2. **OpenAPI TypeScript Codegen** - Secondary Generator
```json
{
  "openapi-typescript-codegen": "^0.29.0"
}
```
- Sinh thêm một client với union types
- Dùng cho các trường hợp đặc biệt cần kiểu dữ liệu phức tạp

### 3. **Axios** - HTTP Client
```json
{
  "axios": "*"
}
```
- Base HTTP client
- Interceptors cho authentication
- Request/Response transformation

---

## 📁 Cấu Trúc Thư Mục

```
project-root/
├── swagger.json                    # OpenAPI specification từ backend
├── orval.config.js                 # Cấu hình Orval
├── .env                            # Environment variables
│
├── Api/
│   ├── doc.readMe                  # Documentation
│   ├── scripts/
│   │   └── generate.ts             # Script tự động sinh API client
│   └── generated-orval/
│       ├── index.ts                # ~90 API functions
│       └── schemas/                # ~100+ TypeScript types/DTOs
│
└── src/
    ├── lib/
    │   └── api/
    │       ├── client.ts           # Axios instance + interceptors
    │       ├── orval-client.ts     # Custom mutator cho Orval
    │       ├── types.ts            # Shared types
    │       └── errors.ts           # Error handling
    │
    └── api/
        └── services/               # Service wrappers (optional)
```

---

## ⚙️ Cấu Hình Chi Tiết

### 1. Environment Variables (`.env`)

```env
VITE_API_URL=https://localhost:7001/
```

### 2. Orval Configuration (`orval.config.js`)

```javascript
module.exports = {
  vietCommerce: {
    input: './swagger.json',                    // OpenAPI spec file
    output: {
      target: './Api/generated-orval/index.ts', // Output file
      schemas: './Api/generated-orval/schemas', // DTO types folder
      client: 'axios',                          // HTTP client
      baseUrl: true,                            // Include baseURL
      clean: true,                              // Clean before generate
      override: {
        mutator: {
          path: './src/lib/api/orval-client.ts', // Custom axios instance
          name: 'apiClient'
        },
        fetch: {
          withCredentials: true                  // Include cookies
        },
        response: true,                          // Generate response types
      },
    },
  },
};
```

**Giải thích các options quan trọng:**
- `mutator`: Custom axios instance với interceptors
- `withCredentials`: Gửi cookies/credentials trong requests
- `response: true`: Bắt buộc để Orval generate đúng response structure

### 3. Custom Mutator (`src/lib/api/orval-client.ts`)

```typescript
import axios, { type AxiosRequestConfig } from 'axios';
import { tokenStorage } from './client';

const axiosInstance = axios.create({
  baseURL: import.meta.env.VITE_API_URL || 'https://localhost:7001',
  timeout: 30000,
  headers: {
    'Content-Type': 'application/json',
    'Accept': 'application/json',
  },
  withCredentials: true,
});

// Tự động attach JWT token vào mọi request
axiosInstance.interceptors.request.use((config) => {
  const token = tokenStorage.getAccessToken();
  if (token) {
    config.headers.Authorization = `Bearer ${token}`;
  }
  return config;
});

// Export function cho Orval sử dụng
export function apiClient<T>(config: AxiosRequestConfig): Promise<T> {
  return axiosInstance.request<T>(config).then((response) => response.data as T);
}
```

**Vai trò:**
- Tự động inject JWT token vào headers
- Xử lý response transformation
- Centralized error handling

---

## 🔐 Authentication Flow

### 1. Token Storage (`src/lib/api/client.ts`)

```typescript
const ACCESS_TOKEN_KEY = 'authToken';
const REFRESH_TOKEN_KEY = 'refreshToken';
const REMEMBER_ME_KEY = 'remember_me';

export const tokenStorage = {
  // Lưu tokens (localStorage hoặc sessionStorage)
  setTokens: (accessToken: string, refreshToken: string, rememberMe?: boolean) => {
    const storage = rememberMe ? localStorage : sessionStorage;
    storage.setItem(ACCESS_TOKEN_KEY, accessToken);
    storage.setItem(REFRESH_TOKEN_KEY, refreshToken);
  },

  // Lấy access token
  getAccessToken: (): string | null => {
    return localStorage.getItem(ACCESS_TOKEN_KEY) || 
           sessionStorage.getItem(ACCESS_TOKEN_KEY);
  },

  // Xóa tất cả tokens
  clearTokens: (): void => {
    localStorage.removeItem(ACCESS_TOKEN_KEY);
    localStorage.removeItem(REFRESH_TOKEN_KEY);
    sessionStorage.removeItem(ACCESS_TOKEN_KEY);
    sessionStorage.removeItem(REFRESH_TOKEN_KEY);
  }
};
```

### 2. Request Interceptor - Auto Attach Token

```typescript
apiClient.interceptors.request.use((config) => {
  const token = tokenStorage.getAccessToken();
  if (token) {
    config.headers.Authorization = `Bearer ${token}`;
  }
  return config;
});
```

### 3. Response Interceptor - Auto Refresh Token

```typescript
apiClient.interceptors.response.use(
  (response) => response,
  async (error) => {
    const originalRequest = error.config;

    // Nếu 401 và chưa retry
    if (error.response?.status === 401 && !originalRequest._retry) {
      originalRequest._retry = true;

      const refreshToken = tokenStorage.getRefreshToken();
      if (!refreshToken) {
        // Redirect to login
        window.location.href = '/login';
        return Promise.reject(error);
      }

      try {
        // Gọi API refresh token
        const response = await axios.post('/auth/refresh-token', { refreshToken });
        const { accessToken, refreshToken: newRefreshToken } = response.data;

        // Lưu tokens mới
        tokenStorage.setTokens(accessToken, newRefreshToken);

        // Retry request ban đầu với token mới
        originalRequest.headers.Authorization = `Bearer ${accessToken}`;
        return apiClient(originalRequest);
      } catch (refreshError) {
        // Refresh failed -> logout
        tokenStorage.clearTokens();
        window.location.href = '/login';
        return Promise.reject(refreshError);
      }
    }

    return Promise.reject(error);
  }
);
```

---

## 🚀 Quy Trình Generate API Client

### Script Tự Động (`Api/scripts/generate.ts`)

```typescript
import { generate } from 'openapi-typescript-codegen';
import { execSync } from 'child_process';
import path from 'path';

const swaggerPath = path.resolve('./swagger.json');

// 1) Generate Orval client
console.log('🚀 Running ORVAL...');
execSync('npx orval --config orval.config.js', { stdio: 'inherit' });

// 2) Generate openapi-typescript-codegen client
console.log('🚀 Running openapi-typescript-codegen...');
generate({
  input: swaggerPath,
  output: './src/Api/generated-client',
  clientName: 'ApiClient',
  useUnionTypes: true,
});

console.log('🎉 All API clients generated successfully!');
```

### NPM Scripts (`package.json`)

```json
{
  "scripts": {
    "api:generate": "node ./Api/scripts/generate.ts",
    "api:watch": "orval --config orval.config.js --watch"
  }
}
```

### Cách Sử Dụng

```bash
# Generate một lần
npm run api:generate

# Watch mode - tự động regenerate khi swagger.json thay đổi
npm run api:watch
```

---

## 💻 Sử Dụng Generated API Client

### 1. Import API Functions

```typescript
import { getVietCommerceAPI } from '@/Api/generated-orval';

const api = getVietCommerceAPI();
```

### 2. Gọi API trong React Component

```typescript
import { useState } from 'react';
import { getVietCommerceAPI } from '@/Api/generated-orval';
import type { ProductDetailDtoApiResponse } from '@/Api/generated-orval/schemas';

function ProductDetail({ productId }: { productId: string }) {
  const [product, setProduct] = useState<ProductDetailDtoApiResponse | null>(null);
  const api = getVietCommerceAPI();

  useEffect(() => {
    const fetchProduct = async () => {
      try {
        const response = await api.getApiV1ProductId(productId);
        setProduct(response);
      } catch (error) {
        console.error('Failed to fetch product:', error);
      }
    };

    fetchProduct();
  }, [productId]);

  return <div>{product?.data?.name}</div>;
}
```

### 3. Sử Dụng với React Query

```typescript
import { useQuery } from '@tanstack/react-query';
import { getVietCommerceAPI } from '@/Api/generated-orval';

function useProduct(productId: string) {
  const api = getVietCommerceAPI();

  return useQuery({
    queryKey: ['product', productId],
    queryFn: () => api.getApiV1ProductId(productId),
    enabled: !!productId,
  });
}

// Sử dụng trong component
function ProductPage({ productId }: { productId: string }) {
  const { data, isLoading, error } = useProduct(productId);

  if (isLoading) return <div>Loading...</div>;
  if (error) return <div>Error: {error.message}</div>;

  return <div>{data?.data?.name}</div>;
}
```

### 4. Mutation với React Query

```typescript
import { useMutation, useQueryClient } from '@tanstack/react-query';
import { getVietCommerceAPI } from '@/Api/generated-orval';
import type { AddToCartDto } from '@/Api/generated-orval/schemas';

function useAddToCart() {
  const api = getVietCommerceAPI();
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: (dto: AddToCartDto) => api.postApiV1CartAdd(dto),
    onSuccess: () => {
      // Invalidate cart queries để refetch
      queryClient.invalidateQueries({ queryKey: ['cart'] });
    },
  });
}

// Sử dụng trong component
function AddToCartButton({ productId }: { productId: string }) {
  const addToCart = useAddToCart();

  const handleClick = () => {
    addToCart.mutate({
      productId,
      quantity: 1,
    });
  };

  return (
    <button onClick={handleClick} disabled={addToCart.isPending}>
      {addToCart.isPending ? 'Adding...' : 'Add to Cart'}
    </button>
  );
}
```

---

## 📝 Generated Code Structure

### API Functions (`Api/generated-orval/index.ts`)

Orval sinh ra ~90 functions cho tất cả endpoints:

```typescript
export const getVietCommerceAPI = () => {
  // Auth endpoints
  const postApiV1AuthRegister = (registerRequestDTO: RegisterRequestDTO) => {
    return apiClient<void>({
      url: `/Auth/register`,
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      data: registerRequestDTO
    });
  };

  const postApiV1AuthLogin = (loginDTO: LoginDTO) => {
    return apiClient<void>({
      url: `/Auth/login`,
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      data: loginDTO
    });
  };

  // Cart endpoints
  const getApiV1Cart = () => {
    return apiClient<void>({
      url: `/Cart`,
      method: 'GET'
    });
  };

  const postApiV1CartAdd = (addToCartDto: AddToCartDto) => {
    return apiClient<void>({
      url: `/Cart/add`,
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      data: addToCartDto
    });
  };

  // ... ~86 more functions

  return {
    postApiV1AuthRegister,
    postApiV1AuthLogin,
    getApiV1Cart,
    postApiV1CartAdd,
    // ... all functions
  };
};
```

### DTO Types (`Api/generated-orval/schemas/`)

Orval sinh ra ~100+ TypeScript interfaces:

```typescript
// AddToCartDto.ts
export interface AddToCartDto {
  productId: string;
  quantity: number;
  variantId?: string;
}

// ProductDetailDto.ts
export interface ProductDetailDto {
  id: string;
  name: string;
  description?: string;
  price: number;
  discountPrice?: number;
  stock: number;
  images: string[];
  categoryId: string;
  storeId: string;
  // ... more fields
}

// ApiResponse wrapper
export interface ProductDetailDtoApiResponse {
  success: boolean;
  data?: ProductDetailDto;
  message?: string;
  errors?: string[];
}
```

---

## 🔄 Workflow: Backend → Frontend Sync

### 1. Backend Developer Updates API

```csharp
// Backend: Add new endpoint
[HttpGet("featured")]
public async Task<ActionResult<ApiResponse<List<ProductDto>>>> GetFeaturedProducts()
{
    var products = await _productService.GetFeaturedProductsAsync();
    return Ok(ApiResponse<List<ProductDto>>.SuccessResponse(products));
}
```

### 2. Backend Generates Swagger

```bash
# Backend tự động generate swagger.json khi build
dotnet build
```

### 3. Frontend Developer Gets Updated Swagger

```bash
# Copy swagger.json từ backend sang frontend project
cp ../Backend/swagger.json ./swagger.json
```

### 4. Regenerate TypeScript Client

```bash
# Chạy script generate
npm run api:generate
```

### 5. TypeScript Compiler Báo Lỗi

```typescript
// TypeScript sẽ báo lỗi nếu code cũ không tương thích
const api = getVietCommerceAPI();

// ❌ Error: Property 'getApiV1ProductFeatured' does not exist
api.getApiV1ProductFeatured(); // Function mới chưa được sử dụng
```

### 6. Update Frontend Code

```typescript
// ✅ Sử dụng function mới
const { data } = await api.getApiV1ProductFeatured();
```

---

## 🎯 Best Practices

### 1. **Luôn Regenerate Sau Khi Backend Thay Đổi**

```bash
# Workflow chuẩn
git pull                    # Pull latest backend changes
npm run api:generate        # Regenerate client
npm run build              # Check for TypeScript errors
```

### 2. **Không Edit Generated Code**

```typescript
// ❌ KHÔNG làm thế này
// File: Api/generated-orval/index.ts
const postApiV1AuthLogin = (loginDTO: LoginDTO) => {
  // Custom logic here - SẼ BỊ MẤT khi regenerate!
};

// ✅ Làm thế này thay vào
// File: src/lib/services/authService.ts
export const login = async (loginDTO: LoginDTO) => {
  const api = getVietCommerceAPI();
  const response = await api.postApiV1AuthLogin(loginDTO);
  // Custom logic here - An toàn!
  return response;
};
```

### 3. **Wrap API Calls trong Services**

```typescript
// src/lib/services/productService.ts
import { getVietCommerceAPI } from '@/Api/generated-orval';
import type { ProductDetailDtoApiResponse } from '@/Api/generated-orval/schemas';

class ProductService {
  private api = getVietCommerceAPI();

  async getProduct(id: string): Promise<ProductDetailDtoApiResponse> {
    try {
      const response = await this.api.getApiV1ProductId(id);
      
      // Custom logic: logging, caching, transformation
      console.log('Product fetched:', response.data?.name);
      
      return response;
    } catch (error) {
      // Custom error handling
      console.error('Failed to fetch product:', error);
      throw error;
    }
  }

  async getFeaturedProducts() {
    return this.api.getApiV1ProductFeatured();
  }
}

export const productService = new ProductService();
```

### 4. **Sử Dụng React Query cho Caching**

```typescript
// src/lib/hooks/useProducts.ts
import { useQuery } from '@tanstack/react-query';
import { productService } from '@/lib/services/productService';

export function useProduct(id: string) {
  return useQuery({
    queryKey: ['product', id],
    queryFn: () => productService.getProduct(id),
    staleTime: 5 * 60 * 1000, // 5 minutes
    cacheTime: 10 * 60 * 1000, // 10 minutes
  });
}

export function useFeaturedProducts() {
  return useQuery({
    queryKey: ['products', 'featured'],
    queryFn: () => productService.getFeaturedProducts(),
    staleTime: 10 * 60 * 1000, // 10 minutes
  });
}
```

---

## 🐛 Troubleshooting

### Issue 1: "Cannot find module '@/Api/generated-orval'"

**Nguyên nhân:** Chưa chạy generate script

**Giải pháp:**
```bash
npm run api:generate
```

### Issue 2: "401 Unauthorized" trên mọi request

**Nguyên nhân:** Token không được attach vào headers

**Kiểm tra:**
```typescript
// Check token storage
console.log('Access Token:', tokenStorage.getAccessToken());

// Check interceptor
apiClient.interceptors.request.use((config) => {
  console.log('Request headers:', config.headers);
  return config;
});
```

### Issue 3: TypeScript errors sau khi regenerate

**Nguyên nhân:** Backend API đã thay đổi structure

**Giải pháp:**
1. Đọc error messages
2. Update code theo API mới
3. Hoặc rollback swagger.json nếu backend chưa stable

### Issue 4: CORS errors

**Nguyên nhân:** Backend chưa config CORS

**Giải pháp Backend:**
```csharp
// Program.cs
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins("http://localhost:5173")
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

app.UseCors("AllowFrontend");
```

---

## 📚 Tài Liệu Tham Khảo

- [Orval Documentation](https://orval.dev/)
- [OpenAPI Specification](https://swagger.io/specification/)
- [Axios Documentation](https://axios-http.com/)
- [React Query Documentation](https://tanstack.com/query/latest)

---

## 🎓 Áp Dụng Cho Dự Án Khác

Để áp dụng cách tiếp cận này cho dự án mới, xem file: `API_INTEGRATION_TEMPLATE.md`
