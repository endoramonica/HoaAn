# VietCommerce System Context Prompt

## 📋 Tổng Quan Hệ Thống

**VietCommerce** là một hệ thống E-commerce Backend được xây dựng bằng **.NET 9.0** với kiến trúc **Clean Architecture** (Layered Architecture). Hệ thống hỗ trợ multi-tenant, multi-store với đầy đủ các tính năng quản lý sản phẩm, đơn hàng, thanh toán, quản lý nhân viên, và nhiều module khác.

---

## 🏗️ Kiến Trúc Tổng Thể

### Cấu Trúc Project (Solution Structure)

```
VietCommerce/
├── VietCommerce.Api/              # Main API - End-user facing API (Route: /api/v1/*)
├── BE/                            # Admin API - Admin/Staff management API (Route: /api/*)
│   └── VietCommerce.AdminApi.csproj
├── VietCommerce.Application/      # Application Layer - Business logic & Services
│   ├── Services/
│   │   ├── Services/              # Core services
│   │   ├── Admin_Staff_Manager/   # Admin-specific services
│   │   └── EndUser/               # End-user services
│   ├── Mappings/                  # AutoMapper profiles
│   ├── Validators/                  # FluentValidation validators
│   ├── Extensions/                # DI registration extensions
│   ├── Middleware/                # Custom middleware
│   ├── Authorization/             # Authorization handlers
│   └── Attributes/                # Custom attributes
├── VietCommerce.Core/             # Domain Layer - Entities, DTOs, Enums, Helpers
│   ├── Entities/                  # Domain entities
│   ├── DTOs/                      # Data Transfer Objects
│   ├── Enums/                     # Domain enumerations
│   ├── Helpers/                   # Utility classes
│   ├── Models/                    # Value objects, View models
│   ├── Common/                    # Common classes (BaseEntity, ApiResponse, etc.)
│   └── Mappers/                   # Manual mappers (if any)
├── VietCommerce.Data/            # Data Access Layer - EF Core, Repositories, Migrations
│   ├── Context/                   # DbContext
│   ├── Repositories/              # Repository implementations
│   ├── Migrations/                # EF Core migrations
│   └── Seeds/Seeders/             # Database seeders
└── VietCommerce.Tests/            # Unit Tests
```

### Kiến Trúc Layers

1. **API Layer** (`VietCommerce.Api`, `BE/VietCommerce.AdminApi`)
   - Controllers (REST API endpoints)
   - Middleware (Exception handling, Permission checking)
   - Configuration (Program.cs, appsettings.json)
   - **Route Conventions**:
     - Main API: `/api/v1/[controller]`
     - Admin API: `/api/[controller]`

2. **Application Layer** (`VietCommerce.Application`)
   - Services (Business logic)
   - Mappings (AutoMapper profiles)
   - Validators (FluentValidation)
   - Extensions (Service registration, Authorization, Repository registration)
   - Middleware (Permission middleware)
   - Authorization (Permission & Role handlers)
   - Attributes (RequirePermission, RequireRole)
   - Helpers (SessionIdHelper, GoogleSettings, JwtHelper via DI)

3. **Domain Layer** (`VietCommerce.Core`)
   - Entities (Domain models)
   - DTOs (Data Transfer Objects) - Organized by feature/module
   - Enums (Domain enumerations) - Organized by feature/module
   - Helpers (Utility classes - JwtHelper, SessionIdHelper, etc.)
   - Models (Value objects, View models - ApiResponse<T>, PaginatedResult<T>, etc.)
   - Common (BaseEntity, AuditableEntity, ISoftDelete, Attributes, Constants)
   - Mappers (Manual mappers if any)

4. **Data Layer** (`VietCommerce.Data`)
   - Context (AppDbContext - EF Core context)
   - Repositories (Data access patterns - Generic & Specific repositories)
   - Migrations (Database schema - EF Core migrations)
   - Seeds/Seeders (Initial data - RBACSeeder, ProductAnalyticsSeeder, CartOrderPermissionSeeder)
   - Tools (EF Core tools configuration)

---

## 🗄️ Database Architecture

### Database: SQL Server với Entity Framework Core 9.0

### Core Entities & Relationships

#### 1. **Organization (Multi-Tenant Architecture)**
- **Tenant**: Công ty/tổ chức chính
  - Quan hệ 1-nhiều với: Stores, Users, Customers, Products, Orders
- **Store**: Cửa hàng thuộc Tenant
  - Quan hệ 1-nhiều với: Users, Customers, Products, Categories, Orders, Inventories, Employees, Shifts

#### 2. **User Management & RBAC (Role-Based Access Control)**
- **User**: Người dùng hệ thống
  - Quan hệ với: Store, Manager (self-reference), Employee (1-1)
  - Hỗ trợ: Soft delete, Audit fields, OAuth providers (Google, Facebook)
