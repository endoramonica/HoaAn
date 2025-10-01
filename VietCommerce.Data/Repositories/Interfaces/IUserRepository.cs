// Updated IUserRepository.cs
using VietCommerce.Core.Entities.Users;
using VietCommerce.Core.DTOs.Users;
using VietCommerce.Data.Repositories.Interfaces;

public interface IUserRepository : IGenericRepository<User>
{
    Task<User?> GetByIdWithRolesAsync(Guid id);
    Task<User?> GetByEmailAsync(string email);
    Task<User?> GetByEmailWithRolesAsync(string email);
    Task<bool> EmailExistsAsync(string email);
    Task<(IEnumerable<UserListDTO> users, int totalCount)> GetUsersPagedAsync(
        int pageNumber, int pageSize, string? searchTerm);
    
    // New methods added
    Task<(IEnumerable<User> users, int totalCount)> GetUsersByRolePagedAsync(
        string roleName, int pageNumber, int pageSize, string? searchTerm = null);
    Task<(IEnumerable<User> users, int totalCount)> GetStaffByStorePagedAsync(
        Guid storeId, int pageNumber, int pageSize, string? searchTerm = null);
}
