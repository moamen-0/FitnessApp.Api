using FitnessApp.Core.Dtos;
using FitnessApp.Core.Entities;
using FitnessApp.Core.Interfaces.IService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace FitnessApp.Api.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	//[Authorize]
	public class MealController : ControllerBase
	{
		private readonly IMealService _mealService;

		public MealController(IMealService mealService)
		{
			_mealService = mealService;
		}

		// GET: api/Meal
		[HttpGet]
		public async Task<ActionResult<IEnumerable<Meal>>> GetAllMeals()
		{
			var meals = await _mealService.GetAllMealsAsync();
			return Ok(meals);
		}

		// GET: api/Meal/{id}
		[HttpGet("{id}")]
		public async Task<ActionResult<Meal>> GetMeal(int id)
		{
			var meal = await _mealService.GetMealByIdAsync(id);

			if (meal == null)
				return NotFound(new { message = "Meal not found" });

			return Ok(meal);
		}

		// GET: api/Meal/byDietPlan/{dietPlanId}
		[HttpGet("byDietPlan/{dietPlanId}")]
		public async Task<ActionResult<IEnumerable<Meal>>> GetMealsByDietPlan(int dietPlanId)
		{
			var meals = await _mealService.GetMealsByDietPlanIdAsync(dietPlanId);
			return Ok(meals);
		}

		// GET: api/Meal/byMealType/{mealType}
		[HttpGet("byMealType/{mealType}")]
		public async Task<ActionResult<IEnumerable<Meal>>> GetMealsByType(string mealType)
		{
			var meals = await _mealService.GetMealsByTypeAsync(mealType);
			return Ok(meals);
		}

		// GET: api/Meal/search
		[HttpGet("search")]
		public async Task<ActionResult<IEnumerable<Meal>>> SearchMeals([FromQuery] string term)
		{
			if (string.IsNullOrEmpty(term))
				return BadRequest(new { message = "Search term is required" });

			var meals = await _mealService.SearchMealsAsync(term);
			return Ok(meals);
		}

		// POST: api/Meal
		[HttpPost]
		public async Task<ActionResult<Meal>> CreateMeal(MealDto mealDto)
		{
			if (!ModelState.IsValid)
				return BadRequest(ModelState);

			try
			{
				var meal = await _mealService.CreateMealAsync(mealDto);
				return CreatedAtAction(nameof(GetMeal), new { id = meal.Id }, meal);
			}
			catch (InvalidOperationException ex)
			{
				return BadRequest(new { message = ex.Message });
			}
		}



		// DELETE: api/Meal/{id}
		[HttpDelete("{id}")]
		public async Task<IActionResult> DeleteMeal(int id)
		{
			var result = await _mealService.DeleteMealAsync(id);

			if (!result)
				return NotFound(new { message = "Meal not found" });

			return NoContent();
		}

		// GET: api/Meal/nutritionRange
		[HttpGet("nutritionRange")]
		public async Task<ActionResult<IEnumerable<Meal>>> GetMealsByNutritionRange(
			[FromQuery] int? minCalories, [FromQuery] int? maxCalories,
			[FromQuery] decimal? minProtein, [FromQuery] decimal? maxProtein,
			[FromQuery] decimal? minCarbs, [FromQuery] decimal? maxCarbs,
			[FromQuery] decimal? minFat, [FromQuery] decimal? maxFat)
		{
			var meals = await _mealService.GetMealsByNutritionRangeAsync(
				minCalories, maxCalories,
				minProtein, maxProtein,
				minCarbs, maxCarbs,
				minFat, maxFat);

			return Ok(meals);
		}
	}
}