- **Role**: Vai trò (Admin, Manager, Staff, Customer, etc.)
- **Permission**: Quyền hạn chi tiết (product.create, order.view, etc.)
- **UserRole**: Nhiều-nhiều giữa User và Role
- **RolePermission**: Nhiều-nhiều giữa Role và Permission
- **RefreshToken**: Quản lý refresh tokens cho JWT

#### 3. **Customer Management**
- **Customer**: Khách hàng
  - Quan hệ: Tenant, Store, User (optional), Addresses, Orders, Carts, CRM Interactions
- **CustomerAddress**: Địa chỉ giao hàng
- **UserAddress**: Địa chỉ của User (staff/admin)

#### 4. **Product Management**
- **Category**: Danh mục sản phẩm (hierarchical - có ParentId)
- **Product**: Sản phẩm
  - Quan hệ: Store, Category, Images, Prices, Inventories, OrderItems, CartItems
  - Hỗ trợ: SKU, Slug, Code (unique), Soft delete
- **ProductImage**: Hình ảnh sản phẩm (nhiều ảnh, có thứ tự)
- **ProductPrice**: Giá sản phẩm (có thể có nhiều giá theo thời gian)
- **ProductFavorite**: Sản phẩm yêu thích (Wishlist)
- **ProductView**: Lượt xem sản phẩm (analytics)
- **ProductReview**: Đánh giá sản phẩm

#### 5. **Inventory Management**
- **Inventory**: Tồn kho theo Product + Store
  - Unique constraint: (ProductId, StoreId)
  - Quan hệ: Product, Store, InventoryMovements
- **InventoryMovement**: Lịch sử nhập/xuất kho
  - Types: IN, OUT, ADJUSTMENT, RESERVED, RELEASED
  - Quan hệ: Inventory, Order (optional), StockTransfer (optional), User (PerformedBy)

#### 6. **Order Management**
- **Order**: Đơn hàng
  - Quan hệ: Store, Customer (optional), CreatedByUser, OrderItems, Payments, OrderStatusHistories, OrderShipping
  - Auto-generate OrderNumber: `ORD{yyyyMMdd}-{8-char-GUID}`
- **OrderItem**: Chi tiết đơn hàng
  - Auto-calculate: TotalPrice = Quantity × UnitPrice
- **OrderStatusHistory**: Lịch sử thay đổi trạng thái đơn hàng
- **OrderShipping**: Thông tin vận chuyển (1-1 với Order)

#### 7. **Cart Management**
- **Cart**: Giỏ hàng
  - Hỗ trợ: User (authenticated) hoặc SessionId (guest)
  - Quan hệ: User (optional), Customer (optional), CartItems
- **CartItem**: Sản phẩm trong giỏ hàng

#### 8. **Payment Management**
- **PaymentMethod**: Phương thức thanh toán
- **Payment**: Thanh toán
  - Quan hệ: Order, PaymentMethod, PaymentTransactions
- **PaymentTransaction**: Giao dịch thanh toán (có thể nhiều transaction cho 1 payment)

#### 9. **Marketing**
- **Campaign**: Chiến dịch marketing
  - Quan hệ: Store, Creator (User), Promotions
- **Promotion**: Khuyến mãi
  - Quan hệ: Campaign, PromotionProducts
- **PromotionProduct**: Sản phẩm áp dụng khuyến mãi (many-to-many)

#### 10. **Notifications**
- **Notification**: Thông báo
  - Quan hệ: User, NotificationTemplate
- **NotificationTemplate**: Template thông báo

#### 11. **CRM (Customer Relationship Management)**
- **CRMInteraction**: Tương tác với khách hàng
  - Quan hệ: Customer, CreatedByUser

#### 12. **Logistics**
- **Supplier**: Nhà cung cấp
  - Quan hệ: StockTransfers, Products (many-to-many)
- **StockTransfer**: Chuyển kho
  - Quan hệ: Supplier, RequestedByUser, ApprovedByUser, TransferItems, InventoryMovements
- **TransferItem**: Chi tiết chuyển kho

#### 13. **HRM (Human Resource Management)**
- **Employee**: Nhân viên
  - Quan hệ: User (1-1), Store, Manager (self-reference), Performance, LeaveRequests, WorkSchedules
- **PerformanceMetric**: Chỉ số hiệu suất
- **LeaveRequest**: Đơn xin nghỉ
- **WorkSchedule**: Lịch làm việc
- **Shift**: Ca làm việc
  - Quan hệ: Store, Staff (User)

#### 14. **Tasks**
- **WorkTask**: Công việc
  - Quan hệ: AssignedToUser, AssignedByUser

#### 15. **Audit**
- **AuditLog**: Nhật ký audit
  - Quan hệ: User

### Database Features

- **Soft Delete**: Entities implement `ISoftDelete` interface
  - Global query filter tự động ẩn deleted records
  - Fields: `IsDeleted`, `DeletedAt`, `DeletedBy`
