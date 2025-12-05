# Template: Tích Hợp API với Orval cho Dự Án Mới

## 📋 Checklist Setup

- [ ] Cài đặt dependencies
- [ ] Tạo cấu trúc thư mục
- [ ] Cấu hình Orval
- [ ] Setup Axios client với interceptors
- [ ] Tạo script generate
- [ ] Test API calls
- [ ] Setup React Query (optional)

---

## 🚀 Bước 1: Cài Đặt Dependencies

```bash
# Core dependencies
npm install axios @tanstack/react-query

# Dev dependencies
npm install -D orval openapi-typescript-codegen

# TypeScript types
npm install -D @types/node
```

### `package.json` scripts

```json
{
  "scripts": {
    "dev": "vite",
    "build": "vite build",
    "api:generate": "node ./api/scripts/generate.ts",
    "api:watch": "orval --config orval.config.js --watch"
  }
}
```

---

## 🗂️ Bước 2: Tạo Cấu Trúc Thư Mục

```bash
mkdir -p api/scripts
mkdir -p api/generated-orval/schemas
mkdir -p src/lib/api
mkdir -p src/lib/services
mkdir -p src/lib/hooks
```

Cấu trúc cuối cùng:

```
project-root/
├── swagger.json                    # OpenAPI spec từ backend
├── orval.config.js                 # Orval config
├── .env                            # Environment variables
│
├── api/
│   ├── scripts/
│   │   └── generate.ts             # Generate script
│   └── generated-orval/
│       ├── index.ts                # Generated API functions
│       └── schemas/                # Generated TypeScript types
│
└── src/
    ├── lib/
    │   ├── api/
    │   │   ├── client.ts           # Axios instance + interceptors
    │   │   ├── orval-client.ts     # Custom mutator
    │   │   ├── types.ts            # Shared types
    │   │   └── errors.ts           # Error handling
    │   ├── services/               # Service wrappers
    │   └── hooks/                  # React Query hooks
    └── ...
```

---

## ⚙️ Bước 3: Cấu Hình Files

### 3.1. Environment Variables (`.env`)

```env
# API Configuration
VITE_API_URL=http://localhost:7001

# Optional: Mock mode for development
VITE_USE_MOCK_DATA=false
```

### 3.2. Orval Config (`orval.config.js`)

```javascript
module.exports = {
  myApi: {
    input: './swagger.json',                    // OpenAPI spec file
    output: {
      target: './api/generated-orval/index.ts', // Output file
      schemas: './api/generated-orval/schemas', // DTO types folder
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

### 3.3. Generate Script (`api/scripts/generate.ts`)

```typescript
import { generate } from 'openapi-typescript-codegen';
import { execSync } from 'child_process';
import path from 'path';

const swaggerPath = path.resolve('./swagger.json');

console.log('🚀 Generating API clients...\n');

// 1) Generate Orval client
console.log('📦 Running Orval...');
try {
  execSync('npx orval --config orval.config.js', { stdio: 'inherit' });
  console.log('✅ Orval generation completed\n');
} catch (error) {
  console.error('❌ Orval generation failed:', error);
  process.exit(1);
}

// 2) Optional: Generate openapi-typescript-codegen client
console.log('📦 Running OpenAPI TypeScript Codegen...');
try {
  generate({
    input: swaggerPath,
    output: './src/api/generated-client',
    clientName: 'ApiClient',
    useUnionTypes: true,
  });
  console.log('✅ OpenAPI TypeScript Codegen completed\n');
} catch (error) {
  console.error('❌ OpenAPI TypeScript Codegen failed:', error);
  process.exit(1);
}

console.log('🎉 All API clients generated successfully!');
```

### 3.4. TypeScript Types (`src/lib/api/types.ts`)

```typescript
/**
 * Shared API Types
 */

// Generic API Response wrapper
export interface ApiResponse<T = any> {
  success: boolean;
  data?: T;
  message?: string;
  errors?: string[];
}

// Pagination
export interface PaginatedResult<T> {
  items: T[];
  totalCount: number;
  pageNumber: number;
  pageSize: number;
  totalPages: number;
  hasNextPage: boolean;
  hasPreviousPage: boolean;
}

// Auth types
export interface LoginRequest {
  email: string;
  password: string;
  rememberMe?: boolean;
}

export interface LoginResponse {
  accessToken: string;
  refreshToken: string;
  expiresIn: number;
  user: UserDto;
}

export interface RefreshTokenRequest {
  refreshToken: string;
}

export interface RefreshTokenResponse {
  accessToken: string;
  refreshToken: string;
  expiresIn: number;
}

