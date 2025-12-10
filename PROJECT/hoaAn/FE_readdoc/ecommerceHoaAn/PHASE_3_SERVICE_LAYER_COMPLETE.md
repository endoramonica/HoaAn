# 🛠️ Phase 3: Service Layer & Custom Hooks - COMPLETED ✅

**Date**: December 7, 2025  
**Status**: ✅ COMPLETE

---

## Summary

Tất cả service layer và custom hooks đã được tạo/cập nhật để hỗ trợ Services-Product Unification.

---

## What Was Done

### 1. Created Service Types & Enums ✅

**File**: `src/types/service.ts`

```typescript
// Service Categories
export enum ServiceCategory {
  ANCESTOR_WORSHIP = 'ancestor-worship',
  OPENING_CEREMONY = 'opening-ceremony',
  WEDDING = 'wedding',
  BUDDHA_WORSHIP = 'buddha-worship',
  NEW_HOUSE = 'new-house',
  FENG_SHUI_CONSULTATION = 'feng-shui-consultation',
}

// Labels
export const SERVICE_CATEGORY_LABELS: Record<ServiceCategory, string> = {
  [ServiceCategory.ANCESTOR_WORSHIP]: 'Cúng Gia Tiên',
  [ServiceCategory.OPENING_CEREMONY]: 'Lễ Khai Trương',
  [ServiceCategory.WEDDING]: 'Lễ Cưới Hỏi',
  [ServiceCategory.BUDDHA_WORSHIP]: 'Cúng Phật',
  [ServiceCategory.NEW_HOUSE]: 'Lễ Tân Gia',
  [ServiceCategory.FENG_SHUI_CONSULTATION]: 'Tư Vấn Phong Thủy',
};

// Helper functions
export function getServiceCategoryLabel(category: ServiceCategory | string): string
export function isValidServiceCategory(category: string): category is ServiceCategory
export function getAllServiceCategories(): ServiceCategory[]
export function getServiceCategoriesWithLabels(): Array<{ value: ServiceCategory; label: string }>
```

---

### 2. Updated ProductService ✅

**File**: `src/lib/services/productService.ts`

**New Methods**:
```typescript
// Get services with filtering
async getServices(params?: ProductFilterParams & { serviceCategory?: string }): Promise<PagedResponse<ProductDto>>

// Get products only (not services)
async getProductsOnly(params?: ProductFilterParams): Promise<PagedResponse<ProductDto>>
```

**Features**:
- ✅ Filter by serviceCategory
- ✅ Search functionality
- ✅ Pagination support
- ✅ Sorting support
- ✅ Type discrimination (type: 'service')

---

### 3. Updated OrderService ✅

**File**: `src/lib/services/orderService.ts`

**New Methods**:
```typescript
// Get my service orders
async getMyServiceOrders(params?: OrderFilterParams & { serviceCategory?: string }): Promise<PagedResponse<any>>

// Get my product orders
async getMyProductOrders(params?: OrderFilterParams): Promise<PagedResponse<any>>

// Get service orders (Admin)
async getServiceOrders(params?: OrderFilterParams & { serviceCategory?: string }): Promise<PagedResponse<any>>

// Get product orders (Admin)
async getProductOrders(params?: OrderFilterParams): Promise<PagedResponse<any>>
```

**Features**:
- ✅ Filter by type (product/service)
- ✅ Filter by serviceCategory
- ✅ Pagination support
- ✅ Status filtering
- ✅ Separate methods for products and services

---

### 4. Created useServices Hook ✅

**File**: `src/lib/hooks/useServices.ts`

```typescript
interface UseServicesReturn {
  services: ProductDto[];
  pagedData: PagedResponse<ProductDto> | null;
  isLoading: boolean;
  isRefreshing: boolean;
  error: string | null;
  params: ServiceFilterOptions;
  setParams: (params: ServiceFilterOptions) => void;
  loadServices: (params?: ServiceFilterOptions) => Promise<void>;
  getServiceById: (id: string) => Promise<ProductDto | null>;
  setServiceCategory: (category: ServiceCategory | string) => void;
  setSearchQuery: (query: string) => void;
  refresh: () => Promise<void>;
  goToPage: (page: number) => void;
  nextPage: () => void;
  previousPage: () => void;
}
```

**Usage**:
```typescript
const {
  services,
  isLoading,
  error,
  setServiceCategory,
  setSearchQuery,
  goToPage,
} = useServices({ autoLoad: true });
```

---

### 5. Created useServiceOrders Hook ✅

**File**: `src/lib/hooks/useServiceOrders.ts`

```typescript
interface UseServiceOrdersReturn {
  serviceOrders: OrderDto[];
  pagedData: PagedResponse<OrderDto> | null;
  isLoading: boolean;
  isRefreshing: boolean;
  error: string | null;
  params: ServiceOrderFilterOptions;
  setParams: (params: ServiceOrderFilterOptions) => void;
  loadServiceOrders: (params?: ServiceOrderFilterOptions) => Promise<void>;
  getServiceOrderById: (id: string) => Promise<OrderDto | null>;
  setServiceCategory: (category: ServiceCategory | string) => void;
  setStatus: (status: string) => void;
  refresh: () => Promise<void>;
  goToPage: (page: number) => void;
  nextPage: () => void;
  previousPage: () => void;
}
```

**Usage**:
```typescript
const {
  serviceOrders,
  isLoading,
  error,
  setServiceCategory,
  setStatus,
  goToPage,
} = useServiceOrders({ autoLoad: true, myOrdersOnly: true });
```

---

## File Structure

