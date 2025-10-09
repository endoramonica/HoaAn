# .NET ASP API Service Structure
## Website Thương Mại Điện Tử Đồ Cúng & Nghi Lễ Truyền Thống

> **Tài liệu này liệt kê cấu trúc service files cần có cho .NET ASP API Backend**

---

## 📁 Project Structure Overview

```
VietCulture.API/
├── Controllers/
├── Services/
│   ├── Interfaces/
│   └── Implementations/
├── Repositories/
│   ├── Interfaces/
│   └── Implementations/
├── Models/
│   ├── Entities/
│   ├── DTOs/
│   └── ViewModels/
├── Infrastructure/
├── Common/
├── Middleware/
└── Configuration/
```

---

## 🔧 1. SERVICE INTERFACES (Services/Interfaces/)

### 1.1 Authentication & User Management
```csharp
// IAuthService.cs
public interface IAuthService
{
    Task<AuthResponseDto> RegisterAsync(RegisterDto request);
    Task<AuthResponseDto> LoginAsync(LoginDto request);
    Task<bool> LogoutAsync(string userId);
    Task<bool> VerifyEmailAsync(string token);
    Task<bool> ForgotPasswordAsync(ForgotPasswordDto request);
    Task<bool> ResetPasswordAsync(ResetPasswordDto request);
    Task<string> RefreshTokenAsync(string refreshToken);
}

// IUserService.cs
public interface IUserService
{
    Task<UserProfileDto> GetProfileAsync(string userId);
    Task<UserProfileDto> UpdateProfileAsync(string userId, UpdateProfileDto request);
    Task<bool> ChangePasswordAsync(string userId, ChangePasswordDto request);
    Task<bool> DeleteAccountAsync(string userId);
    Task<PaginatedResult<UserDto>> GetUsersAsync(UserFilterDto filter);
}

// IRoleService.cs
public interface IRoleService
{
    Task<bool> AssignRoleAsync(string userId, string roleName);
    Task<bool> RemoveRoleAsync(string userId, string roleName);
    Task<List<string>> GetUserRolesAsync(string userId);
    Task<List<RoleDto>> GetAllRolesAsync();
}
```

### 1.2 E-commerce Core
```csharp
// ICategoryService.cs
public interface ICategoryService
{
    Task<List<CategoryDto>> GetAllCategoriesAsync();
    Task<CategoryDto> GetCategoryByIdAsync(int id);
    Task<CategoryDto> GetCategoryBySlugAsync(string slug);
    Task<CategoryDto> CreateCategoryAsync(CreateCategoryDto request);
    Task<CategoryDto> UpdateCategoryAsync(int id, UpdateCategoryDto request);
    Task<bool> DeleteCategoryAsync(int id);
}

// IProductService.cs
public interface IProductService
{
    Task<PaginatedResult<ProductDto>> GetProductsAsync(ProductFilterDto filter);
    Task<ProductDetailDto> GetProductByIdAsync(int id);
    Task<ProductDetailDto> GetProductBySlugAsync(string slug);
    Task<List<ProductDto>> GetPromotedProductsAsync(int count = 10);
    Task<List<ProductDto>> GetRelatedProductsAsync(int productId, int count = 5);
    Task<ProductDto> CreateProductAsync(CreateProductDto request);
    Task<ProductDto> UpdateProductAsync(int id, UpdateProductDto request);
    Task<bool> DeleteProductAsync(int id);
    Task<bool> UpdateStockAsync(int productId, int quantity);
}

// ICartService.cs
public interface ICartService
{
    Task<CartDto> GetCartAsync(string userId);
    Task<CartItemDto> AddToCartAsync(string userId, AddToCartDto request);
    Task<CartItemDto> UpdateCartItemAsync(string userId, int cartItemId, UpdateCartItemDto request);
    Task<bool> RemoveFromCartAsync(string userId, int cartItemId);
    Task<bool> ClearCartAsync(string userId);
    Task<CartSummaryDto> GetCartSummaryAsync(string userId);
}

// IOrderService.cs
public interface IOrderService
{
    Task<OrderDto> CreateOrderAsync(string userId, CreateOrderDto request);
    Task<PaginatedResult<OrderDto>> GetUserOrdersAsync(string userId, OrderFilterDto filter);
    Task<OrderDetailDto> GetOrderByIdAsync(int orderId, string userId);
    Task<OrderDto> UpdateOrderStatusAsync(int orderId, UpdateOrderStatusDto request);
    Task<bool> CancelOrderAsync(int orderId, string userId);
    Task<PaginatedResult<OrderDto>> GetAllOrdersAsync(OrderFilterDto filter); // Admin
}

// IPaymentService.cs
public interface IPaymentService
{
    Task<PaymentDto> ProcessPaymentAsync(ProcessPaymentDto request);
    Task<PaymentDto> GetPaymentByIdAsync(int paymentId);
    Task<PaymentDto> UpdatePaymentStatusAsync(int paymentId, PaymentStatus status);
    Task<List<PaymentMethodDto>> GetPaymentMethodsAsync();
}

// IProductReviewService.cs
public interface IProductReviewService
{
    Task<PaginatedResult<ProductReviewDto>> GetProductReviewsAsync(int productId, ReviewFilterDto filter);
    Task<ProductReviewDto> CreateReviewAsync(string userId, CreateProductReviewDto request);
    Task<ProductReviewDto> UpdateReviewAsync(int reviewId, string userId, UpdateProductReviewDto request);
    Task<bool> DeleteReviewAsync(int reviewId, string userId);
    Task<ReviewSummaryDto> GetReviewSummaryAsync(int productId);
}
```

