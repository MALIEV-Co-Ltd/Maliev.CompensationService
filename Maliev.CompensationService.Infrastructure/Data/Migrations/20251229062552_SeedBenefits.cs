using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Maliev.CompensationService.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class SeedBenefits : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "WaitingPeriodDays",
                table: "benefits",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.InsertData(
                table: "benefits",
                columns: new[] { "id", "name", "description", "benefit_type", "is_active", "created_date", "WaitingPeriodDays" },
                values: new object[,]
                {
                    { Guid.NewGuid(), "Health Insurance", "Comprehensive medical coverage", 0, true, DateTime.UtcNow, 90 },
                    { Guid.NewGuid(), "Dental Insurance", "Dental and oral healthcare", 1, true, DateTime.UtcNow, 90 },
                    { Guid.NewGuid(), "Vision Insurance", "Vision care and eyewear", 2, true, DateTime.UtcNow, 90 },
                    { Guid.NewGuid(), "Life Insurance", "Basic life insurance coverage", 3, true, DateTime.UtcNow, 90 },
                    { Guid.NewGuid(), "Retirement 401k", "401(k) retirement savings plan", 4, true, DateTime.UtcNow, 180 }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "WaitingPeriodDays",
                table: "benefits");
        }
    }
}
