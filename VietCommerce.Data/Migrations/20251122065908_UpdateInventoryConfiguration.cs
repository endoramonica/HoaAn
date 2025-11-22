using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VietCommerce.Data.Migrations
{
    public partial class UpdateInventoryConfiguration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // ======================================================
            // 1) DROP các FK sai và index sai nếu tồn tại
            // ======================================================
            migrationBuilder.Sql(@"
IF EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = 'FK_Inventories_Stores_StoreId1')
ALTER TABLE Inventories DROP CONSTRAINT FK_Inventories_Stores_StoreId1;
");

            migrationBuilder.Sql(@"
IF EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_Inventories_StoreId1')
DROP INDEX IX_Inventories_StoreId1 ON Inventories;
");

            migrationBuilder.Sql(@"
IF EXISTS (SELECT 1 FROM sys.columns 
           WHERE Name = 'StoreId1' AND Object_ID = Object_ID('Inventories'))
ALTER TABLE Inventories DROP COLUMN StoreId1;
");

            migrationBuilder.Sql(@"
IF EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_Inventories_LowStock')
DROP INDEX IX_Inventories_LowStock ON Inventories;
");

            migrationBuilder.Sql(@"
IF EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = 'FK_Inventories_Products_ProductId')
ALTER TABLE Inventories DROP CONSTRAINT FK_Inventories_Products_ProductId;
");

            // ======================================================
            // 2) Tạo lại Index đúng theo InventoryConfiguration
            // ======================================================

            // Unique: StoreId + ProductId
            migrationBuilder.CreateIndex(
                name: "IX_Inventories_Store_Product_Unique",
                table: "Inventories",
                columns: new[] { "StoreId", "ProductId" },
                unique: true);

            // Product index
            migrationBuilder.CreateIndex(
                name: "IX_Inventories_ProductId",
                table: "Inventories",
                column: "ProductId");

            // IsDeleted index
            migrationBuilder.CreateIndex(
                name: "IX_Inventories_IsDeleted",
                table: "Inventories",
                column: "IsDeleted");

            // Low stock (QuantityAvailable + ReorderLevel)
            migrationBuilder.CreateIndex(
                name: "IX_Inventories_LowStock",
                table: "Inventories",
                columns: new[] { "QuantityAvailable", "ReorderLevel" });

            // CreatedAt index
            migrationBuilder.CreateIndex(
                name: "IX_Inventories_CreatedAt",
                table: "Inventories",
                column: "CreatedAt");

            // ======================================================
            // 3) ADD FOREIGN KEY đúng theo config
            // ======================================================

            // FK Store
            migrationBuilder.Sql(@"
IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = 'FK_Inventories_Stores_StoreId')
ALTER TABLE Inventories
ADD CONSTRAINT FK_Inventories_Stores_StoreId
FOREIGN KEY (StoreId) REFERENCES Stores(Id) ON DELETE NO ACTION;
");


            // FK Product
            migrationBuilder.AddForeignKey(
                name: "FK_Inventories_Products_ProductId",
                table: "Inventories",
                column: "ProductId",
                principalTable: "Products",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            // FK Tenant
            migrationBuilder.Sql(@"
IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = 'FK_Inventories_Tenants_TenantId')
ALTER TABLE Inventories
ADD CONSTRAINT FK_Inventories_Tenants_TenantId
FOREIGN KEY (TenantId) REFERENCES Tenants(Id) ON DELETE NO ACTION;
");

        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // ======================================================
            // Rollback: drop new FK & index
            // ======================================================
            migrationBuilder.DropForeignKey(
                name: "FK_Inventories_Stores_StoreId",
                table: "Inventories");

            migrationBuilder.DropForeignKey(
                name: "FK_Inventories_Products_ProductId",
                table: "Inventories");

            migrationBuilder.DropForeignKey(
                name: "FK_Inventories_Tenants_TenantId",
                table: "Inventories");

            migrationBuilder.DropIndex(
                name: "IX_Inventories_Store_Product_Unique",
                table: "Inventories");

            migrationBuilder.DropIndex(
                name: "IX_Inventories_ProductId",
                table: "Inventories");

            migrationBuilder.DropIndex(
                name: "IX_Inventories_IsDeleted",
                table: "Inventories");

            migrationBuilder.DropIndex(
                name: "IX_Inventories_LowStock",
                table: "Inventories");

            migrationBuilder.DropIndex(
                name: "IX_Inventories_CreatedAt",
                table: "Inventories");

            // ======================================================
            // Rollback StoreId1 nếu muốn
            // ======================================================
            migrationBuilder.AddColumn<Guid>(
                name: "StoreId1",
                table: "Inventories",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Inventories_StoreId1",
                table: "Inventories",
                column: "StoreId1");

            migrationBuilder.AddForeignKey(
                name: "FK_Inventories_Stores_StoreId1",
                table: "Inventories",
                column: "StoreId1",
                principalTable: "Stores",
                principalColumn: "Id");
        }
    }
}
