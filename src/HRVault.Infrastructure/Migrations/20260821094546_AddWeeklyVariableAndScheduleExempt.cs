using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HRVault.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddWeeklyVariableAndScheduleExempt : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "RequiredWorkingDaysPerWeek",
                table: "WorkSchedules",
                type: "integer",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RequiredWorkingDaysPerWeek",
                table: "WorkSchedules");
        }
    }
}
