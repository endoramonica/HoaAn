// ================================================================
// FILE: Program.cs
// PROJECT: VietCommerce.AdminAPI
// DESCRIPTION: Entry point configuration for Admin API
// AUTHOR: Nhu Quynh & GPT-5 assistant
// ================================================================

using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using StackExchange.Redis;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.SignalR;
using VietCommerce.AdminApi.Hubs;
using VietCommerce.AdminApi.Services;
using VietCommerce.Application.Extension;
using VietCommerce.Application.Helpers;
using VietCommerce.Application.Mappings;
using VietCommerce.Application.Services.Admin_Staff_Manager;
using VietCommerce.Application.Services.Services;
using VietCommerce.Application.Services.Services.Interfaces;
using VietCommerce.Core.Helpers;
using VietCommerce.Data.Context;
using VietCommerce.Data.Repositories;
using VietCommerce.Data.Repositories.Interfaces;
using VietCommerce.Data.Seeders;
using VietCommerce.Application.Services.Admin_Staff_Manager.Interfaces;
using IInventoryService = VietCommerce.Application.Services.Admin_Staff_Manager.Interfaces.IInventoryService;

var builder = WebApplication.CreateBuilder(new WebApplicationOptions
{
    Args = args,
    WebRootPath = "wwwroot" // ✅ Cấu hình ngay từ đầu
});
// ================================================================
// 1️⃣ CONTROLLERS & VALIDATION
// ================================================================
builder.Services.AddControllers(options =>
{
    // Nếu cần config filter hoặc conventions thì thêm ở đây
})
.ConfigureApiBehaviorOptions(options =>
{
    // Custom JSON response khi model validation fail
    options.InvalidModelStateResponseFactory = context =>
    {
        var errors = context.ModelState
            .Where(x => x.Value.Errors.Count > 0)
            .ToDictionary(
                kvp => kvp.Key,
                kvp => kvp.Value.Errors.Select(e => e.ErrorMessage).ToArray()
            );

        var response = new
        {
            Success = false,
            Message = "Validation failed",
            Errors = errors
        };

        return new Microsoft.AspNetCore.Mvc.BadRequestObjectResult(response);
    };
})
.AddJsonOptions(options =>
{
    options.JsonSerializerOptions.Converters.Add(
        new JsonStringEnumConverter(JsonNamingPolicy.CamelCase, allowIntegerValues: true)
    );
    options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
    options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
    options.JsonSerializerOptions.WriteIndented = false;
});

// ================================================================
// 2️⃣ SWAGGER / OPENAPI CONFIGURATION
// ================================================================
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "VietCommerce Admin API",
        Version = "v1",
        Description = @"VietCommerce Admin Management API - Comprehensive API for managing products, orders, inventory, marketing posts, and administrative operations.
        
**Authentication:**
All endpoints (except analytics endpoints) require JWT Bearer token authentication with Admin role.

**Marketing Post Management:**
- Create, update, delete, and restore marketing posts
- Publish, schedule, and unpublish posts
- Track analytics (views, clicks, shares)
- Bulk operations for efficient management
- Link posts to products for targeted marketing

**Key Features:**
- Soft delete with restore capability
- Priority scoring for display control (1-100)
- Multi-platform social media variants
- SEO metadata management
- Comprehensive filtering and search
- Real-time analytics tracking",
        Contact = new OpenApiContact
        {
            Name = "VietCommerce Support",
            Email = "support@vietcommerce.com"
        }
    });

    // Include XML comments for better API documentation
    var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath))
    {
        c.IncludeXmlComments(xmlPath);
    }

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = @"JWT Authorization header using the Bearer scheme. 
                      Enter 'Bearer' [space] and then your token in the text input below.
                      Example: 'Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...'
                      
**How to get a token:**
1. Call POST /api/admin/auth/login with admin credentials
2. Copy the token from the response
3. Click 'Authorize' button above
4. Enter 'Bearer {your-token}' in the value field
5. Click 'Authorize' and then 'Close'",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                },
                Scheme = "oauth2",
                Name = "Bearer",
                In = ParameterLocation.Header
            },
            new List<string>()
        }
    });

    // Enable annotations for better documentation
    c.EnableAnnotations();

    // Order actions by relative path
    c.OrderActionsBy(apiDesc => apiDesc.RelativePath);

    // Use full type names to avoid conflicts
    c.CustomSchemaIds(type => type.FullName);
});

// ================================================================
// 3️⃣ DATABASE CONFIGURATION (SQL SERVER)
// ================================================================
builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        b => b.MigrationsAssembly("VietCommerce.Data")
    );

    if (builder.Environment.IsDevelopment())
    {
        options.EnableSensitiveDataLogging();
        options.EnableDetailedErrors();
    }
});

