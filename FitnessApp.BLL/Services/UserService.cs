using FitnessApp.Core.Entities;
using FitnessApp.Core.Interfaces;
using FitnessApp.Core.Interfaces.IService;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FitnessApp.BLL.Services
{
	public class UserService : IUserService
	{
		private readonly IUnitOfWork _unitOfWork;
		private readonly UserManager<ApplicationUser> _userManager;
		public UserService(IUnitOfWork unitOfWork, UserManager<ApplicationUser> userManager)
		{
			_unitOfWork = unitOfWork;
			_userManager = userManager;
		}
		public async Task<bool> ChangePasswordAsync(string userId, string currentPassword, string newPassword)
		{
			var user = await _userManager.FindByIdAsync(userId);
			if (user == null) return false;

			var result = await _userManager.ChangePasswordAsync(user, currentPassword, newPassword);
			return result.Succeeded;

		}

		public async Task<ApplicationUser?> GetUserByIdAsync(string id)
		{
			return await _unitOfWork.UserRepository.GetUserByIdAsync(id);

		}

		public Task<bool> InitiatePasswordResetAsync(string email)
		{
			throw new NotImplementedException();
		}

		public async Task<bool> ResetPasswordAsync(string email, string token, string newPassword)
		{
			var user = await _userManager.FindByEmailAsync(email);
			if (user == null) return false;

			// Verify token hasn't expired
			if (user.ResetPasswordTokenExpiry < DateTime.UtcNow)
				return false;

			// Reset the password
			var result = await _userManager.ResetPasswordAsync(user, token, newPassword);

			if (result.Succeeded)
			{
				// Clear the token after successful reset
				user.ResetPasswordToken = null;
				user.ResetPasswordTokenExpiry = null;
				await _userManager.UpdateAsync(user);
			}

			return result.Succeeded;
		}

		public async Task<bool> UpdateUserProfileAsync(ApplicationUser user)
		{
			var existingUser = await _userManager.FindByIdAsync(user.Id);
			if (existingUser == null) return false;

			existingUser.FullName = user.FullName;
			existingUser.Email = user.Email;
			existingUser.UserName = user.Email;

			var result = await _userManager.UpdateAsync(existingUser);
			return result.Succeeded;
		}
	}
}
