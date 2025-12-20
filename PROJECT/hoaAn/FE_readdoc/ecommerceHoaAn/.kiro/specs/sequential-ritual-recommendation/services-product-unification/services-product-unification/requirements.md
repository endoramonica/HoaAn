# Requirements Document: Services-Product Unification

## Introduction

Hiện tại có **2 vấn đề riêng biệt** cần giải quyết:

### Vấn Đề 1: ServicesPage (Browse & Buy Services)
- ServicesPage sử dụng mock data để hiển thị danh sách services
- Không có API integration
- Không thể add to cart, wishlist

### Vấn Đề 2: ProfilePage (Order History)
- **Orders Tab**: Mock data (3 orders hardcoded)
- **Services Tab**: localStorage (ServiceBookingsTab)
- Không có API integration
- Service orders và product orders không thống nhất

## Giải Pháp

Chúng ta sẽ **thống nhất cả Product Model và Order Model**:

### 1. Product Model (Cho ServicesPage)
- Thêm `type='service'` vào Product model
- API: `GET /api/v1/Product?type=service`
- ServicesPage fetch services từ Product API
- User có thể browse, add to cart, wishlist

### 2. Order Model (Cho ProfilePage)
- Thêm `type='product'|'service'` vào Order model
- API: `GET /api/v1/Order/my-orders?type=service`
- ProfilePage fetch orders từ Order API
- Thống nhất product orders và service orders

**Lợi ích:**
- Giảm code duplication
- API integration đầy đủ
- Tái sử dụng logic filtering, pagination, error handling
- Dễ bảo trì và mở rộng
- Thống nhất UX

## Glossary

### Product Model (Catalog)
- **Product**: Một item có thể mua trong catalog (sản phẩm vật lý hoặc dịch vụ)
- **Service**: Một Product với `type = 'service'`
- **ProductListDto**: DTO chứa thông tin product/service từ backend
- **ProductFilterDto**: DTO chứa filter parameters (type, category, search, pagination)
- **ServicesPage**: Component hiển thị danh sách services để browse và mua

### Order Model (Transactions)
- **Order**: Một đơn hàng đã mua (product order hoặc service order)
- **Product Order**: Order với `type = 'product'`
- **Service Order**: Order với `type = 'service'`
- **OrderDetailDto**: DTO chứa thông tin chi tiết order từ backend
- **OrderFilterDto**: DTO chứa filter parameters (type, status, pagination)
- **ProfilePage**: Component hiển thị thông tin user và order history
- **ServiceBookingsTab**: Component hiện tại dùng localStorage (sẽ được thay thế)

### Common
- **Service Category**: Phân loại dịch vụ (cúng gia tiên, lễ khai trương, v.v.)

## Requirements

## Part 1: Product Model (ServicesPage - Browse & Buy)

### Requirement 1: Backend Product Model Enhancement

**User Story:** Là một backend developer, tôi muốn mở rộng Product model để hỗ trợ Services, để ServicesPage có thể hiển thị và bán services như products.

#### Acceptance Criteria

1. WHEN backend receives a request to create a Product, THE system SHALL accept an optional `type` field with values 'product' or 'service'
2. WHEN backend receives a request to create a Service (type='service'), THE system SHALL accept optional service-specific fields (serviceCategory, serviceDuration, rating)
3. WHEN backend receives a GET request to `/api/v1/Product?type=service`, THE system SHALL return only products where type='service'
4. WHEN backend receives a GET request to `/api/v1/Product?type=product`, THE system SHALL return only products where type='product'
5. WHEN backend receives a GET request to `/api/v1/Product` without type filter, THE system SHALL return all products regardless of type
6. WHEN backend stores existing products without type field, THE system SHALL default type to 'product' for backward compatibility

---

## Part 2: Order Model (ProfilePage - Order History)

### Requirement 2: Backend Order Model Enhancement

**User Story:** Là một backend developer, tôi muốn mở rộng Order model để hỗ trợ cả product orders và service orders, để ProfilePage có thể hiển thị lịch sử orders thống nhất.

#### Acceptance Criteria

1. WHEN backend receives a request to create an Order, THE system SHALL accept an optional `type` field with values 'product' or 'service'
2. WHEN backend receives a request to create a Service Order (type='service'), THE system SHALL accept optional service-specific fields (serviceCategory, serviceDuration, serviceLocation, serviceDate, serviceTime, serviceNotes)
3. WHEN backend receives a GET request to `/api/v1/Order/my-orders?type=service`, THE system SHALL return only orders where type='service'
4. WHEN backend receives a GET request to `/api/v1/Order/my-orders?type=product`, THE system SHALL return only orders where type='product'
5. WHEN backend receives a GET request to `/api/v1/Order/my-orders` without type filter, THE system SHALL return all orders regardless of type
6. WHEN backend stores existing orders without type field, THE system SHALL default type to 'product' for backward compatibility

### Requirement 3: Frontend Type Definitions (Product Model)

**User Story:** Là một frontend developer, tôi muốn có TypeScript types cho Product model mở rộng, để có type-safety khi làm việc với services.

#### Acceptance Criteria

1. WHEN ProductListDto is defined, THE system SHALL include optional `type` field with type 'product' | 'service'
2. WHEN ProductListDto is defined, THE system SHALL include optional service-specific fields (serviceCategory, serviceDuration, rating)
3. WHEN ProductFilterDto is defined, THE system SHALL include optional `type` filter parameter
4. WHEN ProductFilterDto is defined, THE system SHALL include optional `serviceCategory` filter parameter

### Requirement 4: Frontend Type Definitions (Order Model)

