using Microsoft.EntityFrameworkCore.Migrations;

namespace Toggler.Infrastructure.Migrations
{
    public partial class IntializeDB : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ServiceToggles",
                columns: table => new
                {
                    UniqueId = table.Column<string>(nullable: false),
                    IsEnabled = table.Column<bool>(nullable: false),
                    ServiceName = table.Column<string>(nullable: true),
                    ServiceVersion = table.Column<string>(nullable: true),
                    ServiceDescription = table.Column<string>(nullable: true),
                    ToggleName = table.Column<string>(nullable: true),
                    ToggleDescription = table.Column<string>(nullable: true),
                    ToggleType = table.Column<int>(nullable: false),
                    IsServiceExcluded = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServiceToggles", x => x.UniqueId);
                });

            migrationBuilder.CreateTable(
                name: "Toggles",
                columns: table => new
                {
                    Name = table.Column<string>(nullable: false),
                    Description = table.Column<string>(nullable: true),
                    ToggleType = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Toggles", x => x.Name);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ServiceToggles");

            migrationBuilder.DropTable(
                name: "Toggles");
        }
    }
}
