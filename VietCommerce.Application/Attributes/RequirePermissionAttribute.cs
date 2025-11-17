using Microsoft.AspNetCore.Authorization;
namespace VietCommerce.Application.Attributes;
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false)]
public class RequirePermissionAttribute : AuthorizeAttribute
{
    public RequirePermissionAttribute(params string[] permissions)
    {
        Policy = string.Join(",", permissions);
    }
}
