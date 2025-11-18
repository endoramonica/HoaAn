# Frontend Pagination Usage Guide

## Overview

This guide explains how to use the server-side pagination system in the POS frontend application.

---

## Quick Start

### 1. Import the Hook

```typescript
import { usePaginatedApi } from '../hooks/usePaginatedApi';
import { api } from '../services/api';
import { Order } from '../types';
```

### 2. Use in Component

```typescript
const MyComponent = () => {
  const {
    data,           // Array of items for current page
    meta,           // Pagination metadata
    isLoading,      // Loading state
    error,          // Error message if any
    page,           // Current page number
    pageSize,       // Items per page
    setPage,        // Function to change page
    setPageSize,    // Function to change page size
    setFilters,     // Function to update filters
    refetch,        // Function to manually refetch data
  } = usePaginatedApi<Order>({
    fetchFn: api.fetchOrders,
    initialPageSize: 10,
    initialFilters: {
      status: 'all',
    },
  });

  return (
    <div>
      {/* Your component JSX */}
    </div>
  );
};
```

---

## Hook API Reference

### `usePaginatedApi<T>(options)`

#### Options

| Option | Type | Required | Default | Description |
|--------|------|----------|---------|-------------|
| `fetchFn` | Function | Yes | - | API function to fetch paginated data |
| `initialPage` | number | No | 1 | Initial page number |
| `initialPageSize` | number | No | 10 | Initial items per page |
| `initialSortBy` | string | No | - | Initial sort field |
| `initialSortOrder` | 'asc' \| 'desc' | No | 'desc' | Initial sort order |
| `initialFilters` | object | No | {} | Initial filter values |
| `autoFetch` | boolean | No | true | Auto-fetch on mount |

#### Return Value

| Property | Type | Description |
|----------|------|-------------|
| `data` | T[] | Array of items for current page |
| `meta` | PaginationMeta \| null | Pagination metadata |
| `isLoading` | boolean | Loading state |
| `error` | string \| null | Error message |
| `page` | number | Current page number |
| `pageSize` | number | Items per page |
| `sortBy` | string \| undefined | Current sort field |
| `sortOrder` | 'asc' \| 'desc' | Current sort order |
| `filters` | object | Current filter values |
| `setPage` | (page: number) => void | Change page |
| `setPageSize` | (size: number) => void | Change page size |
| `setSortBy` | (field?: string) => void | Change sort field |
| `setSortOrder` | (order: 'asc' \| 'desc') => void | Change sort order |
| `setFilters` | (filters) => void | Update filters |
| `refetch` | () => Promise<void> | Manually refetch data |
| `goToNextPage` | () => void | Go to next page |
| `goToPreviousPage` | () => void | Go to previous page |
| `goToFirstPage` | () => void | Go to first page |
| `goToLastPage` | () => void | Go to last page |

---

## Complete Example: Orders Page

```typescript
import React, { useState, useEffect } from 'react';
import { usePaginatedApi } from '../hooks/usePaginatedApi';
import { PaginationCustom } from '../components/ui/pagination-custom';
import { api } from '../services/api';
import { Order } from '../types';
import { Loader2 } from 'lucide-react';

export const OrdersPage = () => {
  const [searchTerm, setSearchTerm] = useState('');
  const [statusFilter, setStatusFilter] = useState('all');
  
  // Initialize pagination
  const {
    data: orders,
    meta,
    isLoading,
    error,
    setPage,
    setPageSize,
    setFilters,
  } = usePaginatedApi<Order>({
    fetchFn: api.fetchOrders,
    initialPageSize: 10,
    initialFilters: {
      status: statusFilter,
      search: searchTerm,
    },
  });
  
  // Update filters when search/status changes
  useEffect(() => {
    setFilters({
      status: statusFilter,
      search: searchTerm,
    });
  }, [searchTerm, statusFilter, setFilters]);
  
  return (
    <div className="space-y-6">
      {/* Search & Filters */}
      <div className="flex gap-4">
        <input
          type="text"
          value={searchTerm}
          onChange={(e) => setSearchTerm(e.target.value)}
          placeholder="Search orders..."
          className="flex-1 px-4 py-2 border rounded"
        />
        
        <select
          value={statusFilter}
          onChange={(e) => setStatusFilter(e.target.value)}
          className="px-4 py-2 border rounded"
        >
          <option value="all">All Status</option>
          <option value="pending">Pending</option>
          <option value="completed">Completed</option>
        </select>
      </div>
      
      {/* Loading State */}
      {isLoading && (
        <div className="flex justify-center py-8">
          <Loader2 className="w-8 h-8 animate-spin" />
        </div>
      )}
      
      {/* Error State */}
      {error && (
        <div className="text-red-500 py-4">
          Error: {error}
        </div>
      )}
      
      {/* Data Table */}
      {!isLoading && !error && (
        <>
          <table className="w-full">
            <thead>
              <tr>
                <th>Order #</th>
                <th>Customer</th>
                <th>Total</th>
                <th>Status</th>
              </tr>
            </thead>
            <tbody>
              {orders.map((order) => (
                <tr key={order.id}>
                  <td>{order.orderNumber}</td>
                  <td>{order.customerName}</td>
                  <td>${order.total.toFixed(2)}</td>
                  <td>{order.status}</td>
                </tr>
              ))}
            </tbody>
          </table>
          
          {/* Pagination */}
          {meta && (
            <PaginationCustom
              currentPage={meta.currentPage}
              totalPages={meta.totalPages}
              itemsPerPage={meta.pageSize}
              totalItems={meta.totalItems}
              onPageChange={setPage}
              onItemsPerPageChange={setPageSize}
            />
          )}
        </>
      )}
    </div>
  );
};
```

