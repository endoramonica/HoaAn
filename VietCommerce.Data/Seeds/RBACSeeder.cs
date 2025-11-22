// ================================================================
// FILE: RBACSeeder.cs
// Author: VietCommerce Seeder Team
// Purpose: Seed role-permission & user-role mapping (safe version)
// ================================================================
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using VietCommerce.Core.Common.Constants;
using VietCommerce.Core.Entities.Users;
using VietCommerce.Core.Enums.Users;
using VietCommerce.Data.Context;

namespace VietCommerce.Data.Seeders
{
    public static class RBACSeeder
    {
        public static async Task SeedAsync(AppDbContext context)
        {
            // 1️⃣ Đảm bảo Role & Permission đã được seed
            await RoleSeed.SeedAsync(context);
            await PermissionSeed.SeedAsync(context);

            // 2️⃣ Lấy Role
            var adminRole = await context.Roles.FirstOrDefaultAsync(r => r.Name == "Administrator");
            var staffRole = await context.Roles.FirstOrDefaultAsync(r => r.Name == "Staff");
            var customerRole = await context.Roles.FirstOrDefaultAsync(r => r.Name == "Customer");
            var managerRole = await context.Roles.FirstOrDefaultAsync(r => r.Name == "Store Manager");

            if (adminRole == null || staffRole == null || customerRole == null || managerRole == null)
                throw new InvalidOperationException("Roles must be seeded before RBAC mapping.");

            var allPerms = await context.Permissions.ToListAsync();

            // ===== ROLE → PERMISSIONS =====
            await AddPermissionsToRole(context, adminRole.Id, allPerms);
            await AddPermissionsToRole(context, staffRole.Id, allPerms.Where(p =>
                p.Name.StartsWith("product.") ||
                p.Name.StartsWith("order.") ||
                p.Name == "inventory.view" ||
                p.Name == "customer.view"));
            await AddPermissionsToRole(context, customerRole.Id, allPerms.Where(p =>
                p.Name == "product.view" ||
                p.Name == "inventory.view" ||
                p.Name == "report.view" ||
                p.Name == PermissionConstants.CustomerAddressManage ||
                p.Name == PermissionConstants.CartView ||
                p.Name == PermissionConstants.CartManage ||
                p.Name == PermissionConstants.OrderCreate ||
                p.Name == PermissionConstants.OrderViewOwn ||
                p.Name == PermissionConstants.OrderCancelOwn));
            await AddPermissionsToRole(context, managerRole.Id, allPerms.Where(p =>
                p.Name == "inventory.view" ||
                p.Name == "order.view" ||
                p.Name == "report.view" ||
                p.Name == "customer.view" ||
                p.Name == PermissionConstants.OrderViewAll ||
                p.Name == PermissionConstants.OrderUpdateStatus ||
                p.Name == PermissionConstants.AdminAddressRead));

            await context.SaveChangesAsync(); // Lưu RolePermissions

            // ================================================================
            // BƯỚC 1: TẠO USERS (nếu chưa có)
            // ================================================================
            var seedUsers = new[]
            {
                new { Email = "system@vietcommerce.com", Name = "system", RoleId = adminRole.Id, Provider = "local" },
                new { Email = "user111@example.com",    Name = "user111", RoleId = staffRole.Id, Provider = "local" },
                new { Email = "gg4999425@gmail.com",    Name = "google_user", RoleId = customerRole.Id, Provider = "google" },
                new { Email = "user@example.com",       Name = "normal_user", RoleId = customerRole.Id, Provider = "local" }
            };

            foreach (var item in seedUsers)
            {
                var userExists = await context.Users.AnyAsync(u => u.Email == item.Email);
                if (!userExists)
                {
                    var newUser = new User
                    {
                        Id = Guid.NewGuid(),
                        Email = item.Email,
                        Name = item.Name,
                        Provider = item.Provider,
                        ProviderId = item.Provider == "google" ? "google_123" : null,
                        IsActive = true,
                        Status = UserStatus.ACTIVE,
                        PasswordHash = "seeded_hash", // Nên dùng BCrypt.HashPassword()
                        CreatedAt = DateTime.UtcNow,
                        LastLogin = null
                    };

                    context.Users.Add(newUser);
                }
            }

            await context.SaveChangesAsync(); // Lưu tất cả Users trước

            // ================================================================
            // BƯỚC 2: GÁN USER → ROLE (sau khi Users đã được lưu)
            // ================================================================
            foreach (var item in seedUsers)
            {
                var user = await context.Users.FirstOrDefaultAsync(u => u.Email == item.Email);

                if (user != null)
                {
                    var roleExists = await context.UserRoles.AnyAsync(ur =>
                        ur.UserId == user.Id && ur.RoleId == item.RoleId);

                    if (!roleExists)
                    {
                        context.UserRoles.Add(new UserRole
                        {
                            Id = Guid.NewGuid(),
                            UserId = user.Id, // chắc chắn UserId không rỗng
                            RoleId = item.RoleId
                        });
                    }
                }
            }

            // ================================================================
            // 🔍 Debug log & detach UserRole vô chủ
            // ================================================================
            var problematicUserRoles = context.ChangeTracker.Entries<UserRole>()
                .Where(e => e.State != EntityState.Unchanged && e.Entity.UserId == Guid.Empty)
                .ToList();

            foreach (var entry in problematicUserRoles)
            {
                var role = context.Roles.AsNoTracking().FirstOrDefault(r => r.Id == entry.Entity.RoleId);
                Console.WriteLine($"⚠️ Detaching UserRole Id: {entry.Entity.Id}, UserId: {entry.Entity.UserId}, RoleId: {entry.Entity.RoleId}, RoleName: {role?.Name ?? "Unknown"}");
                entry.State = EntityState.Detached; // detach để EF Core không ném lỗi
            }

            // ✅ Lưu tất cả UserRoles còn lại
            await context.SaveChangesAsync();
        }

        // Helper: Thêm quyền cho role (tránh lặp code)
        private static async Task AddPermissionsToRole(AppDbContext context, Guid roleId, IEnumerable<Permission> permissions)
        {
            foreach (var perm in permissions)
            {
                if (!await context.RolePermissions.AnyAsync(x => x.RoleId == roleId && x.PermissionId == perm.Id))
                {
                    context.RolePermissions.Add(new RolePermission
                    {
                        Id = Guid.NewGuid(),
                        RoleId = roleId,
                        PermissionId = perm.Id
                    });
                }
            }
        }
    }
}
