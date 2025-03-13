using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FitnessApp.Core.Entities
{
	public class InBodyResult
	{
		public int Id { get; set; }
		public string UserId { get; set; }
		public DateTime Date { get; set; }
		public decimal Weight { get; set; }
		public decimal? BodyFatPercentage { get; set; }
		public decimal? MuscleMass { get; set; }
		public decimal? BMI { get; set; }
		public decimal? VisceralFat { get; set; }
		public int? BMR { get; set; }
		public decimal? TDEE { get; set; }
		public string? ActivityLevel { get; set; } // Sedentary, Light, Moderate, Active, Very Active
		public DateTime? TDEELastCalculated { get; set; }

		// Navigation property
		public virtual ApplicationUser User { get; set; }

	}
}
