using FitnessApp.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FitnessApp.Core.Interfaces.IService
{
	public interface IUserService
	{
		Task<ApplicationUser?> GetUserByIdAsync(string id);
		Task<bool> UpdateUserProfileAsync(ApplicationUser user);
		Task<bool> ChangePasswordAsync(string userId, string currentPassword, string newPassword);
		Task<bool> InitiatePasswordResetAsync(string email);
		Task<bool> ResetPasswordAsync(string email, string token, string newPassword);

	}
}
