using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FitnessApp.Core.Entities
{
	public class Exercise
	{
		public int Id { get; set; }
		public string Name { get; set; }
		public string Description { get; set; }
		public string Category { get; set; }  // Strength, Cardio, Flexibility
		public string MuscleGroup { get; set; }  // Primary muscle targeted
		public string Equipment { get; set; }
		public string DifficultyLevel { get; set; }
		public string InstructionSteps { get; set; }
		public string VideoUrl { get; set; }
		public string ImageUrl { get; set; }
		public string AIModelReference { get; set; }  // For AI form detection

		// Navigation properties
		public virtual ICollection<WorkoutExercise> WorkoutExercises { get; set; }
	}
}
