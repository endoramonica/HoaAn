// File: VietCommerce.Api/Extensions/ServiceCollectionExtensions.cs
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using VietCommerce.Application.Services.Admin_Staff_Manager;
using VietCommerce.Application.Services.EndUser;
using VietCommerce.Application.Services.EndUser.EndUser_Interfaces;
using VietCommerce.Application.Services.Payments;
using VietCommerce.Application.Services.Services;
using VietCommerce.Application.Services.Services.Identity;
using VietCommerce.Application.Services.Services.Interfaces;
using VietCommerce.Application.Services.Services.Interfaces.Identities;
using VietCommerce.Core.Helpers;
using VietCommerce.Data.Repositories;
using VietCommerce.Data.Repositories.Interfaces;

namespace VietCommerce.Application.Extension
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
            services.AddScoped<IWishlistService, WishlistService>();
            services.AddScoped<IAddressService, AddressService>();
            services.AddScoped<IVnpayService, VnpayService>();




            // Repositories
            services.AddScoped<IOrderItemRepository, OrderItemRepository>();

            // RBAC services
            services.AddScoped<IPermissionService, PermissionService>();
            services.AddScoped<ICurrentUser, CurrentUserService>();

            // services.AddScoped<IRoleService, RoleService>();

            return services;
        }
    }
}
