using FitnessApp.Core.Dtos;
using FitnessApp.Core.Entities;
using FitnessApp.Core.Interfaces.IService;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace FitnessApp.Api.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
	public class DietPlanController : ControllerBase
	{
		private readonly IDietPlanService _dietPlanService;

		public DietPlanController(IDietPlanService dietPlanService)
		{
			_dietPlanService = dietPlanService;
		}

		// GET: api/DietPlan
		[HttpGet]
		public async Task<ActionResult<IEnumerable<DietPlan>>> GetAllDietPlans()
		{
			var dietPlans = await _dietPlanService.GetAllDietPlansAsync();
			return Ok(dietPlans);
		}

		// GET: api/DietPlan/{id}
		[HttpGet("{id}")]
		public async Task<ActionResult<DietPlan>> GetDietPlan(int id)
		{
			var dietPlan = await _dietPlanService.GetDietPlanByIdAsync(id);

			if (dietPlan == null)
				return NotFound(new { message = "Diet plan not found" });

			return Ok(dietPlan);
		}

		// GET: api/DietPlan/byType/{dietType}
		[HttpGet("byType/{dietType}")]
		public async Task<ActionResult<IEnumerable<DietPlan>>> GetDietPlansByType(string dietType)
		{
			var dietPlans = await _dietPlanService.GetDietPlansByTypeAsync(dietType);
			return Ok(dietPlans);
		}

		// GET: api/DietPlan/search
		[HttpGet("search")]
		public async Task<ActionResult<IEnumerable<DietPlan>>> SearchDietPlans([FromQuery] string term)
		{
			if (string.IsNullOrEmpty(term))
				return BadRequest(new { message = "Search term is required" });

			var dietPlans = await _dietPlanService.SearchDietPlansAsync(term);
			return Ok(dietPlans);
		}

		// POST: api/DietPlan
		[HttpPost]
		public async Task<ActionResult<DietPlan>> CreateDietPlan(DietPlanDto dietPlanDto)
		{
			if (!ModelState.IsValid)
				return BadRequest(ModelState);

			try
			{
				var dietPlan = await _dietPlanService.CreateDietPlanAsync(dietPlanDto);
				return CreatedAtAction(nameof(GetDietPlan), new { id = dietPlan.Id }, dietPlan);
			}
			catch (InvalidOperationException ex)
			{
				return BadRequest(new { message = ex.Message });
			}
		}

		// Post: api/DietPlan/{id}
		[HttpPost("update/{id}")]
		public async Task<IActionResult> UpdateDietPlan(int id, DietPlanDto dietPlanDto)
		{
			if (dietPlanDto.Id.HasValue && id != dietPlanDto.Id.Value)
				return BadRequest(new { message = "ID mismatch" });

			if (!ModelState.IsValid)
				return BadRequest(ModelState);

			try
			{
				var dietPlan = await _dietPlanService.UpdateDietPlanAsync(id, dietPlanDto);
				return Ok(dietPlan);
			}
			catch (InvalidOperationException ex)
			{
				if (ex.Message.Contains("not found"))
					return NotFound(new { message = ex.Message });

				return BadRequest(new { message = ex.Message });
			}
			catch (DbUpdateConcurrencyException)
			{
				return NotFound(new { message = "Diet plan not found or has been modified" });
			}
		}

		// DELETE: api/DietPlan/{id}
		[HttpDelete("{id}")]
		public async Task<IActionResult> DeleteDietPlan(int id)
		{
			try
			{
				var result = await _dietPlanService.DeleteDietPlanAsync(id);

				if (!result)
					return NotFound(new { message = "Diet plan not found" });

				return NoContent();
			}
			catch (InvalidOperationException ex)
			{
				return BadRequest(new { message = ex.Message });
			}
		}

		// GET: api/DietPlan/nutritionTargets
		[HttpGet("nutritionTargets")]
		public async Task<ActionResult<IEnumerable<DietPlan>>> GetDietPlansByNutritionTargets(
			[FromQuery] int? minCalories, [FromQuery] int? maxCalories,
			[FromQuery] int? minProtein, [FromQuery] int? maxProtein,
			[FromQuery] int? minCarbs, [FromQuery] int? maxCarbs,
			[FromQuery] int? minFat, [FromQuery] int? maxFat)
		{
			var dietPlans = await _dietPlanService.GetDietPlansByNutritionTargetsAsync(
				minCalories, maxCalories,
				minProtein, maxProtein,
				minCarbs, maxCarbs,
				minFat, maxFat);

			return Ok(dietPlans);
		}
	}
}