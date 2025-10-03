using Microsoft.AspNetCore.Authorization;
using VietCommerce.Api.Authorization;

namespace VietCommerce.Api.Extensions;

public static class AuthorizationExtensions
{
    public static IServiceCollection AddRBACAuthorization(this IServiceCollection services)
    {
        services.AddAuthorization(options =>
        {
            // Permission-based policies
            var permissions = GetAllPermissions();
            foreach (var permission in permissions)
            {
                options.AddPolicy(permission, policy => 
                    policy.Requirements.Add(new PermissionRequirement(permission)));
            }

            // Role-based policies
            var roles = GetAllRoles();
            foreach (var role in roles)
            {
                options.AddPolicy($"Role:{role}", policy => 
                    policy.Requirements.Add(new RoleRequirement(role)));
            }

            // Combined policies
            options.AddPolicy("AdminOnly", policy => 
                policy.Requirements.Add(new RoleRequirement("Admin", "SuperAdmin")));

            options.AddPolicy("ManagerOrAbove", policy => 
                policy.Requirements.Add(new RoleRequirement("Admin", "SuperAdmin", "Manager")));
        });

        // Register authorization handlers
        services.AddScoped<IAuthorizationHandler, PermissionAuthorizationHandler>();
        services.AddScoped<IAuthorizationHandler, RoleAuthorizationHandler>();

        return services;
    }

    private static List<string> GetAllPermissions()
    {
        return new List<string>
        {
            // User permissions
            "user.create", "user.read", "user.update", "user.delete",
            
            // Role permissions
            "role.create", "role.read", "role.update", "role.delete", "role.assign",
            
            // Permission permissions
            "permission.create", "permission.read", "permission.update", "permission.delete", "permission.assign", "permission.check",
            
            // Product permissions
            "product.create", "product.read", "product.update", "product.delete",
            
            // Order permissions
            "order.create", "order.read", "order.update", "order.delete",
            
            // Report permissions
            "report.sales", "report.inventory", "report.users",
            
            // System permissions
            "system.admin", "system.config", "system.logs"
        };
    }

    private static List<string> GetAllRoles()
    {
        return new List<string>
        {
            "SuperAdmin", "Admin", "Manager", "Staff", "Customer"
        };
    }
}
