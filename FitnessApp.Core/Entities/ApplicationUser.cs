

using Microsoft.AspNetCore.Identity;

namespace FitnessApp.Core.Entities
{
	public class ApplicationUser : IdentityUser
	{
		public string FullName { get; set; } = string.Empty;
		public string Gender { get; set; } = string.Empty;
		public DateTime DateOfBirth { get; set; }
		public string? ProfileImage { get; set; }
		public int? Age { get; set; }
		public DateTime CreatedAt { get; set; } = DateTime.UtcNow;


		public virtual ICollection<InBodyResult> InBodyResults { get; set; }
		public virtual ICollection<WorkoutPlan> WorkoutPlans { get; set; }
		public virtual ICollection<CompletedWorkout> CompletedWorkouts { get; set; }
		public virtual UserGoal UserGoal { get; set; }
		public virtual ICollection<UserProgress> ProgressEntries { get; set; }

		
		public virtual ICollection<Exercise> FavoriteExercises { get; set; } = new List<Exercise>();
		public virtual ICollection<Meal> FavoriteMeals { get; set; } = new List<Meal>();

		public string? ResetPasswordToken { get; set; }
		public DateTime? ResetPasswordTokenExpiry { get; set; }
	}
}