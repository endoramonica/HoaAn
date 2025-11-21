# BACKEND ENDPOINTS - QUICK REFERENCE

## 🔴 CRITICAL ENDPOINTS (19 endpoints - Bắt buộc)

| # | Method | Endpoint | Controller | Service | Module |
|---|--------|----------|------------|--------|--------|
| 1 | POST | `/api/v1/Auth/login` | AuthController | IAuthService | Auth |
| 2 | GET | `/api/v1/products` | ProductController | IProductService | Product |
| 3 | GET | `/api/v1/products/{id}` | ProductController | IProductService | Product |
| 4 | GET | `/api/v1/products/search` | ProductController | IProductService | Product |
| 5 | GET | `/api/v1/inventory` | InventoryController | IInventoryService | Inventory |
| 6 | GET | `/api/v1/inventory/product/{productId}` | InventoryController | IInventoryService | Inventory |
| 7 | POST | `/api/v1/orders` | OrderController | IOrderService | Order |
| 8 | GET | `/api/v1/orders` | OrderController | IOrderService | Order |
| 9 | GET | `/api/v1/orders/{id}` | OrderController | IOrderService | Order |
| 10 | POST | `/api/v1/pos/calculate` | PaymentController | IPaymentService | Payment |
| 11 | POST | `/api/v1/pos/process-payment` | PaymentController | IPaymentService | Payment |
| 12 | POST | `/api/v1/pos/cash-payment` | PaymentController | IPaymentService | Payment |
| 13 | POST | `/api/v1/pos/card-payment` | PaymentController | IPaymentService | Payment |
| 14 | GET | `/api/v1/shifts/current` | ShiftController | IShiftService | Shift |
| 15 | POST | `/api/v1/shifts/open` | ShiftController | IShiftService | Shift |
| 16 | PUT | `/api/v1/shifts/{id}/close` | ShiftController | IShiftService | Shift |
| 17 | GET | `/api/v1/users/profile` | UserController | IUserService | User |
| 18 | GET | `/api/v1/notifications` | NotificationController | INotificationService | Notification |
| 19 | GET | `/api/v1/tasks` | TaskController | ITaskService | Task |

---

## 🟡 HIGH PRIORITY ENDPOINTS (15 endpoints)

| # | Method | Endpoint | Controller | Service | Module |
|---|--------|----------|------------|--------|--------|
| 20 | POST | `/api/v1/auth/refresh` | AuthController | IAuthService | Auth |
| 21 | GET | `/api/v1/users` | UserController | IUserService | User |
| 22 | POST | `/api/v1/users` | UserController | IUserService | User |
| 23 | PUT | `/api/v1/inventory/{productId}` | InventoryController | IInventoryService | Inventory |
| 24 | POST | `/api/v1/inventory/adjustments` | InventoryController | IInventoryService | Inventory |
| 25 | GET | `/api/v1/inventory/alerts` | InventoryController | IInventoryService | Inventory |
| 26 | PUT | `/api/v1/orders/{id}/status` | OrderController | IOrderService | Order |
| 27 | POST | `/api/v1/orders/{id}/items` | OrderController | IOrderService | Order |
| 28 | GET | `/api/v1/customers` | CustomerController | ICustomerService | CRM |
| 29 | POST | `/api/v1/customers` | CustomerController | ICustomerService | CRM |
| 30 | GET | `/api/v1/customers/search` | CustomerController | ICustomerService | CRM |
| 31 | POST | `/api/v1/tasks` | TaskController | ITaskService | Task |
| 32 | PUT | `/api/v1/tasks/{id}/status` | TaskController | ITaskService | Task |
| 33 | PUT | `/api/v1/notifications/{id}/read` | NotificationController | INotificationService | Notification |
| 34 | GET | `/api/v1/analytics/dashboard` | AnalyticsController | IAnalyticsService | Analytics |

---

## 🟢 MEDIUM PRIORITY ENDPOINTS (61 endpoints)

### Authentication (2)
| Method | Endpoint | Controller | Service |
|--------|----------|------------|---------|
| POST | `/api/v1/auth/logout` | AuthController | IAuthService |
| POST | `/api/v1/auth/change-password` | AuthController | IAuthService |

### User Management (6)
| Method | Endpoint | Controller | Service |
|--------|----------|------------|---------|
| GET | `/api/v1/users/{id}` | UserController | IUserService |
| PUT | `/api/v1/users/{id}` | UserController | IUserService |
| DELETE | `/api/v1/users/{id}` | UserController | IUserService |
| PUT | `/api/v1/users/profile` | UserController | IUserService |
| PUT | `/api/v1/users/{id}/activate` | UserController | IUserService |
| PUT | `/api/v1/users/{id}/deactivate` | UserController | IUserService |

### Products (3)
| Method | Endpoint | Controller | Service |
|--------|----------|------------|---------|
| POST | `/api/v1/products` | ProductController | IProductService |
| PUT | `/api/v1/products/{id}` | ProductController | IProductService |
| DELETE | `/api/v1/products/{id}` | ProductController | IProductService |
| GET | `/api/v1/products/category/{categoryId}` | ProductController | IProductService |

