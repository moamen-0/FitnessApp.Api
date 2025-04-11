using FitnessApp.Core.Dtos;
using FitnessApp.Core.Entities;
using FitnessApp.Core.Interfaces;
using FitnessApp.Core.Interfaces.IService;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FitnessApp.BLL.Services
{
	public class MealService : IMealService
	{
		private readonly IUnitOfWork _unitOfWork;

		public MealService(IUnitOfWork unitOfWork)
		{
			_unitOfWork = unitOfWork;
		}

		public async Task<IEnumerable<Meal>> GetAllMealsAsync()
		{
			return await _unitOfWork.DbContext.Set<Meal>()
				.Include(m => m.DietPlan)
				.ToListAsync();
		}

		public async Task<Meal> GetMealByIdAsync(int id)
		{
			return await _unitOfWork.DbContext.Set<Meal>()
				.Include(m => m.DietPlan)
				.FirstOrDefaultAsync(m => m.Id == id);
		}

		public async Task<IEnumerable<Meal>> GetMealsByDietPlanIdAsync(int dietPlanId)
		{
			return await _unitOfWork.DbContext.Set<Meal>()
				.Where(m => m.DietPlanId == dietPlanId)
				.Include(m => m.DietPlan)
				.ToListAsync();
		}

		public async Task<IEnumerable<Meal>> GetMealsByTypeAsync(string mealType)
		{
			return await _unitOfWork.DbContext.Set<Meal>()
				.Where(m => m.MealType.ToLower() == mealType.ToLower())
				.Include(m => m.DietPlan)
				.ToListAsync();
		}

		public async Task<IEnumerable<Meal>> SearchMealsAsync(string searchTerm)
		{
			if (string.IsNullOrEmpty(searchTerm))
				return new List<Meal>();

			return await _unitOfWork.DbContext.Set<Meal>()
				.Where(m => m.Name.Contains(searchTerm) ||
						   m.Description.Contains(searchTerm) ||
						   m.FoodItems.Contains(searchTerm))
				.Include(m => m.DietPlan)
				.ToListAsync();
		}

		public async Task<Meal> CreateMealAsync(MealDto mealDto)
		{
			// Check if diet plan exists
			var dietPlanExists = await _unitOfWork.DbContext.Set<DietPlan>().AnyAsync(dp => dp.Id == mealDto.DietPlanId);
			if (!dietPlanExists)
				throw new InvalidOperationException("Diet plan not found");

			var meal = new Meal
			{
				DietPlanId = mealDto.DietPlanId,
				MealType = mealDto.MealType,
				Name = mealDto.Name,
				Description = mealDto.Description,
				FoodItems = mealDto.FoodItems,
				Calories = mealDto.Calories,
				Protein = mealDto.Protein,
				Carbs = mealDto.Carbs,
				Fat = mealDto.Fat
			};

			await _unitOfWork.DbContext.Set<Meal>().AddAsync(meal);
			await _unitOfWork.SaveChangesAsync();

			// Reload with included diet plan
			return await GetMealByIdAsync(meal.Id);
		}

		public async Task<Meal> UpdateMealAsync(int id, MealDto mealDto)
		{
			// Check if meal exists
			var existingMeal = await _unitOfWork.DbContext.Set<Meal>().FindAsync(id);
			if (existingMeal == null)
				throw new InvalidOperationException("Meal not found");

			// Check if diet plan exists
			var dietPlanExists = await _unitOfWork.DbContext.Set<DietPlan>().AnyAsync(dp => dp.Id == mealDto.DietPlanId);
			if (!dietPlanExists)
				throw new InvalidOperationException("Diet plan not found");

			// Update meal properties
			existingMeal.DietPlanId = mealDto.DietPlanId;
			existingMeal.MealType = mealDto.MealType;
			existingMeal.Name = mealDto.Name;
			existingMeal.Description = mealDto.Description;
			existingMeal.FoodItems = mealDto.FoodItems;
			existingMeal.Calories = mealDto.Calories;
			existingMeal.Protein = mealDto.Protein;
			existingMeal.Carbs = mealDto.Carbs;
			existingMeal.Fat = mealDto.Fat;

			_unitOfWork.DbContext.Set<Meal>().Update(existingMeal);
			await _unitOfWork.SaveChangesAsync();

			// Reload with included diet plan
			return await GetMealByIdAsync(id);
		}

		public async Task<bool> DeleteMealAsync(int id)
		{
			var meal = await _unitOfWork.DbContext.Set<Meal>().FindAsync(id);

			if (meal == null)
				return false;

			_unitOfWork.DbContext.Set<Meal>().Remove(meal);
			await _unitOfWork.SaveChangesAsync();

			return true;
		}

		public async Task<IEnumerable<Meal>> GetMealsByNutritionRangeAsync(
			int? minCalories, int? maxCalories,
			decimal? minProtein, decimal? maxProtein,
			decimal? minCarbs, decimal? maxCarbs,
			decimal? minFat, decimal? maxFat)
		{
			var query = _unitOfWork.DbContext.Set<Meal>().AsQueryable();

			if (minCalories.HasValue)
				query = query.Where(m => m.Calories >= minCalories.Value);

			if (maxCalories.HasValue)
				query = query.Where(m => m.Calories <= maxCalories.Value);

			if (minProtein.HasValue)
				query = query.Where(m => m.Protein >= minProtein.Value);

			if (maxProtein.HasValue)
				query = query.Where(m => m.Protein <= maxProtein.Value);

			if (minCarbs.HasValue)
				query = query.Where(m => m.Carbs >= minCarbs.Value);

			if (maxCarbs.HasValue)
				query = query.Where(m => m.Carbs <= maxCarbs.Value);

			if (minFat.HasValue)
				query = query.Where(m => m.Fat >= minFat.Value);

			if (maxFat.HasValue)
				query = query.Where(m => m.Fat <= maxFat.Value);

			return await query
				.Include(m => m.DietPlan)
				.ToListAsync();
		}
	}
}