- **Audit Fields**: Entities implement `AuditableEntity` hoặc `BaseEntity`
  - Fields: `CreatedAt`, `CreatedBy`, `UpdatedAt`, `UpdatedBy`
- **Tenant Isolation**: Entities có `TenantId` hoặc `StoreId` để phân tách dữ liệu
- **Indexes**: Tối ưu performance với indexes trên các cột thường query
- **Precision**: Decimal fields có precision (18,2) cho tiền tệ

---

## 🔐 Authentication & Authorization

### JWT Authentication

- **JWT Settings**: Cấu hình trong `appsettings.json`
  - Issuer: `VietCommerce.Api` hoặc `VietCommerce.AdminApi`
  - Audience: `VietCommerce.Client` hoặc `VietCommerce.AdminClient`
  - Key: Symmetric key từ configuration
  - ClockSkew: Zero (không cho phép sai lệch thời gian)

### Authorization: RBAC (Role-Based Access Control)

#### 1. **Role-Based Authorization**
- Roles: Admin, SuperAdmin, Manager, Staff, Customer, etc.
- Policies: `AdminOnly`, `AdminOrModerator`, `Role:{RoleName}`
- Handler: `RoleAuthorizationHandler` - check roles từ JWT claims

#### 2. **Permission-Based Authorization**
- Permissions: Granular permissions (e.g., `product.create`, `order.view`, `inventory.manage`)
- Policies: Dynamic policies cho mỗi permission
- Handler: `PermissionAuthorizationHandler` - check permissions từ database (cached in Redis)
- Caching: Permissions được cache trong Redis để tối ưu performance

#### 3. **Attributes**
- `[RequirePermission("permission.name")]`: Yêu cầu permission cụ thể
- `[RequireRole("RoleName")]`: Yêu cầu role cụ thể
- `[Authorize]`: Yêu cầu authentication

#### 4. **Middleware**
- `PermissionMiddleware`: Attach user context (UserId, Roles, Permissions) vào HttpContext.Items
- Chạy sau `UseAuthorization()`

### Authentication Flow

1. **Login**: Email/Password hoặc OAuth (Google)
2. **JWT Generation**: Tạo Access Token + Refresh Token
3. **Token Storage**: Refresh Token lưu trong database
4. **Request**: Client gửi `Authorization: Bearer {token}`
5. **Validation**: JWT middleware validate token
6. **Authorization**: Permission/Role handlers check quyền
7. **Context**: User info được attach vào HttpContext

---

## 🚀 API Endpoints

### Main API (`VietCommerce.Api`) - Base Route: `/api/v1`

#### Authentication (`/api/v1/auth`)
- `POST /api/v1/auth/register` - Đăng ký tài khoản
- `GET /api/v1/auth/verify-email` - Xác thực email (query: token)
- `POST /api/v1/auth/login` - Đăng nhập (Email/Password)
- `POST /api/v1/auth/login/google` - Đăng nhập với Google OAuth
- `POST /api/v1/auth/refresh-token` - Refresh JWT token
- `POST /api/v1/auth/logout` - Đăng xuất (Authorize required)
- `POST /api/v1/auth/change-password` - Đổi mật khẩu (Authorize required)
- `POST /api/v1/auth/forgot-password` - Quên mật khẩu
- `POST /api/v1/auth/reset-password` - Đặt lại mật khẩu

#### Products (`/api/v1/products`)
- `GET /api/v1/products` - Danh sách sản phẩm (có filter, pagination) - Public
- `GET /api/v1/products/{id}` - Chi tiết sản phẩm - Public
- `GET /api/v1/products/slug/{slug}` - Chi tiết sản phẩm theo slug - Public
- `GET /api/v1/products/store/{storeId}` - Sản phẩm theo store - Public
- `GET /api/v1/products/category/{categoryId}` - Sản phẩm theo category - Public
- `GET /api/v1/products/{id}/stock` - Kiểm tra tồn kho - Public
- `POST /api/v1/products` - Tạo sản phẩm (Authorize + permission required)
- `PUT /api/v1/products/{id}` - Cập nhật sản phẩm (Authorize + permission required)
- `DELETE /api/v1/products/{id}` - Xóa sản phẩm (soft delete) (Authorize + permission required)
- `PATCH /api/v1/products/{id}/stock` - Cập nhật tồn kho (Authorize + permission required)
- `PATCH /api/v1/products/{id}/active` - Bật/tắt sản phẩm (Authorize + permission required)
- `PATCH /api/v1/products/{id}/featured` - Đặt featured (Authorize + permission required)
- `POST /api/v1/products/{id}/view` - Tăng lượt xem - Public
- `POST /api/v1/products/{id}/favorite` - Toggle yêu thích (Authorize required)
- `GET /api/v1/products/favorites` - Danh sách sản phẩm yêu thích (Authorize required)

