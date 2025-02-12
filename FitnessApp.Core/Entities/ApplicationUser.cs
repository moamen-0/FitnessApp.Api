

using Microsoft.AspNetCore.Identity;

namespace FitnessApp.Core.Entities
{
	public class ApplicationUser : IdentityUser
	{
		public string FullName { get; set; } = string.Empty;
		public string Gender { get; set; } = string.Empty;
		public DateTime DateOfBirth { get; set; }
		public string? ProfileImage { get; set; }

		 
		public ICollection<WorkoutPlan> WorkoutPlans { get; set; } = new List<WorkoutPlan>();
		public ICollection<DietPlan> DietPlans { get; set; } = new List<DietPlan>();
		public ICollection<InBodyAnalysis> InBodyAnalyses { get; set; } = new List<InBodyAnalysis>();
	}
}