---

## Filtering Examples

### Simple Filter

```typescript
const {
  data,
  setFilters,
} = usePaginatedApi<Product>({
  fetchFn: api.fetchProducts,
  initialFilters: {
    category: 'electronics',
  },
});

// Update filter
const handleCategoryChange = (category: string) => {
  setFilters({ category });
};
```

### Multiple Filters

```typescript
const {
  data,
  setFilters,
} = usePaginatedApi<Customer>({
  fetchFn: api.fetchCustomers,
  initialFilters: {
    status: 'active',
    minSpent: 100,
  },
});

// Update multiple filters
const handleFilterChange = () => {
  setFilters({
    status: 'active',
    minSpent: 100,
    maxSpent: 1000,
  });
};
```

### Dynamic Filters with State

```typescript
const [filters, setLocalFilters] = useState({
  status: 'all',
  search: '',
  startDate: '',
});

const {
  data,
  setFilters: setApiFilters,
} = usePaginatedApi<Order>({
  fetchFn: api.fetchOrders,
  initialFilters: filters,
});

// Sync local filters with API
useEffect(() => {
  setApiFilters(filters);
}, [filters, setApiFilters]);

// Update local filter
const handleSearchChange = (search: string) => {
  setLocalFilters(prev => ({ ...prev, search }));
};
```

---

## Sorting Examples

### Basic Sorting

```typescript
const {
  data,
  sortBy,
  sortOrder,
  setSortBy,
  setSortOrder,
} = usePaginatedApi<Product>({
  fetchFn: api.fetchProducts,
  initialSortBy: 'name',
  initialSortOrder: 'asc',
});

const handleSort = (field: string) => {
  if (sortBy === field) {
    // Toggle sort order
    setSortOrder(sortOrder === 'asc' ? 'desc' : 'asc');
  } else {
    // Change sort field
    setSortBy(field);
    setSortOrder('asc');
  }
};
```

### Sortable Table Header

```typescript
const SortableHeader = ({ field, label }: { field: string; label: string }) => {
  const { sortBy, sortOrder, setSortBy, setSortOrder } = usePaginatedApi(/* ... */);
  
  const handleClick = () => {
    if (sortBy === field) {
      setSortOrder(sortOrder === 'asc' ? 'desc' : 'asc');
    } else {
      setSortBy(field);
      setSortOrder('asc');
    }
  };
  
  return (
    <th onClick={handleClick} className="cursor-pointer">
      {label}
      {sortBy === field && (
        <span>{sortOrder === 'asc' ? ' ↑' : ' ↓'}</span>
      )}
    </th>
  );
};
```

---

## Advanced Patterns

### Debounced Search

```typescript
import { useState, useEffect } from 'react';
import { useDebounce } from '../hooks/useDebounce'; // You may need to create this

const OrdersPage = () => {
  const [searchInput, setSearchInput] = useState('');
  const debouncedSearch = useDebounce(searchInput, 500); // 500ms delay
  
  const { data, setFilters } = usePaginatedApi<Order>({
    fetchFn: api.fetchOrders,
  });
  
  useEffect(() => {
    setFilters({ search: debouncedSearch });
  }, [debouncedSearch, setFilters]);
  
  return (
    <input
      value={searchInput}
      onChange={(e) => setSearchInput(e.target.value)}
      placeholder="Search..."
    />
  );
};
```

