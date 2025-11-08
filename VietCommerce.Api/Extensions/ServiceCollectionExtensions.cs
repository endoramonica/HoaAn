// File: VietCommerce.Api/Extensions/ServiceCollectionExtensions.cs
using Microsoft.AspNetCore.Mvc;
using VietCommerce.Api.Services;
using VietCommerce.Api.Services.Interfaces;
using VietCommerce.Data.Repositories;
using VietCommerce.Data.Repositories.Interfaces;

namespace VietCommerce.Api.Extensions
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Configure global model validation behavior
        /// </summary>
        public static IServiceCollection AddCustomValidation(this IServiceCollection services)
        {
            services.Configure<ApiBehaviorOptions>(options =>
            {
                options.InvalidModelStateResponseFactory = context =>
                {
                    var errors = context.ModelState
                        .Where(x => x.Value?.Errors.Count > 0)
                        .ToDictionary(
                            kvp => kvp.Key,
                            kvp => kvp.Value?.Errors.Select(e => e.ErrorMessage).ToArray() ?? Array.Empty<string>()
                        );

                    var response = new
                    {
                        success = false,
                        message = "Validation failed",
                        errors
                    };

                    return new BadRequestObjectResult(response);
                };
            });

            return services;
        }

        /// <summary>
        /// Register domain services and repositories
        /// </summary>
        public static IServiceCollection AddAllServices(this IServiceCollection services)
        {
            // ✅ Đăng ký HttpContextAccessor đúng cách
            services.AddHttpContextAccessor();

            // Generic service/repo
            services.AddScoped(typeof(IGenericServices<>), typeof(GenericServices<>));

            // Application services
            services.AddScoped<IOrderService, OrderService>();
            services.AddScoped<ICheckoutService, CheckoutService>();
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IProductService, ProductService>();
            services.AddScoped<ICartService, CartService>();
            services.AddScoped<ICategoryService, CategoryService>();
            services.AddScoped<ILoginAttemptService, RedisLoginAttemptService>();


            // Repositories
            services.AddScoped<IOrderItemRepository, OrderItemRepository>();

            // RBAC services
            services.AddScoped<IPermissionService, PermissionService>();
            // services.AddScoped<IRoleService, RoleService>();

            return services;
        }
    }
}
