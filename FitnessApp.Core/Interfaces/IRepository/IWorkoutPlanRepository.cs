using FitnessApp.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FitnessApp.Core.Interfaces.IRepository
{
	public interface IWorkoutPlanRepository
	{
		Task<WorkoutPlan> CreateWorkoutPlanAsync(WorkoutPlan workoutPlan);
		Task<WorkoutPlan> GetWorkoutPlanByIdAsync(int id);
		Task<IEnumerable<WorkoutPlan>> GetWorkoutPlansByUserIdAsync(string userId);
		Task<IEnumerable<WorkoutPlan>> GetTemplateWorkoutPlansAsync();
		Task<IEnumerable<WorkoutPlan>> GetWorkoutPlansByDifficultyAsync(string difficultyLevel);
		Task<IEnumerable<WorkoutPlan>> GetWorkoutPlansByFocusAreaAsync(string focusArea);
		Task<WorkoutPlan> UpdateWorkoutPlanAsync(WorkoutPlan workoutPlan);
		Task DeleteWorkoutPlanAsync(int id);
		Task<bool> WorkoutPlanExistsAsync(int id);
		Task<WorkoutPlan> CloneTemplateForUserAsync(int templateId, string userId);
	}
}
