using FluentValidation;
using FluentValidation.AspNetCore;
//using VietCommerce.Api.Middlewares;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.IdentityModel.Tokens;
using StackExchange.Redis;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using VietCommerce.Api.Middleware;
using VietCommerce.Application.Extension;
using VietCommerce.Application.Extensions;
using VietCommerce.Application.Helpers;
using VietCommerce.Application.Mappings;
using VietCommerce.Application.Services.Services;
using VietCommerce.Application.Services.Services.Interfaces;
using VietCommerce.Core.Common.Attributes;
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
var builder = WebApplication.CreateBuilder(new WebApplicationOptions
{
    Args = args,
    WebRootPath = "wwwroot" // ✅ Cấu hình ngay từ đầu
});

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
        Description = "VietCommerce E-commerce Platform API - Campaign & Promotion Management System",
        Contact = new Microsoft.OpenApi.Models.OpenApiContact
        {
            Name = "VietCommerce Development Team",
            Email = "dev@vietcommerce.com"
        },
        License = new Microsoft.OpenApi.Models.OpenApiLicense
        {
            Name = "MIT License"
        }
    });
    
    // Add XML documentation comments from controllers
    var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath))
    {
        c.IncludeXmlComments(xmlPath);
    }
    
    // Add XML documentation from DTOs
    var dtoXmlFile = "VietCommerce.Core.xml";
    var dtoXmlPath = Path.Combine(AppContext.BaseDirectory, dtoXmlFile);
    if (File.Exists(dtoXmlPath))
    {
        c.IncludeXmlComments(dtoXmlPath);
    }
    
    // Configure security
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
    
    // Tag operations by controller
    c.TagActionsBy(api =>
    {
        if (api.GroupName != null)
        {
            return new[] { api.GroupName };
        }
        
        var controllerActionDescriptor = api.ActionDescriptor as Microsoft.AspNetCore.Mvc.Controllers.ControllerActionDescriptor;
        if (controllerActionDescriptor != null)
        {
            return new[] { controllerActionDescriptor.ControllerName };
        }
        
        throw new InvalidOperationException("Unable to determine tag for endpoint.");
    });
    
    // Sort tags alphabetically
    c.OrderActionsBy((apiDescA, apiDescB) => apiDescA.RelativePath.CompareTo(apiDescB.RelativePath));
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
builder.Services.AddSingleton<IAuthorizationPolicyProvider, PermissionPolicyProvider>();
builder.Services.AddAuthorization();
// ============================================
// AUTOMAPPER CONFIGURATION
// ============================================
builder.Services.AddAutoMapper(
    typeof(AuthMappingProfile).Assembly,
    typeof(ProductMappingProfile).Assembly,
    typeof(OrderMappingProfile).Assembly,
    typeof(ProductFavoriteMappingProfile).Assembly,
    typeof(MarketingPostMappingProfile).Assembly,
    typeof(CampaignMappingProfile).Assembly
);

// ============================================
// FLUENTVALIDATION CONFIGURATION
// ============================================
builder.Services.AddValidatorsFromAssemblyContaining<VietCommerce.Application.Validators.Marketing.CreateMarketingPostDtoValidator>();
builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddFluentValidationClientsideAdapters();
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
builder.Services.AddRepositories();

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
builder.Services.AddScoped<ProductServiceSeeder>();


// ============================================
// CORS POLICY
// ============================================
builder.Services.AddCors(options =>
{
    options.AddPolicy("VietCommercePolicy", policy =>
    {
        policy.WithOrigins("http://localhost:3000", "http://localhost:3001")
              .AllowCredentials()
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

// Global exception handling middleware (must be early in pipeline)
app.UseExceptionHandlingMiddleware();

app.UseHttpsRedirection();
app.UseCors("VietCommercePolicy");
app.UseAuthentication();
app.UseAuthorization();
// Permission-based middleware (phải sau UseAuthorization)
app.UsePermissionMiddleware();
// ============================================
// ENDPOINTS MAPPING
// ============================================
app.MapControllers();
app.MapGet("/health", () => new { Status = "Healthy", Timestamp = DateTime.UtcNow });
app.MapHealthChecks("/health/redis");
// images middleware
app.UseStaticFiles();
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
    var productServiceSeeder = services.GetRequiredService<ProductServiceSeeder>();

    try
    {
        Console.WriteLine("🧩 Applying migrations...");
        await dbContext.Database.MigrateAsync();
        Console.WriteLine("✅ Migrations applied successfully!");

        Console.WriteLine("🌱 Starting database seeding...");

        await cartOrderPermissionSeeder.SeedAsync(dbContext);
        await productAnalyticsSeeder.SeedAsync();
        await productServiceSeeder.SeedAsync();

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
