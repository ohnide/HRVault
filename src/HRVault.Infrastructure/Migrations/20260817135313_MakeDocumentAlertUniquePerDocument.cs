using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HRVault.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class MakeDocumentAlertUniquePerDocument : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_DocumentAlerts_DocumentId",
                table: "DocumentAlerts");

            migrationBuilder.DropIndex(
                name: "IX_DocumentAlerts_DocumentId_AlertDate",
                table: "DocumentAlerts");

            migrationBuilder.CreateIndex(
                name: "IX_DocumentAlerts_DocumentId",
                table: "DocumentAlerts",
                column: "DocumentId",
                unique: true,
                filter: "\"IsDeleted\" = false");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_DocumentAlerts_DocumentId",
                table: "DocumentAlerts");

            migrationBuilder.CreateIndex(
                name: "IX_DocumentAlerts_DocumentId",
                table: "DocumentAlerts",
                column: "DocumentId");

            migrationBuilder.CreateIndex(
                name: "IX_DocumentAlerts_DocumentId_AlertDate",
                table: "DocumentAlerts",
                columns: new[] { "DocumentId", "AlertDate" },
                unique: true,
                filter: "\"IsDeleted\" = false");
        }
    }
}
