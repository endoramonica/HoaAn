using VietCommerce.Core.DTOs.Users;
using VietCommerce.Core.Models;

namespace VietCommerce.Core.Services.Users;

public interface IUserService
{
    Task<PaginatedResult<UserListDTO>> GetPaginatedAsync(int pageNumber = 1, int pageSize = 10);
    Task<UserListDTO> GetByIdAsync(Guid id);
    Task<Guid> CreateAsync(UserCreateDTO dto);
    Task UpdateAsync(Guid id, UserUpdateDTO dto);
    Task DeleteAsync(Guid id);
    Task<UserListDTO?> AuthenticateAsync(string email, string password);
}