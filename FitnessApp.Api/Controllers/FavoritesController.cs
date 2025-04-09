using FitnessApp.Core.Entities;
using FitnessApp.Core.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace FitnessApp.Api.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class FavoritesController : ControllerBase
	{
		private readonly IUnitOfWork _unitOfWork;

		public FavoritesController(IUnitOfWork unitOfWork)
		{
			_unitOfWork = unitOfWork;
		}

		// GET: api/Favorites/exercises
		[HttpGet("exercises")]
		public async Task<ActionResult<IEnumerable<Exercise>>> GetFavoriteExercises()
		{
			var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
			var favorites = await _unitOfWork.FavoritesRepository.GetFavoriteExercisesAsync(userId);
			return Ok(favorites);
		}

		// POST: api/Favorites/exercises/{id}
		[HttpPost("exercises/{id}")]
		public async Task<IActionResult> AddExerciseToFavorites(int id)
		{
			var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
			var result = await _unitOfWork.FavoritesRepository.AddExerciseToFavoritesAsync(userId, id);

			if (!result)
				return NotFound("Exercise not found");

			return Ok();
		}

		// DELETE: api/Favorites/exercises/{id}
		[HttpDelete("exercises/{id}")]
		public async Task<IActionResult> RemoveExerciseFromFavorites(int id)
		{
			var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
			var result = await _unitOfWork.FavoritesRepository.RemoveExerciseFromFavoritesAsync(userId, id);

			if (!result)
				return NotFound("Exercise not found in favorites");

			return Ok();
		}

		// GET: api/Favorites/exercises/{id}/is-favorite
		[HttpGet("exercises/{id}/is-favorite")]
		public async Task<ActionResult<bool>> IsExerciseFavorited(int id)
		{
			var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
			var result = await _unitOfWork.FavoritesRepository.IsExerciseFavoritedAsync(userId, id);
			return Ok(result);
		}

		// GET: api/Favorites/meals
		[HttpGet("meals")]
		public async Task<ActionResult<IEnumerable<Meal>>> GetFavoriteMeals()
		{
			var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
			var favorites = await _unitOfWork.FavoritesRepository.GetFavoriteMealsAsync(userId);
			return Ok(favorites);
		}

		// POST: api/Favorites/meals/{id}
		[HttpPost("meals/{id}")]
		public async Task<IActionResult> AddMealToFavorites(int id)
		{
			var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
			var result = await _unitOfWork.FavoritesRepository.AddMealToFavoritesAsync(userId, id);

			if (!result)
				return NotFound("Meal not found");

			return Ok();
		}

		// DELETE: api/Favorites/meals/{id}
		[HttpDelete("meals/{id}")]
		public async Task<IActionResult> RemoveMealFromFavorites(int id)
		{
			var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
			var result = await _unitOfWork.FavoritesRepository.RemoveMealFromFavoritesAsync(userId, id);

			if (!result)
				return NotFound("Meal not found in favorites");

			return Ok();
		}

		// GET: api/Favorites/meals/{id}/is-favorite
		[HttpGet("meals/{id}/is-favorite")]
		public async Task<ActionResult<bool>> IsMealFavorited(int id)
		{
			var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
			var result = await _unitOfWork.FavoritesRepository.IsMealFavoritedAsync(userId, id);
			return Ok(result);
		}
	}
}
