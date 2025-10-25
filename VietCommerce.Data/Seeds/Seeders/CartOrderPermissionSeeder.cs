// VietCommerce.Data/Seeders/CartOrderPermissionSeeder.cs
using Microsoft.EntityFrameworkCore;
using VietCommerce.Core.Entities.Users;
using VietCommerce.Data.Context;
using VietCommerce.Data.Seeds;

namespace VietCommerce.Data.Seeders
{
    public class CartOrderPermissionSeeder
    {
        public async Task SeedAsync(AppDbContext context)
        {
            Console.WriteLine("🛒 Seeding Cart & Order Permissions...");

            // Lấy danh sách permissions
            var cartOrderPermissions = CartOrderPermissionSeed.GetCartOrderPermissions();
            var existingPermissions = await context.Permissions.ToListAsync();

            // Thêm permissions mới
            int addedCount = 0;
            foreach (var permission in cartOrderPermissions)
            {
                if (!existingPermissions.Any(p => p.Name == permission.Name))
                {
                    context.Permissions.Add(permission);
                    addedCount++;
                    Console.WriteLine($"  ➕ Added: {permission.Name}");
                }
            }

            if (addedCount > 0)
            {
                await context.SaveChangesAsync();
                Console.WriteLine($"✅ Added {addedCount} Cart & Order Permissions!");
            }
            else
            {
                Console.WriteLine("⏭️  Cart & Order Permissions already exist, skipping...");
            }

            // Gán permissions cho roles
            await AssignPermissionsToRoles(context);
        }

        private async Task AssignPermissionsToRoles(AppDbContext context)
        {
            Console.WriteLine("🔐 Assigning Cart & Order Permissions to Roles...");

            var roles = await context.Roles
                .Include(r => r.RolePermissions)
                .ToListAsync();

            var permissions = await context.Permissions.ToListAsync();

            var adminRole = roles.FirstOrDefault(r => r.Name == "Administrator");
            var customerRole = roles.FirstOrDefault(r => r.Name == "Customer");
            var staffRole = roles.FirstOrDefault(r => r.Name == "Staff");

            int assignedCount = 0;

            // Administrator: Full permissions
            if (adminRole != null)
            {
                var adminPerms = new[]
                {
                    "cart.view", "cart.add_item", "cart.remove_item", "cart.update_item", "cart.clear",
                    "order.create", "order.view_own", "order.view_all", "order.update_status", "order.cancel"
                };

                foreach (var permName in adminPerms)
                {
                    var perm = permissions.FirstOrDefault(p => p.Name == permName);
                    if (perm != null && !adminRole.RolePermissions.Any(rp => rp.PermissionId == perm.Id))
                    {
                        context.RolePermissions.Add(new RolePermission
                        {
                            Id = Guid.NewGuid(),
                            RoleId = adminRole.Id,
                            PermissionId = perm.Id,
                            CreatedAt = DateTime.UtcNow,
                            UpdatedAt = DateTime.UtcNow
                        });
                        assignedCount++;
                        Console.WriteLine($"  ✓ Administrator ← {permName}");
                    }
                }
            }

            // Customer: Cart + Own orders
            if (customerRole != null)
            {
                var customerPerms = new[]
                {
                    "cart.view", "cart.add_item", "cart.remove_item", "cart.update_item", "cart.clear",
                    "order.create", "order.view_own", "order.cancel"
                };

                foreach (var permName in customerPerms)
                {
                    var perm = permissions.FirstOrDefault(p => p.Name == permName);
                    if (perm != null && !customerRole.RolePermissions.Any(rp => rp.PermissionId == perm.Id))
                    {
                        context.RolePermissions.Add(new RolePermission
                        {
                            Id = Guid.NewGuid(),
                            RoleId = customerRole.Id,
                            PermissionId = perm.Id,
                            CreatedAt = DateTime.UtcNow,
                            UpdatedAt = DateTime.UtcNow
                        });
                        assignedCount++;
                        Console.WriteLine($"  ✓ Customer ← {permName}");
                    }
                }
            }

            // Staff: View all orders + Update status
            if (staffRole != null)
            {
                var staffPerms = new[]
                {
                    "order.view_all", "order.update_status"
                };

                foreach (var permName in staffPerms)
                {
                    var perm = permissions.FirstOrDefault(p => p.Name == permName);
                    if (perm != null && !staffRole.RolePermissions.Any(rp => rp.PermissionId == perm.Id))
                    {
                        context.RolePermissions.Add(new RolePermission
                        {
                            Id = Guid.NewGuid(),
                            RoleId = staffRole.Id,
                            PermissionId = perm.Id,
                            CreatedAt = DateTime.UtcNow,
                            UpdatedAt = DateTime.UtcNow
                        });
                        assignedCount++;
                        Console.WriteLine($"  ✓ Staff ← {permName}");
                    }
                }
            }

            if (assignedCount > 0)
            {
                await context.SaveChangesAsync();
                Console.WriteLine($"✅ Assigned {assignedCount} permissions to roles!");
            }
            else
            {
                Console.WriteLine("⏭️  Permissions already assigned, skipping...");
            }
        }
    }
}