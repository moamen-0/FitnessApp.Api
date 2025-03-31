using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FitnessApp.DAL.Migrations
{
    /// <inheritdoc />
    public partial class repairs : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CompletedWorkout_AspNetUsers_UserId",
                table: "CompletedWorkout");

            migrationBuilder.DropForeignKey(
                name: "FK_CompletedWorkout_WorkoutPlans_WorkoutPlanId",
                table: "CompletedWorkout");

            migrationBuilder.DropForeignKey(
                name: "FK_ExercisePerformance_AspNetUsers_UserId",
                table: "ExercisePerformance");

            migrationBuilder.DropForeignKey(
                name: "FK_ExercisePerformance_CompletedWorkout_CompletedWorkoutId",
                table: "ExercisePerformance");

            migrationBuilder.DropForeignKey(
                name: "FK_ExercisePerformance_Exercises_ExerciseId",
                table: "ExercisePerformance");

            migrationBuilder.DropForeignKey(
                name: "FK_UserGoal_AspNetUsers_UserId",
                table: "UserGoal");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UserGoal",
                table: "UserGoal");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ExercisePerformance",
                table: "ExercisePerformance");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CompletedWorkout",
                table: "CompletedWorkout");

            migrationBuilder.RenameTable(
                name: "UserGoal",
                newName: "UserGoals");

            migrationBuilder.RenameTable(
                name: "ExercisePerformance",
                newName: "ExercisePerformances");

            migrationBuilder.RenameTable(
                name: "CompletedWorkout",
                newName: "CompletedWorkouts");

            migrationBuilder.RenameIndex(
                name: "IX_UserGoal_UserId",
                table: "UserGoals",
                newName: "IX_UserGoals_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_ExercisePerformance_UserId",
                table: "ExercisePerformances",
                newName: "IX_ExercisePerformances_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_ExercisePerformance_ExerciseId",
                table: "ExercisePerformances",
                newName: "IX_ExercisePerformances_ExerciseId");

            migrationBuilder.RenameIndex(
                name: "IX_ExercisePerformance_CompletedWorkoutId",
                table: "ExercisePerformances",
                newName: "IX_ExercisePerformances_CompletedWorkoutId");

            migrationBuilder.RenameIndex(
                name: "IX_CompletedWorkout_WorkoutPlanId",
                table: "CompletedWorkouts",
                newName: "IX_CompletedWorkouts_WorkoutPlanId");

            migrationBuilder.RenameIndex(
                name: "IX_CompletedWorkout_UserId",
                table: "CompletedWorkouts",
                newName: "IX_CompletedWorkouts_UserId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserGoals",
                table: "UserGoals",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ExercisePerformances",
                table: "ExercisePerformances",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_CompletedWorkouts",
                table: "CompletedWorkouts",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_CompletedWorkouts_AspNetUsers_UserId",
                table: "CompletedWorkouts",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_CompletedWorkouts_WorkoutPlans_WorkoutPlanId",
                table: "CompletedWorkouts",
                column: "WorkoutPlanId",
                principalTable: "WorkoutPlans",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ExercisePerformances_AspNetUsers_UserId",
                table: "ExercisePerformances",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ExercisePerformances_CompletedWorkouts_CompletedWorkoutId",
                table: "ExercisePerformances",
                column: "CompletedWorkoutId",
                principalTable: "CompletedWorkouts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ExercisePerformances_Exercises_ExerciseId",
                table: "ExercisePerformances",
                column: "ExerciseId",
                principalTable: "Exercises",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserGoals_AspNetUsers_UserId",
                table: "UserGoals",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CompletedWorkouts_AspNetUsers_UserId",
                table: "CompletedWorkouts");

            migrationBuilder.DropForeignKey(
                name: "FK_CompletedWorkouts_WorkoutPlans_WorkoutPlanId",
                table: "CompletedWorkouts");

            migrationBuilder.DropForeignKey(
                name: "FK_ExercisePerformances_AspNetUsers_UserId",
                table: "ExercisePerformances");

            migrationBuilder.DropForeignKey(
                name: "FK_ExercisePerformances_CompletedWorkouts_CompletedWorkoutId",
                table: "ExercisePerformances");

            migrationBuilder.DropForeignKey(
                name: "FK_ExercisePerformances_Exercises_ExerciseId",
                table: "ExercisePerformances");

            migrationBuilder.DropForeignKey(
                name: "FK_UserGoals_AspNetUsers_UserId",
                table: "UserGoals");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UserGoals",
                table: "UserGoals");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ExercisePerformances",
                table: "ExercisePerformances");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CompletedWorkouts",
                table: "CompletedWorkouts");

            migrationBuilder.RenameTable(
                name: "UserGoals",
                newName: "UserGoal");

            migrationBuilder.RenameTable(
                name: "ExercisePerformances",
                newName: "ExercisePerformance");

            migrationBuilder.RenameTable(
                name: "CompletedWorkouts",
                newName: "CompletedWorkout");

            migrationBuilder.RenameIndex(
                name: "IX_UserGoals_UserId",
                table: "UserGoal",
                newName: "IX_UserGoal_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_ExercisePerformances_UserId",
                table: "ExercisePerformance",
                newName: "IX_ExercisePerformance_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_ExercisePerformances_ExerciseId",
                table: "ExercisePerformance",
                newName: "IX_ExercisePerformance_ExerciseId");

            migrationBuilder.RenameIndex(
                name: "IX_ExercisePerformances_CompletedWorkoutId",
                table: "ExercisePerformance",
                newName: "IX_ExercisePerformance_CompletedWorkoutId");

            migrationBuilder.RenameIndex(
                name: "IX_CompletedWorkouts_WorkoutPlanId",
                table: "CompletedWorkout",
                newName: "IX_CompletedWorkout_WorkoutPlanId");

            migrationBuilder.RenameIndex(
                name: "IX_CompletedWorkouts_UserId",
                table: "CompletedWorkout",
                newName: "IX_CompletedWorkout_UserId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserGoal",
                table: "UserGoal",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ExercisePerformance",
                table: "ExercisePerformance",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_CompletedWorkout",
                table: "CompletedWorkout",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_CompletedWorkout_AspNetUsers_UserId",
                table: "CompletedWorkout",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_CompletedWorkout_WorkoutPlans_WorkoutPlanId",
                table: "CompletedWorkout",
                column: "WorkoutPlanId",
                principalTable: "WorkoutPlans",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ExercisePerformance_AspNetUsers_UserId",
                table: "ExercisePerformance",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ExercisePerformance_CompletedWorkout_CompletedWorkoutId",
                table: "ExercisePerformance",
                column: "CompletedWorkoutId",
                principalTable: "CompletedWorkout",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ExercisePerformance_Exercises_ExerciseId",
                table: "ExercisePerformance",
                column: "ExerciseId",
                principalTable: "Exercises",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserGoal_AspNetUsers_UserId",
                table: "UserGoal",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
