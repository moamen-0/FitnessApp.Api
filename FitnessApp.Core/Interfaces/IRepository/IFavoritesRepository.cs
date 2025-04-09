using FitnessApp.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FitnessApp.Core.Interfaces.IRepository
{
	public interface IFavoritesRepository
	{
		// Exercise favorites
		Task<IEnumerable<Exercise>> GetFavoriteExercisesAsync(string userId);
		Task<bool> AddExerciseToFavoritesAsync(string userId, int exerciseId);
		Task<bool> RemoveExerciseFromFavoritesAsync(string userId, int exerciseId);
		Task<bool> IsExerciseFavoritedAsync(string userId, int exerciseId);

		// Meal favorites
		Task<IEnumerable<Meal>> GetFavoriteMealsAsync(string userId);
		Task<bool> AddMealToFavoritesAsync(string userId, int mealId);
		Task<bool> RemoveMealFromFavoritesAsync(string userId, int mealId);
		Task<bool> IsMealFavoritedAsync(string userId, int mealId);
	}
}
