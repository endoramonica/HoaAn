using Microsoft.Extensions.Logging;
using VietCommerce.Api.Services.Interfaces;
using VietCommerce.Core.DTOs.Admin;
using VietCommerce.Core.DTOs.Users;
using VietCommerce.Core.Entities.Users;
using VietCommerce.Core.Enums.Users;
using VietCommerce.Core.Helpers;
using VietCommerce.Core.Models;
using VietCommerce.Core.Common.Constants;
using VietCommerce.Data.Repositories.Interfaces;

namespace VietCommerce.Api.Services;

public class AdminService : IAdminService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IRoleService _roleService;
    private readonly IPermissionService _permissionService;
    private readonly ILogger<AdminService> _logger;

    public AdminService(
        IUnitOfWork unitOfWork,
        IRoleService roleService,
        IPermissionService permissionService,
        ILogger<AdminService> logger)
    {
        _unitOfWork = unitOfWork;
        _roleService = roleService;
        _permissionService = permissionService;
        _logger = logger;
    }

    public async Task<ApiResponse<PaginatedResult<UserListDTO>>> GetAllUsersAsync(int pageNumber, int pageSize, string? searchTerm = null, string? role = null)
    {
        try
        {
            var (users, totalCount) = string.IsNullOrEmpty(role) 
                ? await _unitOfWork.Users.GetUsersPagedAsync(pageNumber, pageSize, searchTerm)
                : await _unitOfWork.Users.GetUsersByRolePagedAsync(role, pageNumber, pageSize, searchTerm);

            var result = new PaginatedResult<UserListDTO>
            {
                Items = users.ToList(),
                TotalItems = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize)
            };

            return ApiResponse<PaginatedResult<UserListDTO>>.SuccessResponse(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving all users");
            return ApiResponse<PaginatedResult<UserListDTO>>.FailureResponse("Failed to retrieve users");
        }
    }

    public async Task<ApiResponse<UserDetailDTO>> GetUserByIdAsync(Guid id)
    {
        try
        {
            var user = await _unitOfWork.Users.GetByIdWithRolesAsync(id);
            if (user == null)
                return ApiResponse<UserDetailDTO>.FailureResponse("User not found");

            var userDto = MapToUserDetailDTO(user);
            return ApiResponse<UserDetailDTO>.SuccessResponse(userDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving user {UserId}", id);
            return ApiResponse<UserDetailDTO>.FailureResponse("Failed to retrieve user");
        }
    }

    public async Task<ApiResponse<UserDetailDTO>> CreateUserAsync(AdminUserCreateDTO request)
    {
        try
        {
            await _unitOfWork.BeginTransactionAsync();

            if (await _unitOfWork.Users.EmailExistsAsync(request.Email))
            {
                await _unitOfWork.RollbackTransactionAsync();
                return ApiResponse<UserDetailDTO>.FailureResponse("Email already exists");
            }

            var user = new User
            {
                Email = request.Email.ToLower(),
                PasswordHash = PasswordHelper.HashPassword(request.Password),
                Name = request.FullName,
                Phone = request.PhoneNumber,
                StoreId = request.StoreId,
                IsActive = request.IsActive,
                Status = request.IsActive ? UserStatus.ACTIVE : UserStatus.INACTIVE
            };

            await _unitOfWork.Users.AddAsync(user);
            await _unitOfWork.SaveChangesAsync();

            // Assign roles if provided
            if (request.RoleIds != null && request.RoleIds.Any())
            {
                var roleAssignResult = await _roleService.AssignRolesToUserAsync(user.Id, request.RoleIds);
                if (!roleAssignResult.Success)
                {
                    await _unitOfWork.RollbackTransactionAsync();
                    return ApiResponse<UserDetailDTO>.FailureResponse("Failed to assign roles to user");
                }
            }

            await _unitOfWork.CommitTransactionAsync();

            var createdUser = await _unitOfWork.Users.GetByIdWithRolesAsync(user.Id);
            var userDto = MapToUserDetailDTO(createdUser!);

            _logger.LogInformation("User created by admin with ID {UserId}", user.Id);
            return ApiResponse<UserDetailDTO>.SuccessResponse(userDto, "User created successfully");
        }
        catch (Exception ex)
        {
            await _unitOfWork.RollbackTransactionAsync();
            _logger.LogError(ex, "Error creating user by admin");
            return ApiResponse<UserDetailDTO>.FailureResponse("Failed to create user");
        }
    }

    public async Task<ApiResponse<UserDetailDTO>> UpdateUserAsync(Guid id, AdminUserUpdateDTO request)
    {
        try
        {
            await _unitOfWork.BeginTransactionAsync();

            var user = await _unitOfWork.Users.GetByIdWithRolesAsync(id);
            if (user == null)
            {
                await _unitOfWork.RollbackTransactionAsync();
                return ApiResponse<UserDetailDTO>.FailureResponse("User not found");
            }

            if (!string.IsNullOrEmpty(request.FullName))
                user.Name = request.FullName;

            if (!string.IsNullOrEmpty(request.PhoneNumber))
                user.Phone = request.PhoneNumber;

            if (!string.IsNullOrEmpty(request.Email) && request.Email != user.Email)
            {
                if (await _unitOfWork.Users.EmailExistsAsync(request.Email))
                {
                    await _unitOfWork.RollbackTransactionAsync();
                    return ApiResponse<UserDetailDTO>.FailureResponse("Email already exists");
                }
                user.Email = request.Email.ToLower();
            }

            if (request.StoreId.HasValue)
                user.StoreId = request.StoreId.Value;

            user.IsActive = request.IsActive;
            user.Status = request.IsActive ? UserStatus.ACTIVE : UserStatus.INACTIVE;

            _unitOfWork.Users.Update(user);

            // Update roles if provided
            if (request.RoleIds != null)
            {
                var roleUpdateResult = await _roleService.SyncUserRolesAsync(user.Id, request.RoleIds);
                if (!roleUpdateResult.Success)
                {
                    await _unitOfWork.RollbackTransactionAsync();
                    return ApiResponse<UserDetailDTO>.FailureResponse("Failed to update user roles");
                }
            }

            await _unitOfWork.SaveChangesAsync();
            await _unitOfWork.CommitTransactionAsync();

            var updatedUser = await _unitOfWork.Users.GetByIdWithRolesAsync(id);
            var userDto = MapToUserDetailDTO(updatedUser!);

            _logger.LogInformation("User {UserId} updated by admin", id);
            return ApiResponse<UserDetailDTO>.SuccessResponse(userDto, "User updated successfully");
        }
        catch (Exception ex)
        {
            await _unitOfWork.RollbackTransactionAsync();
            _logger.LogError(ex, "Error updating user {UserId} by admin", id);
            return ApiResponse<UserDetailDTO>.FailureResponse("Failed to update user");
        }
    }

    public async Task<ApiResponse<bool>> DeleteUserAsync(Guid id)
    {
        try
        {
            await _unitOfWork.BeginTransactionAsync();

            var user = await _unitOfWork.Users.GetByIdAsync(id);
            if (user == null)
            {
                await _unitOfWork.RollbackTransactionAsync();
                return ApiResponse<bool>.FailureResponse("User not found");
            }

            // Soft delete
            user.IsDeleted = true;
            user.DeletedAt = DateTime.UtcNow;
            user.IsActive = false;
            user.Status = UserStatus.INACTIVE;

            _unitOfWork.Users.Update(user);
            await _unitOfWork.SaveChangesAsync();
            await _unitOfWork.CommitTransactionAsync();

            // Clear user permissions cache
            _permissionService.ClearUserPermissionsCache(id);

            _logger.LogInformation("User {UserId} deleted by admin", id);
            return ApiResponse<bool>.SuccessResponse(true, "User deleted successfully");
        }
        catch (Exception ex)
        {
            await _unitOfWork.RollbackTransactionAsync();
            _logger.LogError(ex, "Error deleting user {UserId} by admin", id);
            return ApiResponse<bool>.FailureResponse("Failed to delete user");
        }
    }

    public async Task<ApiResponse<bool>> AssignRolesToUserAsync(Guid userId, List<Guid> roleIds)
    {
        try
        {
            var result = await _roleService.AssignRolesToUserAsync(userId, roleIds);
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error assigning roles to user {UserId} by admin", userId);
            return ApiResponse<bool>.FailureResponse("Failed to assign roles");
        }
    }

    public async Task<ApiResponse<bool>> RemoveRoleFromUserAsync(Guid userId, Guid roleId)
    {
        try
        {
            var result = await _roleService.RemoveRoleFromUserAsync(userId, roleId);
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error removing role from user {UserId} by admin", userId);
            return ApiResponse<bool>.FailureResponse("Failed to remove role");
        }
    }

    public async Task<ApiResponse<List<string>>> GetUserRolesAsync(Guid userId)
    {
        try
        {
            var roles = await _permissionService.GetUserRolesAsync(userId);
            var roleNames = roles.Select(r => r.Name).ToList();
            return ApiResponse<List<string>>.SuccessResponse(roleNames);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving roles for user {UserId}", userId);
            return ApiResponse<List<string>>.FailureResponse("Failed to retrieve user roles");
        }
    }

    public async Task<ApiResponse<AdminDashboardDTO>> GetDashboardDataAsync()
    {
        try
        {
            var totalUsers = await _unitOfWork.Users.CountAsync(u => !u.IsDeleted);
            var activeUsers = await _unitOfWork.Users.CountAsync(u => u.IsActive && !u.IsDeleted);
            var totalCustomers = await _unitOfWork.Customers.CountAsync(c => !c.IsDeleted);
            var totalStores = await _unitOfWork.Stores.CountAsync();

            var dashboard = new AdminDashboardDTO
            {
                TotalUsers = totalUsers,
                ActiveUsers = activeUsers,
                InactiveUsers = totalUsers - activeUsers,
                TotalCustomers = totalCustomers,
                TotalStores = totalStores,
                LastUpdated = DateTime.UtcNow
            };

            return ApiResponse<AdminDashboardDTO>.SuccessResponse(dashboard);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving dashboard data");
            return ApiResponse<AdminDashboardDTO>.FailureResponse("Failed to retrieve dashboard data");
        }
    }

    public async Task<ApiResponse<bool>> ResetUserPasswordAsync(Guid userId, string newPassword)
    {
        try
        {
            var user = await _unitOfWork.Users.GetByIdAsync(userId);
            if (user == null)
                return ApiResponse<bool>.FailureResponse("User not found");

            user.PasswordHash = PasswordHelper.HashPassword(newPassword);
            _unitOfWork.Users.Update(user);
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("Password reset by admin for user {UserId}", userId);
            return ApiResponse<bool>.SuccessResponse(true, "Password reset successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error resetting password for user {UserId} by admin", userId);
            return ApiResponse<bool>.FailureResponse("Failed to reset password");
        }
    }

    public async Task<ApiResponse<bool>> LockUserAccountAsync(Guid userId)
    {
        try
        {
            var user = await _unitOfWork.Users.GetByIdAsync(userId);
            if (user == null)
                return ApiResponse<bool>.FailureResponse("User not found");

            user.Status = UserStatus.LOCKED;
            user.IsActive = false;
            _unitOfWork.Users.Update(user);
            await _unitOfWork.SaveChangesAsync();

            _permissionService.ClearUserPermissionsCache(userId);

            _logger.LogInformation("User account {UserId} locked by admin", userId);
            return ApiResponse<bool>.SuccessResponse(true, "User account locked successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error locking user account {UserId} by admin", userId);
            return ApiResponse<bool>.FailureResponse("Failed to lock user account");
        }
    }

    public async Task<ApiResponse<bool>> UnlockUserAccountAsync(Guid userId)
    {
        try
        {
            var user = await _unitOfWork.Users.GetByIdAsync(userId);
            if (user == null)
                return ApiResponse<bool>.FailureResponse("User not found");

            user.Status = UserStatus.ACTIVE;
            user.IsActive = true;
            _unitOfWork.Users.Update(user);
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("User account {UserId} unlocked by admin", userId);
            return ApiResponse<bool>.SuccessResponse(true, "User account unlocked successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error unlocking user account {UserId} by admin", userId);
            return ApiResponse<bool>.FailureResponse("Failed to unlock user account");
        }
    }

    private static UserDetailDTO MapToUserDetailDTO(User user)
    {
        return new UserDetailDTO
        {
            Id = user.Id,
            Email = user.Email,
            Name = user.Name,
            Phone = user.Phone,
            IsActive = user.IsActive,
            Status = user.Status,
            LastLogin = user.LastLogin,
            StoreId = user.StoreId,
            StoreName = user.Store?.Name ?? "",
            CreatedAt = user.CreatedAt,
            Roles = user.UserRoles?.Select(ur => ur.Role.Name).ToList() ?? new List<string>()
        };
    }
}
