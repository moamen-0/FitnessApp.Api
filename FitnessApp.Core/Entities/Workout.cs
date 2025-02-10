using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FitnessApp.Core.Entities
{
	public class Workout
	{

		public int Id { get; set; }
		public string Name { get; set; } = string.Empty;
		public string Description { get; set; } = string.Empty;
		public string Category { get; set; } = string.Empty;
		public string? VideoUrl { get; set; }
		public string? ImageUrl { get; set; }
		public ICollection<WorkoutPlanWorkout> WorkoutPlanWorkouts { get; set; } = new List<WorkoutPlanWorkout>();

	}
}
