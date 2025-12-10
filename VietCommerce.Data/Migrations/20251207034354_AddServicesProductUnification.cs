using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VietCommerce.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddServicesProductUnification : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ServiceCategory",
                table: "Products",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ServiceDuration",
                table: "Products",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "ServiceRating",
                table: "Products",
                type: "decimal(3,2)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Type",
                table: "Products",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ServiceCategory",
                table: "Orders",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ServiceDate",
                table: "Orders",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ServiceDuration",
                table: "Orders",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ServiceLocation",
                table: "Orders",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ServiceNotes",
                table: "Orders",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ServiceTime",
                table: "Orders",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Type",
                table: "Orders",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ServiceCategory",
                table: "OrderItems",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ServiceDuration",
                table: "OrderItems",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Type",
                table: "OrderItems",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ServiceCategory",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "ServiceDuration",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "ServiceRating",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "Type",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "ServiceCategory",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "ServiceDate",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "ServiceDuration",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "ServiceLocation",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "ServiceNotes",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "ServiceTime",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "Type",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "ServiceCategory",
                table: "OrderItems");

            migrationBuilder.DropColumn(
                name: "ServiceDuration",
                table: "OrderItems");

            migrationBuilder.DropColumn(
                name: "Type",
                table: "OrderItems");
        }
    }
}
