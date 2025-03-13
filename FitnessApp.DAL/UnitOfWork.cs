using FitnessApp.Core.Interfaces;
using FitnessApp.Core.Interfaces.IRepository;
using FitnessApp.DAL.Repositories;
using FitnessApp.DAL.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FitnessApp.DAL
{
	public class UnitOfWork : IUnitOfWork
	{
		private readonly AppDbContext _context;
		private IUserRepository _userRepository;

		public UnitOfWork(AppDbContext context)
		{
			_context = context;
			_userRepository = new UserRepository(_context);
		}

		public IUserRepository UserRepository => _userRepository;

		public async Task<int> SaveChangesAsync()
		{
			return await _context.SaveChangesAsync();
		}
	}
}
