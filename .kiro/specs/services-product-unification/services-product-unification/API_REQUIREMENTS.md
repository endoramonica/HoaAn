# API Requirements: Services-Product Unification

## 📋 Overview

Cần mở rộng **2 models**:
1. **Product Model** - Cho ServicesPage (browse & buy)
2. **Order Model** - Cho ProfilePage (order history)

---

## Part 1: Product Model

### Existing Product Endpoints (from Swagger)

#### GET /api/v1/Product
**Current**: Fetch all products
**Response**: `PaginatedResult<ProductListDto>`
**Query Params**: `pageNumber`, `pageSize`, `searchTerm`, `categoryId`, `sortBy`

**Issue**: No type filtering - returns all products mixed

### Required Changes for Product Model

#### 1. Extend ProductListDto

**Current Structure** (Assumed):
```csharp
public class ProductListDto {
    public string Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public decimal Price { get; set; }
    public string PrimaryImage { get; set; }
    public string CategoryName { get; set; }
    public int StockQuantity { get; set; }
    public bool InStock { get; set; }
}
```

**Required Changes**:
```csharp
public class ProductListDto {
    // Existing fields
    public string Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public decimal Price { get; set; }
    public string PrimaryImage { get; set; }
    public string CategoryName { get; set; }
    public int StockQuantity { get; set; }
    public bool InStock { get; set; }
    
    // NEW: Type discriminator
    public string Type { get; set; } // "product" | "service"
    
    // NEW: Service-specific fields
    public string? ServiceCategory { get; set; } // "ancestor-worship", etc.
    public string? ServiceDuration { get; set; } // "2-3 giờ"
    public double? Rating { get; set; } // 0-5
}
```

#### 2. Modify GET /api/v1/Product

**Enhanced**:
```
GET /api/v1/Product?type=service&serviceCategory=ancestor-worship&pageNumber=1&pageSize=10
```

**Query Parameters**:
| Parameter | Type | Required | Description |
|-----------|------|----------|-------------|
| type | string | No | Filter by type: "product", "service", or null (all) |
| serviceCategory | string | No | Filter by service category (only if type="service") |
| pageNumber | int | No | Page number (default: 1) |
| pageSize | int | No | Items per page (default: 10) |
| searchTerm | string | No | Search in name and description |
| categoryId | string | No | Filter by category ID |
| sortBy | string | No | Sort by: "name", "price", "newest" |
| isDescending | bool | No | Sort order (default: false) |

---

## Part 2: Order Model

### Existing Order Endpoints (from Swagger)

#### 1. GET /api/v1/Order/my-orders
**Current**: Fetch user's orders
**Response**: `PaginatedResult<OrderDetailDto>`
**Query Params**: `pageNumber`, `pageSize`

**Issue**: No type filtering - returns all orders mixed

#### 2. GET /api/v1/Order/{id}
**Current**: Fetch single order detail
**Response**: `OrderDetailDto`

#### 3. PUT /api/v1/Order/{orderId}/status
**Current**: Update order status
**Request**: `OrderStatusUpdateDTO`

#### 4. POST /api/v1/Order/{orderId}/cancel
**Current**: Cancel order

#### 5. GET /api/v1/Checkout/my-orders
**Current**: Fetch checkout orders (similar to Order/my-orders)

---

## 🔧 Required Changes

### 1. Extend OrderDetailDto Model

**Current Structure** (Assumed):
```csharp
public class OrderDetailDto {
    public string Id { get; set; }
    public DateTime OrderDate { get; set; }
    public string Status { get; set; }
    public decimal Total { get; set; }
    public List<OrderItemDTO> Items { get; set; }
    public string ShippingAddress { get; set; }
    public string ShippingMethod { get; set; }
    // ... other fields
}
```

