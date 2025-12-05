using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VietCommerce.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddPriorityScoreToMarketingPost : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "DisplayLocation",
                table: "MarketingPosts",
                type: "nvarchar(max)",
                nullable: true,
                comment: "JSON array of display locations (homepage_banner, product_section, featured_section, sidebar)");

            migrationBuilder.AddColumn<bool>(
                name: "IsFeatured",
                table: "MarketingPosts",
                type: "bit",
                nullable: false,
                defaultValue: false,
                comment: "Auto-set to true when PriorityScore > 80");

            migrationBuilder.AddColumn<int>(
                name: "PriorityScore",
                table: "MarketingPosts",
                type: "int",
                nullable: false,
                defaultValue: 50,
                comment: "Priority score (1-100) for display ranking");

            migrationBuilder.CreateIndex(
                name: "IX_MarketingPosts_Featured_Priority_IsDeleted",
                table: "MarketingPosts",
                columns: new[] { "IsFeatured", "PriorityScore", "IsDeleted" });

            migrationBuilder.CreateIndex(
                name: "IX_MarketingPosts_IsFeatured",
                table: "MarketingPosts",
                column: "IsFeatured",
                filter: "[IsFeatured] = 1");

            migrationBuilder.CreateIndex(
                name: "IX_MarketingPosts_PriorityScore",
                table: "MarketingPosts",
                column: "PriorityScore");

            migrationBuilder.CreateIndex(
                name: "IX_MarketingPosts_PriorityScore_PublishedDate",
                table: "MarketingPosts",
                columns: new[] { "PriorityScore", "PublishedDate" },
                filter: "[PublishedDate] IS NOT NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_MarketingPosts_Featured_Priority_IsDeleted",
                table: "MarketingPosts");

            migrationBuilder.DropIndex(
                name: "IX_MarketingPosts_IsFeatured",
                table: "MarketingPosts");

            migrationBuilder.DropIndex(
                name: "IX_MarketingPosts_PriorityScore",
                table: "MarketingPosts");

            migrationBuilder.DropIndex(
                name: "IX_MarketingPosts_PriorityScore_PublishedDate",
                table: "MarketingPosts");

            migrationBuilder.DropColumn(
                name: "DisplayLocation",
                table: "MarketingPosts");

            migrationBuilder.DropColumn(
                name: "IsFeatured",
                table: "MarketingPosts");

            migrationBuilder.DropColumn(
                name: "PriorityScore",
                table: "MarketingPosts");
        }
    }
}