#### Categories (`/api/categories` hoặc `/api/v1/categories`)
- `GET /api/categories` - Danh sách danh mục - Public
- `GET /api/categories/{id}` - Chi tiết danh mục - Public
- `POST /api/categories` - Tạo danh mục (Authorize required)
- `PUT /api/categories/{id}` - Cập nhật danh mục (Authorize required)
- `DELETE /api/categories/{id}` - Xóa danh mục (Authorize required)
- `PATCH /api/categories/{id}/soft-delete` - Soft delete danh mục (Authorize required)

#### Cart (`/api/v1/cart`)
**User Cart (Authenticated):**
- `GET /api/v1/cart` - Lấy giỏ hàng (Authorize required)
- `GET /api/v1/cart/summary` - Tóm tắt giỏ hàng (Authorize required)
- `POST /api/v1/cart/add` - Thêm sản phẩm vào giỏ (Authorize required)
- `PUT /api/v1/cart/update-item` - Cập nhật số lượng (Authorize required)
- `DELETE /api/v1/cart/items/{cartItemId}` - Xóa sản phẩm (Authorize required)
- `DELETE /api/v1/cart/clear` - Xóa toàn bộ giỏ hàng (Authorize required)
- `POST /api/v1/cart/validate` - Validate giỏ hàng trước checkout (Authorize required)
- `GET /api/v1/cart/item-count` - Số lượng sản phẩm trong giỏ (Authorize required)
- `GET /api/v1/cart/items/{cartItemId}` - Chi tiết cart item (Authorize required)

**Guest Cart (Anonymous):**
- `GET /api/v1/cart/guest` - Lấy giỏ hàng guest - Public
- `GET /api/v1/cart/guest/summary` - Tóm tắt giỏ hàng guest - Public
- `POST /api/v1/cart/guest/add` - Thêm sản phẩm vào giỏ guest - Public
- `PUT /api/v1/cart/guest/items/{cartItemId}` - Cập nhật giỏ hàng guest - Public
- `DELETE /api/v1/cart/guest/items/{cartItemId}` - Xóa sản phẩm khỏi giỏ guest - Public
- `DELETE /api/v1/cart/guest/clear` - Xóa toàn bộ giỏ hàng guest - Public
- `POST /api/v1/cart/guest/validate` - Validate giỏ hàng guest - Public
- `GET /api/v1/cart/guest/item-count` - Số lượng sản phẩm trong giỏ guest - Public

**Cart Utilities:**
- `POST /api/v1/cart/merge` - Merge guest cart vào user cart (Authorize required)
- `POST /api/v1/cart/coupon/apply` - Áp dụng mã giảm giá (Authorize required)
- `POST /api/v1/cart/coupon/remove` - Xóa mã giảm giá (Authorize required)
- `PUT /api/v1/cart/shipping` - Cập nhật thông tin vận chuyển (Authorize required)

#### Orders (`/api/v1/orders`)
- `GET /api/v1/orders/{id}` - Chi tiết đơn hàng (Authorize required)
- `GET /api/v1/orders/my-orders` - Đơn hàng của user hiện tại (Authorize required)
- `GET /api/v1/orders` - Tất cả đơn hàng (Admin/Seller role required)
- `GET /api/v1/orders/search` - Tìm kiếm đơn hàng (query: q) (Authorize required)
- `GET /api/v1/orders/status/{status}` - Đơn hàng theo trạng thái (Admin/Seller role required)
- `GET /api/v1/orders/recent` - Đơn hàng gần đây (Authorize required)
- `GET /api/v1/orders/{orderId}/status-history` - Lịch sử thay đổi trạng thái (Authorize required)
- `PUT /api/v1/orders/{orderId}/status` - Cập nhật trạng thái đơn hàng (Admin/Seller role required)
- `GET /api/v1/orders/{orderId}/can-change-status` - Kiểm tra có thể đổi trạng thái (Authorize required)
- `GET /api/v1/orders/stats/count` - Thống kê số lượng đơn hàng (Admin/Seller role required)
- `GET /api/v1/orders/stats/revenue` - Thống kê doanh thu (Admin/Seller role required)
- `POST /api/v1/orders/bulk-update-status` - Cập nhật trạng thái hàng loạt (Admin/Seller role required)

#### Checkout (`/api/v1/checkout`)
- `POST /api/v1/checkout/process` - Thanh toán từ giỏ hàng (Authorize required)
  - Tạo Order từ Cart
  - Kiểm tra tồn kho
  - Tạo OrderShipping
  - Clear cart sau khi thành công
- `GET /api/v1/checkout/{orderId}` - Chi tiết đơn hàng sau checkout (Authorize required)
- `GET /api/v1/checkout/my-orders` - Lịch sử đơn hàng của user (Authorize required)
- `POST /api/v1/checkout/{orderId}/cancel` - Hủy đơn hàng (Authorize required)

