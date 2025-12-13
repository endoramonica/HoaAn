using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VietCommerce.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddCampaignPromotionIndexes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Campaign index: (StoreId, Status, StartDate, EndDate)
            migrationBuilder.CreateIndex(
                name: "IX_Campaigns_StoreId_Status_StartDate_EndDate",
                table: "Campaigns",
                columns: new[] { "StoreId", "Status", "StartDate", "EndDate" });

            // Promotion index: (CampaignId, Status)
            migrationBuilder.CreateIndex(
                name: "IX_Promotions_CampaignId_Status",
                table: "Promotions",
                columns: new[] { "CampaignId", "Status" });

            // Voucher index: (Code, ExpiryDate)
            migrationBuilder.CreateIndex(
                name: "IX_Vouchers_Code_ExpiryDate",
                table: "Vouchers",
                columns: new[] { "Code", "ExpiryDate" });

            // CampaignImpression index: (CampaignId, RecordedAt)
            migrationBuilder.CreateIndex(
                name: "IX_CampaignImpressions_CampaignId_RecordedAt",
                table: "CampaignImpressions",
                columns: new[] { "CampaignId", "RecordedAt" });

            // CampaignClick index: (CampaignId, RecordedAt)
            migrationBuilder.CreateIndex(
                name: "IX_CampaignClicks_CampaignId_RecordedAt",
                table: "CampaignClicks",
                columns: new[] { "CampaignId", "RecordedAt" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Drop all indexes
            migrationBuilder.DropIndex(
                name: "IX_Campaigns_StoreId_Status_StartDate_EndDate",
                table: "Campaigns");

            migrationBuilder.DropIndex(
                name: "IX_Promotions_CampaignId_Status",
                table: "Promotions");

            migrationBuilder.DropIndex(
                name: "IX_Vouchers_Code_ExpiryDate",
                table: "Vouchers");

            migrationBuilder.DropIndex(
                name: "IX_CampaignImpressions_CampaignId_RecordedAt",
                table: "CampaignImpressions");

            migrationBuilder.DropIndex(
                name: "IX_CampaignClicks_CampaignId_RecordedAt",
                table: "CampaignClicks");
        }
    }
}
