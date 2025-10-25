using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;
using System.Security.Claims;
using VietCommerce.Api.Services.Interfaces;
using VietCommerce.Core.Entities.Tasks;
using VietCommerce.Api.Authorization;
namespace VietCommerce.Api.Authorization;
public class PermissionAuthorizationHandler : AuthorizationHandler<PermissionRequirement>
{
    private readonly IServiceScopeFactory _serviceScopeFactory;
    public PermissionAuthorizationHandler(IServiceScopeFactory serviceScopeFactory)
    {
        _serviceScopeFactory = serviceScopeFactory;
    }
    protected override async Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        PermissionRequirement requirement)
    {
        var userIdClaim = context.User.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out var userId))
        {
            context.Fail();
            return;
        }
        using var scope = _serviceScopeFactory.CreateScope();
        var permissionService = scope.ServiceProvider.GetRequiredService<IPermissionService>();
        foreach (var permission in requirement.Permissions)
        {
            var hasPermission = await permissionService.CheckUserPermissionAsync(userId, permission);
            if (hasPermission)
            {
                context.Succeed(requirement);
                return;
            }
        }
        context.Fail();
    }
}
