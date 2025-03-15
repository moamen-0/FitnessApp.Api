using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FitnessApp.Core.Dtos
{
	public class UserProfileUpdateDto
	{
		public string FullName { get; set; }
		public string Gender { get; set; }
		public DateTime DateOfBirth { get; set; }
		public string? ProfileImage { get; set; }
	}
}
