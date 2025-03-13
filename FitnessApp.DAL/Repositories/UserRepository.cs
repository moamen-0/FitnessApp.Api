using FitnessApp.Core.Entities;
using FitnessApp.Core.Interfaces.IRepository;
using FitnessApp.DAL.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FitnessApp.DAL.Repositories
{
	public class UserRepository : IUserRepository
	{
		private readonly AppDbContext _context;

		public UserRepository(AppDbContext context)
		{
			_context = context;
		}

		public async Task<ApplicationUser> AddUserAsync(ApplicationUser user)
		{
			await _context.ApplicationUsers.AddAsync(user);
			await _context.SaveChangesAsync();
			return user;
		}

		public async Task<ApplicationUser> DeleteUserAsync(string id)
		{
			var user = await _context.ApplicationUsers.FindAsync(id);
			if (user == null) return null;

			_context.ApplicationUsers.Remove(user);
			await _context.SaveChangesAsync();
			return user;
		}

		public async Task<bool> EmailExistsAsync(string email)
		{
			return await _context.ApplicationUsers.AnyAsync(u => u.Email == email);
		}

		public async Task<IEnumerable<ApplicationUser>> GetAllUsersAsync()
		{
			return await _context.ApplicationUsers.ToListAsync();
		}

		public async Task<ApplicationUser> GetUserByEmailAsync(string email)
		{
			return await _context.ApplicationUsers.FirstOrDefaultAsync(u => u.Email == email);
		}

		public async Task<ApplicationUser> GetUserByFullNameAsync(string fullName)
		{
			return await _context.ApplicationUsers.FirstOrDefaultAsync(u => u.FullName == fullName);
		}

		public async Task<ApplicationUser> GetUserByIdAsync(string userId)
		{
			return await _context.ApplicationUsers.FindAsync(userId);
		}

		public async Task<ApplicationUser> UpdateUserAsync(ApplicationUser user)
		{
			_context.ApplicationUsers.Update(user);
			await _context.SaveChangesAsync();
			return user;
		}

		public async Task<bool> UserExistsAsync(string id)
		{
			return await _context.ApplicationUsers.AnyAsync(u => u.Id == id);
		}
	}
}
