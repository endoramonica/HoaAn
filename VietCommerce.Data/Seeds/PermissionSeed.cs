using Microsoft.EntityFrameworkCore;
using VietCommerce.Core.Common.Constants;
using VietCommerce.Core.Entities.Users;
using VietCommerce.Data.Context;
using VietCommerce.Data.Seeds;

namespace VietCommerce.Data.Seeders
{
    public static class PermissionSeed
    {
        public static async Task SeedAsync(AppDbContext context)
        {
            // 1️⃣ Khởi tạo danh sách permission
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
                new Permission { Id = Guid.NewGuid(), Name = "report.view", Description = "Xem báo cáo" },

                // ADDRESS PERMISSIONS
                new Permission { Id = Guid.NewGuid(), Name = PermissionConstants.CustomerAddressManage, Description = "Quản lý địa chỉ cá nhân" },
                new Permission { Id = Guid.NewGuid(), Name = PermissionConstants.AdminAddressManage, Description = "Quản lý tất cả địa chỉ (admin)" },
                new Permission { Id = Guid.NewGuid(), Name = PermissionConstants.AdminAddressRead, Description = "Xem tất cả địa chỉ (admin)" },
            };

            // Add Cart and Order related permissions
            var cartOrderPerms = CartOrderPermissionSeed.GetCartOrderPermissions();
            permissions.AddRange(cartOrderPerms);

            // 2️⃣ Lọc những permission chưa có trong DB
            var existingNames = await context.Permissions.Select(p => p.Name).ToListAsync();
            var permissionsToAdd = permissions.Where(p => !existingNames.Contains(p.Name)).ToList();

            // 3️⃣ Thêm mới nếu có
            if (permissionsToAdd.Any())
            {
                await context.Permissions.AddRangeAsync(permissionsToAdd);
                await context.SaveChangesAsync();
            }
        }
    }
}