```
src/
├── types/
│   └── service.ts                    ✅ NEW - Service types & enums
├── lib/
│   ├── services/
│   │   ├── productService.ts         ✅ UPDATED - Added getServices()
│   │   └── orderService.ts           ✅ UPDATED - Added service methods
│   └── hooks/
│       ├── useServices.ts            ✅ NEW - Services hook
│       └── useServiceOrders.ts       ✅ NEW - Service orders hook
```

---

## API Integration

### ProductService Methods

```typescript
// Get services
const response = await productService.getServices({
  pageNumber: 1,
  pageSize: 10,
  serviceCategory: 'ancestor-worship',
  search: 'cúng',
});

// Get products only
const response = await productService.getProductsOnly({
  pageNumber: 1,
  pageSize: 10,
});
```

### OrderService Methods

```typescript
// Get my service orders
const response = await orderService.getMyServiceOrders({
  pageNumber: 1,
  pageSize: 10,
  serviceCategory: 'ancestor-worship',
});

// Get my product orders
const response = await orderService.getMyProductOrders({
  pageNumber: 1,
  pageSize: 10,
});

// Get all service orders (Admin)
const response = await orderService.getServiceOrders({
  pageNumber: 1,
  pageSize: 10,
  serviceCategory: 'ancestor-worship',
});

// Get all product orders (Admin)
const response = await orderService.getProductOrders({
  pageNumber: 1,
  pageSize: 10,
});
```

---

## Hook Usage Examples

### useServices Hook

```typescript
import { useServices } from '@/lib/hooks/useServices';
import { ServiceCategory } from '@/types/service';

export function ServicesPage() {
  const {
    services,
    isLoading,
    error,
    pagedData,
    setServiceCategory,
    setSearchQuery,
    goToPage,
  } = useServices({ autoLoad: true });

  return (
    <div>
      {/* Category Filter */}
      <select onChange={(e) => setServiceCategory(e.target.value)}>
        <option value="">Tất cả</option>
        <option value={ServiceCategory.ANCESTOR_WORSHIP}>Cúng Gia Tiên</option>
        <option value={ServiceCategory.OPENING_CEREMONY}>Lễ Khai Trương</option>
      </select>

      {/* Search */}
      <input
        type="text"
        placeholder="Tìm kiếm dịch vụ..."
        onChange={(e) => setSearchQuery(e.target.value)}
      />

      {/* Services List */}
      {isLoading ? (
        <LoadingSpinner />
      ) : error ? (
        <ErrorAlert message={error} />
      ) : (
        <div className="grid">
          {services.map(service => (
            <ServiceCard key={service.id} service={service} />
          ))}
        </div>
      )}

      {/* Pagination */}
      <Pagination
        currentPage={pagedData?.pageNumber || 1}
        totalPages={pagedData?.totalPages || 0}
        onPageChange={goToPage}
      />
    </div>
  );
}
```

### useServiceOrders Hook

```typescript
import { useServiceOrders } from '@/lib/hooks/useServiceOrders';

export function ProfilePage() {
  const {
    serviceOrders,
    isLoading,
    error,
    pagedData,
    setServiceCategory,
    setStatus,
    goToPage,
  } = useServiceOrders({ autoLoad: true, myOrdersOnly: true });

  return (
    <div>
      {/* Filters */}
      <select onChange={(e) => setServiceCategory(e.target.value)}>
        <option value="">Tất cả dịch vụ</option>
        <option value="ancestor-worship">Cúng Gia Tiên</option>
        <option value="opening-ceremony">Lễ Khai Trương</option>
      </select>

      <select onChange={(e) => setStatus(e.target.value)}>
        <option value="">Tất cả trạng thái</option>
        <option value="Pending">Chờ xác nhận</option>
        <option value="Confirmed">Đã xác nhận</option>
      </select>

      {/* Service Orders List */}
      {isLoading ? (
        <LoadingSpinner />
      ) : error ? (
        <ErrorAlert message={error} />
      ) : (
        <div className="space-y-4">
          {serviceOrders.map(order => (
            <ServiceOrderCard key={order.orderId} order={order} />
          ))}
        </div>
      )}

      {/* Pagination */}
      <Pagination
        currentPage={pagedData?.pageNumber || 1}
        totalPages={pagedData?.totalPages || 0}
        onPageChange={goToPage}
      />
    </div>
  );
}
```

---

## Type Safety

All hooks and services are fully typed with TypeScript:

```typescript
// Service types
import { ServiceCategory, SERVICE_CATEGORY_LABELS } from '@/types/service';

// Hook types
import type { UseServicesReturn } from '@/lib/hooks/useServices';
import type { UseServiceOrdersReturn } from '@/lib/hooks/useServiceOrders';

// API types
import type { ProductDto, OrderDto, PagedResponse } from '@/lib/api/types';
```

---

## Error Handling

All hooks include error handling:

```typescript
const { error, isLoading } = useServices();

if (error) {
  return <ErrorAlert message={error} />;
}
```

---

## Next Steps: Phase 4

Now we need to:

1. **Update Pages**
   - Update `src/components/ServicesPage.tsx` to use `useServices` hook
   - Update `src/components/ProfilePage.tsx` to use `useServiceOrders` hook
   - Replace mock data with real API calls

2. **Testing**
   - Test services loading
   - Test filtering by category
   - Test search functionality
   - Test pagination
   - Test service orders display

---

## Verification Checklist

- [x] Service types created
- [x] ServiceCategory enum defined
- [x] SERVICE_CATEGORY_LABELS mapping created
- [x] ProductService updated with getServices()
- [x] ProductService updated with getProductsOnly()
- [x] OrderService updated with service methods
- [x] useServices hook created
- [x] useServiceOrders hook created
- [x] All hooks fully typed
- [x] Error handling implemented
- [x] Pagination support added
- [x] Filtering support added

---

## Ready for Phase 4

✅ Service layer is complete and ready for page integration

