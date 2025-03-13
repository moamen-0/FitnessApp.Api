using FitnessApp.Core.Interfaces.IRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FitnessApp.Core.Interfaces
{
	public interface IUnitOfWork
	{
		IUserRepository UserRepository { get; }
		
		Task<int> SaveChangesAsync();
	}
}
