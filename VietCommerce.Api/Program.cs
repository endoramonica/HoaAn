using FluentValidation;
//using VietCommerce.Api.Middlewares;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.IdentityModel.Tokens;
using StackExchange.Redis;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using VietCommerce.Api;
using VietCommerce.Api.Extensions;
using VietCommerce.Api.Services;
using VietCommerce.Api.Services.Interfaces;
using VietCommerce.Application.Mappings;
using VietCommerce.Core.Helpers;
using VietCommerce.Data.Context;
using VietCommerce.Data.Repositories;
using VietCommerce.Data.Repositories.Interfaces;
using VietCommerce.Data.Seeders;
using VietCommerce.Data.Seeds.Seeders;
// <summary>
//Các tính nang chính:
//Full DI Configuration: T?t c? services, repositories, helpers d?u du?c register
//JWT Authentication: C?u hình hoàn ch?nh v?i error handling
//Swagger/OpenAPI: V?i JWT Bearer authorization
//CORS Policy: Cho phép cross-origin requests
//Database Migration: T? d?ng migration trong development
//Comprehensive Logging: Console + Debug logging
//FluentValidation: Auto validation cho t?t c? requests
//Error Handling: Global exception middleware
//Health Check: /health endpoint
//Redis Distributed Cache: Caching layer v?i health check
// </summary>
var builder = WebApplication.CreateBuilder(args);
// ============================================
// CONTROLLERS & API BEHAVIOR
// ============================================
builder.Services.AddControllers()
    .ConfigureApiBehaviorOptions(options =>
    {
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
    });
// ============================================
// SWAGGER/OPENAPI CONFIGURATION
// ============================================
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "VietCommerce API",
        Version = "v1",
        Description = "VietCommerce E-commerce Platform API"
    });
    c.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme (Example: 'Bearer 12345abcdef')",
        Name = "Authorization",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });
    c.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
    {
        {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Reference = new Microsoft.OpenApi.Models.OpenApiReference
                {
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});
// ============================================
// DATABASE CONFIGURATION
// ============================================
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
// ============================================
// REDIS CONFIGURATION (DISTRIBUTED CACHE)
// ============================================
// 1. Register Redis Connection (Singleton - shared connection pool)
builder.Services.AddSingleton<IConnectionMultiplexer>(sp =>
{
    var configuration = ConfigurationOptions.Parse(
        builder.Configuration.GetConnectionString("Redis") ?? "localhost:6379"
    );
    // Production settings
    configuration.AbortOnConnectFail = false; // Don't crash app if Redis is down
    configuration.ConnectTimeout = 5000; // 5 seconds
    configuration.SyncTimeout = 5000;
    configuration.AsyncTimeout = 5000;
    // Logging
    configuration.LoggerFactory = sp.GetRequiredService<ILoggerFactory>();
    var connection = ConnectionMultiplexer.Connect(configuration);
    // Log connection status
    var logger = sp.GetRequiredService<ILogger<Program>>();
    logger.LogInformation("Redis connection status: {Status}",
        connection.IsConnected ? "Connected" : "Disconnected");
    return connection;
});
// 2. Register Cache Service (Scoped - per request)
builder.Services.AddSingleton<ICacheService, RedisCacheService>();
builder.Services.AddHostedService<PermissionCacheInvalidationService>();
// 3. Health check for Redis (optional but recommended)
builder.Services.AddHealthChecks()
    .AddCheck("redis", () =>
    {
        var multiplexer = builder.Services
            .Where(s => s.ServiceType == typeof(IConnectionMultiplexer))
            .Select(s => (IConnectionMultiplexer)s.ImplementationInstance!)
            .FirstOrDefault();

        if (multiplexer == null)
            return HealthCheckResult.Unhealthy("Redis not configured");

        return multiplexer.IsConnected
            ? HealthCheckResult.Healthy("Redis connected")
            : HealthCheckResult.Unhealthy("Redis disconnected");
    });

// ============================================
// JWT AUTHENTICATION & AUTHORIZATION
// ============================================
var jwtSettings = builder.Configuration.GetSection("JwtSettings");
builder.Services.Configure<JwtSettings>(jwtSettings);
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer("Bearer",options =>
{
    options.SaveToken = true;
    options.RequireHttpsMetadata = false;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSettings["Issuer"],
        ValidAudience = jwtSettings["Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(jwtSettings["Key"] ?? throw new InvalidOperationException("JWT Key is required"))
        ),
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
            Console.WriteLine($"Token validated for user: {context.Principal?.Identity?.Name}");
            return Task.CompletedTask;
        }
    };
});
builder.Services.AddAuthorization();
// ============================================
// AUTOMAPPER CONFIGURATION
// ============================================
builder.Services.AddAutoMapper(
    typeof(AuthMappingProfile).Assembly,
    typeof(ProductMappingProfile).Assembly,
    typeof(OrderMappingProfile).Assembly
    
);
// ============================================
// EXTERNAL SETTINGS CONFIGURATION
// ============================================
builder.Services.Configure<GoogleSettings>(builder.Configuration.GetSection("GoogleSettings"));
builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("JwtSettings"));
// ============================================
// HELPERS REGISTRATION
// ============================================
builder.Services.AddScoped<JwtHelper>();
// ============================================
// MEMORY CACHE
// ============================================
builder.Services.AddMemoryCache();
// ============================================
// REPOSITORIES REGISTRATION
// ============================================
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
// RBAC Repositories
builder.Services.AddScoped<IRoleRepository, RoleRepository>();
builder.Services.AddScoped<IPermissionRepository, PermissionRepository>();
builder.Services.AddScoped<IUserRoleRepository, UserRoleRepository>();
builder.Services.AddScoped<IRolePermissionRepository, RolePermissionRepository>();
//Product+Order
builder.Services.AddScoped<IOrderRepository, OrderRepository>();
builder.Services.AddScoped<ICartRepository, CartRepository>();
builder.Services.AddScoped<ICustomerRepository, CustomerRepository>();







