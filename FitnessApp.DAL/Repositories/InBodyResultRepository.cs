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
	public class InBodyResultRepository : IInBodyResultRepository
	{
		private readonly AppDbContext _context;

		public InBodyResultRepository(AppDbContext context)
		{
			_context = context;
		}

		public async Task<InBodyResult> GetInBodyResultByIdAsync(int id)
		{
			return await _context.InBodyResult.FindAsync(id);
		}

		public async Task<IEnumerable<InBodyResult>> GetInBodyResultsByUserIdAsync(string userId)
		{
			return await _context.InBodyResult
				.Where(r => r.UserId == userId)
				.OrderByDescending(r => r.Date)
				.ToListAsync();
		}

		public async Task<InBodyResult> GetLatestInBodyResultAsync(string userId)
		{
			return await _context.InBodyResult
				.Where(r => r.UserId == userId)
				.OrderByDescending(r => r.Date)
				.FirstOrDefaultAsync();
		}

		public async Task<InBodyResult> CreateInBodyResultAsync(InBodyResult inBodyResult)
		{
			// Set the date to current if not provided
			if (inBodyResult.Date == default)
			{
				inBodyResult.Date = DateTime.UtcNow;
			}

			await _context.InBodyResult.AddAsync(inBodyResult);
			await _context.SaveChangesAsync();
			return inBodyResult;
		}

		public async Task<InBodyResult> UpdateInBodyResultAsync(InBodyResult inBodyResult)
		{
			_context.InBodyResult.Update(inBodyResult);
			await _context.SaveChangesAsync();
			return inBodyResult;
		}

		public async Task DeleteInBodyResultAsync(int id)
		{
			var inBodyResult = await _context.InBodyResult.FindAsync(id);
			if (inBodyResult != null)
			{
				_context.InBodyResult.Remove(inBodyResult);
				await _context.SaveChangesAsync();
			}
		}

		public async Task<decimal> CalculateBMIAsync(string userId)
		{
			var latestResult = await GetLatestInBodyResultAsync(userId);

			if (latestResult?.BMI.HasValue == true)
			{
				return latestResult.BMI.Value;
			}

			// If BMI doesn't exist in the latest result, we could calculate it
			// However, we would need the user's height, which isn't stored in InBodyResult
			// This might require fetching from user profile or another source
			return 0;
		}

		public async Task<Dictionary<string, List<decimal>>> GetBodyCompositionTrendsAsync(string userId, DateTime startDate, DateTime endDate)
		{
			var results = await _context.InBodyResult
				.Where(r => r.UserId == userId && r.Date >= startDate && r.Date <= endDate)
				.OrderBy(r => r.Date)
				.ToListAsync();

			var trends = new Dictionary<string, List<decimal>>();

			// Initialize trend lists
			trends["Weight"] = new List<decimal>();
			trends["BodyFat"] = new List<decimal>();
			trends["MuscleMass"] = new List<decimal>();

			foreach (var result in results)
			{
				trends["Weight"].Add(result.Weight);

				if (result.BodyFatPercentage.HasValue)
					trends["BodyFat"].Add(result.BodyFatPercentage.Value);

				if (result.MuscleMass.HasValue)
					trends["MuscleMass"].Add(result.MuscleMass.Value);
			}

			return trends;
		}

		public async Task<decimal> CalculateTDEEAsync(string userId)
		{
			var latestResult = await GetLatestInBodyResultAsync(userId);

			if (latestResult == null)
				return 0;

			// If TDEE is already calculated and recent, return it
			if (latestResult.TDEE.HasValue &&
				latestResult.TDEELastCalculated.HasValue &&
				(DateTime.UtcNow - latestResult.TDEELastCalculated.Value).TotalDays < 7)
			{
				return latestResult.TDEE.Value;
			}

			// Calculate TDEE based on BMR and activity level
			if (latestResult.BMR.HasValue)
			{
				decimal activityMultiplier = latestResult.ActivityLevel?.ToLower() switch
				{
					"sedentary" => 1.2m,
					"light" => 1.375m,
					"moderate" => 1.55m,
					"active" => 1.725m,
					"very active" => 1.9m,
					_ => 1.4m // Default to light-moderate activity
				};

				decimal tdee = latestResult.BMR.Value * activityMultiplier;

				// Update the TDEE in the database
				latestResult.TDEE = tdee;
				latestResult.TDEELastCalculated = DateTime.UtcNow;
				await _context.SaveChangesAsync();

				return tdee;
			}

			return 0;
		}

		public async Task<InBodyResult> GetBaseline(string userId)
		{
			// Get the first recorded InBody result for the user
			return await _context.InBodyResult
				.Where(r => r.UserId == userId)
				.OrderBy(r => r.Date)
				.FirstOrDefaultAsync();
		}

		public async Task<Dictionary<string, decimal>> GetProgressComparisonAsync(string userId)
		{
			var baseline = await GetBaseline(userId);
			var latest = await GetLatestInBodyResultAsync(userId);

			var progressComparison = new Dictionary<string, decimal>();

			if (baseline != null && latest != null)
			{
				// Calculate weight change
				progressComparison["WeightChange"] = latest.Weight - baseline.Weight;

				// Calculate body fat percentage change if available
				if (baseline.BodyFatPercentage.HasValue && latest.BodyFatPercentage.HasValue)
				{
					progressComparison["BodyFatChange"] = latest.BodyFatPercentage.Value - baseline.BodyFatPercentage.Value;
				}

				// Calculate muscle mass change if available
				if (baseline.MuscleMass.HasValue && latest.MuscleMass.HasValue)
				{
					progressComparison["MuscleMassChange"] = latest.MuscleMass.Value - baseline.MuscleMass.Value;
				}
			}

			return progressComparison;
		}
	}
}
