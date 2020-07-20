using Microsoft.EntityFrameworkCore.Migrations;

namespace RedmondLoyaltyMiddleware.Migrations
{
    public partial class PromocodeAttempt_IsNeedCreateNew_Column : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsNeedCreateNew",
                table: "PromocodeUseAttempts",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsNeedCreateNew",
                table: "PromocodeUseAttempts");
        }
    }
}
