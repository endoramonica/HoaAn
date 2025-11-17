using AutoMapper;
using Microsoft.Extensions.Logging;
using System.Linq.Expressions;
using VietCommerce.Application.Services.Services.Interfaces;
using VietCommerce.Core.DTOs.Users;
using VietCommerce.Core.Entities.Users;
using VietCommerce.Core.Enums.Users;
using VietCommerce.Core.Models;
using VietCommerce.Data.Repositories.Interfaces;
namespace VietCommerce.Application.Services.Services;
public class UserService : IUserService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<UserService> _logger;
    private readonly IGenericServices<User> _userGenericService;
    private readonly IMapper _mapper;
    public UserService(
        IUnitOfWork unitOfWork,
        ILogger<UserService> logger,
        IGenericServices<User> userGenericService,
        IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
        _userGenericService = userGenericService;
        _mapper = mapper;
    }
    #region Generic CRUD Operations
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
    #endregion
    #region Business Logic Operations
    /// Get user by ID with detailed information including roles and store
    public async Task<ApiResponse<UserDetailDTO>> GetUserByIdAsync(Guid id)
    {
        try
        {
            var user = await _unitOfWork.Users.GetByIdWithRolesAsync(id);
            if (user == null)
            {
                _logger.LogWarning("User with ID {UserId} not found", id);
                return ApiResponse<UserDetailDTO>.FailureResponse("User not found", null);
            }
            var userDetailDto = _mapper.Map<UserDetailDTO>(user);
            return ApiResponse<UserDetailDTO>.SuccessResponse(userDetailDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving user with ID {UserId}", id);
            return ApiResponse<UserDetailDTO>.FailureResponse("An error occurred while retrieving user", null);
        }
    }
    /// Get current user profile
    public Task<ApiResponse<UserDetailDTO>> GetUserProfileAsync(Guid userId)
        => GetUserByIdAsync(userId);
    /// Update user profile with validation
    public async Task<ApiResponse<UserDetailDTO>> UpdateUserProfileAsync(Guid userId, UserUpdateDTO request)
    {
        try
        {
            await _unitOfWork.BeginTransactionAsync();
            // Fetch user with roles for complete information
            var user = await _unitOfWork.Users.GetByIdWithRolesAsync(userId);
            if (user == null)
            {
                await _unitOfWork.RollbackTransactionAsync();
                _logger.LogWarning("User with ID {UserId} not found for update", userId);
                return ApiResponse<UserDetailDTO>.FailureResponse("User not found", null);
            }
            // Validate email uniqueness if email is being changed
            if (!string.IsNullOrEmpty(request.Email) && 
                !request.Email.Equals(user.Email, StringComparison.OrdinalIgnoreCase))
            {
                var emailExists = await _unitOfWork.Users.EmailExistsAsync(request.Email);
                if (emailExists)
                {
                    await _unitOfWork.RollbackTransactionAsync();
                    _logger.LogWarning("Email {Email} is already in use", request.Email);
                    return ApiResponse<UserDetailDTO>.FailureResponse("Email is already in use", null);
                }
            }
            // Apply updates using AutoMapper
            _mapper.Map(request, user);
            // Save changes
            _unitOfWork.Users.Update(user);
            await _unitOfWork.SaveChangesAsync();
            await _unitOfWork.CommitTransactionAsync();
            // Reload user to get fresh data with all relationships
            user = await _unitOfWork.Users.GetByIdWithRolesAsync(userId);
            var userDetailDto = _mapper.Map<UserDetailDTO>(user!);
            _logger.LogInformation("User profile {UserId} updated successfully", userId);
            return ApiResponse<UserDetailDTO>.SuccessResponse(userDetailDto, "Profile updated successfully");
        }
        catch (Exception ex)
        {
            await _unitOfWork.RollbackTransactionAsync();
            _logger.LogError(ex, "Error updating profile for user {UserId}", userId);
            return ApiResponse<UserDetailDTO>.FailureResponse("An error occurred while updating profile", null);
        }
    }
    /// Get paginated list of users with optional search
    public async Task<ApiResponse<PaginatedResult<UserListDTO>>> GetUsersAsync(
        int pageNumber, 
        int pageSize, 
        string? searchTerm = null)
    {
        try
        {
            // Fetch paginated users from repository
            var (users, totalCount) = await _unitOfWork.Users.GetUsersPagedAsync(pageNumber, pageSize, searchTerm);
            // Map to DTOs using AutoMapper
            var userListDtos = _mapper.Map<List<UserListDTO>>(users);
            // Create paginated result
            var paginatedResult = new PaginatedResult<UserListDTO>
            {
                Items = userListDtos,
                TotalItems = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize)
            };
            _logger.LogInformation(
                "Retrieved {Count} users (Page {PageNumber}/{TotalPages}) with search term: {SearchTerm}", 
                userListDtos.Count, 
                pageNumber, 
                paginatedResult.TotalPages,
                searchTerm ?? "none");
            return ApiResponse<PaginatedResult<UserListDTO>>.SuccessResponse(paginatedResult);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving users list with search term: {SearchTerm}", searchTerm);
            return ApiResponse<PaginatedResult<UserListDTO>>.FailureResponse(
                "An error occurred while retrieving users", null);
        }
    }
    /// Deactivate a user account
    public async Task<ApiResponse<bool>> DeactivateUserAsync(Guid id)
    {
        try
        {
            await _unitOfWork.BeginTransactionAsync();
            var user = await _unitOfWork.Users.GetByIdAsync(id);
            if (user == null)
            {
                await _unitOfWork.RollbackTransactionAsync();
                _logger.LogWarning("User with ID {UserId} not found for deactivation", id);
                return ApiResponse<bool>.FailureResponse("User not found", null);
            }
            // Check if already inactive (idempotent operation)
            if (!user.IsActive && user.Status == UserStatus.INACTIVE)
            {
                await _unitOfWork.RollbackTransactionAsync();
                _logger.LogInformation("User {UserId} is already inactive", id);
                return ApiResponse<bool>.SuccessResponse(true, "User is already inactive");
            }
            // Deactivate user
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
    /// Activate a user account
    public async Task<ApiResponse<bool>> ActivateUserAsync(Guid id)
    {
        try
        {
            await _unitOfWork.BeginTransactionAsync();
            var user = await _unitOfWork.Users.GetByIdAsync(id);
            if (user == null)
            {
                await _unitOfWork.RollbackTransactionAsync();
                _logger.LogWarning("User with ID {UserId} not found for activation", id);
                return ApiResponse<bool>.FailureResponse("User not found", null);
            }
            // Check if already active (idempotent operation)
            if (user.IsActive && user.Status == UserStatus.ACTIVE)
            {
                await _unitOfWork.RollbackTransactionAsync();
                _logger.LogInformation("User {UserId} is already active", id);
                return ApiResponse<bool>.SuccessResponse(true, "User is already active");
            }
            // Activate user
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
    #endregion
}
