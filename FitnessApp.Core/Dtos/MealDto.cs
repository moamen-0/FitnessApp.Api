using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FitnessApp.Core.Dtos
{
	public class MealDto
	{
		public int? Id { get; set; }

		[Required]
		public int DietPlanId { get; set; }

		[Required]
		public string MealType { get; set; }

		[Required]
		public string Name { get; set; }

		public string Description { get; set; }

		[Required]
		public string FoodItems { get; set; }

		[Required]
		[Range(0, int.MaxValue)]
		public int Calories { get; set; }

		[Required]
		[Range(0, double.MaxValue)]
		public decimal Protein { get; set; }

		[Required]
		[Range(0, double.MaxValue)]
		public decimal Carbs { get; set; }

		[Required]
		[Range(0, double.MaxValue)]
		public decimal Fat { get; set; }

		public string? ImageUrl { get; set; }
	}
}