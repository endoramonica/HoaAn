// VietCommerce.Api/Services/Interfaces/IRoleService.cs
using VietCommerce.Core.DTOs.Roles;
using VietCommerce.Core.Entities.Users;
using VietCommerce.Core.Models;
namespace VietCommerce.Application.Services.Services.Interfaces;
public interface IRoleService
{
    // Role CRUD operations
    Task<ApiResponse<RoleDetailDTO>> GetRoleByIdAsync(Guid roleId);
    Task<ApiResponse<PaginatedResult<RoleListDTO>>> GetRolesAsync(int page, int pageSize, string? searchTerm = null);
    Task<ApiResponse<RoleDetailDTO>> CreateRoleAsync(RoleCreateDTO request);
    Task<ApiResponse<RoleDetailDTO>> UpdateRoleAsync(Guid roleId, RoleUpdateDTO request);
    Task<ApiResponse<bool>> DeleteRoleAsync(Guid roleId);
    // Role-Permission management
    Task<ApiResponse<List<Permission>>> GetRolePermissionsAsync(Guid roleId);
    Task<ApiResponse<bool>> AssignPermissionToRoleAsync(Guid roleId, Guid permissionId);
    Task<ApiResponse<bool>> RemovePermissionFromRoleAsync(Guid roleId, Guid permissionId);
    Task<ApiResponse<bool>> AssignPermissionsToRoleAsync(Guid roleId, List<Guid> permissionIds);
    Task<ApiResponse<bool>> SyncRolePermissionsAsync(Guid roleId, List<Guid> permissionIds);
    // User-Role management  
    Task<ApiResponse<bool>> AssignRoleToUserAsync(Guid userId, Guid roleId);
    Task<ApiResponse<bool>> RemoveRoleFromUserAsync(Guid userId, Guid roleId);
    Task<ApiResponse<bool>> AssignRolesToUserAsync(Guid userId, List<Guid> roleIds);
    Task<ApiResponse<bool>> SyncUserRolesAsync(Guid userId, List<Guid> roleIds);
    // Validation
    Task<bool> RoleExistsAsync(string roleName);
    Task<bool> UserHasRoleAsync(Guid userId, Guid roleId);
}
