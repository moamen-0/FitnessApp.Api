using FitnessApp.Core.Entities;
using FitnessApp.Core.Interfaces.IRepository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace FitnessApp.Api.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	[Authorize]
	public class InBodyResultController : ControllerBase
	{
		private readonly IInBodyResultRepository _inBodyResultRepository;

		public InBodyResultController(IInBodyResultRepository inBodyResultRepository)
		{
			_inBodyResultRepository = inBodyResultRepository;
		}

		// GET: api/InBodyResult
		[HttpGet]
		public async Task<ActionResult<IEnumerable<InBodyResult>>> GetUserInBodyResults()
		{
			var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
			var results = await _inBodyResultRepository.GetInBodyResultsByUserIdAsync(userId);
			return Ok(results);
		}

		// GET: api/InBodyResult/{id}
		[HttpGet("{id}")]
		public async Task<ActionResult<InBodyResult>> GetInBodyResult(int id)
		{
			var inBodyResult = await _inBodyResultRepository.GetInBodyResultByIdAsync(id);

			if (inBodyResult == null)
				return NotFound(new { message = "InBody result not found" });

			var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

			// Ensure the user can only access their own InBody results
			if (inBodyResult.UserId != userId)
				return Forbid();

			return Ok(inBodyResult);
		}

		// GET: api/InBodyResult/latest
		[HttpGet("latest")]
		public async Task<ActionResult<InBodyResult>> GetLatestInBodyResult()
		{
			var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
			var latestResult = await _inBodyResultRepository.GetLatestInBodyResultAsync(userId);

			if (latestResult == null)
				return NotFound(new { message = "No InBody results found for user" });

			return Ok(latestResult);
		}

		// GET: api/InBodyResult/baseline
		[HttpGet("baseline")]
		public async Task<ActionResult<InBodyResult>> GetBaselineInBodyResult()
		{
			var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
			var baseline = await _inBodyResultRepository.GetBaseline(userId);

			if (baseline == null)
				return NotFound(new { message = "No baseline InBody result found for user" });

			return Ok(baseline);
		}

		// GET: api/InBodyResult/bmi
		[HttpGet("bmi")]
		public async Task<ActionResult<decimal>> GetBMI()
		{
			var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
			var bmi = await _inBodyResultRepository.CalculateBMIAsync(userId);

			if (bmi == 0)
				return NotFound(new { message = "Unable to calculate BMI. InBody result may be missing." });

			return Ok(new { BMI = bmi });
		}

		// GET: api/InBodyResult/tdee
		[HttpGet("tdee")]
		public async Task<ActionResult<decimal>> GetTDEE()
		{
			var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
			var tdee = await _inBodyResultRepository.CalculateTDEEAsync(userId);

			if (tdee == 0)
				return NotFound(new { message = "Unable to calculate TDEE. InBody result or activity level may be missing." });

			return Ok(new { TDEE = tdee });
		}

		// GET: api/InBodyResult/trends
		[HttpGet("trends")]
		public async Task<ActionResult<Dictionary<string, List<decimal>>>> GetBodyCompositionTrends([FromQuery] DateTime? startDate, [FromQuery] DateTime? endDate)
		{
			var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

			// Default to last 30 days if no dates provided
			var start = startDate ?? DateTime.UtcNow.AddDays(-30);
			var end = endDate ?? DateTime.UtcNow;

			var trends = await _inBodyResultRepository.GetBodyCompositionTrendsAsync(userId, start, end);

			if (trends.Count == 0 || !trends.Any(t => t.Value.Any()))
				return NotFound(new { message = "No trend data found for the specified period" });

			return Ok(trends);
		}

		// GET: api/InBodyResult/progress
		[HttpGet("progress")]
		public async Task<ActionResult<Dictionary<string, decimal>>> GetProgressComparison()
		{
			var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
			var progress = await _inBodyResultRepository.GetProgressComparisonAsync(userId);

			if (progress.Count == 0)
				return NotFound(new { message = "Unable to calculate progress. Insufficient InBody results." });

			return Ok(progress);
		}

		// POST: api/InBodyResult
		[HttpPost]
		public async Task<ActionResult<InBodyResult>> CreateInBodyResult(InBodyResult inBodyResult)
		{
			var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

			// Set the current user ID to ensure users can only add results for themselves
			inBodyResult.UserId = userId;

			// Set the date to current UTC time if not provided
			if (inBodyResult.Date == default)
				inBodyResult.Date = DateTime.UtcNow;

			var createdResult = await _inBodyResultRepository.CreateInBodyResultAsync(inBodyResult);
			return CreatedAtAction(nameof(GetInBodyResult), new { id = createdResult.Id }, createdResult);
		}

		// PUT: api/InBodyResult/{id}
		[HttpPut("{id}")]
		public async Task<IActionResult> UpdateInBodyResult(int id, InBodyResult inBodyResult)
		{
			if (id != inBodyResult.Id)
				return BadRequest(new { message = "ID mismatch" });

			var existingResult = await _inBodyResultRepository.GetInBodyResultByIdAsync(id);

			if (existingResult == null)
				return NotFound(new { message = "InBody result not found" });

			var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

			// Ensure the user can only update their own InBody results
			if (existingResult.UserId != userId)
				return Forbid();

			// Preserve the original user ID
			inBodyResult.UserId = existingResult.UserId;

			await _inBodyResultRepository.UpdateInBodyResultAsync(inBodyResult);
			return NoContent();
		}

		// DELETE: api/InBodyResult/{id}
		[HttpDelete("{id}")]
		public async Task<IActionResult> DeleteInBodyResult(int id)
		{
			var inBodyResult = await _inBodyResultRepository.GetInBodyResultByIdAsync(id);

			if (inBodyResult == null)
				return NotFound(new { message = "InBody result not found" });

			var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

			// Ensure the user can only delete their own InBody results
			if (inBodyResult.UserId != userId)
				return Forbid();

			await _inBodyResultRepository.DeleteInBodyResultAsync(id);
			return NoContent();
		}
	}
}