using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DLPMoneyTracker.Plugins.SQL.Migrations
{
    public partial class AddingBudgetType : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "BudgetType",
                table: "Accounts",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BudgetType",
                table: "Accounts");
        }
    }
}
