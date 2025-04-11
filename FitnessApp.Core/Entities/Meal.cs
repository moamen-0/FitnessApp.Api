namespace FitnessApp.Core.Entities
{
	public class Meal
	{
		public int Id { get; set; }
		public int DietPlanId { get; set; }
		public string MealType { get; set; } // Breakfast, Lunch, Dinner, Snack
		public string Name { get; set; }
		public string Description { get; set; }

		// Meal content - directly integrated
		public string FoodItems { get; set; } // JSON string of food items and quantities

		// Pre-calculated nutrition totals
		public int Calories { get; set; }
		public decimal Protein { get; set; } // in grams
		public decimal Carbs { get; set; } // in grams
		public decimal Fat { get; set; } // in grams
		public string? ImageUrl { get; set; } // URL to meal image

		// Navigation property
		public virtual DietPlan DietPlan { get; set; }

	}
}