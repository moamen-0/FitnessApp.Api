using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FitnessApp.Core.Entities
{
	public class CompletedWorkout
	{
		public int Id { get; set; }
		public string UserId { get; set; }
		public int? WorkoutPlanId { get; set; }
		public DateTime Date { get; set; }
		public int? Duration { get; set; }  // in minutes
		public int? CaloriesBurned { get; set; }
		public string Intensity { get; set; }  // Low, Medium, High
		public decimal? CompletionPercentage { get; set; }
		public string Notes { get; set; }

		// Navigation properties
		public virtual ApplicationUser User { get; set; }
		public virtual WorkoutPlan WorkoutPlan { get; set; }
		public virtual ICollection<ExercisePerformance> Exercises { get; set; }
	}
}
