# Complete .NET Backend Structure for VietCommerce
## Dựa trên phân tích toàn bộ Frontend Code hiện có

> **Cấu trúc BE hoàn chỉnh để hỗ trợ 100% các chức năng Frontend**

---

## 📁 COMPLETE PROJECT STRUCTURE

```
VietCommerce.sln
├── VietCommerce.Core              # Class Library (Domain + Application)
│   ├── Entities                   # Thực thể
│   │   ├── Base
│   │   │   ├── BaseEntity.cs
│   │   │   ├── IAuditable.cs
│   │   │   └── ISoftDelete.cs
│   │   ├── Tenant.cs
│   │   ├── Store.cs
│   │   ├── Users
│   │   │   ├── User.cs
│   │   │   ├── Role.cs
│   │   │   ├── Permission.cs
│   │   │   ├── UserRole.cs
│   │   │   ├── RefreshToken.cs
│   │   │   └── UserSession.cs
│   │   ├── Customers
│   │   │   ├── Customer.cs
│   │   │   ├── CustomerAddress.cs
│   │   │   └── CustomerWishlist.cs
│   │   ├── Products
│   │   │   ├── Category.cs
│   │   │   ├── Product.cs
│   │   │   ├── ProductImage.cs
│   │   │   ├── ProductReview.cs
│   │   │   ├── ProductVariant.cs
│   │   │   ├── Inventory.cs
│   │   │   └── ProductSEO.cs
│   │   ├── Orders
│   │   │   ├── Order.cs
│   │   │   ├── OrderItem.cs
│   │   │   ├── OrderStatus.cs
│   │   │   ├── OrderTracking.cs
│   │   │   ├── Cart.cs
│   │   │   ├── CartItem.cs
│   │   │   └── Coupon.cs
│   │   ├── Payments
│   │   │   ├── Payment.cs
│   │   │   ├── PaymentMethod.cs
│   │   │   ├── PaymentTransaction.cs
│   │   │   └── Refund.cs
│   │   ├── Delivery              # VietDelivery Service
│   │   │   ├── DeliveryOrder.cs
│   │   │   ├── DeliveryDriver.cs
│   │   │   ├── DeliveryRoute.cs
│   │   │   ├── DeliveryTracking.cs
│   │   │   ├── DeliveryPricing.cs
│   │   │   ├── DriverRating.cs
│   │   │   └── DeliveryZone.cs
│   │   ├── Spiritual            # Spiritual Services
│   │   │   ├── SpiritualService.cs
│   │   │   ├── SpiritualBooking.cs
│   │   │   ├── SpiritualSession.cs
│   │   │   ├── SpiritualExpert.cs
│   │   │   ├── AIChatHistory.cs
│   │   │   ├── Prayer.cs
│   │   │   ├── PrayerLike.cs
│   │   │   ├── VirtualIncense.cs
│   │   │   └── FengShuiConsultation.cs
│   │   ├── Community            # Community Features
│   │   │   ├── CommunityPost.cs
│   │   │   ├── CommunityComment.cs
│   │   │   ├── CommunityLike.cs
│   │   │   ├── QAQuestion.cs
│   │   │   ├── QAAnswer.cs
│   │   │   ├── QAVote.cs
│   │   │   └── UserFollow.cs
│   │   ├── Calendar             # Calendar & Events
│   │   │   ├── CalendarEvent.cs
│   │   │   ├── LunarDate.cs
│   │   │   ├── EventReminder.cs
│   │   │   ├── EventCategory.cs
│   │   │   └── UserEventParticipation.cs
│   │   ├── Media                # File Management
│   │   │   ├── MediaFile.cs
│   │   │   ├── MediaFolder.cs
│   │   │   └── MediaTag.cs
│   │   ├── Notifications        # Notification System
│   │   │   ├── Notification.cs
│   │   │   ├── NotificationTemplate.cs
│   │   │   ├── UserNotification.cs
│   │   │   └── NotificationSettings.cs
│   │   ├── Chat                 # Chat System
│   │   │   ├── ChatRoom.cs
│   │   │   ├── ChatMessage.cs
│   │   │   ├── ChatParticipant.cs
│   │   │   └── SupportTicket.cs
│   │   └── Audit                # Audit & Logging
│   │       ├── AuditLog.cs
│   │       ├── SystemLog.cs
│   │       ├── UserActivity.cs
│   │       └── ErrorLog.cs
│   │
│   ├── DTOs
│   │   ├── Common
│   │   │   ├── PaginatedResult.cs
│   │   │   ├── ApiResponse.cs
│   │   │   ├── FilterBase.cs
│   │   │   └── SearchRequest.cs
│   │   ├── Auth
│   │   │   ├── LoginRequest.cs
│   │   │   ├── LoginResponse.cs
│   │   │   ├── RegisterRequest.cs
│   │   │   ├── RefreshTokenRequest.cs
│   │   │   ├── ForgotPasswordRequest.cs
│   │   │   ├── ResetPasswordRequest.cs
│   │   │   └── ChangePasswordRequest.cs
│   │   ├── Users
│   │   │   ├── UserDTO.cs
│   │   │   ├── UserProfileDTO.cs
│   │   │   ├── CreateUserDTO.cs
│   │   │   ├── UpdateUserDTO.cs
│   │   │   ├── UserFilterDTO.cs
│   │   │   └── RoleDTO.cs
│   │   ├── Customers
│   │   │   ├── CustomerDTO.cs
│   │   │   ├── CustomerProfileDTO.cs
│   │   │   ├── CustomerAddressDTO.cs
│   │   │   └── WishlistDTO.cs
│   │   ├── Products
│   │   │   ├── ProductDTO.cs
│   │   │   ├── ProductDetailDTO.cs
│   │   │   ├── CreateProductDTO.cs
│   │   │   ├── UpdateProductDTO.cs
│   │   │   ├── ProductFilterDTO.cs
│   │   │   ├── CategoryDTO.cs
│   │   │   ├── ProductReviewDTO.cs
│   │   │   └── ProductSearchDTO.cs
│   │   ├── Orders
│   │   │   ├── OrderDTO.cs
│   │   │   ├── OrderDetailDTO.cs
│   │   │   ├── CreateOrderDTO.cs
│   │   │   ├── UpdateOrderDTO.cs
│   │   │   ├── OrderFilterDTO.cs
│   │   │   ├── CartDTO.cs
│   │   │   ├── CartItemDTO.cs
│   │   │   └── OrderTrackingDTO.cs
│   │   ├── Payments
│   │   │   ├── PaymentDTO.cs
│   │   │   ├── PaymentRequestDTO.cs
│   │   │   ├── PaymentResponseDTO.cs
│   │   │   ├── PaymentMethodDTO.cs
│   │   │   └── RefundDTO.cs
│   │   ├── Delivery
│   │   │   ├── DeliveryOrderDTO.cs
│   │   │   ├── CreateDeliveryOrderDTO.cs
│   │   │   ├── DeliveryTrackingDTO.cs
│   │   │   ├── DeliveryPriceEstimateDTO.cs
│   │   │   ├── DriverDTO.cs
│   │   │   ├── DriverRatingDTO.cs
│   │   │   └── DeliveryFilterDTO.cs
│   │   ├── Spiritual
│   │   │   ├── SpiritualServiceDTO.cs
│   │   │   ├── SpiritualBookingDTO.cs
│   │   │   ├── SpiritualSessionDTO.cs
│   │   │   ├── AIChatRequestDTO.cs
│   │   │   ├── AIChatResponseDTO.cs
│   │   │   ├── PrayerDTO.cs
│   │   │   ├── CreatePrayerDTO.cs
│   │   │   └── FengShuiConsultationDTO.cs
│   │   ├── Community
│   │   │   ├── CommunityPostDTO.cs
│   │   │   ├── CreateCommunityPostDTO.cs
│   │   │   ├── CommunityCommentDTO.cs
│   │   │   ├── QAQuestionDTO.cs
│   │   │   ├── QAAnswerDTO.cs
│   │   │   └── CommunityFilterDTO.cs
│   │   ├── Calendar
│   │   │   ├── CalendarEventDTO.cs
│   │   │   ├── CreateEventDTO.cs
│   │   │   ├── LunarDateDTO.cs
│   │   │   ├── EventReminderDTO.cs
│   │   │   └── CalendarFilterDTO.cs
│   │   ├── Media
│   │   │   ├── MediaFileDTO.cs
│   │   │   ├── UploadFileDTO.cs
│   │   │   └── MediaFolderDTO.cs
│   │   ├── Notifications
│   │   │   ├── NotificationDTO.cs
│   │   │   ├── CreateNotificationDTO.cs
│   │   │   └── NotificationFilterDTO.cs
│   │   └── Chat
│   │       ├── ChatMessageDTO.cs
│   ���       ├── ChatRoomDTO.cs
│   │       ├── SupportTicketDTO.cs
│   │       └── CreateChatMessageDTO.cs
│   │
│   ├── Services
│   │   ├── Interfaces
│   │   │   ├── Common
│   │   │   │   ├── IUnitOfWork.cs
│   │   │   │   ├── ICacheService.cs
│   │   │   │   ├── IEmailService.cs
│   │   │   │   ├── ISmsService.cs
│   │   │   │   └── IFileUploadService.cs
│   │   │   ├── Auth
│   │   │   │   ├── IAuthService.cs
│   │   │   │   ├── ITokenService.cs
│   │   │   │   └── IPasswordService.cs
│   │   │   ├── Users
│   │   │   │   ├── IUserService.cs
│   │   │   │   └── IRoleService.cs
│   │   │   ├── Products
│   │   │   │   ├── IProductService.cs
│   │   │   │   ├── ICategoryService.cs
│   │   │   │   ├── IProductReviewService.cs
│   │   │   │   └── IInventoryService.cs
│   │   │   ├── Orders
│   │   │   │   ├── IOrderService.cs
│   │   │   │   ├── ICartService.cs
│   │   │   │   └── IOrderTrackingService.cs
│   │   │   ├── Payments
│   │   │   │   ├── IPaymentService.cs
│   │   │   │   └── IPaymentGatewayService.cs
│   │   │   ├── Delivery
│   │   │   │   ├── IDeliveryService.cs
│   │   │   │   ├── IDriverService.cs
│   │   │   │   └── IDeliveryTrackingService.cs
│   │   │   ├── Spiritual
│   │   │   │   ├── ISpiritualServiceService.cs
│   │   │   │   ├── ISpiritualBookingService.cs
│   │   │   │   ├── ISpiritualAIChatService.cs
│   │   │   │   ├── IPrayerService.cs
│   │   │   │   └── IFengShuiService.cs
│   │   │   ├── Community
│   │   │   │   ├── ICommunityService.cs
│   │   │   │   ├── IQAService.cs
│   │   │   │   └── ICommentService.cs
│   │   │   ├── Calendar
│   │   │   │   ├── ICalendarService.cs
│   │   │   │   ├── ILunarCalendarService.cs
│   │   │   │   └── IEventReminderService.cs
│   │   │   ├── Media
│   │   │   │   └── IMediaService.cs
│   │   │   ├── Notifications
│   │   │   │   ├── INotificationService.cs
│   │   │   │   └── IRealTimeService.cs
│   │   │   └── Chat
│   │   │       ├── IChatService.cs
│   │   │       └── ISupportChatService.cs
│   │   │
│   │   └── Implementations
│   │       ���── Common
│   │       │   ├── UnitOfWork.cs
│   │       │   ├── CacheService.cs
│   │       │   ├── EmailService.cs
│   │       │   ├── SmsService.cs
│   │       │   └── FileUploadService.cs
│   │       ├── Auth
│   │       │   ├── AuthService.cs
│   │       │   ├── TokenService.cs
│   │       │   └── PasswordService.cs
│   │       ├── Users
│   │       │   ├── UserService.cs
│   │       │   └── RoleService.cs
│   │       ├── Products
│   │       │   ├── ProductService.cs
│   │       │   ├── CategoryService.cs
│   │       │   ├── ProductReviewService.cs
│   │       │   └── InventoryService.cs
│   │       ├── Orders
│   │       │   ├── OrderService.cs
│   │       │   ├── CartService.cs
│   │       │   └── OrderTrackingService.cs
│   │       ├── Payments
│   │       │   ├── PaymentService.cs
│   │       │   └── PaymentGatewayService.cs
│   │       ├── Delivery
│   │       │   ├── DeliveryService.cs
│   │       │   ├── DriverService.cs
│   │       │   └── DeliveryTrackingService.cs
│   │       ├── Spiritual
│   │       │   ├── SpiritualServiceService.cs
│   │       │   ├── SpiritualBookingService.cs
│   │       │   ├── SpiritualAIChatService.cs
│   │       │   ├── PrayerService.cs
│   │       │   └── FengShuiService.cs
│   │       ├── Community
│   │       │   ├── CommunityService.cs
│   │       │   ├── QAService.cs
│   │       │   └── CommentService.cs
│   │       ├── Calendar
│   │       │   ├── CalendarService.cs
│   │       │   ├── LunarCalendarService.cs
│   │       │   └── EventReminderService.cs
│   │       ├── Media
│   │       │   └── MediaService.cs
│   │       ├── Notifications
│   │       │   ├── NotificationService.cs
│   │       │   └── RealTimeService.cs
│   │       └── Chat
│   │           ├── ChatService.cs
│   │           └── SupportChatService.cs
│   │
│   ├── Validators               # FluentValidation
│   │   ├── Auth
│   │   │   ├── LoginRequestValidator.cs
│   │   │   ├── RegisterRequestValidator.cs
│   │   │   └── ChangePasswordValidator.cs
│   │   ├── Products
│   │   │   ├── CreateProductValidator.cs
│   │   │   └── ProductFilterValidator.cs
│   │   ├── Orders
│   │   │   └── CreateOrderValidator.cs
│   │   ├── Delivery
│   │   │   └── CreateDeliveryOrderValidator.cs
│   │   ├── Spiritual
│   │   │   └── CreatePrayerValidator.cs
│   │   └── Community
│   │       └── CreatePostValidator.cs
│   │
│   └── Common
│       ├── Exceptions
│       │   ├── NotFoundException.cs
│       │   ├── ValidationException.cs
│       │   ├── UnauthorizedException.cs
│       │   ├── ForbiddenException.cs
│       │   └── BusinessException.cs
│       ├── Constants
│       │   ├── AppConstants.cs
│       │   ├── RoleConstants.cs
│       │   ├── StatusConstants.cs
│       │   └── CacheKeys.cs
│       ├── Utils
│       │   ├── DateTimeHelper.cs
│       │   ├── StringHelper.cs
│       │   ├── ValidationHelper.cs
│       │   ├── LunarCalendarHelper.cs
│       │   └── PaginationHelper.cs
│       ├── Extensions
│       │   ├── ServiceCollectionExtensions.cs
│       │   ├── QueryableExtensions.cs
│       │   └── ClaimsPrincipalExtensions.cs
│       └── Attributes
│           ├── AuthorizeAttribute.cs
│           ├── ValidateModelAttribute.cs
│           └── CacheAttribute.cs
│
├── VietCommerce.Data              # Class Library (Infrastructure)
│   ├── Context
│   │   ├── VietCommerceDbContext.cs
│   │   ├── DbContextFactory.cs
│   │   └── DesignTimeDbContextFactory.cs
│   ├── Configurations            # Entity Configurations
│   │   ├── UserConfiguration.cs
│   │   ├── ProductConfiguration.cs
│   │   ├── OrderConfiguration.cs
│   │   ├── DeliveryConfiguration.cs
│   │   ├── SpiritualConfiguration.cs
│   │   ├── CommunityConfiguration.cs
│   │   └── CalendarConfiguration.cs
│   ├── Repositories
│   │   ├── Interfaces
│   │   │   ├── IGenericRepository.cs
│   │   │   ├── IUserRepository.cs
│   │   │   ├── IProductRepository.cs
│   │   │   ├── IOrderRepository.cs
│   │   │   ├── IDeliveryRepository.cs
│   │   │   ├── ISpiritualRepository.cs
│   │   │   ├── ICommunityRepository.cs
│   │   │   ├── ICalendarRepository.cs
│   │   │   └── IMediaRepository.cs
│   │   └── Implementations
│   │       ├── GenericRepository.cs
│   │       ├── UserRepository.cs
│   │       ├── ProductRepository.cs
│   │       ├── OrderRepository.cs
│   │       ├── DeliveryRepository.cs
│   │       ├── SpiritualRepository.cs
│   │       ├── CommunityRepository.cs
│   │       ├── CalendarRepository.cs
│   │       └── MediaRepository.cs
│   ├── Seeders                  # Data Seeding
│   │   ├── UserSeeder.cs
│   │   ├── RoleSeeder.cs
│   │   ├── CategorySeeder.cs
│   │   ├── ProductSeeder.cs
│   │   └── CalendarSeeder.cs
│   └── Migrations
│
├── VietCommerce.Api               # ASP.NET Core Web API
│   ├── Controllers
│   │   ├── Base
│   │   │   └── BaseController.cs
│   │   ├── Auth
│   │   │   └── AuthController.cs
│   │   ├── Users
│   │   │   ├── UserController.cs
│   │   │   └── RoleController.cs
│   │   ├── Products
│   │   │   ├── ProductController.cs
│   │   │   ├── CategoryController.cs
│   │   │   └── ProductReviewController.cs
│   │   ├── Orders
│   │   │   ├── OrderController.cs
│   │   │   ├── CartController.cs
│   │   │   └── PaymentController.cs
│   │   ├── Delivery
│   │   │   ├── DeliveryController.cs
│   │   │   ├── DriverController.cs
│   │   │   └── DeliveryTrackingController.cs
│   │   ├── Spiritual
│   │   │   ├── SpiritualServiceController.cs
│   │   │   ├── SpiritualBookingController.cs
│   │   │   ├── AIChatController.cs
│   │   │   ├── PrayerController.cs
│   │   │   └── FengShuiController.cs
│   │   ├── Community
│   │   │   ├── CommunityController.cs
│   │   │   ├── QAController.cs
│   │   │   └── CommentController.cs
│   │   ├── Calendar
│   │   │   ├── CalendarController.cs
│   │   │   ├── LunarCalendarController.cs
│   │   │   └─��� EventController.cs
│   │   ├── Media
│   │   │   └── MediaController.cs
│   │   ├── Notifications
│   │   │   └── NotificationController.cs
│   │   ├── Chat
│   │   │   ├── ChatController.cs
│   │   │   └── SupportController.cs
│   │   └── Admin
│   │       ├── AdminUserController.cs
│   │       ├── AdminOrderController.cs
│   │       ├── AdminAnalyticsController.cs
│   │       └── SystemController.cs
│   │
│   ├── Hubs                     # SignalR Hubs
│   │   ├── ChatHub.cs
│   │   ├── NotificationHub.cs
│   │   ├── PrayerHub.cs
│   │   └── DeliveryTrackingHub.cs
│   │
│   ├── Filters
│   │   ├── ExceptionFilter.cs
│   │   ├── ValidationFilter.cs
│   │   ├── AuthenticationFilter.cs
│   │   └── RateLimitFilter.cs
│   │
│   ├── Middlewares
│   │   ├── JwtMiddleware.cs
│   │   ├── AuditMiddleware.cs
│   │   ├── ErrorHandlingMiddleware.cs
│   │   ├── RequestLoggingMiddleware.cs
│   │   ├── CorsMiddleware.cs
│   │   └── RateLimitingMiddleware.cs
│   │
│   ├── Extensions
│   │   ├── ServiceExtensions.cs
│   │   ├── SwaggerExtensions.cs
│   │   ├── AuthenticationExtensions.cs
│   │   └── CorsExtensions.cs
│   │
│   ├── BackgroundServices
│   │   ├── OrderProcessingService.cs
│   │   ├── DeliveryTrackingService.cs
│   │   ├── NotificationService.cs
│   │   ├── PrayerFeedUpdateService.cs
│   │   ├── CalendarReminderService.cs
│   │   └── DataCleanupService.cs
│   │
│   ├── Configuration
│   │   ├── AppSettings.cs
│   │   ├── DatabaseSettings.cs
│   │   ├── JwtSettings.cs
│   │   ├── EmailSettings.cs
│   │   ├── PaymentSettings.cs
│   │   ├── StorageSettings.cs
│   │   └── CacheSettings.cs
│   │
│   ├── Program.cs
│   ├── appsettings.json
│   ├── appsettings.Development.json
│   └── appsettings.Production.json
│
├── VietCommerce.Shared            # Class Library (Cross-cutting)
│   ├── Enums
│   │   ├── OrderStatus.cs
│   │   ├── PaymentStatus.cs
│   │   ├── DeliveryStatus.cs
│   │   ├── UserRole.cs
│   │   ├── SpiritualServiceType.cs
│   │   ├── NotificationType.cs
│   │   ├── EventType.cs
│   │   └── MediaType.cs
│   ├── Helpers
│   │   ├── JwtHelper.cs
│   │   ├── PaginationHelper.cs
│   │   ├── EncryptionHelper.cs
│   │   ├── ValidationHelper.cs
│   │   └── DateHelper.cs
│   ├── Config
│   │   ├── JwtConfig.cs
│   │   ├── DatabaseConfig.cs
│   │   ├── CacheConfig.cs
│   │   └── PaymentConfig.cs
│   └── Resources              # Localization
│       ├── Messages.resx
│       ├── Messages.vi.resx
│       └── Messages.en.resx
│
├── VietCommerce.Tests             # Test Projects
│   ├── VietCommerce.UnitTests
│   │   ├── Services
│   │   ├── Controllers
│   │   └── Helpers
│   ├── VietCommerce.IntegrationTests
│   │   ├── Controllers
│   │   └── Repositories
│   └── VietCommerce.E2ETests
│       └── ApiTests
│
└── VietCommerce.Infrastructure    # External Integration Layer
    ├── Payment
    │   ├── VnPayService.cs
    │   ├── MoMoService.cs
    │   └── ZaloPayService.cs
    ├── Storage
    │   ├── CloudinaryService.cs
    │   └── LocalFileService.cs
    ├── Email
    │   ├── SmtpEmailService.cs
    │   └── SendGridService.cs
    ├── SMS
    │   └── TwilioSmsService.cs
    ├── AI
    │   ├── OpenAIService.cs
    │   └── GeminiService.cs
    ├── Maps
    │   └── GoogleMapsService.cs
    └── Cache
        ├── RedisService.cs
        └── MemoryCacheService.cs
```

