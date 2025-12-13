using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VietCommerce.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddTargetingRulesToCampaign : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "TargetingRules",
                table: "Campaigns",
                type: "nvarchar(2000)",
                maxLength: 2000,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TargetingRules",
                table: "Campaigns");
        }
    }
}
