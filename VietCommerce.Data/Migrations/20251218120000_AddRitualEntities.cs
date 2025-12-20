using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VietCommerce.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddRitualEntities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Create Rituals table
            migrationBuilder.CreateTable(
                name: "Rituals",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RitualId = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    ActionSequencePatternJson = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RequiredItemsJson = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ConfidenceThreshold = table.Column<decimal>(type: "decimal(3,2)", nullable: false),
                    CulturalSignificance = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    SourcesJson = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Rituals", x => x.Id);
                });

            // Create Actions table
            migrationBuilder.CreateTable(
                name: "Actions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SessionId = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: false),
                    ActionType = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    ActionTimestamp = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ProductId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CategoryId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    MetadataJson = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Actions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Actions_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            // Create RitualDismissals table
            migrationBuilder.CreateTable(
                name: "RitualDismissals",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RitualId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DismissedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DismissalReason = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    IsDisabled = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RitualDismissals", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RitualDismissals_Rituals_RitualId",
                        column: x => x.RitualId,
                        principalTable: "Rituals",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RitualDismissals_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            // Create RecommendationLogs table
            migrationBuilder.CreateTable(
                name: "RecommendationLogs",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SessionId = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: false),
                    RitualId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ConfidenceScore = table.Column<decimal>(type: "decimal(3,2)", nullable: false),
                    MissingItemsJson = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MatchingMetadataJson = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DisplayedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UserInteraction = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    InteractionAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    SystemReportJson = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RecommendationLogs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RecommendationLogs_Rituals_RitualId",
                        column: x => x.RitualId,
                        principalTable: "Rituals",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RecommendationLogs_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            // Create indexes for Rituals
            migrationBuilder.CreateIndex(
                name: "IX_Rituals_IsActive",
                table: "Rituals",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_Rituals_RitualId",
                table: "Rituals",
                column: "RitualId",
                unique: true);

            // Create indexes for Actions
            migrationBuilder.CreateIndex(
                name: "IX_Actions_ActionTimestamp",
                table: "Actions",
                column: "ActionTimestamp");

            migrationBuilder.CreateIndex(
                name: "IX_Actions_SessionId",
                table: "Actions",
                column: "SessionId");

            migrationBuilder.CreateIndex(
                name: "IX_Actions_UserId_SessionId_ActionTimestamp",
                table: "Actions",
                columns: new[] { "UserId", "SessionId", "ActionTimestamp" });

            // Create indexes for RitualDismissals
            migrationBuilder.CreateIndex(
                name: "IX_RitualDismissals_DismissedAt",
                table: "RitualDismissals",
                column: "DismissedAt");

            migrationBuilder.CreateIndex(
                name: "IX_RitualDismissals_UserId_RitualId",
                table: "RitualDismissals",
                columns: new[] { "UserId", "RitualId" },
                unique: true);

            // Create indexes for RecommendationLogs
            migrationBuilder.CreateIndex(
                name: "IX_RecommendationLogs_RitualId",
                table: "RecommendationLogs",
                column: "RitualId");

            migrationBuilder.CreateIndex(
                name: "IX_RecommendationLogs_SessionId",
                table: "RecommendationLogs",
                column: "SessionId");

            migrationBuilder.CreateIndex(
                name: "IX_RecommendationLogs_UserInteraction",
                table: "RecommendationLogs",
                column: "UserInteraction");

            migrationBuilder.CreateIndex(
                name: "IX_RecommendationLogs_UserId_DisplayedAt",
                table: "RecommendationLogs",
                columns: new[] { "UserId", "DisplayedAt" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RecommendationLogs");

            migrationBuilder.DropTable(
                name: "RitualDismissals");

            migrationBuilder.DropTable(
                name: "Actions");

            migrationBuilder.DropTable(
                name: "Rituals");
        }
    }
}
