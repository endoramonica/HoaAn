// Core/Services/Interfaces/IUserService.cs
using VietCommerce.Core.DTOs.Users;
using VietCommerce.Core.Entities.Users;
using VietCommerce.Core.Models;
namespace VietCommerce.Api.Services.Interfaces;
public interface IUserService : IGenericServices<User>
{
    Task<ApiResponse<UserDetailDTO>> GetUserByIdAsync(Guid id);
    Task<ApiResponse<UserDetailDTO>> GetUserProfileAsync(Guid userId);
    Task<ApiResponse<UserDetailDTO>> UpdateUserProfileAsync(Guid userId, UserUpdateDTO request);
    Task<ApiResponse<PaginatedResult<UserListDTO>>> GetUsersAsync(int pageNumber, int pageSize, string? searchTerm = null);
    Task<ApiResponse<bool>> DeactivateUserAsync(Guid id);
    Task<ApiResponse<bool>> ActivateUserAsync(Guid id);
}
