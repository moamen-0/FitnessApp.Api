using FitnessApp.Core.Dtos;
using FitnessApp.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FitnessApp.Core.Interfaces.IService
{
	public interface IMealService
	{
		Task<IEnumerable<Meal>> GetAllMealsAsync();
		Task<Meal> GetMealByIdAsync(int id);
		Task<IEnumerable<Meal>> GetMealsByDietPlanIdAsync(int dietPlanId);
		Task<IEnumerable<Meal>> GetMealsByTypeAsync(string mealType);
		Task<IEnumerable<Meal>> SearchMealsAsync(string searchTerm);
		Task<Meal> CreateMealAsync(MealDto mealDto);
		Task<Meal> UpdateMealAsync(int id, MealDto mealDto);
		Task<bool> DeleteMealAsync(int id);
		Task<IEnumerable<Meal>> GetMealsByNutritionRangeAsync(
			int? minCalories, int? maxCalories,
			decimal? minProtein, decimal? maxProtein,
			decimal? minCarbs, decimal? maxCarbs,
			decimal? minFat, decimal? maxFat);
	}
}