#### Wishlist (`/api/v1/wishlist`)
- `GET /api/v1/wishlist` - Danh sách yêu thích (Authorize required)
- `POST /api/v1/wishlist/add` - Thêm vào yêu thích (Authorize required)
- `DELETE /api/v1/wishlist/remove/{productId}` - Xóa khỏi yêu thích (Authorize required)

#### Users (`/api/v1/users`)
- `GET /api/v1/users` - Danh sách users (Admin role required)
- `GET /api/v1/users/{id}` - Chi tiết user (Admin role required)
- `PUT /api/v1/users/{id}` - Cập nhật user (Admin role required)
- `POST /api/v1/users/{id}/deactivate` - Vô hiệu hóa user (Admin role required)

#### Customer Addresses (`/api/v1/customer/addresses`)
- `GET /api/v1/customer/addresses` - Danh sách địa chỉ của customer (Authorize required)
- `POST /api/v1/customer/addresses` - Tạo địa chỉ mới (Authorize required)
- `PUT /api/v1/customer/addresses/{id}` - Cập nhật địa chỉ (Authorize required)
- `DELETE /api/v1/customer/addresses/{id}` - Xóa địa chỉ (Authorize required)

#### Roles & Permissions (`/api/v1/roles`, `/api/v1/permissions`, `/api/v1/user-roles`)
- Controllers hiện tại chưa có endpoints (cần implement)

### Admin API (`BE/VietCommerce.AdminApi`) - Base Route: `/api`

#### Inventory Management (`/api/inventory`)
- `GET /api/inventory/check-stock` - Kiểm tra tồn kho (query: storeId, productId, requiredQuantity) (Authorize required)
- `POST /api/inventory/bulk-check-stock` - Kiểm tra tồn kho hàng loạt (query: storeId, body: productIds) (Authorize required)
- `GET /api/inventory/store/{storeId}/product/{productId}` - Chi tiết tồn kho (Authorize required)
- `POST /api/inventory/store/{storeId}/adjust` - Điều chỉnh tồn kho (Authorize required)
- `POST /api/inventory/store/{storeId}/reserve` - Đặt trước tồn kho (Authorize required)
- `POST /api/inventory/store/{storeId}/release-reserved` - Giải phóng tồn kho đã dự trữ (Authorize required)
- `GET /api/inventory/{inventoryId}/movements` - Lịch sử chuyển động kho (Authorize required)
- `GET /api/inventory/store/{storeId}/low-stock` - Danh sách sản phẩm tồn kho thấp (Authorize required)

#### Current User (`/api/current-user`)
- `GET /api/current-user` - Thông tin user hiện tại kèm permissions (Authorize required)
- `POST /api/current-user/has-permission/{permission}` - Kiểm tra permission (Authorize required)
- `GET /api/current-user/details` - Chi tiết user hiện tại (Authorize required)

#### Addresses (`/api/addresses`)
- `GET /api/addresses` - Danh sách địa chỉ (Admin, permission required)
- `GET /api/addresses/{id}` - Chi tiết địa chỉ (Admin, permission required)
- `PUT /api/addresses/{id}` - Cập nhật địa chỉ (Admin, permission required)
- `DELETE /api/addresses/{id}` - Xóa địa chỉ (Admin, permission required)

---

## 🔧 Services & Business Logic

### Core Services

#### 1. **AuthService**
- Đăng ký, đăng nhập (Email/Password, Google OAuth)
- JWT token generation & refresh
- Password reset/change
- Brute-force protection với Redis (exponential backoff)
- Guest cart merge khi login

#### 2. **ProductService**
- CRUD sản phẩm
- Product filtering, searching, pagination
- Product images management
- Product prices management
- Product statistics

#### 3. **CartService**
- Add/Update/Remove items
- Hỗ trợ cả User (authenticated) và SessionId (guest)
- Auto-merge guest cart khi login
- Stock validation

#### 4. **CheckoutService**
- Process checkout từ cart
- Stock validation
- Order creation
- OrderShipping creation
- Cart clearing

#### 5. **OrderService**
- CRUD orders
- Order status management
- Order filtering, pagination
- Order statistics

#### 6. **CategoryService**
- CRUD categories
- Hierarchical category tree
- Category products listing

#### 7. **PermissionService**
- Check user permissions (cached in Redis)
- Bulk permission checking
- Permission cache invalidation

#### 8. **InventoryService** (Admin - `Admin_Staff_Manager/InventoryService`)
- Stock management
- Inventory adjustments
- Stock reservation/release
- Bulk stock checking
- Low stock alerts
- Inventory movements history

#### 9. **WishlistService** (EndUser - `EndUser/WishlistService`)
- Add/Remove products from wishlist
- Get user wishlist

#### 10. **AddressService**
- CRUD địa chỉ
- Customer address management
- User address management
- Address validation

#### 11. **UserService**
- CRUD users
- User profile management
- User deactivation
- User search & pagination

#### 12. **RoleService**
- Role management
- Role assignment
- Role-based access control

#### 13. **ProductStatsService**
- Product statistics
- Product analytics
- View tracking

