using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HRVault.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddColorToAbsenceTypes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Color",
                table: "AbsenceTypes",
                type: "character varying(7)",
                maxLength: 7,
                nullable: false,
                defaultValue: "#3B82F6");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Color",
                table: "AbsenceTypes");
        }
    }
}
