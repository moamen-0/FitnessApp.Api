using FitnessApp.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FitnessApp.Core.Interfaces.IRepository
{
	public interface IExerciseRepository
	{
		Task<Exercise> GetExerciseByIdAsync(int id);
		Task<IEnumerable<Exercise>> GetAllExercisesAsync();
		Task<IEnumerable<Exercise>> GetExercisesByMuscleGroupAsync(string muscleGroup);
		Task<IEnumerable<Exercise>> GetExercisesByEquipmentAsync(string equipment);
		Task<IEnumerable<Exercise>> GetExercisesByCategoryAsync(string category);
		Task<IEnumerable<Exercise>> GetExercisesByDifficultyLevelAsync(string difficultyLevel);
		Task<IEnumerable<Exercise>> GetExercisesByTypeAsync(string type);
		Task<Exercise> CreateExerciseAsync(Exercise exercise);
		Task<Exercise> UpdateExerciseAsync(Exercise exercise);
		Task DeleteExerciseAsync(int id);
		Task<bool> ExerciseExistsAsync(int id);
		Task<IEnumerable<Exercise>> SearchExercisesAsync(string searchTerm);
		Task<IEnumerable<Exercise>> GetExercisesWithAIModelAsync();
		Task<bool> UpdateAIModelReferenceAsync(int exerciseId, string aiModelReference);
	}
}
