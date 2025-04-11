using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FitnessApp.Core.Entities
{
	public class DietPlan
	{
		public int Id { get; set; }
		public string Name { get; set; }
		public string Description { get; set; }
		

		// Basic diet categorization
		public string DietType { get; set; } // Balanced, Keto, Vegetarian, etc.

		// Basic nutritional targets
		public int? DailyCalorieTarget { get; set; }
		public int? DailyProteinTarget { get; set; } // in grams
		public int? DailyCarbsTarget { get; set; } // in grams
		public int? DailyFatTarget { get; set; } // in grams
		public string? ImageUrl { get; set; } // URL to diet plan image

		// Duration
		public int DurationDays { get; set; }
		public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

		public virtual ICollection<Meal> Meals { get; set; }
	}
}