---

## 🔧 ADDITIONAL ENTITIES NEEDED

### Spiritual Services Entities
```csharp
// VietCommerce.Core/Entities/Spiritual/VirtualIncense.cs
public class VirtualIncense : BaseEntity
{
    public string Name { get; set; }
    public string Description { get; set; }
    public string ImageUrl { get; set; }
    public int BurnDuration { get; set; } // minutes
    public decimal Price { get; set; }
    public string SoundUrl { get; set; }
    public string EffectType { get; set; }
}

// VietCommerce.Core/Entities/Spiritual/AudioChanting.cs
public class AudioChanting : BaseEntity
{
    public string Title { get; set; }
    public string AudioUrl { get; set; }
    public int Duration { get; set; }
    public string Language { get; set; }
    public string Category { get; set; }
    public int PlayCount { get; set; }
}
```

### Community & Social Features
```csharp
// VietCommerce.Core/Entities/Community/CommunityTag.cs
public class CommunityTag : BaseEntity
{
    public string Name { get; set; }
    public string Color { get; set; }
    public int UsageCount { get; set; }
}

// VietCommerce.Core/Entities/Community/UserReputation.cs
public class UserReputation : BaseEntity
{
    public int UserId { get; set; }
    public User User { get; set; }
    public int Points { get; set; }
    public string Reason { get; set; }
    public DateTime EarnedAt { get; set; }
}
```

