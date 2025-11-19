# BẢN ĐỒ BACKEND - ĐẶC TẢ CHI TIẾT TỪ FRONTEND

## 📋 MỤC LỤC
1. [Tổng quan](#tổng-quan)
2. [Service Interfaces (I*Service)](#service-interfaces)
3. [Controllers & Endpoints](#controllers--endpoints)
4. [Mapping Controller → Endpoint → Service → Dependency](#mapping)
5. [Phân loại theo Module/Feature](#phân-loại-theo-module)
6. [Đánh dấu Endpoint Quan trọng](#đánh-dấu-endpoint-quan-trọng)

---

## 📌 TỔNG QUAN

**Base URL**: `https://localhost:7131/api/v1` (theo `apiClient.ts`)

**Authentication**: JWT Bearer Token (theo `apiClient.ts` interceptor)

**Response Format**: 
```typescript
{
  success: boolean;
  data?: T;
  meta?: {
    pagination?: PaginationMeta;
    timestamp?: string;
    request_id?: string;
  };
  message?: string;
  errors?: string[];
}
```

**Pagination Format**:
```typescript
{
  data: T[];
  meta: {
    currentPage: number;
    pageSize: number;
    totalItems: number;
    totalPages: number;
    hasNextPage: boolean;
    hasPreviousPage: boolean;
  };
}
```

---

## 🔧 SERVICE INTERFACES (I*Service)

### 1. IAuthService
```csharp
public interface IAuthService
{
    Task<AuthResponse> LoginAsync(LoginRequest request);
    Task<AuthResponse> RefreshTokenAsync(RefreshTokenRequest request);
    Task LogoutAsync(string userId);
    Task<bool> ValidateTokenAsync(string token);
    Task ChangePasswordAsync(ChangePasswordRequest request);
}
```

### 2. IUserService
```csharp
public interface IUserService
{
    // CRUD Operations
    Task<PaginatedResponse<UserDto>> GetUsersAsync(PaginationParams pagination, UserFilters filters);
    Task<UserDto> GetUserByIdAsync(string id);
    Task<UserDto> CreateUserAsync(CreateUserRequest request);
    Task<UserDto> UpdateUserAsync(string id, UpdateUserRequest request);
    Task DeleteUserAsync(string id);
    
    // Profile Management
    Task<UserDto> GetCurrentUserProfileAsync(string userId);
    Task<UserDto> UpdateCurrentUserProfileAsync(string userId, UpdateProfileRequest request);
    
    // Permission Management
    Task<List<PermissionDto>> GetUserPermissionsAsync(string userId);
    Task UpdateUserPermissionsAsync(string userId, List<string> permissions);
    
    // Status Management
    Task ActivateUserAsync(string userId);
    Task DeactivateUserAsync(string userId);
}
```

### 3. IProductService
```csharp
public interface IProductService
{
    Task<PaginatedResponse<ProductDto>> GetProductsAsync(PaginationParams pagination, ProductFilters filters);
    Task<ProductDto> GetProductByIdAsync(string id);
    Task<ProductDto> CreateProductAsync(CreateProductRequest request);
    Task<ProductDto> UpdateProductAsync(string id, UpdateProductRequest request);
    Task DeleteProductAsync(string id);
    Task<List<ProductDto>> SearchProductsAsync(string searchTerm);
    Task<List<ProductDto>> GetProductsByCategoryAsync(string categoryId);
}
```

### 4. IInventoryService
```csharp
public interface IInventoryService
{
    Task<PaginatedResponse<InventoryItemDto>> GetInventoryAsync(PaginationParams pagination, InventoryFilters filters);
    Task<InventoryItemDto> GetInventoryByProductIdAsync(string productId, string storeId);
    Task<InventoryItemDto> UpdateInventoryAsync(string productId, UpdateInventoryRequest request);
    Task<InventoryItemDto> AdjustStockAsync(string productId, StockAdjustmentRequest request);
    Task<List<LowStockAlertDto>> GetLowStockAlertsAsync(string storeId);
    Task<PaginatedResponse<InventoryHistoryDto>> GetInventoryHistoryAsync(string productId, PaginationParams pagination);
    Task<InventoryItemDto> SetStockLevelsAsync(string productId, StockLevelRequest request);
}
```

### 5. IOrderService
```csharp
public interface IOrderService
{
    Task<PaginatedResponse<OrderDto>> GetOrdersAsync(PaginationParams pagination, OrderFilters filters);
    Task<OrderDto> GetOrderByIdAsync(string id);
    Task<OrderDto> CreateOrderAsync(CreateOrderRequest request);
    Task<OrderDto> UpdateOrderAsync(string id, UpdateOrderRequest request);
    Task<OrderDto> UpdateOrderStatusAsync(string id, OrderStatus status);
    Task CancelOrderAsync(string id, string reason);
    Task<OrderDto> RefundOrderAsync(string id, RefundRequest request);
    Task<OrderDto> VoidOrderAsync(string id, string reason);
    
    // Order Items
    Task<OrderDto> AddItemToOrderAsync(string orderId, OrderItemRequest item);
    Task<OrderDto> RemoveItemFromOrderAsync(string orderId, string itemId);
    Task<OrderDto> ApplyDiscountAsync(string orderId, DiscountRequest discount);
}
```

### 6. IPaymentService
```csharp
public interface IPaymentService
{
    Task<PaymentResponse> ProcessPaymentAsync(PaymentRequest request);
    Task<PaymentResponse> ProcessCashPaymentAsync(CashPaymentRequest request);
    Task<PaymentResponse> ProcessCardPaymentAsync(CardPaymentRequest request);
    Task<PaymentCalculationDto> CalculateOrderTotalsAsync(CalculateOrderRequest request);
    Task VoidPaymentAsync(string paymentId, string reason);
    Task<PaginatedResponse<PaymentDto>> GetPaymentHistoryAsync(PaginationParams pagination, PaymentFilters filters);
    Task<RefundResponse> ProcessRefundAsync(string paymentId, RefundRequest request);
}
```

### 7. ICustomerService (CRM)
```csharp
public interface ICustomerService
{
    Task<PaginatedResponse<CustomerDto>> GetCustomersAsync(PaginationParams pagination, CustomerFilters filters);
    Task<CustomerDto> GetCustomerByIdAsync(string id);
    Task<CustomerDto> CreateCustomerAsync(CreateCustomerRequest request);
    Task<CustomerDto> UpdateCustomerAsync(string id, UpdateCustomerRequest request);
    Task DeleteCustomerAsync(string id);
    Task<List<CustomerDto>> SearchCustomersAsync(string searchTerm);
    
    // Customer Orders
    Task<PaginatedResponse<OrderDto>> GetCustomerOrdersAsync(string customerId, PaginationParams pagination);
    
    // Customer Interactions
    Task<PaginatedResponse<CRMInteractionDto>> GetCustomerInteractionsAsync(string customerId, PaginationParams pagination);
    Task<CRMInteractionDto> CreateCustomerInteractionAsync(CreateInteractionRequest request);
    Task<CRMInteractionDto> UpdateCustomerInteractionAsync(string id, UpdateInteractionRequest request);
}
```

### 8. ISupplierService (LRM)
```csharp
public interface ISupplierService
{
    Task<PaginatedResponse<SupplierDto>> GetSuppliersAsync(PaginationParams pagination, SupplierFilters filters);
    Task<SupplierDto> GetSupplierByIdAsync(string id);
    Task<SupplierDto> CreateSupplierAsync(CreateSupplierRequest request);
    Task<SupplierDto> UpdateSupplierAsync(string id, UpdateSupplierRequest request);
    Task DeleteSupplierAsync(string id);
}
```

### 9. IStockTransferService (LRM)
```csharp
public interface IStockTransferService
{
    Task<PaginatedResponse<StockTransferDto>> GetStockTransfersAsync(PaginationParams pagination, StockTransferFilters filters);
    Task<StockTransferDto> GetStockTransferByIdAsync(string id);
    Task<StockTransferDto> CreateStockTransferAsync(CreateStockTransferRequest request);
    Task<StockTransferDto> UpdateStockTransferStatusAsync(string id, StockTransferStatus status);
    Task CancelStockTransferAsync(string id, string reason);
}
```

### 10. IEmployeeService (HRM)
```csharp
public interface IEmployeeService
{
    Task<PaginatedResponse<EmployeeDto>> GetEmployeesAsync(PaginationParams pagination, EmployeeFilters filters);
    Task<EmployeeDto> GetEmployeeByIdAsync(string id);
    Task<EmployeeDto> CreateEmployeeAsync(CreateEmployeeRequest request);
    Task<EmployeeDto> UpdateEmployeeAsync(string id, UpdateEmployeeRequest request);
    Task DeleteEmployeeAsync(string id);
}
```

### 11. ILeaveRequestService (HRM)
```csharp
public interface ILeaveRequestService
{
    Task<PaginatedResponse<LeaveRequestDto>> GetLeaveRequestsAsync(PaginationParams pagination, LeaveRequestFilters filters);
    Task<LeaveRequestDto> GetLeaveRequestByIdAsync(string id);
    Task<LeaveRequestDto> CreateLeaveRequestAsync(CreateLeaveRequestRequest request);
    Task<LeaveRequestDto> UpdateLeaveRequestStatusAsync(string id, LeaveRequestStatus status, string comments);
}
```

### 12. IWorkScheduleService (HRM)
```csharp
public interface IWorkScheduleService
{
    Task<PaginatedResponse<WorkScheduleDto>> GetWorkSchedulesAsync(PaginationParams pagination, WorkScheduleFilters filters);
    Task<WorkScheduleDto> GetWorkScheduleByIdAsync(string id);
    Task<WorkScheduleDto> CreateWorkScheduleAsync(CreateWorkScheduleRequest request);
    Task<WorkScheduleDto> UpdateWorkScheduleAsync(string id, UpdateWorkScheduleRequest request);
    Task DeleteWorkScheduleAsync(string id);
}
```

### 13. IShiftService
```csharp
public interface IShiftService
{
    Task<PaginatedResponse<ShiftDto>> GetShiftsAsync(PaginationParams pagination, ShiftFilters filters);
    Task<ShiftDto> GetShiftByIdAsync(string id);
    Task<ShiftDto> GetCurrentShiftAsync(string userId);
    Task<ShiftDto> OpenShiftAsync(OpenShiftRequest request);
    Task<ShiftDto> CloseShiftAsync(string id, CloseShiftRequest request);
    Task<ShiftDto> UpdateShiftAsync(string id, UpdateShiftRequest request);
}
```

### 14. ITaskService
```csharp
public interface ITaskService
{
    Task<PaginatedResponse<TaskDto>> GetTasksAsync(PaginationParams pagination, TaskFilters filters);
    Task<TaskDto> GetTaskByIdAsync(string id);
    Task<TaskDto> CreateTaskAsync(CreateTaskRequest request);
    Task<TaskDto> UpdateTaskAsync(string id, UpdateTaskRequest request);
    Task<TaskDto> UpdateTaskStatusAsync(string id, TaskStatus status);
    Task DeleteTaskAsync(string id);
    Task AssignTaskAsync(string taskId, string assigneeId);
    Task CompleteTaskAsync(string taskId, string completionNote);
    Task<PaginatedResponse<TaskDto>> GetTasksByAssigneeAsync(string assigneeId, PaginationParams pagination);
}
```

### 15. INotificationService
```csharp
public interface INotificationService
{
    Task<PaginatedResponse<NotificationDto>> GetNotificationsAsync(string userId, PaginationParams pagination);
    Task<NotificationDto> GetNotificationByIdAsync(string id);
    Task MarkNotificationAsReadAsync(string id);
    Task MarkAllNotificationsAsReadAsync(string userId);
    Task DeleteNotificationAsync(string id);
    Task<int> GetUnreadNotificationCountAsync(string userId);
}
```

### 16. IAnalyticsService
```csharp
public interface IAnalyticsService
{
    Task<DashboardMetricsDto> GetDashboardMetricsAsync(string storeId, DateTime? startDate, DateTime? endDate);
    Task<SalesReportDto> GetSalesReportAsync(ReportPeriod period, SalesFilters filters);
    Task<RevenueAnalyticsDto> GetRevenueAnalyticsAsync(DateTime startDate, DateTime endDate, string groupBy);
    Task<List<ProductPerformanceDto>> GetProductPerformanceAsync(ReportPeriod period);
    Task<List<StaffPerformanceDto>> GetStaffPerformanceAsync(ReportPeriod period);
    Task<CustomerAnalyticsDto> GetCustomerAnalyticsAsync(ReportPeriod period);
}
```

### 17. IAuditLogService
```csharp
public interface IAuditLogService
{
    Task<PaginatedResponse<AuditLogDto>> GetAuditLogsAsync(PaginationParams pagination, AuditLogFilters filters);
    Task<AuditLogDto> GetAuditLogByIdAsync(string id);
    Task LogActionAsync(AuditLogEntry entry);
}
```

### 18. ICategoryService
```csharp
public interface ICategoryService
{
    Task<List<CategoryDto>> GetCategoriesAsync(string storeId);
    Task<CategoryDto> GetCategoryByIdAsync(string id);
    Task<CategoryDto> CreateCategoryAsync(CreateCategoryRequest request);
    Task<CategoryDto> UpdateCategoryAsync(string id, UpdateCategoryRequest request);
    Task DeleteCategoryAsync(string id);
}
```

---

## 🎮 CONTROLLERS & ENDPOINTS

### AUTHENTICATION MODULE

#### AuthController
| HTTP Method | Route | Controller Method | Service | Priority |
|------------|-------|-------------------|---------|----------|
| POST | `/api/v1/Auth/login` | `Login` | `IAuthService.LoginAsync` | 🔴 **CRITICAL** |
| POST | `/api/v1/auth/refresh` | `RefreshToken` | `IAuthService.RefreshTokenAsync` | 🔴 **CRITICAL** |
| POST | `/api/v1/auth/logout` | `Logout` | `IAuthService.LogoutAsync` | 🟡 **HIGH** |
| POST | `/api/v1/auth/change-password` | `ChangePassword` | `IAuthService.ChangePasswordAsync` | 🟡 **HIGH** |
| GET | `/api/v1/auth/validate` | `ValidateToken` | `IAuthService.ValidateTokenAsync` | 🟢 **MEDIUM** |

**Dependencies**: 
- `IJwtTokenService` (token generation/validation)
- `IUserRepository` (user lookup)
- `IPasswordHasher` (password verification)

---

### USER MANAGEMENT MODULE

#### UserController
| HTTP Method | Route | Controller Method | Service | Priority |
|------------|-------|-------------------|---------|----------|
| GET | `/api/v1/users` | `GetUsers` | `IUserService.GetUsersAsync` | 🟡 **HIGH** |
| GET | `/api/v1/users/{id}` | `GetUserById` | `IUserService.GetUserByIdAsync` | 🟡 **HIGH** |
| POST | `/api/v1/users` | `CreateUser` | `IUserService.CreateUserAsync` | 🟡 **HIGH** |
| PUT | `/api/v1/users/{id}` | `UpdateUser` | `IUserService.UpdateUserAsync` | 🟡 **HIGH** |
| DELETE | `/api/v1/users/{id}` | `DeleteUser` | `IUserService.DeleteUserAsync` | 🟡 **HIGH** |
| GET | `/api/v1/users/profile` | `GetCurrentUserProfile` | `IUserService.GetCurrentUserProfileAsync` | 🔴 **CRITICAL** |
| PUT | `/api/v1/users/profile` | `UpdateCurrentUserProfile` | `IUserService.UpdateCurrentUserProfileAsync` | 🟡 **HIGH** |
| PUT | `/api/v1/users/{id}/activate` | `ActivateUser` | `IUserService.ActivateUserAsync` | 🟢 **MEDIUM** |
| PUT | `/api/v1/users/{id}/deactivate` | `DeactivateUser` | `IUserService.DeactivateUserAsync` | 🟢 **MEDIUM** |
| GET | `/api/v1/users/{id}/permissions` | `GetUserPermissions` | `IUserService.GetUserPermissionsAsync` | 🟢 **MEDIUM** |
| PUT | `/api/v1/users/{id}/permissions` | `UpdateUserPermissions` | `IUserService.UpdateUserPermissionsAsync` | 🟢 **MEDIUM** |

**Dependencies**:
- `IUserRepository`
- `IPermissionRepository`
- `IStoreRepository`
- `IPasswordHasher`
- `IEmailService` (for user creation notifications)

---

### PRODUCT MANAGEMENT MODULE

#### ProductController
| HTTP Method | Route | Controller Method | Service | Priority |
|------------|-------|-------------------|---------|----------|
| GET | `/api/v1/products` | `GetProducts` | `IProductService.GetProductsAsync` | 🔴 **CRITICAL** |
| GET | `/api/v1/products/{id}` | `GetProductById` | `IProductService.GetProductByIdAsync` | 🔴 **CRITICAL** |
| POST | `/api/v1/products` | `CreateProduct` | `IProductService.CreateProductAsync` | 🟡 **HIGH** |
| PUT | `/api/v1/products/{id}` | `UpdateProduct` | `IProductService.UpdateProductAsync` | 🟡 **HIGH** |
| DELETE | `/api/v1/products/{id}` | `DeleteProduct` | `IProductService.DeleteProductAsync` | 🟡 **HIGH** |
| GET | `/api/v1/products/search` | `SearchProducts` | `IProductService.SearchProductsAsync` | 🔴 **CRITICAL** |
| GET | `/api/v1/products/category/{categoryId}` | `GetProductsByCategory` | `IProductService.GetProductsByCategoryAsync` | 🟡 **HIGH** |

**Dependencies**:
- `IProductRepository`
- `ICategoryRepository`
- `IFileStorageService` (for product images)
- `IInventoryService` (for stock initialization)

---

### INVENTORY MANAGEMENT MODULE

#### InventoryController
| HTTP Method | Route | Controller Method | Service | Priority |
|------------|-------|-------------------|---------|----------|
| GET | `/api/v1/inventory` | `GetInventory` | `IInventoryService.GetInventoryAsync` | 🔴 **CRITICAL** |
| GET | `/api/v1/inventory/product/{productId}` | `GetInventoryByProduct` | `IInventoryService.GetInventoryByProductIdAsync` | 🔴 **CRITICAL** |
| PUT | `/api/v1/inventory/{productId}` | `UpdateInventory` | `IInventoryService.UpdateInventoryAsync` | 🟡 **HIGH** |
| POST | `/api/v1/inventory/adjustments` | `AdjustStock` | `IInventoryService.AdjustStockAsync` | 🟡 **HIGH** |
| GET | `/api/v1/inventory/alerts` | `GetLowStockAlerts` | `IInventoryService.GetLowStockAlertsAsync` | 🟡 **HIGH** |
| GET | `/api/v1/inventory/{productId}/history` | `GetInventoryHistory` | `IInventoryService.GetInventoryHistoryAsync` | 🟢 **MEDIUM** |
| PUT | `/api/v1/inventory/{productId}/levels` | `SetStockLevels` | `IInventoryService.SetStockLevelsAsync` | 🟢 **MEDIUM** |

**Dependencies**:
- `IInventoryRepository`
- `IProductRepository`
- `IInventoryHistoryRepository`
- `INotificationService` (for low stock alerts)
- `IWebSocketService` (for real-time updates)

---

### ORDER MANAGEMENT MODULE

#### OrderController
| HTTP Method | Route | Controller Method | Service | Priority |
|------------|-------|-------------------|---------|----------|
| GET | `/api/v1/orders` | `GetOrders` | `IOrderService.GetOrdersAsync` | 🔴 **CRITICAL** |
| GET | `/api/v1/orders/{id}` | `GetOrderById` | `IOrderService.GetOrderByIdAsync` | 🔴 **CRITICAL** |
| POST | `/api/v1/orders` | `CreateOrder` | `IOrderService.CreateOrderAsync` | 🔴 **CRITICAL** |
| PUT | `/api/v1/orders/{id}` | `UpdateOrder` | `IOrderService.UpdateOrderAsync` | 🟡 **HIGH** |
| PUT | `/api/v1/orders/{id}/status` | `UpdateOrderStatus` | `IOrderService.UpdateOrderStatusAsync` | 🟡 **HIGH** |
| DELETE | `/api/v1/orders/{id}` | `CancelOrder` | `IOrderService.CancelOrderAsync` | 🟡 **HIGH** |
| POST | `/api/v1/orders/{id}/refund` | `RefundOrder` | `IOrderService.RefundOrderAsync` | 🟡 **HIGH** |
| POST | `/api/v1/orders/{id}/void` | `VoidOrder` | `IOrderService.VoidOrderAsync` | 🟢 **MEDIUM** |
| POST | `/api/v1/orders/{id}/items` | `AddItemToOrder` | `IOrderService.AddItemToOrderAsync` | 🟡 **HIGH** |
| DELETE | `/api/v1/orders/{id}/items/{itemId}` | `RemoveItemFromOrder` | `IOrderService.RemoveItemFromOrderAsync` | 🟡 **HIGH** |
| POST | `/api/v1/orders/{id}/discount` | `ApplyDiscount` | `IOrderService.ApplyDiscountAsync` | 🟢 **MEDIUM** |

**Dependencies**:
- `IOrderRepository`
- `IOrderItemRepository`
- `IInventoryService` (for stock deduction)
- `ICustomerService` (for customer linking)
- `IPaymentService` (for payment processing)
- `INotificationService` (for order notifications)
- `IWebSocketService` (for real-time updates)

---

### PAYMENT MODULE (POS)

#### PaymentController
| HTTP Method | Route | Controller Method | Service | Priority |
|------------|-------|-------------------|---------|----------|
| POST | `/api/v1/pos/calculate` | `CalculateOrderTotals` | `IPaymentService.CalculateOrderTotalsAsync` | 🔴 **CRITICAL** |
| POST | `/api/v1/pos/process-payment` | `ProcessPayment` | `IPaymentService.ProcessPaymentAsync` | 🔴 **CRITICAL** |
| POST | `/api/v1/pos/cash-payment` | `ProcessCashPayment` | `IPaymentService.ProcessCashPaymentAsync` | 🔴 **CRITICAL** |
| POST | `/api/v1/pos/card-payment` | `ProcessCardPayment` | `IPaymentService.ProcessCardPaymentAsync` | 🔴 **CRITICAL** |
| POST | `/api/v1/payments/{id}/void` | `VoidPayment` | `IPaymentService.VoidPaymentAsync` | 🟡 **HIGH** |
| GET | `/api/v1/payments` | `GetPaymentHistory` | `IPaymentService.GetPaymentHistoryAsync` | 🟡 **HIGH** |
| POST | `/api/v1/payments/{id}/refund` | `ProcessRefund` | `IPaymentService.ProcessRefundAsync` | 🟡 **HIGH** |

**Dependencies**:
- `IPaymentRepository`
- `IOrderService` (for order updates)
- `IPaymentGatewayService` (Stripe/PayPal integration)
- `IShiftService` (for cash drawer management)
- `IReceiptService` (for receipt generation)

---

### CUSTOMER MANAGEMENT MODULE (CRM)

#### CustomerController
| HTTP Method | Route | Controller Method | Service | Priority |
|------------|-------|-------------------|---------|----------|
| GET | `/api/v1/customers` | `GetCustomers` | `ICustomerService.GetCustomersAsync` | 🟡 **HIGH** |
| GET | `/api/v1/customers/{id}` | `GetCustomerById` | `ICustomerService.GetCustomerByIdAsync` | 🟡 **HIGH** |
| POST | `/api/v1/customers` | `CreateCustomer` | `ICustomerService.CreateCustomerAsync` | 🟡 **HIGH** |
| PUT | `/api/v1/customers/{id}` | `UpdateCustomer` | `ICustomerService.UpdateCustomerAsync` | 🟡 **HIGH** |
| DELETE | `/api/v1/customers/{id}` | `DeleteCustomer` | `ICustomerService.DeleteCustomerAsync` | 🟢 **MEDIUM** |
| GET | `/api/v1/customers/search` | `SearchCustomers` | `ICustomerService.SearchCustomersAsync` | 🟡 **HIGH** |
| GET | `/api/v1/customers/{id}/orders` | `GetCustomerOrders` | `ICustomerService.GetCustomerOrdersAsync` | 🟡 **HIGH** |
| GET | `/api/v1/customers/{id}/interactions` | `GetCustomerInteractions` | `ICustomerService.GetCustomerInteractionsAsync` | 🟢 **MEDIUM** |
| POST | `/api/v1/customers/{id}/interactions` | `CreateCustomerInteraction` | `ICustomerService.CreateCustomerInteractionAsync` | 🟡 **HIGH** |
| PUT | `/api/v1/customers/interactions/{id}` | `UpdateCustomerInteraction` | `ICustomerService.UpdateCustomerInteractionAsync` | 🟢 **MEDIUM** |

**Dependencies**:
- `ICustomerRepository`
- `IOrderService` (for customer order history)
- `ICRMInteractionRepository`
- `ILoyaltyService` (for loyalty points)

---

### SUPPLIER MANAGEMENT MODULE (LRM)

#### SupplierController
| HTTP Method | Route | Controller Method | Service | Priority |
|------------|-------|-------------------|---------|----------|
| GET | `/api/v1/suppliers` | `GetSuppliers` | `ISupplierService.GetSuppliersAsync` | 🟢 **MEDIUM** |
| GET | `/api/v1/suppliers/{id}` | `GetSupplierById` | `ISupplierService.GetSupplierByIdAsync` | 🟢 **MEDIUM** |
| POST | `/api/v1/suppliers` | `CreateSupplier` | `ISupplierService.CreateSupplierAsync` | 🟢 **MEDIUM** |
| PUT | `/api/v1/suppliers/{id}` | `UpdateSupplier` | `ISupplierService.UpdateSupplierAsync` | 🟢 **MEDIUM** |
| DELETE | `/api/v1/suppliers/{id}` | `DeleteSupplier` | `ISupplierService.DeleteSupplierAsync` | 🟢 **MEDIUM** |

**Dependencies**:
- `ISupplierRepository`

---

### STOCK TRANSFER MODULE (LRM)

#### StockTransferController
| HTTP Method | Route | Controller Method | Service | Priority |
|------------|-------|-------------------|---------|----------|
| GET | `/api/v1/stock-transfers` | `GetStockTransfers` | `IStockTransferService.GetStockTransfersAsync` | 🟢 **MEDIUM** |
| GET | `/api/v1/stock-transfers/{id}` | `GetStockTransferById` | `IStockTransferService.GetStockTransferByIdAsync` | 🟢 **MEDIUM** |
| POST | `/api/v1/stock-transfers` | `CreateStockTransfer` | `IStockTransferService.CreateStockTransferAsync` | 🟢 **MEDIUM** |
| PUT | `/api/v1/stock-transfers/{id}/status` | `UpdateStockTransferStatus` | `IStockTransferService.UpdateStockTransferStatusAsync` | 🟢 **MEDIUM** |
| DELETE | `/api/v1/stock-transfers/{id}` | `CancelStockTransfer` | `IStockTransferService.CancelStockTransferAsync` | 🟢 **MEDIUM** |

**Dependencies**:
- `IStockTransferRepository`
- `IInventoryService` (for stock updates)

---

### EMPLOYEE MANAGEMENT MODULE (HRM)

#### EmployeeController
| HTTP Method | Route | Controller Method | Service | Priority |
|------------|-------|-------------------|---------|----------|
| GET | `/api/v1/employees` | `GetEmployees` | `IEmployeeService.GetEmployeesAsync` | 🟢 **MEDIUM** |
| GET | `/api/v1/employees/{id}` | `GetEmployeeById` | `IEmployeeService.GetEmployeeByIdAsync` | 🟢 **MEDIUM** |
| POST | `/api/v1/employees` | `CreateEmployee` | `IEmployeeService.CreateEmployeeAsync` | 🟢 **MEDIUM** |
| PUT | `/api/v1/employees/{id}` | `UpdateEmployee` | `IEmployeeService.UpdateEmployeeAsync` | 🟢 **MEDIUM** |
| DELETE | `/api/v1/employees/{id}` | `DeleteEmployee` | `IEmployeeService.DeleteEmployeeAsync` | 🟢 **MEDIUM** |

**Dependencies**:
- `IEmployeeRepository`
- `IUserService` (for user account creation)

---

### LEAVE REQUEST MODULE (HRM)

#### LeaveRequestController
| HTTP Method | Route | Controller Method | Service | Priority |
|------------|-------|-------------------|---------|----------|
| GET | `/api/v1/leave-requests` | `GetLeaveRequests` | `ILeaveRequestService.GetLeaveRequestsAsync` | 🟢 **MEDIUM** |
| GET | `/api/v1/leave-requests/{id}` | `GetLeaveRequestById` | `ILeaveRequestService.GetLeaveRequestByIdAsync` | 🟢 **MEDIUM** |
| POST | `/api/v1/leave-requests` | `CreateLeaveRequest` | `ILeaveRequestService.CreateLeaveRequestAsync` | 🟢 **MEDIUM** |
| PUT | `/api/v1/leave-requests/{id}/status` | `UpdateLeaveRequestStatus` | `ILeaveRequestService.UpdateLeaveRequestStatusAsync` | 🟢 **MEDIUM** |

**Dependencies**:
- `ILeaveRequestRepository`
- `INotificationService` (for approval notifications)

---

### WORK SCHEDULE MODULE (HRM)

#### WorkScheduleController
| HTTP Method | Route | Controller Method | Service | Priority |
|------------|-------|-------------------|---------|----------|
| GET | `/api/v1/work-schedules` | `GetWorkSchedules` | `IWorkScheduleService.GetWorkSchedulesAsync` | 🟢 **MEDIUM** |
| GET | `/api/v1/work-schedules/{id}` | `GetWorkScheduleById` | `IWorkScheduleService.GetWorkScheduleByIdAsync` | 🟢 **MEDIUM** |
| POST | `/api/v1/work-schedules` | `CreateWorkSchedule` | `IWorkScheduleService.CreateWorkScheduleAsync` | 🟢 **MEDIUM** |
| PUT | `/api/v1/work-schedules/{id}` | `UpdateWorkSchedule` | `IWorkScheduleService.UpdateWorkScheduleAsync` | 🟢 **MEDIUM** |
| DELETE | `/api/v1/work-schedules/{id}` | `DeleteWorkSchedule` | `IWorkScheduleService.DeleteWorkScheduleAsync` | 🟢 **MEDIUM** |

**Dependencies**:
- `IWorkScheduleRepository`
- `IEmployeeService`

---

### SHIFT MANAGEMENT MODULE

#### ShiftController
| HTTP Method | Route | Controller Method | Service | Priority |
|------------|-------|-------------------|---------|----------|
| GET | `/api/v1/shifts` | `GetShifts` | `IShiftService.GetShiftsAsync` | 🟡 **HIGH** |
| GET | `/api/v1/shifts/{id}` | `GetShiftById` | `IShiftService.GetShiftByIdAsync` | 🟡 **HIGH** |
| GET | `/api/v1/shifts/current` | `GetCurrentShift` | `IShiftService.GetCurrentShiftAsync` | 🔴 **CRITICAL** |
| POST | `/api/v1/shifts/open` | `OpenShift` | `IShiftService.OpenShiftAsync` | 🔴 **CRITICAL** |
| PUT | `/api/v1/shifts/{id}/close` | `CloseShift` | `IShiftService.CloseShiftAsync` | 🔴 **CRITICAL** |
| PUT | `/api/v1/shifts/{id}` | `UpdateShift` | `IShiftService.UpdateShiftAsync` | 🟢 **MEDIUM** |

**Dependencies**:
- `IShiftRepository`
- `IUserService` (for staff validation)
- `IPaymentService` (for cash reconciliation)

---

### TASK MANAGEMENT MODULE

#### TaskController
| HTTP Method | Route | Controller Method | Service | Priority |
|------------|-------|-------------------|---------|----------|
| GET | `/api/v1/tasks` | `GetTasks` | `ITaskService.GetTasksAsync` | 🟡 **HIGH** |
| GET | `/api/v1/tasks/{id}` | `GetTaskById` | `ITaskService.GetTaskByIdAsync` | 🟡 **HIGH** |
| POST | `/api/v1/tasks` | `CreateTask` | `ITaskService.CreateTaskAsync` | 🟡 **HIGH** |
| PUT | `/api/v1/tasks/{id}` | `UpdateTask` | `ITaskService.UpdateTaskAsync` | 🟡 **HIGH** |
| PUT | `/api/v1/tasks/{id}/status` | `UpdateTaskStatus` | `ITaskService.UpdateTaskStatusAsync` | 🟡 **HIGH** |
| DELETE | `/api/v1/tasks/{id}` | `DeleteTask` | `ITaskService.DeleteTaskAsync` | 🟢 **MEDIUM** |
| POST | `/api/v1/tasks/{id}/assign` | `AssignTask` | `ITaskService.AssignTaskAsync` | 🟡 **HIGH** |
| POST | `/api/v1/tasks/{id}/complete` | `CompleteTask` | `ITaskService.CompleteTaskAsync` | 🟡 **HIGH** |
| GET | `/api/v1/tasks/assignee/{assigneeId}` | `GetTasksByAssignee` | `ITaskService.GetTasksByAssigneeAsync` | 🟡 **HIGH** |

**Dependencies**:
- `ITaskRepository`
- `IUserService` (for assignee validation)
- `INotificationService` (for task assignment notifications)
- `IWebSocketService` (for real-time task updates)

---

### NOTIFICATION MODULE

#### NotificationController
| HTTP Method | Route | Controller Method | Service | Priority |
|------------|-------|-------------------|---------|----------|
| GET | `/api/v1/notifications` | `GetNotifications` | `INotificationService.GetNotificationsAsync` | 🟡 **HIGH** |
| GET | `/api/v1/notifications/{id}` | `GetNotificationById` | `INotificationService.GetNotificationByIdAsync` | 🟢 **MEDIUM** |
| PUT | `/api/v1/notifications/{id}/read` | `MarkNotificationAsRead` | `INotificationService.MarkNotificationAsReadAsync` | 🟡 **HIGH** |
| PUT | `/api/v1/notifications/read-all` | `MarkAllNotificationsAsRead` | `INotificationService.MarkAllNotificationsAsReadAsync` | 🟡 **HIGH** |
| DELETE | `/api/v1/notifications/{id}` | `DeleteNotification` | `INotificationService.DeleteNotificationAsync` | 🟢 **MEDIUM** |
| GET | `/api/v1/notifications/unread-count` | `GetUnreadNotificationCount` | `INotificationService.GetUnreadNotificationCountAsync` | 🟡 **HIGH** |

**Dependencies**:
- `INotificationRepository`
- `IWebSocketService` (for real-time notifications)

---

### ANALYTICS MODULE

#### AnalyticsController
| HTTP Method | Route | Controller Method | Service | Priority |
|------------|-------|-------------------|---------|----------|
| GET | `/api/v1/analytics/dashboard` | `GetDashboardMetrics` | `IAnalyticsService.GetDashboardMetricsAsync` | 🟡 **HIGH** |
| GET | `/api/v1/analytics/sales` | `GetSalesReport` | `IAnalyticsService.GetSalesReportAsync` | 🟢 **MEDIUM** |
| GET | `/api/v1/analytics/revenue` | `GetRevenueAnalytics` | `IAnalyticsService.GetRevenueAnalyticsAsync` | 🟢 **MEDIUM** |
| GET | `/api/v1/analytics/products` | `GetProductPerformance` | `IAnalyticsService.GetProductPerformanceAsync` | 🟢 **MEDIUM** |
| GET | `/api/v1/analytics/staff` | `GetStaffPerformance` | `IAnalyticsService.GetStaffPerformanceAsync` | 🟢 **MEDIUM** |
| GET | `/api/v1/analytics/customers` | `GetCustomerAnalytics` | `IAnalyticsService.GetCustomerAnalyticsAsync` | 🟢 **MEDIUM** |

**Dependencies**:
- `IOrderRepository` (for sales data)
- `IProductRepository` (for product performance)
- `IUserRepository` (for staff performance)
- `ICustomerRepository` (for customer analytics)

---

### AUDIT LOG MODULE

#### AuditLogController
| HTTP Method | Route | Controller Method | Service | Priority |
|------------|-------|-------------------|---------|----------|
| GET | `/api/v1/audit-logs` | `GetAuditLogs` | `IAuditLogService.GetAuditLogsAsync` | 🟢 **MEDIUM** |
| GET | `/api/v1/audit-logs/{id}` | `GetAuditLogById` | `IAuditLogService.GetAuditLogByIdAsync` | 🟢 **MEDIUM** |

**Dependencies**:
- `IAuditLogRepository`

---

### CATEGORY MODULE

#### CategoryController
| HTTP Method | Route | Controller Method | Service | Priority |
|------------|-------|-------------------|---------|----------|
| GET | `/api/v1/categories` | `GetCategories` | `ICategoryService.GetCategoriesAsync` | 🟡 **HIGH** |
| GET | `/api/v1/categories/{id}` | `GetCategoryById` | `ICategoryService.GetCategoryByIdAsync` | 🟢 **MEDIUM** |
| POST | `/api/v1/categories` | `CreateCategory` | `ICategoryService.CreateCategoryAsync` | 🟢 **MEDIUM** |
| PUT | `/api/v1/categories/{id}` | `UpdateCategory` | `ICategoryService.UpdateCategoryAsync` | 🟢 **MEDIUM** |
| DELETE | `/api/v1/categories/{id}` | `DeleteCategory` | `ICategoryService.DeleteCategoryAsync` | 🟢 **MEDIUM** |

**Dependencies**:
- `ICategoryRepository`

---

## 🔗 MAPPING: CONTROLLER → ENDPOINT → SERVICE → DEPENDENCY

### Module: Authentication
```
AuthController
├── POST /api/v1/Auth/login
│   ├── IAuthService.LoginAsync
│   ├── → IUserRepository (find user by email)
│   ├── → IPasswordHasher (verify password)
│   ├── → IJwtTokenService (generate tokens)
│   └── → IAuditLogService (log login attempt)
│
├── POST /api/v1/auth/refresh
│   ├── IAuthService.RefreshTokenAsync
│   ├── → IJwtTokenService (validate & refresh token)
│   └── → IUserRepository (get user info)
│
└── POST /api/v1/auth/logout
    ├── IAuthService.LogoutAsync
    ├── → ITokenBlacklistService (blacklist token)
    └── → IAuditLogService (log logout)
```

### Module: Products & Inventory
```
ProductController
├── GET /api/v1/products
│   ├── IProductService.GetProductsAsync
│   ├── → IProductRepository (query products)
│   ├── → ICategoryRepository (join categories)
│   └── → IInventoryService (join stock levels)
│
└── POST /api/v1/products
    ├── IProductService.CreateProductAsync
    ├── → IProductRepository (save product)
    ├── → IFileStorageService (upload images)
    └── → IInventoryService (initialize stock)

InventoryController
├── GET /api/v1/inventory
│   ├── IInventoryService.GetInventoryAsync
│   ├── → IInventoryRepository (query inventory)
│   └── → IProductRepository (join product info)
│
└── POST /api/v1/inventory/adjustments
    ├── IInventoryService.AdjustStockAsync
    ├── → IInventoryRepository (update stock)
    ├── → IInventoryHistoryRepository (log adjustment)
    ├── → INotificationService (low stock alerts)
    └── → IWebSocketService (broadcast update)
```

### Module: Orders & POS
```
OrderController
├── POST /api/v1/orders
│   ├── IOrderService.CreateOrderAsync
│   ├── → IOrderRepository (save order)
│   ├── → IOrderItemRepository (save items)
│   ├── → IInventoryService (deduct stock)
│   ├── → ICustomerService (link customer)
│   ├── → INotificationService (notify staff)
│   └── → IWebSocketService (broadcast new order)
│
└── PUT /api/v1/orders/{id}/status
    ├── IOrderService.UpdateOrderStatusAsync
    ├── → IOrderRepository (update status)
    ├── → INotificationService (status change notification)
    └── → IWebSocketService (broadcast update)

PaymentController
├── POST /api/v1/pos/calculate
│   ├── IPaymentService.CalculateOrderTotalsAsync
│   ├── → IOrderService (get order items)
│   ├── → ITaxCalculator (calculate tax)
│   └── → IDiscountCalculator (apply discounts)
│
└── POST /api/v1/pos/process-payment
    ├── IPaymentService.ProcessPaymentAsync
    ├── → IPaymentRepository (save payment)
    ├── → IPaymentGatewayService (process card payment)
    ├── → IOrderService (update order status)
    ├── → IShiftService (update cash drawer)
    ├── → IReceiptService (generate receipt)
    └── → IWebSocketService (broadcast payment)
```

### Module: CRM
```
CustomerController
├── GET /api/v1/customers
│   ├── ICustomerService.GetCustomersAsync
│   ├── → ICustomerRepository (query customers)
│   └── → IOrderService (aggregate order stats)
│
└── POST /api/v1/customers/{id}/interactions
    ├── ICustomerService.CreateCustomerInteractionAsync
    ├── → ICRMInteractionRepository (save interaction)
    ├── → INotificationService (follow-up reminders)
    └── → IWebSocketService (notify managers)
```

### Module: Tasks
```
TaskController
├── POST /api/v1/tasks
│   ├── ITaskService.CreateTaskAsync
│   ├── → ITaskRepository (save task)
│   ├── → IUserService (validate assignee)
│   ├── → INotificationService (notify assignee)
│   └── → IWebSocketService (broadcast new task)
│
└── PUT /api/v1/tasks/{id}/status
    ├── ITaskService.UpdateTaskStatusAsync
    ├── → ITaskRepository (update status)
    ├── → INotificationService (notify creator)
    └── → IWebSocketService (broadcast update)
```

---

## 📦 PHÂN LOẠI THEO MODULE/FEATURE

### 🔴 CRITICAL MODULES (Cần triển khai đầu tiên)

#### 1. Authentication & Authorization
- **Controllers**: `AuthController`, `UserController`
- **Services**: `IAuthService`, `IUserService`
- **Endpoints**: 12 endpoints
- **Dependencies**: JWT, Password hashing, User repository

#### 2. Product & Inventory
- **Controllers**: `ProductController`, `InventoryController`, `CategoryController`
- **Services**: `IProductService`, `IInventoryService`, `ICategoryService`
- **Endpoints**: 15 endpoints
- **Dependencies**: Product/Inventory repositories, File storage, WebSocket

#### 3. Orders & POS
- **Controllers**: `OrderController`, `PaymentController`
- **Services**: `IOrderService`, `IPaymentService`
- **Endpoints**: 18 endpoints
- **Dependencies**: Order/Payment repositories, Payment gateway, Inventory service, WebSocket

#### 4. Shift Management
- **Controllers**: `ShiftController`
- **Services**: `IShiftService`
- **Endpoints**: 6 endpoints
- **Dependencies**: Shift repository, Payment service

---

### 🟡 HIGH PRIORITY MODULES

#### 5. Customer Management (CRM)
- **Controllers**: `CustomerController`
- **Services**: `ICustomerService`
- **Endpoints**: 10 endpoints
- **Dependencies**: Customer repository, Order service, Notification service

#### 6. Task Management
- **Controllers**: `TaskController`
- **Services**: `ITaskService`
- **Endpoints**: 9 endpoints
- **Dependencies**: Task repository, User service, Notification service, WebSocket

#### 7. Notifications
- **Controllers**: `NotificationController`
- **Services**: `INotificationService`
- **Endpoints**: 6 endpoints
- **Dependencies**: Notification repository, WebSocket service

---

### 🟢 MEDIUM PRIORITY MODULES

#### 8. Analytics & Reporting
- **Controllers**: `AnalyticsController`
- **Services**: `IAnalyticsService`
- **Endpoints**: 6 endpoints
- **Dependencies**: Multiple repositories (Orders, Products, Users, Customers)

#### 9. HRM (Human Resources)
- **Controllers**: `EmployeeController`, `LeaveRequestController`, `WorkScheduleController`
- **Services**: `IEmployeeService`, `ILeaveRequestService`, `IWorkScheduleService`
- **Endpoints**: 15 endpoints
- **Dependencies**: Employee/Leave/Schedule repositories

#### 10. LRM (Logistics & Resource Management)
- **Controllers**: `SupplierController`, `StockTransferController`
- **Services**: `ISupplierService`, `IStockTransferService`
- **Endpoints**: 9 endpoints
- **Dependencies**: Supplier/StockTransfer repositories, Inventory service

#### 11. Audit & System
- **Controllers**: `AuditLogController`
- **Services**: `IAuditLogService`
- **Endpoints**: 2 endpoints
- **Dependencies**: AuditLog repository

---

## 🎯 ĐÁNH DẤU ENDPOINT QUAN TRỌNG

### 🔴 CRITICAL ENDPOINTS (Bắt buộc để FE chạy đúng)

| Endpoint | Module | Lý do |
|----------|--------|-------|
| `POST /api/v1/Auth/login` | Auth | Đăng nhập - không có thì không vào được hệ thống |
| `GET /api/v1/products` | Product | Hiển thị sản phẩm trong POS và Inventory |
| `GET /api/v1/products/{id}` | Product | Chi tiết sản phẩm |
| `GET /api/v1/products/search` | Product | Tìm kiếm sản phẩm trong POS |
| `GET /api/v1/inventory` | Inventory | Hiển thị tồn kho |
| `GET /api/v1/inventory/product/{productId}` | Inventory | Kiểm tra stock khi bán |
| `POST /api/v1/orders` | Order | Tạo đơn hàng - chức năng chính của POS |
| `GET /api/v1/orders` | Order | Danh sách đơn hàng |
| `GET /api/v1/orders/{id}` | Order | Chi tiết đơn hàng |
| `POST /api/v1/pos/calculate` | Payment | Tính toán tổng tiền trước khi thanh toán |
| `POST /api/v1/pos/process-payment` | Payment | Xử lý thanh toán - chức năng core |
| `POST /api/v1/pos/cash-payment` | Payment | Thanh toán tiền mặt |
| `POST /api/v1/pos/card-payment` | Payment | Thanh toán thẻ |
| `GET /api/v1/shifts/current` | Shift | Lấy ca làm việc hiện tại |
| `POST /api/v1/shifts/open` | Shift | Mở ca làm việc |
| `PUT /api/v1/shifts/{id}/close` | Shift | Đóng ca làm việc |
| `GET /api/v1/users/profile` | User | Thông tin user hiện tại |
| `GET /api/v1/notifications` | Notification | Thông báo cho user |
| `GET /api/v1/tasks` | Task | Danh sách công việc |

**Tổng cộng: 19 endpoints CRITICAL**

---

### 🟡 HIGH PRIORITY ENDPOINTS (Quan trọng cho tính năng chính)

| Endpoint | Module | Lý do |
|----------|--------|-------|
| `POST /api/v1/auth/refresh` | Auth | Refresh token để duy trì session |
| `GET /api/v1/users` | User | Quản lý nhân viên (Manager) |
| `POST /api/v1/users` | User | Tạo nhân viên mới |
| `PUT /api/v1/inventory/{productId}` | Inventory | Điều chỉnh tồn kho |
| `POST /api/v1/inventory/adjustments` | Inventory | Ghi nhận điều chỉnh tồn kho |
| `GET /api/v1/inventory/alerts` | Inventory | Cảnh báo tồn kho thấp |
| `PUT /api/v1/orders/{id}/status` | Order | Cập nhật trạng thái đơn hàng |
| `POST /api/v1/orders/{id}/items` | Order | Thêm sản phẩm vào đơn |
| `GET /api/v1/customers` | CRM | Danh sách khách hàng |
| `POST /api/v1/customers` | CRM | Tạo khách hàng mới |
| `GET /api/v1/customers/search` | CRM | Tìm kiếm khách hàng |
| `POST /api/v1/tasks` | Task | Tạo công việc mới |
| `PUT /api/v1/tasks/{id}/status` | Task | Cập nhật trạng thái công việc |
| `PUT /api/v1/notifications/{id}/read` | Notification | Đánh dấu đã đọc |
| `GET /api/v1/analytics/dashboard` | Analytics | Dashboard metrics |

**Tổng cộng: 15 endpoints HIGH PRIORITY**

---

### 🟢 MEDIUM PRIORITY ENDPOINTS (Hỗ trợ, tùy chọn)

- Tất cả endpoints còn lại trong các module HRM, LRM, Audit, Export
- Các tính năng nâng cao như refund, void, export reports
- Quản lý suppliers, stock transfers, leave requests

**Tổng cộng: ~50 endpoints MEDIUM/LOW PRIORITY**

---

## 📊 TỔNG KẾT SỐ LƯỢNG

### Service Interfaces: **18 services**
1. IAuthService
2. IUserService
3. IProductService
4. IInventoryService
5. IOrderService
6. IPaymentService
7. ICustomerService
8. ISupplierService
9. IStockTransferService
10. IEmployeeService
11. ILeaveRequestService
12. IWorkScheduleService
13. IShiftService
14. ITaskService
15. INotificationService
16. IAnalyticsService
17. IAuditLogService
18. ICategoryService

### Controllers: **17 controllers**
1. AuthController
2. UserController
3. ProductController
4. InventoryController
5. OrderController
6. PaymentController
7. CustomerController
8. SupplierController
9. StockTransferController
10. EmployeeController
11. LeaveRequestController
12. WorkScheduleController
13. ShiftController
14. TaskController
15. NotificationController
16. AnalyticsController
17. AuditLogController
18. CategoryController

### Total Endpoints: **~95 endpoints**

- 🔴 **CRITICAL**: 19 endpoints
- 🟡 **HIGH**: 15 endpoints
- 🟢 **MEDIUM/LOW**: ~61 endpoints

---

## 🔄 DEPENDENCY GRAPH

### Core Dependencies (Shared Services)
```
IWebSocketService
├── Used by: InventoryService, OrderService, TaskService, NotificationService
└── Purpose: Real-time updates to frontend

INotificationService
├── Used by: OrderService, InventoryService, TaskService, CustomerService
└── Purpose: Send notifications to users

IAuditLogService
├── Used by: All services (via middleware)
└── Purpose: Log all critical actions

IFileStorageService
├── Used by: ProductService, UserService
└── Purpose: Store images/files

IPaymentGatewayService
├── Used by: PaymentService
└── Purpose: Process card/digital payments
```

### Repository Dependencies
```
All Services → Their respective Repositories
├── IUserRepository → UserService
├── IProductRepository → ProductService
├── IInventoryRepository → InventoryService
├── IOrderRepository → OrderService
├── IPaymentRepository → PaymentService
├── ICustomerRepository → CustomerService
├── ITaskRepository → TaskService
├── IShiftRepository → ShiftService
└── ... (one per entity)
```

---

## 📝 GHI CHÚ QUAN TRỌNG

1. **Pagination**: Tất cả endpoints GET list đều phải hỗ trợ pagination với format đã định nghĩa
2. **Authentication**: Tất cả endpoints (trừ login) đều cần JWT token trong header
3. **Permissions**: Mỗi endpoint cần check permissions theo role (Staff/Manager/Admin)
4. **Real-time**: Các thay đổi quan trọng (orders, inventory, tasks) cần broadcast qua WebSocket
5. **Error Handling**: Tất cả endpoints phải trả về format error chuẩn
6. **Validation**: Input validation cho tất cả requests
7. **Audit Logging**: Log tất cả CREATE/UPDATE/DELETE operations

---

## 🚀 KHUYẾN NGHỊ TRIỂN KHAI

### Phase 1 (Week 1-2): Foundation
1. ✅ Database schema
2. ✅ Authentication (AuthController + IAuthService)
3. ✅ User management (UserController + IUserService)
4. ✅ Basic CRUD cho Products, Inventory, Categories

### Phase 2 (Week 3-4): Core POS
1. ✅ Orders (OrderController + IOrderService)
2. ✅ Payments (PaymentController + IPaymentService)
3. ✅ Shifts (ShiftController + IShiftService)
4. ✅ WebSocket setup

### Phase 3 (Week 5-6): Supporting Features
1. ✅ CRM (CustomerController + ICustomerService)
2. ✅ Tasks (TaskController + ITaskService)
3. ✅ Notifications (NotificationController + INotificationService)
4. ✅ Analytics (AnalyticsController + IAnalyticsService)

### Phase 4 (Week 7-8): Advanced Features
1. ✅ HRM modules
2. ✅ LRM modules
3. ✅ Audit logs
4. ✅ Export functionality

---

**Tài liệu này được tạo tự động từ phân tích codebase Frontend.**
**Cập nhật: 2024**

