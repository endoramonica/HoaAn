using System.ComponentModel.DataAnnotations;
using VietCommerce.Core.Entities.Logistics;

namespace VietCommerce.Core.DTOs.Suppliers;

/// <summary>
/// Request model for creating a new supplier
/// </summary>
public class CreateSupplierRequest
{
    /// <summary>
    /// Supplier name (unique)
    /// </summary>
    [Required(ErrorMessage = "Supplier name is required")]
    [MaxLength(200, ErrorMessage = "Supplier name cannot exceed 200 characters")]
    [MinLength(2, ErrorMessage = "Supplier name must be at least 2 characters")]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Supplier email (unique)
    /// </summary>
    [Required(ErrorMessage = "Email is required")]
    [EmailAddress(ErrorMessage = "Invalid email format")]
    [MaxLength(255, ErrorMessage = "Email cannot exceed 255 characters")]
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// Supplier phone number
    /// </summary>
    [Required(ErrorMessage = "Phone number is required")]
    [MaxLength(20, ErrorMessage = "Phone number cannot exceed 20 characters")]
    [RegularExpression(@"^[\d\s\-\+\(\)]+$", ErrorMessage = "Invalid phone number format")]
    public string Phone { get; set; } = string.Empty;

    /// <summary>
    /// Supplier address
    /// </summary>
    [Required(ErrorMessage = "Address is required")]
    [MaxLength(500, ErrorMessage = "Address cannot exceed 500 characters")]
    [MinLength(10, ErrorMessage = "Address must be at least 10 characters")]
    public string Address { get; set; } = string.Empty;

    /// <summary>
    /// Supplier status (default: Active)
    /// </summary>
    [Required(ErrorMessage = "Status is required")]
    public SupplierStatus Status { get; set; } = SupplierStatus.Active;

    /// <summary>
    /// Payment terms (e.g., "Net 30", "Net 60", "COD")
    /// </summary>
    [Required(ErrorMessage = "Payment terms are required")]
    [MaxLength(100, ErrorMessage = "Payment terms cannot exceed 100 characters")]
    public string PaymentTerms { get; set; } = string.Empty;

    /// <summary>
    /// Delivery schedule (e.g., "Daily", "Weekly", "Bi-weekly")
    /// </summary>
    [Required(ErrorMessage = "Delivery schedule is required")]
    [MaxLength(100, ErrorMessage = "Delivery schedule cannot exceed 100 characters")]
    public string DeliverySchedule { get; set; } = string.Empty;
}