### 1.3 VietDelivery Service
```csharp
// IDeliveryService.cs
public interface IDeliveryService
{
    Task<DeliveryPriceEstimateDto> EstimatePriceAsync(DeliveryPriceEstimateDto request);
    Task<DeliveryOrderDto> CreateDeliveryOrderAsync(string userId, CreateDeliveryOrderDto request);
    Task<PaginatedResult<DeliveryOrderDto>> GetUserDeliveryOrdersAsync(string userId, DeliveryFilterDto filter);
    Task<DeliveryTrackingDto> TrackDeliveryAsync(string trackingNumber);
    Task<DeliveryOrderDto> UpdateDeliveryStatusAsync(int deliveryOrderId, UpdateDeliveryStatusDto request);
    Task<bool> RateDriverAsync(string userId, int deliveryOrderId, RateDriverDto request);
}

// IDriverService.cs
public interface IDriverService
{
    Task<PaginatedResult<DriverDto>> GetAvailableDriversAsync(DriverFilterDto filter);
    Task<DriverDto> GetDriverByIdAsync(int driverId);
    Task<DriverDto> UpdateDriverLocationAsync(int driverId, UpdateLocationDto request);
    Task<DriverAssignmentDto> AssignDriverAsync(int deliveryOrderId, int driverId);
    Task<PaginatedResult<DriverReviewDto>> GetDriverReviewsAsync(int driverId, ReviewFilterDto filter);
}

// IDeliveryTrackingService.cs
public interface IDeliveryTrackingService
{
    Task<DeliveryTrackingDto> GetTrackingInfoAsync(string trackingNumber);
    Task<bool> UpdateLocationAsync(int deliveryOrderId, UpdateLocationDto request);
    Task<PaginatedResult<DeliveryStatusHistoryDto>> GetStatusHistoryAsync(int deliveryOrderId);
    Task<bool> NotifyStatusChangeAsync(int deliveryOrderId, DeliveryStatus status);
}
```

