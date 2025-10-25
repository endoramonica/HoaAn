using VietCommerce.Core.Entities.Products;
using VietCommerce.Core.Entities.Organization;
using VietCommerce.Core.Entities.Users;
using VietCommerce.Data.Context;
using VietCommerce.Core.Enums.Products;

namespace VietCommerce.Tests.SeedData;

public static class TestDataSeeder
{
    public static async Task SeedAsync(AppDbContext context)
    {
        // Nếu đã có dữ liệu, không seed lại
        if (context.Categories.Any() || context.Products.Any())
            return;

        var now = DateTime.UtcNow;
        var storeId = Guid.Parse("47AA5519-C503-4CFA-8101-2EDB36FD9D8C");
        var tenantId = Guid.Parse("F40EC7E0-FC21-4E67-831C-07D14D0B304A");

        // ✅ 1. Seed user tối thiểu (Creator)
        var testUser = new User
        {
            Id = Guid.Parse("11111111-1111-1111-1111-111111111111"),
            Email = "tester@vietcommerce.local",
            Name = "Test User",
            CreatedAt = now,
            UpdatedAt = now
        };
        await context.Users.AddAsync(testUser);

        // ✅ 2. Seed store
        var store = new Store
        {
            Id = storeId,
            TenantId = tenantId,
            Name = "Cửa hàng VietCommerce Test",
            CreatedAt = now,
            UpdatedAt = now
        };
        await context.Stores.AddAsync(store);

        // ✅ 3. Seed category
        var category1 = new Category
        {
            Id = Guid.Parse("00000000-0000-0000-0000-000000000010"),
            StoreId = storeId,
            TenantId = tenantId,
            Name = "Đồ thờ",
            CreatedAt = now,
            UpdatedAt = now
        };

        var category2 = new Category
        {
            Id = Guid.Parse("00000000-0000-0000-0000-000000000011"),
            StoreId = storeId,
            TenantId = tenantId,
            Name = "Bàn thờ",
            CreatedAt = now,
            UpdatedAt = now
        };

        await context.Categories.AddRangeAsync(category1, category2);

        // ✅ 4. Seed products
        var product1 = new Product
        {
            Id = Guid.NewGuid(),
            StoreId = storeId,
            CategoryId = category1.Id,
            Name = "Bát hương men rạn",
            Code = "BH01",
            Slug = "bat-huong-men-ran",
            SKU = "SKU-BH01",
            Stock = 20,
            IsActive = true,
            ViewCount = 100,
            CreatedAt = now,
            UpdatedAt = now
        };

        var product2 = new Product
        {
            Id = Guid.NewGuid(),
            StoreId = storeId,
            CategoryId = category2.Id,
            Name = "Bàn thờ gỗ mít",
            Code = "BT01",
            Slug = "ban-tho-go-mit",
            SKU = "SKU-BT01",
            Stock = 10,
            IsActive = true,
            ViewCount = 200,
            CreatedAt = now,
            UpdatedAt = now
        };

        await context.Products.AddRangeAsync(product1, product2);
        // ✅ 4.1 Seed Inventories cho 2 sản phẩm
        //
        var inventory1 = new Inventory
        {
            Id = Guid.NewGuid(),
            ProductId = product1.Id,
            QuantityAvailable = 20,
            IsDeleted = false,
            CreatedAt = now,
            UpdatedAt = now
        };

        var inventory2 = new Inventory
        {
            Id = Guid.NewGuid(),
            ProductId = product2.Id,
            QuantityAvailable = 10,
            IsDeleted = false,
            CreatedAt = now,
            UpdatedAt = now
        };

        await context.Inventories.AddRangeAsync(inventory1, inventory2);

        // ✅ 5. Seed ProductPrices
        var prices = new List<ProductPrice>
        {
            new()
            {
                Id = Guid.NewGuid(),
                ProductId = product1.Id,
                PriceType = PriceType.REGULAR,
                Price = 500000m,
                EffectiveFrom = now.AddDays(-7),
                EffectiveTo = null,
                IsActive = true,
                CreatedAt = now,
                UpdatedAt = now,
                CreatedBy = testUser.Id,
                Product = product1,
                Creator = testUser
            },
            new()
            {
                Id = Guid.NewGuid(),
                ProductId = product2.Id,
                PriceType = PriceType.REGULAR,
                Price = 3000000m,
                EffectiveFrom = now.AddDays(-7),
                EffectiveTo = null,
                IsActive = true,
                CreatedAt = now,
                UpdatedAt = now,
                CreatedBy = testUser.Id,
                Product = product2,
                Creator = testUser
            }
        };

        await context.ProductPrices.AddRangeAsync(prices);

        // ✅ 6. Lưu tất cả
        await context.SaveChangesAsync();
    }
}
