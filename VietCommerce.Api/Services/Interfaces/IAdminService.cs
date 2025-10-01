using Microsoft.Extensions.Logging;
using VietCommerce.Core.DTOs.Admin;
using VietCommerce.Core.DTOs.Users;
using VietCommerce.Core.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace VietCommerce.Api.Services.Interfaces;

public interface IAdminService
{
    Task<ApiResponse<PaginatedResult<UserListDTO>>> GetAllUsersAsync(int pageNumber, int pageSize, string? searchTerm = null, string? role = null);
    Task<ApiResponse<UserDetailDTO>> GetUserByIdAsync(Guid id);
    Task<ApiResponse<UserDetailDTO>> CreateUserAsync(AdminUserCreateDTO request);
    Task<ApiResponse<UserDetailDTO>> UpdateUserAsync(Guid id, AdminUserUpdateDTO request);
    Task<ApiResponse<bool>> DeleteUserAsync(Guid id);
    Task<ApiResponse<bool>> AssignRolesToUserAsync(Guid userId, List<Guid> roleIds);
    Task<ApiResponse<bool>> RemoveRoleFromUserAsync(Guid userId, Guid roleId);
    Task<ApiResponse<List<string>>> GetUserRolesAsync(Guid userId);
    Task<ApiResponse<AdminDashboardDTO>> GetDashboardDataAsync();
    Task<ApiResponse<bool>> ResetUserPasswordAsync(Guid userId, string newPassword);
    Task<ApiResponse<bool>> LockUserAccountAsync(Guid userId);
    Task<ApiResponse<bool>> UnlockUserAccountAsync(Guid userId);
}