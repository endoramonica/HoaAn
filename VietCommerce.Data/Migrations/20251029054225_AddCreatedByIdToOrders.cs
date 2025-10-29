using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VietCommerce.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddCreatedByIdToOrders : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "CreatedById",
                table: "Orders",
                type: "uniqueidentifier",
                nullable: true);
            migrationBuilder.AddColumn<Guid>(
               name: "CreatedById",
               table: "OrderItems",
               type: "uniqueidentifier",
               nullable: true);
            migrationBuilder.AddColumn<Guid>(
               name: "CreatedById",
               table: "OrderShippings",
               type: "uniqueidentifier",
               nullable: true);
            migrationBuilder.AddColumn<Guid>(
               name: "CreatedById",
               table: "OrderStatusHistories",
               type: "uniqueidentifier",

               nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedById",
                table: "Orders");
            migrationBuilder.DropColumn(
                name: "CreatedById",
                table: "OrderItems");
            migrationBuilder.DropColumn(
                name: "CreatedById",
                table: "OrderShippings");
            migrationBuilder.DropColumn(
                name: "CreatedById",
                table: "OrderStatusHistories");
        }

    }
}
