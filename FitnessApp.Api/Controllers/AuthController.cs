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

		//[Authorize(Roles = Roles.Admin)]
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

			 
			var user = await _userManager.FindByEmailAsync(model.Email);
			if (user == null)
				return Unauthorized(new { message = "Invalid credentials" });

			 
			var isPasswordValid = await _userManager.CheckPasswordAsync(user, model.Password);
			if (!isPasswordValid)
				return Unauthorized(new { message = "Invalid credentials" });

			 
			var userRoles = await _userManager.GetRolesAsync(user);

			 
			var claims = new List<Claim>
			{
				new Claim(ClaimTypes.NameIdentifier, user.Id),
				new Claim(ClaimTypes.Name, user.FullName),
				new Claim(ClaimTypes.Email, user.Email),
				new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
			};

			 
			foreach (var role in userRoles)
			{
				claims.Add(new Claim(ClaimTypes.Role, role));
			}

			 
			var token = GenerateJwtToken(claims);

			return Ok(new
			{
				
				email = user.Email,
				fullName = user.FullName,
				token = new JwtSecurityTokenHandler().WriteToken(token),
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

			 
			await _userManager.AddToRoleAsync(newUser, Roles.User);

			return Ok(new
			{
				message = "User registered successfully",
			
			});
		}


		[AllowAnonymous]
		[HttpPost("forgot-password")]
		public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordDto forgotPasswordDto)
		{
			if (!ModelState.IsValid)
				return BadRequest(new { errors = ModelState });

			var result = await _userService.InitiatePasswordResetAsync(forgotPasswordDto.Email);

			if (!result)
				return BadRequest(new { message = "Failed to initiate password reset. Please check the email provided." });

			return Ok(new { message = "Password reset link has been sent to your email." });
		}

		[AllowAnonymous]
		[HttpPost("reset-password")]
		public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDto resetPasswordDto)
		{
			if (!ModelState.IsValid)
				return BadRequest(new { errors = ModelState });

			var result = await _userService.ResetPasswordAsync(
				resetPasswordDto.Email,
				resetPasswordDto.Token,
				resetPasswordDto.NewPassword);

			if (!result)
				return BadRequest(new { message = "Failed to reset password. Please check the token and try again." });

			return Ok(new { message = "Password has been reset successfully." });
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

	 
	public class AssignRoleModel
	{
		public string UserId { get; set; }
		public string RoleName { get; set; }
	}

}

