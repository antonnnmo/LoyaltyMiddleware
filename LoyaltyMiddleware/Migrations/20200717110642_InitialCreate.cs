using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace RedmondLoyaltyMiddleware.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PromocodePools",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CanUseManyTimes = table.Column<bool>(nullable: false),
                    IsActual = table.Column<bool>(nullable: false),
                    UseCountRestriction = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PromocodePools", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PromocodeUseAttempts",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Promocode = table.Column<string>(nullable: true),
                    Date = table.Column<DateTime>(nullable: false),
                    PoolId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PromocodeUseAttempts", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PromocodePools");

            migrationBuilder.DropTable(
                name: "PromocodeUseAttempts");
        }
    }
}
