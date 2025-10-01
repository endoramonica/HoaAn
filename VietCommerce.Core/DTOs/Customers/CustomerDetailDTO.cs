namespace VietCommerce.Core.DTOs.Customers;

public class CustomerDetailDTO
{
    public Guid Id { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string? Email { get; set; }
    public string? PhoneNumber { get; set; }
    public Guid? UserId { get; set; }
    public string? UserEmail { get; set; }
    public DateTime CreatedDate { get; set; }
    public DateTime UpdatedDate { get; set; }
    public bool IsActive { get; set; }
    public int LoyaltyPoints { get; set; }
    public string? Tier { get; set; }
    public int TotalOrders { get; set; }
    public decimal TotalSpent { get; set; }
}