### 1.4 Spiritual Services
```csharp
// ISpiritualServiceService.cs
public interface ISpiritualServiceService
{
    Task<PaginatedResult<SpiritualServiceDto>> GetServicesAsync(SpiritualServiceFilterDto filter);
    Task<SpiritualServiceDetailDto> GetServiceByIdAsync(int serviceId);
    Task<SpiritualServiceDetailDto> GetServiceBySlugAsync(string slug);
    Task<SpiritualServiceDto> CreateServiceAsync(CreateSpiritualServiceDto request);
    Task<SpiritualServiceDto> UpdateServiceAsync(int id, UpdateSpiritualServiceDto request);
}

// ISpiritualBookingService.cs
public interface ISpiritualBookingService
{
    Task<SpiritualBookingDto> CreateBookingAsync(string userId, CreateSpiritualBookingDto request);
    Task<PaginatedResult<SpiritualBookingDto>> GetUserBookingsAsync(string userId, BookingFilterDto filter);
    Task<SpiritualBookingDetailDto> GetBookingByIdAsync(int bookingId, string userId);
    Task<SpiritualBookingDto> UpdateBookingStatusAsync(int bookingId, UpdateBookingStatusDto request);
    Task<bool> CancelBookingAsync(int bookingId, string userId);
}

// ISpiritualSessionService.cs
public interface ISpiritualSessionService
{
    Task<SpiritualSessionDto> StartSessionAsync(int bookingId, StartSessionDto request);
    Task<SpiritualSessionDto> EndSessionAsync(int sessionId, EndSessionDto request);
    Task<PaginatedResult<SpiritualSessionDto>> GetBookingSessionsAsync(int bookingId);
    Task<SpiritualSessionDto> GetSessionByIdAsync(int sessionId);
}

// ISpiritualAIChatService.cs
public interface ISpiritualAIChatService
{
    Task<AIChatResponseDto> ProcessChatAsync(string userId, AIChatRequestDto request);
    Task<PaginatedResult<AIChatHistoryDto>> GetChatHistoryAsync(string userId, ChatFilterDto filter);
    Task<bool> ClearChatHistoryAsync(string userId);
    Task<List<string>> GetChatSuggestionsAsync(string query);
}

// ISpiritualExpertService.cs
public interface ISpiritualExpertService
{
    Task<PaginatedResult<SpiritualExpertDto>> GetExpertsAsync(ExpertFilterDto filter);
    Task<SpiritualExpertDetailDto> GetExpertByIdAsync(int expertId);
    Task<PaginatedResult<ExpertReviewDto>> GetExpertReviewsAsync(int expertId, ReviewFilterDto filter);
    Task<List<AvailableTimeSlotDto>> GetExpertAvailabilityAsync(int expertId, DateTime date);
}
```

### 1.5 Community & Prayer System
```csharp
// IPrayerService.cs
public interface IPrayerService
{
    Task<PrayerDto> SubmitPrayerAsync(string userId, CreatePrayerDto request);
    Task<PaginatedResult<PrayerDto>> GetPrayerFeedAsync(PrayerFilterDto filter);
    Task<PaginatedResult<PrayerDto>> GetUserPrayersAsync(string userId, PrayerFilterDto filter);
    Task<bool> LikePrayerAsync(string userId, int prayerId);
    Task<bool> UnlikePrayerAsync(string userId, int prayerId);
    Task<bool> DeletePrayerAsync(int prayerId, string userId);
}

// ICommunityService.cs
public interface ICommunityService
{
    Task<CommunityPostDto> CreatePostAsync(string userId, CreateCommunityPostDto request);
    Task<PaginatedResult<CommunityPostDto>> GetPostsAsync(CommunityPostFilterDto filter);
    Task<CommunityPostDetailDto> GetPostByIdAsync(int postId);
    Task<PaginatedResult<CommunityPostDto>> GetUserPostsAsync(string userId, CommunityPostFilterDto filter);
    Task<bool> LikePostAsync(string userId, int postId);
    Task<bool> DeletePostAsync(int postId, string userId);
}

// ICommentService.cs
public interface ICommentService
{
    Task<CommentDto> AddCommentAsync(string userId, int postId, CreateCommentDto request);
    Task<PaginatedResult<CommentDto>> GetPostCommentsAsync(int postId, CommentFilterDto filter);
    Task<CommentDto> UpdateCommentAsync(int commentId, string userId, UpdateCommentDto request);
    Task<bool> DeleteCommentAsync(int commentId, string userId);
}

// IQAService.cs
public interface IQAService
{
    Task<QAQuestionDto> SubmitQuestionAsync(string userId, CreateQAQuestionDto request);
    Task<PaginatedResult<QAQuestionDto>> GetQuestionsAsync(QAFilterDto filter);
    Task<QAQuestionDetailDto> GetQuestionByIdAsync(int questionId);
    Task<PaginatedResult<QAQuestionDto>> GetUserQuestionsAsync(string userId, QAFilterDto filter);
    Task<QAAnswerDto> SubmitAnswerAsync(string userId, int questionId, CreateQAAnswerDto request);
    Task<PaginatedResult<QAAnswerDto>> GetQuestionAnswersAsync(int questionId, AnswerFilterDto filter);
    Task<bool> LikeQuestionAsync(string userId, int questionId);
    Task<bool> LikeAnswerAsync(string userId, int answerId);
}
```

