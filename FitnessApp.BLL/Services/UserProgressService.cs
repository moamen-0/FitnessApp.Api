using FitnessApp.Core.Entities;
using FitnessApp.Core.Interfaces;
using FitnessApp.Core.Interfaces.IRepository;
using FitnessApp.Core.Interfaces.IService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FitnessApp.BLL.Services
{
	public class UserProgressService : IUserProgressService
	{
		private readonly IUnitOfWork _unitOfWork;
		private readonly IInBodyResultRepository _inBodyResultRepository;
		private readonly IUserProgressRepository _userProgressRepository;

		public UserProgressService(
			IUnitOfWork unitOfWork,
			IInBodyResultRepository inBodyResultRepository,
			IUserProgressRepository userProgressRepository)
		{
			_unitOfWork = unitOfWork;
			_inBodyResultRepository = inBodyResultRepository;
			_userProgressRepository = userProgressRepository;
		}

		public async Task<UserProgress> AddProgressEntryAsync(string userId, string metricType, decimal metricValue, string notes = null)
		{
			var entry = new UserProgress
			{
				UserId = userId,
				MetricType = metricType,
				MetricValue = metricValue,
				Date = DateTime.UtcNow,
				Notes = notes
			};

			await _userProgressRepository.AddProgressEntryAsync(entry);

			// After adding progress, check if any goals are affected
			await TrackGoalProgressAsync(userId);

			return entry;
		}

		public async Task<bool> DeleteProgressEntryAsync(int entryId, string userId)
		{
			return await _userProgressRepository.DeleteProgressEntryAsync(entryId, userId);
		}

		public async Task<IDictionary<string, List<UserProgress>>> GetAllUserProgressAsync(string userId)
		{
			var progressEntries = await _userProgressRepository.GetUserProgressHistoryAsync(userId);

			// Group by metric type
			return progressEntries
				.GroupBy(p => p.MetricType)
				.ToDictionary(
					group => group.Key,
					group => group.OrderBy(p => p.Date).ToList()
				);
		}

		public async Task<IEnumerable<UserProgress>> GetUserProgressHistoryAsync(string userId, string metricType = null)
		{
			return await _userProgressRepository.GetUserProgressHistoryAsync(userId, metricType);
		}

		public async Task<IDictionary<string, decimal>> GetProgressSummaryAsync(string userId)
		{
			var summary = new Dictionary<string, decimal>();

			// Get latest InBody results
			var latestInBody = await _inBodyResultRepository.GetLatestInBodyResultAsync(userId);
			if (latestInBody != null)
			{
				summary["Weight"] = latestInBody.Weight;
				if (latestInBody.BodyFatPercentage.HasValue)
					summary["BodyFat"] = latestInBody.BodyFatPercentage.Value;
				if (latestInBody.MuscleMass.HasValue)
					summary["MuscleMass"] = latestInBody.MuscleMass.Value;
			}

			// Get progress comparison for changes
			var progressComparison = await _inBodyResultRepository.GetProgressComparisonAsync(userId);
			foreach (var item in progressComparison)
			{
				summary[item.Key] = item.Value;
			}

			// Add other metric types from user progress
			var recentProgress = await _userProgressRepository.GetLatestProgressByTypeAsync(userId);
			foreach (var entry in recentProgress)
			{
				if (!summary.ContainsKey(entry.Key))
				{
					summary[entry.Key] = entry.Value.MetricValue;
				}
			}

			return summary;
		}

		public async Task<UserProgress> UpdateProgressEntryAsync(int entryId, string userId, decimal newValue, string notes = null)
		{
			return await _userProgressRepository.UpdateProgressEntryAsync(entryId, userId, newValue, notes);
		}

		public async Task<bool> TrackGoalProgressAsync(string userId)
		{
			// Get user's goal
			var user = await _unitOfWork.UserRepository.GetUserByIdAsync(userId);
			if (user?.UserGoal == null) return false;

			var goal = user.UserGoal;

			// If goal is already achieved, nothing to do
			if (goal.Achieved) return true;

			// Get latest progress for the goal type
			decimal currentValue = 0;

			// For weight goal, get latest InBody result
			if (goal.GoalType.ToLower() == "weight")
			{
				var latestInBody = await _inBodyResultRepository.GetLatestInBodyResultAsync(userId);
				if (latestInBody != null)
				{
					currentValue = latestInBody.Weight;
				}
			}
			else
			{
				// For other goals, check user progress
				var latestProgress = await _userProgressRepository.GetLatestProgressEntryAsync(userId, goal.GoalType);
				if (latestProgress != null)
				{
					currentValue = latestProgress.MetricValue;
				}
			}

			// Determine if goal is achieved based on direction
			bool isAchieved = false;

			// For weight loss, target < start
			if (goal.TargetValue < goal.StartValue)
			{
				isAchieved = currentValue <= goal.TargetValue;
			}
			// For weight gain or other increases, target > start
			else
			{
				isAchieved = currentValue >= goal.TargetValue;
			}

			if (isAchieved && !goal.Achieved)
			{
				// Update goal as achieved
				goal.Achieved = true;
				goal.DateAchieved = DateTime.UtcNow;
				await _unitOfWork.SaveChangesAsync();
				return true;
			}

			return false;
		}
	}
}