using FitnessApp.Core.Entities;
using FitnessApp.Core.Interfaces.IRepository;
using FitnessApp.DAL.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FitnessApp.DAL.Repositories
{
	public class FavoritesRepository : IFavoritesRepository
	{
		private readonly AppDbContext _context;

		public FavoritesRepository(AppDbContext context)
		{
			_context = context;
		}

		// Exercise favorites
		public async Task<IEnumerable<Exercise>> GetFavoriteExercisesAsync(string userId)
		{
			var user = await _context.Users
				.Include(u => u.FavoriteExercises)
				.FirstOrDefaultAsync(u => u.Id == userId);

			return user?.FavoriteExercises ?? new List<Exercise>();
		}

		public async Task<bool> AddExerciseToFavoritesAsync(string userId, int exerciseId)
		{
			var user = await _context.Users
				.Include(u => u.FavoriteExercises)
				.FirstOrDefaultAsync(u => u.Id == userId);

			if (user == null)
				return false;

			var exercise = await _context.Exercises.FindAsync(exerciseId);
			if (exercise == null)
				return false;

			if (user.FavoriteExercises.Any(e => e.Id == exerciseId))
				return true; // Already favorited

			user.FavoriteExercises.Add(exercise);
			await _context.SaveChangesAsync();

			return true;
		}

		public async Task<bool> RemoveExerciseFromFavoritesAsync(string userId, int exerciseId)
		{
			var user = await _context.Users
				.Include(u => u.FavoriteExercises)
				.FirstOrDefaultAsync(u => u.Id == userId);

			if (user == null)
				return false;

			var exercise = user.FavoriteExercises.FirstOrDefault(e => e.Id == exerciseId);
			if (exercise == null)
				return false; // Not in favorites

			user.FavoriteExercises.Remove(exercise);
			await _context.SaveChangesAsync();

			return true;
		}

		public async Task<bool> IsExerciseFavoritedAsync(string userId, int exerciseId)
		{
			var user = await _context.Users
				.Include(u => u.FavoriteExercises)
				.FirstOrDefaultAsync(u => u.Id == userId);

			return user?.FavoriteExercises.Any(e => e.Id == exerciseId) ?? false;
		}

		// Meal favorites
		public async Task<IEnumerable<Meal>> GetFavoriteMealsAsync(string userId)
		{
			var user = await _context.Users
				.Include(u => u.FavoriteMeals)
				.FirstOrDefaultAsync(u => u.Id == userId);

			return user?.FavoriteMeals ?? new List<Meal>();
		}

		public async Task<bool> AddMealToFavoritesAsync(string userId, int mealId)
		{
			var user = await _context.Users
				.Include(u => u.FavoriteMeals)
				.FirstOrDefaultAsync(u => u.Id == userId);

			if (user == null)
				return false;

			var meal = await _context.Meals.FindAsync(mealId);
			if (meal == null)
				return false;

			if (user.FavoriteMeals.Any(m => m.Id == mealId))
				return true; // Already favorited

			user.FavoriteMeals.Add(meal);
			await _context.SaveChangesAsync();

			return true;
		}

		public async Task<bool> RemoveMealFromFavoritesAsync(string userId, int mealId)
		{
			var user = await _context.Users
				.Include(u => u.FavoriteMeals)
				.FirstOrDefaultAsync(u => u.Id == userId);

			if (user == null)
				return false;

			var meal = user.FavoriteMeals.FirstOrDefault(m => m.Id == mealId);
			if (meal == null)
				return false; // Not in favorites

			user.FavoriteMeals.Remove(meal);
			await _context.SaveChangesAsync();

			return true;
		}

		public async Task<bool> IsMealFavoritedAsync(string userId, int mealId)
		{
			var user = await _context.Users
				.Include(u => u.FavoriteMeals)
				.FirstOrDefaultAsync(u => u.Id == userId);

			return user?.FavoriteMeals.Any(m => m.Id == mealId) ?? false;
		}
	}
}
