# 🚀 Frontend Integration Guide - Services-Product Unification

## 📋 Quick Summary

Backend đã sẵn sàng! Bạn có thể bắt đầu integrate Services vào frontend.

**Status**: ✅ Backend Ready  
**Date**: December 6, 2025

---

## 🎯 What You Need to Do

### 1. Update TypeScript Interfaces

#### ProductListDto
```typescript
// File: src/types/product.ts

export interface ProductListDto {
  // Existing fields
  id: string;
  name: string;
  code: string;
  price: number;
  compareAtPrice?: number;
  stockQuantity: number;
  categoryName?: string;
  primaryImage?: string;
  viewCount: number;
  favoriteCount: number;
  averageRating: number;
  
  // ✅ NEW FIELDS
  type: 'product' | 'service';  // Default: 'product'
  serviceCategory?: ServiceCategory;
  serviceDuration?: string;  // "2-3 giờ"
  rating?: number;  // 0-5
}

export enum ServiceCategory {
  ANCESTOR_WORSHIP = 'ancestor-worship',
  OPENING_CEREMONY = 'opening-ceremony',
  WEDDING = 'wedding',
  BUDDHA_WORSHIP = 'buddha-worship',
  NEW_HOUSE = 'new-house',
  FENG_SHUI_CONSULTATION = 'feng-shui-consultation'
}

export const SERVICE_CATEGORY_LABELS: Record<ServiceCategory, string> = {
  [ServiceCategory.ANCESTOR_WORSHIP]: 'Cúng Gia Tiên',
  [ServiceCategory.OPENING_CEREMONY]: 'Lễ Khai Trương',
  [ServiceCategory.WEDDING]: 'Lễ Cưới Hỏi',
  [ServiceCategory.BUDDHA_WORSHIP]: 'Cúng Phật',
  [ServiceCategory.NEW_HOUSE]: 'Lễ Tân Gia',
  [ServiceCategory.FENG_SHUI_CONSULTATION]: 'Tư Vấn Phong Thủy'
};
```

#### ProductFilterDto
```typescript
// File: src/types/product.ts

export interface ProductFilterDto {
  // Existing fields
  page?: number;
  pageSize?: number;
  searchTerm?: string;
  categoryId?: string;
  storeId?: string;
  isActive?: boolean;
  minPrice?: number;
  maxPrice?: number;
  sortBy?: string;
  isDescending?: boolean;
  
  // ✅ NEW FIELDS
  type?: 'product' | 'service';
  serviceCategory?: string;
}
```

#### OrderDetailDto
```typescript
// File: src/types/order.ts

export interface OrderDetailDto {
  // Existing fields
  orderId: string;
  orderNumber: string;
  status: OrderStatus;
  statusText: string;
  totalAmount: number;
  createdAt: string;
  items: OrderItemDTO[];
  
  // ✅ NEW FIELDS
  type: 'product' | 'service';
  serviceCategory?: string;
  serviceDuration?: string;
  serviceLocation?: string;
  serviceDate?: string;
  serviceTime?: string;
  serviceNotes?: string;
}
```

#### OrderItemDTO
```typescript
// File: src/types/order.ts

export interface OrderItemDTO {
  // Existing fields
  id: string;
  productId: string;
  productName: string;
  unitPrice: number;
  quantity: number;
  totalPrice: number;
  
  // ✅ NEW FIELDS
  type: 'product' | 'service';
  serviceCategory?: string;
  serviceDuration?: string;
}
```

#### OrderFilterDTO
```typescript
// File: src/types/order.ts

export interface OrderFilterDTO {
  // Existing fields
  page?: number;
  pageSize?: number;
  keyword?: string;
  customerId?: string;
  storeId?: string;
  status?: OrderStatus;
  fromDate?: string;
  toDate?: string;
  sortBy?: string;
  sortDescending?: boolean;
  
  // ✅ NEW FIELDS
  type?: 'product' | 'service';
  serviceCategory?: string;
}
```

---

### 2. Update API Service

#### ProductService
```typescript
// File: src/services/ProductService.ts

class ProductService {
  async getProducts(filter: ProductFilterDto): Promise<PaginatedResult<ProductListDto>> {
    const response = await apiClient.get('/api/Product', { params: filter });
    return response.data.data;
  }
  
  // ✅ NEW: Convenience method for services
  async getServices(filter?: ProductFilterDto): Promise<PaginatedResult<ProductListDto>> {
    return this.getProducts({ ...filter, type: 'service' });
  }
}

export const productService = new ProductService();
```

