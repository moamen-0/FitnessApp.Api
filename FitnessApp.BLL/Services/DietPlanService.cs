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
	public class DietPlanService : IDietPlanService
	{
		private readonly IUnitOfWork _unitOfWork;

		public DietPlanService(IUnitOfWork unitOfWork)
		{
			_unitOfWork = unitOfWork;
		}

		public async Task<IEnumerable<DietPlan>> GetAllDietPlansAsync()
		{
			return await _unitOfWork.DbContext.Set<DietPlan>()
				.Include(dp => dp.Meals)
				.ToListAsync();
		}

		public async Task<DietPlan> GetDietPlanByIdAsync(int id)
		{
			return await _unitOfWork.DbContext.Set<DietPlan>()
				.Include(dp => dp.Meals)
				.FirstOrDefaultAsync(dp => dp.Id == id);
		}

		public async Task<IEnumerable<DietPlan>> GetDietPlansByTypeAsync(string dietType)
		{
			return await _unitOfWork.DbContext.Set<DietPlan>()
				.Where(dp => dp.DietType.ToLower() == dietType.ToLower())
				.Include(dp => dp.Meals)
				.ToListAsync();
		}

		public async Task<IEnumerable<DietPlan>> SearchDietPlansAsync(string searchTerm)
		{
			if (string.IsNullOrEmpty(searchTerm))
				return new List<DietPlan>();

			return await _unitOfWork.DbContext.Set<DietPlan>()
				.Where(dp => dp.Name.Contains(searchTerm) ||
						   dp.Description.Contains(searchTerm) ||
						   dp.DietType.Contains(searchTerm))
				.Include(dp => dp.Meals)
				.ToListAsync();
		}

		public async Task<DietPlan> CreateDietPlanAsync(DietPlanDto dietPlanDto)
		{
			var dietPlan = new DietPlan
			{
				Name = dietPlanDto.Name,
				Description = dietPlanDto.Description,
				DietType = dietPlanDto.DietType,
				DailyCalorieTarget = dietPlanDto.DailyCalorieTarget,
				DailyProteinTarget = dietPlanDto.DailyProteinTarget,
				DailyCarbsTarget = dietPlanDto.DailyCarbsTarget,
				DailyFatTarget = dietPlanDto.DailyFatTarget,
				DurationDays = dietPlanDto.DurationDays,
				CreatedAt = DateTime.UtcNow,
				Meals = new List<Meal>()
			};

			await _unitOfWork.DbContext.Set<DietPlan>().AddAsync(dietPlan);
			await _unitOfWork.SaveChangesAsync();

			// Reload with meals included
			return await GetDietPlanByIdAsync(dietPlan.Id);
		}

		public async Task<DietPlan> UpdateDietPlanAsync(int id, DietPlanDto dietPlanDto)
		{
			// Check if diet plan exists
			var existingDietPlan = await _unitOfWork.DbContext.Set<DietPlan>().FindAsync(id);
			if (existingDietPlan == null)
				throw new InvalidOperationException("Diet plan not found");

			// Update diet plan properties
			existingDietPlan.Name = dietPlanDto.Name;
			existingDietPlan.Description = dietPlanDto.Description;
			existingDietPlan.DietType = dietPlanDto.DietType;
			existingDietPlan.DailyCalorieTarget = dietPlanDto.DailyCalorieTarget;
			existingDietPlan.DailyProteinTarget = dietPlanDto.DailyProteinTarget;
			existingDietPlan.DailyCarbsTarget = dietPlanDto.DailyCarbsTarget;
			existingDietPlan.DailyFatTarget = dietPlanDto.DailyFatTarget;
			existingDietPlan.DurationDays = dietPlanDto.DurationDays;

			_unitOfWork.DbContext.Set<DietPlan>().Update(existingDietPlan);
			await _unitOfWork.SaveChangesAsync();

			// Reload with meals included
			return await GetDietPlanByIdAsync(id);
		}

		public async Task<bool> DeleteDietPlanAsync(int id)
		{
			var dietPlan = await _unitOfWork.DbContext.Set<DietPlan>().FindAsync(id);

			if (dietPlan == null)
				return false;

			// Check if there are any meals associated with this diet plan
			var hasMeals = await _unitOfWork.DbContext.Set<Meal>().AnyAsync(m => m.DietPlanId == id);
			if (hasMeals)
				throw new InvalidOperationException("Cannot delete diet plan with associated meals");

			_unitOfWork.DbContext.Set<DietPlan>().Remove(dietPlan);
			await _unitOfWork.SaveChangesAsync();

			return true;
		}

		public async Task<IEnumerable<DietPlan>> GetDietPlansByNutritionTargetsAsync(
			int? minCalories, int? maxCalories,
			int? minProtein, int? maxProtein,
			int? minCarbs, int? maxCarbs,
			int? minFat, int? maxFat)
		{
			var query = _unitOfWork.DbContext.Set<DietPlan>().AsQueryable();

			if (minCalories.HasValue)
				query = query.Where(dp => dp.DailyCalorieTarget >= minCalories.Value);

			if (maxCalories.HasValue)
				query = query.Where(dp => dp.DailyCalorieTarget <= maxCalories.Value);

			if (minProtein.HasValue)
				query = query.Where(dp => dp.DailyProteinTarget >= minProtein.Value);

			if (maxProtein.HasValue)
				query = query.Where(dp => dp.DailyProteinTarget <= maxProtein.Value);

			if (minCarbs.HasValue)
				query = query.Where(dp => dp.DailyCarbsTarget >= minCarbs.Value);

			if (maxCarbs.HasValue)
				query = query.Where(dp => dp.DailyCarbsTarget <= maxCarbs.Value);

			if (minFat.HasValue)
				query = query.Where(dp => dp.DailyFatTarget >= minFat.Value);

			if (maxFat.HasValue)
				query = query.Where(dp => dp.DailyFatTarget <= maxFat.Value);

			return await query
				.Include(dp => dp.Meals)
				.ToListAsync();
		}
	}
}