**User Story:** Là một frontend developer, tôi muốn có TypeScript types cho Order model mở rộng, để có type-safety khi làm việc với service orders.

#### Acceptance Criteria

1. WHEN OrderDetailDto is defined, THE system SHALL include optional `type` field with type 'product' | 'service'
2. WHEN OrderDetailDto is defined, THE system SHALL include optional service-specific fields (serviceCategory, serviceDuration, serviceLocation, serviceDate, serviceTime, serviceNotes)
3. WHEN OrderItemDTO is defined, THE system SHALL include optional `type` field to distinguish product items from service items
4. WHEN OrderFilterDto is defined, THE system SHALL include optional `type` filter parameter
5. WHEN OrderFilterDto is defined, THE system SHALL include optional `serviceCategory` filter parameter

### Requirement 5: API Service Layer Enhancement (Product Model)

**User Story:** Là một frontend developer, tôi muốn có service methods để fetch services từ Product API, để ServicesPage có thể hiển thị services.

#### Acceptance Criteria

1. WHEN `productService.getServices(filter)` is called, THE system SHALL call `/api/v1/Product?type=service` with appropriate filters
2. WHEN `productService.getServices()` is called with pagination params, THE system SHALL return paginated results with totalPages, currentPage, totalCount
3. WHEN `productService.getServices()` is called with serviceCategory filter, THE system SHALL only return services matching that category
4. WHEN `productService.getServices()` is called with search term, THE system SHALL search across service name and description
5. WHEN `productService.getServices()` fails, THE system SHALL throw an error with descriptive message

### Requirement 6: API Service Layer Enhancement (Order Model)

**User Story:** Là một frontend developer, tôi muốn có service methods để fetch orders từ Order API, để ProfilePage có thể hiển thị order history.

#### Acceptance Criteria

1. WHEN `orderService.getOrders(filter)` is called, THE system SHALL call `/api/v1/Order/my-orders` with appropriate filters
2. WHEN `orderService.getServiceOrders(filter)` is called, THE system SHALL call `/api/v1/Order/my-orders?type=service`
3. WHEN `orderService.getProductOrders(filter)` is called, THE system SHALL call `/api/v1/Order/my-orders?type=product`
4. WHEN `orderService.getOrders()` is called with pagination params, THE system SHALL return paginated results
5. WHEN `orderService.getOrders()` fails, THE system SHALL throw an error with descriptive message

### Requirement 7: React Hooks

**User Story:** Là một frontend developer, tôi muốn có custom hooks để fetch services và orders, để dễ dàng manage state.

#### Acceptance Criteria

1. WHEN `useServices()` hook is called, THE system SHALL return services list, loading state, error state, and pagination info
2. WHEN `useOrders()` hook is called, THE system SHALL return orders list, loading state, error state, and pagination info
3. WHEN `useServiceOrders()` hook is called, THE system SHALL return service orders only
4. WHEN hooks receive filter changes, THE system SHALL automatically refetch data
5. WHEN hooks encounter errors, THE system SHALL display error toast and maintain previous data

### Requirement 8: ServicesPage Refactoring

**User Story:** Là một user, tôi muốn trang Services hiển thị dịch vụ từ backend API, để có thể browse và mua services.

#### Acceptance Criteria

1. WHEN ServicesPage loads, THE system SHALL fetch services from backend using `useServices()` hook
2. WHEN user filters by service category, THE system SHALL update URL params and refetch services
3. WHEN user searches for a service, THE system SHALL update URL params and refetch services
4. WHEN user adds a service to cart, THE system SHALL call cart API and show success toast
5. WHEN services are loading, THE system SHALL display skeleton loaders
6. WHEN no services are found, THE system SHALL display empty state
7. WHEN an error occurs, THE system SHALL display error alert and allow user to retry

### Requirement 9: ProfilePage Orders Tab Refactoring

**User Story:** Là một user, tôi muốn xem lịch sử product orders từ backend API, để có thể track orders của mình.

#### Acceptance Criteria

1. WHEN ProfilePage Orders Tab loads, THE system SHALL fetch product orders using `useOrders()` hook with type='product'
2. WHEN user filters by status, THE system SHALL refetch orders with status filter
3. WHEN orders are loading, THE system SHALL display skeleton loaders
4. WHEN no orders are found, THE system SHALL display empty state
5. WHEN an error occurs, THE system SHALL display error alert

### Requirement 10: ProfilePage Services Tab Refactoring

**User Story:** Là một user, tôi muốn xem lịch sử service orders từ backend API, để có thể track service bookings của mình.

#### Acceptance Criteria

1. WHEN ProfilePage Services Tab loads, THE system SHALL fetch service orders using `useServiceOrders()` hook
2. WHEN user filters by service category, THE system SHALL refetch orders with category filter
3. WHEN user filters by status, THE system SHALL refetch orders with status filter
4. WHEN service orders are loading, THE system SHALL display skeleton loaders
5. WHEN no service orders are found, THE system SHALL display empty state with link to ServicesPage
6. WHEN an error occurs, THE system SHALL display error alert

### Requirement 11: Backward Compatibility

**User Story:** Là một developer, tôi muốn đảm bảo rằng refactoring không làm hỏng existing functionality.

#### Acceptance Criteria

1. WHEN ProductsPage loads, THE system SHALL only fetch products where type='product' or type is null
2. WHEN ProductsPage filters by category, THE system SHALL not include services in results
3. WHEN user navigates between pages, THE system SHALL maintain separate filter states
4. WHEN existing tests run, THE system SHALL pass all tests without modification