// ============================================
// SERVICES REGISTRATION
// ============================================
// ✅ ADD THIS - Configure JSON serialization to handle enums by value
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        // Allow enums to be serialized/deserialized by numeric value

        // Allow numeric enum values (1, 2, 3) to deserialize to enum
        options.JsonSerializerOptions.Converters.Add(
            new JsonStringEnumConverter(JsonNamingPolicy.CamelCase, allowIntegerValues: true)
        );
        options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
        options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
        options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
        options.JsonSerializerOptions.WriteIndented = false;
    });

builder.Services.AddAllServices();

// ============================================
// SEEDERS REGISTRATION
// ============================================
builder.Services.AddScoped<ProductAnalyticsSeeder>();
builder.Services.AddScoped<CartOrderPermissionSeeder>();


// ============================================
// CORS POLICY
// ============================================
builder.Services.AddCors(options =>
{
    options.AddPolicy("VietCommercePolicy", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});
// ============================================
// LOGGING CONFIGURATION
// ============================================
builder.Services.AddLogging(logging =>
{
    logging.ClearProviders();
    logging.AddConsole();
    logging.AddDebug();
    if (builder.Environment.IsDevelopment())
    {
        logging.SetMinimumLevel(LogLevel.Debug);
    }
    else
    {
        logging.SetMinimumLevel(LogLevel.Information);
    }
});
// ============================================
// HTTP CLIENT
// ============================================
builder.Services.AddHttpClient();
// ============================================
// BUILD APPLICATION
// ============================================
var app = builder.Build();
// ============================================
// MIDDLEWARE PIPELINE
// ============================================
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseDeveloperExceptionPage();
}
app.UseHttpsRedirection();
app.UseCors("VietCommercePolicy");
//app.UseMiddleware<ExceptionMiddleware>();
app.UseAuthentication();
app.UseAuthorization();
// ============================================
// ENDPOINTS MAPPING
// ============================================
app.MapControllers();
app.MapGet("/health", () => new { Status = "Healthy", Timestamp = DateTime.UtcNow });
app.MapHealthChecks("/health/redis");
// ============================================
// DATABASE MIGRATION & SEEDING
// ============================================
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var dbContext = services.GetRequiredService<AppDbContext>();

    // Seeder instances
    var cartOrderPermissionSeeder = services.GetRequiredService<CartOrderPermissionSeeder>();
    var productAnalyticsSeeder = services.GetRequiredService<ProductAnalyticsSeeder>();

    try
    {
        Console.WriteLine("🧩 Applying migrations...");
        await dbContext.Database.MigrateAsync();
        Console.WriteLine("✅ Migrations applied successfully!");

        Console.WriteLine("🌱 Starting database seeding...");

        await cartOrderPermissionSeeder.SeedAsync(dbContext);
        await productAnalyticsSeeder.SeedAsync();

        Console.WriteLine("🎉 Database seeding completed!");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"❌ Error during seeding: {ex.Message}");
        Console.WriteLine(ex.StackTrace);
        throw;
    }
}

// ============================================
// APPLICATION STARTUP
// ============================================
Console.WriteLine("?? VietCommerce API is starting...");
Console.WriteLine($"Environment: {app.Environment.EnvironmentName}");
Console.WriteLine($"Swagger UI: {(app.Environment.IsDevelopment() ? "Available at /" : "Disabled in production")}");
app.Run();
