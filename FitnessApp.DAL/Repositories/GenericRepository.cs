using FitnessApp.Core.Interfaces;
using FitnessApp.FitnessApp.DAL.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FitnessApp.DAL.Repositories
{
	public class GenericRepository<T> : IGenericRepository<T> where T : class
	{
		private readonly AppDbContext _context;
		private readonly DbSet<T> _dbSet;

		public GenericRepository(AppDbContext context)
		{
			_context = context;
			_dbSet = context.Set<T>();
		}

		public Task AddAsync(T entity)
		{
			_dbSet.Add(entity);
			return _context.SaveChangesAsync();


		}

		public async Task DeleteAsync(T entity)
		{
			_dbSet.Remove(entity);
			await _context.SaveChangesAsync();

		}

		public async Task<IEnumerable<T>> GetAllAsync()
		{
			return await Task.FromResult(_dbSet.AsEnumerable());
			}

		public async Task<T?> GetByIdAsync(int id)
		{
			var entity = _dbSet.Find(id);
			if (entity == null)
			{
				return await Task.FromResult<T?>(null);
			}

			return await _dbSet.FindAsync(id);

		}

		public async Task UpdateAsync(T entity)
		{
			_dbSet.Update(entity);
			await _context.SaveChangesAsync();
		}
	}
}