export interface UserDto {
  id: string;
  email: string;
  fullName: string;
  phoneNumber?: string;
  avatar?: string;
  role: string;
  isEmailConfirmed: boolean;
  createdAt: string;
  updatedAt: string;
}

// Error types
export interface ApiError {
  status: number;
  message: string;
  errors?: Record<string, string[]>;
  timestamp: string;
}
```

### 3.5. Error Handling (`src/lib/api/errors.ts`)

```typescript
/**
 * API Error Handling
 */

export interface ApiError {
  status: number;
  message: string;
  errors?: Record<string, string[]>;
  timestamp: string;
}

export const ErrorMessages = {
  NETWORK_ERROR: 'Không thể kết nối đến server. Vui lòng kiểm tra kết nối mạng.',
  UNAUTHORIZED: 'Phiên đăng nhập đã hết hạn. Vui lòng đăng nhập lại.',
  FORBIDDEN: 'Bạn không có quyền truy cập tài nguyên này.',
  NOT_FOUND: 'Không tìm thấy tài nguyên.',
  SERVER_ERROR: 'Lỗi server. Vui lòng thử lại sau.',
  VALIDATION_ERROR: 'Dữ liệu không hợp lệ.',
  UNKNOWN_ERROR: 'Đã xảy ra lỗi không xác định.',
};

export function createApiError(status: number, data: any): ApiError {
  const error: ApiError = {
    status,
    message: getErrorMessage(status, data),
    timestamp: new Date().toISOString(),
  };

  // Extract validation errors if present
  if (data?.errors) {
    error.errors = data.errors;
  }

  return error;
}

export function getErrorMessage(status: number, data: any): string {
  // Try to get message from response
  if (data?.message) {
    return data.message;
  }

  // Default messages based on status code
  switch (status) {
    case 400:
      return ErrorMessages.VALIDATION_ERROR;
    case 401:
      return ErrorMessages.UNAUTHORIZED;
    case 403:
      return ErrorMessages.FORBIDDEN;
    case 404:
      return ErrorMessages.NOT_FOUND;
    case 500:
    case 502:
    case 503:
      return ErrorMessages.SERVER_ERROR;
    default:
      return ErrorMessages.UNKNOWN_ERROR;
  }
}

export function isApiError(error: any): error is ApiError {
  return error && typeof error.status === 'number' && typeof error.message === 'string';
}
```

### 3.6. Token Storage & Axios Client (`src/lib/api/client.ts`)

```typescript
/**
 * API Client Layer - Axios instance với interceptors, JWT, logging
 */

import axios, { type AxiosInstance, type AxiosRequestConfig, type AxiosResponse, type AxiosError } from 'axios';
import { createApiError, ErrorMessages } from './errors';
import type { RefreshTokenRequest, RefreshTokenResponse } from './types';

// API Base URL
const API_BASE_URL = import.meta.env.VITE_API_URL || 'http://localhost:7001';

// Token storage keys
const ACCESS_TOKEN_KEY = 'accessToken';
const REFRESH_TOKEN_KEY = 'refreshToken';
const REMEMBER_ME_KEY = 'remember_me';

/**
 * Token Storage Utilities
 */
export const tokenStorage = {
  isRememberMe: (): boolean => {
    return localStorage.getItem(REMEMBER_ME_KEY) === 'true';
  },

  setRememberMe: (remember: boolean): void => {
    if (remember) {
      localStorage.setItem(REMEMBER_ME_KEY, 'true');
    } else {
      localStorage.removeItem(REMEMBER_ME_KEY);
    }
  },

  getStorage: (): Storage => {
    return tokenStorage.isRememberMe() ? localStorage : sessionStorage;
  },

  getAccessToken: (): string | null => {
    return localStorage.getItem(ACCESS_TOKEN_KEY) || 
           sessionStorage.getItem(ACCESS_TOKEN_KEY);
  },

  setAccessToken: (token: string): void => {
    const storage = tokenStorage.getStorage();
    storage.setItem(ACCESS_TOKEN_KEY, token);
  },

  getRefreshToken: (): string | null => {
    return localStorage.getItem(REFRESH_TOKEN_KEY) || 
           sessionStorage.getItem(REFRESH_TOKEN_KEY);
  },

  setRefreshToken: (token: string): void => {
    const storage = tokenStorage.getStorage();
    storage.setItem(REFRESH_TOKEN_KEY, token);
  },

  clearTokens: (): void => {
    localStorage.removeItem(ACCESS_TOKEN_KEY);
    localStorage.removeItem(REFRESH_TOKEN_KEY);
    localStorage.removeItem(REMEMBER_ME_KEY);
    sessionStorage.removeItem(ACCESS_TOKEN_KEY);
    sessionStorage.removeItem(REFRESH_TOKEN_KEY);
  },

  setTokens: (accessToken: string, refreshToken: string, rememberMe?: boolean): void => {
    if (rememberMe !== undefined) {
      tokenStorage.setRememberMe(rememberMe);
    }
    tokenStorage.setAccessToken(accessToken);
    tokenStorage.setRefreshToken(refreshToken);
  }
};

