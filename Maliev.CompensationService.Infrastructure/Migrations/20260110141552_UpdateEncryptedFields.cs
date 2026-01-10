using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Maliev.CompensationService.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateEncryptedFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "previous_salary",
                table: "salary_histories",
                type: "character varying",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric");

            migrationBuilder.AlterColumn<string>(
                name: "new_salary",
                table: "salary_histories",
                type: "character varying",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric");

            migrationBuilder.AlterColumn<string>(
                name: "national_id",
                table: "dependents",
                type: "character varying",
                maxLength: 500,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(500)",
                oldMaxLength: 500,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "base_salary",
                table: "compensation_records",
                type: "character varying",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "previous_salary",
                table: "salary_histories",
                type: "numeric",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying");

            migrationBuilder.AlterColumn<decimal>(
                name: "new_salary",
                table: "salary_histories",
                type: "numeric",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying");

            migrationBuilder.AlterColumn<string>(
                name: "national_id",
                table: "dependents",
                type: "character varying(500)",
                maxLength: 500,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying",
                oldMaxLength: 500,
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "base_salary",
                table: "compensation_records",
                type: "numeric",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying");
        }
    }
}