### Inventory (3)
| Method | Endpoint | Controller | Service |
|--------|----------|------------|---------|
| GET | `/api/v1/inventory/{productId}/history` | InventoryController | IInventoryService |
| PUT | `/api/v1/inventory/{productId}/levels` | InventoryController | IInventoryService |

### Orders (7)
| Method | Endpoint | Controller | Service |
|--------|----------|------------|---------|
| PUT | `/api/v1/orders/{id}` | OrderController | IOrderService |
| DELETE | `/api/v1/orders/{id}` | OrderController | IOrderService |
| POST | `/api/v1/orders/{id}/refund` | OrderController | IOrderService |
| POST | `/api/v1/orders/{id}/void` | OrderController | IOrderService |
| DELETE | `/api/v1/orders/{id}/items/{itemId}` | OrderController | IOrderService |
| POST | `/api/v1/orders/{id}/discount` | OrderController | IOrderService |

### Payments (3)
| Method | Endpoint | Controller | Service |
|--------|----------|------------|---------|
| POST | `/api/v1/payments/{id}/void` | PaymentController | IPaymentService |
| GET | `/api/v1/payments` | PaymentController | IPaymentService |
| POST | `/api/v1/payments/{id}/refund` | PaymentController | IPaymentService |

### Customers/CRM (6)
| Method | Endpoint | Controller | Service |
|--------|----------|------------|---------|
| GET | `/api/v1/customers/{id}` | CustomerController | ICustomerService |
| PUT | `/api/v1/customers/{id}` | CustomerController | ICustomerService |
| DELETE | `/api/v1/customers/{id}` | CustomerController | ICustomerService |
| GET | `/api/v1/customers/{id}/orders` | CustomerController | ICustomerService |
| GET | `/api/v1/customers/{id}/interactions` | CustomerController | ICustomerService |
| POST | `/api/v1/customers/{id}/interactions` | CustomerController | ICustomerService |
| PUT | `/api/v1/customers/interactions/{id}` | CustomerController | ICustomerService |

### Suppliers/LRM (5)
| Method | Endpoint | Controller | Service |
|--------|----------|------------|---------|
| GET | `/api/v1/suppliers` | SupplierController | ISupplierService |
| GET | `/api/v1/suppliers/{id}` | SupplierController | ISupplierService |
| POST | `/api/v1/suppliers` | SupplierController | ISupplierService |
| PUT | `/api/v1/suppliers/{id}` | SupplierController | ISupplierService |
| DELETE | `/api/v1/suppliers/{id}` | SupplierController | ISupplierService |

### Stock Transfers/LRM (5)
| Method | Endpoint | Controller | Service |
|--------|----------|------------|---------|
| GET | `/api/v1/stock-transfers` | StockTransferController | IStockTransferService |
| GET | `/api/v1/stock-transfers/{id}` | StockTransferController | IStockTransferService |
| POST | `/api/v1/stock-transfers` | StockTransferController | IStockTransferService |
| PUT | `/api/v1/stock-transfers/{id}/status` | StockTransferController | IStockTransferService |
| DELETE | `/api/v1/stock-transfers/{id}` | StockTransferController | IStockTransferService |

### Employees/HRM (5)
| Method | Endpoint | Controller | Service |
|--------|----------|------------|---------|
| GET | `/api/v1/employees` | EmployeeController | IEmployeeService |
| GET | `/api/v1/employees/{id}` | EmployeeController | IEmployeeService |
| POST | `/api/v1/employees` | EmployeeController | IEmployeeService |
| PUT | `/api/v1/employees/{id}` | EmployeeController | IEmployeeService |
| DELETE | `/api/v1/employees/{id}` | EmployeeController | IEmployeeService |

### Leave Requests/HRM (4)
| Method | Endpoint | Controller | Service |
|--------|----------|------------|---------|
| GET | `/api/v1/leave-requests` | LeaveRequestController | ILeaveRequestService |
| GET | `/api/v1/leave-requests/{id}` | LeaveRequestController | ILeaveRequestService |
| POST | `/api/v1/leave-requests` | LeaveRequestController | ILeaveRequestService |
| PUT | `/api/v1/leave-requests/{id}/status` | LeaveRequestController | ILeaveRequestService |

### Work Schedules/HRM (5)
| Method | Endpoint | Controller | Service |
|--------|----------|------------|---------|
| GET | `/api/v1/work-schedules` | WorkScheduleController | IWorkScheduleService |
| GET | `/api/v1/work-schedules/{id}` | WorkScheduleController | IWorkScheduleService |
| POST | `/api/v1/work-schedules` | WorkScheduleController | IWorkScheduleService |
| PUT | `/api/v1/work-schedules/{id}` | WorkScheduleController | IWorkScheduleService |
| DELETE | `/api/v1/work-schedules/{id}` | WorkScheduleController | IWorkScheduleService |

### Shifts (3)
| Method | Endpoint | Controller | Service |
|--------|----------|------------|---------|
| GET | `/api/v1/shifts` | ShiftController | IShiftService |
| GET | `/api/v1/shifts/{id}` | ShiftController | IShiftService |
| PUT | `/api/v1/shifts/{id}` | ShiftController | IShiftService |

