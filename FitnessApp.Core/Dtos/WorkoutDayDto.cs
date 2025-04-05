using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FitnessApp.Core.Dtos
{
	public class WorkoutDayDto
	{
		public int Id { get; set; }
		public string Name { get; set; }
		public int DayNumber { get; set; }
		// No reference back to WorkoutPlan
		public List<WorkoutExerciseDto> Exercises { get; set; }
	}
}
