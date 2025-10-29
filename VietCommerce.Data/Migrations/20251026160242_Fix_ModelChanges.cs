using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VietCommerce.Data.Migrations
{
    /// <inheritdoc />
    public partial class Fix_ModelChanges : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "ChangedAt",
                table: "OrderStatusHistories",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<Guid>(
                name: "ChangedBy",
                table: "OrderStatusHistories",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_OrderStatusHistories_ChangedBy",
                table: "OrderStatusHistories",
                column: "ChangedBy");

            migrationBuilder.AddForeignKey(
                name: "FK_OrderStatusHistories_Users_ChangedBy",
                table: "OrderStatusHistories",
                column: "ChangedBy",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OrderStatusHistories_Users_ChangedBy",
                table: "OrderStatusHistories");

            migrationBuilder.DropIndex(
                name: "IX_OrderStatusHistories_ChangedBy",
                table: "OrderStatusHistories");

            migrationBuilder.DropColumn(
                name: "ChangedAt",
                table: "OrderStatusHistories");

            migrationBuilder.DropColumn(
                name: "ChangedBy",
                table: "OrderStatusHistories");
        }
    }
}
