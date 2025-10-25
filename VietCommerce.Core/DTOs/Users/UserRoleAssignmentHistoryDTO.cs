namespace VietCommerce.Core.DTOs.Users;
public class UserRoleAssignmentHistoryDTO
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string UserName { get; set; } = string.Empty;
    public Guid RoleId { get; set; }
    public string RoleName { get; set; } = string.Empty;
    public string Action { get; set; } = string.Empty; // "Assigned" or "Removed"
    public DateTime ActionDate { get; set; }
    public Guid? PerformedBy { get; set; }
    public string? PerformedByName { get; set; }
    public string? Reason { get; set; }
}
