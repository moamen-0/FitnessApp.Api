using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FitnessApp.Core.Entities
{
	public class WorkoutPlanWorkout
	{
		public int WorkoutPlanId { get; set; }
		public WorkoutPlan WorkoutPlan { get; set; } = null!;

		public int WorkoutId { get; set; }
		public Workout Workout { get; set; } = null!;

		public int Sets { get; set; }
		public int Reps { get; set; }
		public int RestTime { get; set; }

	}
}
