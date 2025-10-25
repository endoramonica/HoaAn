// ================================================================
// FILE: ProductAnalyticsSeeder.cs
// Author: VietCommerce Seeder Team
// Purpose: Seed Categories, Products, Inventories, Views, Favorites, Reviews
// Context: Ngành hàng ĐỒ THỜ CÚNG
// ================================================================

using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VietCommerce.Core.Entities.Products;
using VietCommerce.Data.Context;

namespace VietCommerce.Data.Seeds.Seeders
{
    public class ProductAnalyticsSeeder
    {
        private readonly AppDbContext _context;

        public ProductAnalyticsSeeder(AppDbContext context)
        {
            _context = context;
        }

        public async Task SeedAsync()
        {
            Console.WriteLine("🚀 Starting ProductAnalyticsSeeder (Đồ Thờ Cúng)...");

            try
            {
                await SeedCategoriesAsync();

                if (!await _context.Products.AnyAsync())
                {
                    await SeedProductsAsync();
                    await SeedInventoriesAsync();
                    await SeedProductViewsAsync();
                    await SeedProductFavoritesAsync();
                    await SeedProductReviewsAsync();
                    await _context.SaveChangesAsync();

                    Console.WriteLine("✅ ProductAnalyticsSeeder: Seed đồ thờ cúng hoàn tất!");
                }
                else
                {
                    Console.WriteLine("⏭️ Bỏ qua Products: đã tồn tại.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ ProductAnalyticsSeeder Error: {ex.Message}");
                throw;
            }
        }

        // =======================================================
        // CATEGORY SEEDER
        // =======================================================
        private async Task SeedCategoriesAsync()
        {
            if (await _context.Categories.AnyAsync())
            {
                Console.WriteLine("⏭️ Skipped Categories: already exist.");
                return;
            }

            var now = DateTime.UtcNow;
            var tenantId = Guid.Parse("F40EC7E0-FC21-4E67-831C-07D14D0B304A");
            var storeId = Guid.Parse("47AA5519-C503-4CFA-8101-2EDB36FD9D8C");

            var categories = new List<Category>
            {
                new Category
                {
                    Id = Guid.Parse("00000000-0000-0000-0000-000000000010"),
                    StoreId = storeId,
                    TenantId = tenantId,
                    Name = "Bàn thờ & Tủ thờ",
                    IsActive = true,
                    IsDeleted = false,
                    CreatedAt = now,
                    UpdatedAt = now
                },
                new Category
                {
                    Id = Guid.Parse("00000000-0000-0000-0000-000000000011"),
                    StoreId = storeId,
                    TenantId = tenantId,
                    Name = "Bát hương & Bộ đồ thờ",
                    IsActive = true,
                    IsDeleted = false,
                    CreatedAt = now,
                    UpdatedAt = now
                },
                new Category
                {
                    Id = Guid.Parse("00000000-0000-0000-0000-000000000012"),
                    StoreId = storeId,
                    TenantId = tenantId,
                    Name = "Đèn dầu, nến & phụ kiện thờ cúng",
                    IsActive = true,
                    IsDeleted = false,
                    CreatedAt = now,
                    UpdatedAt = now
                },
                new Category
                {
                    Id = Guid.Parse("00000000-0000-0000-0000-000000000013"),
                    StoreId = storeId,
                    TenantId = tenantId,
                    Name = "Hoa, quả & đồ cúng lễ",
                    IsActive = true,
                    IsDeleted = false,
                    CreatedAt = now,
                    UpdatedAt = now
                },
                new Category
                {
                    Id = Guid.Parse("00000000-0000-0000-0000-000000000014"),
                    StoreId = storeId,
                    TenantId = tenantId,
                    Name = "Tượng Phật & đồ trang trí tâm linh",
                    IsActive = true,
                    IsDeleted = false,
                    CreatedAt = now,
                    UpdatedAt = now
                }
            };

            await _context.Categories.AddRangeAsync(categories);
            await _context.SaveChangesAsync();

            Console.WriteLine($"   + Seeded {categories.Count} danh mục Đồ thờ cúng thành công ✅");
        }

        // =======================================================
        // PRODUCT SEEDER
        // =======================================================
        private async Task SeedProductsAsync()
        {
            var now = DateTime.UtcNow;
            var tenantId = Guid.Parse("F40EC7E0-FC21-4E67-831C-07D14D0B304A");
            var storeId = Guid.Parse("47AA5519-C503-4CFA-8101-2EDB36FD9D8C");

            var categoryBanTho = Guid.Parse("00000000-0000-0000-0000-000000000010");
            var categoryBatHuong = Guid.Parse("00000000-0000-0000-0000-000000000011");
            var categoryDenDau = Guid.Parse("00000000-0000-0000-0000-000000000012");
            var categoryHoaQua = Guid.Parse("00000000-0000-0000-0000-000000000013");
            var categoryTuongPhat = Guid.Parse("00000000-0000-0000-0000-000000000014");

            var products = new List<Product>
            {
                new Product
                {
                    Id = Guid.Parse("00000000-0000-0000-0000-000000001000"),
                    StoreId = storeId,
                  //  TenantId = tenantId,
                    CategoryId = categoryBanTho,
                    Name = "Bàn thờ gỗ mít chạm rồng phượng",
                    SKU = "BTGM001",
                    Code = "BT01",
                    Slug = "ban-tho-go-mit-cham-rong-phuong",
                    Stock = 30,
                    IsActive = true,
                    IsDeleted = false,
                    ViewCount = 500,
                    FavoriteCount = 80,
                    PurchaseCount = 25,
                    ReviewCount = 10,
                    AvgRating = 4.8m,
                    TrendingScore = 90.5m,
                    StatsUpdatedAt = now,
                    CreatedAt = now,
                    UpdatedAt = now
                },
                new Product
                {
                    Id = Guid.Parse("00000000-0000-0000-0000-000000001001"),
                    StoreId = storeId,
                   // TenantId = tenantId,
                    CategoryId = categoryBatHuong,
                    Name = "Bát hương men rạn đắp nổi Long Phụng",
                    SKU = "BHMR001",
                    Code = "BH01",
                    Slug = "bat-huong-men-ran-dap-noi-long-phung",
                    Stock = 100,
                    IsActive = true,
                    IsDeleted = false,
                    ViewCount = 350,
                    FavoriteCount = 60,
                    PurchaseCount = 40,
                    ReviewCount = 15,
                    AvgRating = 4.7m,
                    TrendingScore = 85.3m,
                    StatsUpdatedAt = now,
                    CreatedAt = now,
                    UpdatedAt = now
                },
                new Product
                {
                    Id = Guid.Parse("00000000-0000-0000-0000-000000001002"),
                    StoreId = storeId,
                  //  TenantId = tenantId,
                    CategoryId = categoryDenDau,
                    Name = "Đèn dầu đồng cổ cao cấp",
                    SKU = "DDDC001",
                    Code = "DD01",
                    Slug = "den-dau-dong-co-cao-cap",
                    Stock = 200,
                    IsActive = true,
                    IsDeleted = false,
                    ViewCount = 220,
                    FavoriteCount = 45,
                    PurchaseCount = 30,
                    ReviewCount = 12,
                    AvgRating = 4.6m,
                    TrendingScore = 78.2m,
                    StatsUpdatedAt = now,
                    CreatedAt = now,
                    UpdatedAt = now
                },
                new Product
                {
                    Id = Guid.Parse("00000000-0000-0000-0000-000000001003"),
                    StoreId = storeId,
                  //  TenantId = tenantId,
                    CategoryId = categoryHoaQua,
                    Name = "Hoa sen gỗ cúng lễ cao cấp",
                    SKU = "HSG001",
                    Code = "HS01",
                    Slug = "hoa-sen-go-cung-le-cao-cap",
                    Stock = 150,
                    IsActive = true,
                    IsDeleted = false,
                    ViewCount = 180,
                    FavoriteCount = 30,
                    PurchaseCount = 20,
                    ReviewCount = 8,
                    AvgRating = 4.5m,
                    TrendingScore = 65.4m,
                    StatsUpdatedAt = now,
                    CreatedAt = now,
                    UpdatedAt = now
                },
                new Product
                {
                    Id = Guid.Parse("00000000-0000-0000-0000-000000001004"),
                    StoreId = storeId,
                    //TenantId = tenantId,
                    CategoryId = categoryTuongPhat,
                    Name = "Tượng Phật A Di Đà gỗ hương",
                    SKU = "TPAD001",
                    Code = "TP01",
                    Slug = "tuong-phat-a-di-da-go-huong",
                    Stock = 50,
                    IsActive = true,
                    IsDeleted = false,
                    ViewCount = 400,
                    FavoriteCount = 70,
                    PurchaseCount = 35,
                    ReviewCount = 20,
                    AvgRating = 4.9m,
                    TrendingScore = 98.1m,
                    StatsUpdatedAt = now,
                    CreatedAt = now,
                    UpdatedAt = now
                }
            };

            await _context.Products.AddRangeAsync(products);
            Console.WriteLine($"   + Seeded {products.Count} sản phẩm đồ thờ cúng ✅");
        }

        // =======================================================
        // INVENTORY SEEDER
        // =======================================================
        private async Task SeedInventoriesAsync()
        {
            var storeId = Guid.Parse("47AA5519-C503-4CFA-8101-2EDB36FD9D8C");
            var tenantId = Guid.Parse("F40EC7E0-FC21-4E67-831C-07D14D0B304A"); // 🆕 Hardcode TenantId
            var now = DateTime.UtcNow;

            var productIds = new[]
            {
        Guid.Parse("00000000-0000-0000-0000-000000001000"),
        Guid.Parse("00000000-0000-0000-0000-000000001001"),
        Guid.Parse("00000000-0000-0000-0000-000000001002"),
        Guid.Parse("00000000-0000-0000-0000-000000001003"),
        Guid.Parse("00000000-0000-0000-0000-000000001004")
    };

            var inventories = productIds.Select(pid => new Inventory
            {
                Id = Guid.NewGuid(),
                StoreId = storeId,
                ProductId = pid,
                TenantId = tenantId, // ✅ Gán TenantId cố định
                QuantityAvailable = 100,
                ReorderLevel = 10,
                IsDeleted = false,
                IsActive = true,
                CreatedAt = now
            }).ToList();

            await _context.Inventories.AddRangeAsync(inventories);
            Console.WriteLine($"   + Seeded {inventories.Count} inventories ✅");
        }

        

        // =======================================================
        // PRODUCT ANALYTICS PLACEHOLDERS
        // =======================================================
        private async Task SeedProductViewsAsync() { /* Giữ chỗ */ }
        private async Task SeedProductFavoritesAsync() { /* Giữ chỗ */ }
        private async Task SeedProductReviewsAsync() { /* Giữ chỗ */ }
    }
}
