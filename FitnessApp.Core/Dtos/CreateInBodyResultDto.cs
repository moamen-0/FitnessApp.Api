using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FitnessApp.Core.Dtos
{
	public class CreateInBodyResultDto
	{
		[Required]
		[Range(20, 500)]
		public decimal Weight { get; set; }

		[Range(1, 60)]
		public decimal? BodyFatPercentage { get; set; }

		[Range(10, 100)]
		public decimal? MuscleMass { get; set; }

		[Range(10, 50)]
		public decimal? BMI { get; set; }

		[Range(1, 30)]
		public decimal? VisceralFat { get; set; }

		[Range(500, 5000)]
		public int? BMR { get; set; }

		[Required]
		public string ActivityLevel { get; set; }

		public DateTime? Date { get; set; }
	}
}
