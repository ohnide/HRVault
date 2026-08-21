using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HRVault.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddAttendanceModule : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AttendanceDevices",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CompanyId = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    Manufacturer = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Model = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    SerialNumber = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: true),
                    IpAddress = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    Port = table.Column<int>(type: "integer", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    SettingsJson = table.Column<string>(type: "jsonb", nullable: true),
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
                    table.PrimaryKey("PK_AttendanceDevices", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AttendanceDevices_Companies_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Companies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "AttendanceEvents",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CompanyId = table.Column<Guid>(type: "uuid", nullable: false),
                    AttendanceDeviceId = table.Column<Guid>(type: "uuid", nullable: false),
                    ExternalEventId = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    ExternalUserId = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    TimestampUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Direction = table.Column<int>(type: "integer", nullable: false),
                    ReaderCode = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: true),
                    RawPayload = table.Column<string>(type: "jsonb", nullable: true),
                    IsProcessed = table.Column<bool>(type: "boolean", nullable: false),
                    ProcessedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ProcessingError = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    UpdatedBy = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AttendanceEvents", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AttendanceEvents_AttendanceDevices_AttendanceDeviceId",
                        column: x => x.AttendanceDeviceId,
                        principalTable: "AttendanceDevices",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "EmployeeDeviceIdentities",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CompanyId = table.Column<Guid>(type: "uuid", nullable: false),
                    EmployeeId = table.Column<Guid>(type: "uuid", nullable: false),
                    AttendanceDeviceId = table.Column<Guid>(type: "uuid", nullable: false),
                    ExternalUserId = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    CardNumber = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    UpdatedBy = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmployeeDeviceIdentities", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EmployeeDeviceIdentities_AttendanceDevices_AttendanceDevice~",
                        column: x => x.AttendanceDeviceId,
                        principalTable: "AttendanceDevices",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_EmployeeDeviceIdentities_Employees_EmployeeId",
                        column: x => x.EmployeeId,
                        principalTable: "Employees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TimePunches",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CompanyId = table.Column<Guid>(type: "uuid", nullable: false),
                    EmployeeId = table.Column<Guid>(type: "uuid", nullable: false),
                    TimestampUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Source = table.Column<int>(type: "integer", nullable: false),
                    Direction = table.Column<int>(type: "integer", nullable: false),
                    AttendanceDeviceId = table.Column<Guid>(type: "uuid", nullable: true),
                    AttendanceEventId = table.Column<Guid>(type: "uuid", nullable: true),
                    AdjustmentReason = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    IsVoided = table.Column<bool>(type: "boolean", nullable: false),
                    VoidReason = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    VoidedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    VoidedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    UpdatedBy = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TimePunches", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TimePunches_AttendanceDevices_AttendanceDeviceId",
                        column: x => x.AttendanceDeviceId,
                        principalTable: "AttendanceDevices",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TimePunches_AttendanceEvents_AttendanceEventId",
                        column: x => x.AttendanceEventId,
                        principalTable: "AttendanceEvents",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TimePunches_Employees_EmployeeId",
                        column: x => x.EmployeeId,
                        principalTable: "Employees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AttendanceDevices_CompanyId_Name",
                table: "AttendanceDevices",
                columns: new[] { "CompanyId", "Name" },
                unique: true,
                filter: "\"IsDeleted\" = false");

            migrationBuilder.CreateIndex(
                name: "IX_AttendanceDevices_CompanyId_SerialNumber",
                table: "AttendanceDevices",
                columns: new[] { "CompanyId", "SerialNumber" },
                unique: true,
                filter: "\"SerialNumber\" IS NOT NULL AND \"IsDeleted\" = false");

            migrationBuilder.CreateIndex(
                name: "IX_AttendanceEvents_AttendanceDeviceId",
                table: "AttendanceEvents",
                column: "AttendanceDeviceId");

            migrationBuilder.CreateIndex(
                name: "IX_AttendanceEvents_CompanyId_AttendanceDeviceId_ExternalEvent~",
                table: "AttendanceEvents",
                columns: new[] { "CompanyId", "AttendanceDeviceId", "ExternalEventId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AttendanceEvents_CompanyId_IsProcessed",
                table: "AttendanceEvents",
                columns: new[] { "CompanyId", "IsProcessed" });

            migrationBuilder.CreateIndex(
                name: "IX_AttendanceEvents_CompanyId_TimestampUtc",
                table: "AttendanceEvents",
                columns: new[] { "CompanyId", "TimestampUtc" });

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeDeviceIdentities_AttendanceDeviceId",
                table: "EmployeeDeviceIdentities",
                column: "AttendanceDeviceId");

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeDeviceIdentities_CompanyId_AttendanceDeviceId_Exter~",
                table: "EmployeeDeviceIdentities",
                columns: new[] { "CompanyId", "AttendanceDeviceId", "ExternalUserId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeDeviceIdentities_CompanyId_EmployeeId_AttendanceDev~",
                table: "EmployeeDeviceIdentities",
                columns: new[] { "CompanyId", "EmployeeId", "AttendanceDeviceId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeDeviceIdentities_EmployeeId",
                table: "EmployeeDeviceIdentities",
                column: "EmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_TimePunches_AttendanceDeviceId",
                table: "TimePunches",
                column: "AttendanceDeviceId");

            migrationBuilder.CreateIndex(
                name: "IX_TimePunches_AttendanceEventId",
                table: "TimePunches",
                column: "AttendanceEventId",
                unique: true,
                filter: "\"AttendanceEventId\" IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_TimePunches_CompanyId_EmployeeId_TimestampUtc",
                table: "TimePunches",
                columns: new[] { "CompanyId", "EmployeeId", "TimestampUtc" });

            migrationBuilder.CreateIndex(
                name: "IX_TimePunches_EmployeeId",
                table: "TimePunches",
                column: "EmployeeId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EmployeeDeviceIdentities");

            migrationBuilder.DropTable(
                name: "TimePunches");

            migrationBuilder.DropTable(
                name: "AttendanceEvents");

            migrationBuilder.DropTable(
                name: "AttendanceDevices");
        }
    }
}
