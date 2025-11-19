using System;
using System.ComponentModel.DataAnnotations;
using VietCommerce.Core.Enums.Common;

namespace VietCommerce.Core.DTOs.Address;

/// <summary>
/// DTO for customer address details
/// </summary>
public class CustomerAddressDto
{
    public Guid Id { get; set; }
    public Guid CustomerId { get; set; }

    // Address Info
    public string StreetAddress { get; set; } = string.Empty;
    public string? City { get; set; }
    public string? PostalCode { get; set; }
    public string? State { get; set; }
    public string? Country { get; set; }

    public AddressType AddressType { get; set; }
    public string AddressTypeText { get; set; } = string.Empty;

    // Recipient Info
    public string? RecipientName { get; set; }
    public string? PhoneNumber { get; set; }
    public string? Email { get; set; }

    // Flags
    public bool IsDefault { get; set; }
    public bool IsPrimary { get; set; }
    public bool IsActive { get; set; }

    // Audit
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

/// <summary>
/// Request for creating new customer address
/// </summary>
public class CreateCustomerAddressRequest
{
    [Required(ErrorMessage = "Customer ID is required")]
    public Guid CustomerId { get; set; }

    [Required(ErrorMessage = "Street address is required")]
    [StringLength(500, ErrorMessage = "Street address cannot exceed 500 characters")]
    public string StreetAddress { get; set; } = string.Empty;

    [StringLength(200, ErrorMessage = "City cannot exceed 200 characters")]
    public string? City { get; set; }

    [StringLength(50, ErrorMessage = "Postal code cannot exceed 50 characters")]
    public string? PostalCode { get; set; }

    [StringLength(200, ErrorMessage = "State cannot exceed 200 characters")]
    public string? State { get; set; }

    [StringLength(100, ErrorMessage = "Country cannot exceed 100 characters")]
    public string? Country { get; set; }

    [Required(ErrorMessage = "Address type is required")]
    public AddressType AddressType { get; set; }

    // Recipient Info
    [StringLength(200, ErrorMessage = "Recipient name cannot exceed 200 characters")]
    public string? RecipientName { get; set; }

    [Phone(ErrorMessage = "Invalid phone number format")]
    [StringLength(20, ErrorMessage = "Phone number cannot exceed 20 characters")]
    public string? PhoneNumber { get; set; }

    [EmailAddress(ErrorMessage = "Invalid email format")]
    [StringLength(200, ErrorMessage = "Email cannot exceed 200 characters")]
    public string? Email { get; set; }

    public bool IsDefault { get; set; }
    public bool IsPrimary { get; set; }
    public bool IsActive { get; set; } = true;
}

/// <summary>
/// Request for updating customer address
/// </summary>
public class UpdateCustomerAddressRequest
{
    [StringLength(500, ErrorMessage = "Street address cannot exceed 500 characters")]
    public string? StreetAddress { get; set; }

    [StringLength(200, ErrorMessage = "City cannot exceed 200 characters")]
    public string? City { get; set; }

    [StringLength(50, ErrorMessage = "Postal code cannot exceed 50 characters")]
    public string? PostalCode { get; set; }

    [StringLength(200, ErrorMessage = "State cannot exceed 200 characters")]
    public string? State { get; set; }

    [StringLength(100, ErrorMessage = "Country cannot exceed 100 characters")]
    public string? Country { get; set; }

    public AddressType? AddressType { get; set; }

    // Recipient Info
    [StringLength(200, ErrorMessage = "Recipient name cannot exceed 200 characters")]
    public string? RecipientName { get; set; }

    [Phone(ErrorMessage = "Invalid phone number format")]
    [StringLength(20, ErrorMessage = "Phone number cannot exceed 20 characters")]
    public string? PhoneNumber { get; set; }

    [EmailAddress(ErrorMessage = "Invalid email format")]
    [StringLength(200, ErrorMessage = "Email cannot exceed 200 characters")]
    public string? Email { get; set; }

    public bool? IsDefault { get; set; }
    public bool? IsPrimary { get; set; }
    public bool? IsActive { get; set; }
}