using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FitnessApp.Core.Dtos
{
	public class UserProfileResponseDto
	{
		public string Id { get; set; }
		public string Email { get; set; }
		public string FullName { get; set; }
		public string? Gender { get; set; }
		public DateTime? DateOfBirth { get; set; }
		public string? ProfileImage { get; set; }
		public int? Age { get; set; }
		public DateTime? CreatedAt { get; set; }
		public string? PhoneNumber { get; set; }
	}
}
