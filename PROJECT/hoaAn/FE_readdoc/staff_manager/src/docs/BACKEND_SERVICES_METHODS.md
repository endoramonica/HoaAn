# Backend Services Methods & Controllers

## 1. ManagerServices Class

### User & Staff Management
```typescript
// Staff Operations
async createStaff(staffData: CreateStaffRequest): Promise<StaffResponse>
async updateStaff(staffId: string, updateData: UpdateStaffRequest): Promise<StaffResponse>
async deleteStaff(staffId: string): Promise<void>
async getAllStaff(pagination: PaginationParams): Promise<PaginatedResponse<StaffResponse>>
async getStaffById(staffId: string): Promise<StaffResponse>
async activateStaff(staffId: string): Promise<void>
async deactivateStaff(staffId: string): Promise<void>
async assignStaffRole(staffId: string, role: UserRole): Promise<void>
async getStaffPermissions(staffId: string): Promise<PermissionResponse[]>
async updateStaffPermissions(staffId: string, permissions: string[]): Promise<void>

// Shift Management (Manager Level)
async createShift(shiftData: CreateShiftRequest): Promise<ShiftResponse>
async updateShift(shiftId: string, updateData: UpdateShiftRequest): Promise<ShiftResponse>
async deleteShift(shiftId: string): Promise<void>
async getAllShifts(pagination: PaginationParams, filters?: ShiftFilters): Promise<PaginatedResponse<ShiftResponse>>
async assignStaffToShift(shiftId: string, staffIds: string[]): Promise<void>
async removeStaffFromShift(shiftId: string, staffId: string): Promise<void>
async approveShiftChange(shiftChangeId: string): Promise<void>
async rejectShiftChange(shiftChangeId: string, reason: string): Promise<void>
```

### Analytics & Reporting
```typescript
// Sales Analytics
async getDashboardMetrics(): Promise<ManagerDashboardMetrics>
async getSalesReport(period: ReportPeriod, filters?: SalesFilters): Promise<SalesReportResponse>
async getRevenueAnalytics(startDate: Date, endDate: Date): Promise<RevenueAnalyticsResponse>
async getProductPerformance(period: ReportPeriod): Promise<ProductPerformanceResponse[]>
async getStaffPerformance(period: ReportPeriod): Promise<StaffPerformanceResponse[]>
async getCustomerAnalytics(period: ReportPeriod): Promise<CustomerAnalyticsResponse>

// Financial Reports
async getFinancialSummary(period: ReportPeriod): Promise<FinancialSummaryResponse>
async getProfitLossReport(startDate: Date, endDate: Date): Promise<ProfitLossResponse>
async getCashFlowReport(period: ReportPeriod): Promise<CashFlowResponse>
async getExpenseReport(period: ReportPeriod): Promise<ExpenseReportResponse>

// Export Functionality
async exportSalesReport(period: ReportPeriod, format: 'PDF' | 'Excel'): Promise<ExportResponse>
async exportInventoryReport(format: 'PDF' | 'Excel'): Promise<ExportResponse>
async exportStaffReport(period: ReportPeriod, format: 'PDF' | 'Excel'): Promise<ExportResponse>
```

### Inventory Management (Full Access)
```typescript
// Product Management
async createProduct(productData: CreateProductRequest): Promise<ProductResponse>
async updateProduct(productId: string, updateData: UpdateProductRequest): Promise<ProductResponse>
async deleteProduct(productId: string): Promise<void>
async getAllProducts(pagination: PaginationParams, filters?: ProductFilters): Promise<PaginatedResponse<ProductResponse>>
async getProductById(productId: string): Promise<ProductResponse>
async bulkUpdateProducts(updates: BulkProductUpdate[]): Promise<BulkUpdateResponse>

// Stock Management
async adjustStock(productId: string, adjustment: StockAdjustmentRequest): Promise<StockResponse>
async performStockAudit(auditData: StockAuditRequest): Promise<StockAuditResponse>
async getStockHistory(productId: string, pagination: PaginationParams): Promise<PaginatedResponse<StockHistoryResponse>>
async getLowStockAlert(): Promise<LowStockAlertResponse[]>
async setStockLevels(productId: string, levels: StockLevelRequest): Promise<void>

// Category Management
async createCategory(categoryData: CreateCategoryRequest): Promise<CategoryResponse>
async updateCategory(categoryId: string, updateData: UpdateCategoryRequest): Promise<CategoryResponse>
async deleteCategory(categoryId: string): Promise<void>
async getAllCategories(): Promise<CategoryResponse[]>
```