/**
 * Create Axios Instance
 */
const createAxiosInstance = (): AxiosInstance => {
  const instance = axios.create({
    baseURL: API_BASE_URL,
    timeout: 30000,
    headers: {
      'Content-Type': 'application/json',
      'Accept': 'application/json',
    },
  });

  return instance;
};

/**
 * Main API Client Instance
 */
export const apiClient = createAxiosInstance();

/**
 * Refresh Token State
 */
let isRefreshing = false;
let failedQueue: Array<{
  resolve: (value?: any) => void;
  reject: (reason?: any) => void;
}> = [];

const processQueue = (error: any, token: string | null = null) => {
  failedQueue.forEach(promise => {
    if (error) {
      promise.reject(error);
    } else {
      promise.resolve(token);
    }
  });
  failedQueue = [];
};

/**
 * Request Interceptor - Auto Attach JWT Token
 */
apiClient.interceptors.request.use(
  (config) => {
    const token = tokenStorage.getAccessToken();
    if (token) {
      config.headers.Authorization = `Bearer ${token}`;
    }

    // Dev logging
    if (import.meta.env.DEV) {
      console.log(`[API] ${config.method?.toUpperCase()} ${config.url}`);
    }

    return config;
  },
  (error) => {
    return Promise.reject(error);
  }
);

/**
 * Response Interceptor - Handle Errors & Auto Refresh Token
 */
apiClient.interceptors.response.use(
  (response: AxiosResponse) => {
    // Dev logging
    if (import.meta.env.DEV) {
      console.log(`[API] Response:`, response.data);
    }
    return response;
  },
  async (error: AxiosError) => {
    const originalRequest = error.config as AxiosRequestConfig & { _retry?: boolean };

    // Network error
    if (!error.response) {
      return Promise.reject(createApiError(0, { message: ErrorMessages.NETWORK_ERROR }));
    }

    const status = error.response.status;

    // Auto refresh token on 401
    if (status === 401 && !originalRequest._retry) {
      if (isRefreshing) {
        return new Promise((resolve, reject) => {
          failedQueue.push({ resolve, reject });
        })
          .then(token => {
            if (originalRequest.headers) {
              originalRequest.headers.Authorization = `Bearer ${token}`;
            }
            return apiClient(originalRequest);
          })
          .catch(err => Promise.reject(err));
      }

      originalRequest._retry = true;
      isRefreshing = true;

      const refreshToken = tokenStorage.getRefreshToken();

      if (!refreshToken) {
        tokenStorage.clearTokens();
        window.location.href = '/login';
        return Promise.reject(createApiError(401, { message: ErrorMessages.UNAUTHORIZED }));
      }

      try {
        const response = await axios.post<RefreshTokenResponse>(
          `${API_BASE_URL}/auth/refresh-token`,
          { refreshToken } as RefreshTokenRequest,
          {
            headers: { 'Content-Type': 'application/json' },
          }
        );

        const { accessToken, refreshToken: newRefreshToken } = response.data;
        tokenStorage.setTokens(accessToken, newRefreshToken);
        processQueue(null, accessToken);

        if (originalRequest.headers) {
          originalRequest.headers.Authorization = `Bearer ${accessToken}`;
        }
        return apiClient(originalRequest);
      } catch (refreshError) {
        processQueue(refreshError, null);
        tokenStorage.clearTokens();
        window.location.href = '/login';
        return Promise.reject(createApiError(401, { message: ErrorMessages.UNAUTHORIZED }));
      } finally {
        isRefreshing = false;
      }
    }

    const apiError = createApiError(status, error.response.data);
    return Promise.reject(apiError);
  }
);

/**
 * Generic API Request Wrapper
 */
