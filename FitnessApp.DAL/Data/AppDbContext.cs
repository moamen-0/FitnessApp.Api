using FitnessApp.Core.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace FitnessApp.FitnessApp.DAL.Data
{
	public class AppDbContext : DbContext
	{

		public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
		{
		}

		public DbSet<AIFormAnalysis> AIFormAnalyses { get; set; } = null!;
		public DbSet<DietPlan> DietPlans { get; set; } = null!;
		public DbSet<Meal> Meals { get; set; } = null!;
		public DbSet<WorkoutPlan> WorkoutPlans { get; set; } = null!;
		public DbSet<WorkoutDay> WorkoutDays { get; set; } = null!;
		public DbSet<Exercise> Exercises { get; set; } = null!;
		public DbSet<WorkoutExercise> WorkoutExercises { get; set; } = null!;
		public DbSet<ApplicationUser> ApplicationUsers { get; set; } = null!;
		


		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{

		}
	}
}
