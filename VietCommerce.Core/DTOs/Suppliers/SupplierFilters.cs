using System.ComponentModel.DataAnnotations;
using VietCommerce.Core.Entities.Logistics;

namespace VietCommerce.Core.DTOs.Suppliers;

/// <summary>
/// Filters for querying suppliers
/// </summary>
public class SupplierFilters
{
    /// <summary>
    /// Search by supplier name (partial match)
    /// </summary>
    [MaxLength(200)]
    public string? Name { get; set; }

    /// <summary>
    /// Search by email (partial match)
    /// </summary>
    [MaxLength(255)]
    public string? Email { get; set; }

    /// <summary>
    /// Search by phone (partial match)
    /// </summary>
    [MaxLength(20)]
    public string? Phone { get; set; }

    /// <summary>
    /// Filter by supplier status
    /// </summary>
    public SupplierStatus? Status { get; set; }

    /// <summary>
    /// Search by address (partial match)
    /// </summary>
    [MaxLength(500)]
    public string? Address { get; set; }

    /// <summary>
    /// Filter by creation date from
    /// </summary>
    public DateTime? CreatedFrom { get; set; }

    /// <summary>
    /// Filter by creation date to
    /// </summary>
    public DateTime? CreatedTo { get; set; }

    /// <summary>
    /// Filter by payment terms (partial match)
    /// </summary>
    [MaxLength(100)]
    public string? PaymentTerms { get; set; }

    /// <summary>
    /// General search term (searches across Name, Email, Phone, Address)
    /// </summary>
    [MaxLength(500)]
    public string? SearchTerm { get; set; }
}