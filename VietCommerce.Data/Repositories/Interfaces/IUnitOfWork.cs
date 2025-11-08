using Microsoft.EntityFrameworkCore.Storage;
using VietCommerce.Core.Entities.Orders;

namespace VietCommerce.Data.Repositories.Interfaces;

public interface IUnitOfWork : IDisposable
{
    //  USER & AUTH REPOSITORIES
    IUserRepository Users { get; }
    IRefreshTokenRepository RefreshTokens { get; }
    IRoleRepository Roles { get; }
    IPermissionRepository Permissions { get; }
    IUserRoleRepository UserRoles { get; }
    IRolePermissionRepository RolePermissions { get; }
    
    // PRODUCT REPOSITORIES
    IProductRepository Products { get; }
    ICategoryRepository Categories { get; }

    // CART REPOSITORIES 
    ICartRepository Carts { get; }
    IGenericRepository<CartItem> CartItems { get; }
    // ORDER REPOSITORIES
    IOrderRepository Orders { get; }
    IOrderItemRepository OrderItems { get; }
    IOrderStatusHistoryRepository OrderStatusHistories { get; }
    IOrderShippingRepository OrderShipping { get; }
    // CUSTOMER REPOSITORIES
    ICustomerRepository Customers { get; }


    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    Task<IDbContextTransaction> BeginTransactionAsync();
    Task CommitTransactionAsync();
    Task RollbackTransactionAsync();
}
