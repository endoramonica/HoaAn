using System;
using System.ComponentModel.DataAnnotations;
using VietCommerce.Core.DTOs.Address;

namespace VietCommerce.Core.DTOs.Customers;

/// <summary>
/// DTO for detailed customer information
/// </summary>
public class CustomerDetailDto
{
    public Guid Id { get; set; }

    // Basic Info
    public string Name { get; set; } = string.Empty;
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public string? Avatar { get; set; }

    // Loyalty & Tier
    public int LoyaltyPoints { get; set; }
    public string? Tier { get; set; }

    // Status
    public bool IsActive { get; set; }

    // Relations
    public Guid? UserId { get; set; }
    public Guid? StoreId { get; set; }
    public string? StoreName { get; set; }

    // Tenant
    public Guid TenantId { get; set; }

    // Audit
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public Guid CreatedBy { get; set; }
    public Guid? UpdatedBy { get; set; }

    // Statistics
    public int TotalOrders { get; set; }
    public decimal TotalSpent { get; set; }
    public int TotalInteractions { get; set; }

    // User Info (from User entity)
    public string? UserProvider { get; set; }
    public DateTime? LastLogin { get; set; }
    public string? UserStatus { get; set; }

    // Collections
    public List<CustomerAddressDto> Addresses { get; set; } = new();
}

