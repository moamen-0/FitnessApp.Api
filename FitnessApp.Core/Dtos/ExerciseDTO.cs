using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FitnessApp.Core.Dtos
{
	public class ExerciseDTO
	{
		// Optional for create (assigned by server), required for update
		public int? Id { get; set; }

		[Required]
		public string Name { get; set; }

		public string Description { get; set; }

		public string Category { get; set; }

		public string MuscleGroup { get; set; }

		public string Equipment { get; set; }

		public string DifficultyLevel { get; set; }

		public string InstructionSteps { get; set; }

		public string VideoUrl { get; set; }

		public string ImageUrl { get; set; }

		public string AiModelReference { get; set; } = string.Empty;
	}
}