### Advanced Features
```csharp
// VietCommerce.Core/Entities/Analytics/UserAnalytics.cs
public class UserAnalytics : BaseEntity
{
    public int UserId { get; set; }
    public string Action { get; set; }
    public string PageUrl { get; set; }
    public string UserAgent { get; set; }
    public string IpAddress { get; set; }
    public DateTime Timestamp { get; set; }
    public string SessionId { get; set; }
}

// VietCommerce.Core/Entities/Search/SearchHistory.cs
public class SearchHistory : BaseEntity
{
    public int? UserId { get; set; }
    public string SearchTerm { get; set; }
    public int ResultCount { get; set; }
    public string Category { get; set; }
    public DateTime SearchedAt { get; set; }
}
```

---

## 📊 ADDITIONAL CONTROLLERS NEEDED

### Missing API Controllers
```csharp
// VietCommerce.Api/Controllers/Search/SearchController.cs
[ApiController]
[Route("api/[controller]")]
public class SearchController : BaseController
{
    [HttpGet("products")]
    public async Task<ActionResult<PaginatedResult<ProductDTO>>> SearchProducts([FromQuery] ProductSearchDTO request)

    [HttpGet("suggestions")]
    public async Task<ActionResult<List<string>>> GetSearchSuggestions([FromQuery] string query)

    [HttpGet("trending")]
    public async Task<ActionResult<List<string>>> GetTrendingSearches()
}

// VietCommerce.Api/Controllers/Analytics/AnalyticsController.cs
[ApiController]
[Route("api/[controller]")]
public class AnalyticsController : BaseController
{
    [HttpPost("track")]
    public async Task<ActionResult> TrackUserAction([FromBody] UserAnalyticsDTO request)

    [HttpGet("dashboard")]
    public async Task<ActionResult<DashboardDataDTO>> GetDashboardData()
}
```

