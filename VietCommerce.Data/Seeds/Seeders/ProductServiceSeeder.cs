using VietCommerce.Core.Entities.Products;
using VietCommerce.Data.Context;

namespace VietCommerce.Data.Seeders
{
    /// <summary>
    /// Seeder for Services-Product Unification
    /// Creates 6 sample services (one per category)
    /// </summary>
    public class ProductServiceSeeder
    {
        private readonly AppDbContext _context;

        public ProductServiceSeeder(AppDbContext context)
        {
            _context = context;
        }

        public async Task SeedAsync()
        {
            // Check if services already exist
            var existingServices = _context.Products
                .Where(p => p.Type == "service")
                .Count();

            if (existingServices > 0)
            {
                Console.WriteLine("✅ Services already seeded. Skipping...");
                return;
            }

            // Get default store (or create one)
            var store = _context.Stores.FirstOrDefault();
            if (store == null)
            {
                Console.WriteLine("⚠️ No store found. Cannot seed services.");
                return;
            }

            var services = new List<Product>
            {
                // 1. Ancestor Worship Service
                new Product
                {
                    Id = Guid.NewGuid(),
                    StoreId = store.Id,
                    Name = "Mâm Cúng Gia Tiên Trọn Gói Cao Cấp",
                    Code = "SVC-ANCESTOR-001",
                    SKU = "SKU-SVC-ANCESTOR-001",
                    Slug = "mam-cung-gia-tien-tron-goi-cao-cap",
                    Type = "service",
                    ServiceCategory = "ancestor-worship",
                    ServiceDuration = "2-3 giờ",
                    ServiceRating = 4.8m,
                    Stock = 100,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = Guid.Empty,
                    UpdatedAt = DateTime.UtcNow,
                    UpdatedBy = Guid.Empty
                },

                // 2. Opening Ceremony Service
                new Product
                {
                    Id = Guid.NewGuid(),
                    StoreId = store.Id,
                    Name = "Dịch Vụ Lễ Khai Trương Chuyên Nghiệp",
                    Code = "SVC-OPENING-001",
                    SKU = "SKU-SVC-OPENING-001",
                    Slug = "dich-vu-le-khai-truong-chuyen-nghiep",
                    Type = "service",
                    ServiceCategory = "opening-ceremony",
                    ServiceDuration = "4-6 giờ",
                    ServiceRating = 4.7m,
                    Stock = 100,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = Guid.Empty,
                    UpdatedAt = DateTime.UtcNow,
                    UpdatedBy = Guid.Empty
                },

                // 3. Wedding Service
                new Product
                {
                    Id = Guid.NewGuid(),
                    StoreId = store.Id,
                    Name = "Gói Dịch Vụ Lễ Cưới Hỏi Trọn Vẹn",
                    Code = "SVC-WEDDING-001",
                    SKU = "SKU-SVC-WEDDING-001",
                    Slug = "goi-dich-vu-le-cuoi-hoi-tron-ven",
                    Type = "service",
                    ServiceCategory = "wedding",
                    ServiceDuration = "8-10 giờ",
                    ServiceRating = 4.9m,
                    Stock = 100,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = Guid.Empty,
                    UpdatedAt = DateTime.UtcNow,
                    UpdatedBy = Guid.Empty
                },

                // 4. Buddha Worship Service
                new Product
                {
                    Id = Guid.NewGuid(),
                    StoreId = store.Id,
                    Name = "Mâm Cúng Phật Tây Phương Cao Cấp",
                    Code = "SVC-BUDDHA-001",
                    SKU = "SKU-SVC-BUDDHA-001",
                    Slug = "mam-cung-phat-tay-phuong-cao-cap",
                    Type = "service",
                    ServiceCategory = "buddha-worship",
                    ServiceDuration = "1-2 giờ",
                    ServiceRating = 4.6m,
                    Stock = 100,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = Guid.Empty,
                    UpdatedAt = DateTime.UtcNow,
                    UpdatedBy = Guid.Empty
                },

                // 5. New House Service
                new Product
                {
                    Id = Guid.NewGuid(),
                    StoreId = store.Id,
                    Name = "Lễ Tân Gia Trọn Gói Đầy Đủ",
                    Code = "SVC-NEWHOUSE-001",
                    SKU = "SKU-SVC-NEWHOUSE-001",
                    Slug = "le-tan-gia-tron-goi-day-du",
                    Type = "service",
                    ServiceCategory = "new-house",
                    ServiceDuration = "3-4 giờ",
                    ServiceRating = 4.7m,
                    Stock = 100,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = Guid.Empty,
                    UpdatedAt = DateTime.UtcNow,
                    UpdatedBy = Guid.Empty
                },

                // 6. Feng Shui Consultation Service
                new Product
                {
                    Id = Guid.NewGuid(),
                    StoreId = store.Id,
                    Name = "Tư Vấn Phong Thủy Chuyên Sâu",
                    Code = "SVC-FENGSHUI-001",
                    SKU = "SKU-SVC-FENGSHUI-001",
                    Slug = "tu-van-phong-thuy-chuyen-sau",
                    Type = "service",
                    ServiceCategory = "feng-shui-consultation",
                    ServiceDuration = "1-2 giờ",
                    ServiceRating = 4.8m,
                    Stock = 100,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = Guid.Empty,
                    UpdatedAt = DateTime.UtcNow,
                    UpdatedBy = Guid.Empty
                }
            };

            _context.Products.AddRange(services);
            await _context.SaveChangesAsync();

            Console.WriteLine($"✅ Successfully seeded {services.Count} services!");
        }
    }
}
