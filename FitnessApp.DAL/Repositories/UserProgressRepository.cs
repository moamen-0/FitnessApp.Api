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
	public class UserProgressRepository : IUserProgressRepository
	{
		private readonly AppDbContext _context;

		public UserProgressRepository(AppDbContext context)
		{
			_context = context;
		}

		public async Task<UserProgress> AddProgressEntryAsync(UserProgress progress)
		{
			await _context.UserProgress.AddAsync(progress);
			await _context.SaveChangesAsync();
			return progress;
		}

		public async Task<bool> DeleteProgressEntryAsync(int entryId, string userId)
		{
			var entry = await _context.UserProgress
				.FirstOrDefaultAsync(p => p.Id == entryId && p.UserId == userId);

			if (entry == null) return false;

			_context.UserProgress.Remove(entry);
			await _context.SaveChangesAsync();
			return true;
		}

		public async Task<UserProgress> GetLatestProgressEntryAsync(string userId, string metricType)
		{
			return await _context.UserProgress
				.Where(p => p.UserId == userId && p.MetricType == metricType)
				.OrderByDescending(p => p.Date)
				.FirstOrDefaultAsync();
		}

		public async Task<Dictionary<string, UserProgress>> GetLatestProgressByTypeAsync(string userId)
		{
			var result = new Dictionary<string, UserProgress>();

			// Get all metric types for this user
			var metricTypes = await _context.UserProgress
				.Where(p => p.UserId == userId)
				.Select(p => p.MetricType)
				.Distinct()
				.ToListAsync();

			foreach (var metricType in metricTypes)
			{
				var latestEntry = await GetLatestProgressEntryAsync(userId, metricType);
				if (latestEntry != null)
				{
					result[metricType] = latestEntry;
				}
			}

			return result;
		}

		public async Task<UserProgress> GetProgressEntryByIdAsync(int entryId)
		{
			return await _context.UserProgress.FindAsync(entryId);
		}

		public async Task<IEnumerable<UserProgress>> GetUserProgressHistoryAsync(string userId, string metricType = null)
		{
			var query = _context.UserProgress.Where(p => p.UserId == userId);

			if (!string.IsNullOrEmpty(metricType))
			{
				query = query.Where(p => p.MetricType == metricType);
			}

			return await query.OrderByDescending(p => p.Date).ToListAsync();
		}

		public async Task<UserProgress> UpdateProgressEntryAsync(int entryId, string userId, decimal newValue, string notes = null)
		{
			var entry = await _context.UserProgress
				.FirstOrDefaultAsync(p => p.Id == entryId && p.UserId == userId);

			if (entry == null) return null;

			entry.MetricValue = newValue;

			if (notes != null)
			{
				entry.Notes = notes;
			}

			await _context.SaveChangesAsync();
			return entry;
		}
	}
}