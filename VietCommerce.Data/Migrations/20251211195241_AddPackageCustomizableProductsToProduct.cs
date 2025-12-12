using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VietCommerce.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddPackageCustomizableProductsToProduct : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
               name: "DetailsJson",
               table: "Products",
               type: "nvarchar(max)",
               nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CustomizableOptionsJson",
                table: "Products",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Products_Id",
                table: "Products",
                column: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Products_Id",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "DetailsJson",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "CustomizableOptionsJson",
                table: "Products");
        }
    }
}
