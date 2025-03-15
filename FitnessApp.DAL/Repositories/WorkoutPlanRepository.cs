using FitnessApp.Core.Entities;
using FitnessApp.Core.Interfaces.IRepository;
using FitnessApp.DAL.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FitnessApp.DAL.Repositories
{
	public class WorkoutPlanRepository : IWorkoutPlanRepository
	{
		private readonly AppDbContext _context;

		public WorkoutPlanRepository(AppDbContext context)
		{
			_context = context;
		}

		public async Task<WorkoutPlan> CreateWorkoutPlanAsync(WorkoutPlan workoutPlan)
		{
			await _context.WorkoutPlans.AddAsync(workoutPlan);
			await _context.SaveChangesAsync();
			return workoutPlan;
		}

		public async Task<WorkoutPlan> GetWorkoutPlanByIdAsync(int id)
		{
			return await _context.WorkoutPlans
				.Include(wp => wp.WorkoutDays)
					.ThenInclude(wd => wd.Exercises)
						.ThenInclude(we => we.Exercise)
				.FirstOrDefaultAsync(wp => wp.Id == id);
		}

		public async Task<IEnumerable<WorkoutPlan>> GetWorkoutPlansByUserIdAsync(string userId)
		{
			return await _context.WorkoutPlans
				.Where(wp => wp.UserId == userId)
				.Include(wp => wp.WorkoutDays)
				.ToListAsync();
		}

		public async Task<IEnumerable<WorkoutPlan>> GetTemplateWorkoutPlansAsync()
		{
			return await _context.WorkoutPlans
				.Where(wp => wp.IsTemplate)
				.Include(wp => wp.WorkoutDays)
				.ToListAsync();
		}

		public async Task<IEnumerable<WorkoutPlan>> GetWorkoutPlansByDifficultyAsync(string difficultyLevel)
		{
			return await _context.WorkoutPlans
				.Where(wp => wp.DifficultyLevel == difficultyLevel)
				.Include(wp => wp.WorkoutDays)
				.ToListAsync();
		}

		public async Task<IEnumerable<WorkoutPlan>> GetWorkoutPlansByFocusAreaAsync(string focusArea)
		{
			return await _context.WorkoutPlans
				.Where(wp => wp.FocusArea == focusArea)
				.Include(wp => wp.WorkoutDays)
				.ToListAsync();
		}

		public async Task<WorkoutPlan> UpdateWorkoutPlanAsync(WorkoutPlan workoutPlan)
		{
			_context.WorkoutPlans.Update(workoutPlan);
			await _context.SaveChangesAsync();
			return workoutPlan;
		}

		public async Task DeleteWorkoutPlanAsync(int id)
		{
			var workoutPlan = await _context.WorkoutPlans.FindAsync(id);
			if (workoutPlan != null)
			{
				_context.WorkoutPlans.Remove(workoutPlan);
				await _context.SaveChangesAsync();
			}
		}

		public async Task<bool> WorkoutPlanExistsAsync(int id)
		{
			return await _context.WorkoutPlans.AnyAsync(wp => wp.Id == id);
		}

		public async Task<WorkoutPlan> CloneTemplateForUserAsync(int templateId, string userId)
		{
			var template = await _context.WorkoutPlans
				.Include(wp => wp.WorkoutDays)
					.ThenInclude(wd => wd.Exercises)
				.FirstOrDefaultAsync(wp => wp.Id == templateId && wp.IsTemplate);

			if (template == null)
				return null;
 
			var userPlan = new WorkoutPlan
			{
				Name = template.Name,
				Description = template.Description,
				UserId = userId,
				IsTemplate = false,
				DifficultyLevel = template.DifficultyLevel,
				FocusArea = template.FocusArea,
				DurationWeeks = template.DurationWeeks,
				CreatedAt = DateTime.UtcNow,
				WorkoutDays = new List<WorkoutDay>()
			};

			 
			foreach (var day in template.WorkoutDays)
			{
				var newDay = new WorkoutDay
				{
					Name = day.Name,
					DayNumber = day.DayNumber,
					Exercises = new List<WorkoutExercise>()
				};

				foreach (var exercise in day.Exercises)
				{
					newDay.Exercises.Add(new WorkoutExercise
					{
						ExerciseId = exercise.ExerciseId,
						Sets = exercise.Sets,
						RepsPerSet = exercise.RepsPerSet,
						Duration = exercise.Duration,
						OrderInWorkout = exercise.OrderInWorkout,
						RestBetweenSets = exercise.RestBetweenSets,
						Notes = exercise.Notes
					});
				}

				userPlan.WorkoutDays.Add(newDay);
			}

			await _context.WorkoutPlans.AddAsync(userPlan);
			await _context.SaveChangesAsync();
			return userPlan;
		}
	}
}