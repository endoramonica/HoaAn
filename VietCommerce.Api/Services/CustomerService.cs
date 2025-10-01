using Microsoft.Extensions.Logging;
using System.Linq.Expressions;
using VietCommerce.Api.Services.Interfaces;
using VietCommerce.Core.DTOs.Customers;
using VietCommerce.Core.Entities.Customers;
using VietCommerce.Core.Models;
using VietCommerce.Data.Repositories.Interfaces;

namespace VietCommerce.Api.Services;

public class CustomerService : ICustomerService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<CustomerService> _logger;
    private readonly IGenericServices<Customer> _genericService;

    public CustomerService(
        IUnitOfWork unitOfWork,
        ILogger<CustomerService> logger,
        IGenericServices<Customer> genericService)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
        _genericService = genericService;
    }

    // Generic CRUD operations
    public Task<IEnumerable<Customer>> GetAllAsync() => _genericService.GetAllAsync();
    public Task<Customer?> GetByIdAsync(Guid id) => _genericService.GetByIdAsync(id);
    public Task<IEnumerable<Customer>> FindAsync(Expression<Func<Customer, bool>> predicate) => _genericService.FindAsync(predicate);
    public Task<Customer> AddAsync(Customer entity) => _genericService.AddAsync(entity);
    public Task<IEnumerable<Customer>> AddRangeAsync(IEnumerable<Customer> entities) => _genericService.AddRangeAsync(entities);
    public Task<Customer> UpdateAsync(Customer entity) => _genericService.UpdateAsync(entity);
    public Task DeleteAsync(Customer entity) => _genericService.DeleteAsync(entity);
    public Task DeleteByIdAsync(Guid id) => _genericService.DeleteByIdAsync(id);
    public Task DeleteRangeAsync(IEnumerable<Customer> entities) => _genericService.DeleteRangeAsync(entities);
    public Task<bool> ExistsAsync(Expression<Func<Customer, bool>> predicate) => _genericService.ExistsAsync(predicate);
    public Task<int> CountAsync(Expression<Func<Customer, bool>> predicate) => _genericService.CountAsync(predicate);

    public async Task<ApiResponse<CustomerDetailDTO>> GetCustomerByIdAsync(Guid id)
    {
        try
        {
            var customer = await _unitOfWork.Customers.GetCustomerWithDetailsAsync(id);
            if (customer == null)
                return ApiResponse<CustomerDetailDTO>.FailureResponse("Customer not found");

            var customerDto = MapToCustomerDetailDTO(customer);
            return ApiResponse<CustomerDetailDTO>.SuccessResponse(customerDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving customer {CustomerId}", id);
            return ApiResponse<CustomerDetailDTO>.FailureResponse("Failed to retrieve customer");
        }
    }

    public async Task<ApiResponse<PaginatedResult<CustomerListDTO>>> GetCustomersAsync(int pageNumber, int pageSize, string? searchTerm = null)
    {
        try
        {
            var (customers, totalCount) = await _unitOfWork.Customers.GetCustomersPagedAsync(pageNumber, pageSize, searchTerm);

            var result = new PaginatedResult<CustomerListDTO>
            {
                Items = customers,
                TotalItems = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize)
            };

            return ApiResponse<PaginatedResult<CustomerListDTO>>.SuccessResponse(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving customers");
            return ApiResponse<PaginatedResult<CustomerListDTO>>.FailureResponse("Failed to retrieve customers");
        }
    }

    public async Task<ApiResponse<CustomerDetailDTO>> CreateCustomerAsync(CustomerCreateDTO request)
    {
        try
        {
            await _unitOfWork.BeginTransactionAsync();

            // Check if email exists if provided
            if (!string.IsNullOrEmpty(request.Email) && await _unitOfWork.Customers.EmailExistsAsync(request.Email))
            {
                await _unitOfWork.RollbackTransactionAsync();
                return ApiResponse<CustomerDetailDTO>.FailureResponse("Email already exists");
            }

            var customer = new Customer
            {
                Name = request.FullName,
                Email = request.Email?.ToLower(),
                Phone = request.PhoneNumber,
                UserId = request.UserId,
                IsActive = true,
                StoreId = Guid.NewGuid() // This should come from context or be set appropriately
            };

            await _unitOfWork.Customers.AddAsync(customer);
            await _unitOfWork.SaveChangesAsync();
            await _unitOfWork.CommitTransactionAsync();

            var createdCustomer = await _unitOfWork.Customers.GetCustomerWithDetailsAsync(customer.Id);
            var customerDto = MapToCustomerDetailDTO(createdCustomer!);

            _logger.LogInformation("Customer created successfully with ID {CustomerId}", customer.Id);
            return ApiResponse<CustomerDetailDTO>.SuccessResponse(customerDto, "Customer created successfully");
        }
        catch (Exception ex)
        {
            await _unitOfWork.RollbackTransactionAsync();
            _logger.LogError(ex, "Error creating customer");
            return ApiResponse<CustomerDetailDTO>.FailureResponse("Failed to create customer");
        }
    }

    public async Task<ApiResponse<CustomerDetailDTO>> UpdateCustomerAsync(Guid id, CustomerUpdateDTO request)
    {
        try
        {
            await _unitOfWork.BeginTransactionAsync();

            var customer = await _unitOfWork.Customers.GetByIdAsync(id);
            if (customer == null)
            {
                await _unitOfWork.RollbackTransactionAsync();
                return ApiResponse<CustomerDetailDTO>.FailureResponse("Customer not found");
            }

            // Check email uniqueness if changing
            if (!string.IsNullOrEmpty(request.Email) && request.Email != customer.Email)
            {
                if (await _unitOfWork.Customers.EmailExistsAsync(request.Email))
                {
                    await _unitOfWork.RollbackTransactionAsync();
                    return ApiResponse<CustomerDetailDTO>.FailureResponse("Email already exists");
                }
                customer.Email = request.Email.ToLower();
            }

            if (!string.IsNullOrEmpty(request.FullName))
                customer.Name = request.FullName;

            if (!string.IsNullOrEmpty(request.PhoneNumber))
                customer.Phone = request.PhoneNumber;

            _unitOfWork.Customers.Update(customer);
            await _unitOfWork.SaveChangesAsync();
            await _unitOfWork.CommitTransactionAsync();

            var updatedCustomer = await _unitOfWork.Customers.GetCustomerWithDetailsAsync(id);
            var customerDto = MapToCustomerDetailDTO(updatedCustomer!);

            _logger.LogInformation("Customer {CustomerId} updated successfully", id);
            return ApiResponse<CustomerDetailDTO>.SuccessResponse(customerDto, "Customer updated successfully");
        }
        catch (Exception ex)
        {
            await _unitOfWork.RollbackTransactionAsync();
            _logger.LogError(ex, "Error updating customer {CustomerId}", id);
            return ApiResponse<CustomerDetailDTO>.FailureResponse("Failed to update customer");
        }
    }

    public async Task<ApiResponse<bool>> DeleteCustomerAsync(Guid id)
    {
        try
        {
            await _unitOfWork.BeginTransactionAsync();

            var customer = await _unitOfWork.Customers.GetByIdAsync(id);
            if (customer == null)
            {
                await _unitOfWork.RollbackTransactionAsync();
                return ApiResponse<bool>.FailureResponse("Customer not found");
            }

            // Soft delete
            customer.IsDeleted = true;
            customer.DeletedAt = DateTime.UtcNow;
            // customer.DeletedBy should be set from current user context

            _unitOfWork.Customers.Update(customer);
            await _unitOfWork.SaveChangesAsync();
            await _unitOfWork.CommitTransactionAsync();

            _logger.LogInformation("Customer {CustomerId} deleted successfully", id);
            return ApiResponse<bool>.SuccessResponse(true, "Customer deleted successfully");
        }
        catch (Exception ex)
        {
            await _unitOfWork.RollbackTransactionAsync();
            _logger.LogError(ex, "Error deleting customer {CustomerId}", id);
            return ApiResponse<bool>.FailureResponse("Failed to delete customer");
        }
    }

    public async Task<ApiResponse<CustomerDetailDTO>> GetCustomerByUserIdAsync(Guid userId)
    {
        try
        {
            var customer = await _unitOfWork.Customers.GetCustomerByUserIdAsync(userId);
            if (customer == null)
                return ApiResponse<CustomerDetailDTO>.FailureResponse("Customer not found for this user");

            var customerDto = MapToCustomerDetailDTO(customer);
            return ApiResponse<CustomerDetailDTO>.SuccessResponse(customerDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving customer for user {UserId}", userId);
            return ApiResponse<CustomerDetailDTO>.FailureResponse("Failed to retrieve customer");
        }
    }

    public async Task<ApiResponse<bool>> LinkCustomerToUserAsync(Guid customerId, Guid userId)
    {
        try
        {
            var customer = await _unitOfWork.Customers.GetByIdAsync(customerId);
            if (customer == null)
                return ApiResponse<bool>.FailureResponse("Customer not found");

            var user = await _unitOfWork.Users.GetByIdAsync(userId);
            if (user == null)
                return ApiResponse<bool>.FailureResponse("User not found");

            customer.UserId = userId;
            _unitOfWork.Customers.Update(customer);
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("Customer {CustomerId} linked to user {UserId}", customerId, userId);
            return ApiResponse<bool>.SuccessResponse(true, "Customer linked to user successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error linking customer {CustomerId} to user {UserId}", customerId, userId);
            return ApiResponse<bool>.FailureResponse("Failed to link customer to user");
        }
    }

    public async Task<ApiResponse<bool>> UnlinkCustomerFromUserAsync(Guid customerId)
    {
        try
        {
            var customer = await _unitOfWork.Customers.GetByIdAsync(customerId);
            if (customer == null)
                return ApiResponse<bool>.FailureResponse("Customer not found");

            customer.UserId = null;
            _unitOfWork.Customers.Update(customer);
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("Customer {CustomerId} unlinked from user", customerId);
            return ApiResponse<bool>.SuccessResponse(true, "Customer unlinked from user successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error unlinking customer {CustomerId} from user", customerId);
            return ApiResponse<bool>.FailureResponse("Failed to unlink customer from user");
        }
    }

    private static CustomerDetailDTO MapToCustomerDetailDTO(Customer customer)
    {
        return new CustomerDetailDTO
        {
            Id = customer.Id,
            FullName = customer.Name ?? "",
            Email = customer.Email,
            PhoneNumber = customer.Phone,
            UserId = customer.UserId,
            CreatedDate = customer.CreatedAt,
            UpdatedDate = customer.UpdatedAt,
            IsActive = customer.IsActive,
            LoyaltyPoints = customer.LoyaltyPoints,
            Tier = customer.Tier
        };
    }
}
