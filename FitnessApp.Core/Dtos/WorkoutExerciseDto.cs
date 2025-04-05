using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FitnessApp.Core.Dtos
{
	public class WorkoutExerciseDto
	{
		public int Id { get; set; }
		public int ExerciseId { get; set; }

		// Exercise details
		public string ExerciseName { get; set; }
		public string ExerciseDescription { get; set; }
		public string MuscleGroup { get; set; }
		public string Equipment { get; set; }
		public string DifficultyLevel { get; set; }
		public string ExerciseType { get; set; }
		public string InstructionVideo { get; set; }

		// Exercise parameters
		public int Sets { get; set; }
		public int? Reps { get; set; }
		public int? Duration { get; set; } // In seconds
		public double? Weight { get; set; }
		public string Notes { get; set; }
		public int Order { get; set; }
		public string RestBetweenSets { get; set; }
	}
}
