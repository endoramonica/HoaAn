using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VietCommerce.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddMarketingPostEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "MarketingPosts",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false, comment: "Marketing post title"),
                    Content = table.Column<string>(type: "nvarchar(max)", maxLength: 10000, nullable: false, comment: "Main content of the marketing post"),
                    ShortDescription = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true, comment: "Short description for previews"),
                    ImageUrl = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true, comment: "Primary image URL"),
                    ImageData = table.Column<string>(type: "nvarchar(max)", nullable: true, comment: "Base64 image data for backward compatibility"),
                    ImageUrls = table.Column<string>(type: "nvarchar(max)", nullable: true, comment: "JSON array of multiple image URLs"),
                    ProductId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ProductName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true, comment: "Denormalized product name for performance"),
                    Topic = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Platform = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true, comment: "Target platform (e.g., Facebook, Instagram)"),
                    Tone = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true, comment: "Content tone (e.g., Professional, Casual)"),
                    Hashtags = table.Column<string>(type: "nvarchar(max)", nullable: true, comment: "JSON array of hashtags"),
                    MetaTitle = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true, comment: "SEO meta title"),
                    MetaDescription = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true, comment: "SEO meta description"),
                    MetaKeywords = table.Column<string>(type: "nvarchar(max)", nullable: true, comment: "JSON array of SEO keywords"),
                    FacebookPost = table.Column<string>(type: "nvarchar(max)", nullable: true, comment: "Facebook-optimized content"),
                    InstagramPost = table.Column<string>(type: "nvarchar(max)", nullable: true, comment: "Instagram-optimized content"),
                    TwitterPost = table.Column<string>(type: "nvarchar(max)", nullable: true, comment: "Twitter-optimized content"),
                    LinkedInPost = table.Column<string>(type: "nvarchar(max)", nullable: true, comment: "LinkedIn-optimized content"),
                    Status = table.Column<int>(type: "int", nullable: false, defaultValue: 0, comment: "Publishing status: Draft, Published, Scheduled"),
                    ScheduledDate = table.Column<DateTime>(type: "datetime2", nullable: true, comment: "Scheduled publication date"),
                    PublishedDate = table.Column<DateTime>(type: "datetime2", nullable: true, comment: "Actual publication date"),
                    Views = table.Column<int>(type: "int", nullable: false, defaultValue: 0, comment: "Total view count"),
                    Clicks = table.Column<int>(type: "int", nullable: false, defaultValue: 0, comment: "Total click count"),
                    Shares = table.Column<int>(type: "int", nullable: false, defaultValue: 0, comment: "Total share count"),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true, comment: "Soft delete timestamp"),
                    DeletedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true, comment: "User who deleted the post"),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false, comment: "Concurrency token"),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UpdatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MarketingPosts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MarketingPosts_Products",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateIndex(
                name: "IX_MarketingPosts_CreatedAt",
                table: "MarketingPosts",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_MarketingPosts_IsDeleted",
                table: "MarketingPosts",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_MarketingPosts_ProductId",
                table: "MarketingPosts",
                column: "ProductId",
                filter: "[ProductId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_MarketingPosts_ProductId_IsDeleted",
                table: "MarketingPosts",
                columns: new[] { "ProductId", "IsDeleted" },
                filter: "[ProductId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_MarketingPosts_PublishedDate",
                table: "MarketingPosts",
                column: "PublishedDate",
                filter: "[PublishedDate] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_MarketingPosts_ScheduledDate",
                table: "MarketingPosts",
                column: "ScheduledDate",
                filter: "[ScheduledDate] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_MarketingPosts_Status",
                table: "MarketingPosts",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_MarketingPosts_Status_IsDeleted",
                table: "MarketingPosts",
                columns: new[] { "Status", "IsDeleted" });

            migrationBuilder.CreateIndex(
                name: "IX_MarketingPosts_Status_ScheduledDate",
                table: "MarketingPosts",
                columns: new[] { "Status", "ScheduledDate" },
                filter: "[ScheduledDate] IS NOT NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MarketingPosts");
        }
    }
}
