using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Maliev.CompensationService.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddBulkJobsTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "bulk_jobs");
        }
    }
}
