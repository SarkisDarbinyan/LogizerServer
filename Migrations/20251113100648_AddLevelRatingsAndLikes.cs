using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LogizerServer.Migrations
{
    /// <inheritdoc />
    public partial class AddLevelRatingsAndLikes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "LevelLikes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    UserId = table.Column<int>(type: "INTEGER", nullable: false),
                    LevelId = table.Column<int>(type: "INTEGER", nullable: false),
                    LikedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LevelLikes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LevelLikes_Levels_LevelId",
                        column: x => x.LevelId,
                        principalTable: "Levels",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_LevelLikes_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "LevelRatings",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    UserId = table.Column<int>(type: "INTEGER", nullable: false),
                    LevelId = table.Column<int>(type: "INTEGER", nullable: false),
                    DifficultyRating = table.Column<int>(type: "INTEGER", nullable: false),
                    RatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LevelRatings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LevelRatings_Levels_LevelId",
                        column: x => x.LevelId,
                        principalTable: "Levels",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_LevelRatings_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_LevelLikes_LevelId",
                table: "LevelLikes",
                column: "LevelId");

            migrationBuilder.CreateIndex(
                name: "IX_LevelLikes_UserId_LevelId",
                table: "LevelLikes",
                columns: new[] { "UserId", "LevelId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_LevelRatings_LevelId",
                table: "LevelRatings",
                column: "LevelId");

            migrationBuilder.CreateIndex(
                name: "IX_LevelRatings_UserId_LevelId",
                table: "LevelRatings",
                columns: new[] { "UserId", "LevelId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "LevelLikes");

            migrationBuilder.DropTable(
                name: "LevelRatings");
        }
    }
}
