using Microsoft.Extensions.Logging;
using VietCommerce.Api.Services.Interfaces;
using VietCommerce.Core.DTOs.Staff;
using VietCommerce.Core.Entities.Users;
using VietCommerce.Core.Enums.Users;
using VietCommerce.Core.Helpers;
using VietCommerce.Core.Models;
using VietCommerce.Core.Common.Constants;
using VietCommerce.Data.Repositories.Interfaces;

namespace VietCommerce.Api.Services;

public class StaffService 
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IRoleService _roleService;
    private readonly ILogger<StaffService> _logger;

    public StaffService(
        IUnitOfWork unitOfWork,
        IRoleService roleService,
        ILogger<StaffService> logger)
    {
        _unitOfWork = unitOfWork;
        _roleService = roleService;
        _logger = logger;
    }

    public async Task<ApiResponse<StaffDetailDTO>> GetStaffByIdAsync(Guid id)
    {
        try
        {
            var user = await _unitOfWork.Users.GetByIdWithRolesAsync(id);
            if (user == null)
                return ApiResponse<StaffDetailDTO>.FailureResponse("Staff not found");

            var isStaff = user.UserRoles.Any(ur => ur.Role.Name == Roles.Staff);
            if (!isStaff)
                return ApiResponse<StaffDetailDTO>.FailureResponse("User is not a staff member");

            var staffDto = MapToStaffDetailDTO(user);
            return ApiResponse<StaffDetailDTO>.SuccessResponse(staffDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving staff {StaffId}", id);
            return ApiResponse<StaffDetailDTO>.FailureResponse("Failed to retrieve staff");
        }
    }

    public async Task<ApiResponse<PaginatedResult<StaffListDTO>>> GetStaffAsync(int pageNumber, int pageSize, string? searchTerm = null)
    {
        try
        {
            var (users, totalCount) = await _unitOfWork.Users.GetUsersByRolePagedAsync(
                Roles.Staff, pageNumber, pageSize, searchTerm);

            var staffList = users.Select(MapToStaffListDTO).ToList();

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
            _logger.LogError(ex, "Error retrieving staff list");
            return ApiResponse<PaginatedResult<StaffListDTO>>.FailureResponse("Failed to retrieve staff");
        }
    }

    public async Task<ApiResponse<StaffDetailDTO>> CreateStaffAsync(StaffCreateDTO request)
    {
        try
        {
            await _unitOfWork.BeginTransactionAsync();

            // Check if email exists
            if (await _unitOfWork.Users.EmailExistsAsync(request.Email))
            {
                await _unitOfWork.RollbackTransactionAsync();
                return ApiResponse<StaffDetailDTO>.FailureResponse("Email already exists");
            }

            // Create user
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

            // Assign Staff role
            var staffRoleResponse = await _roleService.AssignRoleToUserAsync(user.Id, await GetStaffRoleIdAsync());
            if (!staffRoleResponse.Success)
            {
                await _unitOfWork.RollbackTransactionAsync();
                return ApiResponse<StaffDetailDTO>.FailureResponse("Failed to assign staff role");
            }

            await _unitOfWork.CommitTransactionAsync();

            var createdUser = await _unitOfWork.Users.GetByIdWithRolesAsync(user.Id);
            var staffDto = MapToStaffDetailDTO(createdUser!);

            _logger.LogInformation("Staff created successfully with ID {StaffId}", user.Id);
            return ApiResponse<StaffDetailDTO>.SuccessResponse(staffDto, "Staff created successfully");
        }
        catch (Exception ex)
        {
            await _unitOfWork.RollbackTransactionAsync();
            _logger.LogError(ex, "Error creating staff");
            return ApiResponse<StaffDetailDTO>.FailureResponse("Failed to create staff");
        }
    }

    public async Task<ApiResponse<StaffDetailDTO>> UpdateStaffAsync(Guid id, StaffUpdateDTO request)
    {
        try
        {
            await _unitOfWork.BeginTransactionAsync();

            var user = await _unitOfWork.Users.GetByIdWithRolesAsync(id);
            if (user == null)
            {
                await _unitOfWork.RollbackTransactionAsync();
                return ApiResponse<StaffDetailDTO>.FailureResponse("Staff not found");
            }

            var isStaff = user.UserRoles.Any(ur => ur.Role.Name == Roles.Staff);
            if (!isStaff)
            {
                await _unitOfWork.RollbackTransactionAsync();
                return ApiResponse<StaffDetailDTO>.FailureResponse("User is not a staff member");
            }

            // Update fields if provided
            if (!string.IsNullOrEmpty(request.FullName))
                user.Name = request.FullName;

            if (!string.IsNullOrEmpty(request.PhoneNumber))
                user.Phone = request.PhoneNumber;

            if (!string.IsNullOrEmpty(request.Email) && request.Email != user.Email)
            {
                if (await _unitOfWork.Users.EmailExistsAsync(request.Email))
                {
                    await _unitOfWork.RollbackTransactionAsync();
                    return ApiResponse<StaffDetailDTO>.FailureResponse("Email already exists");
                }
                user.Email = request.Email.ToLower();
            }

            // if (request.StoreId.HasValue)
            //     user.StoreId = request.StoreId.Value;

            // user.IsActive = request.IsActive;

            _unitOfWork.Users.Update(user);
            await _unitOfWork.SaveChangesAsync();
            await _unitOfWork.CommitTransactionAsync();

            var updatedUser = await _unitOfWork.Users.GetByIdWithRolesAsync(id);
            var staffDto = MapToStaffDetailDTO(updatedUser!);

            _logger.LogInformation("Staff {StaffId} updated successfully", id);
            return ApiResponse<StaffDetailDTO>.SuccessResponse(staffDto, "Staff updated successfully");
        }
        catch (Exception ex)
        {
            await _unitOfWork.RollbackTransactionAsync();
            _logger.LogError(ex, "Error updating staff {StaffId}", id);
            return ApiResponse<StaffDetailDTO>.FailureResponse("Failed to update staff");
        }
    }

    public async Task<ApiResponse<bool>> DeleteStaffAsync(Guid id)
    {
        try
        {
            await _unitOfWork.BeginTransactionAsync();

            var user = await _unitOfWork.Users.GetByIdWithRolesAsync(id);
            if (user == null)
            {
                await _unitOfWork.RollbackTransactionAsync();
                return ApiResponse<bool>.FailureResponse("Staff not found");
            }

            var isStaff = user.UserRoles.Any(ur => ur.Role.Name == Roles.Staff);
            if (!isStaff)
            {
                await _unitOfWork.RollbackTransactionAsync();
                return ApiResponse<bool>.FailureResponse("User is not a staff member");
            }

            // Soft delete
            user.IsDeleted = true;
            user.DeletedAt = DateTime.UtcNow;
            user.IsActive = false;
            user.Status = UserStatus.INACTIVE;

            _unitOfWork.Users.Update(user);
            await _unitOfWork.SaveChangesAsync();
            await _unitOfWork.CommitTransactionAsync();

            _logger.LogInformation("Staff {StaffId} deleted successfully", id);
            return ApiResponse<bool>.SuccessResponse(true, "Staff deleted successfully");
        }
        catch (Exception ex)
        {
            await _unitOfWork.RollbackTransactionAsync();
            _logger.LogError(ex, "Error deleting staff {StaffId}", id);
            return ApiResponse<bool>.FailureResponse("Failed to delete staff");
        }
    }

    public async Task<ApiResponse<bool>> AssignStaffToStoreAsync(Guid staffId, Guid storeId)
    {
        try
        {
            var user = await _unitOfWork.Users.GetByIdAsync(staffId);
            if (user == null)
                return ApiResponse<bool>.FailureResponse("Staff not found");

            user.StoreId = storeId;
            _unitOfWork.Users.Update(user);
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("Staff {StaffId} assigned to store {StoreId}", staffId, storeId);
            return ApiResponse<bool>.SuccessResponse(true, "Staff assigned to store successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error assigning staff {StaffId} to store {StoreId}", staffId, storeId);
            return ApiResponse<bool>.FailureResponse("Failed to assign staff to store");
        }
    }

    private async Task<Guid> GetStaffRoleIdAsync()
    {
        var staffRole = await _unitOfWork.Roles.GetRoleByNameAsync(Roles.Staff);
        if (staffRole == null)
            throw new InvalidOperationException("Staff role not found");
        return staffRole.Id;
    }

    private static StaffDetailDTO MapToStaffDetailDTO(User user)
    {
        return new StaffDetailDTO
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
            
            Roles = user.UserRoles?.Select(ur => ur.Role.Name).ToList() ?? new List<string>()
        };
    }

    private static StaffListDTO MapToStaffListDTO(User user)
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
