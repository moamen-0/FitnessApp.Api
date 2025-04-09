using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FitnessApp.DAL.Migrations
{
    /// <inheritdoc />
    public partial class FavMealExercise : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "UserFavoriteExercises",
                columns: table => new
                {
                    ApplicationUserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    FavoriteExercisesId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserFavoriteExercises", x => new { x.ApplicationUserId, x.FavoriteExercisesId });
                    table.ForeignKey(
                        name: "FK_UserFavoriteExercises_AspNetUsers_ApplicationUserId",
                        column: x => x.ApplicationUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserFavoriteExercises_Exercises_FavoriteExercisesId",
                        column: x => x.FavoriteExercisesId,
                        principalTable: "Exercises",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserFavoriteMeals",
                columns: table => new
                {
                    ApplicationUserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    FavoriteMealsId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserFavoriteMeals", x => new { x.ApplicationUserId, x.FavoriteMealsId });
                    table.ForeignKey(
                        name: "FK_UserFavoriteMeals_AspNetUsers_ApplicationUserId",
                        column: x => x.ApplicationUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserFavoriteMeals_Meals_FavoriteMealsId",
                        column: x => x.FavoriteMealsId,
                        principalTable: "Meals",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserFavoriteExercises_FavoriteExercisesId",
                table: "UserFavoriteExercises",
                column: "FavoriteExercisesId");

            migrationBuilder.CreateIndex(
                name: "IX_UserFavoriteMeals_FavoriteMealsId",
                table: "UserFavoriteMeals",
                column: "FavoriteMealsId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserFavoriteExercises");

            migrationBuilder.DropTable(
                name: "UserFavoriteMeals");
        }
    }
}
