using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Maliev.CompensationService.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
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
                    WaitingPeriodDays = table.Column<int>(type: "integer", nullable: false),
                    is_active = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    created_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    modified_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_benefits", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "bulk_jobs",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    job_type = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    status = table.Column<int>(type: "integer", nullable: false),
                    parameters = table.Column<string>(type: "jsonb", maxLength: 500, nullable: false),
                    success_count = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    failure_count = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    error_details = table.Column<string>(type: "jsonb", maxLength: 500, nullable: true),
                    started_by = table.Column<Guid>(type: "uuid", nullable: false),
                    started_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    completed_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    webhook_url = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_bulk_jobs", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "compensation_records",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    employee_id = table.Column<Guid>(type: "uuid", nullable: false),
                    department_id = table.Column<Guid>(type: "uuid", nullable: false),
                    effective_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    base_salary = table.Column<decimal>(type: "numeric", nullable: false),
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
                name: "salary_histories",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    employee_id = table.Column<Guid>(type: "uuid", nullable: false),
                    compensation_record_id = table.Column<Guid>(type: "uuid", nullable: false),
                    previous_salary = table.Column<decimal>(type: "numeric", nullable: false),
                    new_salary = table.Column<decimal>(type: "numeric", nullable: false),
                    change_amount = table.Column<decimal>(type: "numeric", nullable: false),
                    change_percentage = table.Column<decimal>(type: "numeric", nullable: false),
                    is_high_increase = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    effective_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    change_type = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    changed_by = table.Column<Guid>(type: "uuid", nullable: false),
                    created_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_salary_histories", x => x.id);
                    table.ForeignKey(
                        name: "FK_salary_histories_compensation_records_compensation_record_id",
                        column: x => x.compensation_record_id,
                        principalTable: "compensation_records",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
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
                    national_id = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
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
                name: "idx_bulk_jobs_started_at",
                table: "bulk_jobs",
                column: "started_at");

            migrationBuilder.CreateIndex(
                name: "idx_bulk_jobs_started_by",
                table: "bulk_jobs",
                column: "started_by");

            migrationBuilder.CreateIndex(
                name: "idx_bulk_jobs_status",
                table: "bulk_jobs",
                column: "status");

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
                name: "idx_dependents_enrollment",
                table: "dependents",
                column: "benefits_enrollment_id");

            migrationBuilder.CreateIndex(
                name: "idx_salary_hist_employee",
                table: "salary_histories",
                columns: new[] { "employee_id", "effective_date" });

            migrationBuilder.CreateIndex(
                name: "IX_salary_histories_compensation_record_id",
                table: "salary_histories",
                column: "compensation_record_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "bulk_jobs");

            migrationBuilder.DropTable(
                name: "dependents");

            migrationBuilder.DropTable(
                name: "salary_histories");

            migrationBuilder.DropTable(
                name: "benefits_enrollments");

            migrationBuilder.DropTable(
                name: "compensation_records");

            migrationBuilder.DropTable(
                name: "benefits");
        }
    }
}
