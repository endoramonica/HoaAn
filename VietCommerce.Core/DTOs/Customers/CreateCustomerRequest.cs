using System.ComponentModel.DataAnnotations;

namespace VietCommerce.Core.DTOs.Customers;

/// <summary>
/// Request for creating new customer
/// </summary>
public class CreateCustomerRequest
{
    [Required(ErrorMessage = "Name is required")]
    [StringLength(200, ErrorMessage = "Name cannot exceed 200 characters")]
    public string Name { get; set; } = string.Empty;

    [EmailAddress(ErrorMessage = "Invalid email format")]
    [StringLength(200, ErrorMessage = "Email cannot exceed 200 characters")]
    public string? Email { get; set; }

    [Phone(ErrorMessage = "Invalid phone number format")]
    [StringLength(20, ErrorMessage = "Phone cannot exceed 20 characters")]
    public string? Phone { get; set; }

    public Guid? UserId { get; set; }
    public Guid? StoreId { get; set; }

    [StringLength(50, ErrorMessage = "Tier cannot exceed 50 characters")]
    public string? Tier { get; set; }

    public bool IsActive { get; set; } = true;
}

// Helper Request Classes cho Controller (nếu chưa có trong Core.DTOs)
public class AddPointsRequest
{
    public int Points { get; set; }
    public string Reason { get; set; } = string.Empty;
}

public class DeductPointsRequest
{
    public int Points { get; set; }
    public string Reason { get; set; } = string.Empty;
}

public class UpdateTierRequest
{
    public string Tier { get; set; } = string.Empty;
}