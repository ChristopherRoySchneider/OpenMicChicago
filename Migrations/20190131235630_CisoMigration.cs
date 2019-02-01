using Microsoft.EntityFrameworkCore.Migrations;

namespace OpenMicChicago.Migrations
{
    public partial class CisoMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "longitude",
                table: "Venues",
                newName: "Longitude");

            migrationBuilder.RenameColumn(
                name: "latitude",
                table: "Venues",
                newName: "Latitude");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Longitude",
                table: "Venues",
                newName: "longitude");

            migrationBuilder.RenameColumn(
                name: "Latitude",
                table: "Venues",
                newName: "latitude");
        }
    }
}
