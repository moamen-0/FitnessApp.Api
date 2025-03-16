using FitnessApp.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FitnessApp.Core.Interfaces.IRepository
{
	public interface IUserProgressRepository
	{
		Task<UserProgress> AddProgressEntryAsync(UserProgress progress);
		Task<IEnumerable<UserProgress>> GetUserProgressHistoryAsync(string userId, string metricType = null);
		Task<UserProgress> GetProgressEntryByIdAsync(int entryId);
		Task<UserProgress> GetLatestProgressEntryAsync(string userId, string metricType);
		Task<Dictionary<string, UserProgress>> GetLatestProgressByTypeAsync(string userId);
		Task<UserProgress> UpdateProgressEntryAsync(int entryId, string userId, decimal newValue, string notes = null);
		Task<bool> DeleteProgressEntryAsync(int entryId, string userId);
	}
}
