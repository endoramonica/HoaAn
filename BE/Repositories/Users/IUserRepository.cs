using VietCommerce.Core.Entities.Users;
using VietCommerce.Data.Repositories.Generic;

namespace VietCommerce.Data.Repositories.Users;

public interface IUserRepository : IGenericRepository<User>
{
    Task<User?> GetByEmailAsync(string email);
    Task<bool> ExistsByEmailAsync(string email);
}