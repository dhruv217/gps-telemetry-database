using Microsoft.EntityFrameworkCore.Migrations;

namespace gps_telemetry_database_test.Migrations
{
    public partial class Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "GeoPoints",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(nullable: true),
                    Latitude = table.Column<decimal>(type: "Decimal (8,6)", nullable: false),
                    Longitude = table.Column<decimal>(type: "Decimal (9,6)", nullable: false),
                    Address = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GeoPoints", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "GeoPoints");
        }
    }
}
