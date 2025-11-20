using AutoMapper;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VietCommerce.Application.Services.Services.Interfaces;
using VietCommerce.Application.Services.Services.Interfaces.Identities;
using VietCommerce.Core.DTOs.Address;
using VietCommerce.Core.DTOs.CRM;
using VietCommerce.Core.DTOs.Customers;
using VietCommerce.Core.DTOs.Orders;
using VietCommerce.Core.Entities.CRM;
using VietCommerce.Core.Entities.Customers;
using VietCommerce.Core.Models;
using VietCommerce.Data.Repositories.Interfaces;

namespace VietCommerce.Application.Services.Services;

public class CustomerService : BaseService, ICustomerService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ICurrentUser _currentUser;

    // Định nghĩa hằng số permission để tránh magic string
    private const string PERM_VIEW = "customer.view";
    private const string PERM_MANAGE = "customer.manage";

    public CustomerService(
        IUnitOfWork unitOfWork,
        IMapper mapper,
        ICurrentUser currentUser,
        ILogger<CustomerService> logger,
        ICacheService cacheService)
        : base(logger, cacheService)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _currentUser = currentUser;
    }

    // Helper check quyền nhanh
    private async Task CheckPermission(string permission)
    {
        if (!await _currentUser.HasPermissionAsync(permission))
        {
            throw new UnauthorizedAccessException($"Bạn không có quyền thực hiện hành động này. Yêu cầu quyền: {permission}");
        }
    }

    #region Customer Management

    public async Task<ApiResponse<PaginatedResult<CustomerListDto>>> GetCustomersAsync(
        PaginationParams pagination,
        CustomerFilters filters)
    {
        return await ExecuteAsApiResponseAsync(async () =>
        {
            await CheckPermission(PERM_VIEW); // ✅ Permission Check

            ValidateNotNull(pagination, nameof(pagination));
            ValidateNotNull(filters, nameof(filters));

            if (!filters.IsValid(out var errorMessage))
                throw new ArgumentException(errorMessage);

            // Cache key bao gồm cả tham số filter để tránh data sai
            var cacheKey = CreateCacheKey("customers", "list", pagination.Page, pagination.PageSize, filters.GetHashCode());

            return await GetFromCacheOrExecuteAsync(cacheKey, async () =>
            {
                var query = (await _unitOfWork.Customers.GetAllAsync()).AsQueryable();
                query = ApplyCustomerFilters(query, filters);
                var totalItems = query.Count();
                query = ApplySorting(query, pagination);

                var items = query
                    .Skip(pagination.Skip)
                    .Take(pagination.PageSize)
                    .ToList();

                var dtos = _mapper.Map<List<CustomerListDto>>(items);

                foreach (var dto in dtos)
                {
                    var orders = await _unitOfWork.Orders.GetByCustomerIdAsync(dto.Id);
                    dto.TotalOrders = orders.Count();
                    dto.TotalSpent = orders.Sum(o => o.TotalAmount);
                }

                return new PaginatedResult<CustomerListDto>(
                    dtos,
                    pagination.Page,
                    pagination.PageSize,
                    totalItems
                );
            }, TimeSpan.FromMinutes(5));

        }, "GetCustomersAsync");
    }

    public async Task<ApiResponse<CustomerDetailDto>> GetCustomerByIdAsync(Guid id)
    {
        return await ExecuteAsApiResponseAsync(async () =>
        {
            await CheckPermission(PERM_VIEW); // ✅ Permission Check
            ValidateId(id, nameof(id));

            var cacheKey = CreateCacheKey("customer", id);

            return await GetFromCacheOrExecuteAsync(cacheKey, async () =>
            {
                var customer = await _unitOfWork.Customers.GetByIdAsync(id);
                if (customer == null)
                    throw new KeyNotFoundException($"Customer with ID {id} not found");

                var dto = _mapper.Map<CustomerDetailDto>(customer);

                var orders = await _unitOfWork.Orders.GetByCustomerIdAsync(id);
                dto.TotalOrders = orders.Count();
                dto.TotalSpent = orders.Sum(o => o.TotalAmount);

                var addresses = await _unitOfWork.CustomerAddresses.GetByCustomerIdAsync(id);
                dto.Addresses = _mapper.Map<List<CustomerAddressDto>>(addresses);

                return dto;
            });

        }, "GetCustomerByIdAsync");
    }

    public async Task<ApiResponse<CustomerDetailDto>> CreateCustomerAsync(CreateCustomerRequest request)
    {
        return await ExecuteAsApiResponseAsync(async () =>
        {
            await CheckPermission(PERM_MANAGE); // ✅ Permission Check
            ValidateNotNull(request, nameof(request));
            ValidateNotEmpty(request.Name, nameof(request.Name));

            if (!string.IsNullOrEmpty(request.Email))
            {
                var emailExists = await _unitOfWork.Customers.EmailExistsAsync(request.Email);
                if (emailExists)
                    throw new InvalidOperationException($"Email {request.Email} already exists");
            }

            var customer = _mapper.Map<Customer>(request);
            // customer.CreatedBy = _currentUser.UserId; // Uncomment nếu Entity có BaseTracking

            await _unitOfWork.Customers.AddAsync(customer);
            await _unitOfWork.SaveChangesAsync();

            await InvalidateCacheByPrefixAsync("customers:list");

            LogInfo($"✅ Created customer: {customer.Id} - {customer.Name} by User: {_currentUser.UserName}");

            return _mapper.Map<CustomerDetailDto>(customer);
        }, "CreateCustomerAsync", "Customer created successfully");
    }

    public async Task<ApiResponse<CustomerDetailDto>> UpdateCustomerAsync(
        Guid id,
        UpdateCustomerRequest request)
    {
        return await ExecuteAsApiResponseAsync(async () =>
        {
            await CheckPermission(PERM_MANAGE); // ✅ Permission Check
            ValidateId(id, nameof(id));
            ValidateNotNull(request, nameof(request));

            var customer = await _unitOfWork.Customers.GetByIdAsync(id);
            if (customer == null)
                throw new KeyNotFoundException($"Customer with ID {id} not found");

            if (!string.IsNullOrEmpty(request.Email) && request.Email != customer.Email)
            {
                var emailExists = await _unitOfWork.Customers.EmailExistsAsync(request.Email);
                if (emailExists)
                    throw new InvalidOperationException($"Email {request.Email} already exists");
            }

            _mapper.Map(request, customer);
            customer.UpdatedAt = DateTime.UtcNow;
            // customer.UpdatedBy = _currentUser.UserId;

            _unitOfWork.Customers.Update(customer);
            await _unitOfWork.SaveChangesAsync();

            await InvalidateMultipleCachesAsync(
                CreateCacheKey("customer", id),
                "customers:list" // Xóa cache list vì thông tin cơ bản có thể thay đổi
            );

            LogInfo($"✅ Updated customer: {id}");

            return _mapper.Map<CustomerDetailDto>(customer);
        }, "UpdateCustomerAsync", "Customer updated successfully");
    }

    public async Task<ApiResponse<bool>> DeleteCustomerAsync(Guid id)
    {
        return await ExecuteAsApiResponseAsync(async () =>
        {
            await CheckPermission(PERM_MANAGE); // ✅ Permission Check
            ValidateId(id, nameof(id));

            var customer = await _unitOfWork.Customers.GetByIdAsync(id);
            if (customer == null)
                throw new KeyNotFoundException($"Customer with ID {id} not found");

            customer.IsDeleted = true;
            customer.DeletedAt = DateTime.UtcNow;
            customer.DeletedBy = _currentUser.UserId;

            _unitOfWork.Customers.Update(customer);
            await _unitOfWork.SaveChangesAsync();

            await InvalidateMultipleCachesAsync(
                CreateCacheKey("customer", id),
                "customers:list"
            );

            LogInfo($"🗑️ Soft deleted customer: {id}");

            return true;
        }, "DeleteCustomerAsync", "Customer deleted successfully");
    }

    public async Task<ApiResponse<List<CustomerListDto>>> SearchCustomersAsync(string searchTerm)
    {
        return await ExecuteAsApiResponseAsync(async () =>
        {
            await CheckPermission(PERM_VIEW); // ✅ Permission Check
            ValidateNotEmpty(searchTerm, nameof(searchTerm));

            var customers = await _unitOfWork.Customers.FindAsync(c =>
                (c.Name != null && c.Name.Contains(searchTerm)) ||
                (c.Email != null && c.Email.Contains(searchTerm)) ||
                (c.Phone != null && c.Phone.Contains(searchTerm))
            );

            var dtos = _mapper.Map<List<CustomerListDto>>(customers);

            foreach (var dto in dtos)
            {
                var orders = await _unitOfWork.Orders.GetByCustomerIdAsync(dto.Id);
                dto.TotalOrders = orders.Count();
                dto.TotalSpent = orders.Sum(o => o.TotalAmount);
            }

            return dtos;
        }, "SearchCustomersAsync");
    }

    public async Task<ApiResponse<CustomerStatisticsDto>> GetCustomerStatisticsAsync(Guid customerId)
    {
        return await ExecuteAsApiResponseAsync(async () =>
        {
            await CheckPermission(PERM_VIEW); // ✅ Permission Check
            ValidateId(customerId, nameof(customerId));

            var customer = await _unitOfWork.Customers.GetByIdAsync(customerId);
            if (customer == null)
                throw new KeyNotFoundException($"Customer with ID {customerId} not found");

            var orders = (await _unitOfWork.Orders.GetByCustomerIdAsync(customerId)).ToList();

            return new CustomerStatisticsDto
            {
                CustomerId = customerId,
                TotalOrders = orders.Count,
                TotalSpent = orders.Sum(o => o.TotalAmount),
                AverageOrderValue = orders.Any() ? orders.Average(o => o.TotalAmount) : 0,
                LastOrderDate = orders.Any() ? orders.Max(o => o.CreatedAt) : null,
                LoyaltyPoints = customer.LoyaltyPoints,
                Tier = customer.Tier
            };
        }, "GetCustomerStatisticsAsync");
    }

    #endregion

    #region Customer Orders

    public async Task<ApiResponse<PaginatedResult<OrderDetailDto>>> GetCustomerOrdersAsync(
        Guid customerId,
        PaginationParams pagination)
    {
        return await ExecuteAsApiResponseAsync(async () =>
        {
            await CheckPermission(PERM_VIEW); // ✅ Permission Check
            ValidateId(customerId, nameof(customerId));
            ValidateNotNull(pagination, nameof(pagination));

            var customer = await _unitOfWork.Customers.GetByIdAsync(customerId);
            if (customer == null)
                throw new KeyNotFoundException($"Customer with ID {customerId} not found");

            var allOrders = (await _unitOfWork.Orders.GetByCustomerIdAsync(customerId)).ToList();
            var totalItems = allOrders.Count;

            var orders = allOrders
                .OrderByDescending(o => o.CreatedAt)
                .Skip(pagination.Skip)
                .Take(pagination.PageSize)
                .ToList();

            var dtos = _mapper.Map<List<OrderDetailDto>>(orders);

            return new PaginatedResult<OrderDetailDto>(
                dtos,
                pagination.Page,
                pagination.PageSize,
                totalItems
            );
        }, "GetCustomerOrdersAsync");
    }

    public async Task<ApiResponse<CustomerOrderSummaryDto>> GetCustomerOrderSummaryAsync(Guid customerId)
    {
        return await ExecuteAsApiResponseAsync(async () =>
        {
            await CheckPermission(PERM_VIEW); // ✅ Permission Check
            ValidateId(customerId, nameof(customerId));

            var orders = (await _unitOfWork.Orders.GetByCustomerIdAsync(customerId)).ToList();

            return new CustomerOrderSummaryDto
            {
                CustomerId = customerId,
                TotalOrders = orders.Count,
                TotalSpent = orders.Sum(o => o.TotalAmount),
                AverageOrderValue = orders.Any() ? orders.Average(o => o.TotalAmount) : 0,
                FirstOrderDate = orders.Any() ? orders.Min(o => o.CreatedAt) : null,
                LastOrderDate = orders.Any() ? orders.Max(o => o.CreatedAt) : null
            };
        }, "GetCustomerOrderSummaryAsync");
    }

    #endregion

    #region Customer Addresses

    public async Task<ApiResponse<List<CustomerAddressDto>>> GetCustomerAddressesAsync(Guid customerId)
    {
        return await ExecuteAsApiResponseAsync(async () =>
        {
            await CheckPermission(PERM_VIEW); // ✅ Permission Check
            ValidateId(customerId, nameof(customerId));

            var addresses = await _unitOfWork.CustomerAddresses.GetByCustomerIdAsync(customerId);
            return _mapper.Map<List<CustomerAddressDto>>(addresses);
        }, "GetCustomerAddressesAsync");
    }

    public async Task<ApiResponse<CustomerAddressDto>> GetCustomerAddressByIdAsync(Guid addressId)
    {
        return await ExecuteAsApiResponseAsync(async () =>
        {
            await CheckPermission(PERM_VIEW); // ✅ Permission Check
            ValidateId(addressId, nameof(addressId));

            var address = await _unitOfWork.CustomerAddresses.GetByIdAsync(addressId);
            if (address == null)
                throw new KeyNotFoundException($"Address with ID {addressId} not found");

            return _mapper.Map<CustomerAddressDto>(address);
        }, "GetCustomerAddressByIdAsync");
    }

    public async Task<ApiResponse<CustomerAddressDto>> CreateCustomerAddressAsync(
        CreateCustomerAddressRequest request)
    {
        return await ExecuteAsApiResponseAsync(async () =>
        {
            await CheckPermission(PERM_MANAGE); // ✅ Permission Check - Thay đổi địa chỉ là Manage
            ValidateNotNull(request, nameof(request));
            ValidateId(request.CustomerId, nameof(request.CustomerId));

            var customer = await _unitOfWork.Customers.GetByIdAsync(request.CustomerId);
            if (customer == null)
                throw new KeyNotFoundException($"Customer with ID {request.CustomerId} not found");

            if (request.IsDefault)
            {
                await _unitOfWork.CustomerAddresses.ClearDefaultAsync(request.CustomerId);
            }

            var address = _mapper.Map<CustomerAddress>(request);
            await _unitOfWork.CustomerAddresses.AddAsync(address);
            await _unitOfWork.SaveChangesAsync();

            // Invalidate Customer Cache vì địa chỉ thay đổi
            await InvalidateCacheAsync(CreateCacheKey("customer", request.CustomerId));

            LogInfo($"✅ Created address for customer: {request.CustomerId}");

            return _mapper.Map<CustomerAddressDto>(address);
        }, "CreateCustomerAddressAsync", "Address created successfully");
    }

    public async Task<ApiResponse<CustomerAddressDto>> UpdateCustomerAddressAsync(
        Guid addressId,
        UpdateCustomerAddressRequest request)
    {
        return await ExecuteAsApiResponseAsync(async () =>
        {
            await CheckPermission(PERM_MANAGE); // ✅ Permission Check
            ValidateId(addressId, nameof(addressId));
            ValidateNotNull(request, nameof(request));

            var address = await _unitOfWork.CustomerAddresses.GetByIdAsync(addressId);
            if (address == null)
                throw new KeyNotFoundException($"Address with ID {addressId} not found");

            if (request.IsDefault == true)
            {
                await _unitOfWork.CustomerAddresses.ClearDefaultAsync(address.CustomerId);
            }

            _mapper.Map(request, address);
            address.UpdatedAt = DateTime.UtcNow;

            _unitOfWork.CustomerAddresses.Update(address);
            await _unitOfWork.SaveChangesAsync();

            await InvalidateCacheAsync(CreateCacheKey("customer", address.CustomerId));

            LogInfo($"✅ Updated address: {addressId}");

            return _mapper.Map<CustomerAddressDto>(address);
        }, "UpdateCustomerAddressAsync", "Address updated successfully");
    }

    public async Task<ApiResponse<bool>> DeleteCustomerAddressAsync(Guid addressId)
    {
        return await ExecuteAsApiResponseAsync(async () =>
        {
            await CheckPermission(PERM_MANAGE); // ✅ Permission Check
            ValidateId(addressId, nameof(addressId));

            var address = await _unitOfWork.CustomerAddresses.GetByIdAsync(addressId);
            if (address == null)
                throw new KeyNotFoundException($"Address with ID {addressId} not found");

            address.IsDeleted = true;
            address.DeletedAt = DateTime.UtcNow;
            address.DeletedBy = _currentUser.UserId;

            _unitOfWork.CustomerAddresses.Update(address);
            await _unitOfWork.SaveChangesAsync();

            await InvalidateCacheAsync(CreateCacheKey("customer", address.CustomerId));

            LogInfo($"🗑️ Deleted address: {addressId}");

            return true;
        }, "DeleteCustomerAddressAsync", "Address deleted successfully");
    }

    public async Task<ApiResponse<bool>> SetDefaultAddressAsync(Guid customerId, Guid addressId)
    {
        return await ExecuteAsApiResponseAsync(async () =>
        {
            await CheckPermission(PERM_MANAGE); // ✅ Permission Check
            ValidateId(customerId, nameof(customerId));
            ValidateId(addressId, nameof(addressId));

            var address = await _unitOfWork.CustomerAddresses.GetByIdAndCustomerIdAsync(addressId, customerId);
            if (address == null)
                throw new KeyNotFoundException($"Address not found or doesn't belong to customer");

            await _unitOfWork.CustomerAddresses.ClearDefaultAsync(customerId);

            address.IsDefault = true;
            address.UpdatedAt = DateTime.UtcNow;

            _unitOfWork.CustomerAddresses.Update(address);
            await _unitOfWork.SaveChangesAsync();

            await InvalidateCacheAsync(CreateCacheKey("customer", customerId));

            LogInfo($"✅ Set default address: {addressId} for customer: {customerId}");

            return true;
        }, "SetDefaultAddressAsync", "Default address set successfully");
    }

    #endregion

    #region CRM Interactions

    public async Task<ApiResponse<PaginatedResult<CRMInteractionListDto>>> GetCustomerInteractionsAsync(
        Guid customerId,
        PaginationParams pagination,
        CRMInteractionFilters? filters = null)
    {
        return await ExecuteAsApiResponseAsync(async () =>
        {
            await CheckPermission(PERM_VIEW); // ✅ Permission Check
            ValidateId(customerId, nameof(customerId));
            ValidateNotNull(pagination, nameof(pagination));

            var customer = await _unitOfWork.Customers.GetByIdAsync(customerId);
            if (customer == null)
                throw new KeyNotFoundException($"Customer with ID {customerId} not found");

            var allInteractions = await _unitOfWork.CRMInteractions.GetByCustomerIdAsync(customerId);
            var query = allInteractions.AsQueryable();

            if (filters != null)
            {
                if (!filters.IsValid(out var errorMessage))
                    throw new ArgumentException(errorMessage);
                query = ApplyCRMInteractionFilters(query, filters);
            }

            var totalItems = query.Count();
            query = query.OrderByDescending(i => i.CreatedAt);

            var items = query
                .Skip(pagination.Skip)
                .Take(pagination.PageSize)
                .ToList();

            var dtos = _mapper.Map<List<CRMInteractionListDto>>(items);

            return new PaginatedResult<CRMInteractionListDto>(
                dtos,
                pagination.Page,
                pagination.PageSize,
                totalItems
            );
        }, "GetCustomerInteractionsAsync");
    }

    public async Task<ApiResponse<CRMInteractionDto>> GetInteractionByIdAsync(Guid interactionId)
    {
        return await ExecuteAsApiResponseAsync(async () =>
        {
            await CheckPermission(PERM_VIEW); // ✅ Permission Check
            ValidateId(interactionId, nameof(interactionId));

            var interaction = await _unitOfWork.CRMInteractions.GetByIdAsync(interactionId);
            if (interaction == null)
                throw new KeyNotFoundException($"Interaction with ID {interactionId} not found");

            return _mapper.Map<CRMInteractionDto>(interaction);
        }, "GetInteractionByIdAsync");
    }

    public async Task<ApiResponse<CRMInteractionDto>> CreateCustomerInteractionAsync(
        CreateInteractionRequest request)
    {
        return await ExecuteAsApiResponseAsync(async () =>
        {
            await CheckPermission(PERM_MANAGE); // ✅ Permission Check (CRM ghi chép cũng là manage)
            ValidateNotNull(request, nameof(request));
            ValidateId(request.CustomerId, nameof(request.CustomerId));
            ValidateNotEmpty(request.Title, nameof(request.Title));
            ValidateNotEmpty(request.Description, nameof(request.Description));

            var customer = await _unitOfWork.Customers.GetByIdAsync(request.CustomerId);
            if (customer == null)
                throw new KeyNotFoundException($"Customer with ID {request.CustomerId} not found");

            var interaction = _mapper.Map<CRMInteraction>(request);
            interaction.CreatedBy = _currentUser.UserId;
            interaction.UpdatedBy = _currentUser.UserId;

            await _unitOfWork.CRMInteractions.AddAsync(interaction);
            await _unitOfWork.SaveChangesAsync();

            await InvalidateCacheByPrefixAsync($"crm:customer:{request.CustomerId}");

            LogInfo($"✅ Created CRM interaction: {interaction.Id} for customer: {request.CustomerId}");

            var created = await _unitOfWork.CRMInteractions.GetByIdAsync(interaction.Id);
            return _mapper.Map<CRMInteractionDto>(created);
        }, "CreateCustomerInteractionAsync", "Interaction created successfully");
    }

    public async Task<ApiResponse<CRMInteractionDto>> UpdateCustomerInteractionAsync(
        Guid interactionId,
        UpdateInteractionRequest request)
    {
        return await ExecuteAsApiResponseAsync(async () =>
        {
            await CheckPermission(PERM_MANAGE); // ✅ Permission Check
            ValidateId(interactionId, nameof(interactionId));
            ValidateNotNull(request, nameof(request));

            var interaction = await _unitOfWork.CRMInteractions.GetByIdAsync(interactionId);
            if (interaction == null)
                throw new KeyNotFoundException($"Interaction with ID {interactionId} not found");

            _mapper.Map(request, interaction);
            interaction.UpdatedBy = _currentUser.UserId;
            interaction.UpdatedAt = DateTime.UtcNow;

            _unitOfWork.CRMInteractions.Update(interaction);
            await _unitOfWork.SaveChangesAsync();

            await InvalidateMultipleCachesAsync(
                CreateCacheKey("crm:interaction", interactionId),
                CreateCacheKey("crm:customer", interaction.CustomerId)
            );

            LogInfo($"✅ Updated CRM interaction: {interactionId}");

            var updated = await _unitOfWork.CRMInteractions.GetByIdAsync(interactionId);
            return _mapper.Map<CRMInteractionDto>(updated);
        }, "UpdateCustomerInteractionAsync", "Interaction updated successfully");
    }

    public async Task<ApiResponse<bool>> DeleteCustomerInteractionAsync(Guid interactionId)
    {
        return await ExecuteAsApiResponseAsync(async () =>
        {
            await CheckPermission(PERM_MANAGE); // ✅ Permission Check
            ValidateId(interactionId, nameof(interactionId));

            var interaction = await _unitOfWork.CRMInteractions.GetByIdAsync(interactionId);
            if (interaction == null)
                throw new KeyNotFoundException($"Interaction with ID {interactionId} not found");

            _unitOfWork.CRMInteractions.Delete(interaction);
            await _unitOfWork.SaveChangesAsync();

            await InvalidateMultipleCachesAsync(
                CreateCacheKey("crm:interaction", interactionId),
                CreateCacheKey("crm:customer", interaction.CustomerId)
            );

            LogInfo($"🗑️ Deleted CRM interaction: {interactionId}");

            return true;
        }, "DeleteCustomerInteractionAsync", "Interaction deleted successfully");
    }

    public async Task<ApiResponse<List<CRMInteractionListDto>>> GetUpcomingFollowUpsAsync(
        DateTime? fromDate = null,
        DateTime? toDate = null)
    {
        return await ExecuteAsApiResponseAsync(async () =>
        {
            await CheckPermission(PERM_VIEW); // ✅ Permission Check

            var from = fromDate ?? DateTime.UtcNow.Date;
            var to = toDate ?? DateTime.UtcNow.Date.AddDays(30);

            var interactions = await _unitOfWork.CRMInteractions.GetUpcomingFollowUpsAsync(from, to);
            var dtos = _mapper.Map<List<CRMInteractionListDto>>(interactions);

            LogInfo($"📅 Retrieved {dtos.Count} upcoming follow-ups");

            return dtos;
        }, "GetUpcomingFollowUpsAsync");
    }

    public async Task<ApiResponse<CRMInteractionDto>> CompleteInteractionAsync(Guid interactionId)
    {
        return await ExecuteAsApiResponseAsync(async () =>
        {
            await CheckPermission(PERM_MANAGE); // ✅ Permission Check
            ValidateId(interactionId, nameof(interactionId));

            var interaction = await _unitOfWork.CRMInteractions.GetByIdAsync(interactionId);
            if (interaction == null)
                throw new KeyNotFoundException($"Interaction with ID {interactionId} not found");

            ThrowIf(interaction.Status == CRMInteractionStatus.Completed, "Interaction is already completed");

            interaction.Status = CRMInteractionStatus.Completed;
            interaction.UpdatedBy = _currentUser.UserId;
            interaction.UpdatedAt = DateTime.UtcNow;

            _unitOfWork.CRMInteractions.Update(interaction);
            await _unitOfWork.SaveChangesAsync();

            await InvalidateMultipleCachesAsync(
                CreateCacheKey("crm:interaction", interactionId),
                CreateCacheKey("crm:customer", interaction.CustomerId)
            );

            LogInfo($"✅ Completed CRM interaction: {interactionId}");

            var completed = await _unitOfWork.CRMInteractions.GetByIdAsync(interactionId);
            return _mapper.Map<CRMInteractionDto>(completed);
        }, "CompleteInteractionAsync", "Interaction marked as completed");
    }

    #endregion

    #region CRM Private Helper

    private IQueryable<CRMInteraction> ApplyCRMInteractionFilters(
        IQueryable<CRMInteraction> query,
        CRMInteractionFilters filters)
    {
        if (filters.Type.HasValue)
            query = query.Where(i => i.Type == filters.Type.Value);

        if (filters.Status.HasValue)
            query = query.Where(i => i.Status == filters.Status.Value);

        if (!string.IsNullOrEmpty(filters.SearchTerm))
        {
            var searchTerm = filters.SearchTerm.ToLower();
            query = query.Where(i =>
                i.Title.ToLower().Contains(searchTerm) ||
                i.Description.ToLower().Contains(searchTerm)
            );
        }

        if (filters.FromDate.HasValue)
            query = query.Where(i => i.CreatedAt >= filters.FromDate.Value);

        if (filters.ToDate.HasValue)
            query = query.Where(i => i.CreatedAt <= filters.ToDate.Value);

        if (filters.FollowUpFromDate.HasValue)
            query = query.Where(i => i.FollowUpDate.HasValue && i.FollowUpDate >= filters.FollowUpFromDate.Value);

        if (filters.FollowUpToDate.HasValue)
            query = query.Where(i => i.FollowUpDate.HasValue && i.FollowUpDate <= filters.FollowUpToDate.Value);

        if (filters.CreatedBy.HasValue)
            query = query.Where(i => i.CreatedBy == filters.CreatedBy.Value);

        if (filters.HasPendingFollowUp.HasValue && filters.HasPendingFollowUp.Value)
        {
            var today = DateTime.UtcNow.Date;
            query = query.Where(i =>
                i.Status == CRMInteractionStatus.Pending &&
                i.FollowUpDate.HasValue &&
                i.FollowUpDate.Value.Date >= today
            );
        }

        return query;
    }

    #endregion

    #region Customer Loyalty

    public async Task<ApiResponse<CustomerDetailDto>> AddLoyaltyPointsAsync(
        Guid customerId,
        int points,
        string reason)
    {
        return await ExecuteAsApiResponseAsync(async () =>
        {
            await CheckPermission(PERM_MANAGE); // ✅ Permission Check
            ValidateId(customerId, nameof(customerId));
            ThrowIf(points <= 0, "Points must be positive");
            ValidateNotEmpty(reason, nameof(reason));

            var customer = await _unitOfWork.Customers.GetByIdAsync(customerId);
            if (customer == null)
                throw new KeyNotFoundException($"Customer with ID {customerId} not found");

            customer.LoyaltyPoints += points;
            customer.UpdatedAt = DateTime.UtcNow;

            _unitOfWork.Customers.Update(customer);
            await _unitOfWork.SaveChangesAsync();

            LogInfo($"✅ Added {points} loyalty points to customer {customerId}. Reason: {reason}");

            // Xóa cache để cập nhật điểm
            await InvalidateCacheAsync(CreateCacheKey("customer", customerId));

            return _mapper.Map<CustomerDetailDto>(customer);
        }, "AddLoyaltyPointsAsync", "Loyalty points added successfully");
    }

    public async Task<ApiResponse<CustomerDetailDto>> DeductLoyaltyPointsAsync(
        Guid customerId,
        int points,
        string reason)
    {
        return await ExecuteAsApiResponseAsync(async () =>
        {
            await CheckPermission(PERM_MANAGE); // ✅ Permission Check
            ValidateId(customerId, nameof(customerId));
            ThrowIf(points <= 0, "Points must be positive");
            ValidateNotEmpty(reason, nameof(reason));

            var customer = await _unitOfWork.Customers.GetByIdAsync(customerId);
            if (customer == null)
                throw new KeyNotFoundException($"Customer with ID {customerId} not found");

            ThrowIf(customer.LoyaltyPoints < points, "Insufficient loyalty points");

            customer.LoyaltyPoints -= points;
            customer.UpdatedAt = DateTime.UtcNow;

            _unitOfWork.Customers.Update(customer);
            await _unitOfWork.SaveChangesAsync();

            LogInfo($"✅ Deducted {points} loyalty points from customer {customerId}. Reason: {reason}");

            await InvalidateCacheAsync(CreateCacheKey("customer", customerId));

            return _mapper.Map<CustomerDetailDto>(customer);
        }, "DeductLoyaltyPointsAsync", "Loyalty points deducted successfully");
    }

    public async Task<ApiResponse<CustomerDetailDto>> UpdateCustomerTierAsync(Guid customerId, string tier)
    {
        return await ExecuteAsApiResponseAsync(async () =>
        {
            await CheckPermission(PERM_MANAGE); // ✅ Permission Check
            ValidateId(customerId, nameof(customerId));
            ValidateNotEmpty(tier, nameof(tier));

            var customer = await _unitOfWork.Customers.GetByIdAsync(customerId);
            if (customer == null)
                throw new KeyNotFoundException($"Customer with ID {customerId} not found");

            customer.Tier = tier;
            customer.UpdatedAt = DateTime.UtcNow;

            _unitOfWork.Customers.Update(customer);
            await _unitOfWork.SaveChangesAsync();

            LogInfo($"✅ Updated tier to {tier} for customer {customerId}");

            await InvalidateCacheAsync(CreateCacheKey("customer", customerId));

            return _mapper.Map<CustomerDetailDto>(customer);
        }, "UpdateCustomerTierAsync", "Customer tier updated successfully");
    }

    public async Task<ApiResponse<List<LoyaltyHistoryDto>>> GetLoyaltyHistoryAsync(Guid customerId)
    {
        return await ExecuteAsApiResponseAsync(async () =>
        {
            await CheckPermission(PERM_VIEW); // ✅ Permission Check
            ValidateId(customerId, nameof(customerId));

            // TODO: Implement when LoyaltyHistory entity/repository is available
            LogWarning($"⚠️ Loyalty history not yet implemented for customer {customerId}");
            return new List<LoyaltyHistoryDto>();
        }, "GetLoyaltyHistoryAsync");
    }

    #endregion

    #region Private Helper Methods

    private IQueryable<Customer> ApplyCustomerFilters(IQueryable<Customer> query, CustomerFilters filters)
    {
        if (!string.IsNullOrEmpty(filters.SearchTerm))
        {
            var searchTerm = filters.SearchTerm.ToLower();
            query = query.Where(c =>
                (c.Name != null && c.Name.ToLower().Contains(searchTerm)) ||
                (c.Email != null && c.Email.ToLower().Contains(searchTerm)) ||
                (c.Phone != null && c.Phone.ToLower().Contains(searchTerm))
            );
        }

        if (!string.IsNullOrEmpty(filters.Tier))
            query = query.Where(c => c.Tier == filters.Tier);

        if (filters.IsActive.HasValue)
            query = query.Where(c => c.IsActive == filters.IsActive.Value);

        if (filters.FromDate.HasValue)
            query = query.Where(c => c.CreatedAt >= filters.FromDate.Value);

        if (filters.ToDate.HasValue)
            query = query.Where(c => c.CreatedAt <= filters.ToDate.Value);

        if (filters.MinLoyaltyPoints.HasValue)
            query = query.Where(c => c.LoyaltyPoints >= filters.MinLoyaltyPoints.Value);

        if (filters.MaxLoyaltyPoints.HasValue)
            query = query.Where(c => c.LoyaltyPoints <= filters.MaxLoyaltyPoints.Value);

        if (filters.HasEmail.HasValue)
        {
            if (filters.HasEmail.Value)
                query = query.Where(c => !string.IsNullOrEmpty(c.Email));
            else
                query = query.Where(c => string.IsNullOrEmpty(c.Email));
        }

        if (filters.HasPhone.HasValue)
        {
            if (filters.HasPhone.Value)
                query = query.Where(c => !string.IsNullOrEmpty(c.Phone));
            else
                query = query.Where(c => string.IsNullOrEmpty(c.Phone));
        }

        query = query.Where(c => !c.IsDeleted);
        return query;
    }

    private IQueryable<Customer> ApplySorting(IQueryable<Customer> query, PaginationParams pagination)
    {
        return pagination.SortBy.ToLower() switch
        {
            "name" => pagination.SortDescending
                ? query.OrderByDescending(c => c.Name)
                : query.OrderBy(c => c.Name),
            "email" => pagination.SortDescending
                ? query.OrderByDescending(c => c.Email)
                : query.OrderBy(c => c.Email),
            "loyaltypoints" => pagination.SortDescending
                ? query.OrderByDescending(c => c.LoyaltyPoints)
                : query.OrderBy(c => c.LoyaltyPoints),
            "tier" => pagination.SortDescending
                ? query.OrderByDescending(c => c.Tier)
                : query.OrderBy(c => c.Tier),
            "createdat" or _ => pagination.SortDescending
                ? query.OrderByDescending(c => c.CreatedAt)
                : query.OrderBy(c => c.CreatedAt)
        };
    }

    #endregion
}