using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FitnessApp.Core.Entities
{
	public class UserGoal
	{
		public int Id { get; set; }
		public string UserId { get; set; }
		public string GoalType { get; set; }  // Weight, BodyFat, Strength, etc.
		public decimal TargetValue { get; set; }
		public decimal StartValue { get; set; }
		public DateTime StartDate { get; set; }
		public DateTime TargetDate { get; set; }
		public bool Achieved { get; set; } = false;
		public DateTime? DateAchieved { get; set; }

		// Navigation property
		public virtual ApplicationUser User { get; set; }
	}
}
