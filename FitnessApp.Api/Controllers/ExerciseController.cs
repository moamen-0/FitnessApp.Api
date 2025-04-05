using FitnessApp.Core.Dtos;
using FitnessApp.Core.Entities;
using FitnessApp.Core.Interfaces.IRepository;
using FitnessApp.DAL.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace FitnessApp.Api.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class ExerciseController : ControllerBase
	{
		private readonly IExerciseRepository _exerciseRepository;

		public ExerciseController(IExerciseRepository exerciseRepository)
		{
			_exerciseRepository = exerciseRepository;
		}

		// GET: api/Exercise
		[HttpGet]
		public async Task<ActionResult<IEnumerable<Exercise>>> GetAllExercises()
		{
			var exercises = await _exerciseRepository.GetAllExercisesAsync();
			return Ok(exercises);
		}

		// GET: api/Exercise/{id}
		[HttpGet("{id}")]
		public async Task<ActionResult<Exercise>> GetExercise(int id)
		{
			var exercise = await _exerciseRepository.GetExerciseByIdAsync(id);

			if (exercise == null)
				return NotFound(new { message = "Exercise not found" });

			return Ok(exercise);
		}

		// GET: api/Exercise/search
		[HttpGet("search")]
		public async Task<ActionResult<IEnumerable<Exercise>>> SearchExercises([FromQuery] string term)
		{
			if (string.IsNullOrEmpty(term))
				return BadRequest(new { message = "Search term is required" });

			var exercises = await _exerciseRepository.SearchExercisesAsync(term);
			return Ok(exercises);
		}

		// GET: api/Exercise/muscleGroup/{group}
		[HttpGet("muscleGroup/{group}")]
		public async Task<ActionResult<IEnumerable<Exercise>>> GetExercisesByMuscleGroup(string group)
		{
			var exercises = await _exerciseRepository.GetExercisesByMuscleGroupAsync(group);
			return Ok(exercises);
		}

		// GET: api/Exercise/equipment/{equipment}
		[HttpGet("equipment/{equipment}")]
		public async Task<ActionResult<IEnumerable<Exercise>>> GetExercisesByEquipment(string equipment)
		{
			var exercises = await _exerciseRepository.GetExercisesByEquipmentAsync(equipment);
			return Ok(exercises);
		}

		// GET: api/Exercise/category/{category}
		[HttpGet("category/{category}")]
		public async Task<ActionResult<IEnumerable<Exercise>>> GetExercisesByCategory(string category)
		{
			var exercises = await _exerciseRepository.GetExercisesByCategoryAsync(category);
			return Ok(exercises);
		}

		// GET: api/Exercise/difficulty/{level}
		[HttpGet("difficulty/{level}")]
		public async Task<ActionResult<IEnumerable<Exercise>>> GetExercisesByDifficultyLevel(string level)
		{
			var exercises = await _exerciseRepository.GetExercisesByDifficultyLevelAsync(level);
			return Ok(exercises);
		}

		// GET: api/Exercise/type/{type}
		[HttpGet("type/{type}")]
		public async Task<ActionResult<IEnumerable<Exercise>>> GetExercisesByType(string type)
		{
			var exercises = await _exerciseRepository.GetExercisesByTypeAsync(type);
			return Ok(exercises);
		}

		// GET: api/Exercise/ai
		[HttpGet("ai")]
		public async Task<ActionResult<IEnumerable<Exercise>>> GetExercisesWithAIModel()
		{
			var exercises = await _exerciseRepository.GetExercisesWithAIModelAsync();
			return Ok(exercises);
		}

		// POST: api/Exercise
		//[Authorize(Roles = Roles.Admin)]
		[HttpPost]
		public async Task<IActionResult> CreateExercise(ExerciseDTO exerciseDTO)
		{
			if (!ModelState.IsValid)
				return BadRequest(ModelState);

			var exercise = new Exercise
			{
				Name = exerciseDTO.Name,
				Description = exerciseDTO.Description,
				Category = exerciseDTO.Category,
				MuscleGroup = exerciseDTO.MuscleGroup,
				Equipment = exerciseDTO.Equipment,
				DifficultyLevel = exerciseDTO.DifficultyLevel,
				InstructionSteps = exerciseDTO.InstructionSteps,
				VideoUrl = exerciseDTO.VideoUrl,
				ImageUrl = exerciseDTO.ImageUrl,
				AIModelReference = exerciseDTO.AiModelReference
			};

			var createdExercise = await _exerciseRepository.CreateExerciseAsync(exercise);
			return CreatedAtAction(nameof(GetExercise), new { id = createdExercise.Id }, createdExercise);
		}

		// PUT: api/Exercise/{id}
		//[Authorize(Roles = Roles.Admin)]
		[HttpPut("{id}")]
		public async Task<IActionResult> UpdateExercise(int id, ExerciseDTO exerciseDTO)
		{
			if (exerciseDTO.Id.HasValue && id != exerciseDTO.Id.Value)
				return BadRequest("ID in URL does not match ID in request body");

			if (!ModelState.IsValid)
				return BadRequest(ModelState);

			var existingExercise = await _exerciseRepository.GetExerciseByIdAsync(id);
			if (existingExercise == null)
				return NotFound();

			// Update the existing exercise's properties
			existingExercise.Name = exerciseDTO.Name;
			existingExercise.Description = exerciseDTO.Description;
			existingExercise.Category = exerciseDTO.Category;
			existingExercise.MuscleGroup = exerciseDTO.MuscleGroup;
			existingExercise.Equipment = exerciseDTO.Equipment;
			existingExercise.DifficultyLevel = exerciseDTO.DifficultyLevel;
			existingExercise.InstructionSteps = exerciseDTO.InstructionSteps;
			existingExercise.VideoUrl = exerciseDTO.VideoUrl;
			existingExercise.ImageUrl = exerciseDTO.ImageUrl;
			existingExercise.AIModelReference = exerciseDTO.AiModelReference;

			var updatedExercise = await _exerciseRepository.UpdateExerciseAsync(existingExercise);

			// Return the updated exercise
			return Ok(updatedExercise);
		}

		// DELETE: api/Exercise/{id}
		//[Authorize(Roles = Roles.Admin)]
		[HttpDelete("{id}")]
		public async Task<IActionResult> DeleteExercise(int id)
		{
			var exists = await _exerciseRepository.ExerciseExistsAsync(id);
			if (!exists)
				return NotFound(new { message = "Exercise not found" });

			await _exerciseRepository.DeleteExerciseAsync(id);
			return NoContent();
		}

		// PUT: api/Exercise/{id}/aimodel
		//[Authorize(Roles = Roles.Admin)]
		[HttpPut("{id}/aimodel")]
		public async Task<IActionResult> UpdateAIModelReference(int id, [FromBody] AIModelReferenceUpdate update)
		{
			if (string.IsNullOrEmpty(update.AIModelReference))
				return BadRequest(new { message = "AI model reference is required" });

			var exists = await _exerciseRepository.ExerciseExistsAsync(id);
			if (!exists)
				return NotFound(new { message = "Exercise not found" });

			var success = await _exerciseRepository.UpdateAIModelReferenceAsync(id, update.AIModelReference);
			if (!success)
				return BadRequest(new { message = "Failed to update AI model reference" });

			return NoContent();
		}
	}

	public class AIModelReferenceUpdate
	{
		public string AIModelReference { get; set; }
	}
}