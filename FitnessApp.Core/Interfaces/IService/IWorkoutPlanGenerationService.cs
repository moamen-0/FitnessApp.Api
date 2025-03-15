using FitnessApp.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FitnessApp.Core.Interfaces.IService
{
	public interface IWorkoutPlanGenerationService
	{
		Task<WorkoutPlan> GenerateCustomWorkoutPlanAsync(string userId, string goal, string fitnessLevel);

	}
}
