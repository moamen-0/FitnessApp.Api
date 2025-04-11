using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FitnessApp.Core.Dtos
{
	public class DietPlanDto
	{
		public int? Id { get; set; }

		[Required]
		public string Name { get; set; }

		[Required]
		public string Description { get; set; }

		[Required]
		public string DietType { get; set; }

		public int? DailyCalorieTarget { get; set; }

		public int? DailyProteinTarget { get; set; }

		public int? DailyCarbsTarget { get; set; }

		public int? DailyFatTarget { get; set; }

		[Required]
		[Range(1, 365)]
		public int DurationDays { get; set; }
	}
}