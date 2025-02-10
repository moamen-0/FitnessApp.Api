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
		public string Name { get; set; } = string.Empty;
		public string UserId { get; set; }
		public ApplicationUser User { get; set; } = null!;
		public ICollection<DietPlanMeal> DietPlanMeals { get; set; } = new List<DietPlanMeal>();
	}
}
