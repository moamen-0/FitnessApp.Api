using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FitnessApp.Core.Entities
{
	public class ExercisePerformance
	{
		public int Id { get; set; }
		public int CompletedWorkoutId { get; set; }
		public int ExerciseId { get; set; }
		public string UserId { get; set; }
		public DateTime Date { get; set; }
		public decimal? Weight { get; set; }
		public int? Reps { get; set; }
		public int? Sets { get; set; }
		public decimal? FormQuality { get; set; }  // AI-calculated form score (0-1)
		public string FormFeedback { get; set; }

		// Navigation properties
		public virtual CompletedWorkout CompletedWorkout { get; set; }
		public virtual Exercise Exercise { get; set; }
		public virtual ApplicationUser User { get; set; }
	}
}
