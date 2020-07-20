using Microsoft.EntityFrameworkCore.Migrations;

namespace RedmondLoyaltyMiddleware.Migrations
{
    public partial class PromocodeAttemptsErrors : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Error",
                table: "PromocodeUseAttempts",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsError",
                table: "PromocodeUseAttempts",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Error",
                table: "PromocodeUseAttempts");

            migrationBuilder.DropColumn(
                name: "IsError",
                table: "PromocodeUseAttempts");
        }
    }
}
