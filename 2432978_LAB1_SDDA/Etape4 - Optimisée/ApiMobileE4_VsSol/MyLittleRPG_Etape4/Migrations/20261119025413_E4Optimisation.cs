using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyLittleRPG_Etape4.Migrations
{
    /// <inheritdoc />
    public partial class E4Optimisation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Description",
                table: "Tile");

            migrationBuilder.DropColumn(
                name: "TypeTxt",
                table: "Tile");

            migrationBuilder.CreateIndex(
                name: "IX_TuileExploree_X_Y",
                table: "TuileExploree",
                columns: new[] { "X", "Y" });

            migrationBuilder.AddForeignKey(
                name: "FK_TuileExploree_Personnage_idPersonnage",
                table: "TuileExploree",
                column: "idPersonnage",
                principalTable: "Personnage",
                principalColumn: "idPersonnage",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TuileExploree_Tile_X_Y",
                table: "TuileExploree",
                columns: new[] { "X", "Y" },
                principalTable: "Tile",
                principalColumns: new[] { "X", "Y" },
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TuileExploree_Personnage_idPersonnage",
                table: "TuileExploree");

            migrationBuilder.DropForeignKey(
                name: "FK_TuileExploree_Tile_X_Y",
                table: "TuileExploree");

            migrationBuilder.DropIndex(
                name: "IX_TuileExploree_X_Y",
                table: "TuileExploree");

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "Tile",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "TypeTxt",
                table: "Tile",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");
        }
    }
}
