using FitnessApp.Core.Interfaces.IRepository;
using Microsoft.EntityFrameworkCore;
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
		IWorkoutPlanRepository WorkoutPlanRepository { get; }
		IInBodyResultRepository InBodyResultRepository { get; }
		IExerciseRepository ExerciseRepository { get; }
		IUserProgressRepository UserProgressRepository { get; }
		IFavoritesRepository FavoritesRepository { get; }
		DbContext DbContext { get; }
		Task<int> SaveChangesAsync();
	}
}