#### 14. **ProductImageService**
- Product image management
- Image upload
- Image ordering
- Main image selection

#### 15. **FileUploadService**
- File upload handling
- Image processing
- File validation

#### 16. **RedisLoginAttemptService** (`ILoginAttemptService`)
- Brute-force protection
- Login attempt tracking với Redis
- Exponential backoff
- Lockout mechanism

#### 17. **RedisCacheService** (`ICacheService`)
- Redis caching implementation
- TTL management
- Cache invalidation
- Pub/Sub support

#### 18. **PermissionCacheInvalidationService** (Hosted Service)
- Background service cho cache invalidation
- Pub/Sub listener
- Automatic cache refresh

#### 19. **CurrentUserService** (`ICurrentUser`)
- Get current user context
- Permission checking
- Role checking
- Store context

#### 20. **GenericServices<T>**
- Generic CRUD operations
- Reusable service base

### Service Pattern

- **BaseService**: Base class cho tất cả services
  - Logging (ILogger)
  - Caching (ICacheService - Redis)
  - Exception handling
  - ApiResponse pattern
  - Version: 2.0
  - Features: Automatic exception handling, Redis caching with TTL, Standardized logging, Pub/Sub support

- **Service Organization**:
  - `Services/Services/` - Core application services
  - `Services/Admin_Staff_Manager/` - Admin-specific services
  - `Services/EndUser/` - End-user services
  - `Services/Services/Identity/` - Identity-related services

- **Dependency Injection**: Tất cả services đăng ký trong `ServiceCollectionExtensions.AddAllServices()`

---

## 📦 Repositories & Data Access

### Repository Pattern

- **GenericRepository<T>**: Generic CRUD operations
  - `GetByIdAsync`, `GetAllAsync`, `FindAsync`, `GetFirstOrDefaultAsync`
  - `AnyAsync`, `CountAsync`
  - `AddAsync`, `AddRangeAsync`
  - `Update`, `UpdateRange`
  - `Delete`, `DeleteRange`
  - `SoftDeleteAsync` (nếu entity support ISoftDelete)

- **Specific Repositories**:
  - **User & RBAC**: `UserRepository`, `RoleRepository`, `PermissionRepository`, `UserRoleRepository`, `RolePermissionRepository`, `RefreshTokenRepository`
  - **Product & Category**: `ProductRepository`, `CategoryRepository`, `ProductFavoriteRepository`, `ProductImageRepository`
  - **Inventory**: `InventoryRepository`, `InventoryMovementRepository`
  - **Order & Cart**: `OrderRepository`, `OrderItemRepository`, `OrderStatusHistoryRepository`, `OrderShippingRepository`, `CartRepository`
  - **Customer & Address**: `CustomerRepository`, `CustomerAddressRepository`

- **UnitOfWork**: Transaction management, commit changes
  - Properties: `Users`, `RefreshTokens`, `Roles`, `Permissions`, `UserRoles`, `RolePermissions`, `Products`, `ProductImages`, `Categories`, `ProductFavorites`, `Inventories`, `InventoryMovements`, `Carts`, `CartItems`, `Orders`, `OrderItems`, `OrderStatusHistories`, `OrderShipping`, `Customers`, `CustomerAddresses`
  - Methods: `SaveChangesAsync`, `BeginTransactionAsync`, `CommitTransactionAsync`, `RollbackTransactionAsync`

### Repository Interfaces

- `IGenericRepository<T>`: Generic operations cho BaseEntity
- `IUserRepository`, `IProductRepository`, `IOrderRepository`, `ICartRepository`, `ICategoryRepository`, `IInventoryRepository`, `ICustomerRepository`, `ICustomerAddressRepository`, `IProductFavoriteRepository`, `IProductImageRepository`, `IRoleRepository`, `IPermissionRepository`, `IUserRoleRepository`, `IRolePermissionRepository`, `IRefreshTokenRepository`, `IOrderItemRepository`, `IOrderStatusHistoryRepository`, `IOrderShippingRepository`, `IInventoryMovementRepository`
- `IUnitOfWork`: SaveChanges, transaction support

---

## 🗺️ Mapping & DTOs

### AutoMapper

- **Mapping Profiles**:
  - `AuthMappingProfile` - Auth DTOs mapping
  - `ProductMappingProfile` - Product DTOs mapping
  - `OrderMappingProfile` - Order DTOs mapping
  - `CartMappingProfile` - Cart DTOs mapping
  - `AddressMappingProfile` - Address DTOs mapping
  - `InventoryMappingProfile` - Inventory DTOs mapping
  - `ProductFavoriteMappingProfile` - Wishlist DTOs mapping
  - `UserMappingProfile` - User DTOs mapping

### DTO Structure

- **Request DTOs**: Input từ client (Create, Update, Filter)
  - `{Entity}CreateDto` / `Create{Entity}Dto`
  - `{Entity}UpdateDto` / `Update{Entity}Dto`
  - `{Entity}FilterDto` / `Filter{Entity}Dto`
  
