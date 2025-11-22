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
        /// Register application services and repositories
        /// </summary>
        public static IServiceCollection AddAllServices(this IServiceCollection services)
        {
            // Http accessor
            services.AddHttpContextAccessor();

            // Generic
            services.AddScoped(typeof(IGenericServices<>), typeof(GenericServices<>));
            services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));

            // Helpers
            services.AddScoped<JwtHelper>();

            // Application Services
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
            services.AddScoped<IPaymentService, PaymentService>();
            services.AddScoped<ISupplierService, SupplierService>();
            services.AddScoped<IStockTransferService, StockTransferService>();
            services.AddScoped<IEmployeeService, EmployeeService>();
            services.AddScoped<ILeaveRequestService, LeaveRequestService>();
            services.AddScoped<IWorkScheduleService, WorkScheduleService>();
            
            // RBAC / Auth
            services.AddScoped<IPermissionService, PermissionService>();
            services.AddScoped<ICurrentUser, CurrentUserService>();

            // -------------------------------
            // REPOSITORIES (FULL LIST ADDED)
            // -------------------------------

            services.AddScoped<IUnitOfWork, UnitOfWork>();

            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IProductRepository, ProductRepository>();
            services.AddScoped<ICategoryRepository, CategoryRepository>();
            services.AddScoped<IOrderRepository, OrderRepository>();
            services.AddScoped<ICartRepository, CartRepository>();
            services.AddScoped<IInventoryRepository, InventoryRepository>();

            services.AddScoped<IRoleRepository, RoleRepository>();
            services.AddScoped<IUserRoleRepository, UserRoleRepository>();
            services.AddScoped<IPermissionRepository, PermissionRepository>();
            services.AddScoped<IRolePermissionRepository, RolePermissionRepository>();

            services.AddScoped<ICustomerAddressRepository, CustomerAddressRepository>();
            services.AddScoped<IPaymentMethodRepository, PaymentMethodRepository>();
            services.AddScoped<ISupplierRepository, SupplierRepository>();
            services.AddScoped<IStockTransferRepository, StockTransferRepository>();
            services.AddScoped<ITransferItemRepository, TransferItemRepository>();
            services.AddScoped<ICRMInteractionRepository, CRMInteractionRepository>();

            services.AddScoped<IPaymentRepository, PaymentRepository>();
            services.AddScoped<IProductImageRepository, ProductImageRepository>();
            services.AddScoped<IInventoryMovementRepository, InventoryMovementRepository>();
            services.AddScoped<IProductFavoriteRepository, ProductFavoriteRepository>();

            services.AddScoped<IOrderStatusHistoryRepository, OrderStatusHistoryRepository>();
            services.AddScoped<IOrderShippingRepository, OrderShippingRepository>();
            services.AddScoped<ICustomerRepository, CustomerRepository>();

            services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();

            services.AddScoped<IEmployeeRepository, EmployeeRepository>();
            services.AddScoped<ILeaveRequestRepository, LeaveRequestRepository>();
            services.AddScoped<IWorkScheduleRepository, WorkScheduleRepository>();
            services.AddScoped<IShiftRepository, ShiftRepository>();

            services.AddScoped<IOrderItemRepository, OrderItemRepository>();

            return services;
        }
    }
}