### 1.6 Calendar & Events
```csharp
// ICalendarService.cs
public interface ICalendarService
{
    Task<PaginatedResult<CalendarEventDto>> GetEventsAsync(CalendarEventFilterDto filter);
    Task<List<CalendarEventDto>> GetTodayEventsAsync();
    Task<PaginatedResult<CalendarEventDto>> GetUpcomingEventsAsync(UpcomingEventFilterDto filter);
    Task<CalendarEventDetailDto> GetEventByIdAsync(int eventId);
    Task<List<CalendarEventDto>> GetEventsByDateAsync(DateTime date);
    Task<MonthCalendarDto> GetMonthCalendarAsync(int year, int month);
}

// ILunarCalendarService.cs
public interface ILunarCalendarService
{
    Task<LunarDateDto> GetLunarDateAsync(DateTime solarDate);
    Task<DateTime> GetSolarDateAsync(LunarDateDto lunarDate);
    Task<List<LunarDateDto>> GetLunarMonthAsync(int year, int month);
    Task<LunarYearInfoDto> GetLunarYearInfoAsync(int year);
}

// IEventReminderService.cs
public interface IEventReminderService
{
    Task<EventReminderDto> SetReminderAsync(string userId, int eventId, CreateReminderDto request);
    Task<PaginatedResult<EventReminderDto>> GetUserRemindersAsync(string userId, ReminderFilterDto filter);
    Task<bool> UpdateReminderAsync(int reminderId, string userId, UpdateReminderDto request);
    Task<bool> DeleteReminderAsync(int reminderId, string userId);
    Task<List<EventReminderDto>> GetDueRemindersAsync();
}
```

---

## 🏗️ 2. SERVICE IMPLEMENTATIONS (Services/Implementations/)

### 2.1 Core Services
```csharp
// AuthService.cs
public class AuthService : IAuthService
{
    private readonly IUserRepository _userRepository;
    private readonly ITokenService _tokenService;
    private readonly IEmailService _emailService;
    private readonly IPasswordHasher<User> _passwordHasher;
    // Implementation methods...
}

// UserService.cs
public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;
    private readonly IMapper _mapper;
    private readonly IFileUploadService _fileUploadService;
    // Implementation methods...
}

// ProductService.cs
public class ProductService : IProductService
{
    private readonly IProductRepository _productRepository;
    private readonly ICategoryRepository _categoryRepository;
    private readonly IMapper _mapper;
    private readonly ICacheService _cacheService;
    // Implementation methods...
}
```

### 2.2 Specialized Services
```csharp
// DeliveryService.cs
public class DeliveryService : IDeliveryService
{
    private readonly IDeliveryOrderRepository _deliveryOrderRepository;
    private readonly IDriverRepository _driverRepository;
    private readonly IMapService _mapService;
    private readonly INotificationService _notificationService;
    // Implementation methods...
}

// SpiritualAIChatService.cs
public class SpiritualAIChatService : ISpiritualAIChatService
{
    private readonly IAIService _aiService;
    private readonly IChatHistoryRepository _chatHistoryRepository;
    private readonly IKnowledgeBaseService _knowledgeBaseService;
    // Implementation methods...
}

// PrayerService.cs
public class PrayerService : IPrayerService
{
    private readonly IPrayerRepository _prayerRepository;
    private readonly INotificationService _notificationService;
    private readonly IRealTimeService _realTimeService;
    // Implementation methods...
}
```

---

## 🔧 3. UTILITY & INFRASTRUCTURE SERVICES

