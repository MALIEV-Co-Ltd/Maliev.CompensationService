using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Maliev.CompensationService.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddIsHighIncrease : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SalaryHistory_compensation_records_CompensationRecordId",
                table: "SalaryHistory");

            migrationBuilder.DropPrimaryKey(
                name: "PK_SalaryHistory",
                table: "SalaryHistory");

            migrationBuilder.RenameTable(
                name: "SalaryHistory",
                newName: "salary_histories");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "salary_histories",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "PreviousSalary",
                table: "salary_histories",
                newName: "previous_salary_encrypted");

            migrationBuilder.RenameColumn(
                name: "NewSalary",
                table: "salary_histories",
                newName: "new_salary_encrypted");

            migrationBuilder.RenameColumn(
                name: "EmployeeId",
                table: "salary_histories",
                newName: "employee_id");

            migrationBuilder.RenameColumn(
                name: "EffectiveDate",
                table: "salary_histories",
                newName: "effective_date");

            migrationBuilder.RenameColumn(
                name: "CreatedDate",
                table: "salary_histories",
                newName: "created_date");

            migrationBuilder.RenameColumn(
                name: "CompensationRecordId",
                table: "salary_histories",
                newName: "compensation_record_id");

            migrationBuilder.RenameColumn(
                name: "ChangedBy",
                table: "salary_histories",
                newName: "changed_by");

            migrationBuilder.RenameColumn(
                name: "ChangeType",
                table: "salary_histories",
                newName: "change_type");

            migrationBuilder.RenameColumn(
                name: "ChangePercentage",
                table: "salary_histories",
                newName: "change_percentage");

            migrationBuilder.RenameColumn(
                name: "ChangeAmount",
                table: "salary_histories",
                newName: "change_amount");

            migrationBuilder.RenameIndex(
                name: "IX_SalaryHistory_CompensationRecordId",
                table: "salary_histories",
                newName: "IX_salary_histories_compensation_record_id");

            migrationBuilder.AlterColumn<string>(
                name: "previous_salary_encrypted",
                table: "salary_histories",
                type: "text",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric");

            migrationBuilder.AlterColumn<string>(
                name: "new_salary_encrypted",
                table: "salary_histories",
                type: "text",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric");

            migrationBuilder.AlterColumn<string>(
                name: "change_type",
                table: "salary_histories",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(500)",
                oldMaxLength: 500);

            migrationBuilder.AddColumn<bool>(
                name: "is_high_increase",
                table: "salary_histories",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddPrimaryKey(
                name: "PK_salary_histories",
                table: "salary_histories",
                column: "id");

            migrationBuilder.CreateIndex(
                name: "idx_salary_hist_employee",
                table: "salary_histories",
                columns: new[] { "employee_id", "effective_date" });

            migrationBuilder.AddForeignKey(
                name: "FK_salary_histories_compensation_records_compensation_record_id",
                table: "salary_histories",
                column: "compensation_record_id",
                principalTable: "compensation_records",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_salary_histories_compensation_records_compensation_record_id",
                table: "salary_histories");

            migrationBuilder.DropPrimaryKey(
                name: "PK_salary_histories",
                table: "salary_histories");

            migrationBuilder.DropIndex(
                name: "idx_salary_hist_employee",
                table: "salary_histories");

            migrationBuilder.DropColumn(
                name: "is_high_increase",
                table: "salary_histories");

            migrationBuilder.RenameTable(
                name: "salary_histories",
                newName: "SalaryHistory");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "SalaryHistory",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "previous_salary_encrypted",
                table: "SalaryHistory",
                newName: "PreviousSalary");

            migrationBuilder.RenameColumn(
                name: "new_salary_encrypted",
                table: "SalaryHistory",
                newName: "NewSalary");

            migrationBuilder.RenameColumn(
                name: "employee_id",
                table: "SalaryHistory",
                newName: "EmployeeId");

            migrationBuilder.RenameColumn(
                name: "effective_date",
                table: "SalaryHistory",
                newName: "EffectiveDate");

            migrationBuilder.RenameColumn(
                name: "created_date",
                table: "SalaryHistory",
                newName: "CreatedDate");

            migrationBuilder.RenameColumn(
                name: "compensation_record_id",
                table: "SalaryHistory",
                newName: "CompensationRecordId");

            migrationBuilder.RenameColumn(
                name: "changed_by",
                table: "SalaryHistory",
                newName: "ChangedBy");

            migrationBuilder.RenameColumn(
                name: "change_type",
                table: "SalaryHistory",
                newName: "ChangeType");

            migrationBuilder.RenameColumn(
                name: "change_percentage",
                table: "SalaryHistory",
                newName: "ChangePercentage");

            migrationBuilder.RenameColumn(
                name: "change_amount",
                table: "SalaryHistory",
                newName: "ChangeAmount");

            migrationBuilder.RenameIndex(
                name: "IX_salary_histories_compensation_record_id",
                table: "SalaryHistory",
                newName: "IX_SalaryHistory_CompensationRecordId");

            migrationBuilder.AlterColumn<decimal>(
                name: "PreviousSalary",
                table: "SalaryHistory",
                type: "numeric",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<decimal>(
                name: "NewSalary",
                table: "SalaryHistory",
                type: "numeric",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "ChangeType",
                table: "SalaryHistory",
                type: "character varying(500)",
                maxLength: 500,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(50)",
                oldMaxLength: 50);

            migrationBuilder.AddPrimaryKey(
                name: "PK_SalaryHistory",
                table: "SalaryHistory",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_SalaryHistory_compensation_records_CompensationRecordId",
                table: "SalaryHistory",
                column: "CompensationRecordId",
                principalTable: "compensation_records",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
