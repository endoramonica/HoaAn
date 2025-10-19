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




    
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    Task BeginTransactionAsync();
    Task CommitTransactionAsync();
    Task RollbackTransactionAsync();
}
