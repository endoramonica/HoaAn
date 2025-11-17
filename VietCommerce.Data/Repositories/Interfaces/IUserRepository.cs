using VietCommerce.Core.DTOs.Users;
using VietCommerce.Core.Entities.Users;
using VietCommerce.Data.Repositories.Interfaces;

public interface IUserRepository : IGenericRepository<User>
{
       // Cho phép null nếu không tìm thấy
    Task<User?> GetByIdWithRolesAsync(Guid id);      // Cho phép null nếu không tìm thấy
    Task<User?> GetByEmailAsync(string email);       // Thêm vào interface
    Task<User?> GetByEmailWithRolesAsync(string email); // Thêm vào interface
    Task<bool> EmailExistsAsync(string email);
    Task<(IEnumerable<UserListDTO> users, int totalCount)> GetUsersPagedAsync(
        int pageNumber,
        int pageSize,
        string? searchTerm
    );
    Task<User?> GetByEmailWithRolesAndPermissionsAsync(string email);


}