export const apiRequest = {
  get: <T>(url: string, config?: AxiosRequestConfig): Promise<T> => {
    return apiClient.get<T>(url, config).then(response => response.data);
  },

  post: <T>(url: string, data?: any, config?: AxiosRequestConfig): Promise<T> => {
    return apiClient.post<T>(url, data, config).then(response => response.data);
  },

  put: <T>(url: string, data?: any, config?: AxiosRequestConfig): Promise<T> => {
    return apiClient.put<T>(url, data, config).then(response => response.data);
  },

  patch: <T>(url: string, data?: any, config?: AxiosRequestConfig): Promise<T> => {
    return apiClient.patch<T>(url, data, config).then(response => response.data);
  },

  delete: <T>(url: string, config?: AxiosRequestConfig): Promise<T> => {
    return apiClient.delete<T>(url, config).then(response => response.data);
  },
};

export default apiClient;
```

### 3.7. Orval Custom Mutator (`src/lib/api/orval-client.ts`)

```typescript
/**
 * Orval Custom Mutator
 * Axios instance được Orval sử dụng để gọi API
 */

import axios, { type AxiosRequestConfig } from 'axios';
import { tokenStorage } from './client';

const axiosInstance = axios.create({
  baseURL: import.meta.env.VITE_API_URL || 'http://localhost:5000/api/v1',
  timeout: 30000,
  headers: {
    'Content-Type': 'application/json',
    'Accept': 'application/json',
  },
  withCredentials: true,
});

// Auto attach JWT token
axiosInstance.interceptors.request.use((config) => {
  const token = tokenStorage.getAccessToken();
  if (token) {
    config.headers.Authorization = `Bearer ${token}`;
  }
  return config;
});

// Export function for Orval
export function apiClient<T>(config: AxiosRequestConfig): Promise<T> {
  return axiosInstance.request<T>(config).then((response) => response.data as T);
}
```

---

## 🔧 Bước 4: Generate API Client

### 4.1. Lấy Swagger/OpenAPI Spec từ Backend

**Option 1: Download từ backend endpoint**
```bash
curl https://localhost:7001/swagger/v1/swagger.json -o swagger.json
```

**Option 2: Copy từ backend project**
```bash

```

**Option 3: Backend tự động export**
```csharp
// ASP.NET Core - Program.cs
app.UseSwagger();
app.UseSwaggerUI();

// Export swagger.json on startup
var swaggerJson = app.Services.GetRequiredService<ISwaggerProvider>()
    .GetSwagger("v1")
    .SerializeAsJson(OpenApiSpecVersion.OpenApi3_0);
File.WriteAllText("swagger.json", swaggerJson);
```

### 4.2. Chạy Generate Script

```bash
# Generate API client
npm run api:generate

# Hoặc watch mode (auto-regenerate khi swagger.json thay đổi)
npm run api:watch
```

### 4.3. Verify Generated Files

```bash
# Check generated files
ls -la api/generated-orval/
ls -la api/generated-orval/schemas/
```

---

## 💻 Bước 5: Sử Dụng API Client

### 5.1. Basic Usage

```typescript
import { getMyAPI } from '@/api/generated-orval';

const api = getMyAPI();

// GET request
const products = await api.getApiV1Products();

// POST request
const newProduct = await api.postApiV1Products({
  name: 'Product Name',
  price: 99.99,
});

// PUT request
const updated = await api.putApiV1ProductsId('product-id', {
  name: 'Updated Name',
});

// DELETE request
await api.deleteApiV1ProductsId('product-id');
```

### 5.2. Service Layer (Recommended)

```typescript
// src/lib/services/productService.ts
import { getMyAPI } from '@/api/generated-orval';
import type { ProductDto, CreateProductDto } from '@/api/generated-orval/schemas';

class ProductService {
  private api = getMyAPI();

  async getProducts() {
    try {
      const response = await this.api.getApiV1Products();
      return response.data;
    } catch (error) {
      console.error('Failed to fetch products:', error);
      throw error;
    }
  }

  async getProduct(id: string) {
    return this.api.getApiV1ProductsId(id);
  }

  async createProduct(dto: CreateProductDto) {
    return this.api.postApiV1Products(dto);
  }

  async updateProduct(id: string, dto: Partial<ProductDto>) {
    return this.api.putApiV1ProductsId(id, dto);
  }

  async deleteProduct(id: string) {
    return this.api.deleteApiV1ProductsId(id);
  }
}

export const productService = new ProductService();
```

### 5.3. React Query Hooks

```typescript
// src/lib/hooks/useProducts.ts
import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';
import { productService } from '@/lib/services/productService';
import type { CreateProductDto } from '@/api/generated-orval/schemas';

// Query: Get all products
export function useProducts() {
  return useQuery({
    queryKey: ['products'],
    queryFn: () => productService.getProducts(),
  });
}

