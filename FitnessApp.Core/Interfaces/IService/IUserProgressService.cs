using FitnessApp.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FitnessApp.Core.Interfaces.IService
{
	public interface IUserProgressService
	{
		Task<UserProgress> AddProgressEntryAsync(string userId, string metricType, decimal metricValue, string notes = null);
		Task<IEnumerable<UserProgress>> GetUserProgressHistoryAsync(string userId, string metricType = null);
		Task<IDictionary<string, List<UserProgress>>> GetAllUserProgressAsync(string userId);
		Task<IDictionary<string, decimal>> GetProgressSummaryAsync(string userId);
		Task<bool> DeleteProgressEntryAsync(int entryId, string userId);
		Task<UserProgress> UpdateProgressEntryAsync(int entryId, string userId, decimal newValue, string notes = null);
		Task<bool> TrackGoalProgressAsync(string userId);
	}
}
