using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VietCommerce.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddRitualProductCatalogSeed : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Get default store and tenant IDs (assuming they exist)
            // These are placeholder GUIDs that should match your actual store/tenant setup
            var storeId = new Guid("00000000-0000-0000-0000-000000000001");
            var tenantId = new Guid("00000000-0000-0000-0000-000000000001");

            // Create categories for rituals
            var categoryIds = new[]
            {
                new Guid("00000000-0000-0000-0000-000000000001"), // Mâm Cúng & Vật Phẩm Cúng
                new Guid("00000000-0000-0000-0000-000000000002"), // Hoa & Quả
                new Guid("00000000-0000-0000-0000-000000000003"), // Tết - Hoa & Trang Trí
                new Guid("00000000-0000-0000-0000-000000000004"), // Tết - Bánh & Mứt
                new Guid("00000000-0000-0000-0000-000000000005"), // Lễ Cúng Tổ Tiên - Nến & Hương
                new Guid("00000000-0000-0000-0000-000000000006"), // Lễ Cúng Tổ Tiên - Thịt & Hải Sản
                new Guid("00000000-0000-0000-0000-000000000007"), // Lễ Cúng Thần Tài - Vàng & Tiền
                new Guid("00000000-0000-0000-0000-000000000008")  // Tết Trung Thu - Đèn Lồng & Bánh
            };

            // Insert categories
            migrationBuilder.InsertData(
                table: "Categories",
                columns: new[] { "Id", "StoreId", "TenantId", "Name", "ParentId", "IsActive", "IsDeleted", "CreatedAt", "CreatedBy", "UpdatedAt", "UpdatedBy" },
                values: new object[,]
                {
                    { categoryIds[0], storeId, tenantId, "Mâm Cúng & Vật Phẩm Cúng", null, true, false, DateTime.UtcNow, null, null, null },
                    { categoryIds[1], storeId, tenantId, "Hoa & Quả", null, true, false, DateTime.UtcNow, null, null, null },
                    { categoryIds[2], storeId, tenantId, "Tết - Hoa & Trang Trí", null, true, false, DateTime.UtcNow, null, null, null },
                    { categoryIds[3], storeId, tenantId, "Tết - Bánh & Mứt", null, true, false, DateTime.UtcNow, null, null, null },
                    { categoryIds[4], storeId, tenantId, "Lễ Cúng Tổ Tiên - Nến & Hương", null, true, false, DateTime.UtcNow, null, null, null },
                    { categoryIds[5], storeId, tenantId, "Lễ Cúng Tổ Tiên - Thịt & Hải Sản", null, true, false, DateTime.UtcNow, null, null, null },
                    { categoryIds[6], storeId, tenantId, "Lễ Cúng Thần Tài - Vàng & Tiền", null, true, false, DateTime.UtcNow, null, null, null },
                    { categoryIds[7], storeId, tenantId, "Tết Trung Thu - Đèn Lồng & Bánh", null, true, false, DateTime.UtcNow, null, null, null }
                }
            );

            // Insert products for Đầy Tháng (Category 1 & 2)
            migrationBuilder.InsertData(
                table: "Products",
                columns: new[] { "Id", "StoreId", "CategoryId", "Name", "Code", "Slug", "SKU", "Stock", "IsActive", "IsDeleted", "Type", "CreatedAt", "CreatedBy", "UpdatedAt", "UpdatedBy" },
                values: new object[,]
                {
                    { new Guid("00000000-0000-0000-0000-000000000101"), storeId, categoryIds[0], "Mâm Cúng Đầy Tháng", "MAM-CUNG-DAY-THANG", "mam-cung-day-thang", "MAM001", 50, true, false, "product", DateTime.UtcNow, null, null, null },
                    { new Guid("00000000-0000-0000-0000-000000000102"), storeId, categoryIds[0], "Bộ Tam Sên Đầy Tháng", "BO-TAM-SEN-DAY-THANG", "bo-tam-sen-day-thang", "TAM001", 30, true, false, "product", DateTime.UtcNow, null, null, null },
                    { new Guid("00000000-0000-0000-0000-000000000201"), storeId, categoryIds[1], "Hoa Hồng Đỏ (Bó 10 bông)", "HOA-HONG-DO-10", "hoa-hong-do-10", "HOA001", 100, true, false, "product", DateTime.UtcNow, null, null, null },
                    { new Guid("00000000-0000-0000-0000-000000000202"), storeId, categoryIds[1], "Ngũ Quả Cúng (Bộ 5)", "NGU-QUA-CUNG-5", "ngu-qua-cung-5", "QUA001", 80, true, false, "product", DateTime.UtcNow, null, null, null }
                }
            );

            // Insert products for Tết (Category 3 & 4)
            migrationBuilder.InsertData(
                table: "Products",
                columns: new[] { "Id", "StoreId", "CategoryId", "Name", "Code", "Slug", "SKU", "Stock", "IsActive", "IsDeleted", "Type", "CreatedAt", "CreatedBy", "UpdatedAt", "UpdatedBy" },
                values: new object[,]
                {
                    { new Guid("00000000-0000-0000-0000-000000000301"), storeId, categoryIds[2], "Hoa Đào Tết", "HOA-DAO-TET", "hoa-dao-tet", "HOA002", 60, true, false, "product", DateTime.UtcNow, null, null, null },
                    { new Guid("00000000-0000-0000-0000-000000000302"), storeId, categoryIds[2], "Hoa Mai Vàng", "HOA-MAI-VANG", "hoa-mai-vang", "HOA003", 60, true, false, "product", DateTime.UtcNow, null, null, null },
                    { new Guid("00000000-0000-0000-0000-000000000303"), storeId, categoryIds[2], "Cây Quất Bonsai", "CAY-QUAT-BONSAI", "cay-quat-bonsai", "CAY001", 40, true, false, "product", DateTime.UtcNow, null, null, null },
                    { new Guid("00000000-0000-0000-0000-000000000401"), storeId, categoryIds[3], "Bánh Chưng Truyền Thống", "BANH-CHUNG-TT", "banh-chung-truyen-thong", "BANH001", 100, true, false, "product", DateTime.UtcNow, null, null, null },
                    { new Guid("00000000-0000-0000-0000-000000000402"), storeId, categoryIds[3], "Bánh Tét Chuối", "BANH-TET-CHUOI", "banh-tet-chuoi", "BANH002", 80, true, false, "product", DateTime.UtcNow, null, null, null }
                }
            );

            // Insert products for Lễ Cúng Tổ Tiên (Category 5 & 6)
            migrationBuilder.InsertData(
                table: "Products",
                columns: new[] { "Id", "StoreId", "CategoryId", "Name", "Code", "Slug", "SKU", "Stock", "IsActive", "IsDeleted", "Type", "CreatedAt", "CreatedBy", "UpdatedAt", "UpdatedBy" },
                values: new object[,]
                {
                    { new Guid("00000000-0000-0000-0000-000000000501"), storeId, categoryIds[4], "Nến Cúng Tổ Tiên (Bộ 3)", "NEN-CUNG-TO-TIEN-3", "nen-cung-to-tien-3", "NEN001", 70, true, false, "product", DateTime.UtcNow, null, null, null },
                    { new Guid("00000000-0000-0000-0000-000000000502"), storeId, categoryIds[4], "Hương Cúng Tổ Tiên (Bó 100)", "HUONG-CUNG-TO-TIEN-100", "huong-cung-to-tien-100", "HUO001", 90, true, false, "product", DateTime.UtcNow, null, null, null },
                    { new Guid("00000000-0000-0000-0000-000000000601"), storeId, categoryIds[5], "Thịt Heo Quay", "THIT-HEO-QUAY", "thit-heo-quay", "THI001", 40, true, false, "product", DateTime.UtcNow, null, null, null },
                    { new Guid("00000000-0000-0000-0000-000000000602"), storeId, categoryIds[5], "Cá Chép Nướng", "CA-CHEP-NUONG", "ca-chep-nuong", "CA001", 35, true, false, "product", DateTime.UtcNow, null, null, null },
                    { new Guid("00000000-0000-0000-0000-000000000603"), storeId, categoryIds[5], "Trứng Gà Luộc", "TRUNG-GA-LUOC", "trung-ga-luoc", "TRU001", 120, true, false, "product", DateTime.UtcNow, null, null, null }
                }
            );

            // Insert products for Lễ Cúng Thần Tài (Category 7)
            migrationBuilder.InsertData(
                table: "Products",
                columns: new[] { "Id", "StoreId", "CategoryId", "Name", "Code", "Slug", "SKU", "Stock", "IsActive", "IsDeleted", "Type", "CreatedAt", "CreatedBy", "UpdatedAt", "UpdatedBy" },
                values: new object[,]
                {
                    { new Guid("00000000-0000-0000-0000-000000000701"), storeId, categoryIds[6], "Vàng Mã Cúng Thần Tài (Bộ 10)", "VANG-MA-THAN-TAI-10", "vang-ma-than-tai-10", "VAM001", 150, true, false, "product", DateTime.UtcNow, null, null, null },
                    { new Guid("00000000-0000-0000-0000-000000000702"), storeId, categoryIds[6], "Tiền Vàng Cúng (Bộ 5)", "TIEN-VANG-CUNG-5", "tien-vang-cung-5", "TIE001", 100, true, false, "product", DateTime.UtcNow, null, null, null },
                    { new Guid("00000000-0000-0000-0000-000000000703"), storeId, categoryIds[6], "Bánh Chưng Vàng", "BANH-CHUNG-VANG", "banh-chung-vang", "BAV001", 60, true, false, "product", DateTime.UtcNow, null, null, null }
                }
            );

            // Insert products for Tết Trung Thu (Category 8)
            migrationBuilder.InsertData(
                table: "Products",
                columns: new[] { "Id", "StoreId", "CategoryId", "Name", "Code", "Slug", "SKU", "Stock", "IsActive", "IsDeleted", "Type", "CreatedAt", "CreatedBy", "UpdatedAt", "UpdatedBy" },
                values: new object[,]
                {
                    { new Guid("00000000-0000-0000-0000-000000000801"), storeId, categoryIds[7], "Đèn Lồng Trung Thu (Bộ 5)", "DEN-LONG-TRUNG-THU-5", "den-long-trung-thu-5", "DEN001", 80, true, false, "product", DateTime.UtcNow, null, null, null },
                    { new Guid("00000000-0000-0000-0000-000000000802"), storeId, categoryIds[7], "Bánh Trung Thu Truyền Thống", "BANH-TRUNG-THU-TT", "banh-trung-thu-truyen-thong", "BAT001", 120, true, false, "product", DateTime.UtcNow, null, null, null },
                    { new Guid("00000000-0000-0000-0000-000000000803"), storeId, categoryIds[7], "Mặt Nạ Trung Thu", "MAT-NA-TRUNG-THU", "mat-na-trung-thu", "MAN001", 150, true, false, "product", DateTime.UtcNow, null, null, null }
                }
            );
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Delete products
            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValues: new object[]
                {
                    new Guid("00000000-0000-0000-0000-000000000101"),
                    new Guid("00000000-0000-0000-0000-000000000102"),
                    new Guid("00000000-0000-0000-0000-000000000201"),
                    new Guid("00000000-0000-0000-0000-000000000202"),
                    new Guid("00000000-0000-0000-0000-000000000301"),
                    new Guid("00000000-0000-0000-0000-000000000302"),
                    new Guid("00000000-0000-0000-0000-000000000303"),
                    new Guid("00000000-0000-0000-0000-000000000401"),
                    new Guid("00000000-0000-0000-0000-000000000402"),
                    new Guid("00000000-0000-0000-0000-000000000501"),
                    new Guid("00000000-0000-0000-0000-000000000502"),
                    new Guid("00000000-0000-0000-0000-000000000601"),
                    new Guid("00000000-0000-0000-0000-000000000602"),
                    new Guid("00000000-0000-0000-0000-000000000603"),
                    new Guid("00000000-0000-0000-0000-000000000701"),
                    new Guid("00000000-0000-0000-0000-000000000702"),
                    new Guid("00000000-0000-0000-0000-000000000703"),
                    new Guid("00000000-0000-0000-0000-000000000801"),
                    new Guid("00000000-0000-0000-0000-000000000802"),
                    new Guid("00000000-0000-0000-0000-000000000803")
                }
            );

            // Delete categories
            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValues: new object[]
                {
                    new Guid("00000000-0000-0000-0000-000000000001"),
                    new Guid("00000000-0000-0000-0000-000000000002"),
                    new Guid("00000000-0000-0000-0000-000000000003"),
                    new Guid("00000000-0000-0000-0000-000000000004"),
                    new Guid("00000000-0000-0000-0000-000000000005"),
                    new Guid("00000000-0000-0000-0000-000000000006"),
                    new Guid("00000000-0000-0000-0000-000000000007"),
                    new Guid("00000000-0000-0000-0000-000000000008")
                }
            );
        }
    }
}
