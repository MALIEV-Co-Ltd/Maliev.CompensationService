using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Maliev.CompensationService.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddBenefitsTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "benefits",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    benefit_type = table.Column<int>(type: "integer", nullable: false),
                    employer_contribution = table.Column<decimal>(type: "numeric", nullable: true),
                    employee_contribution = table.Column<decimal>(type: "numeric", nullable: true),
                    is_active = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    created_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    modified_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_benefits", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "benefits_enrollments",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    employee_id = table.Column<Guid>(type: "uuid", nullable: false),
                    benefit_id = table.Column<Guid>(type: "uuid", nullable: false),
                    enrollment_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    termination_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    status = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    employee_contribution = table.Column<decimal>(type: "numeric", nullable: true),
                    coverage_level = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    created_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    modified_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_benefits_enrollments", x => x.id);
                    table.ForeignKey(
                        name: "FK_benefits_enrollments_benefits_benefit_id",
                        column: x => x.benefit_id,
                        principalTable: "benefits",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "dependents",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    benefits_enrollment_id = table.Column<Guid>(type: "uuid", nullable: false),
                    first_name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    last_name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    relationship = table.Column<int>(type: "integer", nullable: false),
                    date_of_birth = table.Column<DateTime>(type: "date", nullable: false),
                    national_id_encrypted = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    created_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    modified_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_dependents", x => x.id);
                    table.ForeignKey(
                        name: "FK_dependents_benefits_enrollments_benefits_enrollment_id",
                        column: x => x.benefits_enrollment_id,
                        principalTable: "benefits_enrollments",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "idx_benefits_active",
                table: "benefits",
                column: "is_active",
                filter: "is_active = true");

            migrationBuilder.CreateIndex(
                name: "idx_benefits_type",
                table: "benefits",
                column: "benefit_type");

            migrationBuilder.CreateIndex(
                name: "idx_benefits_enroll_benefit",
                table: "benefits_enrollments",
                column: "benefit_id");

            migrationBuilder.CreateIndex(
                name: "idx_benefits_enroll_employee",
                table: "benefits_enrollments",
                column: "employee_id");

            migrationBuilder.CreateIndex(
                name: "idx_benefits_enroll_status",
                table: "benefits_enrollments",
                columns: new[] { "employee_id", "status" });

            migrationBuilder.CreateIndex(
                name: "IX_benefits_enrollments_employee_id_benefit_id_status",
                table: "benefits_enrollments",
                columns: new[] { "employee_id", "benefit_id", "status" },
                unique: true,
                filter: "status = 0");

            migrationBuilder.CreateIndex(
                name: "idx_dependents_enrollment",
                table: "dependents",
                column: "benefits_enrollment_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "dependents");

            migrationBuilder.DropTable(
                name: "benefits_enrollments");

            migrationBuilder.DropTable(
                name: "benefits");
        }
    }
}