// ================================================================
// 4️⃣ REDIS CONFIGURATION (DISTRIBUTED CACHE)
// ================================================================
builder.Services.AddSingleton<IConnectionMultiplexer>(sp =>
{
    var configuration = ConfigurationOptions.Parse(
        builder.Configuration.GetConnectionString("Redis") ?? "localhost:6379"
    );
    configuration.AbortOnConnectFail = false;
    configuration.ConnectTimeout = 5000;
    configuration.SyncTimeout = 5000;
    configuration.AsyncTimeout = 5000;
    configuration.LoggerFactory = sp.GetRequiredService<ILoggerFactory>();

    var connection = ConnectionMultiplexer.Connect(configuration);
    var logger = sp.GetRequiredService<ILogger<Program>>();
    logger.LogInformation("Redis connection status: {Status}", connection.IsConnected ? "Connected" : "Disconnected");
    return connection;
});

builder.Services.AddSingleton<ICacheService, RedisCacheService>();
builder.Services.AddHostedService<PermissionCacheInvalidationService>();

builder.Services.AddHealthChecks()
    .AddCheck("redis", () =>
    {
        try
        {
            var redisConn = builder.Configuration.GetConnectionString("Redis") ?? "localhost:6379";
            var mux = ConnectionMultiplexer.Connect(redisConn);
            return mux.IsConnected
                ? HealthCheckResult.Healthy("Redis connected")
                : HealthCheckResult.Unhealthy("Redis disconnected");
        }
        catch
        {
            return HealthCheckResult.Unhealthy("Redis connection failed");
        }
    });

builder.Services.AddSignalR();
builder.Services.AddScoped<IRealtimeService, AdminSignalRRealtimeService>();

// ================================================================
// 5️⃣ JWT AUTHENTICATION & AUTHORIZATION
// ================================================================
var jwtSettings = builder.Configuration.GetSection("JwtSettings");
builder.Services.Configure<JwtSettings>(jwtSettings);

var jwtKey = jwtSettings["Key"] ?? throw new InvalidOperationException("JWT Key is required");

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer("Bearer", options =>
{
    options.SaveToken = true;
    options.RequireHttpsMetadata = false;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuers = new[] { "VietCommerce.Api", "VietCommerce.AdminApi" },
        ValidAudiences = new[] { "VietCommerce.Client", "VietCommerce.AdminClient" },
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),
        ClockSkew = TimeSpan.Zero
    };
    options.Events = new JwtBearerEvents
    {
        OnAuthenticationFailed = context =>
        {
            Console.WriteLine($"Authentication failed: {context.Exception.Message}");
            return Task.CompletedTask;
        },
        OnTokenValidated = context =>
        {
            var role = context.Principal?.FindFirst("role")?.Value;
            Console.WriteLine($"Token validated - Role: {role}");
            return Task.CompletedTask;
        }
    };
});

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly", policy => policy.RequireRole("Admin", "SuperAdmin"));
    options.AddPolicy("AdminOrModerator", policy => policy.RequireRole("Admin", "SuperAdmin", "Moderator"));
});

// ================================================================
// 6️⃣ AUTOMAPPER
// ================================================================
builder.Services.AddAutoMapper(
    typeof(AuthMappingProfile).Assembly,
    typeof(ProductMappingProfile).Assembly,
    typeof(OrderMappingProfile).Assembly,
    typeof(InventoryMappingProfile).Assembly,
    typeof(CartMappingProfile).Assembly,
    typeof(TaskMappingProfile).Assembly,
    typeof(SupplierMappingProfile).Assembly,
    typeof(CustomerMappingProfile).Assembly,
    typeof(HRMMappingProfile).Assembly,
    typeof(NotificationMappingProfile).Assembly,
    typeof(StockTransferMappingProfile).Assembly,
    typeof(ProductFavoriteMappingProfile).Assembly,
    typeof(MarketingPostMappingProfile).Assembly,
    typeof(CampaignMappingProfile).Assembly
);

// ================================================================
// 6️⃣.1️⃣ FLUENTVALIDATION
// ================================================================
builder.Services.AddValidatorsFromAssemblyContaining<VietCommerce.Application.Validators.Marketing.CreateMarketingPostDtoValidator>();
builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddFluentValidationClientsideAdapters();

