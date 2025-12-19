# Design Document: Services-Product Unification

## Overview

Dự án này thống nhất Services và Products bằng cách mở rộng **2 models**:

1. **Product Model** - Cho ServicesPage (browse & buy services)
2. **Order Model** - Cho ProfilePage (order history)

**Lợi ích chính:**
- Giảm code duplication
- API integration đầy đủ
- Tái sử dụng logic filtering, pagination, error handling
- Dễ bảo trì và mở rộng
- Thống nhất UX

---

## Part 1: Product Model (ServicesPage)

### Architecture

```
ServicesPage
    ↓
useServices() hook
    ↓
ProductService.getServices()
    ↓
GET /api/v1/Product?type=service
    ↓
ASP.NET Core API
```

### Data Models

#### ProductListDto (Extended)

```typescript
interface ProductListDto {
  // Existing fields
  id: string;
  name: string;
  description?: string;
  price: number;
  primaryImage?: string;
  categoryName?: string;
  stockQuantity: number;
  inStock: boolean;
  
  // NEW: Type discriminator
  type?: 'product' | 'service';  // default: 'product'
  
  // NEW: Service-specific fields
  serviceCategory?: 'ancestor-worship' | 'opening-ceremony' | 'wedding' 
                  | 'buddha-worship' | 'new-house' | 'feng-shui-consultation';
  serviceDuration?: string;  // "2-3 giờ"
  rating?: number;           // 0-5
}
```

#### ProductFilterDto (Extended)

```typescript
interface ProductFilterDto {
  // Existing fields
  pageNumber?: number;
  pageSize?: number;
  searchTerm?: string;
  categoryId?: string;
  sortBy?: string;
  isDescending?: boolean;
  
  // NEW: Type filter
  type?: 'product' | 'service';
  
  // NEW: Service category filter
  serviceCategory?: string;
}
```

### Service Layer

```typescript
class ProductService {
  // NEW: Service-specific methods
  async getServices(filter?: ProductFilterDto): Promise<PaginatedResult<ProductListDto>> {
    return this.getProducts({ ...filter, type: 'service' });
  }
  
  async getServiceById(id: string): Promise<ProductDetailDto> {
    const product = await this.getProductById(id);
    if (product.type !== 'service') {
      throw new Error('Product is not a service');
    }
    return product;
  }
}
```

### Custom Hook: useServices()

```typescript
function useServices(options?: UseServicesOptions): UseServicesReturn {
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
    setServiceCategory: (cat) => productsHook.setParams({ serviceCategory: cat, pageNumber: 1 }),
    setSearchQuery: (q) => productsHook.setParams({ searchTerm: q, pageNumber: 1 }),
    goToPage: productsHook.goToPage,
    refetch: productsHook.refetch
  };
}
```

### ServicesPage Component

```typescript
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
  
  // Render services grid with filtering and pagination
}
```

---

## Part 2: Order Model (ProfilePage)

### Architecture

```
ProfilePage
├── Orders Tab
│   ↓
│   useOrders({ type: 'product' })
│   ↓
│   OrderService.getProductOrders()
│   ↓
│   GET /api/v1/Order/my-orders?type=product
│
└── Services Tab
    ↓
    useServiceOrders()
    ↓
    OrderService.getServiceOrders()
    ↓
    GET /api/v1/Order/my-orders?type=service
```

### Data Models

#### OrderDetailDto (Extended)

```typescript
interface OrderDetailDto {
  // Existing fields
  id: string;
  orderDate: string;
  status: 'pending' | 'processing' | 'shipped' | 'delivered' | 'cancelled';
  total: number;
  items: OrderItemDTO[];
  shippingAddress?: string;
  shippingMethod?: string;
  
  // NEW: Type discriminator
  type?: 'product' | 'service';
  
  // NEW: Service-specific fields (only if type='service')
  serviceCategory?: string;
  serviceDuration?: string;
  serviceLocation?: string;
  serviceDate?: string;
  serviceTime?: string;
  serviceNotes?: string;
}
```

#### OrderItemDTO (Extended)

```typescript
interface OrderItemDTO {
  // Existing fields
  id: string;
  productId: string;
  productName: string;
  price: number;
  quantity: number;
  image?: string;
  
  // NEW: Type field
  type?: 'product' | 'service';
  
  // NEW: Service-specific fields
  serviceCategory?: string;
  serviceDuration?: string;
}
```

#### OrderFilterDto (New)

