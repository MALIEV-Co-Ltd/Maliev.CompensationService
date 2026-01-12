using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Maliev.CompensationService.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RemoveEncryptionAtRest : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("ALTER TABLE compensation_records ALTER COLUMN base_salary TYPE NUMERIC(18,2) USING base_salary::numeric(18,2);");
            migrationBuilder.Sql("ALTER TABLE salary_histories ALTER COLUMN previous_salary TYPE NUMERIC(18,2) USING previous_salary::numeric(18,2);");
            migrationBuilder.Sql("ALTER TABLE salary_histories ALTER COLUMN new_salary TYPE NUMERIC(18,2) USING new_salary::numeric(18,2);");
            migrationBuilder.Sql("ALTER TABLE dependents ALTER COLUMN national_id TYPE VARCHAR(50);");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("ALTER TABLE compensation_records ALTER COLUMN base_salary TYPE VARCHAR;");
            migrationBuilder.Sql("ALTER TABLE salary_histories ALTER COLUMN previous_salary TYPE VARCHAR;");
            migrationBuilder.Sql("ALTER TABLE salary_histories ALTER COLUMN new_salary TYPE VARCHAR;");
            migrationBuilder.Sql("ALTER TABLE dependents ALTER COLUMN national_id TYPE VARCHAR;");
        }
    }
}