**Required Changes**:
```csharp
public class OrderDetailDto {
    // Existing fields
    public string Id { get; set; }
    public DateTime OrderDate { get; set; }
    public string Status { get; set; }
    public decimal Total { get; set; }
    public List<OrderItemDTO> Items { get; set; }
    public string ShippingAddress { get; set; }
    public string ShippingMethod { get; set; }
    
    // NEW: Type discriminator
    public string Type { get; set; } // "product" | "service"
    
    // NEW: Service-specific fields
    public string? ServiceCategory { get; set; } // "ancestor-worship", "opening-ceremony", etc.
    public string? ServiceDuration { get; set; } // "2-3 giờ"
    public string? ServiceLocation { get; set; } // "123 Đường Láng, Hà Nội"
    public string? ServiceNotes { get; set; } // Customer notes
    public DateTime? ServiceDate { get; set; } // Scheduled service date
    public string? ServiceTime { get; set; } // Scheduled service time
}
```

### 2. Extend OrderItemDTO Model

**Current Structure** (Assumed):
```csharp
public class OrderItemDTO {
    public string Id { get; set; }
    public string ProductId { get; set; }
    public string ProductName { get; set; }
    public decimal Price { get; set; }
    public int Quantity { get; set; }
    public string? Image { get; set; }
}
```

**Required Changes**:
```csharp
public class OrderItemDTO {
    // Existing fields
    public string Id { get; set; }
    public string ProductId { get; set; }
    public string ProductName { get; set; }
    public decimal Price { get; set; }
    public int Quantity { get; set; }
    public string? Image { get; set; }
    
    // NEW: Type field
    public string Type { get; set; } // "product" | "service"
    
    // NEW: Service-specific fields (if type="service")
    public string? ServiceCategory { get; set; }
    public string? ServiceDuration { get; set; }
}
```

### 3. Create OrderFilterDto

**New DTO**:
```csharp
public class OrderFilterDto {
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    
    // Existing filters
    public string? Status { get; set; } // "pending", "processing", "shipped", "delivered", "cancelled"
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public string? SearchTerm { get; set; }
    
    // NEW: Type filter
    public string? Type { get; set; } // "product" | "service" | null (all)
    
    // NEW: Service-specific filters
    public string? ServiceCategory { get; set; } // "ancestor-worship", etc.
    
    // Sorting
    public string? SortBy { get; set; } // "date", "total", "status"
    public bool IsDescending { get; set; } = true;
}
```

---

## 📡 API Endpoint Changes

### 1. GET /api/v1/Order/my-orders (MODIFIED)

**Current**:
```
GET /api/v1/Order/my-orders?pageNumber=1&pageSize=10
```

**Enhanced**:
```
GET /api/v1/Order/my-orders?pageNumber=1&pageSize=10&type=service&serviceCategory=ancestor-worship&status=delivered
```

**Query Parameters**:
| Parameter | Type | Required | Description |
|-----------|------|----------|-------------|
| pageNumber | int | No | Page number (default: 1) |
| pageSize | int | No | Items per page (default: 10) |
| type | string | No | Filter by type: "product", "service", or null (all) |
| serviceCategory | string | No | Filter by service category (only if type="service") |
| status | string | No | Filter by status: "pending", "processing", "shipped", "delivered", "cancelled" |
| startDate | datetime | No | Filter orders from this date |
| endDate | datetime | No | Filter orders until this date |
| searchTerm | string | No | Search in order ID or product names |
| sortBy | string | No | Sort by: "date", "total", "status" |
| isDescending | bool | No | Sort order (default: true) |

**Response**:
```json
{
  "data": [
    {
      "id": "DH001",
      "orderDate": "2025-01-10T00:00:00Z",
      "status": "delivered",
      "total": 2890000,
      "type": "service",
      "serviceCategory": "ancestor-worship",
      "serviceDuration": "2-3 giờ",
      "serviceLocation": "123 Đường Láng, Hà Nội",
      "serviceDate": "2025-01-15T00:00:00Z",
      "serviceTime": "09:00",
      "items": [
        {
          "id": "item1",
          "productId": "svc001",
          "productName": "Mâm Cúng Trọn Gói Cao Cấp",
          "price": 2890000,
          "quantity": 1,
          "type": "service",
          "serviceCategory": "ancestor-worship",
          "image": "https://..."
        }
      ]
    }
  ],
  "totalCount": 15,
  "totalPages": 2,
  "currentPage": 1,
  "pageSize": 10
}
```

