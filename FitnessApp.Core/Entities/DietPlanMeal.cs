namespace FitnessApp.Core.Entities
{
	public class DietPlanMeal
	{
		public int DietPlanId { get; set; }
		public DietPlan DietPlan { get; set; } = null!;

		public int MealId { get; set; }
		public Meal Meal { get; set; } = null!;

		public string MealTime { get; set; } = string.Empty; 

	}
}