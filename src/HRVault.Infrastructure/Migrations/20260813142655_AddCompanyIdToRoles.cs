using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HRVault.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddCompanyIdToRoles : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
{
    migrationBuilder.AddColumn<Guid>(
        name: "CompanyId",
        table: "Roles",
        type: "uuid",
        nullable: true);

    migrationBuilder.Sql("""
        UPDATE "Roles"
        SET "CompanyId" = (
            SELECT "Id"
            FROM "Companies"
            WHERE "IsDeleted" = false
            ORDER BY "CreatedAt"
            LIMIT 1
        )
        WHERE "CompanyId" IS NULL;
        """);

    migrationBuilder.AlterColumn<Guid>(
        name: "CompanyId",
        table: "Roles",
        type: "uuid",
        nullable: false,
        oldClrType: typeof(Guid),
        oldType: "uuid",
        oldNullable: true);

    migrationBuilder.CreateIndex(
        name: "IX_Roles_CompanyId_Name",
        table: "Roles",
        columns: new[] { "CompanyId", "Name" },
        unique: true,
        filter: "\"IsDeleted\" = false");

    migrationBuilder.AddForeignKey(
        name: "FK_Roles_Companies_CompanyId",
        table: "Roles",
        column: "CompanyId",
        principalTable: "Companies",
        principalColumn: "Id",
        onDelete: ReferentialAction.Restrict);
}

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
{
    migrationBuilder.DropForeignKey(
        name: "FK_Roles_Companies_CompanyId",
        table: "Roles");

    migrationBuilder.DropIndex(
        name: "IX_Roles_CompanyId_Name",
        table: "Roles");

    migrationBuilder.DropColumn(
        name: "CompanyId",
        table: "Roles");
}
    }
}
