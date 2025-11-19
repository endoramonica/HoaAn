namespace VietCommerce.Core.DTOs.Customers;

/// <summary>
/// DTO for customer list view
/// </summary>
public class CustomerListDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public int LoyaltyPoints { get; set; }
    public string? Tier { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public int TotalOrders { get; set; }
    public decimal TotalSpent { get; set; }
}
