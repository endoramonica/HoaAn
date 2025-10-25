// VietCommerce.Data/Seeds/CartOrderPermissionSeed.cs
using VietCommerce.Core.Entities.Users;

namespace VietCommerce.Data.Seeds
{
    public static class CartOrderPermissionSeed
    {
        /// <summary>
        /// Danh sách permissions cho Cart & Order Management
        /// Gọi trong PermissionSeed.cs hoặc RBACSeeder.cs
        /// </summary>
        public static List<Permission> GetCartOrderPermissions()
        {
            return new List<Permission>
            {
                // ============ CART PERMISSIONS ============
                new Permission
                {
                    Id = Guid.NewGuid(),
                    Name = "cart.view",
                    Description = "Xem giỏ hàng của mình",
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                },
                new Permission
                {
                    Id = Guid.NewGuid(),
                    Name = "cart.add_item",
                    Description = "Thêm sản phẩm vào giỏ hàng",
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                },
                new Permission
                {
                    Id = Guid.NewGuid(),
                    Name = "cart.remove_item",
                    Description = "Xóa sản phẩm khỏi giỏ hàng",
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                },
                new Permission
                {
                    Id = Guid.NewGuid(),
                    Name = "cart.update_item",
                    Description = "Cập nhật số lượng sản phẩm trong giỏ",
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                },
                new Permission
                {
                    Id = Guid.NewGuid(),
                    Name = "cart.clear",
                    Description = "Xóa toàn bộ giỏ hàng",
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                },

                // ============ ORDER PERMISSIONS ============
                new Permission
                {
                    Id = Guid.NewGuid(),
                    Name = "order.create",
                    Description = "Tạo đơn hàng mới từ giỏ",
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                },
                new Permission
                {
                    Id = Guid.NewGuid(),
                    Name = "order.view_own",
                    Description = "Xem đơn hàng của mình",
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                },
                new Permission
                {
                    Id = Guid.NewGuid(),
                    Name = "order.view_all",
                    Description = "Xem tất cả đơn hàng (Admin)",
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                },
                new Permission
                {
                    Id = Guid.NewGuid(),
                    Name = "order.update_status",
                    Description = "Cập nhật trạng thái đơn hàng (Admin)",
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                },
                new Permission
                {
                    Id = Guid.NewGuid(),
                    Name = "order.cancel",
                    Description = "Hủy đơn hàng",
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                }
            };
        }

        /// <summary>
        /// Gán permissions tới role (dùng trong RBACSeeder)
        /// </summary>
        public static void AssignCartOrderPermissionsToRoles(
            List<Role> roles,
            List<Permission> permissions)
        {
            var customerRole = roles.FirstOrDefault(r => r.Name == "Customer");
            var sellerRole = roles.FirstOrDefault(r => r.Name == "Seller");
            var adminRole = roles.FirstOrDefault(r => r.Name == "Admin");

            if (customerRole != null)
            {
                // Customer: có thể quản lý giỏ hàng riêng và xem đơn hàng của mình
                var customerPerms = new[]
                {
                    "cart.view", "cart.add_item", "cart.remove_item",
                    "cart.update_item", "cart.clear",
                    "order.create", "order.view_own", "order.cancel"
                };

                foreach (var permName in customerPerms)
                {
                    var perm = permissions.FirstOrDefault(p => p.Name == permName);
                    if (perm != null && !customerRole.RolePermissions.Any(rp => rp.PermissionId == perm.Id))
                    {
                        customerRole.RolePermissions.Add(new RolePermission
                        {
                            Id = Guid.NewGuid(),
                            RoleId = customerRole.Id,
                            PermissionId = perm.Id,
                            CreatedAt = DateTime.UtcNow,
                            UpdatedAt = DateTime.UtcNow
                        });
                    }
                }
            }

            if (sellerRole != null)
            {
                // Seller: có thể xem tất cả đơn hàng, cập nhật trạng thái
                var sellerPerms = new[]
                {
                    "order.view_all", "order.update_status"
                };

                foreach (var permName in sellerPerms)
                {
                    var perm = permissions.FirstOrDefault(p => p.Name == permName);
                    if (perm != null && !sellerRole.RolePermissions.Any(rp => rp.PermissionId == perm.Id))
                    {
                        sellerRole.RolePermissions.Add(new RolePermission
                        {
                            Id = Guid.NewGuid(),
                            RoleId = sellerRole.Id,
                            PermissionId = perm.Id,
                            CreatedAt = DateTime.UtcNow,
                            UpdatedAt = DateTime.UtcNow
                        });
                    }
                }
            }

            if (adminRole != null)
            {
                // Admin: có toàn quyền
                var allPermNames = new[]
                {
                    "cart.view", "cart.add_item", "cart.remove_item",
                    "cart.update_item", "cart.clear",
                    "order.create", "order.view_own", "order.view_all",
                    "order.update_status", "order.cancel"
                };

                foreach (var permName in allPermNames)
                {
                    var perm = permissions.FirstOrDefault(p => p.Name == permName);
                    if (perm != null && !adminRole.RolePermissions.Any(rp => rp.PermissionId == perm.Id))
                    {
                        adminRole.RolePermissions.Add(new RolePermission
                        {
                            Id = Guid.NewGuid(),
                            RoleId = adminRole.Id,
                            PermissionId = perm.Id,
                            CreatedAt = DateTime.UtcNow,
                            UpdatedAt = DateTime.UtcNow
                        });
                    }
                }
            }
        }
    }
}