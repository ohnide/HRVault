using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HRVault.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddEmployeeDocumentTypes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Documents_Employees_EmployeeId",
                table: "Documents");

            migrationBuilder.DropIndex(
                name: "IX_Documents_Category",
                table: "Documents");
				
			migrationBuilder.Sql(
				"DELETE FROM \"Documents\";");

            migrationBuilder.DropColumn(
                name: "Category",
                table: "Documents");

            migrationBuilder.AddColumn<Guid>(
                name: "EmployeeDocumentTypeId",
                table: "Documents",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<DateOnly>(
                name: "ExpirationDate",
                table: "Documents",
                type: "date",
                nullable: true);

            migrationBuilder.AddColumn<DateOnly>(
                name: "IssueDate",
                table: "Documents",
                type: "date",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Notes",
                table: "Documents",
                type: "character varying(1000)",
                maxLength: 1000,
                nullable: true);

            migrationBuilder.CreateTable(
                name: "EmployeeDocumentTypes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CompanyId = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    Description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    HasExpiration = table.Column<bool>(type: "boolean", nullable: false),
                    ExpirationWarningDays = table.Column<int>(type: "integer", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    UpdatedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    DeletedBy = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmployeeDocumentTypes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EmployeeDocumentTypes_Companies_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Companies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Documents_EmployeeDocumentTypeId",
                table: "Documents",
                column: "EmployeeDocumentTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_Documents_ExpirationDate",
                table: "Documents",
                column: "ExpirationDate");

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeDocumentTypes_CompanyId_Name",
                table: "EmployeeDocumentTypes",
                columns: new[] { "CompanyId", "Name" },
                unique: true,
                filter: "\"IsDeleted\" = false");

            migrationBuilder.AddForeignKey(
                name: "FK_Documents_EmployeeDocumentTypes_EmployeeDocumentTypeId",
                table: "Documents",
                column: "EmployeeDocumentTypeId",
                principalTable: "EmployeeDocumentTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Documents_Employees_EmployeeId",
                table: "Documents",
                column: "EmployeeId",
                principalTable: "Employees",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Documents_EmployeeDocumentTypes_EmployeeDocumentTypeId",
                table: "Documents");

            migrationBuilder.DropForeignKey(
                name: "FK_Documents_Employees_EmployeeId",
                table: "Documents");

            migrationBuilder.DropTable(
                name: "EmployeeDocumentTypes");

            migrationBuilder.DropIndex(
                name: "IX_Documents_EmployeeDocumentTypeId",
                table: "Documents");

            migrationBuilder.DropIndex(
                name: "IX_Documents_ExpirationDate",
                table: "Documents");

            migrationBuilder.DropColumn(
                name: "EmployeeDocumentTypeId",
                table: "Documents");

            migrationBuilder.DropColumn(
                name: "ExpirationDate",
                table: "Documents");

            migrationBuilder.DropColumn(
                name: "IssueDate",
                table: "Documents");

            migrationBuilder.DropColumn(
                name: "Notes",
                table: "Documents");

            migrationBuilder.AddColumn<string>(
                name: "Category",
                table: "Documents",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_Documents_Category",
                table: "Documents",
                column: "Category");

            migrationBuilder.AddForeignKey(
                name: "FK_Documents_Employees_EmployeeId",
                table: "Documents",
                column: "EmployeeId",
                principalTable: "Employees",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
