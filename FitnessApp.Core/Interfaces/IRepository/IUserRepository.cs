using FitnessApp.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FitnessApp.Core.Interfaces.IRepository
{
	public interface IUserRepository
	{
		Task<ApplicationUser> AddUserAsync(ApplicationUser user);
		Task<ApplicationUser> UpdateUserAsync(ApplicationUser user);
		Task<ApplicationUser> DeleteUserAsync(string id);
		Task<IEnumerable<ApplicationUser>> GetAllUsersAsync();
		Task<ApplicationUser> GetUserByIdAsync(string userId);
		Task<ApplicationUser> GetUserByEmailAsync(string email);
		Task<ApplicationUser> GetUserByFullNameAsync(string fullName);
		Task<bool> UserExistsAsync(string id);
		Task<bool> EmailExistsAsync(string email);

	}
}
