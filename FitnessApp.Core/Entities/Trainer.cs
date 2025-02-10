using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FitnessApp.Core.Entities
{
	public class Trainer
	{
		public int Id { get; set; }
		public string UserId { get; set; }
		public ApplicationUser User { get; set; } = null!;
		public string Certification { get; set; } = string.Empty;
		public string Specialization { get; set; } = string.Empty;
		public ICollection<WorkoutPlan> WorkoutPlans { get; set; } = new List<WorkoutPlan>();

	}
}
