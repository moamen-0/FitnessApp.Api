using FitnessApp.Core.Entities;
using FitnessApp.Core.Interfaces;
using FitnessApp.Core.Interfaces.IService;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
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
		private readonly IEmailService _emailService;
		private readonly IConfiguration _configuration;

		public UserService(IUnitOfWork unitOfWork, UserManager<ApplicationUser> userManager, IEmailService emailService, IConfiguration configuration)
		{
			_unitOfWork = unitOfWork;
			_userManager = userManager;
			_emailService = emailService;
			_configuration = configuration;
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

		public async Task<bool> InitiatePasswordResetAsync(string email)
		{
			var user = await _userManager.FindByEmailAsync(email);
			if (user == null) return false;

			// Generate a token
			var token = await _userManager.GeneratePasswordResetTokenAsync(user);

			// Store the token and expiry in user record
			user.ResetPasswordToken = token;
			user.ResetPasswordTokenExpiry = DateTime.UtcNow.AddHours(24);
			await _userManager.UpdateAsync(user);

			// Create reset link (use absolute URL with the token)
			var resetLink = $"{_configuration["ApplicationUrl"]}/Auth/ResetPassword?email={Uri.EscapeDataString(email)}&token={Uri.EscapeDataString(token)}";

			// Send email
			await _emailService.SendPasswordResetEmailAsync(email, resetLink);

			return true;
		}
		public async Task<bool> ResetPasswordAsync(string email, string token, string newPassword)
		{
			var user = await _userManager.FindByEmailAsync(email);
			if (user == null) return false;

			// Decode the token
			token = Uri.UnescapeDataString(token);

			
			if (user.ResetPasswordTokenExpiry < DateTime.UtcNow)
				return false;

			var result = await _userManager.ResetPasswordAsync(user, token, newPassword);

			if (result.Succeeded)
			{
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
