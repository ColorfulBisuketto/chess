using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Chess.API.Migrations
{
    /// <inheritdoc />
    public partial class Rename_Collumns : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "PastBoards",
                table: "Boards",
                newName: "PastBoardOccurrences");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "PastBoardOccurrences",
                table: "Boards",
                newName: "PastBoards");
        }
    }
}
