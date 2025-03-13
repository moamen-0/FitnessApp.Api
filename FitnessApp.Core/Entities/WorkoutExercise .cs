using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FitnessApp.Core.Entities
{
	public class WorkoutExercise
	{
		public int Id { get; set; }
		public int WorkoutDayId { get; set; }
		public int ExerciseId { get; set; }
		public int Sets { get; set; }
		public int? RepsPerSet { get; set; }
		public int? Duration { get; set; }  // For timed exercises
		public int OrderInWorkout { get; set; }
		public int? RestBetweenSets { get; set; }  // in seconds
		public string Notes { get; set; }

		// Navigation properties
		public virtual WorkoutDay WorkoutDay { get; set; }
		public virtual Exercise Exercise { get; set; }
	}
}