- **Response DTOs**: Output cho client (Detail, List, Summary)
  - `{Entity}DetailDto` / `{Entity}Dto`
  - `{Entity}ListDto` / `{Entity}ListResponseDto`
  - `{Entity}SummaryDto`

- **DTO Categories**:
  - **Auth**: `RegisterRequestDTO`, `LoginDTO`, `RefreshTokenRequestDTO`, `ChangePasswordRequestDTO`, `ForgotPasswordRequestDTO`, `ResetPasswordRequestDTO`, `SocialLoginRequestDTO`, `AuthResponseDTO`, `UserInfoDTO`
  - **Products**: `ProductCreateDto`, `ProductUpdateDto`, `ProductDetailDto`, `ProductListDto`, `ProductFilterDto`, `ProductPriceDto`, `ProductImageDto`
  - **Orders**: `OrderCreateDTO`, `OrderUpdateDTO`, `OrderDetailDTO`, `OrderListDTO`, `OrderFilterDTO`, `CheckoutDto`, `CheckoutResponseDto`, `OrderShippingDto`
  - **Cart**: `AddToCartDto`, `UpdateCartItemDto`, `CartDto`, `CartSummaryDto`, `GetCartResponseDto`
  - **Categories**: `CreateCategoryDto`, `UpdateCategoryDto`, `CategoryDto`, `CategoryDetailDto`
  - **Inventory**: `InventoryDto`, `AdjustInventoryRequest`, `ReserveStockRequest`, `CheckStockResponse`, `BulkCheckStockResponse`, `InventoryMovementDto`
  - **Address**: `CreateAddressDto`, `UpdateAddressDto`, `AddressResponseDto`, `AddressListResponseDto`
  - **Wishlist**: `AddToWishlistRequest`, `WishlistItemDto`
  - **Users**: `UserCreateDTO`, `UserUpdateDTO`, `UserDetailDTO`, `UserListDTO`
  - **Permissions**: `PermissionDetailDTO`, `PermissionListDTO`, `AssignPermissionsToRoleDTO`
  - **Roles**: `RoleDetailDTO`, `RoleListDTO`, `RoleCreateDTO`, `RoleUpdateDTO`

- **Naming Convention**: 
  - DTOs: `{Entity}{Action}Dto` hoặc `{Action}{Entity}Dto`
  - Request: Thường kết thúc bằng `Request` hoặc `Dto`
  - Response: Thường kết thúc bằng `Response`, `Dto`, hoặc `DTO`

---

## ✅ Validation

### FluentValidation

- Validators cho các DTOs
- Auto-validation trong controllers
- Custom error messages
- Validation rules: Required, Email, Range, etc.

- **Validator Locations**: `VietCommerce.Application/Validators/`
  - **Auth**: `RegisterRequestValidator`, `LoginValidator`, etc.
  - **Permissions**: `PermissionCreateValidator`, `PermissionUpdateValidator`
  - **Roles**: `RoleCreateValidator`, `RoleUpdateValidator`
  - **UserRoles**: `UserRoleAssignmentValidator`

- **Validation Features**:
  - Automatic model validation trong controllers
  - Custom error messages với localization support
  - Conditional validation rules
  - Cross-field validation

---

## 💾 Caching Strategy

### Redis Cache

- **Connection**: Singleton `IConnectionMultiplexer`
- **Cache Service**: `RedisCacheService` (implements `ICacheService`)
- **Cache Keys**: Structured keys (e.g., `user:{userId}`, `product:{productId}`)
- **TTL**: Time-to-live cho cached data
- **Invalidation**: 
  - Manual invalidation
  - Pub/Sub for distributed cache invalidation
  - `PermissionCacheInvalidationService` (hosted service)

### Cached Data

- User permissions (cached với TTL)
- Product details
- Category trees
- Other frequently accessed data
- Login attempt tracking (Redis-based brute-force protection)

### Cache Invalidation

- **Manual**: Direct cache key invalidation
- **Automatic**: Pub/Sub for distributed cache invalidation
- **Hosted Service**: `PermissionCacheInvalidationService` - Background service cho permission cache refresh

---

## 🔄 Middleware Pipeline

### Order of Middleware

1. **Exception Handling** (if enabled)
2. **HTTPS Redirection**
3. **CORS** (`VietCommercePolicy` hoặc `AdminPolicy`)
4. **Authentication** (`UseAuthentication`)
5. **Authorization** (`UseAuthorization`)
6. **Permission Middleware** (`UsePermissionMiddleware`)
7. **Controllers** (`MapControllers`)

---

## 🧪 Testing

### Test Structure

- **BaseTestClass**: Base class cho unit tests
- **In-Memory Database**: Sử dụng EF Core InMemory cho testing
- **Test Data**: Seed test data trong `InitializeAsync()`

---