### 2. GET /api/v1/Order/my-orders/services (NEW - Optional)

**Purpose**: Convenience endpoint for fetching only service orders

```
GET /api/v1/Order/my-orders/services?pageNumber=1&pageSize=10&serviceCategory=ancestor-worship
```

**Query Parameters**:
| Parameter | Type | Required | Description |
|-----------|------|----------|-------------|
| pageNumber | int | No | Page number (default: 1) |
| pageSize | int | No | Items per page (default: 10) |
| serviceCategory | string | No | Filter by service category |
| status | string | No | Filter by status |
| startDate | datetime | No | Filter orders from this date |
| endDate | datetime | No | Filter orders until this date |
| sortBy | string | No | Sort by: "date", "total", "status" |
| isDescending | bool | No | Sort order (default: true) |

**Response**: Same as `/api/v1/Order/my-orders` but only service orders

### 3. GET /api/v1/Order/my-orders/products (NEW - Optional)

**Purpose**: Convenience endpoint for fetching only product orders

```
GET /api/v1/Order/my-orders/products?pageNumber=1&pageSize=10
```

**Response**: Same as `/api/v1/Order/my-orders` but only product orders

### 4. GET /api/v1/Order/{id} (MODIFIED)

**Current**: No changes needed to endpoint
**Response**: Updated `OrderDetailDto` with new fields

### 5. POST /api/v1/Order (MODIFIED - if creating orders from API)

**Purpose**: Create new order (for checkout)

**Request**:
```json
{
  "items": [
    {
      "productId": "svc001",
      "quantity": 1,
      "type": "service",
      "serviceCategory": "ancestor-worship",
      "serviceDate": "2025-01-15T00:00:00Z",
      "serviceTime": "09:00",
      "serviceLocation": "123 Đường Láng, Hà Nội",
      "serviceNotes": "Vui lòng đến sớm 15 phút"
    }
  ],
  "shippingAddress": "123 Đường Láng, Hà Nội",
  "shippingMethod": "standard",
  "paymentMethod": "credit-card"
}
```

**Response**: `OrderDetailDto` with new fields

---

## 🔄 Service Categories Enum

**Backend**:
```csharp
public enum ServiceCategory {
    [Display(Name = "Cúng Gia Tiên")]
    AncestorWorship = 0,
    
    [Display(Name = "Lễ Khai Trương")]
    OpeningCeremony = 1,
    
    [Display(Name = "Lễ Cưới Hỏi")]
    Wedding = 2,
    
    [Display(Name = "Cúng Phật")]
    BuddhaWorship = 3,
    
    [Display(Name = "Lễ Tân Gia")]
    NewHouse = 4,
    
    [Display(Name = "Tư Vấn Phong Thủy")]
    FengShuiConsultation = 5
}
```

**Frontend** (TypeScript):
```typescript
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

---

## 🗂️ Data Migration

### Migration Script (Backend)

```sql
-- Add new columns to Orders table
ALTER TABLE Orders ADD COLUMN Type NVARCHAR(50) DEFAULT 'product';
ALTER TABLE Orders ADD COLUMN ServiceCategory NVARCHAR(100) NULL;
ALTER TABLE Orders ADD COLUMN ServiceDuration NVARCHAR(100) NULL;
ALTER TABLE Orders ADD COLUMN ServiceLocation NVARCHAR(500) NULL;
ALTER TABLE Orders ADD COLUMN ServiceNotes NVARCHAR(1000) NULL;
ALTER TABLE Orders ADD COLUMN ServiceDate DATETIME NULL;
ALTER TABLE Orders ADD COLUMN ServiceTime NVARCHAR(50) NULL;

-- Add new columns to OrderItems table
ALTER TABLE OrderItems ADD COLUMN Type NVARCHAR(50) DEFAULT 'product';
ALTER TABLE OrderItems ADD COLUMN ServiceCategory NVARCHAR(100) NULL;
ALTER TABLE OrderItems ADD COLUMN ServiceDuration NVARCHAR(100) NULL;

