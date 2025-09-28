using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace VietCommerce.Core.Authorization;

public class RoleAuthorizationHandler : AuthorizationHandler<RoleRequirement>
{
    protected override Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        RoleRequirement requirement)
    {
        var userRoles = context.User.FindAll(ClaimTypes.Role).Select(c => c.Value).ToList();

        var hasRequiredRole = requirement.Roles.Any(role => userRoles.Contains(role, StringComparer.OrdinalIgnoreCase));

        if (hasRequiredRole)
        {
            context.Succeed(requirement);
        }
        else
        {
            context.Fail();
        }

        return Task.CompletedTask;
    }
}
