using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LogizerServer.Migrations
{
    /// <inheritdoc />
    public partial class AddLevelStats : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Difficulty",
                table: "Levels",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "LikeCount",
                table: "Levels",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "PlayCount",
                table: "Levels",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Difficulty",
                table: "Levels");

            migrationBuilder.DropColumn(
                name: "LikeCount",
                table: "Levels");

            migrationBuilder.DropColumn(
                name: "PlayCount",
                table: "Levels");
        }
    }
}