#### OrderService
```typescript
// File: src/services/OrderService.ts

class OrderService {
  async getOrders(filter: OrderFilterDTO): Promise<PaginatedResult<OrderDetailDto>> {
    const response = await apiClient.get('/api/Order/my-orders', { params: filter });
    return response.data.data;
  }
  
  // ✅ NEW: Convenience methods
  async getServiceOrders(filter?: OrderFilterDTO): Promise<PaginatedResult<OrderDetailDto>> {
    return this.getOrders({ ...filter, type: 'service' });
  }
  
  async getProductOrders(filter?: OrderFilterDTO): Promise<PaginatedResult<OrderDetailDto>> {
    return this.getOrders({ ...filter, type: 'product' });
  }
}

export const orderService = new OrderService();
```

---

### 3. Create Custom Hooks

#### useServices Hook
```typescript
// File: src/hooks/useServices.ts

import { useProducts } from './useProducts';

export function useServices(options?: {
  autoLoad?: boolean;
  initialParams?: ProductFilterDto;
}) {
  const productsHook = useProducts({
    autoLoad: options?.autoLoad ?? true,
    initialParams: {
      ...options?.initialParams,
      type: 'service'
    }
  });
  
  return {
    services: productsHook.products,
    isLoading: productsHook.isLoading,
    error: productsHook.error,
    totalCount: productsHook.totalCount,
    totalPages: productsHook.totalPages,
    currentPage: productsHook.currentPage,
    setServiceCategory: (cat: string) => 
      productsHook.setParams({ serviceCategory: cat, page: 1 }),
    setSearchQuery: (q: string) => 
      productsHook.setParams({ searchTerm: q, page: 1 }),
    goToPage: productsHook.goToPage,
    refetch: productsHook.refetch
  };
}
```

#### useOrders Hook
```typescript
// File: src/hooks/useOrders.ts

import { useState, useEffect } from 'react';
import { orderService } from '@/services/OrderService';

export function useOrders(options?: {
  autoLoad?: boolean;
  initialParams?: OrderFilterDTO;
}) {
  const [orders, setOrders] = useState<OrderDetailDto[]>([]);
  const [isLoading, setIsLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const [filter, setFilter] = useState<OrderFilterDTO>(
    options?.initialParams || {}
  );
  const [totalPages, setTotalPages] = useState(0);
  const [currentPage, setCurrentPage] = useState(1);
  
  const fetchOrders = async () => {
    setIsLoading(true);
    setError(null);
    try {
      const result = await orderService.getOrders(filter);
      setOrders(result.data);
      setTotalPages(result.totalPages);
      setCurrentPage(result.currentPage);
    } catch (err: any) {
      setError(err.message);
    } finally {
      setIsLoading(false);
    }
  };
  
  useEffect(() => {
    if (options?.autoLoad) {
      fetchOrders();
    }
  }, [filter]);
  
  return {
    orders,
    isLoading,
    error,
    totalPages,
    currentPage,
    setType: (type: 'product' | 'service') => 
      setFilter({ ...filter, type, page: 1 }),
    setStatus: (status: OrderStatus) => 
      setFilter({ ...filter, status, page: 1 }),
    setServiceCategory: (cat: string) => 
      setFilter({ ...filter, serviceCategory: cat, page: 1 }),
    goToPage: (page: number) => 
      setFilter({ ...filter, page }),
    refetch: fetchOrders
  };
}
```

#### useServiceOrders Hook
```typescript
// File: src/hooks/useServiceOrders.ts

import { useOrders } from './useOrders';

export function useServiceOrders(options?: {
  autoLoad?: boolean;
  initialParams?: OrderFilterDTO;
}) {
  return useOrders({
    ...options,
    initialParams: {
      ...options?.initialParams,
      type: 'service'
    }
  });
}
```

---

### 4. Update Pages

