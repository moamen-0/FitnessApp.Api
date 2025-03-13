using FitnessApp.Core.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace FitnessApp.DAL.Data
{
	public class AppDbContext : IdentityDbContext<ApplicationUser>
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
			base.OnModelCreating(modelBuilder);

			// AIFormAnalysis relationships
			modelBuilder.Entity<AIFormAnalysis>()
				.HasOne(a => a.User)
				.WithMany()
				.HasForeignKey(a => a.UserId)
				.OnDelete(DeleteBehavior.Restrict);

			modelBuilder.Entity<AIFormAnalysis>()
				.HasOne(a => a.Exercise)
				.WithMany()
				.HasForeignKey(a => a.ExerciseId);

			// ApplicationUser relationships are already defined via navigation properties

			// CompletedWorkout relationships
			modelBuilder.Entity<CompletedWorkout>()
				.HasOne(c => c.User)
				.WithMany(u => u.CompletedWorkouts)
				.HasForeignKey(c => c.UserId)
				.OnDelete(DeleteBehavior.Restrict);

			modelBuilder.Entity<CompletedWorkout>()
				.HasOne(c => c.WorkoutPlan)
				.WithMany()
				.HasForeignKey(c => c.WorkoutPlanId)
				.IsRequired(false);

			// DietPlan relationships
			modelBuilder.Entity<DietPlan>()
				.HasMany(d => d.Meals)
				.WithOne(m => m.DietPlan)
				.HasForeignKey(m => m.DietPlanId);

			// Exercise relationships
			modelBuilder.Entity<Exercise>()
				.HasMany(e => e.WorkoutExercises)
				.WithOne(we => we.Exercise)
				.HasForeignKey(we => we.ExerciseId);

			// ExercisePerformance relationships
			modelBuilder.Entity<ExercisePerformance>()
				.HasOne(ep => ep.CompletedWorkout)
				.WithMany(cw => cw.Exercises)
				.HasForeignKey(ep => ep.CompletedWorkoutId);

			modelBuilder.Entity<ExercisePerformance>()
				.HasOne(ep => ep.Exercise)
				.WithMany()
				.HasForeignKey(ep => ep.ExerciseId);

			modelBuilder.Entity<ExercisePerformance>()
				.HasOne(ep => ep.User)
				.WithMany()
				.HasForeignKey(ep => ep.UserId)
				.OnDelete(DeleteBehavior.Restrict);

			// InBodyResult relationships
			modelBuilder.Entity<InBodyResult>()
				.HasOne(ir => ir.User)
				.WithMany(u => u.InBodyResults)
				.HasForeignKey(ir => ir.UserId)
				.OnDelete(DeleteBehavior.Restrict);

			// Meal relationships already defined in DietPlan section

			// UserGoal relationships
			modelBuilder.Entity<UserGoal>()
				.HasOne(ug => ug.User)
				.WithOne(u => u.UserGoal)
				.HasForeignKey<UserGoal>(ug => ug.UserId)
				.OnDelete(DeleteBehavior.Restrict);

			// UserProgress relationships
			modelBuilder.Entity<UserProgress>()
				.HasOne(up => up.User)
				.WithMany(u => u.ProgressEntries)
				.HasForeignKey(up => up.UserId)
				.OnDelete(DeleteBehavior.Restrict);

			// WorkoutDay relationships
			modelBuilder.Entity<WorkoutDay>()
				.HasOne(wd => wd.WorkoutPlan)
				.WithMany(wp => wp.WorkoutDays)
				.HasForeignKey(wd => wd.WorkoutPlanId);

			// WorkoutExercise relationships
			modelBuilder.Entity<WorkoutExercise>()
				.HasOne(we => we.WorkoutDay)
				.WithMany(wd => wd.Exercises)
				.HasForeignKey(we => we.WorkoutDayId);

			// WorkoutPlan relationships
			modelBuilder.Entity<WorkoutPlan>()
				.HasOne(wp => wp.User)
				.WithMany(u => u.WorkoutPlans)
				.HasForeignKey(wp => wp.UserId)
			
				.OnDelete(DeleteBehavior.Restrict);
		}
	}
}
