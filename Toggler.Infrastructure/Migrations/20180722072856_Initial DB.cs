using Microsoft.EntityFrameworkCore.Migrations;

namespace Toggler.Infrastructure.Migrations
{
    public partial class InitialDB : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Services",
                columns: table => new
                {
                    Name = table.Column<string>(nullable: false),
                    Description = table.Column<string>(nullable: true),
                    Version = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Services", x => x.Name);
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

            migrationBuilder.CreateTable(
                name: "ServiceToggles",
                columns: table => new
                {
                    UniqueId = table.Column<string>(nullable: false),
                    IsEnabled = table.Column<bool>(nullable: false),
                    ServiceName = table.Column<string>(nullable: true),
                    ToggleName = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServiceToggles", x => x.UniqueId);
                    table.ForeignKey(
                        name: "FK_ServiceToggles_Services_ServiceName",
                        column: x => x.ServiceName,
                        principalTable: "Services",
                        principalColumn: "Name",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ServiceToggles_Toggles_ToggleName",
                        column: x => x.ToggleName,
                        principalTable: "Toggles",
                        principalColumn: "Name",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ServiceToggles_ServiceName",
                table: "ServiceToggles",
                column: "ServiceName");

            migrationBuilder.CreateIndex(
                name: "IX_ServiceToggles_ToggleName",
                table: "ServiceToggles",
                column: "ToggleName");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ServiceToggles");

            migrationBuilder.DropTable(
                name: "Services");

            migrationBuilder.DropTable(
                name: "Toggles");
        }
    }
}
