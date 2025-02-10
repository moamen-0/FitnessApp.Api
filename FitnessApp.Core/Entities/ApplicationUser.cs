

using Microsoft.AspNetCore.Identity;

namespace FitnessApp.Core.Entities
{
	public class ApplicationUser : IdentityUser
	{
		public string DisplayName { get; set; }
	}
}