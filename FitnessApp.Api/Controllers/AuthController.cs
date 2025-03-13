using FitnessApp.Core.Dtos;
using FitnessApp.Core.Entities;
using FitnessApp.Core.Interfaces.IService;
using FitnessApp.DAL.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace FitnessApp.Api.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class AuthController : ControllerBase
	{
		private readonly UserManager<ApplicationUser> _userManager;
		private readonly IUserService _userService;
		private readonly IConfiguration _configuration;

		public AuthController(
			UserManager<ApplicationUser> userManager,
			IUserService userService,
			IConfiguration configuration)
		{
			_userManager = userManager;
			_userService = userService;
			_configuration = configuration;
		}

		[Authorize(Roles = Roles.Admin)]
		[HttpPost("assign-role")]
		public async Task<IActionResult> AssignRole([FromBody] AssignRoleModel model)
		{
			if (model == null || string.IsNullOrEmpty(model.UserId) || string.IsNullOrEmpty(model.RoleName))
				return BadRequest(new { message = "User ID and role name are required" });

			var user = await _userManager.FindByIdAsync(model.UserId);
			if (user == null)
				return NotFound(new { message = "User not found" });

			var result = await _userManager.AddToRoleAsync(user, model.RoleName);
			if (!result.Succeeded)
				return BadRequest(new { errors = result.Errors });

			return Ok(new { message = "Role assigned successfully" });
		}

		[HttpPost("login")]
		public async Task<IActionResult> Login([FromBody] LoginDto model)
		{
			if (!ModelState.IsValid)
				return BadRequest(new { errors = ModelState });

			// Find user by email
			var user = await _userManager.FindByEmailAsync(model.Email);
			if (user == null)
				return Unauthorized(new { message = "Invalid credentials" });

			// Check password
			var isPasswordValid = await _userManager.CheckPasswordAsync(user, model.Password);
			if (!isPasswordValid)
				return Unauthorized(new { message = "Invalid credentials" });

			// Get user roles
			var userRoles = await _userManager.GetRolesAsync(user);

			// Create claims for the token
			var claims = new List<Claim>
			{
				new Claim(ClaimTypes.NameIdentifier, user.Id),
				new Claim(ClaimTypes.Name, user.FullName),
				new Claim(ClaimTypes.Email, user.Email),
				new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
			};

			// Add role claims
			foreach (var role in userRoles)
			{
				claims.Add(new Claim(ClaimTypes.Role, role));
			}

			// Create token
			var token = GenerateJwtToken(claims);

			return Ok(new
			{
				
				email = user.Email,
				fullName = user.FullName
			});
		}

		[HttpPost("register")]
		public async Task<IActionResult> Register([FromBody] RegisterDto model)
		{
			if (!ModelState.IsValid)
				return BadRequest(new { errors = ModelState });

			var newUser = new ApplicationUser
			{
				FullName = model.FullName,
				Email = model.Email,
				UserName = model.Email
			};

			var result = await _userManager.CreateAsync(newUser, model.Password);

			if (!result.Succeeded)
				return BadRequest(new { errors = result.Errors });

			// Assign default role
			await _userManager.AddToRoleAsync(newUser, Roles.User);

			return Ok(new
			{
				message = "User registered successfully",
			
			});
		}

		// No need for explicit logout with JWT - client simply discards the token
		// But we can implement token blacklisting if needed

		[HttpPost("forgot-password")]
		public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordModel model)
		{
			if (string.IsNullOrEmpty(model.Email))
				return BadRequest(new { message = "Email is required" });

			// Attempt to send a reset link regardless of whether the email exists
			// This prevents email enumeration attacks
			await _userService.InitiatePasswordResetAsync(model.Email);

			// Always return success to maintain privacy
			return Ok(new { message = "If your email exists in our system, you will receive a password reset link" });
		}

		[HttpPost("reset-password")]
		public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDto model)
		{
			if (!ModelState.IsValid)
				return BadRequest(new { errors = ModelState });

			if (string.IsNullOrEmpty(model.Email) || string.IsNullOrEmpty(model.Token))
				return BadRequest(new { message = "Email and token are required" });

			var result = await _userService.ResetPasswordAsync(model.Email, model.Token, model.NewPassword);
			if (!result)
				return BadRequest(new { message = "Invalid reset attempt. The token may have expired." });

			return Ok(new { message = "Password has been reset successfully" });
		}

		private JwtSecurityToken GenerateJwtToken(IEnumerable<Claim> claims)
		{
			var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Secret"]));
			var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
			var expires = DateTime.Now.AddHours(Convert.ToDouble(_configuration["JWT:ExpirationInHours"]));

			var token = new JwtSecurityToken(
				issuer: _configuration["JWT:ValidIssuer"],
				audience: _configuration["JWT:ValidAudience"],
				claims: claims,
				expires: expires,
				signingCredentials: credentials
			);

			return token;
		}
	}

	// Models for API requests
	public class AssignRoleModel
	{
		public string UserId { get; set; }
		public string RoleName { get; set; }
	}

	public class ForgotPasswordModel
	{
		public string Email { get; set; }
	}
}

