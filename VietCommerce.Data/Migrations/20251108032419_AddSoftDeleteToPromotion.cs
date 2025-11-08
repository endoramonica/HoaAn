using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VietCommerce.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddSoftDeleteToPromotion : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                table: "Promotions",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "DeletedBy",
                table: "Promotions",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "Promotions",
                type: "bit",
                nullable: false,
                defaultValue: false);
            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "Promotions",
                type: "bit",
                nullable: false,
                defaultValue: false); // hoặc false tùy logic

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                table: "PromotionProducts",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "DeletedBy",
                table: "PromotionProducts",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "PromotionProducts",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "PromotionProducts",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "Promotions");

            migrationBuilder.DropColumn(
                name: "DeletedBy",
                table: "Promotions");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "Promotions");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "PromotionProducts");

            migrationBuilder.DropColumn(
                name: "DeletedBy",
                table: "PromotionProducts");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "PromotionProducts");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "PromotionProducts");
            migrationBuilder.DropColumn(
                   name: "IsActive",
                   table: "Promotions");

        }
    }
}
