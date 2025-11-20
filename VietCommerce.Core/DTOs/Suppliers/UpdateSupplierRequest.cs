using System.ComponentModel.DataAnnotations;
using VietCommerce.Core.Entities.Logistics;

namespace VietCommerce.Core.DTOs.Suppliers;

/// <summary>
/// Request model for updating an existing supplier
/// </summary>
public class UpdateSupplierRequest
{
    /// <summary>
    /// Supplier name (unique)
    /// </summary>
    [MaxLength(200, ErrorMessage = "Supplier name cannot exceed 200 characters")]
    [MinLength(2, ErrorMessage = "Supplier name must be at least 2 characters")]
    public string? Name { get; set; }

    /// <summary>
    /// Supplier email (unique)
    /// </summary>
    [EmailAddress(ErrorMessage = "Invalid email format")]
    [MaxLength(255, ErrorMessage = "Email cannot exceed 255 characters")]
    public string? Email { get; set; }

    /// <summary>
    /// Supplier phone number
    /// </summary>
    [MaxLength(20, ErrorMessage = "Phone number cannot exceed 20 characters")]
    [RegularExpression(@"^[\d\s\-\+\(\)]+$", ErrorMessage = "Invalid phone number format")]
    public string? Phone { get; set; }

    /// <summary>
    /// Supplier address
    /// </summary>
    [MaxLength(500, ErrorMessage = "Address cannot exceed 500 characters")]
    [MinLength(10, ErrorMessage = "Address must be at least 10 characters")]
    public string? Address { get; set; }

    /// <summary>
    /// Supplier status
    /// </summary>
    public SupplierStatus? Status { get; set; }

    /// <summary>
    /// Payment terms (e.g., "Net 30", "Net 60", "COD")
    /// </summary>
    [MaxLength(100, ErrorMessage = "Payment terms cannot exceed 100 characters")]
    public string? PaymentTerms { get; set; }

    /// <summary>
    /// Delivery schedule (e.g., "Daily", "Weekly", "Bi-weekly")
    /// </summary>
    [MaxLength(100, ErrorMessage = "Delivery schedule cannot exceed 100 characters")]
    public string? DeliverySchedule { get; set; }
}