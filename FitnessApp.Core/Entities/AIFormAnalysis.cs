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
		public int ExerciseId { get; set; }
		public Exercise Exercise { get; set; } = null!;
		public DateTime AnalysisDate { get; set; }
		public string Feedback { get; set; } = string.Empty;
		public float Score { get; set; }
	}
}