### Manual Refetch

```typescript
const { data, refetch } = usePaginatedApi<Order>({
  fetchFn: api.fetchOrders,
  autoFetch: false, // Don't fetch on mount
});

// Fetch when button clicked
const handleRefresh = async () => {
  await refetch();
};

// Fetch after mutation
const handleCreateOrder = async (orderData: any) => {
  await createOrder(orderData);
  await refetch(); // Refresh the list
};
```

### Role-Based Filtering

```typescript
import { useAuth } from '../contexts/AuthContext';

const OrdersPage = () => {
  const { user, hasPermission } = useAuth();
  const canViewAll = hasPermission(PERMISSIONS.VIEW_ALL_ORDERS);
  
  const { data } = usePaginatedApi<Order>({
    fetchFn: api.fetchOrders,
    initialFilters: {
      // Staff only see their orders
      staffId: canViewAll ? undefined : user?.id,
    },
  });
};
```

---

## Best Practices

### 1. Always Handle Loading State

```typescript
{isLoading && <LoadingSpinner />}
{!isLoading && data.map(...)}
```

### 2. Always Handle Error State

```typescript
{error && (
  <Alert variant="destructive">
    <AlertTitle>Error</AlertTitle>
    <AlertDescription>{error}</AlertDescription>
  </Alert>
)}
```

### 3. Reset Page When Filters Change

The hook automatically does this, but if you're managing page state separately:

```typescript
useEffect(() => {
  setPage(1); // Reset to first page
  setFilters(newFilters);
}, [newFilters]);
```

### 4. Use Meaningful Initial Values

```typescript
usePaginatedApi<Order>({
  fetchFn: api.fetchOrders,
  initialPageSize: 20,        // Good for tables
  initialSortBy: 'createdAt', // Latest first
  initialSortOrder: 'desc',
  initialFilters: {
    status: 'pending',        // Show pending by default
  },
});
```

### 5. Memoize Filter Objects

```typescript
const filters = useMemo(() => ({
  status: statusFilter,
  search: searchTerm,
}), [statusFilter, searchTerm]);

useEffect(() => {
  setFilters(filters);
}, [filters, setFilters]);
```

---

## Troubleshooting

### Data Not Updating
- Check if `autoFetch` is true
- Ensure filters are being passed correctly
- Call `refetch()` manually if needed

### Infinite Re-renders
- Wrap filter objects in `useMemo`
- Use `useCallback` for filter update functions
- Check dependency arrays in `useEffect`

### Incorrect Total Count
- Verify backend is returning correct `totalItems`
- Check if filters are applied before counting

### Performance Issues
- Reduce `pageSize` for large datasets
- Implement debouncing for search inputs
- Use `React.memo` for list items

---

## Migration from Client-Side Pagination

### Before (Client-Side)
```typescript
const { paginatedData } = usePagination({
  data: allOrders,
  initialItemsPerPage: 10,
});
```

### After (Server-Side)
```typescript
const { data } = usePaginatedApi<Order>({
  fetchFn: api.fetchOrders,
  initialPageSize: 10,
});
```

### Key Differences
- No need to pass `data` array
- Filters handled server-side (better performance)
- Loading states managed automatically
- Total count accurate for filtered results

---

## Testing

### Unit Test Example

```typescript
import { renderHook, waitFor } from '@testing-library/react';
import { usePaginatedApi } from '../usePaginatedApi';

test('fetches paginated data', async () => {
  const mockFetchFn = jest.fn().mockResolvedValue({
    data: [{ id: 1 }, { id: 2 }],
    meta: { currentPage: 1, totalPages: 1, totalItems: 2 },
  });
  
  const { result } = renderHook(() =>
    usePaginatedApi({
      fetchFn: mockFetchFn,
    })
  );
  
  await waitFor(() => {
    expect(result.current.isLoading).toBe(false);
  });
  
  expect(result.current.data).toHaveLength(2);
  expect(mockFetchFn).toHaveBeenCalledWith({
    page: 1,
    pageSize: 10,
    sortOrder: 'desc',
    filters: {},
  });
});
```

---

## Related Documentation

- [Server-Side Pagination Implementation](/docs/07_server_side_pagination_implementation.md) - Full backend guide
- [Quick Start (Vietnamese)](/docs/PAGINATION_QUICKSTART_VI.md) - Quick reference
- [API Service](/services/api.ts) - Mock API implementation

---

**Version**: 1.0  
**Last Updated**: November 12, 2025  
**Maintained By**: Frontend Team