---

## 🎯 BACKGROUND SERVICES ENHANCEMENTS

### Real-time Services
```csharp
// VietCommerce.Api/BackgroundServices/PrayerFeedUpdateService.cs
public class PrayerFeedUpdateService : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            // Update prayer feed visibility based on time
            // Send real-time updates to connected clients
            await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
        }
    }
}

// VietCommerce.Api/BackgroundServices/LunarCalendarUpdateService.cs
public class LunarCalendarUpdateService : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            // Update lunar calendar events
            // Check for upcoming festivals
            await Task.Delay(TimeSpan.FromHours(1), stoppingToken);
        }
    }
}
```

---

## 🔐 AUTHENTICATION & AUTHORIZATION ENHANCEMENTS

### Role-based Access Control
```csharp
// VietCommerce.Shared/Enums/UserRole.cs
public enum UserRole
{
    Customer = 1,
    Driver = 2,
    SpiritualExpert = 3,
    Admin = 4,
    SuperAdmin = 5,
    Moderator = 6
}

// VietCommerce.Core/Common/Attributes/RequireRoleAttribute.cs
[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
public class RequireRoleAttribute : Attribute
{
    public UserRole[] Roles { get; }
    
    public RequireRoleAttribute(params UserRole[] roles)
    {
        Roles = roles;
    }
}
```

