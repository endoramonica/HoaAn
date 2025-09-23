using AutoMapper;
using BCrypt.Net;
using FluentValidation;
using Microsoft.Extensions.Logging;
using VietCommerce.Core.Common.Exceptions;
using VietCommerce.Core.DTOs.Users;
using VietCommerce.Core.Models;
using VietCommerce.Core.Services.Users;
using VietCommerce.Data.Repositories.Users;
using VietCommerce.Core.Entities.Users;

namespace VietCommerce.Core.Services.Users;

public class UserService : IUserService
{
    private readonly IUserRepository _repository;
    private readonly IMapper _mapper;
    private readonly IValidator<UserCreateDTO> _createValidator;
    private readonly IValidator<UserUpdateDTO> _updateValidator;
    private readonly ILogger<UserService> _logger;

    public UserService(IUserRepository repository, IMapper mapper, IValidator<UserCreateDTO> createValidator, IValidator<UserUpdateDTO> updateValidator, ILogger<UserService> logger)
    {
        _repository = repository;
        _mapper = mapper;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
        _logger = logger;
    }

    public async Task<PaginatedResult<UserListDTO>> GetPaginatedAsync(int pageNumber = 1, int pageSize = 10)
    {
        var users = await _repository.GetPaginatedAsync(pageNumber, pageSize);
        var total = await _repository.GetTotalCountAsync();
        var dtos = _mapper.Map<IEnumerable<UserListDTO>>(users);
        return new PaginatedResult<UserListDTO>(dtos, pageNumber, pageSize, total);
    }

    public async Task<UserListDTO> GetByIdAsync(Guid id)
    {
        var user = await _repository.GetByIdAsync(id);
        if (user == null)
        {
            throw new BusinessException("User not found");
        }
        return _mapper.Map<UserListDTO>(user);
    }

    public async Task<Guid> CreateAsync(UserCreateDTO dto)
    {
        await _createValidator.ValidateAndThrowAsync(dto);
        if (await _repository.ExistsByEmailAsync(dto.Email))
        {
            throw new BusinessException("Email already exists");
        }

        var user = _mapper.Map<User>(dto);
        user.Id = Guid.NewGuid();
        user.CreatedDate = DateTime.UtcNow;
        user.IsActive = true;
        user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password); // Use BCrypt

        await _repository.CreateAsync(user);
        _logger.LogInformation("User created: {Id}", user.Id);
        return user.Id;
    }

    public async Task UpdateAsync(Guid id, UserUpdateDTO dto)
    {
        await _updateValidator.ValidateAndThrowAsync(dto);
        var user = await _repository.GetByIdAsync(id);
        if (user == null)
        {
            throw new BusinessException("User not found");
        }

        _mapper.Map(dto, user);
        user.UpdatedDate = DateTime.UtcNow;
        await _repository.UpdateAsync(user);
        _logger.LogInformation("User updated: {Id}", id);
    }

    public async Task DeleteAsync(Guid id)
    {
        var user = await _repository.GetByIdAsync(id);
        if (user == null)
        {
            throw new BusinessException("User not found");
        }

        await _repository.DeleteAsync(id);
        _logger.LogInformation("User deleted: {Id}", id);
    }

    public async Task<UserListDTO?> AuthenticateAsync(string email, string password)
    {
        var user = await _repository.GetByEmailAsync(email);
        if (user != null && BCrypt.Net.BCrypt.Verify(password, user.PasswordHash)) // Use BCrypt
        {
            return _mapper.Map<UserListDTO>(user);
        }
        return null;
    }
}