// Query: Get single product
export function useProduct(id: string) {
  return useQuery({
    queryKey: ['product', id],
    queryFn: () => productService.getProduct(id),
    enabled: !!id,
  });
}

// Mutation: Create product
export function useCreateProduct() {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: (dto: CreateProductDto) => productService.createProduct(dto),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['products'] });
    },
  });
}

// Mutation: Update product
export function useUpdateProduct() {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: ({ id, dto }: { id: string; dto: Partial<ProductDto> }) =>
      productService.updateProduct(id, dto),
    onSuccess: (_, variables) => {
      queryClient.invalidateQueries({ queryKey: ['products'] });
      queryClient.invalidateQueries({ queryKey: ['product', variables.id] });
    },
  });
}

// Mutation: Delete product
export function useDeleteProduct() {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: (id: string) => productService.deleteProduct(id),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['products'] });
    },
  });
}
```

### 5.4. React Component Usage

```typescript
// src/pages/ProductsPage.tsx
import { useProducts, useCreateProduct } from '@/lib/hooks/useProducts';

function ProductsPage() {
  const { data: products, isLoading, error } = useProducts();
  const createProduct = useCreateProduct();

  const handleCreate = () => {
    createProduct.mutate({
      name: 'New Product',
      price: 99.99,
    });
  };

  if (isLoading) return <div>Loading...</div>;
  if (error) return <div>Error: {error.message}</div>;

  return (
    <div>
      <button onClick={handleCreate}>Create Product</button>
      <ul>
        {products?.map(product => (
          <li key={product.id}>{product.name}</li>
        ))}
      </ul>
    </div>
  );
}
```

---

## 🧪 Bước 6: Testing

### 6.1. Test API Client

```typescript
// src/lib/api/__tests__/client.test.ts
import { describe, it, expect, beforeEach } from 'vitest';
import { tokenStorage } from '../client';

describe('Token Storage', () => {
  beforeEach(() => {
    tokenStorage.clearTokens();
  });

  it('should store and retrieve access token', () => {
    const token = 'test-token';
    tokenStorage.setAccessToken(token);
    expect(tokenStorage.getAccessToken()).toBe(token);
  });

  it('should clear all tokens', () => {
    tokenStorage.setTokens('access', 'refresh');
    tokenStorage.clearTokens();
    expect(tokenStorage.getAccessToken()).toBeNull();
    expect(tokenStorage.getRefreshToken()).toBeNull();
  });
});
```

### 6.2. Test Service

```typescript
// src/lib/services/__tests__/productService.test.ts
import { describe, it, expect, vi } from 'vitest';
import { productService } from '../productService';

vi.mock('@/api/generated-orval', () => ({
  getMyAPI: () => ({
    getApiV1Products: vi.fn().mockResolvedValue({ data: [] }),
  }),
}));

describe('Product Service', () => {
  it('should fetch products', async () => {
    const products = await productService.getProducts();
    expect(Array.isArray(products)).toBe(true);
  });
});
```

---

## 📝 Bước 7: Documentation

### 7.1. Tạo README cho API

```markdown
# API Integration

## Generate API Client

\`\`\`bash
npm run api:generate
\`\`\`

## Usage

\`\`\`typescript
import { getMyAPI } from '@/api/generated-orval';

const api = getMyAPI();
const products = await api.getApiV1Products();
\`\`\`

## Services

- `productService` - Product CRUD operations
- `authService` - Authentication
- `cartService` - Shopping cart

## Hooks

- `useProducts()` - Fetch all products
- `useProduct(id)` - Fetch single product
- `useCreateProduct()` - Create product mutation
\`\`\`
```

---

## 🎯 Best Practices Summary

1. **Luôn regenerate sau khi backend thay đổi**
2. **Không edit generated code trực tiếp**
3. **Wrap API calls trong services**
4. **Sử dụng React Query cho caching**
5. **Handle errors properly**
6. **Add TypeScript types cho mọi thứ**
7. **Test API integration**
8. **Document API usage**

---

## 🔗 Next Steps

- [ ] Setup CI/CD để auto-generate khi backend deploy
- [ ] Add API mocking cho testing
- [ ] Setup error monitoring (Sentry)
- [ ] Add request/response logging
- [ ] Implement retry logic
- [ ] Add rate limiting handling

---

## 📚 Resources

- [Orval Documentation](https://orval.dev/)
- [React Query Documentation](https://tanstack.com/query/latest)
- [Axios Documentation](https://axios-http.com/)