```typescript
interface OrderFilterDto {
  pageNumber?: number;
  pageSize?: number;
  
  // NEW: Type filter
  type?: 'product' | 'service';
  
  // NEW: Service category filter
  serviceCategory?: string;
  
  // Status filter
  status?: string;
  
  // Date range
  startDate?: string;
  endDate?: string;
  
  // Search
  searchTerm?: string;
  
  // Sorting
  sortBy?: string;
  isDescending?: boolean;
}
```

### Service Layer

```typescript
class OrderService {
  // NEW: Order methods
  async getOrders(filter?: OrderFilterDto): Promise<PaginatedResult<OrderDetailDto>> {
    return this.apiClient.get('/api/v1/Order/my-orders', { params: filter });
  }
  
  async getServiceOrders(filter?: OrderFilterDto): Promise<PaginatedResult<OrderDetailDto>> {
    return this.getOrders({ ...filter, type: 'service' });
  }
  
  async getProductOrders(filter?: OrderFilterDto): Promise<PaginatedResult<OrderDetailDto>> {
    return this.getOrders({ ...filter, type: 'product' });
  }
}
```

### Custom Hooks

#### useOrders()

```typescript
function useOrders(options?: UseOrdersOptions): UseOrdersReturn {
  const [orders, setOrders] = useState<OrderDetailDto[]>([]);
  const [isLoading, setIsLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const [filter, setFilter] = useState<OrderFilterDto>(options?.initialParams || {});
  
  const fetchOrders = async () => {
    setIsLoading(true);
    try {
      const result = await orderService.getOrders(filter);
      setOrders(result.data);
    } catch (err) {
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
    setType: (type) => setFilter({ ...filter, type, pageNumber: 1 }),
    setStatus: (status) => setFilter({ ...filter, status, pageNumber: 1 }),
    goToPage: (page) => setFilter({ ...filter, pageNumber: page }),
    refetch: fetchOrders
  };
}
```

#### useServiceOrders()

```typescript
function useServiceOrders(options?: UseOrdersOptions): UseOrdersReturn {
  return useOrders({
    ...options,
    initialParams: {
      ...options?.initialParams,
      type: 'service'
    }
  });
}
```

### ProfilePage Component

```typescript
export function ProfilePage() {
  const [activeTab, setActiveTab] = useState('profile');
  
  // Orders Tab - Product orders
  const {
    orders: productOrders,
    isLoading: ordersLoading,
    error: ordersError
  } = useOrders({
    autoLoad: activeTab === 'orders',
    initialParams: { type: 'product' }
  });
  
  // Services Tab - Service orders
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
        {/* Display product orders */}
      </TabsContent>
      
      <TabsContent value="services">
        {/* Display service orders */}
      </TabsContent>
    </Tabs>
  );
}
```

---

## Service Categories Enum

```typescript
enum ServiceCategory {
  ANCESTOR_WORSHIP = 'ancestor-worship',      // Cúng Gia Tiên
  OPENING_CEREMONY = 'opening-ceremony',      // Lễ Khai Trương
  WEDDING = 'wedding',                        // Lễ Cưới Hỏi
  BUDDHA_WORSHIP = 'buddha-worship',          // Cúng Phật
  NEW_HOUSE = 'new-house',                    // Lễ Tân Gia
  FENG_SHUI_CONSULTATION = 'feng-shui-consultation'  // Tư Vấn Phong Thủy
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

---

## Error Handling

### API Errors

```typescript
// 400 Bad Request
{
  status: 400,
  message: "Invalid type value",
  errors: { type: ["Must be 'product' or 'service'"] }
}

// 404 Not Found
{
  status: 404,
  message: "Order not found"
}

// 500 Server Error
{
  status: 500,
  message: "Internal server error"
}
```

### Frontend Error Handling

```typescript
if (!error.response) {
  toast.error('Không thể kết nối đến server');
} else if (error.status === 400) {
  toast.error('Dữ liệu không hợp lệ');
} else if (error.status === 404) {
  toast.error('Không tìm thấy');
} else if (error.status >= 500) {
  toast.error('Lỗi server. Vui lòng thử lại sau');
}
```

---

## Testing Strategy

### Unit Testing

**Product Model**:
- Test ProductService.getServices()
- Test useServices() hook
- Test ServicesPage component

**Order Model**:
- Test OrderService.getOrders()
- Test OrderService.getServiceOrders()
- Test useOrders() hook
- Test useServiceOrders() hook
- Test ProfilePage Orders Tab
- Test ProfilePage Services Tab

### Integration Testing

- Test complete flow: Browse services → Add to cart → Checkout → View in ProfilePage
- Test ProductsPage still works (backward compatibility)
- Test filtering and pagination

### Test Framework

- **Unit Tests**: Vitest + React Testing Library
- **Integration Tests**: Cypress or Playwright