#### ServicesPage
```typescript
// File: src/pages/ServicesPage.tsx

import { useServices } from '@/hooks/useServices';
import { ServiceCategory, SERVICE_CATEGORY_LABELS } from '@/types/product';

export function ServicesPage() {
  const {
    services,
    isLoading,
    error,
    totalPages,
    currentPage,
    setServiceCategory,
    setSearchQuery,
    goToPage
  } = useServices({ autoLoad: true });
  
  if (isLoading) return <LoadingSpinner />;
  if (error) return <ErrorAlert message={error} />;
  
  return (
    <div>
      <h1>Dịch Vụ</h1>
      
      {/* Category Filter */}
      <select onChange={(e) => setServiceCategory(e.target.value)}>
        <option value="">Tất cả</option>
        {Object.entries(SERVICE_CATEGORY_LABELS).map(([key, label]) => (
          <option key={key} value={key}>{label}</option>
        ))}
      </select>
      
      {/* Search */}
      <input 
        type="text" 
        placeholder="Tìm kiếm dịch vụ..."
        onChange={(e) => setSearchQuery(e.target.value)}
      />
      
      {/* Services Grid */}
      <div className="grid">
        {services.map(service => (
          <ServiceCard key={service.id} service={service} />
        ))}
      </div>
      
      {/* Pagination */}
      <Pagination 
        currentPage={currentPage}
        totalPages={totalPages}
        onPageChange={goToPage}
      />
    </div>
  );
}
```

#### ProfilePage - Orders Tab
```typescript
// File: src/pages/ProfilePage.tsx

import { useOrders } from '@/hooks/useOrders';

export function ProfilePage() {
  const [activeTab, setActiveTab] = useState('orders');
  
  // Product Orders
  const {
    orders: productOrders,
    isLoading: ordersLoading,
    error: ordersError
  } = useOrders({
    autoLoad: activeTab === 'orders',
    initialParams: { type: 'product' }
  });
  
  // Service Orders
  const {
    orders: serviceOrders,
    isLoading: servicesLoading,
    error: servicesError
  } = useServiceOrders({
    autoLoad: activeTab === 'services'
  });
  
  return (
    <Tabs value={activeTab} onValueChange={setActiveTab}>
      <TabsList>
        <TabsTrigger value="orders">Đơn hàng</TabsTrigger>
        <TabsTrigger value="services">Dịch vụ</TabsTrigger>
      </TabsList>
      
      <TabsContent value="orders">
        {ordersLoading ? <LoadingSpinner /> : (
          <OrdersList orders={productOrders} />
        )}
      </TabsContent>
      
      <TabsContent value="services">
        {servicesLoading ? <LoadingSpinner /> : (
          <ServiceOrdersList orders={serviceOrders} />
        )}
      </TabsContent>
    </Tabs>
  );
}
```

---

## 🧪 Testing

### Test Scenarios

1. **ServicesPage**:
   - [ ] Services load from API
   - [ ] Category filter works
   - [ ] Search works
   - [ ] Pagination works
   - [ ] Service cards display correctly

2. **ProfilePage - Orders Tab**:
   - [ ] Product orders load
   - [ ] Status filter works
   - [ ] Pagination works

3. **ProfilePage - Services Tab**:
   - [ ] Service orders load
   - [ ] Service category filter works
   - [ ] Service details display correctly (location, date, time)

### API Test URLs

```bash
# Get all services
GET http://localhost:5000/api/Product?type=service

# Get services by category
GET http://localhost:5000/api/Product?type=service&serviceCategory=ancestor-worship

# Get service orders
GET http://localhost:5000/api/Order/my-orders?type=service

# Get product orders
GET http://localhost:5000/api/Order/my-orders?type=product
```

---

## 📞 Need Help?

**Backend Team Contact**:
- Check: `BACKEND_IMPLEMENTATION_COMPLETE.md` for full API documentation
- Migration file: `VietCommerce.Data/Migrations/20251206000000_AddServicesProductUnification.cs`

**Questions?**
- API not working? Check if migration was run
- Missing fields? Check TypeScript interfaces match backend DTOs
- Filtering not working? Check query parameter names

---

## ✅ Checklist

- [ ] Update TypeScript interfaces
- [ ] Update ProductService
- [ ] Update OrderService
- [ ] Create useServices hook
- [ ] Create useOrders hook
- [ ] Create useServiceOrders hook
- [ ] Update ServicesPage
- [ ] Update ProfilePage Orders Tab
- [ ] Update ProfilePage Services Tab
- [ ] Test all scenarios
- [ ] Remove mock data
- [ ] Remove localStorage usage

---

Good luck! 🚀