### Order Management (Full Access)
```typescript
// Order Operations
async getAllOrders(pagination: PaginationParams, filters?: OrderFilters): Promise<PaginatedResponse<OrderResponse>>
async getOrderById(orderId: string): Promise<OrderResponse>
async updateOrderStatus(orderId: string, status: OrderStatus): Promise<OrderResponse>
async refundOrder(orderId: string, refundData: RefundRequest): Promise<RefundResponse>
async voidOrder(orderId: string, reason: string): Promise<void>

// Payment Management
async getPaymentHistory(pagination: PaginationParams, filters?: PaymentFilters): Promise<PaginatedResponse<PaymentResponse>>
async processRefund(paymentId: string, refundAmount: number, reason: string): Promise<RefundResponse>
async reconcilePayments(date: Date): Promise<PaymentReconciliationResponse>
```

### CRM Management (Full Access)
```typescript
// Customer Management
async createCustomer(customerData: CreateCustomerRequest): Promise<CustomerResponse>
async updateCustomer(customerId: string, updateData: UpdateCustomerRequest): Promise<CustomerResponse>
async deleteCustomer(customerId: string): Promise<void>
async getAllCustomers(pagination: PaginationParams, filters?: CustomerFilters): Promise<PaginatedResponse<CustomerResponse>>
async getCustomerById(customerId: string): Promise<CustomerResponse>
async getCustomerOrders(customerId: string, pagination: PaginationParams): Promise<PaginatedResponse<OrderResponse>>

// Customer Interaction
async createCustomerInteraction(interactionData: CreateInteractionRequest): Promise<InteractionResponse>
async updateCustomerInteraction(interactionId: string, updateData: UpdateInteractionRequest): Promise<InteractionResponse>
async getCustomerInteractions(customerId: string, pagination: PaginationParams): Promise<PaginatedResponse<InteractionResponse>>

// Loyalty Program
async createLoyaltyProgram(programData: CreateLoyaltyProgramRequest): Promise<LoyaltyProgramResponse>
async updateLoyaltyProgram(programId: string, updateData: UpdateLoyaltyProgramRequest): Promise<LoyaltyProgramResponse>
async enrollCustomerInLoyalty(customerId: string, programId: string): Promise<void>
async processLoyaltyRedemption(customerId: string, pointsToRedeem: number): Promise<LoyaltyRedemptionResponse>
```

### Task Management (Full Access)
```typescript
// Task Operations
async createTask(taskData: CreateTaskRequest): Promise<TaskResponse>
async updateTask(taskId: string, updateData: UpdateTaskRequest): Promise<TaskResponse>
async deleteTask(taskId: string): Promise<void>
async getAllTasks(pagination: PaginationParams, filters?: TaskFilters): Promise<PaginatedResponse<TaskResponse>>
async getTaskById(taskId: string): Promise<TaskResponse>
async assignTask(taskId: string, assigneeId: string): Promise<void>
async completeTask(taskId: string, completionNote?: string): Promise<void>
async getTasksByAssignee(assigneeId: string, pagination: PaginationParams): Promise<PaginatedResponse<TaskResponse>>
```

## 2. StaffServices Class

### Personal Management
```typescript
// Profile Management
async getMyProfile(): Promise<StaffProfileResponse>
async updateMyProfile(updateData: UpdateProfileRequest): Promise<StaffProfileResponse>
async changePassword(passwordData: ChangePasswordRequest): Promise<void>
async getMyPermissions(): Promise<PermissionResponse[]>
```

### Shift Management (Own Shifts)
```typescript
// Shift Operations
async getMyShifts(pagination: PaginationParams, filters?: ShiftFilters): Promise<PaginatedResponse<ShiftResponse>>
async clockIn(shiftId: string): Promise<ClockInResponse>
async clockOut(shiftId: string): Promise<ClockOutResponse>
async takeBreak(shiftId: string): Promise<BreakResponse>
async endBreak(shiftId: string): Promise<BreakResponse>
async requestShiftChange(shiftChangeData: ShiftChangeRequest): Promise<ShiftChangeResponse>
async getMyAttendance(startDate: Date, endDate: Date): Promise<AttendanceResponse[]>
```

### POS Operations
```typescript
// Order Processing
async createOrder(orderData: CreateOrderRequest): Promise<OrderResponse>
async updateOrder(orderId: string, updateData: UpdateOrderRequest): Promise<OrderResponse>
async addItemToOrder(orderId: string, itemData: OrderItemRequest): Promise<OrderResponse>
async removeItemFromOrder(orderId: string, itemId: string): Promise<OrderResponse>
async applyDiscount(orderId: string, discountData: DiscountRequest): Promise<OrderResponse>

// Payment Processing
async processPayment(orderId: string, paymentData: PaymentRequest): Promise<PaymentResponse>
async processCashPayment(orderId: string, cashData: CashPaymentRequest): Promise<PaymentResponse>
async processCardPayment(orderId: string, cardData: CardPaymentRequest): Promise<PaymentResponse>
async voidPayment(paymentId: string, reason: string): Promise<void>
```