### 3.1 Common Services (Common/)
```csharp
// ITokenService.cs
public interface ITokenService
{
    string GenerateJwtToken(User user);
    string GenerateRefreshToken();
    ClaimsPrincipal GetPrincipalFromExpiredToken(string token);
    bool ValidateToken(string token);
}

// IEmailService.cs
public interface IEmailService
{
    Task<bool> SendEmailAsync(string to, string subject, string body);
    Task<bool> SendVerificationEmailAsync(string to, string token);
    Task<bool> SendPasswordResetEmailAsync(string to, string token);
    Task<bool> SendOrderConfirmationAsync(string to, OrderDto order);
}

// ISmsService.cs
public interface ISmsService
{
    Task<bool> SendSmsAsync(string phoneNumber, string message);
    Task<bool> SendOrderUpdateAsync(string phoneNumber, string orderNumber, string status);
    Task<bool> SendDeliveryUpdateAsync(string phoneNumber, string trackingNumber, string status);
}

// IFileUploadService.cs
public interface IFileUploadService
{
    Task<string> UploadImageAsync(IFormFile file, string folder);
    Task<List<string>> UploadMultipleImagesAsync(List<IFormFile> files, string folder);
    Task<bool> DeleteFileAsync(string fileUrl);
    Task<string> GetFileUrlAsync(string fileName, string folder);
}

// ICacheService.cs
public interface ICacheService
{
    Task<T> GetAsync<T>(string key) where T : class;
    Task SetAsync<T>(string key, T value, TimeSpan? expiry = null) where T : class;
    Task RemoveAsync(string key);
    Task RemoveByPatternAsync(string pattern);
    Task<bool> ExistsAsync(string key);
}

// INotificationService.cs
public interface INotificationService
{
    Task SendPushNotificationAsync(string userId, NotificationDto notification);
    Task SendEmailNotificationAsync(string userId, EmailNotificationDto notification);
    Task SendSmsNotificationAsync(string userId, SmsNotificationDto notification);
    Task<PaginatedResult<NotificationDto>> GetUserNotificationsAsync(string userId, NotificationFilterDto filter);
    Task MarkAsReadAsync(string userId, int notificationId);
}

// IRealTimeService.cs
public interface IRealTimeService
{
    Task SendToUserAsync(string userId, string method, object data);
    Task SendToGroupAsync(string groupName, string method, object data);
    Task AddToGroupAsync(string connectionId, string groupName);
    Task RemoveFromGroupAsync(string connectionId, string groupName);
}
```

### 3.2 External Integration Services
```csharp
// IMapService.cs
public interface IMapService
{
    Task<DistanceCalculationDto> CalculateDistanceAsync(string fromAddress, string toAddress);
    Task<GeocodeResultDto> GeocodeAddressAsync(string address);
    Task<List<RouteDto>> GetOptimalRouteAsync(string fromAddress, string toAddress);
    Task<bool> ValidateAddressAsync(string address);
}

// IPaymentGatewayService.cs
public interface IPaymentGatewayService
{
    Task<PaymentResponseDto> ProcessVnPayPaymentAsync(VnPayRequestDto request);
    Task<PaymentResponseDto> ProcessMoMoPaymentAsync(MoMoRequestDto request);
    Task<PaymentResponseDto> ProcessZaloPayPaymentAsync(ZaloPayRequestDto request);
    Task<PaymentVerificationDto> VerifyPaymentAsync(string transactionId, PaymentMethod method);
}

// IAIService.cs
public interface IAIService
{
    Task<string> GenerateResponseAsync(string prompt, List<ChatMessage> history = null);
    Task<List<string>> GenerateSuggestionsAsync(string input);
    Task<string> TranslateTextAsync(string text, string targetLanguage);
    Task<SentimentAnalysisDto> AnalyzeSentimentAsync(string text);
}

// IVideoCallService.cs
public interface IVideoCallService
{
    Task<VideoRoomDto> CreateRoomAsync(CreateVideoRoomDto request);
    Task<VideoTokenDto> GenerateTokenAsync(string roomId, string userId);
    Task<bool> EndRoomAsync(string roomId);
    Task<VideoCallRecordingDto> StartRecordingAsync(string roomId);
    Task<bool> StopRecordingAsync(string roomId);
}
```

---

## 📊 4. BACKGROUND SERVICES (Services/Background/)

```csharp
// OrderProcessingService.cs
public class OrderProcessingService : BackgroundService
{
    // Process pending orders, send notifications, update inventory
}

// DeliveryTrackingService.cs
public class DeliveryTrackingService : BackgroundService
{
    // Real-time tracking updates, driver location updates
}

// NotificationService.cs
public class NotificationService : BackgroundService
{
    // Send scheduled notifications, reminders
}

// PrayerFeedUpdateService.cs
public class PrayerFeedUpdateService : BackgroundService
{
    // Update prayer feed in real-time, manage prayer visibility
}

// CalendarReminderService.cs
public class CalendarReminderService : BackgroundService
{
    // Send calendar event reminders
}

// DataCleanupService.cs
public class DataCleanupService : BackgroundService
{
    // Clean up expired tokens, old logs, temporary files
}
```

