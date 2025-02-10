using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FitnessApp.Core.Entities
{
	public class InBodyAnalysis
	{
		public int Id { get; set; }
		public string UserId { get; set; }
		public ApplicationUser User { get; set; } = null!;

		public DateTime ScanDate { get; set; }
		public float Weight { get; set; }
		public float MuscleMass { get; set; }
		public float BodyFatPercentage { get; set; }
		public float BMI { get; set; }
		public float BMR { get; set; }

	}
}