### Order Management (Own Orders)
```typescript
// Order Operations
async getMyOrders(pagination: PaginationParams, filters?: OrderFilters): Promise<PaginatedResponse<OrderResponse>>
async getOrderById(orderId: string): Promise<OrderResponse>
async updateOrderStatus(orderId: string, status: OrderStatus): Promise<OrderResponse>
```

### Inventory Management (Limited Access)
```typescript
// Product Operations
async getProducts(pagination: PaginationParams, filters?: ProductFilters): Promise<PaginatedResponse<ProductResponse>>
async getProductById(productId: string): Promise<ProductResponse>
async searchProducts(searchTerm: string): Promise<ProductResponse[]>
async getProductsByCategory(categoryId: string): Promise<ProductResponse[]>
async getCategories(): Promise<CategoryResponse[]>

// Stock Operations (Limited)
async checkStock(productId: string): Promise<StockResponse>
async requestStockAdjustment(adjustmentData: StockAdjustmentRequest): Promise<StockAdjustmentRequestResponse>
```

### Customer Management (Limited Access)
```typescript
// Customer Operations
async getCustomers(pagination: PaginationParams, filters?: CustomerFilters): Promise<PaginatedResponse<CustomerResponse>>
async getCustomerById(customerId: string): Promise<CustomerResponse>
async createCustomer(customerData: CreateCustomerRequest): Promise<CustomerResponse>
async updateCustomer(customerId: string, updateData: UpdateCustomerRequest): Promise<CustomerResponse>
async searchCustomers(searchTerm: string): Promise<CustomerResponse[]>

// Customer Interaction
async createCustomerInteraction(interactionData: CreateInteractionRequest): Promise<InteractionResponse>
async getCustomerInteractions(customerId: string, pagination: PaginationParams): Promise<PaginatedResponse<InteractionResponse>>
```

### Task Management (Own Tasks)
```typescript
// Task Operations
async getMyTasks(pagination: PaginationParams, filters?: TaskFilters): Promise<PaginatedResponse<TaskResponse>>
async getTaskById(taskId: string): Promise<TaskResponse>
async updateTaskStatus(taskId: string, status: TaskStatus): Promise<TaskResponse>
async completeTask(taskId: string, completionNote?: string): Promise<void>
async addTaskComment(taskId: string, comment: string): Promise<TaskCommentResponse>
```

## 3. Required Controllers

### AuthController
```typescript
class AuthController {
  async login(req: LoginRequest): Promise<AuthResponse>
  async logout(req: LogoutRequest): Promise<void>
  async refreshToken(req: RefreshTokenRequest): Promise<AuthResponse>
  async forgotPassword(req: ForgotPasswordRequest): Promise<void>
  async resetPassword(req: ResetPasswordRequest): Promise<void>
  async validateToken(req: ValidateTokenRequest): Promise<TokenValidationResponse>
}
```

### ManagerController
```typescript
class ManagerController {
  // Staff Management
  async createStaff(req: Request): Promise<Response>
  async updateStaff(req: Request): Promise<Response>
  async deleteStaff(req: Request): Promise<Response>
  async getAllStaff(req: Request): Promise<Response>
  async getStaffById(req: Request): Promise<Response>
  
  // Analytics
  async getDashboardMetrics(req: Request): Promise<Response>
  async getSalesReport(req: Request): Promise<Response>
  async getRevenueAnalytics(req: Request): Promise<Response>
  async exportReport(req: Request): Promise<Response>
  
  // Inventory (Full Access)
  async createProduct(req: Request): Promise<Response>
  async updateProduct(req: Request): Promise<Response>
  async deleteProduct(req: Request): Promise<Response>
  async adjustStock(req: Request): Promise<Response>
  
  // Order Management (Full Access)
  async getAllOrders(req: Request): Promise<Response>
  async refundOrder(req: Request): Promise<Response>
  async voidOrder(req: Request): Promise<Response>
  
  // Task Management (Full Access)
  async createTask(req: Request): Promise<Response>
  async assignTask(req: Request): Promise<Response>
  async getAllTasks(req: Request): Promise<Response>
}
```

