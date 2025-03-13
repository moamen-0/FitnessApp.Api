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
		public DateTime Date { get; set; }
		public string MetricType { get; set; }  // Weight, BodyFat, Strength, etc.
		public decimal MetricValue { get; set; }
		public string? Notes { get; set; }

		// Navigation property
		public virtual ApplicationUser User { get; set; }
	}
}
