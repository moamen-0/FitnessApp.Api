using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FitnessApp.Core.Dtos
{
	public class WorkoutPlanDto
	{
		public int Id { get; set; }
		public string Name { get; set; }
		public string Description { get; set; }
		public string DifficultyLevel { get; set; }
		public string FocusArea { get; set; }
		public int DurationWeeks { get; set; }
		public DateTime CreatedAt { get; set; }
		public bool IsTemplate { get; set; }
		// Navigation properties without back-references
		public List<WorkoutDayDto> WorkoutDays { get; set; }
	}
}
