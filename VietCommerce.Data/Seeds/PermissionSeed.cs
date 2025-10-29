// ================================================================
// FILE: PermissionSeed.cs
// Author: VietCommerce Seeder Team
// Purpose: Seed base permissions for RBAC
// ================================================================

using Microsoft.EntityFrameworkCore;
using VietCommerce.Core.Entities.Users;
using VietCommerce.Data.Context;
using VietCommerce.Data.Seeds;

namespace VietCommerce.Data.Seeders
{
    public static class PermissionSeed
    {
        public static async Task SeedAsync(AppDbContext context)
        {
            if (await context.Permissions.AnyAsync()) return;

            var permissions = new List<Permission>
            {
                // Product
                new Permission { Id = Guid.NewGuid(), Name = "product.view", Description = "Xem sản phẩm" },
                new Permission { Id = Guid.NewGuid(), Name = "product.create", Description = "Tạo sản phẩm" },
                new Permission { Id = Guid.NewGuid(), Name = "product.update", Description = "Cập nhật sản phẩm" },
                new Permission { Id = Guid.NewGuid(), Name = "product.delete", Description = "Xóa sản phẩm" },

                // Order
                new Permission { Id = Guid.NewGuid(), Name = "order.view", Description = "Xem đơn hàng" },
                new Permission { Id = Guid.NewGuid(), Name = "order.create", Description = "Tạo đơn hàng" },
                new Permission { Id = Guid.NewGuid(), Name = "order.update", Description = "Cập nhật đơn hàng" },

                // Customer
                new Permission { Id = Guid.NewGuid(), Name = "customer.view", Description = "Xem khách hàng" },
                new Permission { Id = Guid.NewGuid(), Name = "customer.manage", Description = "Quản lý khách hàng" },

                // Inventory & Reports
                new Permission { Id = Guid.NewGuid(), Name = "inventory.view", Description = "Xem kho hàng" },
                new Permission { Id = Guid.NewGuid(), Name = "report.view", Description = "Xem báo cáo" }
            };
            // Add Cart and Order related permissions
            var cartOrderPerms = CartOrderPermissionSeed.GetCartOrderPermissions();
            permissions.AddRange(cartOrderPerms);


            await context.Permissions.AddRangeAsync(permissions);
            await context.SaveChangesAsync();
        }
    }
}