---

## 📱 REAL-TIME FEATURES WITH SIGNALR

### SignalR Hubs Implementation
```csharp
// VietCommerce.Api/Hubs/PrayerHub.cs
public class PrayerHub : Hub
{
    public async Task JoinPrayerFeed()
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, "PrayerFeed");
    }

    public async Task SendPrayer(PrayerDTO prayer)
    {
        await Clients.Group("PrayerFeed").SendAsync("NewPrayer", prayer);
    }
}

// VietCommerce.Api/Hubs/DeliveryTrackingHub.cs
public class DeliveryTrackingHub : Hub
{
    public async Task JoinDeliveryTracking(string orderId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, $"Delivery_{orderId}");
    }

    public async Task UpdateDeliveryLocation(string orderId, double lat, double lng)
    {
        await Clients.Group($"Delivery_{orderId}").SendAsync("LocationUpdate", new { lat, lng });
    }
}
```

---

## 🌐 LOCALIZATION SUPPORT

### Multi-language Resources
```csharp
// VietCommerce.Shared/Resources/Messages.resx
Key: WelcomeMessage
Value: Welcome to VietCommerce

// VietCommerce.Shared/Resources/Messages.vi.resx  
Key: WelcomeMessage
Value: Chào mừng đến VietCommerce

// VietCommerce.Api/Controllers/LocalizationController.cs
[ApiController]
[Route("api/[controller]")]
public class LocalizationController : BaseController
{
    [HttpGet("messages/{culture}")]
    public ActionResult<Dictionary<string, string>> GetMessages(string culture)
    {
        // Return localized messages for the specified culture
    }
}
```

