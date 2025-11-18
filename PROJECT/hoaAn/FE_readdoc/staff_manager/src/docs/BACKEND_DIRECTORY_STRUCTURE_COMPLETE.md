# VietCommerce Backend Complete Directory Structure
## Comprehensive Backend Architecture for Full-Stack POS System

Based on complete frontend code analysis, here's the enhanced backend directory structure with all required entities, DTOs, services, and controllers to support all frontend features.

---

## Complete Directory Structure

```
VietCommerce.sln
├── VietCommerce.Core              # Class Library (Domain + Application)
│   ├── Entities                   # Domain Entities
│   │   ├── Common
│   │   │   ├── BaseEntity.cs             # Base entity với Id, CreatedAt, UpdatedAt
│   │   │   ├── AuditableEntity.cs        # Entity với audit fields
│   │   │   └── ISoftDelete.cs            # Interface cho soft delete
│   │   │
│   │   ├── Organization
│   │   │   ├── Tenant.cs                 # Multi-tenant support
│   │   │   └── Store.cs                  # Store/Location entity
│   │   │
│   │   ├── Users
│   │   │   ├── User.cs                   # User entity với role-based access
│   │   │   ├── Role.cs                   # Roles (Staff, Manager, Admin)
│   │   │   ├── Permission.cs             # Granular permissions
│   │   │   ├── UserRole.cs               # User-Role mapping
│   │   │   ├── UserPermission.cs         # User-specific permissions
│   │   │   ├── RolePermission.cs         # Role-Permission mapping
│   │   │   └── RefreshToken.cs           # JWT refresh tokens
│   │   │
│   │   ├── Customers
│   │   │   ├── Customer.cs               # Customer entity với loyalty info
│   │   │   ├── CustomerInteraction.cs    # CRM interactions
│   │   │   ├── CustomerAddress.cs        # Customer addresses
│   │   │   ├── CustomerNote.cs           # Customer notes/comments
│   │   │   └── CustomerTag.cs            # Customer tagging system
│   │   │
│   │   ├── Products
│   │   │   ├── Category.cs               # Product categories (hierarchical)
│   │   │   ├── Product.cs                # Product master data
│   │   │   ├── ProductImage.cs           # Product images
│   │   │   ├── ProductVariant.cs         # Product variants (size, color, etc.)
│   │   │   ├── Inventory.cs              # Store-specific inventory
│   │   │   ├── InventoryAdjustment.cs    # Stock adjustments
│   │   │   ├── StockTransfer.cs          # Inter-store transfers
│   │   │   ├── StockTransferItem.cs      # Transfer line items
│   │   │   ├── Supplier.cs               # Supplier information
│   │   │   ├── PurchaseOrder.cs          # Purchase orders
│   │   │   ├── PurchaseOrderItem.cs      # PO line items
│   │   │   └── ProductSupplier.cs        # Product-Supplier mapping
│   │   │
│   │   ├── Orders
│   │   │   ├── Order.cs                  # Order header
│   │   │   ├── OrderItem.cs              # Order line items
│   │   │   ├── OrderStatusHistory.cs     # Status change tracking
│   │   │   ├── OrderDiscount.cs          # Order-level discounts
│   │   │   ├── Cart.cs                   # Shopping cart
│   │   │   ├── CartItem.cs               # Cart line items
│   │   │   ├── OrderShipping.cs          # Shipping information
│   │   │   └── OrderNote.cs              # Order notes/comments
│   │   │
│   │   ├── Payments
│   │   │   ├── Payment.cs                # Payment records
│   │   │   ├── PaymentMethod.cs          # Payment method types
│   │   │   ├── PaymentTransaction.cs     # Payment gateway transactions
│   │   │   ├── Refund.cs                 # Refund records
│   │   │   └── PaymentGatewayLog.cs      # Gateway communication logs
│   │   │
│   │   ├── HRM                           # Human Resource Management
│   │   │   ├── Employee.cs               # Employee information
│   │   │   ├── Department.cs             # Departments
│   │   │   ├── Position.cs               # Job positions
│   │   │   ├── Shift.cs                  # Work shifts với cash handling
│   │   │   ├── ShiftActivity.cs          # Shift activities/breaks
│   │   │   ├── Attendance.cs             # Attendance tracking
│   │   │   ├── LeaveRequest.cs           # Leave applications
│   │   │   ├── LeaveType.cs              # Leave types (sick, vacation, etc.)
│   │   │   ├── WorkSchedule.cs           # Employee schedules
│   │   │   ├── PerformanceReview.cs      # Performance evaluations
│   │   │   ├── Training.cs               # Training records
│   │   │   ├── Skill.cs                  # Skills catalog
│   │   │   ├── EmployeeSkill.cs          # Employee-Skill mapping
│   │   │   └── Payroll.cs                # Payroll information
│   │   │
│   │   ├── Tasks
│   │   │   ├── Task.cs                   # Task management
│   │   │   ├── TaskCategory.cs           # Task categories
│   │   │   ├── TaskComment.cs            # Task comments
│   │   │   ├── TaskAttachment.cs         # Task attachments
│   │   │   ├── TaskChecklistItem.cs      # Task checklists
│   │   │   └── TaskTimeLog.cs            # Time tracking
│   │   │
│   │   ├── CRM                           # Customer Relationship Management
│   │   │   ├── Lead.cs                   # Sales leads
│   │   │   ├── Opportunity.cs            # Sales opportunities
│   │   │   ├── Campaign.cs               # Marketing campaigns
│   │   │   ├── CustomerSegment.cs        # Customer segmentation
│   │   │   ├── CustomerLifecycleStage.cs # Customer lifecycle
│   │   │   ├── SalesTarget.cs            # Sales targets
│   │   │   ├── SalesActivity.cs          # Sales activities
│   │   │   └── CustomerFeedback.cs       # Customer feedback
│   │   │
│   │   ├── LRM                           # Loyalty & Reward Management  
│   │   │   ├── LoyaltyProgram.cs         # Loyalty programs
│   │   │   ├── LoyaltyTier.cs            # Loyalty tiers (Bronze, Silver, Gold)
│   │   │   ├── LoyaltyTransaction.cs     # Points earn/redeem transactions
│   │   │   ├── LoyaltyRule.cs            # Program rules
│   │   │   ├── Reward.cs                 # Available rewards
│   │   │   ├── CustomerReward.cs         # Customer-specific rewards
│   │   │   ├── PromotionCode.cs          # Promotional codes
│   │   │   ├── Promotion.cs              # Promotions/discounts
│   │   │   └── PromotionRule.cs          # Promotion conditions
│   │   │
│   │   ├── Analytics
│   │   │   ├── SalesReport.cs            # Sales reporting
│   │   │   ├── InventoryReport.cs        # Inventory reporting
│   │   │   ├── StaffPerformanceReport.cs # Staff performance
│   │   │   ├── CustomerReport.cs         # Customer analytics
│   │   │   ├── ReportSchedule.cs         # Scheduled reports
│   │   │   ├── DashboardWidget.cs        # Dashboard configurations
│   │   │   └── KPIMetric.cs              # Key Performance Indicators
│   │   │
│   │   ├── Notifications
│   │   │   ├── Notification.cs           # System notifications
│   │   │   ├── NotificationTemplate.cs   # Notification templates
│   │   │   ├── NotificationSubscription.cs # User notification preferences
│   │   │   ├── NotificationChannel.cs    # Delivery channels (email, SMS, push)
│   │   │   ├── EmailTemplate.cs          # Email templates
│   │   │   ├── SMSTemplate.cs            # SMS templates
│   │   │   └── PushNotificationToken.cs  # Mobile push tokens
│   │   │
│   │   ├── Integrations
│   │   │   ├── IntegrationConfig.cs      # Third-party integration configs
│   │   │   ├── WebhookEndpoint.cs        # Webhook endpoints
│   │   │   ├── WebhookLog.cs             # Webhook delivery logs
│   │   │   ├── APIKey.cs                 # API key management
│   │   │   ├── ExternalSystemLog.cs      # External system communication
│   │   │   └── SyncJob.cs                # Data synchronization jobs
│   │   │
│   │   ├── System
│   │   │   ├── Setting.cs                # System settings
│   │   │   ├── Feature.cs                # Feature flags
│   │   │   ├── UserFeature.cs            # User-specific feature access
│   │   │   ├── SystemLog.cs              # System activity logs
│   │   │   ├── ErrorLog.cs               # Error tracking
│   │   │   ├── FileUpload.cs             # File management
│   │   │   └── Backup.cs                 # Backup management
│   │   │
│   │   └── Audit
│   │       ├── AuditLog.cs               # Comprehensive audit trail
│   │       ├── DataChangeLog.cs          # Data change tracking
│   │       ├── LoginAttempt.cs           # Login attempt tracking
│   │       ├── UserSession.cs            # User session management
│   │       └── SecurityEvent.cs          # Security-related events
│   │
│   ├── DTOs                       # Data Transfer Objects
│   │   ├── Common
│   │   │   ├── BaseResponse.cs           # Standard API response
│   │   │   ├── PaginatedResponse.cs      # Paginated response wrapper
│   │   │   ├── ApiError.cs               # Error response structure
│   │   │   ├── ValidationError.cs        # Validation error details
│   │   │   └── FileUploadDTO.cs          # File upload DTO
│   │   │
│   │   ├── Auth
│   │   │   ├── LoginRequest.cs           # Login credentials
│   │   │   ├── LoginResponse.cs          # Login response with tokens
│   │   │   ├── RefreshTokenRequest.cs    # Token refresh request
│   │   │   ├── RefreshTokenResponse.cs   # Token refresh response
│   │   │   ├── ChangePasswordRequest.cs  # Password change request
│   │   │   ├── ForgotPasswordRequest.cs  # Password reset request
│   │   │   ├── ResetPasswordRequest.cs   # Password reset confirmation
│   │   │   └── UserPermissionsDTO.cs     # User permissions response
│   │   │
│   │   ├── Users
│   │   │   ├── UserDTO.cs                # User information
│   │   │   ├── UserCreateDTO.cs          # User creation request
│   │   │   ├── UserUpdateDTO.cs          # User update request
│   │   │   ├── UserListDTO.cs            # User list item
│   │   │   ├── UserDetailDTO.cs          # Detailed user information
│   │   │   ├── RoleDTO.cs                # Role information
│   │   │   ├── PermissionDTO.cs          # Permission details
│   │   │   └── UserStatsDTO.cs           # User statistics
│   │   │
│   │   ├── Stores
│   │   │   ├── StoreDTO.cs               # Store information
│   │   │   ├── StoreCreateDTO.cs         # Store creation request
│   │   │   ├── StoreUpdateDTO.cs         # Store update request
│   │   │   ├── StoreListDTO.cs           # Store list item
│   │   │   ├── StoreDetailDTO.cs         # Detailed store information
│   │   │   └── StoreStatsDTO.cs          # Store statistics
│   │   │
│   │   ├── Customers
│   │   │   ├── CustomerDTO.cs            # Customer information
│   │   │   ├── CustomerCreateDTO.cs      # Customer creation request
│   │   │   ├── CustomerUpdateDTO.cs      # Customer update request
│   │   │   ├── CustomerListDTO.cs        # Customer list item
│   │   │   ├── CustomerDetailDTO.cs      # Detailed customer info
│   │   │   ├── CustomerInteractionDTO.cs # CRM interaction
│   │   │   ├── CustomerStatsDTO.cs       # Customer statistics
│   │   │   ├── CustomerSearchDTO.cs      # Customer search filters
│   │   │   └── CustomerImportDTO.cs      # Bulk customer import
│   │   │
│   │   ├── Products
│   │   │   ├── ProductDTO.cs             # Product information
│   │   │   ├── ProductCreateDTO.cs       # Product creation request
│   │   │   ├── ProductUpdateDTO.cs       # Product update request
│   │   │   ├── ProductListDTO.cs         # Product list item
│   │   │   ├── ProductDetailDTO.cs       # Detailed product info
│   │   │   ├── CategoryDTO.cs            # Category information
│   │   │   ├── CategoryCreateDTO.cs      # Category creation
│   │   │   ├── InventoryDTO.cs           # Inventory information
│   │   │   ├── InventoryAdjustmentDTO.cs # Stock adjustment
│   │   │   ├── StockTransferDTO.cs       # Stock transfer
│   │   │   ├── SupplierDTO.cs            # Supplier information
│   │   │   ├── PurchaseOrderDTO.cs       # Purchase order
│   │   │   ├── ProductSearchDTO.cs       # Product search filters
│   │   │   └── BarcodeSearchDTO.cs       # Barcode search
│   │   │
│   │   ├── Orders
│   │   │   ├── OrderDTO.cs               # Order information
│   │   │   ├── OrderCreateDTO.cs         # Order creation request
│   │   │   ├── OrderUpdateDTO.cs         # Order update request
│   │   │   ├── OrderListDTO.cs           # Order list item
│   │   │   ├── OrderDetailDTO.cs         # Detailed order info
│   │   │   ├── OrderItemDTO.cs           # Order line item
│   │   │   ├── CartDTO.cs                # Shopping cart
│   │   │   ├── CartItemDTO.cs            # Cart item
│   │   │   ├── OrderCalculationDTO.cs    # Order total calculation
│   │   │   ├── OrderSearchDTO.cs         # Order search filters
│   │   │   └── OrderStatsDTO.cs          # Order statistics
│   │   │
│   │   ├── Payments
│   │   │   ├── PaymentDTO.cs             # Payment information
│   │   │   ├── PaymentCreateDTO.cs       # Payment processing request
│   │   │   ├── PaymentMethodDTO.cs       # Payment method info
│   │   │   ├── PaymentTransactionDTO.cs  # Transaction details
│   │   │   ├── RefundDTO.cs              # Refund information
│   │   │   ├── RefundCreateDTO.cs        # Refund request
│   │   │   ├── PaymentStatsDTO.cs        # Payment statistics
│   │   │   └── PaymentGatewayResponse.cs # Gateway response
│   │   │
│   │   ├── POS
│   │   │   ├── POSSessionDTO.cs          # POS session info
│   │   │   ├── CashRegisterDTO.cs        # Cash register state
│   │   │   ├── ReceiptDTO.cs             # Receipt generation
│   │   │   ├── VoidTransactionDTO.cs     # Void transaction
│   │   │   ├── TillCountDTO.cs           # Till counting
│   │   │   └── POSReportDTO.cs           # POS reports
│   │   │
│   │   ├── HRM
│   │   │   ├── EmployeeDTO.cs            # Employee information
│   │   │   ├── EmployeeCreateDTO.cs      # Employee creation
│   │   │   ├── EmployeeUpdateDTO.cs      # Employee update
│   │   │   ├── EmployeeListDTO.cs        # Employee list item
│   │   │   ├── EmployeeDetailDTO.cs      # Detailed employee info
│   │   │   ├── DepartmentDTO.cs          # Department information
│   │   │   ├── PositionDTO.cs            # Position information
│   │   │   ├── ShiftDTO.cs               # Shift information
│   │   │   ├── ShiftCreateDTO.cs         # Shift creation
│   │   │   ├── AttendanceDTO.cs          # Attendance tracking
│   │   │   ├── LeaveRequestDTO.cs        # Leave request
│   │   │   ├── WorkScheduleDTO.cs        # Work schedule
│   │   │   ├── PerformanceReviewDTO.cs   # Performance review
│   │   │   ├── TrainingDTO.cs            # Training records
│   │   │   ├── PayrollDTO.cs             # Payroll information
│   │   │   └── EmployeeStatsDTO.cs       # Employee statistics
│   │   │
│   │   ├── Tasks
│   │   │   ├── TaskDTO.cs                # Task information
│   │   │   ├── TaskCreateDTO.cs          # Task creation request
│   │   │   ├── TaskUpdateDTO.cs          # Task update request
│   │   │   ├── TaskListDTO.cs            # Task list item
│   │   │   ├── TaskDetailDTO.cs          # Detailed task info
│   │   │   ├── TaskCommentDTO.cs         # Task comment
│   │   │   ├── TaskAttachmentDTO.cs      # Task attachment
│   │   │   ├── TaskStatsDTO.cs           # Task statistics
│   │   │   └── TaskSearchDTO.cs          # Task search filters
│   │   │
│   │   ├── CRM
│   │   │   ├── LeadDTO.cs                # Lead information
│   │   │   ├── OpportunityDTO.cs         # Sales opportunity
│   │   │   ├── CampaignDTO.cs            # Marketing campaign
│   │   │   ├── CustomerSegmentDTO.cs     # Customer segment
│   │   │   ├── SalesActivityDTO.cs       # Sales activity
│   │   │   ├── SalesTargetDTO.cs         # Sales target
│   │   │   ├── CustomerFeedbackDTO.cs    # Customer feedback
│   │   │   └── CRMStatsDTO.cs            # CRM statistics
│   │   │
│   │   ├── LRM
│   │   │   ├── LoyaltyProgramDTO.cs      # Loyalty program
│   │   │   ├── LoyaltyTierDTO.cs         # Loyalty tier
│   │   │   ├── LoyaltyTransactionDTO.cs  # Loyalty transaction
│   │   │   ├── RewardDTO.cs              # Reward information
│   │   │   ├── PromotionDTO.cs           # Promotion/discount
│   │   │   ├── PromotionCodeDTO.cs       # Promotion code
│   │   │   ├── CustomerLoyaltyDTO.cs     # Customer loyalty status
│   │   │   └── LoyaltyStatsDTO.cs        # Loyalty statistics
│   │   │
│   │   ├── Analytics
│   │   │   ├── DashboardDTO.cs           # Dashboard data
│   │   │   ├── SalesReportDTO.cs         # Sales report
│   │   │   ├── InventoryReportDTO.cs     # Inventory report
│   │   │   ├── StaffReportDTO.cs         # Staff performance report
│   │   │   ├── CustomerReportDTO.cs      # Customer analytics
│   │   │   ├── RevenueAnalyticsDTO.cs    # Revenue analytics
│   │   │   ├── ProductAnalyticsDTO.cs    # Product performance
│   │   │   ├── KPIMetricDTO.cs           # KPI metrics
│   │   │   └── TrendAnalysisDTO.cs       # Trend analysis
│   │   │
│   │   ├── Notifications
│   │   │   ├── NotificationDTO.cs        # Notification information
│   │   │   ├── NotificationCreateDTO.cs  # Notification creation
│   │   │   ├── NotificationListDTO.cs    # Notification list
│   │   │   ├── NotificationSettingsDTO.cs # User notification settings
│   │   │   ├── EmailDTO.cs               # Email sending
│   │   │   ├── SMSDTO.cs                 # SMS sending
│   │   │   └── PushNotificationDTO.cs    # Push notification
│   │   │
│   │   └── Exports
│   │       ├── ExportRequestDTO.cs       # Export request
│   │       ├── ExportStatusDTO.cs        # Export status
│   │       ├── ReportExportDTO.cs        # Report export
│   │       └── DataExportDTO.cs          # Data export
│   │
│   ├── Services                   # Business Logic Services
│   │   ├── Interfaces
│   │   │   ├── Common
│   │   │   │   ├── IGenericService.cs    # Generic service interface
│   │   │   │   ├── IFileService.cs       # File handling service
│   │   │   │   ├── IEmailService.cs      # Email service
│   │   │   │   ├── ISMSService.cs        # SMS service
│   │   │   │   ├── IPushNotificationService.cs # Push notifications
│   │   │   │   ├── IExportService.cs     # Data export service
│   │   │   │   ├── IReportService.cs     # Report generation
│   │   │   │   └── ICacheService.cs      # Caching service
│   │   │   │
│   │   │   ├── IAuthService.cs           # Authentication service
│   │   │   ├── IUserService.cs           # User management service
│   │   │   ├── IStoreService.cs          # Store management service
│   │   │   ├── ICustomerService.cs       # Customer service
│   │   │   ├── IProductService.cs        # Product service
│   │   │   ├── IInventoryService.cs      # Inventory service
│   │   │   ├── IOrderService.cs          # Order service
│   │   │   ├── IPaymentService.cs        # Payment processing service
│   │   │   ├── IPOSService.cs            # POS operations service\n│   │   │   ├── IShiftService.cs          # Shift management service\n│   │   │   ├── ITaskService.cs           # Task management service\n│   │   │   ├── IAnalyticsService.cs      # Analytics service\n│   │   │   ├── INotificationService.cs   # Notification service\n│   │   │   ├── ICRMService.cs            # CRM service\n│   │   │   ├── ILRMService.cs            # Loyalty/Reward service\n│   │   │   ├── IHRMService.cs            # HR management service\n│   │   │   ├── IAuditService.cs          # Audit logging service\n│   │   │   ├── IPermissionService.cs     # Permission checking service\n│   │   │   ├── IIntegrationService.cs    # Third-party integrations\n│   │   │   └── IWebhookService.cs        # Webhook handling service\n│   │   │\n│   │   ├── AuthService.cs                # Authentication logic\n│   │   ├── UserService.cs                # User management logic\n│   │   ├── StoreService.cs               # Store management logic\n│   │   ├── CustomerService.cs            # Customer service logic\n│   │   ├── ProductService.cs             # Product service logic\n│   │   ├── InventoryService.cs           # Inventory management logic\n│   │   ├── OrderService.cs               # Order processing logic\n│   │   ├── PaymentService.cs             # Payment processing logic\n│   │   ├── POSService.cs                 # POS operations logic\n│   │   ├── ShiftService.cs               # Shift management logic\n│   │   ├── TaskService.cs                # Task management logic\n│   │   ├── AnalyticsService.cs           # Analytics calculation logic\n│   │   ├── NotificationService.cs        # Notification logic\n│   │   ├── CRMService.cs                 # CRM business logic\n│   │   ├── LRMService.cs                 # Loyalty/Reward logic\n│   │   ├── HRMService.cs                 # HR management logic\n│   │   ├── AuditService.cs               # Audit logging logic\n│   │   ├── PermissionService.cs          # Permission checking logic\n│   │   ├── IntegrationService.cs         # Integration management\n│   │   ├── WebhookService.cs             # Webhook processing\n│   │   ├── FileService.cs                # File management\n│   │   ├── EmailService.cs               # Email sending\n│   │   ├── SMSService.cs                 # SMS sending\n│   │   ├── PushNotificationService.cs    # Push notifications\n│   │   ├── ExportService.cs              # Data export\n│   │   ├── ReportService.cs              # Report generation\n│   │   └── CacheService.cs               # Caching logic\n│   │\n│   ├── Validators                # Data Validation\n│   │   ├── Auth\n│   │   │   ├── LoginRequestValidator.cs\n│   │   │   ├── RegisterRequestValidator.cs\n│   │   │   └── ChangePasswordValidator.cs\n│   │   ├── Users\n│   │   │   ├── UserCreateValidator.cs\n│   │   │   ├── UserUpdateValidator.cs\n│   │   │   └── UserPermissionValidator.cs\n│   │   ├── Products\n│   │   │   ├── ProductCreateValidator.cs\n│   │   │   ├── ProductUpdateValidator.cs\n│   │   │   ├── InventoryAdjustmentValidator.cs\n│   │   │   └── StockTransferValidator.cs\n│   │   ├── Orders\n│   │   │   ├── OrderCreateValidator.cs\n│   │   │   ├── OrderUpdateValidator.cs\n│   │   │   └── OrderCalculationValidator.cs\n│   │   ├── Payments\n│   │   │   ├── PaymentCreateValidator.cs\n│   │   │   └── RefundRequestValidator.cs\n│   │   ├── CRM\n│   │   │   ├── CustomerCreateValidator.cs\n│   │   │   ├── CustomerUpdateValidator.cs\n│   │   │   └── InteractionCreateValidator.cs\n│   │   ├── HRM\n│   │   │   ├── EmployeeCreateValidator.cs\n│   │   │   ├── LeaveRequestValidator.cs\n│   │   │   └── ShiftCreateValidator.cs\n│   │   └── Tasks\n│   │       ├── TaskCreateValidator.cs\n│   │       └── TaskUpdateValidator.cs\n│   │\n│   └── Common                    # Shared Components\n│       ├── Exceptions\n│       │   ├── BusinessException.cs       # Business logic exceptions\n│       │   ├── ValidationException.cs     # Validation exceptions\n│       │   ├── NotFoundException.cs       # Not found exceptions\n│       │   ├── UnauthorizedException.cs   # Authorization exceptions\n│       │   ├── ConflictException.cs       # Conflict exceptions\n│       │   └── ExternalServiceException.cs # External service exceptions\n│       │\n│       ├── Constants\n│       │   ├── Roles.cs                   # Role constants\n│       │   ├── Permissions.cs             # Permission constants\n│       │   ├── OrderStatus.cs             # Order status constants\n│       │   ├── PaymentStatus.cs           # Payment status constants\n│       │   ├── NotificationTypes.cs       # Notification types\n│       │   ├── ErrorCodes.cs              # Error code constants\n│       │   ├── CacheKeys.cs               # Cache key constants\n│       │   └── AppSettings.cs             # Application settings\n│       │\n│       ├── Utils\n│       │   ├── DateTimeHelper.cs          # Date/time utilities\n│       │   ├── StringHelper.cs            # String utilities\n│       │   ├── ValidationHelper.cs        # Validation utilities\n│       │   ├── EncryptionHelper.cs        # Encryption utilities\n│       │   ├── FileHelper.cs              # File handling utilities\n│       │   ├── ImageHelper.cs             # Image processing utilities\n│       │   ├── PaginationHelper.cs        # Pagination utilities\n│       │   ├── ReportHelper.cs            # Report generation utilities\n│       │   └── MappingHelper.cs           # Object mapping utilities\n│       │\n│       ├── Extensions\n│       │   ├── StringExtensions.cs        # String extension methods\n│       │   ├── DateTimeExtensions.cs      # DateTime extension methods\n│       │   ├── EnumExtensions.cs          # Enum extension methods\n│       │   ├── QueryableExtensions.cs     # IQueryable extensions\n│       │   └── HttpContextExtensions.cs   # HttpContext extensions\n│       │\n│       └── Attributes\n│           ├── PermissionAttribute.cs     # Permission authorization\n│           ├── AuditAttribute.cs          # Audit logging\n│           ├── CacheAttribute.cs          # Caching\n│           ├── RateLimitAttribute.cs      # Rate limiting\n│           └── ValidateModelAttribute.cs  # Model validation\n│\n├── VietCommerce.Data              # Class Library (Infrastructure)\n│   ├── Context\n│   │   ├── VietCommerceDbContext.cs       # Main database context\n│   │   ├── AuditDbContext.cs              # Audit database context\n│   │   └── ReadOnlyDbContext.cs           # Read-only database context\n│   │\n│   ├── Configurations             # Entity Framework Configurations\n│   │   ├── UserConfiguration.cs\n│   │   ├── StoreConfiguration.cs\n│   │   ├── ProductConfiguration.cs\n│   │   ├── OrderConfiguration.cs\n│   │   ├── CustomerConfiguration.cs\n│   │   ├── InventoryConfiguration.cs\n│   │   ├── PaymentConfiguration.cs\n│   │   ├── ShiftConfiguration.cs\n│   │   ├── TaskConfiguration.cs\n│   │   ├── NotificationConfiguration.cs\n│   │   └── AuditLogConfiguration.cs\n│   │\n│   ├── Repositories\n│   │   ├── Interfaces\n│   │   │   ├── IGenericRepository.cs      # Generic repository interface\n│   │   │   ├── IUnitOfWork.cs             # Unit of work pattern\n│   │   │   ├── IUserRepository.cs\n│   │   │   ├── IStoreRepository.cs\n│   │   │   ├── IProductRepository.cs\n│   │   │   ├── IInventoryRepository.cs\n│   │   │   ├── IOrderRepository.cs\n│   │   │   ├── ICustomerRepository.cs\n│   │   │   ├── IPaymentRepository.cs\n│   │   │   ├── IShiftRepository.cs\n│   │   │   ├── ITaskRepository.cs\n│   │   │   ├── INotificationRepository.cs\n│   │   │   ├── ICRMRepository.cs\n│   │   │   ├── ILRMRepository.cs\n│   │   │   ├── IHRMRepository.cs\n│   │   │   ├── IAnalyticsRepository.cs\n│   │   │   └── IAuditRepository.cs\n│   │   │\n│   │   ├── GenericRepository.cs           # Generic repository implementation\n│   │   ├── UnitOfWork.cs                  # Unit of work implementation\n│   │   ├── UserRepository.cs\n│   │   ├── StoreRepository.cs\n│   │   ├── ProductRepository.cs\n│   │   ├── InventoryRepository.cs\n│   │   ├── OrderRepository.cs\n│   │   ├── CustomerRepository.cs\n│   ��   ├── PaymentRepository.cs\n│   │   ├── ShiftRepository.cs\n│   │   ├── TaskRepository.cs\n│   │   ├── NotificationRepository.cs\n│   │   ├── CRMRepository.cs\n│   │   ├── LRMRepository.cs\n│   │   ├── HRMRepository.cs\n│   │   ├── AnalyticsRepository.cs\n│   │   └── AuditRepository.cs\n│   │\n│   ├── Seeders                    # Database Seeding\n│   │   ├── RoleSeeder.cs                  # Seed roles and permissions\n│   │   ├── UserSeeder.cs                  # Seed initial users\n│   │   ├── StoreSeeder.cs                 # Seed sample stores\n│   │   ├── ProductSeeder.cs               # Seed sample products\n│   │   ├── CategorySeeder.cs              # Seed product categories\n│   │   ├── LoyaltySeeder.cs               # Seed loyalty programs\n│   │   └── SettingsSeeder.cs              # Seed system settings\n│   │\n│   ├── Migrations                 # Entity Framework Migrations\n│   │   └── (Auto-generated migration files)\n│   │\n│   └── Interceptors               # EF Core Interceptors\n│       ├── AuditInterceptor.cs            # Automatic audit logging\n│       ├── SoftDeleteInterceptor.cs       # Soft delete handling\n│       └── TimestampInterceptor.cs        # Automatic timestamps\n│\n├── VietCommerce.Api               # ASP.NET Core Web API\n│   ├── Controllers\n│   │   ├── V1                     # API Version 1\n│   │   │   ├── AuthController.cs          # Authentication endpoints\n│   │   │   ├── UsersController.cs         # User management\n│   │   │   ├── StoresController.cs        # Store management\n│   │   │   ├── ProductsController.cs      # Product management\n│   │   │   ├── InventoryController.cs     # Inventory management\n│   │   │   ├── OrdersController.cs        # Order management\n│   │   │   ├── CustomersController.cs     # Customer management\n│   │   │   ├── PaymentsController.cs      # Payment processing\n│   │   │   ├── POSController.cs           # POS operations\n│   │   │   ├── ShiftsController.cs        # Shift management\n│   │   │   ├── TasksController.cs         # Task management\n│   │   │   ├── AnalyticsController.cs     # Analytics and reporting\n│   │   │   ├── NotificationsController.cs # Notification management\n│   │   │   ├── CRMController.cs           # CRM operations\n│   │   │   ├── LRMController.cs           # Loyalty/Reward management\n│   │   │   ├── HRMController.cs           # HR management\n│   │   │   ├── ExportsController.cs       # Data export\n│   │   │   ├── ReportsController.cs       # Report generation\n│   │   │   ├── IntegrationsController.cs  # Third-party integrations\n│   │   │   ├── WebhooksController.cs      # Webhook endpoints\n│   │   │   ├── HealthController.cs        # Health checks\n│   │   │   └── UploadController.cs        # File upload\n│   │   │\n│   │   └── V2                     # API Version 2 (Future)\n│   │       └── (Future API versions)\n│   │\n│   ├── Hubs                       # SignalR Hubs\n│   │   ├── NotificationHub.cs             # Real-time notifications\n│   │   ├── InventoryHub.cs                # Real-time inventory updates\n│   │   ├── OrderHub.cs                    # Real-time order updates\n│   │   ├── POSHub.cs                      # Real-time POS updates\n│   │   └── AnalyticsHub.cs                # Real-time analytics\n│   │\n│   ├── Filters                    # Action Filters\n│   │   ├── ExceptionFilter.cs             # Global exception handling\n│   │   ├── ValidationFilter.cs            # Model validation\n│   │   ├── AuthorizationFilter.cs         # Authorization checks\n│   │   ├── PermissionFilter.cs            # Permission validation\n│   │   ├── AuditFilter.cs                 # Audit logging\n│   │   ├── RateLimitFilter.cs             # Rate limiting\n│   │   └── CacheFilter.cs                 # Response caching\n│   │\n│   ├── Middlewares\n│   │   ├── JwtMiddleware.cs               # JWT token validation\n│   │   ├── ExceptionMiddleware.cs         # Global exception handling\n│   │   ├── AuditMiddleware.cs             # Request/response auditing\n│   │   ├── RequestLoggingMiddleware.cs    # Request logging\n│   │   ├── SecurityHeadersMiddleware.cs   # Security headers\n│   │   ├── CorsMiddleware.cs              # CORS handling\n│   │   ├── CompressionMiddleware.cs       # Response compression\n│   │   └── TenantMiddleware.cs            # Multi-tenant context\n│   │\n│   ├── BackgroundServices         # Background Tasks\n│   │   ├── NotificationWorker.cs          # Notification processing\n│   │   ├── ReportGenerationWorker.cs      # Scheduled report generation\n│   │   ├── InventoryAlertWorker.cs        # Low stock alerts\n│   │   ├── DataSyncWorker.cs              # External system sync\n│   │   ├── CleanupWorker.cs               # Data cleanup tasks\n│   │   ├── BackupWorker.cs                # Database backup\n│   │   └── AnalyticsWorker.cs             # Analytics computation\n│   │\n│   ├── Extensions                 # Service Extensions\n│   │   ├── ServiceCollectionExtensions.cs # DI container setup\n│   │   ├── ApplicationBuilderExtensions.cs # Middleware pipeline\n│   │   ├── SwaggerExtensions.cs           # Swagger configuration\n│   │   ├── AuthExtensions.cs              # Authentication setup\n│   │   ├── CorsExtensions.cs              # CORS configuration\n│   │   ├── DatabaseExtensions.cs          # Database setup\n│   │   ├── CacheExtensions.cs             # Caching setup\n│   │   ├── LoggingExtensions.cs           # Logging configuration\n│   │   └── HealthCheckExtensions.cs       # Health check setup\n│   │\n│   ├── Configuration              # Configuration Classes\n│   │   ├── JwtSettings.cs                 # JWT configuration\n│   │   ├── DatabaseSettings.cs            # Database configuration\n│   │   ├── CacheSettings.cs               # Cache configuration\n│   │   ├── EmailSettings.cs               # Email configuration\n│   │   ├── SMSSettings.cs                 # SMS configuration\n│   │   ├── PaymentSettings.cs             # Payment gateway configuration\n│   │   ├── StorageSettings.cs             # File storage configuration\n│   │   ├── IntegrationSettings.cs         # Third-party integrations\n│   │   └── FeatureFlags.cs                # Feature flag configuration\n│   │\n│   ├── Program.cs                         # Application entry point\n│   ├── appsettings.json                   # Application settings\n│   ├── appsettings.Development.json       # Development settings\n│   ├── appsettings.Production.json        # Production settings\n│   └── appsettings.Staging.json           # Staging settings\n│\n├── VietCommerce.Shared            # Class Library (Cross-cutting)\n│   ├── Enums\n│   │   ├── UserRoles.cs                   # User role enumeration\n│   │   ├── OrderStatus.cs                 # Order status enumeration\n│   │   ├── PaymentStatus.cs               # Payment status enumeration\n│   │   ├── PaymentMethod.cs               # Payment method enumeration\n│   │   ├── InventoryStatus.cs             # Inventory status enumeration\n│   │   ├── TaskStatus.cs                  # Task status enumeration\n│   │   ├── TaskPriority.cs                # Task priority enumeration\n│   │   ├── NotificationStatus.cs          # Notification status enumeration\n│   │   ├── NotificationType.cs            # Notification type enumeration\n│   │   ├── ShiftStatus.cs                 # Shift status enumeration\n│   │   ├── LeaveStatus.cs                 # Leave request status\n│   │   ├── LeaveType.cs                   # Leave type enumeration\n│   │   ├── CustomerStatus.cs              # Customer status enumeration\n│   │   ├── SupplierStatus.cs              # Supplier status enumeration\n│   │   ├── CampaignStatus.cs              # Campaign status enumeration\n│   │   ├── LoyaltyTier.cs                 # Loyalty tier enumeration\n│   │   ├── ReportType.cs                  # Report type enumeration\n│   │   ├── IntegrationStatus.cs           # Integration status enumeration\n│   │   └── AuditAction.cs                 # Audit action enumeration\n│   │\n│   ├── Helpers\n│   │   ├── JwtHelper.cs                   # JWT token utilities\n│   │   ├── PasswordHelper.cs              # Password hashing utilities\n│   │   ├── PaginationHelper.cs            # Pagination utilities\n│   │   ├── SearchHelper.cs                # Search utilities\n│   │   ├── FilterHelper.cs                # Filtering utilities\n│   │   ├── SortHelper.cs                  # Sorting utilities\n│   │   ├── CurrencyHelper.cs              # Currency utilities\n│   │   ├── TaxHelper.cs                   # Tax calculation utilities\n│   │   ├── DiscountHelper.cs              # Discount calculation utilities\n│   │   ├── BarcodeHelper.cs               # Barcode utilities\n│   │   ├── QRCodeHelper.cs                # QR code utilities\n│   │   ├── ExcelHelper.cs                 # Excel export utilities\n│   │   ├── PDFHelper.cs                   # PDF generation utilities\n│   │   ├── CsvHelper.cs                   # CSV export utilities\n│   │   └── EmailTemplateHelper.cs         # Email template utilities\n│   │\n│   ├── Models\n│   │   ├── ApiResponse.cs                 # Standard API response model\n│   │   ├── PaginatedResult.cs             # Paginated result model\n│   │   ├── SearchCriteria.cs              # Search criteria model\n│   │   ├── FilterCriteria.cs              # Filter criteria model\n│   │   ├── SortCriteria.cs                # Sort criteria model\n│   │   ├── EmailMessage.cs                # Email message model\n│   │   ├── SMSMessage.cs                  # SMS message model\n│   │   ├── PushNotificationMessage.cs     # Push notification model\n│   │   ├── FileUploadResult.cs            # File upload result model\n│   │   ├── ExportRequest.cs               # Export request model\n│   │   ├── ReportRequest.cs               # Report request model\n│   │   ├── WebhookPayload.cs              # Webhook payload model\n│   │   └── IntegrationResponse.cs         # Integration response model\n│   │\n│   └── Configuration\n│       ├── ConnectionStrings.cs           # Database connection strings\n│       ├── CacheConfiguration.cs          # Cache configuration\n│       ├── LoggingConfiguration.cs        # Logging configuration\n│       ├── SecurityConfiguration.cs       # Security configuration\n│       ├── PerformanceConfiguration.cs    # Performance settings\n│       └── FeatureToggleConfiguration.cs  # Feature toggle settings\n│\n├── VietCommerce.Tests             # Test Projects\n│   ├── VietCommerce.Core.Tests    # Core logic tests\n│   │   ├── Services\n│   │   │   ├── UserServiceTests.cs\n│   │   │   ├── ProductServiceTests.cs\n│   │   │   ├── OrderServiceTests.cs\n│   │   │   ├── PaymentServiceTests.cs\n│   │   │   ├── InventoryServiceTests.cs\n│   │   │   ├── CRMServiceTests.cs\n│   │   │   ├── LRMServiceTests.cs\n│   │   │   ├── HRMServiceTests.cs\n│   │   │   └── AnalyticsServiceTests.cs\n│   │   │\n│   │   ├── Validators\n│   │   │   ├── UserValidatorTests.cs\n│   │   │   ├── ProductValidatorTests.cs\n│   │   │   └── OrderValidatorTests.cs\n│   │   │\n│   │   └── Helpers\n│   │       ├── TestHelper.cs\n│   │       └── MockDataHelper.cs\n│   │\n│   ├── VietCommerce.Api.Tests     # API integration tests\n│   │   ├── Controllers\n│   │   │   ├── AuthControllerTests.cs\n│   │   │   ├── UsersControllerTests.cs\n│   │   │   ├── ProductsControllerTests.cs\n│   │   │   ├── OrdersControllerTests.cs\n│   │   │   ├── PaymentsControllerTests.cs\n│   │   │   ├── CRMControllerTests.cs\n│   │   │   ├── LRMControllerTests.cs\n│   │   │   └── HRMControllerTests.cs\n│   │   │\n│   │   ├── Middlewares\n│   │   │   ├── AuthMiddlewareTests.cs\n│   │   │   └── ExceptionMiddlewareTests.cs\n│   │   │\n│   │   └── Integration\n│   │       ├── DatabaseIntegrationTests.cs\n│   │       ├── PaymentIntegrationTests.cs\n│   │       └── EmailIntegrationTests.cs\n│   │\n│   └── VietCommerce.Data.Tests    # Data layer tests\n│       ├── Repositories\n│       │   ├── UserRepositoryTests.cs\n│       │   ├── ProductRepositoryTests.cs\n│       │   └── OrderRepositoryTests.cs\n│       │\n│       └── Context\n│           └── DbContextTests.cs\n│\n└── VietCommerce.Tools             # Development Tools\n    ├── DatabaseSeeder             # Database seeding tool\n    │   ├── Program.cs\n    │   └── SeedData\n    │       ├── users.json\n    │       ├── products.json\n    │       ├── categories.json\n    │       └── stores.json\n    │\n    ├── CodeGenerator              # Code generation tools\n    │   ├── Program.cs\n    │   └── Templates\n    │       ├── ControllerTemplate.cs\n    │       ├── ServiceTemplate.cs\n    │       ├── DTOTemplate.cs\n    │       └── RepositoryTemplate.cs\n    │\n    └── DataMigration              # Data migration tools\n        ├── Program.cs\n        └── Migrations\n            ├── LegacyDataImporter.cs\n            └── DataTransformer.cs\n```\n\n---\n\n## Key Enhancements Added\n\n### 1. **Complete Entity Coverage**\nBased on frontend analysis, added missing entities:\n- **HRM Entities**: Employee, Department, Position, LeaveRequest, WorkSchedule, Attendance, PerformanceReview, Training, Skill\n- **CRM Entities**: Lead, Opportunity, Campaign, CustomerSegment, SalesActivity, CustomerFeedback\n- **LRM Entities**: LoyaltyProgram, LoyaltyTier, LoyaltyTransaction, Reward, Promotion, PromotionCode\n- **Advanced Product Entities**: ProductVariant, Supplier, PurchaseOrder, StockTransfer\n- **Notification Entities**: NotificationTemplate, NotificationSubscription, EmailTemplate, SMSTemplate\n- **Integration Entities**: IntegrationConfig, WebhookEndpoint, APIKey, SyncJob\n- **System Entities**: Setting, Feature, UserFeature, SystemLog, ErrorLog, Backup\n\n### 2. **Comprehensive DTOs**\nAdded DTOs for all modules matching frontend requirements:\n- **Complete CRUD DTOs** for all entities (Create, Update, List, Detail)\n- **Search and Filter DTOs** with pagination support\n- **Statistics and Analytics DTOs** for dashboard data\n- **Export and Report DTOs** for data export functionality\n- **Real-time Communication DTOs** for WebSocket updates\n\n### 3. **Advanced Services**\nExtended service layer to support all frontend features:\n- **File Service** for image and document uploads\n- **Email/SMS Services** for notifications\n- **Push Notification Service** for mobile alerts\n- **Export Service** for CSV/Excel/PDF generation\n- **Report Service** for analytics and reporting\n- **Cache Service** for performance optimization\n- **Integration Service** for third-party APIs\n- **Webhook Service** for external system integration\n\n### 4. **Validation Layer**\nAdded comprehensive validation for all entities:\n- **Business Rule Validation** using FluentValidation\n- **Permission-based Validation** \n- **Cross-entity Validation** for complex business logic\n- **File Upload Validation**\n- **API Input Validation**\n\n### 5. **Infrastructure Enhancements**\n- **SignalR Hubs** for real-time updates (inventory, orders, notifications)\n- **Background Services** for scheduled tasks and data processing\n- **Multiple Database Contexts** (main, audit, read-only)\n- **Entity Framework Interceptors** for automatic auditing and soft delete\n- **Database Seeders** for initial data setup\n\n### 6. **API Layer Enhancements**\n- **Versioned Controllers** (V1, V2 support)\n- **Comprehensive Filtering** with action filters\n- **Security Middlewares** for JWT, CORS, security headers\n- **Health Check Endpoints** for monitoring\n- **File Upload Controllers** for media management\n- **Webhook Controllers** for external system integration\n\n### 7. **Cross-cutting Concerns**\n- **Comprehensive Enums** for all status types and categories\n- **Advanced Helpers** for business calculations, exports, templates\n- **Configuration Models** for all system settings\n- **Shared Models** for common API responses and requests\n\n### 8. **Testing Framework**\n- **Unit Tests** for all services and business logic\n- **Integration Tests** for API endpoints and database\n- **Mock Data Helpers** for testing\n- **Performance Tests** for load testing\n\n### 9. **Development Tools**\n- **Database Seeder** for sample data generation\n- **Code Generator** for boilerplate code\n- **Data Migration Tools** for legacy system imports\n\n---\n\n## Frontend-Backend Feature Mapping\n\n### Frontend Features → Backend Implementation\n\n| Frontend Feature | Backend Support |\n|------------------|----------------|\n| **Authentication & Authorization** | ✅ JWT + Role-based permissions |\n| **Multi-language Support** | ✅ Localization in templates |\n| **Dark/Light Theme** | ✅ User preference storage |\n| **Real-time Updates** | ✅ SignalR hubs |\n| **Pagination** | ✅ PaginatedResponse DTOs |\n| **Search & Filtering** | ✅ SearchCriteria models |\n| **File Upload** | ✅ FileService + UploadController |\n| **Data Export** | ✅ ExportService (CSV/Excel/PDF) |\n| **Audit Logging** | ✅ AuditService + AuditInterceptor |\n| **Notifications** | ✅ Email/SMS/Push services |\n| **Analytics Dashboard** | ✅ AnalyticsService + DTOs |\n| **POS Operations** | ✅ POSService + cash handling |\n| **Inventory Management** | ✅ Real-time stock updates |\n| **CRM Integration** | ✅ Complete CRM module |\n| **LRM Integration** | ✅ Loyalty points system |\n| **HRM Integration** | ✅ HR management module |\n| **Task Management** | ✅ TaskService with assignments |\n| **Shift Management** | ✅ ShiftService with cash tracking |\n| **Payment Processing** | ✅ PaymentService + gateway integration |\n| **Multi-store Support** | ✅ Store-based data isolation |\n\n---\n\n## Implementation Priority\n\n### Phase 1 (Core - Week 1-2)\n1. ✅ Basic entities (User, Store, Product, Order, Customer)\n2. ✅ Authentication & authorization\n3. ✅ Basic CRUD operations\n4. ✅ Database setup with migrations\n5. ✅ JWT middleware\n\n### Phase 2 (Essential - Week 3-4)\n1. ✅ Inventory management\n2. ✅ Payment processing\n3. ✅ POS operations\n4. ✅ Real-time updates (SignalR)\n5. ✅ Notification system\n\n### Phase 3 (Extended - Week 5-6)\n1. ✅ CRM module\n2. ✅ LRM module\n3. ✅ HRM module\n4. ✅ Analytics & reporting\n5. ✅ Data export functionality\n\n### Phase 4 (Advanced - Week 7-8)\n1. ✅ Third-party integrations\n2. ✅ Advanced analytics\n3. ✅ Performance optimizations\n4. ✅ Comprehensive testing\n5. ✅ Production deployment\n\n---\n\nThis comprehensive backend structure provides complete support for all frontend features and ensures scalability, maintainability, and performance for the VietCommerce POS system.