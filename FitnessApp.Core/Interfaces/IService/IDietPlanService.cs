using FitnessApp.Core.Dtos;
using FitnessApp.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FitnessApp.Core.Interfaces.IService
{
	public interface IDietPlanService
	{
		Task<IEnumerable<DietPlan>> GetAllDietPlansAsync();
		Task<DietPlan> GetDietPlanByIdAsync(int id);
		Task<IEnumerable<DietPlan>> GetDietPlansByTypeAsync(string dietType);
		Task<IEnumerable<DietPlan>> SearchDietPlansAsync(string searchTerm);
		Task<DietPlan> CreateDietPlanAsync(DietPlanDto dietPlanDto);
		Task<DietPlan> UpdateDietPlanAsync(int id, DietPlanDto dietPlanDto);
		Task<bool> DeleteDietPlanAsync(int id);
		Task<IEnumerable<DietPlan>> GetDietPlansByNutritionTargetsAsync(
			int? minCalories, int? maxCalories,
			int? minProtein, int? maxProtein,
			int? minCarbs, int? maxCarbs,
			int? minFat, int? maxFat);
	}
}