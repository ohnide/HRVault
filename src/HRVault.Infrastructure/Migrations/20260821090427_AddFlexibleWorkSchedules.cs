using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HRVault.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddFlexibleWorkSchedules : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Type",
                table: "WorkSchedules",
                type: "integer",
                nullable: false,
                defaultValue: 1);

            migrationBuilder.AddColumn<TimeOnly>(
                name: "RequiredDailyTime",
                table: "WorkScheduleDays",
                type: "time without time zone",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Type",
                table: "WorkSchedules");

            migrationBuilder.DropColumn(
                name: "RequiredDailyTime",
                table: "WorkScheduleDays");
        }
    }
}
