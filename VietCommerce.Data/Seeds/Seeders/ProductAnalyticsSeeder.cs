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
using VietCommerce.Core.Enums.Products;
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
                    await SeedProductImagesAsync();
                    await SeedProductPricesAsync();
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
                },
                new Product
                {
                    Id = Guid.Parse("00000000-0000-0000-0000-000000001005"),
                    StoreId = storeId,
                    CategoryId = categoryBanTho,
                    Name = "Bàn thờ gỗ gụ chạm sen cổ điển",
                    SKU = "BTGG002",
                    Code = "BT02",
                    Slug = "ban-tho-go-gu-cham-sen-co-dien",
                    Stock = 25,
                    IsActive = true,
                    IsDeleted = false,
                    ViewCount = 410,
                    FavoriteCount = 60,
                    PurchaseCount = 22,
                    ReviewCount = 9,
                    AvgRating = 4.6m,
                    TrendingScore = 72.8m,
                    StatsUpdatedAt = now,
                    CreatedAt = now,
                    UpdatedAt = now
                },
                new Product
                {
                    Id = Guid.Parse("00000000-0000-0000-0000-000000001006"),
                    StoreId = storeId,
                    CategoryId = categoryBanTho,
                    Name = "Tủ thờ gỗ lim hoa văn rồng mây",
                    SKU = "TTGL001",
                    Code = "TT01",
                    Slug = "tu-tho-go-lim-hoa-van-rong-may",
                    Stock = 15,
                    IsActive = true,
                    IsDeleted = false,
                    ViewCount = 500,
                    FavoriteCount = 75,
                    PurchaseCount = 30,
                    ReviewCount = 14,
                    AvgRating = 4.9m,
                    TrendingScore = 95.2m,
                    StatsUpdatedAt = now,
                    CreatedAt = now,
                    UpdatedAt = now
                },
                new Product
                {
                    Id = Guid.Parse("00000000-0000-0000-0000-000000001007"),
                    StoreId = storeId,
                    CategoryId = categoryBatHuong,
                    Name = "Bộ đồ thờ gốm sứ Bát Tràng cao cấp",
                    SKU = "BDTBT001",
                    Code = "BD01",
                    Slug = "bo-do-tho-gom-su-bat-trang-cao-cap",
                    Stock = 80,
                    IsActive = true,
                    IsDeleted = false,
                    ViewCount = 300,
                    FavoriteCount = 40,
                    PurchaseCount = 20,
                    ReviewCount = 7,
                    AvgRating = 4.5m,
                    TrendingScore = 68.1m,
                    StatsUpdatedAt = now,
                    CreatedAt = now,
                    UpdatedAt = now
                },
                new Product
                {
                    Id = Guid.Parse("00000000-0000-0000-0000-000000001008"),
                    StoreId = storeId,
                    CategoryId = categoryBatHuong,
                    Name = "Chân nến đồng vàng chạm nổi rồng phượng",
                    SKU = "CNDV001",
                    Code = "CN01",
                    Slug = "chan-nen-dong-vang-cham-noi-rong-phuong",
                    Stock = 60,
                    IsActive = true,
                    IsDeleted = false,
                    ViewCount = 290,
                    FavoriteCount = 42,
                    PurchaseCount = 18,
                    ReviewCount = 6,
                    AvgRating = 4.4m,
                    TrendingScore = 63.7m,
                    StatsUpdatedAt = now,
                    CreatedAt = now,
                    UpdatedAt = now
                },
                new Product
                {
                    Id = Guid.Parse("00000000-0000-0000-0000-000000001009"),
                    StoreId = storeId,
                    CategoryId = categoryDenDau,
                    Name = "Đèn nến thủy tinh hình sen",
                    SKU = "DNHT001",
                    Code = "DN01",
                    Slug = "den-nen-thuy-tinh-hinh-sen",
                    Stock = 120,
                    IsActive = true,
                    IsDeleted = false,
                    ViewCount = 230,
                    FavoriteCount = 38,
                    PurchaseCount = 19,
                    ReviewCount = 8,
                    AvgRating = 4.6m,
                    TrendingScore = 70.3m,
                    StatsUpdatedAt = now,
                    CreatedAt = now,
                    UpdatedAt = now
                },
                new Product
                {
                    Id = Guid.Parse("00000000-0000-0000-0000-000000001010"),
                    StoreId = storeId,
                    CategoryId = categoryHoaQua,
                    Name = "Bình hoa sen giả lụa thờ cúng cao cấp",
                    SKU = "BHSL001",
                    Code = "BH02",
                    Slug = "binh-hoa-sen-gia-lua-tho-cung",
                    Stock = 90,
                    IsActive = true,
                    IsDeleted = false,
                    ViewCount = 340,
                    FavoriteCount = 55,
                    PurchaseCount = 24,
                    ReviewCount = 11,
                    AvgRating = 4.7m,
                    TrendingScore = 80.9m,
                    StatsUpdatedAt = now,
                    CreatedAt = now,
                    UpdatedAt = now
                },
                new Product
                {
                    Id = Guid.Parse("00000000-0000-0000-0000-000000001011"),
                    StoreId = storeId,
                    CategoryId = categoryTuongPhat,
                    Name = "Tượng Quan Âm Bồ Tát gỗ trắc đứng đài sen",
                    SKU = "TQAB001",
                    Code = "TQ01",
                    Slug = "tuong-quan-am-bo-tat-go-trac-dung-dai-sen",
                    Stock = 35,
                    IsActive = true,
                    IsDeleted = false,
                    ViewCount = 480,
                    FavoriteCount = 70,
                    PurchaseCount = 29,
                    ReviewCount = 13,
                    AvgRating = 4.9m,
                    TrendingScore = 96.4m,
                    StatsUpdatedAt = now,
                    CreatedAt = now,
                    UpdatedAt = now
                },
                new Product
                {
                    Id = Guid.Parse("00000000-0000-0000-0000-000000001012"),
                    StoreId = storeId,
                    CategoryId = categoryTuongPhat,
                    Name = "Tượng Phật Di Lặc gỗ hương dáng ngồi",
                    SKU = "TPDL001",
                    Code = "TP02",
                    Slug = "tuong-phat-di-lac-go-huong-dang-ngoi",
                    Stock = 40,
                    IsActive = true,
                    IsDeleted = false,
                    ViewCount = 510,
                    FavoriteCount = 85,
                    PurchaseCount = 38,
                    ReviewCount = 17,
                    AvgRating = 4.8m,
                    TrendingScore = 97.7m,
                    StatsUpdatedAt = now,
                    CreatedAt = now,
                    UpdatedAt = now
                },
                new Product
                {
                    Id = Guid.Parse("00000000-0000-0000-0000-000000001013"),
                    StoreId = storeId,
                    CategoryId = categoryDenDau,
                    Name = "Đèn thờ đồng cổ chạm hoa văn cổ điển",
                    SKU = "DDC002",
                    Code = "DD02",
                    Slug = "den-tho-dong-co-cham-hoa-van-co-dien",
                    Stock = 110,
                    IsActive = true,
                    IsDeleted = false,
                    ViewCount = 270,
                    FavoriteCount = 45,
                    PurchaseCount = 21,
                    ReviewCount = 10,
                    AvgRating = 4.5m,
                    TrendingScore = 74.5m,
                    StatsUpdatedAt = now,
                    CreatedAt = now,
                    UpdatedAt = now
                },
                new Product
                {
                    Id = Guid.Parse("00000000-0000-0000-0000-000000001014"),
                    StoreId = storeId,
                    CategoryId = categoryHoaQua,
                    Name = "Đĩa ngũ quả thờ cúng gốm men lam",
                    SKU = "DNQ001",
                    Code = "DQ01",
                    Slug = "dia-ngu-qua-tho-cung-gom-men-lam",
                    Stock = 75,
                    IsActive = true,
                    IsDeleted = false,
                    ViewCount = 320,
                    FavoriteCount = 52,
                    PurchaseCount = 27,
                    ReviewCount = 12,
                    AvgRating = 4.6m,
                    TrendingScore = 82.1m,
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
            var tenantId = Guid.Parse("F40EC7E0-FC21-4E67-831C-07D14D0B304A");
            var now = DateTime.UtcNow;

            var productIds = new[]
            {
                Guid.Parse("00000000-0000-0000-0000-000000001000"),
                Guid.Parse("00000000-0000-0000-0000-000000001001"),
                Guid.Parse("00000000-0000-0000-0000-000000001002"),
                Guid.Parse("00000000-0000-0000-0000-000000001003"),
                Guid.Parse("00000000-0000-0000-0000-000000001004"),
                Guid.Parse("00000000-0000-0000-0000-000000001005"),
                Guid.Parse("00000000-0000-0000-0000-000000001006"),
                Guid.Parse("00000000-0000-0000-0000-000000001007"),
                Guid.Parse("00000000-0000-0000-0000-000000001008"),
                Guid.Parse("00000000-0000-0000-0000-000000001009"),
                Guid.Parse("00000000-0000-0000-0000-000000001010"),
                Guid.Parse("00000000-0000-0000-0000-000000001011"),
                Guid.Parse("00000000-0000-0000-0000-000000001012"),
                Guid.Parse("00000000-0000-0000-0000-000000001013"),
                Guid.Parse("00000000-0000-0000-0000-000000001014")
            };

            var inventories = productIds.Select(pid => new Inventory
            {
                Id = Guid.NewGuid(),
                StoreId = storeId,
                ProductId = pid,
                TenantId = tenantId,
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
        // PRODUCT IMAGE SEEDER
        // =======================================================
        private async Task SeedProductImagesAsync()
        {
            if (await _context.ProductImages.AnyAsync())
            {
                Console.WriteLine("⏭️ Skipped ProductImages: already exist.");
                return;
            }

            var imageUrls = new Dictionary<Guid, string>
            {
                { Guid.Parse("00000000-0000-0000-0000-000000001000"), "https://example.com/images/ban-tho-go-mit.jpg" },
                { Guid.Parse("00000000-0000-0000-0000-000000001001"), "https://example.com/images/bat-huong-men-ran.jpg" },
                { Guid.Parse("00000000-0000-0000-0000-000000001002"), "https://example.com/images/den-dau-dong-co.jpg" },
                { Guid.Parse("00000000-0000-0000-0000-000000001003"), "https://example.com/images/hoa-sen-go.jpg" },
                { Guid.Parse("00000000-0000-0000-0000-000000001004"), "https://example.com/images/tuong-phat-a-di-da.jpg" },
                { Guid.Parse("00000000-0000-0000-0000-000000001005"), "https://example.com/images/ban-tho-go-gu-sen.jpg" },
                { Guid.Parse("00000000-0000-0000-0000-000000001006"), "https://example.com/images/tu-tho-go-lim.jpg" },
                { Guid.Parse("00000000-0000-0000-0000-000000001007"), "https://example.com/images/bo-do-tho-bat-trang.jpg" },
                { Guid.Parse("00000000-0000-0000-0000-000000001008"), "https://example.com/images/chan-nen-dong-vang.jpg" },
                { Guid.Parse("00000000-0000-0000-0000-000000001009"), "https://example.com/images/den-nen-thuy-tinh.jpg" },
                { Guid.Parse("00000000-0000-0000-0000-000000001010"), "https://example.com/images/binh-hoa-sen-gia.jpg" },
                { Guid.Parse("00000000-0000-0000-0000-000000001011"), "https://example.com/images/tuong-quan-am-bo-tat.jpg" },
                { Guid.Parse("00000000-0000-0000-0000-000000001012"), "https://example.com/images/tuong-phat-di-lac.jpg" },
                { Guid.Parse("00000000-0000-0000-0000-000000001013"), "https://example.com/images/den-tho-dong-co-2.jpg" },
                { Guid.Parse("00000000-0000-0000-0000-000000001014"), "https://example.com/images/dia-ngu-qua.jpg" }
            };

            var images = imageUrls.Select(kvp => new ProductImage
            {
                Id = Guid.NewGuid(),
                ProductId = kvp.Key,
                Url = kvp.Value,
                IsMain = true,
                CreatedAt = DateTime.UtcNow
            }).ToList();

            await _context.ProductImages.AddRangeAsync(images);
            Console.WriteLine($"   + Seeded {images.Count} product images ✅");
        }

        // =======================================================
        // PRODUCT PRICE SEEDER (REGULAR + PROMOTION)
        // =======================================================
        private async Task SeedProductPricesAsync()
        {
            if (await _context.ProductPrices.AnyAsync())
            {
                Console.WriteLine("⏭️ Skipped ProductPrices: already exist.");
                return;
            }

            var now = DateTime.UtcNow;
            var prices = new List<ProductPrice>();

            // Định nghĩa giá REGULAR và PROMOTION cho mỗi sản phẩm
            var priceData = new List<(Guid ProductId, decimal RegularPrice, decimal PromotionPrice)>
            {
                (Guid.Parse("00000000-0000-0000-0000-000000001000"), 8500000m, 7200000m),
                (Guid.Parse("00000000-0000-0000-0000-000000001001"), 1250000m, 990000m),
                (Guid.Parse("00000000-0000-0000-0000-000000001002"), 3200000m, 2560000m),
                (Guid.Parse("00000000-0000-0000-0000-000000001003"), 850000m, 680000m),
                (Guid.Parse("00000000-0000-0000-0000-000000001004"), 5500000m, 4400000m),
                (Guid.Parse("00000000-0000-0000-0000-000000001005"), 7200000m, 5760000m),
                (Guid.Parse("00000000-0000-0000-0000-000000001006"), 12000000m, 9600000m),
                (Guid.Parse("00000000-0000-0000-0000-000000001007"), 2500000m, 2000000m),
                (Guid.Parse("00000000-0000-0000-0000-000000001008"), 1800000m, 1440000m),
                (Guid.Parse("00000000-0000-0000-0000-000000001009"), 650000m, 520000m),
                (Guid.Parse("00000000-0000-0000-0000-000000001010"), 1100000m, 880000m),
                (Guid.Parse("00000000-0000-0000-0000-000000001011"), 6500000m, 5200000m),
                (Guid.Parse("00000000-0000-0000-0000-000000001012"), 4200000m, 3360000m),
                (Guid.Parse("00000000-0000-0000-0000-000000001013"), 2800000m, 2240000m),
                (Guid.Parse("00000000-0000-0000-0000-000000001014"), 950000m, 760000m)
            };

            // Tạo giá REGULAR và PROMOTION cho mỗi sản phẩm
            foreach (var (productId, regularPrice, promotionPrice) in priceData)
            {
                // Giá REGULAR
                prices.Add(new ProductPrice
                {
                    Id = Guid.NewGuid(),
                    ProductId = productId,
                    PriceType = PriceType.REGULAR,
                    Price = regularPrice,
                    EffectiveFrom = now,
                    CreatedAt = now,
                    IsActive = true
                });

                // Giá PROMOTION (rẻ hơn)
                prices.Add(new ProductPrice
                {
                    Id = Guid.NewGuid(),
                    ProductId = productId,
                    PriceType = PriceType.FLASH_SALE,
                    Price = promotionPrice,
                    EffectiveFrom = now,
                    CreatedAt = now,
                    IsActive = true
                });
            }

            await _context.ProductPrices.AddRangeAsync(prices);
            Console.WriteLine($"   + Seeded {prices.Count} product prices (REGULAR + PROMOTION) ✅");
        }

        // =======================================================
        // PRODUCT REVIEW SEEDER
        // =======================================================
        private async Task SeedProductReviewsAsync()
        {
            if (await _context.ProductReviews.AnyAsync())
            {
                Console.WriteLine("⏭️ Skipped ProductReviews: already exist.");
                return;
            }

            var now = DateTime.UtcNow;
            var demoUserId = Guid.Parse("00000000-0000-0000-0000-000000009999");

            var productReviews = new List<(Guid ProductId, string Comment, int Rating)>
            {
                (Guid.Parse("00000000-0000-0000-0000-000000001000"), "Bàn thờ rất đẹp, chạm khắc tỉ mỉ, chất lượng gỗ mít tuyệt vời!", 5),
                (Guid.Parse("00000000-0000-0000-0000-000000001001"), "Bát hương men rạn rất tinh xảo, màu sắc đẹp mắt, kích thước phù hợp", 5),
                (Guid.Parse("00000000-0000-0000-0000-000000001002"), "Đèn dầu đồng cổ chất lượng tốt, chiếu sáng đều, không bay khí xấu", 5),
                (Guid.Parse("00000000-0000-0000-0000-000000001003"), "Hoa sen gỗ đẹp lắm, điêu khắc tinh tế, bền lâu", 4),
                (Guid.Parse("00000000-0000-0000-0000-000000001004"), "Tượng Phật A Di Đà gỗ hương mùi thơm nhẹ, tỉ mỉ từng chi tiết", 5),
                (Guid.Parse("00000000-0000-0000-0000-000000001005"), "Bàn thờ gỗ gụ chạm sen cổ điển rất sang trọng, giá hợp lý", 4),
                (Guid.Parse("00000000-0000-0000-0000-000000001006"), "Tủ thờ gỗ lim tuyệt vời, hoa văn rồng mây rất sắc sảo!", 5),
                (Guid.Parse("00000000-0000-0000-0000-000000001007"), "Bộ đồ thờ Bát Tràng cao cấp, màu xanh lam đẹp, chất ceramic tốt", 4),
                (Guid.Parse("00000000-0000-0000-0000-000000001008"), "Chân nến đồng vàng chạm nổi rồng phượng rất đẹp, bền", 4),
                (Guid.Parse("00000000-0000-0000-0000-000000001009"), "Đèn nến thủy tinh hình sen thanh lịch, ánh sáng ấm áp", 4),
                (Guid.Parse("00000000-0000-0000-0000-000000001010"), "Bình hoa sen giả lụa trông như thật, không cần chăm sóc", 5),
                (Guid.Parse("00000000-0000-0000-0000-000000001011"), "Tượng Quan Âm Bồ Tát gỗ trắc đẹp tuyệt vời, phù điêu tỉ mỉ", 5),
                (Guid.Parse("00000000-0000-0000-0000-000000001012"), "Tượng Phật Di Lặc gỗ hương dáng ngồi tươi cười hạnh phúc", 5),
                (Guid.Parse("00000000-0000-0000-0000-000000001013"), "Đèn thờ đồng cổ chạm hoa văn rất cổ điển, chất lượng cao", 4),
                (Guid.Parse("00000000-0000-0000-0000-000000001014"), "Đĩa ngũ quả gốm men lam tuyệt đẹp, thích hợp để cúng lễ", 5)
            };

            var reviews = new List<ProductReview>();
            var helpfulCounts = new[] { 3, 5, 4, 2, 6, 3, 5, 2, 3, 4, 5, 6, 7, 4, 5 };

            for (int i = 0; i < productReviews.Count; i++)
            {
                var (productId, comment, rating) = productReviews[i];
                reviews.Add(new ProductReview
                {
                    Id = Guid.NewGuid(),
                    ProductId = productId,
                    UserId = demoUserId,
                    OrderItemId = Guid.NewGuid(),
                    Rating = rating,
                    Comment = comment,
                    IsVisible = true,
                    HelpfulCount = helpfulCounts[i],
                    CreatedAt = now.AddDays(-i)
                });
            }

            await _context.ProductReviews.AddRangeAsync(reviews);
            Console.WriteLine($"   + Seeded {reviews.Count} product reviews ✅");
        }

        // =======================================================
        // PRODUCT VIEW SEEDER
        // =======================================================
        private async Task SeedProductViewsAsync()
        {
            if (await _context.ProductViews.AnyAsync())
            {
                Console.WriteLine("⏭️ Skipped ProductViews: already exist.");
                return;
            }

            var productIds = new[]
            {
                Guid.Parse("00000000-0000-0000-0000-000000001000"),
                Guid.Parse("00000000-0000-0000-0000-000000001001"),
                Guid.Parse("00000000-0000-0000-0000-000000001002"),
                Guid.Parse("00000000-0000-0000-0000-000000001003"),
                Guid.Parse("00000000-0000-0000-0000-000000001004"),
                Guid.Parse("00000000-0000-0000-0000-000000001005"),
                Guid.Parse("00000000-0000-0000-0000-000000001006"),
                Guid.Parse("00000000-0000-0000-0000-000000001007"),
                Guid.Parse("00000000-0000-0000-0000-000000001008"),
                Guid.Parse("00000000-0000-0000-0000-000000001009"),
                Guid.Parse("00000000-0000-0000-0000-000000001010"),
                Guid.Parse("00000000-0000-0000-0000-000000001011"),
                Guid.Parse("00000000-0000-0000-0000-000000001012"),
                Guid.Parse("00000000-0000-0000-0000-000000001013"),
                Guid.Parse("00000000-0000-0000-0000-000000001014")
            };

            var views = new List<ProductView>();
            var now = DateTime.UtcNow;

            // Tạo nhiều view cho mỗi sản phẩm (giả lập lịch sử xem)
            foreach (var productId in productIds)
            {
                for (int i = 0; i < 3; i++)
                {
                    views.Add(new ProductView
                    {
                        Id = Guid.NewGuid(),
                        ProductId = productId,
                        SessionId = Guid.NewGuid().ToString(),
                        ViewedAt = now.AddHours(-i)
                    });
                }
            }

            await _context.ProductViews.AddRangeAsync(views);
            Console.WriteLine($"   + Seeded {views.Count} product views ✅");
        }

        // =======================================================
        // PRODUCT FAVORITE SEEDER
        // =======================================================
        private async Task SeedProductFavoritesAsync()
        {
            // Placeholder cho ProductFavorites seeder
            // Có thể được mở rộng để thêm dữ liệu favorite
            Console.WriteLine("   ℹ️ ProductFavorites: sẵn sàng (chưa implement)");
            await Task.CompletedTask;
        }
    }
}