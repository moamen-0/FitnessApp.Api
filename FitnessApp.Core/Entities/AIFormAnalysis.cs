using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FitnessApp.Core.Entities
{
	public class AIFormAnalysis
	{
		public int Id { get; set; }
		public string UserId { get; set; }
		public ApplicationUser User { get; set; } = null!;
		public int WorkoutId { get; set; }
		public Workout Workout { get; set; } = null!;
		public DateTime AnalysisDate { get; set; }
		public string Feedback { get; set; } = string.Empty;
		public float Score { get; set; }
	}
}
