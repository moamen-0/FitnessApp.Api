using FitnessApp.Core.Entities;
using FitnessApp.Core.Interfaces.IRepository;
using FitnessApp.DAL.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FitnessApp.DAL.Repositories
{
	public class ExerciseRepository : IExerciseRepository
	{
		private readonly AppDbContext _context;

		public ExerciseRepository(AppDbContext context)
		{
			_context = context;
		}

		public async Task<Exercise> GetExerciseByIdAsync(int id)
		{
			return await _context.Exercises.FindAsync(id);
		}

		public async Task<IEnumerable<Exercise>> GetAllExercisesAsync()
		{
			return await _context.Exercises.ToListAsync();
		}

		public async Task<IEnumerable<Exercise>> GetExercisesByMuscleGroupAsync(string muscleGroup)
		{
			return await _context.Exercises
				.Where(e => e.MuscleGroup.Contains(muscleGroup))
				.ToListAsync();
		}

		public async Task<IEnumerable<Exercise>> GetExercisesByEquipmentAsync(string equipment)
		{
			return await _context.Exercises
				.Where(e => e.Equipment.Contains(equipment))
				.ToListAsync();
		}

		public async Task<IEnumerable<Exercise>> GetExercisesByCategoryAsync(string category)
		{
			return await _context.Exercises
				.Where(e => e.Category == category)
				.ToListAsync();
		}

		public async Task<IEnumerable<Exercise>> GetExercisesByDifficultyLevelAsync(string difficultyLevel)
		{
			return await _context.Exercises
				.Where(e => e.DifficultyLevel == difficultyLevel)
				.ToListAsync();
		}

		public async Task<IEnumerable<Exercise>> GetExercisesByTypeAsync(string type)
		{
			// Type could be "Compound", "Isolation", etc.
			// This might be stored in the Description or could be added as a new field
			return await _context.Exercises
				.Where(e => e.Description.Contains(type) || e.Category.Contains(type))
				.ToListAsync();
		}

		public async Task<Exercise> CreateExerciseAsync(Exercise exercise)
		{
			await _context.Exercises.AddAsync(exercise);
			await _context.SaveChangesAsync();
			return exercise;
		}

		public async Task<Exercise> UpdateExerciseAsync(Exercise exercise)
		{
			_context.Exercises.Update(exercise);
			await _context.SaveChangesAsync();
			return exercise;
		}

		public async Task DeleteExerciseAsync(int id)
		{
			var exercise = await _context.Exercises.FindAsync(id);
			if (exercise != null)
			{
				_context.Exercises.Remove(exercise);
				await _context.SaveChangesAsync();
			}
		}

		public async Task<bool> ExerciseExistsAsync(int id)
		{
			return await _context.Exercises.AnyAsync(e => e.Id == id);
		}

		public async Task<IEnumerable<Exercise>> SearchExercisesAsync(string searchTerm)
		{
			if (string.IsNullOrEmpty(searchTerm))
				return await GetAllExercisesAsync();

			return await _context.Exercises
				.Where(e => e.Name.Contains(searchTerm) ||
						   e.Description.Contains(searchTerm) ||
						   e.MuscleGroup.Contains(searchTerm) ||
						   e.Category.Contains(searchTerm))
				.ToListAsync();
		}

		public async Task<IEnumerable<Exercise>> GetExercisesWithAIModelAsync()
		{
			return await _context.Exercises
				.Where(e => !string.IsNullOrEmpty(e.AIModelReference))
				.ToListAsync();
		}

		public async Task<bool> UpdateAIModelReferenceAsync(int exerciseId, string aiModelReference)
		{
			var exercise = await _context.Exercises.FindAsync(exerciseId);
			if (exercise == null)
				return false;

			exercise.AIModelReference = aiModelReference;
			await _context.SaveChangesAsync();
			return true;
		}
	}
}