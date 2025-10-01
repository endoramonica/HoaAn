namespace VietCommerce.Core.DTOs.Admin;

public class AdminDashboardDTO
{
    public int TotalUsers { get; set; }
    public int ActiveUsers { get; set; }
    public int InactiveUsers { get; set; }
    public int TotalCustomers { get; set; }
    public int TotalStores { get; set; }
    public DateTime LastUpdated { get; set; }
}
