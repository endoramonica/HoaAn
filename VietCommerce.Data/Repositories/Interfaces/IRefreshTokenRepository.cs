// Data/Repositories/Interfaces/IRefreshTokenRepository.cs
using VietCommerce.Core.Entities.Users;

namespace VietCommerce.Data.Repositories.Interfaces;

public interface IRefreshTokenRepository : IGenericRepository<RefreshToken>
{
   
    Task<List<RefreshToken>> GetByUserIdAsync(Guid userId);
    Task<RefreshToken?> GetValidTokenAsync(string token);
    //Task InvalidateTokensAsync(Guid userId);
}