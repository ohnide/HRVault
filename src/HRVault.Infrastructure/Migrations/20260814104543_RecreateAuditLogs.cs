using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HRVault.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RecreateAuditLogs : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
{
    migrationBuilder.DropTable(
        name: "AuditLogs");

    migrationBuilder.CreateTable(
        name: "AuditLogs",
        columns: table => new
        {
            Id = table.Column<Guid>(
                type: "uuid",
                nullable: false),

            CompanyId = table.Column<Guid>(
                type: "uuid",
                nullable: true),

            UserId = table.Column<Guid>(
                type: "uuid",
                nullable: true),

            UserName = table.Column<string>(
                type: "character varying(200)",
                maxLength: 200,
                nullable: true),

            Action = table.Column<string>(
                type: "character varying(150)",
                maxLength: 150,
                nullable: false),

            EntityName = table.Column<string>(
                type: "character varying(150)",
                maxLength: 150,
                nullable: false),

            EntityId = table.Column<Guid>(
                type: "uuid",
                nullable: true),

            OldValues = table.Column<string>(
                type: "jsonb",
                nullable: true),

            NewValues = table.Column<string>(
                type: "jsonb",
                nullable: true),

            CreatedAt = table.Column<DateTime>(
                type: "timestamp with time zone",
                nullable: false),

            IpAddress = table.Column<string>(
                type: "character varying(100)",
                maxLength: 100,
                nullable: true)
        },
        constraints: table =>
        {
            table.PrimaryKey(
                "PK_AuditLogs",
                x => x.Id);
        });

    migrationBuilder.CreateIndex(
        name: "IX_AuditLogs_CompanyId",
        table: "AuditLogs",
        column: "CompanyId");

    migrationBuilder.CreateIndex(
        name: "IX_AuditLogs_CompanyId_CreatedAt",
        table: "AuditLogs",
        columns: new[] { "CompanyId", "CreatedAt" });

    migrationBuilder.CreateIndex(
        name: "IX_AuditLogs_CreatedAt",
        table: "AuditLogs",
        column: "CreatedAt");

    migrationBuilder.CreateIndex(
        name: "IX_AuditLogs_EntityId",
        table: "AuditLogs",
        column: "EntityId");

    migrationBuilder.CreateIndex(
        name: "IX_AuditLogs_EntityName",
        table: "AuditLogs",
        column: "EntityName");

    migrationBuilder.CreateIndex(
        name: "IX_AuditLogs_UserId",
        table: "AuditLogs",
        column: "UserId");
}

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_AuditLogs_CompanyId",
                table: "AuditLogs");

            migrationBuilder.DropIndex(
                name: "IX_AuditLogs_CompanyId_CreatedAt",
                table: "AuditLogs");

            migrationBuilder.DropIndex(
                name: "IX_AuditLogs_CreatedAt",
                table: "AuditLogs");

            migrationBuilder.DropIndex(
                name: "IX_AuditLogs_EntityId",
                table: "AuditLogs");

            migrationBuilder.DropIndex(
                name: "IX_AuditLogs_EntityName",
                table: "AuditLogs");

            migrationBuilder.DropIndex(
                name: "IX_AuditLogs_UserId",
                table: "AuditLogs");

            migrationBuilder.DropColumn(
                name: "EntityName",
                table: "AuditLogs");

            migrationBuilder.DropColumn(
                name: "UserName",
                table: "AuditLogs");

            migrationBuilder.RenameColumn(
                name: "IpAddress",
                table: "AuditLogs",
                newName: "IPAddress");

            migrationBuilder.RenameColumn(
                name: "EntityId",
                table: "AuditLogs",
                newName: "UpdatedBy");

            migrationBuilder.RenameColumn(
                name: "CompanyId",
                table: "AuditLogs",
                newName: "DeletedBy");

            migrationBuilder.AlterColumn<Guid>(
                name: "UserId",
                table: "AuditLogs",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "OldValues",
                table: "AuditLogs",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "jsonb",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "NewValues",
                table: "AuditLogs",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "jsonb",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "IPAddress",
                table: "AuditLogs",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(100)",
                oldMaxLength: 100,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Action",
                table: "AuditLogs",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(150)",
                oldMaxLength: 150);

            migrationBuilder.AddColumn<Guid>(
                name: "CreatedBy",
                table: "AuditLogs",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                table: "AuditLogs",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "AuditLogs",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "RecordId",
                table: "AuditLogs",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "TableName",
                table: "AuditLogs",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "AuditLogs",
                type: "timestamp with time zone",
                nullable: true);
        }
    }
}
