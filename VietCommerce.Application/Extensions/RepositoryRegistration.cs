using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VietCommerce.Data.Repositories;
using VietCommerce.Data.Repositories.Interfaces;

namespace VietCommerce.Application.Extensions
{
    /// <summary>
    /// Extension methods for registering all repositories in DI container
    /// </summary>
    public static class RepositoryRegistration
    {
        /// <summary>
        /// Register all repositories and UnitOfWork
        /// </summary>
        public static IServiceCollection AddRepositories(this IServiceCollection services)
        {
            // Core
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));

            // User & RBAC
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IRoleRepository, RoleRepository>();
            services.AddScoped<IPermissionRepository, PermissionRepository>();
            services.AddScoped<IUserRoleRepository, UserRoleRepository>();
            services.AddScoped<IRolePermissionRepository, RolePermissionRepository>();

            // Customer & Address
            services.AddScoped<ICustomerRepository, CustomerRepository>();
            services.AddScoped<ICustomerAddressRepository, CustomerAddressRepository>();

            // Product & Category
            services.AddScoped<IProductRepository, ProductRepository>();
            services.AddScoped<ICategoryRepository, CategoryRepository>();
            services.AddScoped<IProductFavoriteRepository, ProductFavoriteRepository>();

            // Order & Cart
            services.AddScoped<IOrderRepository, OrderRepository>();
            services.AddScoped<ICartRepository, CartRepository>();

            // Marketing
            services.AddScoped<IMarketingPostRepository, MarketingPostRepository>();

            return services;
        }
    }
}
