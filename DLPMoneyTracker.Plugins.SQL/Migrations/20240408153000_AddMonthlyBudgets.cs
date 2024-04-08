using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DLPMoneyTracker.Plugins.SQL.Migrations
{
    public partial class AddMonthlyBudgets : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "CurrentBudget",
                table: "Accounts",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "DefaultBudget",
                table: "Accounts",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CurrentBudget",
                table: "Accounts");

            migrationBuilder.DropColumn(
                name: "DefaultBudget",
                table: "Accounts");
        }
    }
}
