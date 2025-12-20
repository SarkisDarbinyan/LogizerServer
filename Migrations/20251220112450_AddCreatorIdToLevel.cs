using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LogizerServer.Migrations
{
    /// <inheritdoc />
    public partial class AddCreatorIdToLevel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Levels_Name",
                table: "Levels");

            migrationBuilder.AddColumn<int>(
                name: "CreatorId",
                table: "Levels",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Levels_CreatorId",
                table: "Levels",
                column: "CreatorId");

            migrationBuilder.CreateIndex(
                name: "IX_Levels_Name_CreatorId",
                table: "Levels",
                columns: new[] { "Name", "CreatorId" },
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Levels_Users_CreatorId",
                table: "Levels",
                column: "CreatorId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Levels_Users_CreatorId",
                table: "Levels");

            migrationBuilder.DropIndex(
                name: "IX_Levels_CreatorId",
                table: "Levels");

            migrationBuilder.DropIndex(
                name: "IX_Levels_Name_CreatorId",
                table: "Levels");

            migrationBuilder.DropColumn(
                name: "CreatorId",
                table: "Levels");

            migrationBuilder.CreateIndex(
                name: "IX_Levels_Name",
                table: "Levels",
                column: "Name",
                unique: true);
        }
    }
}
