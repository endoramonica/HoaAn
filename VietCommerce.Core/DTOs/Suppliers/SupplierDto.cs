using VietCommerce.Core.Entities.Logistics;

namespace VietCommerce.Core.DTOs.Suppliers;

/// <summary>
/// Supplier data transfer object
/// </summary>
public class SupplierDto
{
    public Guid Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public string Phone { get; set; } = string.Empty;

    public string Address { get; set; } = string.Empty;

    public SupplierStatus Status { get; set; }

    public string StatusDisplay { get; set; } = string.Empty;

    public string PaymentTerms { get; set; } = string.Empty;

    public string DeliverySchedule { get; set; } = string.Empty;

    /// <summary>
    /// Number of products associated with this supplier
    /// </summary>
    public int ProductCount { get; set; }

    /// <summary>
    /// Number of stock transfers associated with this supplier
    /// </summary>
    public int StockTransferCount { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }
}