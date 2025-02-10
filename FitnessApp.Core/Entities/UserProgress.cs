using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FitnessApp.Core.Entities
{
	public class UserProgress
	{
		public int Id { get; set; }
		public string UserId { get; set; }
		public ApplicationUser User { get; set; } = null!;
		public int WorkoutId { get; set; }
		public Workout Workout { get; set; } = null!;
		public DateTime Date { get; set; }
		public int RepsCompleted { get; set; }
		public int WeightLifted { get; set; } 
	}
}
