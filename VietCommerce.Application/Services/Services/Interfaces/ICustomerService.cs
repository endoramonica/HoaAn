using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using VietCommerce.Core.DTOs.Customers;
using VietCommerce.Core.DTOs.Orders;
using VietCommerce.Core.DTOs.CRM;
using VietCommerce.Core.DTOs.Address;
using VietCommerce.Core.Models;

namespace VietCommerce.Application.Services.Services.Interfaces;

/// <summary>
/// Service interface for managing customers and CRM operations
/// </summary>
public interface ICustomerService
{
    // ========================================
    // CUSTOMER MANAGEMENT
    // ========================================

    /// <summary>
    /// Get paginated list of customers with filters
    /// </summary>
    Task<ApiResponse<PaginatedResult<CustomerListDto>>> GetCustomersAsync(
        PaginationParams pagination,
        CustomerFilters filters);

    /// <summary>
    /// Get detailed customer information by ID
    /// </summary>
    Task<ApiResponse<CustomerDetailDto>> GetCustomerByIdAsync(Guid id);

    /// <summary>
    /// Create a new customer
    /// </summary>
    Task<ApiResponse<CustomerDetailDto>> CreateCustomerAsync(CreateCustomerRequest request);

    /// <summary>
    /// Update existing customer
    /// </summary>
    Task<ApiResponse<CustomerDetailDto>> UpdateCustomerAsync(Guid id, UpdateCustomerRequest request);

    /// <summary>
    /// Soft delete a customer
    /// </summary>
    Task<ApiResponse<bool>> DeleteCustomerAsync(Guid id);

    /// <summary>
    /// Search customers by search term (Name, Email, Phone)
    /// </summary>
    Task<ApiResponse<List<CustomerListDto>>> SearchCustomersAsync(string searchTerm);

    /// <summary>
    /// Get customer statistics
    /// </summary>
    Task<ApiResponse<CustomerStatisticsDto>> GetCustomerStatisticsAsync(Guid customerId);

    // ========================================
    // CUSTOMER ORDERS
    // ========================================

    /// <summary>
    /// Get paginated list of customer orders
    /// </summary>
    Task<ApiResponse<PaginatedResult<OrderDetailDto>>> GetCustomerOrdersAsync(
        Guid customerId,
        PaginationParams pagination);

    /// <summary>
    /// Get customer order summary
    /// </summary>
    Task<ApiResponse<CustomerOrderSummaryDto>> GetCustomerOrderSummaryAsync(Guid customerId);

    // ========================================
    // CUSTOMER ADDRESSES
    // ========================================

    /// <summary>
    /// Get all addresses for a customer
    /// </summary>
    Task<ApiResponse<List<CustomerAddressDto>>> GetCustomerAddressesAsync(Guid customerId);

    /// <summary>
    /// Get specific address by ID
    /// </summary>
    Task<ApiResponse<CustomerAddressDto>> GetCustomerAddressByIdAsync(Guid addressId);

    /// <summary>
    /// Add new address for customer
    /// </summary>
    Task<ApiResponse<CustomerAddressDto>> CreateCustomerAddressAsync(CreateCustomerAddressRequest request);

    /// <summary>
    /// Update customer address
    /// </summary>
    Task<ApiResponse<CustomerAddressDto>> UpdateCustomerAddressAsync(
        Guid addressId,
        UpdateCustomerAddressRequest request);

    /// <summary>
    /// Delete customer address
    /// </summary>
    Task<ApiResponse<bool>> DeleteCustomerAddressAsync(Guid addressId);

    /// <summary>
    /// Set address as default for customer
    /// </summary>
    Task<ApiResponse<bool>> SetDefaultAddressAsync(Guid customerId, Guid addressId);

    // ========================================
    // CRM INTERACTIONS
    // ========================================

    /// <summary>
    /// Get paginated list of customer interactions with filters
    /// </summary>
    Task<ApiResponse<PaginatedResult<CRMInteractionListDto>>> GetCustomerInteractionsAsync(
        Guid customerId,
        PaginationParams pagination,
        CRMInteractionFilters? filters = null);

    /// <summary>
    /// Get interaction details by ID
    /// </summary>
    Task<ApiResponse<CRMInteractionDto>> GetInteractionByIdAsync(Guid interactionId);

    /// <summary>
    /// Create new customer interaction
    /// </summary>
    Task<ApiResponse<CRMInteractionDto>> CreateCustomerInteractionAsync(CreateInteractionRequest request);

    /// <summary>
    /// Update existing interaction
    /// </summary>
    Task<ApiResponse<CRMInteractionDto>> UpdateCustomerInteractionAsync(
        Guid interactionId,
        UpdateInteractionRequest request);

    /// <summary>
    /// Delete interaction
    /// </summary>
    Task<ApiResponse<bool>> DeleteCustomerInteractionAsync(Guid interactionId);

    /// <summary>
    /// Get upcoming follow-ups for all customers
    /// </summary>
    Task<ApiResponse<List<CRMInteractionListDto>>> GetUpcomingFollowUpsAsync(
        DateTime? fromDate = null,
        DateTime? toDate = null);

    /// <summary>
    /// Mark interaction as completed
    /// </summary>
    Task<ApiResponse<CRMInteractionDto>> CompleteInteractionAsync(Guid interactionId);

    // ========================================
    // CUSTOMER LOYALTY
    // ========================================

    /// <summary>
    /// Add loyalty points to customer
    /// </summary>
    Task<ApiResponse<CustomerDetailDto>> AddLoyaltyPointsAsync(Guid customerId, int points, string reason);

    /// <summary>
    /// Deduct loyalty points from customer
    /// </summary>
    Task<ApiResponse<CustomerDetailDto>> DeductLoyaltyPointsAsync(Guid customerId, int points, string reason);

    /// <summary>
    /// Update customer tier
    /// </summary>
    Task<ApiResponse<CustomerDetailDto>> UpdateCustomerTierAsync(Guid customerId, string tier);

    /// <summary>
    /// Get customer loyalty history
    /// </summary>
    Task<ApiResponse<List<LoyaltyHistoryDto>>> GetLoyaltyHistoryAsync(Guid customerId);
}

