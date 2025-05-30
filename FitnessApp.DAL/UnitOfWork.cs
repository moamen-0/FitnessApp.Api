﻿using FitnessApp.Core.Interfaces;
using FitnessApp.Core.Interfaces.IRepository;
using FitnessApp.DAL.Repositories;
using FitnessApp.DAL.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace FitnessApp.DAL
{
	public class UnitOfWork : IUnitOfWork
	{
		private readonly AppDbContext _context;
		private IUserRepository _userRepository;
		private IWorkoutPlanRepository _workoutPlanRepository;
		private IInBodyResultRepository _inBodyResultRepository;
		private IExerciseRepository _exerciseRepository;
		private IUserProgressRepository _userProgressRepository;
		private IFavoritesRepository _favoritesRepository;

		public UnitOfWork(AppDbContext context)
		{
			_context = context;
			_userRepository = new UserRepository(_context);
			_workoutPlanRepository = new WorkoutPlanRepository(_context);
			_inBodyResultRepository = new InBodyResultRepository(_context);
			_exerciseRepository = new ExerciseRepository(_context);
			_userProgressRepository = new UserProgressRepository(_context);
			_favoritesRepository = new FavoritesRepository(_context);
		}

		public IUserRepository UserRepository => _userRepository;
		public IWorkoutPlanRepository WorkoutPlanRepository => _workoutPlanRepository;
		public IInBodyResultRepository InBodyResultRepository => _inBodyResultRepository;
		public IExerciseRepository ExerciseRepository => _exerciseRepository;
		public IUserProgressRepository UserProgressRepository => _userProgressRepository;
		public IFavoritesRepository FavoritesRepository => _favoritesRepository;

		public DbContext DbContext => _context;
		public async Task<int> SaveChangesAsync()
		{
			return await _context.SaveChangesAsync();
		}
	}
}
