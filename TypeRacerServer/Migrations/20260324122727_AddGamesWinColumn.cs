using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TypeRacerServer.Migrations
{
    /// <inheritdoc />
    public partial class AddGamesWinColumn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "GamesWin",
                table: "Users",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "GamesWin",
                table: "Users");
        }
    }
}