// ================================================================
// 7️⃣ REPOSITORIES & SERVICES
// ================================================================
//builder.Services.AddScoped<JwtHelper>();
//builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
//builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
//builder.Services.AddScoped<IUserRepository, UserRepository>();
//builder.Services.AddScoped<IProductRepository, ProductRepository>();
//builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
//builder.Services.AddScoped<IOrderRepository, OrderRepository>();
//builder.Services.AddScoped<ICartRepository, CartRepository>();
//builder.Services.AddScoped<IInventoryRepository, InventoryRepository>();
//builder.Services.AddScoped<IRoleRepository, RoleRepository>();
//builder.Services.AddScoped<IUserRoleRepository, UserRoleRepository>();
//builder.Services.AddScoped<IPermissionRepository, PermissionRepository>();
//builder.Services.AddScoped<IRolePermissionRepository, RolePermissionRepository>();
//builder.Services.AddScoped<ICustomerAddressRepository, CustomerAddressRepository>();
//builder.Services.AddScoped<IPaymentMethodRepository, PaymentMethodRepository>();
//builder.Services.AddScoped<ISupplierRepository, SupplierRepository>();
//builder.Services.AddScoped<IStockTransferRepository, StockTransferRepository>();
//builder.Services.AddScoped<ITransferItemRepository, TransferItemRepository>();
//builder.Services.AddScoped<ICRMInteractionRepository, CRMInteractionRepository>();
//builder.Services.AddScoped<IPaymentRepository, PaymentRepository>();
//builder.Services.AddScoped<IProductImageRepository, ProductImageRepository>();
//builder.Services.AddScoped<IInventoryMovementRepository, InventoryMovementRepository>();
//builder.Services.AddScoped<IProductFavoriteRepository, ProductFavoriteRepository>();
//builder.Services.AddScoped<IOrderStatusHistoryRepository, OrderStatusHistoryRepository>();
//builder.Services.AddScoped<IOrderShippingRepository, OrderShippingRepository>();
//builder.Services.AddScoped<ICustomerRepository, CustomerRepository>();  
//builder.Services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
//builder.Services.AddScoped<IEmployeeRepository, EmployeeRepository>();
//builder.Services.AddScoped<ILeaveRequestRepository, LeaveRequestRepository>();
//builder.Services.AddScoped<IWorkScheduleRepository, WorkScheduleRepository>();
//builder.Services.AddScoped<IShiftRepository, ShiftRepository>();


//// ✅ Register all application-level services
//builder.Services.AddScoped<IInventoryService, InventoryService>();
//builder.Services.AddScoped<IAuthService, AuthService>();
//builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IInventoryService, InventoryService>();
builder.Services.AddCustomValidation();
builder.Services.AddAllServices(builder.Configuration);

// ================================================================
// 8️⃣ CORS CONFIGURATION
// ================================================================
builder.Services.AddCors(options =>
{
    options.AddPolicy("AdminPolicy", policy =>
    {
        policy.WithOrigins("http://localhost:3000", "http://localhost:3001", "http://localhost:5173")
              .AllowCredentials()
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials();
    });
});

// ================================================================
// 9️⃣ LOGGING CONFIGURATION
// ================================================================
builder.Services.AddLogging(logging =>
{
    logging.ClearProviders();
    logging.AddConsole();
    logging.AddDebug();
    logging.SetMinimumLevel(builder.Environment.IsDevelopment() ? LogLevel.Debug : LogLevel.Information);
});

// ================================================================
// 🔟 BUILD APP PIPELINE
// ================================================================
var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseDeveloperExceptionPage();
}

app.UseHttpsRedirection();
app.UseCors("AdminPolicy");
app.UseAuthentication();
app.UseAuthorization();
app.UseStaticFiles();
app.MapControllers();
app.MapHub<AdminRealtimeHub>("/admin/hubs/realtime");
//app.MapGet("/health", () => new { Status = "Healthy", Timestamp = DateTime.UtcNow });
//app.MapHealthChecks("/health/redis");

// ================================================================
// 1️⃣1️⃣ DATABASE MIGRATION & SEEDING
// ================================================================
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var dbContext = services.GetRequiredService<AppDbContext>();

    try
    {
        Console.WriteLine("🧩 Applying migrations...");
        await dbContext.Database.MigrateAsync();
        Console.WriteLine("✅ Migrations applied successfully!");

        Console.WriteLine("🌱 Seeding RBAC roles & permissions...");
        await RBACSeeder.SeedAsync(dbContext);
        Console.WriteLine("✅ RBAC seeded successfully!");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"❌ Error during seeding: {ex.Message}");
        Console.WriteLine(ex.StackTrace);
        throw;
    }
}

// ================================================================
// ✅ STARTUP COMPLETE
// ================================================================
Console.WriteLine("🔐 VietCommerce Admin API is starting...");
Console.WriteLine($"Environment: {app.Environment.EnvironmentName}");
Console.WriteLine($"Swagger UI: {(app.Environment.IsDevelopment() ? "Available at /" : "Disabled in production")}");
app.Run();