## 📝 Configuration

### appsettings.json

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "SQL Server connection string",
    "Redis": "Redis connection string"
  },
  "JwtSettings": {
    "Key": "JWT secret key",
    "Issuer": "VietCommerce.Api",
    "Audience": "VietCommerce.Client",
    "ExpirationMinutes": 60
  },
  "GoogleSettings": {
    "ClientId": "Google OAuth client ID"
  }
}
```

---

## 🚦 Health Checks

- `GET /health` - Application health
- `GET /health/redis` - Redis connection health

---

## 🔍 Key Features

### 1. **Multi-Tenant & Multi-Store**
- Tenant isolation
- Store-level data separation
- Cross-tenant queries prevented

### 2. **Soft Delete**
- Global query filter
- Audit trail (DeletedAt, DeletedBy)
- Data recovery possible

### 3. **Audit Trail**
- CreatedAt, CreatedBy, UpdatedAt, UpdatedBy
- AuditLog entity for important actions

### 4. **Stock Management**
- Real-time inventory tracking
- Stock reservation for orders
- Inventory movements history
- Stock transfer between stores

### 5. **Order Management**
- Auto-generate order numbers
- Order status workflow
- Order history tracking
- Shipping management

### 6. **Cart Management**
- Guest cart (SessionId)
- User cart (authenticated)
- Auto-merge on login

### 7. **Permission System**
- Granular permissions
- Role-based + Permission-based
- Redis caching for performance

### 8. **OAuth Integration**
- Google OAuth login
- Extensible for other providers

### 9. **Image Management**
- Product images with ordering
- Main image selection
- File upload service

### 10. **Analytics**
- Product views tracking
- Order statistics
- Performance metrics

---

## 🛠️ Technology Stack

- **.NET 9.0**: Framework
- **Entity Framework Core 9.0**: ORM
- **SQL Server**: Database
- **Redis**: Caching & Session (StackExchange.Redis)
- **JWT Bearer**: Authentication & Authorization
- **AutoMapper**: Object mapping
- **FluentValidation**: Validation
- **Swagger/OpenAPI**: API documentation
- **StackExchange.Redis**: Redis client library
- **BCrypt.Net**: Password hashing
- **System.Text.Json**: JSON serialization (with custom converters for enums)

---

## 📚 Code Conventions

### Naming
- **Controllers**: `{Entity}Controller.cs`
- **Services**: `{Entity}Service.cs`, Interface: `I{Entity}Service.cs`
- **Repositories**: `{Entity}Repository.cs`, Interface: `I{Entity}Repository.cs`
- **DTOs**: `{Action}{Entity}Dto.cs` (e.g., `CreateProductDto`, `ProductDetailDto`)
- **Entities**: `{Entity}.cs` (PascalCase)

### Patterns
- **Repository Pattern**: Data access abstraction
- **Service Pattern**: Business logic encapsulation
- **DTO Pattern**: Data transfer objects
- **Unit of Work**: Transaction management
- **Dependency Injection**: Loose coupling

---

## 🔐 Security Features

1. **JWT Authentication**: Secure token-based auth
2. **Password Hashing**: BCrypt for password storage
3. **RBAC**: Role & Permission-based access control
4. **CORS**: Configured for specific origins
5. **Input Validation**: FluentValidation
6. **SQL Injection Protection**: EF Core parameterized queries
7. **Brute-force Protection**: Redis-based login attempt tracking
8. **HTTPS**: Enforced in production

---

## 📊 Response Format

### Success Response
```json
{
  "success": true,
  "data": { ... },
  "message": "Operation successful"
}
```

### Error Response
```json
{
  "success": false,
  "message": "Error message",
  "errors": { ... }
}
```

### ApiResponse<T> Pattern
- Generic response wrapper
- Consistent error handling
- Type-safe responses

---

## 🎯 Development Workflow

1. **Database Changes**: 
   - Update entities in `VietCommerce.Core/Entities`
   - Add migration: `dotnet ef migrations add {Name}`
   - Apply migration: Auto-applied on startup (Development)

2. **New Feature**:
   - Create Entity → DTO → Repository → Service → Controller
   - Register in DI container
   - Add AutoMapper profile
   - Add FluentValidation rules

3. **Testing**:
   - Unit tests in `VietCommerce.Tests`
   - Use InMemory database
   - Test repositories, services

---

## 📖 Summary

VietCommerce là một hệ thống E-commerce backend hoàn chỉnh với:
- ✅ Multi-tenant, multi-store architecture
- ✅ Full RBAC (Role & Permission-based)
- ✅ Complete product, order, inventory management
- ✅ Redis caching for performance
- ✅ JWT authentication
- ✅ Soft delete & audit trail
- ✅ Clean architecture pattern
- ✅ Comprehensive API endpoints
- ✅ Admin & End-user APIs separation

Hệ thống được thiết kế để scale, maintainable, và secure với best practices của .NET ecosystem.

