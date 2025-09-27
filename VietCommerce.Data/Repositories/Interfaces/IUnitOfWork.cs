// Data/Repositories/Interfaces/IUnitOfWork.cs
namespace VietCommerce.Data.Repositories.Interfaces;

public interface IUnitOfWork : IDisposable
{
    IUserRepository Users { get; }
    // Add other repositories as needed
    
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    Task BeginTransactionAsync();
    Task CommitTransactionAsync();
    Task RollbackTransactionAsync();
    IRefreshTokenRepository RefreshTokens { get; }
}