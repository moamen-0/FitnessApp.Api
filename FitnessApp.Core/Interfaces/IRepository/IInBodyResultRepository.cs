using FitnessApp.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FitnessApp.Core.Interfaces.IRepository
{
	public interface IInBodyResultRepository
	{
		Task<InBodyResult> GetInBodyResultByIdAsync(int id);
		Task<IEnumerable<InBodyResult>> GetInBodyResultsByUserIdAsync(string userId);
		Task<InBodyResult> GetLatestInBodyResultAsync(string userId);
		Task<InBodyResult> CreateInBodyResultAsync(InBodyResult inBodyResult);
		Task<InBodyResult> UpdateInBodyResultAsync(InBodyResult inBodyResult);
		Task DeleteInBodyResultAsync(int id);
		Task<decimal> CalculateBMIAsync(string userId);
		Task<Dictionary<string, List<decimal>>> GetBodyCompositionTrendsAsync(string userId, DateTime startDate, DateTime endDate);
		Task<decimal> CalculateTDEEAsync(string userId);
		Task<InBodyResult> GetBaseline(string userId);
		Task<Dictionary<string, decimal>> GetProgressComparisonAsync(string userId);
	}
}
