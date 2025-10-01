using Microsoft.Extensions.Logging;
using VietCommerce.Api.Services.Interfaces;
using VietCommerce.Core.DTOs.Manager;
using VietCommerce.Core.DTOs.Staff;
using VietCommerce.Core.Entities.Users;
using VietCommerce.Core.Enums.Users;
using VietCommerce.Core.Helpers;
using VietCommerce.Core.Models;
using VietCommerce.Core.Common.Constants;
using VietCommerce.Data.Repositories.Interfaces;

namespace VietCommerce.Api.Services;

public class ManagerService : IManagerService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IRoleService _roleService;
    private readonly ILogger<ManagerService> _logger;

    public ManagerService(
        IUnitOfWork unitOfWork,
        IRoleService roleService,
        ILogger<ManagerService> logger)
    {
        _unitOfWork = unitOfWork;
        _roleService = roleService;
        _logger = logger;
    }

    public async Task<ApiResponse<ManagerDetailDTO>> GetManagerByIdAsync(Guid id)
    {
        try
        {
            var user = await _unitOfWork.Users.GetByIdWithRolesAsync(id);
            if (user == null)
                return ApiResponse<ManagerDetailDTO>.FailureResponse("Manager not found");

            var isManager = user.UserRoles.Any(ur => ur.Role.Name == Roles.Manager);
            if (!isManager)
                return ApiResponse<ManagerDetailDTO>.FailureResponse("User is not a manager");

            var managerDto = MapToManagerDetailDTO(user);
            return ApiResponse<ManagerDetailDTO>.SuccessResponse(managerDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving manager {ManagerId}", id);
            return ApiResponse<ManagerDetailDTO>.FailureResponse("Failed to retrieve manager");
        }
    }

    public async Task<ApiResponse<PaginatedResult<ManagerListDTO>>> GetManagersAsync(int pageNumber, int pageSize, string? searchTerm = null)
    {
        try
        {
            var (users, totalCount) = await _unitOfWork.Users.GetUsersByRolePagedAsync(
                Roles.Manager, pageNumber, pageSize, searchTerm);

            var managerList = users.Select(MapToManagerListDTO).ToList();

            var result = new PaginatedResult<ManagerListDTO>
            {
                Items = managerList,
                TotalItems = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize)
            };

            return ApiResponse<PaginatedResult<ManagerListDTO>>.SuccessResponse(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving manager list");
            return ApiResponse<PaginatedResult<ManagerListDTO>>.FailureResponse("Failed to retrieve managers");
        }
    }

    public async Task<ApiResponse<ManagerDetailDTO>> CreateManagerAsync(ManagerCreateDTO request)
    {
        try
        {
            await _unitOfWork.BeginTransactionAsync();

            if (await _unitOfWork.Users.EmailExistsAsync(request.Email))
            {
                await _unitOfWork.RollbackTransactionAsync();
                return ApiResponse<ManagerDetailDTO>.FailureResponse("Email already exists");
            }

            var user = new User
            {
                Email = request.Email.ToLower(),
                PasswordHash = PasswordHelper.HashPassword(request.Password),
                Name = request.FullName,
                Phone = request.PhoneNumber,
                StoreId = request.StoreId,
                IsActive = true,
                Status = UserStatus.ACTIVE
            };

            await _unitOfWork.Users.AddAsync(user);
            await _unitOfWork.SaveChangesAsync();

            // Assign Manager role
            var managerRoleResponse = await _roleService.AssignRoleToUserAsync(user.Id, await GetManagerRoleIdAsync());
            if (!managerRoleResponse.Success)
            {
                await _unitOfWork.RollbackTransactionAsync();
                return ApiResponse<ManagerDetailDTO>.FailureResponse("Failed to assign manager role");
            }

            await _unitOfWork.CommitTransactionAsync();

            var createdUser = await _unitOfWork.Users.GetByIdWithRolesAsync(user.Id);
            var managerDto = MapToManagerDetailDTO(createdUser!);

            _logger.LogInformation("Manager created successfully with ID {ManagerId}", user.Id);
            return ApiResponse<ManagerDetailDTO>.SuccessResponse(managerDto, "Manager created successfully");
        }
        catch (Exception ex)
        {
            await _unitOfWork.RollbackTransactionAsync();
            _logger.LogError(ex, "Error creating manager");
            return ApiResponse<ManagerDetailDTO>.FailureResponse("Failed to create manager");
        }
    }

    public async Task<ApiResponse<ManagerDetailDTO>> UpdateManagerAsync(Guid id, ManagerUpdateDTO request)
    {
        try
        {
            await _unitOfWork.BeginTransactionAsync();

            var user = await _unitOfWork.Users.GetByIdWithRolesAsync(id);
            if (user == null)
            {
                await _unitOfWork.RollbackTransactionAsync();
                return ApiResponse<ManagerDetailDTO>.FailureResponse("Manager not found");
            }

            var isManager = user.UserRoles.Any(ur => ur.Role.Name == Roles.Manager);
            if (!isManager)
            {
                await _unitOfWork.RollbackTransactionAsync();
                return ApiResponse<ManagerDetailDTO>.FailureResponse("User is not a manager");
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
                    return ApiResponse<ManagerDetailDTO>.FailureResponse("Email already exists");
                }
                user.Email = request.Email.ToLower();
            }

            if (request.StoreId.HasValue)
                user.StoreId = request.StoreId.Value;

            user.IsActive = request.IsActive;

            _unitOfWork.Users.Update(user);
            await _unitOfWork.SaveChangesAsync();
            await _unitOfWork.CommitTransactionAsync();

            var updatedUser = await _unitOfWork.Users.GetByIdWithRolesAsync(id);
            var managerDto = MapToManagerDetailDTO(updatedUser!);

            _logger.LogInformation("Manager {ManagerId} updated successfully", id);
            return ApiResponse<ManagerDetailDTO>.SuccessResponse(managerDto, "Manager updated successfully");
        }
        catch (Exception ex)
        {
            await _unitOfWork.RollbackTransactionAsync();
            _logger.LogError(ex, "Error updating manager {ManagerId}", id);
            return ApiResponse<ManagerDetailDTO>.FailureResponse("Failed to update manager");
        }
    }

    public async Task<ApiResponse<bool>> DeleteManagerAsync(Guid id)
    {
        try
        {
            await _unitOfWork.BeginTransactionAsync();

            var user = await _unitOfWork.Users.GetByIdWithRolesAsync(id);
            if (user == null)
            {
                await _unitOfWork.RollbackTransactionAsync();
                return ApiResponse<bool>.FailureResponse("Manager not found");
            }

            var isManager = user.UserRoles.Any(ur => ur.Role.Name == Roles.Manager);
            if (!isManager)
            {
                await _unitOfWork.RollbackTransactionAsync();
                return ApiResponse<bool>.FailureResponse("User is not a manager");
            }

            user.IsDeleted = true;
            user.DeletedAt = DateTime.UtcNow;
            user.IsActive = false;
            user.Status = UserStatus.INACTIVE;

            _unitOfWork.Users.Update(user);
            await _unitOfWork.SaveChangesAsync();
            await _unitOfWork.CommitTransactionAsync();

            _logger.LogInformation("Manager {ManagerId} deleted successfully", id);
            return ApiResponse<bool>.SuccessResponse(true, "Manager deleted successfully");
        }
        catch (Exception ex)
        {
            await _unitOfWork.RollbackTransactionAsync();
            _logger.LogError(ex, "Error deleting manager {ManagerId}", id);
            return ApiResponse<bool>.FailureResponse("Failed to delete manager");
        }
    }

    public async Task<ApiResponse<PaginatedResult<StaffListDTO>>> GetManagedStaffAsync(Guid managerId, int pageNumber, int pageSize, string? searchTerm = null)
    {
        try
        {
            var manager = await _unitOfWork.Users.GetByIdAsync(managerId);
            if (manager == null)
                return ApiResponse<PaginatedResult<StaffListDTO>>.FailureResponse("Manager not found");

            // Get staff in the same store as the manager
            var (users, totalCount) = await _unitOfWork.Users.GetStaffByStorePagedAsync(
                manager.StoreId, pageNumber, pageSize, searchTerm);

            var staffList = users.Select(MapUserToStaffListDTO).ToList();

            var result = new PaginatedResult<StaffListDTO>
            {
                Items = staffList,
                TotalItems = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize)
            };

            return ApiResponse<PaginatedResult<StaffListDTO>>.SuccessResponse(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving staff for manager {ManagerId}", managerId);
            return ApiResponse<PaginatedResult<StaffListDTO>>.FailureResponse("Failed to retrieve managed staff");
        }
    }

    public async Task<ApiResponse<bool>> AssignStoreToManagerAsync(Guid managerId, Guid storeId)
    {
        try
        {
            var user = await _unitOfWork.Users.GetByIdAsync(managerId);
            if (user == null)
                return ApiResponse<bool>.FailureResponse("Manager not found");

            user.StoreId = storeId;
            _unitOfWork.Users.Update(user);
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("Manager {ManagerId} assigned to store {StoreId}", managerId, storeId);
            return ApiResponse<bool>.SuccessResponse(true, "Manager assigned to store successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error assigning manager {ManagerId} to store {StoreId}", managerId, storeId);
            return ApiResponse<bool>.FailureResponse("Failed to assign manager to store");
        }
    }

    private async Task<Guid> GetManagerRoleIdAsync()
    {
        var managerRole = await _unitOfWork.Roles.GetRoleByNameAsync(Roles.Manager);
        if (managerRole == null)
            throw new InvalidOperationException("Manager role not found");
        return managerRole.Id;
    }

    private static ManagerDetailDTO MapToManagerDetailDTO(User user)
    {
        return new ManagerDetailDTO
        {
            Id = user.Id,
            FullName = user.Name ?? "",
            Email = user.Email,
            PhoneNumber = user.Phone,
            StoreId = user.StoreId,
            StoreName = user.Store?.Name ?? "",
            IsActive = user.IsActive,
            Status = user.Status,
            LastLogin = user.LastLogin,
            CreatedAt = user.CreatedAt,
            UpdatedAt = user.UpdatedAt,
            Roles = user.UserRoles?.Select(ur => ur.Role.Name).ToList() ?? new List<string>()
        };
    }

    private static ManagerListDTO MapToManagerListDTO(User user)
    {
        return new ManagerListDTO
        {
            Id = user.Id,
            FullName = user.Name ?? "",
            Email = user.Email,
            PhoneNumber = user.Phone,
            StoreName = user.Store?.Name ?? "",
            IsActive = user.IsActive,
            CreatedDate = user.CreatedAt,
            LastLogin = user.LastLogin
        };
    }

    private static StaffListDTO MapUserToStaffListDTO(User user)
    {
        return new StaffListDTO
        {
            Id = user.Id,
            FullName = user.Name ?? "",
            Email = user.Email,
            PhoneNumber = user.Phone,
            StoreName = user.Store?.Name ?? "",
            IsActive = user.IsActive,
            CreatedDate = user.CreatedAt,
            LastLogin = user.LastLogin
        };
    }
}
