// ================================================================
// FILE: RoleSeed.cs
// Author: VietCommerce Seeder Team
// Purpose: Seed default system roles
// ================================================================

using Microsoft.EntityFrameworkCore;
using VietCommerce.Core.Entities.Users;
using VietCommerce.Data.Context;

namespace VietCommerce.Data.Seeders
{
    public static class RoleSeed
    {
        public static async Task SeedAsync(AppDbContext context)
        {
            if (await context.Roles.AnyAsync()) return;

            var roles = new List<Role>
            {
                new Role { Id = Guid.Parse("C6E837B8-626F-4905-86EB-C0D09CE03CCB"), Name = "Administrator", Description = "Quản trị hệ thống" },
                new Role { Id = Guid.Parse("53509FBF-A401-4ADD-A7AB-43A66BE7F0D1"), Name = "Staff", Description = "Nhân viên bán hàng" },
                new Role { Id = Guid.Parse("9F21385E-AC62-448F-91DD-04296F09C354"), Name = "Customer", Description = "Khách hàng" },
                new Role { Id = Guid.Parse("7941149C-5AE8-46F6-B599-CA49E635CB84"), Name = "Store Manager", Description = "Quản lý cửa hàng" },
            };

            await context.Roles.AddRangeAsync(roles);
            await context.SaveChangesAsync();
        }
    }
}