---

## 🎯 5. ADMIN & MANAGEMENT SERVICES

```csharp
// IAdminUserService.cs
public interface IAdminUserService
{
    Task<PaginatedResult<AdminUserDto>> GetUsersAsync(AdminUserFilterDto filter);
    Task<AdminUserDetailDto> GetUserByIdAsync(string userId);
    Task<bool> BanUserAsync(string userId, BanUserDto request);
    Task<bool> UnbanUserAsync(string userId);
    Task<UserActivityDto> GetUserActivityAsync(string userId);
}

// IAdminOrderService.cs
public interface IAdminOrderService
{
    Task<PaginatedResult<AdminOrderDto>> GetOrdersAsync(AdminOrderFilterDto filter);
    Task<AdminOrderDetailDto> GetOrderByIdAsync(int orderId);
    Task<bool> UpdateOrderStatusAsync(int orderId, AdminUpdateOrderStatusDto request);
    Task<OrderStatisticsDto> GetOrderStatisticsAsync(StatisticsFilterDto filter);
}

// IAdminAnalyticsService.cs
public interface IAdminAnalyticsService
{
    Task<DashboardDataDto> GetDashboardDataAsync();
    Task<SalesReportDto> GenerateSalesReportAsync(ReportFilterDto filter);
    Task<UserEngagementReportDto> GetUserEngagementReportAsync(ReportFilterDto filter);
    Task<PopularProductsReportDto> GetPopularProductsReportAsync(ReportFilterDto filter);
}

// ISystemLogService.cs
public interface ISystemLogService
{
    Task LogAsync(LogLevel level, string message, object data = null);
    Task<PaginatedResult<SystemLogDto>> GetLogsAsync(LogFilterDto filter);
    Task<bool> ClearOldLogsAsync(int daysToKeep);
    Task<SystemHealthDto> GetSystemHealthAsync();
}
```

---

## 📝 6. IMPLEMENTATION NOTES

### 6.1 Dependency Injection Setup (Program.cs)
```csharp
// Core Services
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<IOrderService, OrderService>();

// Delivery Services
builder.Services.AddScoped<IDeliveryService, DeliveryService>();
builder.Services.AddScoped<IDriverService, DriverService>();

// Spiritual Services
builder.Services.AddScoped<ISpiritualServiceService, SpiritualServiceService>();
builder.Services.AddScoped<ISpiritualAIChatService, SpiritualAIChatService>();

// Community Services
builder.Services.AddScoped<IPrayerService, PrayerService>();
builder.Services.AddScoped<ICommunityService, CommunityService>();

// Utility Services
builder.Services.AddSingleton<ICacheService, RedisCacheService>();
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<IFileUploadService, CloudinaryUploadService>();

// Background Services
builder.Services.AddHostedService<OrderProcessingService>();
builder.Services.AddHostedService<DeliveryTrackingService>();
```

### 6.2 Service Layer Best Practices
1. **Async/Await**: Tất cả service methods sử dụng async/await
2. **Error Handling**: Implement proper exception handling và logging
3. **Validation**: Input validation trước khi process business logic
4. **Caching**: Cache frequently accessed data
5. **Performance**: Optimize database queries và minimize N+1 problems
6. **Security**: Validate user permissions và sanitize inputs
7. **Pagination**: Implement efficient pagination cho large datasets
8. **Testing**: Unit tests cho tất cả service methods

### 6.3 Cultural Considerations for Services
- **Vietnamese Text Processing**: Special handling cho diacritics trong search
- **Lunar Calendar Integration**: Accurate lunar date calculations
- **Time Zone Handling**: Default ICT (UTC+7) timezone
- **Currency Formatting**: VND formatting throughout
- **Address Validation**: Vietnamese address format validation

---

> **Service files này sẽ handle toàn bộ business logic cho ứng dụng, được organize theo domain và follow .NET best practices với dependency injection, async operations, và proper error handling.**