using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyLittleRPG_Etape4.Migrations
{
    /// <inheritdoc />
    public partial class ExplorerTUile : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TuileExploree",
                columns: table => new
                {
                    idPersonnage = table.Column<int>(type: "int", nullable: false),
                    X = table.Column<int>(type: "int", nullable: false),
                    Y = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TuileExploree", x => new { x.idPersonnage, x.X, x.Y });
                })
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TuileExploree");
        }
    }
}