### StaffController
```typescript
class StaffController {
  // Profile Management
  async getMyProfile(req: Request): Promise<Response>
  async updateMyProfile(req: Request): Promise<Response>
  async changePassword(req: Request): Promise<Response>
  
  // POS Operations
  async createOrder(req: Request): Promise<Response>
  async processPayment(req: Request): Promise<Response>
  async getProducts(req: Request): Promise<Response>
  
  // Order Management (Own Orders)
  async getMyOrders(req: Request): Promise<Response>
  async updateOrderStatus(req: Request): Promise<Response>
  
  // Customer Management (Limited)
  async getCustomers(req: Request): Promise<Response>
  async createCustomer(req: Request): Promise<Response>
  async createCustomerInteraction(req: Request): Promise<Response>
  
  // Task Management (Own Tasks)
  async getMyTasks(req: Request): Promise<Response>
  async updateTaskStatus(req: Request): Promise<Response>
  async completeTask(req: Request): Promise<Response>
  
  // Shift Management
  async getMyShifts(req: Request): Promise<Response>
  async clockIn(req: Request): Promise<Response>
  async clockOut(req: Request): Promise<Response>
}
```

### SharedController (Common Operations)
```typescript
class SharedController {
  // Notifications
  async getNotifications(req: Request): Promise<Response>
  async markNotificationAsRead(req: Request): Promise<Response>
  async markAllNotificationsAsRead(req: Request): Promise<Response>
  
  // General
  async getCategories(req: Request): Promise<Response>
  async getDashboardData(req: Request): Promise<Response>
  async uploadFile(req: Request): Promise<Response>
}
```

## 4. Additional Specialized Controllers

### InventoryController
```typescript
class InventoryController {
  async getProducts(req: Request): Promise<Response>
  async getProductById(req: Request): Promise<Response>
  async createProduct(req: Request): Promise<Response> // Manager only
  async updateProduct(req: Request): Promise<Response> // Manager only
  async deleteProduct(req: Request): Promise<Response> // Manager only
  async adjustStock(req: Request): Promise<Response>
  async getStockHistory(req: Request): Promise<Response>
  async getLowStockAlerts(req: Request): Promise<Response>
}
```

### OrderController
```typescript
class OrderController {
  async createOrder(req: Request): Promise<Response>
  async getOrders(req: Request): Promise<Response>
  async getOrderById(req: Request): Promise<Response>
  async updateOrderStatus(req: Request): Promise<Response>
  async refundOrder(req: Request): Promise<Response> // Manager only
  async voidOrder(req: Request): Promise<Response> // Manager only
}
```

### PaymentController
```typescript
class PaymentController {
  async processPayment(req: Request): Promise<Response>
  async processCashPayment(req: Request): Promise<Response>
  async processCardPayment(req: Request): Promise<Response>
  async getPaymentHistory(req: Request): Promise<Response>
  async processRefund(req: Request): Promise<Response> // Manager only
}
```

### CustomerController
```typescript
class CustomerController {
  async getCustomers(req: Request): Promise<Response>
  async getCustomerById(req: Request): Promise<Response>
  async createCustomer(req: Request): Promise<Response>
  async updateCustomer(req: Request): Promise<Response>
  async deleteCustomer(req: Request): Promise<Response> // Manager only
  async getCustomerOrders(req: Request): Promise<Response>
  async createInteraction(req: Request): Promise<Response>
  async getInteractions(req: Request): Promise<Response>
}
```

### TaskController
```typescript
class TaskController {
  async getTasks(req: Request): Promise<Response>
  async getTaskById(req: Request): Promise<Response>
  async createTask(req: Request): Promise<Response> // Manager only
  async updateTask(req: Request): Promise<Response>
  async deleteTask(req: Request): Promise<Response> // Manager only
  async assignTask(req: Request): Promise<Response> // Manager only
  async completeTask(req: Request): Promise<Response>
}
```

## 5. Key Implementation Notes

### Permission-Based Access
- Mỗi method cần check permissions trước khi thực thi
- Staff chỉ có thể truy cập dữ liệu của mình hoặc dữ liệu công khai
- Manager có full access nhưng vẫn cần validate business rules

### Error Handling
- Implement consistent error handling across all services
- Return appropriate HTTP status codes
- Provide clear error messages for frontend

### Validation
- Input validation cho tất cả requests
- Business rule validation
- Permission validation

### Logging & Audit
- Log tất cả critical operations
- Audit trail cho sensitive actions
- Performance monitoring

### Rate Limiting
- Implement rate limiting cho API endpoints
- Different limits for different roles
- Protect against abuse

## 6. Suggested Implementation Order

1. **Phase 1**: AuthController + Basic User Management
2. **Phase 2**: InventoryController + ProductController  
3. **Phase 3**: OrderController + PaymentController
4. **Phase 4**: CustomerController + Basic CRM
5. **Phase 5**: TaskController + Staff Management
6. **Phase 6**: Analytics + Reporting (Manager)
7. **Phase 7**: Advanced features (LRM, HRM, etc.)