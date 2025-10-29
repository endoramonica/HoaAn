using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VietCommerce.Data.Migrations
{
    /// <inheritdoc />
    public partial class MakeAuditFieldsNullable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<Guid>(
    name: "UpdatedBy",
    table: "Orders",
    type: "uniqueidentifier",
    nullable: true,
    oldClrType: typeof(Guid),
    oldType: "uniqueidentifier");

            migrationBuilder.AlterColumn<Guid>(
                name: "UpdatedBy",
                table: "OrderStatusHistories",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");
            migrationBuilder.AlterColumn<Guid>(
    name: "UpdatedBy",
    table: "OrderShippings",
    type: "uniqueidentifier",
    nullable: true,
    oldClrType: typeof(Guid),
    oldType: "uniqueidentifier");

            

        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
