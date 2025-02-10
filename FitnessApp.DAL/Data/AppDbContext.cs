using FitnessApp.Core.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace FitnessApp.Infrastructure.Data
{
	public class AppDbContext : DbContext
	{

		public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
		{
		}
		public DbSet<Trainer> Trainers { get; set; } = null!;
		public DbSet<Workout> Workouts { get; set; } = null!;
		public DbSet<WorkoutPlan> WorkoutPlans { get; set; } = null!;
		public DbSet<AIFormAnalysis> AIFormAnalyses { get; set; } = null!;
		public DbSet<UserProgress> UserProgresses { get; set; } = null!;
		public DbSet<Meal> Meals { get; set; } = null!;
		public DbSet<DietPlan> DietPlans { get; set; } = null!;
		public DbSet<DietPlanMeal> DietPlanMeals { get; set; } = null!;
		public DbSet<ApplicationUser> ApplicationUsers { get; set; } = null!;
		public DbSet<WorkoutPlanWorkout> workoutPlanWorkouts { get; set; } = null!;
		public DbSet<InBodyAnalysis> InBodyAnalysis { get; set; }


		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{

		}
	}
}