-- Set Type='product' for all existing orders
UPDATE Orders SET Type = 'product' WHERE Type IS NULL;
UPDATE OrderItems SET Type = 'product' WHERE Type IS NULL;

-- Create index for filtering
CREATE INDEX IX_Orders_Type ON Orders(Type);
CREATE INDEX IX_Orders_ServiceCategory ON Orders(ServiceCategory);
CREATE INDEX IX_OrderItems_Type ON OrderItems(Type);
```

---

## 🧪 Testing Scenarios

### Test Case 1: Filter Service Orders
```
GET /api/v1/Order/my-orders?type=service
Expected: Only orders with type='service'
```

### Test Case 2: Filter Product Orders
```
GET /api/v1/Order/my-orders?type=product
Expected: Only orders with type='product'
```

### Test Case 3: Filter by Service Category
```
GET /api/v1/Order/my-orders?type=service&serviceCategory=ancestor-worship
Expected: Only service orders with serviceCategory='ancestor-worship'
```

### Test Case 4: Pagination
```
GET /api/v1/Order/my-orders?pageNumber=2&pageSize=5
Expected: Items 6-10 from total results
```

### Test Case 5: Combined Filters
```
GET /api/v1/Order/my-orders?type=service&status=delivered&serviceCategory=ancestor-worship&pageNumber=1&pageSize=10
Expected: Delivered service orders for ancestor worship, paginated
```

---

## 📝 Implementation Checklist for Backend

- [ ] Add `Type` field to Order entity
- [ ] Add `ServiceCategory` field to Order entity
- [ ] Add `ServiceDuration` field to Order entity
- [ ] Add `ServiceLocation` field to Order entity
- [ ] Add `ServiceNotes` field to Order entity
- [ ] Add `ServiceDate` field to Order entity
- [ ] Add `ServiceTime` field to Order entity
- [ ] Add `Type` field to OrderItem entity
- [ ] Add `ServiceCategory` field to OrderItem entity
- [ ] Add `ServiceDuration` field to OrderItem entity
- [ ] Create `OrderFilterDto` class
- [ ] Update `OrderDetailDto` to include new fields
- [ ] Update `OrderItemDTO` to include new fields
- [ ] Update `GET /api/v1/Order/my-orders` to support type filtering
- [ ] Create `GET /api/v1/Order/my-orders/services` endpoint (optional)
- [ ] Create `GET /api/v1/Order/my-orders/products` endpoint (optional)
- [ ] Update `POST /api/v1/Order` to accept service fields
- [ ] Create database migration
- [ ] Update existing orders: set Type='product'
- [ ] Add validation for service fields
- [ ] Add unit tests for new endpoints
- [ ] Update Swagger documentation
- [ ] Test with Postman/Insomnia

---

## 🔗 Frontend Integration Points

### 1. OrderService.ts
```typescript
async getOrders(filter: OrderFilterDto): Promise<PaginatedResult<OrderDetailDto>>
async getServiceOrders(filter?: OrderFilterDto): Promise<PaginatedResult<OrderDetailDto>>
async getProductOrders(filter?: OrderFilterDto): Promise<PaginatedResult<OrderDetailDto>>
```

### 2. useOrders Hook
```typescript
const { orders, isLoading, error, totalPages, currentPage, setType, setServiceCategory } = useOrders();
```

### 3. ProfilePage Component
```typescript
// Orders Tab
const { orders: productOrders } = useOrders({ type: 'product' });

// Services Tab
const { orders: serviceOrders } = useOrders({ type: 'service' });
```

---

## ⚠️ Important Notes

1. **Backward Compatibility**: All existing orders should have `Type='product'` to maintain backward compatibility
2. **Validation**: Service fields should only be required when `Type='service'`
3. **Indexing**: Add database indexes on `Type` and `ServiceCategory` for performance
4. **Pagination**: Always paginate results to avoid loading too much data
5. **Error Handling**: Return meaningful error messages for invalid filters
6. **Documentation**: Update Swagger/OpenAPI documentation with new fields and parameters