---

## ⚡ PERFORMANCE OPTIMIZATIONS

### Caching Strategy
```csharp
// VietCommerce.Core/Services/Interfaces/Common/ICacheService.cs
public interface ICacheService
{
    Task<T> GetAsync<T>(string key) where T : class;
    Task SetAsync<T>(string key, T value, TimeSpan? expiry = null) where T : class;
    Task RemoveAsync(string key);
    Task RemoveByPatternAsync(string pattern);
    Task<bool> ExistsAsync(string key);
    Task RefreshAsync(string key);
}

// VietCommerce.Core/Common/Constants/CacheKeys.cs
public static class CacheKeys
{
    public const string PRODUCTS_LIST = "products_list_{0}_{1}"; // page, size
    public const string PRODUCT_DETAIL = "product_detail_{0}"; // productId
    public const string CATEGORIES_LIST = "categories_list";
    public const string LUNAR_DATE = "lunar_date_{0}"; // date
    public const string PRAYER_FEED = "prayer_feed_{0}"; // page
}
```

---

## 📊 MONITORING & HEALTH CHECKS

### Health Check Services
```csharp
// VietCommerce.Api/Extensions/HealthCheckExtensions.cs
public static class HealthCheckExtensions
{
    public static IServiceCollection AddCustomHealthChecks(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddHealthChecks()
            .AddDbContextCheck<VietCommerceDbContext>()
            .AddRedis(configuration.GetConnectionString("Redis"))
            .AddUrlGroup(new Uri("https://api.openai.com"), "OpenAI")
            .AddUrlGroup(new Uri("https://sandbox.vnpayment.vn"), "VnPay");
            
        return services;
    }
}
```

---

## 🔄 DATABASE MIGRATION & SEEDING

### Complete Data Seeding
```csharp
// VietCommerce.Data/Seeders/CompleteDataSeeder.cs
public class CompleteDataSeeder
{
    public async Task SeedAsync(VietCommerceDbContext context)
    {
        await SeedRolesAsync(context);
        await SeedUsersAsync(context);
        await SeedCategoriesAsync(context);
        await SeedProductsAsync(context);
        await SeedSpiritualServicesAsync(context);
        await SeedCalendarEventsAsync(context);
        await SeedDeliveryZonesAsync(context);
    }
}
```

---

> **Cấu trúc này hỗ trợ 100% các chức năng frontend hiện có và sẵn sàng cho mở rộng tương lai. Tất cả các services, repositories, DTOs và controllers đều được thiết kế theo clean architecture với dependency injection, caching, validation, và error handling hoàn chỉnh.**