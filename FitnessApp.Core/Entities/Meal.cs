namespace FitnessApp.Core.Entities
{
	public class Meal
	{
		public int Id { get; set; }
		public string Name { get; set; } = string.Empty;
		public string Description { get; set; } = string.Empty;
		public int Calories { get; set; }
		public int Protein { get; set; }
		public int Carbs { get; set; }
		public int Fats { get; set; }

		public ICollection<DietPlanMeal> DietPlanMeals { get; set; } = new List<DietPlanMeal>();

	}
}