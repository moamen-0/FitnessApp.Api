using FitnessApp.Core.Dtos;
using FitnessApp.Core.Entities;
using FitnessApp.Core.Interfaces.IService;
using FitnessApp.Core.Interfaces;
using FitnessApp.DAL.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace FitnessApp.Api.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class UserController : ControllerBase
	{
		private readonly IUserService _userService;
		private readonly UserManager<ApplicationUser> _userManager;
		private readonly IUnitOfWork _unitOfWork;

		public UserController(
			IUserService userService,
			UserManager<ApplicationUser> userManager,
			IUnitOfWork unitOfWork)
		{
			_userService = userService;
			_userManager = userManager;
			_unitOfWork = unitOfWork;
		}

		// GET: api/User/profile
		[Authorize]
		[HttpGet("profile")]
		public async Task<ActionResult<ApplicationUser>> GetUserProfile()
		{
			var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
			var user = await _userService.GetUserByIdAsync(userId);

			if (user == null)
				return NotFound(new { message = "User not found" });

			 
			user.PasswordHash = null;
			user.SecurityStamp = null;
			user.ConcurrencyStamp = null;
			user.ResetPasswordToken = null;
			user.ResetPasswordTokenExpiry = null;

			return Ok(user);
		}

		// PUT: api/User/profile
		[Authorize]
		[HttpPut("profile")]
		public async Task<IActionResult> UpdateUserProfile(UserProfileUpdateDto profileUpdate)
		{
			if (!ModelState.IsValid)
				return BadRequest(new { errors = ModelState });

			var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
			var user = await _userService.GetUserByIdAsync(userId);

			if (user == null)
				return NotFound(new { message = "User not found" });

			// Update user properties
			user.FullName = profileUpdate.FullName;
			user.Gender = profileUpdate.Gender;
			user.DateOfBirth = profileUpdate.DateOfBirth;

			// Calculate age if date of birth provided
			if (profileUpdate.DateOfBirth != default)
			{
				int age = DateTime.Today.Year - profileUpdate.DateOfBirth.Year;
				if (profileUpdate.DateOfBirth.Date > DateTime.Today.AddYears(-age))
					age--;

				user.Age = age;
			}

			if (!string.IsNullOrEmpty(profileUpdate.ProfileImage))
				user.ProfileImage = profileUpdate.ProfileImage;

			var result = await _userService.UpdateUserProfileAsync(user);

			if (!result)
				return BadRequest(new { message = "Failed to update profile" });

			return NoContent();
		}

		// POST: api/User/change-password
		[Authorize]
		[HttpPost("change-password")]
		public async Task<IActionResult> ChangePassword(ChangePasswordDto changePasswordDto)
		{
			if (!ModelState.IsValid)
				return BadRequest(new { errors = ModelState });

			var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
			var result = await _userService.ChangePasswordAsync(
				userId,
				changePasswordDto.CurrentPassword,
				changePasswordDto.NewPassword);

			if (!result)
				return BadRequest(new { message = "Failed to change password. Please check your current password." });

			return Ok(new { message = "Password changed successfully" });
		}

		// GET: api/User
		[Authorize(Roles = Roles.Admin)]
		[HttpGet]
		public async Task<ActionResult<IEnumerable<ApplicationUser>>> GetAllUsers()
		{
			var users = await _unitOfWork.UserRepository.GetAllUsersAsync();
 
			foreach (var user in users)
			{
				user.PasswordHash = null;
				user.SecurityStamp = null;
				user.ConcurrencyStamp = null;
				user.ResetPasswordToken = null;
				user.ResetPasswordTokenExpiry = null;
			}

			return Ok(users);
		}

		// GET: api/User/{id}
		[Authorize(Roles = Roles.Admin)]
		[HttpGet("{id}")]
		public async Task<ActionResult<ApplicationUser>> GetUserById(string id)
		{
			var user = await _userService.GetUserByIdAsync(id);

			if (user == null)
				return NotFound(new { message = "User not found" });

			 
			user.PasswordHash = null;
			user.SecurityStamp = null;
			user.ConcurrencyStamp = null;
			user.ResetPasswordToken = null;
			user.ResetPasswordTokenExpiry = null;

			return Ok(user);
		}

		// DELETE: api/User/{id}
		[Authorize(Roles = Roles.Admin)]
		[HttpDelete("{id}")]
		public async Task<IActionResult> DeleteUser(string id)
		{
			var user = await _userManager.FindByIdAsync(id);

			if (user == null)
				return NotFound(new { message = "User not found" });

			var result = await _userManager.DeleteAsync(user);

			if (!result.Succeeded)
				return BadRequest(new { errors = result.Errors });

			return NoContent();
		}

	}
}
