// ================================================================
// FILE: RBACSeeder.cs
// Author: VietCommerce Seeder Team
// Purpose: Seed role-permission & user-role mapping
// ================================================================

using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

using VietCommerce.Core.Entities.Users;
using VietCommerce.Data.Context;

namespace VietCommerce.Data.Seeders
{
    public static class RBACSeeder
    {
        public static async Task SeedAsync(AppDbContext context)
        {
            // Ensure Roles and Permissions exist
            await RoleSeed.SeedAsync(context);
            await PermissionSeed.SeedAsync(context);

            // ROLE → PERMISSIONS
            var adminRole = await context.Roles.FirstOrDefaultAsync(r => r.Name == "Administrator");
            var staffRole = await context.Roles.FirstOrDefaultAsync(r => r.Name == "Staff");
            var customerRole = await context.Roles.FirstOrDefaultAsync(r => r.Name == "Customer");
            var managerRole = await context.Roles.FirstOrDefaultAsync(r => r.Name == "Store Manager");

            var allPerms = await context.Permissions.ToListAsync();

            // Admin có tất cả quyền
            foreach (var perm in allPerms)
            {
                if (!await context.RolePermissions.AnyAsync(x => x.RoleId == adminRole.Id && x.PermissionId == perm.Id))
                {
                    context.RolePermissions.Add(new RolePermission { Id = Guid.NewGuid(), RoleId = adminRole.Id, PermissionId = perm.Id });
                }
            }

            // Staff
            var staffPerms = allPerms.Where(p => p.Name.StartsWith("product.") || p.Name.StartsWith("order."));
            foreach (var perm in staffPerms)
            {
                if (!await context.RolePermissions.AnyAsync(x => x.RoleId == staffRole.Id && x.PermissionId == perm.Id))
                {
                    context.RolePermissions.Add(new RolePermission { Id = Guid.NewGuid(), RoleId = staffRole.Id, PermissionId = perm.Id });
                }
            }

            // Customer chỉ được xem
            var customerPerms = allPerms.Where(p => p.Name.EndsWith(".view"));
            foreach (var perm in customerPerms)
            {
                if (!await context.RolePermissions.AnyAsync(x => x.RoleId == customerRole.Id && x.PermissionId == perm.Id))
                {
                    context.RolePermissions.Add(new RolePermission { Id = Guid.NewGuid(), RoleId = customerRole.Id, PermissionId = perm.Id });
                }
            }

            // Store Manager
            var managerPerms = allPerms.Where(p =>
                p.Name == "inventory.view" || p.Name == "order.view" || p.Name == "report.view");
            foreach (var perm in managerPerms)
            {
                if (!await context.RolePermissions.AnyAsync(x => x.RoleId == managerRole.Id && x.PermissionId == perm.Id))
                {
                    context.RolePermissions.Add(new RolePermission { Id = Guid.NewGuid(), RoleId = managerRole.Id, PermissionId = perm.Id });
                }
            }

            await context.SaveChangesAsync();

            // GÁN USER → ROLE
            var systemUser = await context.Users.FirstOrDefaultAsync(u => u.Email == "system@vietcommerce.com");
            var user111 = await context.Users.FirstOrDefaultAsync(u => u.Email == "user111@example.com");
            var googleUser = await context.Users.FirstOrDefaultAsync(u => u.Email == "gg4999425@gmail.com");
            var normalUser = await context.Users.FirstOrDefaultAsync(u => u.Email == "user@example.com");

            if (systemUser != null && !await context.UserRoles.AnyAsync(x => x.UserId == systemUser.Id))
                context.UserRoles.Add(new UserRole { Id = Guid.NewGuid(), UserId = systemUser.Id, RoleId = adminRole.Id });

            if (user111 != null && !await context.UserRoles.AnyAsync(x => x.UserId == user111.Id))
                context.UserRoles.Add(new UserRole { Id = Guid.NewGuid(), UserId = user111.Id, RoleId = staffRole.Id });
        }
    }
}
