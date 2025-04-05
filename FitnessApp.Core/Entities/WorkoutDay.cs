using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace FitnessApp.Core.Entities
{
	public class WorkoutDay
	{
		public int Id { get; set; }
		public int WorkoutPlanId { get; set; }
		public string Name { get; set; }  // e.g., "Leg Day", "Upper Body"
		public int DayNumber { get; set; } // Day 1, Day 2, etc.

		// Navigation properties
		[JsonIgnore]
		public virtual WorkoutPlan WorkoutPlan { get; set; }
		public virtual ICollection<WorkoutExercise> Exercises { get; set; }
	}
}