### Tasks (7)
| Method | Endpoint | Controller | Service |
|--------|----------|------------|---------|
| GET | `/api/v1/tasks/{id}` | TaskController | ITaskService |
| PUT | `/api/v1/tasks/{id}` | TaskController | ITaskService |
| DELETE | `/api/v1/tasks/{id}` | TaskController | ITaskService |
| POST | `/api/v1/tasks/{id}/assign` | TaskController | ITaskService |
| POST | `/api/v1/tasks/{id}/complete` | TaskController | ITaskService |
| GET | `/api/v1/tasks/assignee/{assigneeId}` | TaskController | ITaskService |

### Notifications (4)
| Method | Endpoint | Controller | Service |
|--------|----------|------------|---------|
| GET | `/api/v1/notifications/{id}` | NotificationController | INotificationService |
| PUT | `/api/v1/notifications/read-all` | NotificationController | INotificationService |
| DELETE | `/api/v1/notifications/{id}` | NotificationController | INotificationService |
| GET | `/api/v1/notifications/unread-count` | NotificationController | INotificationService |

### Analytics (5)
| Method | Endpoint | Controller | Service |
|--------|----------|------------|---------|
| GET | `/api/v1/analytics/sales` | AnalyticsController | IAnalyticsService |
| GET | `/api/v1/analytics/revenue` | AnalyticsController | IAnalyticsService |
| GET | `/api/v1/analytics/products` | AnalyticsController | IAnalyticsService |
| GET | `/api/v1/analytics/staff` | AnalyticsController | IAnalyticsService |
| GET | `/api/v1/analytics/customers` | AnalyticsController | IAnalyticsService |

### Audit Logs (2)
| Method | Endpoint | Controller | Service |
|--------|----------|------------|---------|
| GET | `/api/v1/audit-logs` | AuditLogController | IAuditLogService |
| GET | `/api/v1/audit-logs/{id}` | AuditLogController | IAuditLogService |

### Categories (5)
| Method | Endpoint | Controller | Service |
|--------|----------|------------|---------|
| GET | `/api/v1/categories` | CategoryController | ICategoryService |
| GET | `/api/v1/categories/{id}` | CategoryController | ICategoryService |
| POST | `/api/v1/categories` | CategoryController | ICategoryService |
| PUT | `/api/v1/categories/{id}` | CategoryController | ICategoryService |
| DELETE | `/api/v1/categories/{id}` | CategoryController | ICategoryService |

---

## 📊 SERVICE INTERFACES SUMMARY

| # | Service Interface | Methods | Used By Controllers |
|---|-------------------|---------|---------------------|
| 1 | IAuthService | 5 | AuthController |
| 2 | IUserService | 12 | UserController |
| 3 | IProductService | 7 | ProductController |
| 4 | IInventoryService | 7 | InventoryController |
| 5 | IOrderService | 11 | OrderController |
| 6 | IPaymentService | 7 | PaymentController |
| 7 | ICustomerService | 10 | CustomerController |
| 8 | ISupplierService | 5 | SupplierController |
| 9 | IStockTransferService | 5 | StockTransferController |
| 10 | IEmployeeService | 5 | EmployeeController |
| 11 | ILeaveRequestService | 4 | LeaveRequestController |
| 12 | IWorkScheduleService | 5 | WorkScheduleController |
| 13 | IShiftService | 6 | ShiftController |
| 14 | ITaskService | 9 | TaskController |
| 15 | INotificationService | 6 | NotificationController |
| 16 | IAnalyticsService | 6 | AnalyticsController |
| 17 | IAuditLogService | 2 | AuditLogController |
| 18 | ICategoryService | 5 | CategoryController |

**Total: 18 Service Interfaces, 117 Methods**

---

## 🎯 PRIORITY BREAKDOWN

- 🔴 **CRITICAL**: 19 endpoints (20%)
- 🟡 **HIGH**: 15 endpoints (16%)
- 🟢 **MEDIUM**: 61 endpoints (64%)

**Total: 95 endpoints**

---

## 🔗 DEPENDENCY SUMMARY

### Shared Services (Used by multiple modules)
- `IWebSocketService` → Real-time updates
- `INotificationService` → User notifications
- `IAuditLogService` → Action logging
- `IFileStorageService` → File/image storage
- `IPaymentGatewayService` → Payment processing

### Core Repositories (One per entity)
- UserRepository, ProductRepository, InventoryRepository
- OrderRepository, PaymentRepository, CustomerRepository
- TaskRepository, ShiftRepository, NotificationRepository
- ... (18 repositories total)

---

## 📝 NOTES

1. **Base URL**: `https://localhost:7131/api/v1`
2. **Auth Header**: `Authorization: Bearer {token}`
3. **Pagination**: All GET list endpoints support pagination
4. **Response Format**: Standardized success/error format
5. **Real-time**: WebSocket for critical updates (orders, inventory, tasks)

---

**Quick Reference - Generated from Frontend Analysis**

