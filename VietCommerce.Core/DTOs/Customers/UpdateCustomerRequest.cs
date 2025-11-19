using System.ComponentModel.DataAnnotations;

namespace VietCommerce.Core.DTOs.Customers;

/// <summary>
/// Request for updating customer
/// </summary>
public class UpdateCustomerRequest
{
    [StringLength(200, ErrorMessage = "Name cannot exceed 200 characters")]
    public string? Name { get; set; }

    [EmailAddress(ErrorMessage = "Invalid email format")]
    [StringLength(200, ErrorMessage = "Email cannot exceed 200 characters")]
    public string? Email { get; set; }

    [Phone(ErrorMessage = "Invalid phone number format")]
    [StringLength(20, ErrorMessage = "Phone cannot exceed 20 characters")]
    public string? Phone { get; set; }

    [Range(0, int.MaxValue, ErrorMessage = "Loyalty points must be non-negative")]
    public int? LoyaltyPoints { get; set; }

    [StringLength(50, ErrorMessage = "Tier cannot exceed 50 characters")]
    public string? Tier { get; set; }

    public bool? IsActive { get; set; }

    public Guid? StoreId { get; set; }
}
