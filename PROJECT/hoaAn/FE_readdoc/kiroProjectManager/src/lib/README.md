# API Integration Guide

Hướng dẫn sử dụng API client, hooks và services được generate từ Swagger/OpenAPI.

## Cấu trúc thư mục

```
src/lib/
├── api/              # API client configuration
│   ├── client.ts     # Main Axios instance với token refresh
│   ├── orval-client.ts  # Orval-specific Axios instance
│   ├── types.ts      # TypeScript types
│   └── errors.ts     # Error handling utilities
├── hooks/            # React Query hooks
│   ├── useCustomers.ts
│   ├── useProducts.ts
│   ├── useCategories.ts
│   └── index.ts
├── services/         # Service layer
│   ├── authService.ts
│   ├── customerService.ts
│   ├── productService.ts
│   ├── categoryService.ts
│   └── index.ts
└── utils/            # Utility functions
```

## Generate API Client

### Lệnh generate

```bash
# Generate một lần
npm run api:generate

# Watch mode (tự động generate khi swagger.json thay đổi)
npm run api:watch
```

### Output

Orval sẽ generate:
- `api/generated-orval/{tag}/{tag}.ts` - API client functions
- `api/generated-orval/{tag}/{tag}.hooks.ts` - React Query hooks (nếu dùng client: 'react-query')
- `api/generated-orval/schemas/` - TypeScript types

## Sử dụng Hooks (Recommended)

Hooks cung cấp caching, refetching, và state management tự động.

### Fetch data với useQuery

```tsx
import { useCustomers, useCustomer } from '@/lib/hooks';

function CustomerList() {
  // Fetch danh sách customers với pagination
  const { data, isLoading, error } = useCustomers({
    Page: 1,
    PageSize: 10,
    SearchTerm: 'John'
  });

  if (isLoading) return <div>Loading...</div>;
  if (error) return <div>Error: {error.message}</div>;

  return (
    <div>
      {data?.items?.map(customer => (
        <div key={customer.id}>{customer.name}</div>
      ))}
    </div>
  );
}

function CustomerDetail({ id }: { id: string }) {
  // Fetch single customer
  const { data: customer } = useCustomer(id);

  return <div>{customer?.name}</div>;
}
```

### Mutations với useMutation

```tsx
import { useCreateCustomer, useUpdateCustomer, useDeleteCustomer } from '@/lib/hooks';

function CustomerForm() {
  const createCustomer = useCreateCustomer();
  const updateCustomer = useUpdateCustomer();
  const deleteCustomer = useDeleteCustomer();

  const handleCreate = async (data: any) => {
    try {
      await createCustomer.mutateAsync(data);
      // Success! List sẽ tự động refetch
    } catch (error) {
      console.error('Failed to create customer:', error);
    }
  };

  const handleUpdate = async (id: string, data: any) => {
    await updateCustomer.mutateAsync({ id, data });
  };

  const handleDelete = async (id: string) => {
    await deleteCustomer.mutateAsync(id);
  };

  return (
    <form onSubmit={(e) => {
      e.preventDefault();
      handleCreate({ name: 'John Doe' });
    }}>
      {/* Form fields */}
      <button type="submit" disabled={createCustomer.isPending}>
        {createCustomer.isPending ? 'Creating...' : 'Create'}
      </button>
    </form>
  );
}
```

### Advanced: Query options

```tsx
import { useCustomers } from '@/lib/hooks';

function CustomerList() {
  const { data } = useCustomers(
    { Page: 1, PageSize: 10 },
    {
      // Refetch every 30 seconds
      refetchInterval: 30000,
      
      // Only fetch when component is visible
      refetchOnWindowFocus: true,
      
      // Cache for 5 minutes
      staleTime: 5 * 60 * 1000,
      
      // Retry failed requests
      retry: 3,
    }
  );

  return <div>{/* ... */}</div>;
}
```

## Sử dụng Services (Alternative)

Services cung cấp API calls trực tiếp mà không có caching.

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
    console.error('Failed to fetch customers:', error);
  }
}

async function createCustomer(data: any) {
  try {
    const newCustomer = await customerService.createCustomer(data);
    console.log('Created:', newCustomer);
  } catch (error) {
    console.error('Failed to create customer:', error);
  }
}
```

## Sử dụng Generated API trực tiếp

```tsx
import { getAuth } from '@/api/generated-orval/admin-customer/admin-customer';

const customerApi = getAuth();

async function fetchCustomers() {
  const response = await customerApi.getApiAdminCustomer({
    Page: 1,
    PageSize: 10
  });
  console.log(response.data);
}
```

## Authentication

Token được quản lý tự động bởi `client.ts`:

```tsx
import { tokenStorage } from '@/lib/api/client';

// Login
tokenStorage.setTokens(accessToken, refreshToken, rememberMe);

// Logout
tokenStorage.clearTokens();

// Get current token
const token = tokenStorage.getAccessToken();
```

Token refresh được xử lý tự động khi API trả về 401.

## Error Handling

### Trong Hooks

```tsx
import { useCustomers } from '@/lib/hooks';

function CustomerList() {
  const { data, error, isError } = useCustomers();

  if (isError) {
    return <div>Error: {error.message}</div>;
  }

  return <div>{/* ... */}</div>;
}
```

### Trong Services

```tsx
import { customerService } from '@/lib/services';

try {
  const customers = await customerService.getCustomers();
} catch (error) {
  // Error đã được format bởi service layer
  console.error(error.message);
}
```

## Best Practices

1. **Ưu tiên sử dụng Hooks** cho component-based data fetching
2. **Sử dụng Services** cho imperative calls (event handlers, utilities)
3. **Sử dụng Generated API** khi cần control chi tiết
4. **Invalidate queries** sau mutations để refetch data
5. **Sử dụng query keys** để quản lý cache hiệu quả
6. **Handle loading và error states** trong UI

## Tạo Hooks và Services mới

### 1. Tạo Service

```typescript
// src/lib/services/orderService.ts
import { getAuth } from '@/api/generated-orval/order/order';

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
  
  // ... other methods
};
```

### 2. Tạo Hooks

```typescript
// src/lib/hooks/useOrders.ts
import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';
import { orderService } from '../services/orderService';

export const orderKeys = {
  all: ['orders'] as const,
  lists: () => [...orderKeys.all, 'list'] as const,
  list: (params?: any) => [...orderKeys.lists(), params] as const,
};

export function useOrders(params?: any) {
  return useQuery({
    queryKey: orderKeys.list(params),
    queryFn: () => orderService.getOrders(params),
  });
}

export function useCreateOrder() {
  const queryClient = useQueryClient();
  
  return useMutation({
    mutationFn: (data: any) => orderService.createOrder(data),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: orderKeys.lists() });
    },
  });
}
```

### 3. Export trong index files

```typescript
// src/lib/hooks/index.ts
export * from './useOrders';

// src/lib/services/index.ts
export { orderService } from './orderService';
```

## Troubleshooting

### API không generate

- Kiểm tra `swagger.json` có hợp lệ không
- Chạy `npm run api:generate` và xem error messages
- Kiểm tra `orval.config.js` configuration

### Hooks không hoạt động

- Đảm bảo đã wrap app với `QueryClientProvider`
- Kiểm tra React Query DevTools để debug
- Verify query keys đúng format

### Authentication issues

- Kiểm tra token trong localStorage/sessionStorage
- Verify API base URL trong `.env`
- Check network tab để xem request headers
