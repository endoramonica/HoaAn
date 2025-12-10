// Core/DTOs/Users/UserDetailDTO.cs
using VietCommerce.Core.Enums.Users;
namespace VietCommerce.Core.DTOs.Users;

public class UserDetailDTO
{
    public Guid Id { get; set; }
    public string Email { get; set; } = string.Empty;
    public string? Name { get; set; }
    public string? Phone { get; set; }
    public string? AvatarUrl { get; set; }
    public bool IsActive { get; set; }
    public UserStatus Status { get; set; }
    public string? StatusText { get; set; }
    public DateTime? LastLogin { get; set; }
    public Guid StoreId { get; set; }
    public string StoreName { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public List<string> Roles { get; set; } = new();
}
