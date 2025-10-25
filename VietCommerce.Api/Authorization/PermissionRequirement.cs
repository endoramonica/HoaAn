using Microsoft.AspNetCore.Authorization;
namespace VietCommerce.Api.Authorization;
public class PermissionRequirement : IAuthorizationRequirement
{
    public string[] Permissions { get; }
    public PermissionRequirement(params string[] permissions)
    {
        Permissions = permissions ?? throw new ArgumentNullException(nameof(permissions));
    }
}
