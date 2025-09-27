using Microsoft.Extensions.Logging;
using System.Linq.Expressions;
using VietCommerce.Api.Services.Interfaces;
using VietCommerce.Core.DTOs.Users;
using VietCommerce.Core.Entities.Users;
using VietCommerce.Core.Enums.Users;
using VietCommerce.Core.Models;
using VietCommerce.Data.Repositories.Interfaces;

namespace VietCommerce.Api.Services;

public class UserService : IUserService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<UserService> _logger;
    private readonly IGenericServices<User> _userGenericService;

    public UserService(
        IUnitOfWork unitOfWork,
        ILogger<UserService> logger,
        IGenericServices<User> userGenericService)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
        _userGenericService = userGenericService;
    }

    // Tận dụng generic service CRUD
    public Task<IEnumerable<User>> GetAllAsync() => _userGenericService.GetAllAsync();
    public Task<User?> GetByIdAsync(Guid id) => _userGenericService.GetByIdAsync(id);
    public Task<IEnumerable<User>> FindAsync(Expression<Func<User, bool>> predicate) => _userGenericService.FindAsync(predicate);
    public Task<User> AddAsync(User entity) => _userGenericService.AddAsync(entity);
    public Task<IEnumerable<User>> AddRangeAsync(IEnumerable<User> entities) => _userGenericService.AddRangeAsync(entities);
    public Task<User> UpdateAsync(User entity) => _userGenericService.UpdateAsync(entity);
    public Task DeleteAsync(User entity) => _userGenericService.DeleteAsync(entity);
    public Task DeleteByIdAsync(Guid id) => _userGenericService.DeleteByIdAsync(id);
    public Task DeleteRangeAsync(IEnumerable<User> entities) => _userGenericService.DeleteRangeAsync(entities);
    public Task<bool> ExistsAsync(Expression<Func<User, bool>> predicate) => _userGenericService.ExistsAsync(predicate);
    public Task<int> CountAsync(Expression<Func<User, bool>> predicate) => _userGenericService.CountAsync(predicate);

    // Business logic đặc thù
    public async Task<ApiResponse<UserDetailDTO>> GetUserByIdAsync(Guid id)
    {
        try
        {
            var user = await _unitOfWork.Users.GetByIdWithRolesAsync(id);
            if (user == null)
                return ApiResponse<UserDetailDTO>.FailureResponse("User not found", null);

            return ApiResponse<UserDetailDTO>.SuccessResponse(MapToUserDetailDTO(user));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting user {UserId}", id);
            return ApiResponse<UserDetailDTO>.FailureResponse("An error occurred while retrieving user", null);
        }
    }

    public Task<ApiResponse<UserDetailDTO>> GetUserProfileAsync(Guid userId)
        => GetUserByIdAsync(userId);

    public async Task<ApiResponse<UserDetailDTO>> UpdateUserProfileAsync(Guid userId, UserUpdateDTO request)
    {
        try
        {
            await _unitOfWork.BeginTransactionAsync();

            var user = await _unitOfWork.Users.GetByIdWithRolesAsync(userId);
            if (user == null)
            {
                await _unitOfWork.RollbackTransactionAsync();
                return ApiResponse<UserDetailDTO>.FailureResponse("User not found", null);
            }

            if (!string.IsNullOrEmpty(request.Email) && request.Email != user.Email)
            {
                if (await _unitOfWork.Users.EmailExistsAsync(request.Email))
                {
                    await _unitOfWork.RollbackTransactionAsync();
                    return ApiResponse<UserDetailDTO>.FailureResponse("Email is already in use", null);
                }
                user.Email = request.Email.ToLower();
            }

            if (!string.IsNullOrEmpty(request.FullName))
                user.Name = request.FullName;

            if (!string.IsNullOrEmpty(request.PhoneNumber))
                user.Phone = request.PhoneNumber;

            user.IsActive = request.IsActive;

            _unitOfWork.Users.Update(user);
            await _unitOfWork.SaveChangesAsync();
            await _unitOfWork.CommitTransactionAsync();

            return ApiResponse<UserDetailDTO>.SuccessResponse(MapToUserDetailDTO(user), "Profile updated successfully");
        }
        catch (Exception ex)
        {
            await _unitOfWork.RollbackTransactionAsync();
            _logger.LogError(ex, "Error updating profile for user {UserId}", userId);
            return ApiResponse<UserDetailDTO>.FailureResponse("An error occurred while updating profile", null);
        }
    }

    public async Task<ApiResponse<PaginatedResult<UserListDTO>>> GetUsersAsync(int pageNumber, int pageSize, string? searchTerm = null)
    {
        try
        {
            var (users, totalCount) = await _unitOfWork.Users.GetUsersPagedAsync(pageNumber, pageSize, searchTerm);

            var paginatedResult = new PaginatedResult<UserListDTO>
            {
                Items = users.ToList(),
                TotalItems = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize)
            };

            return ApiResponse<PaginatedResult<UserListDTO>>.SuccessResponse(paginatedResult);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting users list with search term: {SearchTerm}", searchTerm);
            return ApiResponse<PaginatedResult<UserListDTO>>.FailureResponse("An error occurred while retrieving users", null);
        }
    }

    public async Task<ApiResponse<bool>> DeactivateUserAsync(Guid id)
    {
        try
        {
            await _unitOfWork.BeginTransactionAsync();

            var user = await _unitOfWork.Users.GetByIdAsync(id);
            if (user == null)
            {
                await _unitOfWork.RollbackTransactionAsync();
                return ApiResponse<bool>.FailureResponse("User not found", null);
            }

            user.IsActive = false;
            user.Status = UserStatus.INACTIVE;

            _unitOfWork.Users.Update(user);
            await _unitOfWork.SaveChangesAsync();
            await _unitOfWork.CommitTransactionAsync();

            _logger.LogInformation("User {UserId} deactivated successfully", id);
            return ApiResponse<bool>.SuccessResponse(true, "User deactivated successfully");
        }
        catch (Exception ex)
        {
            await _unitOfWork.RollbackTransactionAsync();
            _logger.LogError(ex, "Error deactivating user {UserId}", id);
            return ApiResponse<bool>.FailureResponse("An error occurred while deactivating user", null);
        }
    }

    public async Task<ApiResponse<bool>> ActivateUserAsync(Guid id)
    {
        try
        {
            await _unitOfWork.BeginTransactionAsync();

            var user = await _unitOfWork.Users.GetByIdAsync(id);
            if (user == null)
            {
                await _unitOfWork.RollbackTransactionAsync();
                return ApiResponse<bool>.FailureResponse("User not found", null);
            }

            user.IsActive = true;
            user.Status = UserStatus.ACTIVE;

            _unitOfWork.Users.Update(user);
            await _unitOfWork.SaveChangesAsync();
            await _unitOfWork.CommitTransactionAsync();

            _logger.LogInformation("User {UserId} activated successfully", id);
            return ApiResponse<bool>.SuccessResponse(true, "User activated successfully");
        }
        catch (Exception ex)
        {
            await _unitOfWork.RollbackTransactionAsync();
            _logger.LogError(ex, "Error activating user {UserId}", id);
            return ApiResponse<bool>.FailureResponse("An error occurred while activating user", null);
        }
    }

    private UserDetailDTO MapToUserDetailDTO(User user)
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
