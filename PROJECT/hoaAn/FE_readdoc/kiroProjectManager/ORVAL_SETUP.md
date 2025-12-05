# Orval API Integration - Hướng dẫn đầy đủ

## 📋 Tổng quan

Dự án đã được cấu hình để tự động generate:
- ✅ TypeScript types từ Swagger/OpenAPI
- ✅ Axios API client functions
- ✅ React Query hooks (useQuery, useMutation)
- ✅ Service layer với error handling
- ✅ Custom hooks với cache invalidation

## 🏗️ Cấu trúc thư mục

```
├── api/
│   └── generated-orval/          # Code được generate bởi Orval
│       ├── admin-customer/       # Customer API + hooks
│       ├── auth/                 # Auth API + hooks
│       ├── category/             # Category API + hooks
│       ├── product/              # Product API + hooks
│       ├── schemas/              # TypeScript types
│       └── ...                   # Các modules khác
│
├── src/lib/
│   ├── api/                      # API client configuration
│   │   ├── client.ts             # Main Axios với token refresh
│   │   ├── orval-client.ts       # Orval Axios instance
│   │   ├── types.ts              # TypeScript types
│   │   └── errors.ts             # Error handling
│   │
│   ├── hooks/                    # Custom React Query hooks
│   │   ├── useCustomers.ts       # Customer hooks
│   │   ├── useProducts.ts        # Product hooks
│   │   ├── useCategories.ts      # Category hooks
│   │   ├── generated.ts          # Re-export tất cả generated hooks
│   │   └── index.ts              # Main export
│   │
│   ├── services/                 # Service layer
│   │   ├── authService.ts        # Auth service
│   │   ├── customerService.ts    # Customer service
│   │   ├── productService.ts     # Product service
│   │   ├── categoryService.ts    # Category service
│   │   └── index.ts              # Main export
│   │
│   └── utils/                    # Utility functions
│
├── orval.config.js               # Orval configuration
└── swagger.json                  # OpenAPI specification
```

## 🚀 Cách sử dụng

### 1. Generate API Client

```bash
# Generate một lần
npm run api:generate

# Watch mode (tự động generate khi swagger.json thay đổi)
npm run api:watch
```

### 2. Sử dụng Hooks trong Component

#### Fetch data (GET)

```tsx
import { useCustomers, useCustomer } from '@/lib/hooks';

function CustomerList() {
  const { data, isLoading, error } = useCustomers({
    Page: 1,
    PageSize: 10,
    SearchTerm: 'John'
  });

  if (isLoading) return <div>Loading...</div>;
  if (error) return <div>Error: {error.message}</div>;

  return (
    <div>
      {data?.data?.items?.map(customer => (
        <div key={customer.id}>{customer.fullName}</div>
      ))}
    </div>
  );
}
```

#### Create/Update/Delete (POST/PUT/DELETE)

```tsx
import { useCreateCustomer, useUpdateCustomer, useDeleteCustomer } from '@/lib/hooks';

function CustomerForm() {
  const createCustomer = useCreateCustomer();
  const updateCustomer = useUpdateCustomer();
  const deleteCustomer = useDeleteCustomer();

  const handleCreate = async (data: any) => {
    await createCustomer.mutateAsync({ data });
    // List sẽ tự động refetch sau khi create thành công
  };

  const handleUpdate = async (id: string, data: any) => {
    await updateCustomer.mutateAsync({ id, data });
  };

  const handleDelete = async (id: string) => {
    await deleteCustomer.mutateAsync({ id });
  };

  return (
    <button onClick={() => handleCreate({ name: 'John' })}>
      {createCustomer.isPending ? 'Creating...' : 'Create'}
    </button>
  );
}
```

### 3. Sử dụng Services (Alternative)

Nếu không muốn dùng hooks, có thể dùng services trực tiếp:

```tsx
import { customerService } from '@/lib/services';

async function fetchCustomers() {
  try {
    const customers = await customerService.getCustomers({
      Page: 1,
      PageSize: 10
    });
    console.log(customers);
  } catch (error) {
    console.error('Failed:', error);
  }
}
```

### 4. Sử dụng Generated API trực tiếp

```tsx
import { useGetApiAdminCustomer } from '@/lib/hooks/generated';

function CustomerList() {
  const { data } = useGetApiAdminCustomer({ Page: 1, PageSize: 10 });
  return <div>{/* ... */}</div>;
}
```

## 📦 Các modules đã có sẵn

### Hooks
- ✅ `useCustomers` - Quản lý khách hàng
- ✅ `useProducts` - Quản lý sản phẩm
- ✅ `useCategories` - Quản lý danh mục
- 🔄 Tất cả generated hooks từ Swagger

### Services
- ✅ `authService` - Authentication
- ✅ `customerService` - Customer management
- ✅ `productService` - Product management
- ✅ `categoryService` - Category management

## 🔧 Configuration

### Orval Config (`orval.config.js`)

```javascript
module.exports = {
    'api-client': {
        input: './swagger.json',
        output: {
            mode: 'tags-split',              // Split by API tags
            target: './api/generated-orval/index.ts',
            schemas: './api/generated-orval/schemas',
            client: 'react-query',           // Generate React Query hooks
            clean: true,                     // Clean before generate
            prettier: true,                  // Format with prettier
            override: {
                mutator: {
                    path: './src/lib/api/orval-client.ts',
                    name: 'apiClient',       // Custom Axios instance
                },
            }
        }
    }
};
```

