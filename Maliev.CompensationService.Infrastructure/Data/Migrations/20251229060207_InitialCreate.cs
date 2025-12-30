using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Maliev.CompensationService.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "compensation_records",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    employee_id = table.Column<Guid>(type: "uuid", nullable: false),
                    effective_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    base_salary_encrypted = table.Column<string>(type: "text", nullable: false),
                    currency = table.Column<string>(type: "character varying(3)", maxLength: 3, nullable: false, defaultValue: "USD"),
                    compensation_type = table.Column<int>(type: "integer", nullable: false),
                    bonus_percentage = table.Column<decimal>(type: "numeric", nullable: true),
                    commission_rate = table.Column<decimal>(type: "numeric", nullable: true),
                    change_reason = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    approved_by = table.Column<Guid>(type: "uuid", nullable: true),
                    is_current = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    created_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    modified_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_compensation_records", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "SalaryHistory",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    EmployeeId = table.Column<Guid>(type: "uuid", nullable: false),
                    CompensationRecordId = table.Column<Guid>(type: "uuid", nullable: false),
                    PreviousSalary = table.Column<decimal>(type: "numeric", nullable: false),
                    NewSalary = table.Column<decimal>(type: "numeric", nullable: false),
                    ChangeAmount = table.Column<decimal>(type: "numeric", nullable: false),
                    ChangePercentage = table.Column<decimal>(type: "numeric", nullable: false),
                    EffectiveDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ChangeType = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    ChangedBy = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SalaryHistory", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SalaryHistory_compensation_records_CompensationRecordId",
                        column: x => x.CompensationRecordId,
                        principalTable: "compensation_records",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "idx_comp_records_current",
                table: "compensation_records",
                columns: new[] { "employee_id", "is_current" },
                unique: true,
                filter: "is_current = true");

            migrationBuilder.CreateIndex(
                name: "idx_comp_records_employee",
                table: "compensation_records",
                column: "employee_id");

            migrationBuilder.CreateIndex(
                name: "IX_SalaryHistory_CompensationRecordId",
                table: "SalaryHistory",
                column: "CompensationRecordId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SalaryHistory");

            migrationBuilder.DropTable(
                name: "compensation_records");
        }
    }
}
