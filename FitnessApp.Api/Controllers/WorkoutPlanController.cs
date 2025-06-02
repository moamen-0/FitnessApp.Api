using FitnessApp.Core.Entities;
using FitnessApp.Core.Interfaces.IRepository;
using FitnessApp.Core.Interfaces.IService;
using FitnessApp.DAL.Utilities;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using FitnessApp.Core.Dtos;
using FitnessApp.Core.Extensions;

namespace FitnessApp.Api.Controllers
{	[Route("api/[controller]")]
	[ApiController]
	[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
	public class WorkoutPlanController : ControllerBase
	{
		private readonly IWorkoutPlanRepository _workoutPlanRepository;
		private readonly IWorkoutPlanGenerationService _workoutPlanGenerationService;

		public WorkoutPlanController(
			IWorkoutPlanRepository workoutPlanRepository,
			IWorkoutPlanGenerationService workoutPlanGenerationService)
		{
			_workoutPlanRepository = workoutPlanRepository;
			_workoutPlanGenerationService = workoutPlanGenerationService;
		}

		// GET: api/WorkoutPlan
		[HttpGet]
		public async Task<ActionResult<IEnumerable<WorkoutPlan>>> GetUserWorkoutPlans()
		{
			var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
			var workoutPlans = await _workoutPlanRepository.GetWorkoutPlansByUserIdAsync(userId);
			return Ok(workoutPlans);
		}

		// GET: api/WorkoutPlan/{id}
		[HttpGet("{id}")]
		public async Task<ActionResult<WorkoutPlan>> GetWorkoutPlan(int id)
		{
			var workoutPlan = await _workoutPlanRepository.GetWorkoutPlanByIdAsync(id);

			if (workoutPlan == null)
				return NotFound(new { message = "Workout plan not found" });

			var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

			 
			if (workoutPlan.UserId != userId && !User.IsInRole(Roles.Admin) && !workoutPlan.IsTemplate)
				return Forbid();

			return Ok(workoutPlan);
		}

		// GET: api/WorkoutPlan/templates
		[HttpGet("templates")]
		public async Task<ActionResult<IEnumerable<WorkoutPlan>>> GetTemplateWorkoutPlans()
		{
			var templates = await _workoutPlanRepository.GetTemplateWorkoutPlansAsync();
			return Ok(templates);
		}

		// POST: api/WorkoutPlan
		[HttpPost]
		public async Task<ActionResult<WorkoutPlan>> CreateWorkoutPlan(WorkoutPlan workoutPlan)
		{
			var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
			workoutPlan.UserId = userId;
			workoutPlan.CreatedAt = DateTime.UtcNow;

			var createdPlan = await _workoutPlanRepository.CreateWorkoutPlanAsync(workoutPlan);			return CreatedAtAction(nameof(GetWorkoutPlan), new { id = createdPlan.Id }, createdPlan);
		}

		// POST: api/WorkoutPlan/generate
		[HttpPost("generate")]
		[AllowAnonymous]
		public async Task<ActionResult<WorkoutPlanDto>> GenerateWorkoutPlan([FromBody] WorkoutPlanGenerationRequest request)
		{
			if (string.IsNullOrEmpty(request.Goal) || string.IsNullOrEmpty(request.FitnessLevel))
				return BadRequest(new { message = "Goal and fitness level are required" });

			// Get userId if user is authenticated, otherwise use null for anonymous generation
			var userId = User?.Identity?.IsAuthenticated == true ? User.FindFirstValue(ClaimTypes.NameIdentifier) : null;
			var generatedPlan = await _workoutPlanGenerationService.GenerateCustomWorkoutPlanAsync(
				userId, request.Goal, request.FitnessLevel);

			return CreatedAtAction(nameof(GetWorkoutPlan), new { id = generatedPlan.Id }, generatedPlan.ToDto());
		}

		// POST: api/WorkoutPlan/clone/{templateId}
		[HttpPost("clone/{templateId}")]
		public async Task<ActionResult<WorkoutPlan>> CloneTemplateWorkoutPlan(int templateId)
		{
			var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
			var clonedPlan = await _workoutPlanRepository.CloneTemplateForUserAsync(templateId, userId);

			if (clonedPlan == null)
				return NotFound(new { message = "Template workout plan not found" });

			return CreatedAtAction(nameof(GetWorkoutPlan), new { id = clonedPlan.Id }, clonedPlan);
		}

		// PUT: api/WorkoutPlan/{id}
		[HttpPut("{id}")]
		public async Task<IActionResult> UpdateWorkoutPlan(int id, WorkoutPlan workoutPlan)
		{
			if (id != workoutPlan.Id)
				return BadRequest(new { message = "ID mismatch" });

			var existingPlan = await _workoutPlanRepository.GetWorkoutPlanByIdAsync(id);

			if (existingPlan == null)
				return NotFound(new { message = "Workout plan not found" });

			var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
 
			if (existingPlan.UserId != userId && !User.IsInRole(Roles.Admin))
				return Forbid();

			 
			workoutPlan.UserId = existingPlan.UserId;
			workoutPlan.CreatedAt = existingPlan.CreatedAt;

			await _workoutPlanRepository.UpdateWorkoutPlanAsync(workoutPlan);
			return NoContent();
		}

		// DELETE: api/WorkoutPlan/{id}
		[HttpDelete("{id}")]
		public async Task<IActionResult> DeleteWorkoutPlan(int id)
		{
			var workoutPlan = await _workoutPlanRepository.GetWorkoutPlanByIdAsync(id);

			if (workoutPlan == null)
				return NotFound(new { message = "Workout plan not found" });

			var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

			 
			if (workoutPlan.UserId != userId && !User.IsInRole(Roles.Admin))
				return Forbid();

			 
			if (workoutPlan.IsTemplate && !User.IsInRole(Roles.Admin))
				return Forbid();

			await _workoutPlanRepository.DeleteWorkoutPlanAsync(id);
			return NoContent();
		}

		// GET: api/WorkoutPlan/difficulty/{level}
		[HttpGet("difficulty/{level}")]
		public async Task<ActionResult<IEnumerable<WorkoutPlan>>> GetWorkoutPlansByDifficulty(string level)
		{
			var workoutPlans = await _workoutPlanRepository.GetWorkoutPlansByDifficultyAsync(level);
			return Ok(workoutPlans);
		}

		// GET: api/WorkoutPlan/focus/{area}
		[HttpGet("focus/{area}")]
		public async Task<ActionResult<IEnumerable<WorkoutPlan>>> GetWorkoutPlansByFocusArea(string area)
		{
			var workoutPlans = await _workoutPlanRepository.GetWorkoutPlansByFocusAreaAsync(area);
			return Ok(workoutPlans);
		}
	}
	public class WorkoutPlanGenerationRequest
	{
		public required string Goal { get; set; }  
		public required string FitnessLevel { get; set; }  
	}
}