### Environment Variables (`.env`)

```env
VITE_API_URL=http://localhost:3000/api
VITE_ENV=development
```

## 🎯 Best Practices

### 1. Ưu tiên sử dụng Custom Hooks

```tsx
// ✅ Good - Có cache invalidation tự động
import { useCustomers, useCreateCustomer } from '@/lib/hooks';

// ⚠️ OK - Nhưng phải tự quản lý cache
import { useGetApiAdminCustomer } from '@/lib/hooks/generated';
```

### 2. Handle Loading và Error States

```tsx
function CustomerList() {
  const { data, isLoading, error, isError } = useCustomers();

  if (isLoading) return <LoadingSpinner />;
  if (isError) return <ErrorMessage error={error} />;
  if (!data) return <EmptyState />;

  return <CustomerTable data={data} />;
}
```

### 3. Sử dụng Query Options

```tsx
const { data } = useCustomers(
  { Page: 1, PageSize: 10 },
  {
    query: {
      refetchInterval: 30000,        // Refetch mỗi 30s
      refetchOnWindowFocus: true,    // Refetch khi focus window
      staleTime: 5 * 60 * 1000,      // Cache 5 phút
      retry: 3,                      // Retry 3 lần nếu fail
    }
  }
);
```

### 4. Invalidate Cache sau Mutations

```tsx
const queryClient = useQueryClient();
const createCustomer = useCreateCustomer();

await createCustomer.mutateAsync({ data });

// Invalidate để refetch
queryClient.invalidateQueries({ queryKey: ['customers'] });
```

## 🆕 Tạo Hooks và Services mới

### 1. Generate từ Swagger

Khi thêm API mới vào `swagger.json`:

```bash
npm run api:generate
```

Orval sẽ tự động generate hooks và types.

### 2. Tạo Custom Hook

```typescript
// src/lib/hooks/useOrders.ts
import { useQueryClient } from '@tanstack/react-query';
import {
    useGetApiOrder,
    usePostApiOrder,
    getGetApiOrderQueryKey,
} from '../../../api/generated-orval/order/order';

// Re-export với tên đơn giản hơn
export const useOrders = useGetApiOrder;

// Custom hook với cache invalidation
export function useCreateOrder() {
    const queryClient = useQueryClient();

    return usePostApiOrder({
        mutation: {
            onSuccess: () => {
                queryClient.invalidateQueries({ 
                    queryKey: getGetApiOrderQueryKey() 
                });
            },
        },
    });
}
```

### 3. Tạo Service

```typescript
// src/lib/services/orderService.ts
import { getAuth } from '../../../api/generated-orval/order/order';

const orderApi = getAuth();

export const orderService = {
    async getOrders(params?: any) {
        try {
            const response = await orderApi.getApiOrder(params);
            return response.data;
        } catch (error) {
            throw new Error('Failed to fetch orders');
        }
    },
};
```

### 4. Export trong index files

```typescript
// src/lib/hooks/index.ts
export * from './useOrders';

// src/lib/services/index.ts
export { orderService } from './orderService';
```

## 🐛 Troubleshooting

### API không generate

```bash
# Kiểm tra swagger.json có hợp lệ
npm run api:generate

# Xem error messages chi tiết
```

### Hooks không hoạt động

1. Đảm bảo đã wrap app với `QueryClientProvider`:

```tsx
import { QueryClient, QueryClientProvider } from '@tanstack/react-query';

const queryClient = new QueryClient();

function App() {
  return (
    <QueryClientProvider client={queryClient}>
      {/* Your app */}
    </QueryClientProvider>
  );
}
```

2. Kiểm tra React Query DevTools:

```tsx
import { ReactQueryDevtools } from '@tanstack/react-query-devtools';

<QueryClientProvider client={queryClient}>
  <App />
  <ReactQueryDevtools initialIsOpen={false} />
</QueryClientProvider>
```

### Authentication issues

1. Kiểm tra token trong storage:

```typescript
import { tokenStorage } from '@/lib/api/client';

console.log(tokenStorage.getAccessToken());
```

2. Verify API base URL:

```typescript
console.log(import.meta.env.VITE_API_URL);
```

## 📚 Tài liệu tham khảo

- [Orval Documentation](https://orval.dev/)
- [React Query Documentation](https://tanstack.com/query/latest)
- [Axios Documentation](https://axios-http.com/)

## 📝 Examples

Xem file `src/lib/hooks/useCustomers.example.tsx` để có các ví dụ chi tiết về:
- Pagination
- Search với debounce
- Create/Update/Delete
- Optimistic updates
- Error handling
- Loading states

## ✅ Checklist

- [x] Orval config đã được cấu hình
- [x] Generate API client thành công
- [x] React Query hooks được generate
- [x] Custom hooks với cache invalidation
- [x] Service layer với error handling
- [x] TypeScript types đầy đủ
- [x] Examples và documentation
- [x] Authentication với token refresh

## 🎉 Kết luận

Bạn đã có một hệ thống API integration hoàn chỉnh với:
- ✅ Type-safe API calls
- ✅ Automatic caching và refetching
- ✅ Error handling
- ✅ Token refresh tự động
- ✅ React Query hooks
- ✅ Service layer
- ✅ Examples và documentation

Chỉ cần chạy `npm run api:generate` mỗi khi swagger.json thay đổi!
