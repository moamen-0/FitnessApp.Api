 
using FitnessApp.Core.Entities;
using FitnessApp.Core.Interfaces.IService;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace FitnessApp.Api.Controllers
{
	//[Authorize]
	[Route("api/[controller]")]
	[ApiController]
	[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
	public class ProgressController : ControllerBase
	{
		private readonly IUserProgressService _progressService;

		public ProgressController(IUserProgressService progressService)
		{
			_progressService = progressService;
		}

		// GET: api/Progress
		[HttpGet]
		public async Task<IActionResult> GetAllProgress()
		{
			var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
			var progress = await _progressService.GetAllUserProgressAsync(userId);
			return Ok(progress);
		}

		// GET: api/Progress/summary
		[HttpGet("summary")]
		public async Task<IActionResult> GetProgressSummary()
		{
			var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
			var summary = await _progressService.GetProgressSummaryAsync(userId);
			return Ok(summary);
		}

		// GET: api/Progress/{metricType}
		[HttpGet("{metricType}")]
		public async Task<IActionResult> GetProgressByType(string metricType)
		{
			var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
			var history = await _progressService.GetUserProgressHistoryAsync(userId, metricType);
			return Ok(history);
		}

		// POST: api/Progress
		[HttpPost]
		public async Task<IActionResult> AddProgressEntry([FromBody] AddProgressEntryModel model)
		{
			if (!ModelState.IsValid)
				return BadRequest(ModelState);

			var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
			var entry = await _progressService.AddProgressEntryAsync(
				userId,
				model.MetricType,
				model.MetricValue,
				model.Notes);

			return CreatedAtAction(
				nameof(GetProgressByType),
				new { metricType = model.MetricType },
				entry);
		}

		// PUT: api/Progress/{id}
		[HttpPut("{id}")]
		public async Task<IActionResult> UpdateProgressEntry(int id, [FromBody] UpdateProgressEntryModel model)
		{
			if (!ModelState.IsValid)
				return BadRequest(ModelState);

			var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
			var updatedEntry = await _progressService.UpdateProgressEntryAsync(
				id,
				userId,
				model.MetricValue,
				model.Notes);

			if (updatedEntry == null)
				return NotFound();

			return Ok(updatedEntry);
		}

		// DELETE: api/Progress/{id}
		[HttpDelete("{id}")]
		public async Task<IActionResult> DeleteProgressEntry(int id)
		{
			var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
			var result = await _progressService.DeleteProgressEntryAsync(id, userId);

			if (!result)
				return NotFound();

			return NoContent();
		}

		// POST: api/Progress/track-goals
		[HttpPost("track-goals")]
		public async Task<IActionResult> TrackGoalProgress()
		{
			var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
			var goalReached = await _progressService.TrackGoalProgressAsync(userId);

			return Ok(new { goalReached });
		}
	}

	public class AddProgressEntryModel
	{
		public string MetricType { get; set; }
		public decimal MetricValue { get; set; }
		public string Notes { get; set; }
	}

	public class UpdateProgressEntryModel
	{
		public decimal MetricValue { get; set; }
		public string Notes { get; set; }
	}
}