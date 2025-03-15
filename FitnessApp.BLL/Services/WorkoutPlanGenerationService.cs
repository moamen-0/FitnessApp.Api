using FitnessApp.Core.Entities;
using FitnessApp.Core.Interfaces.IRepository;
using FitnessApp.Core.Interfaces.IService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FitnessApp.BLL.Services
{
	public class WorkoutPlanGenerationService : IWorkoutPlanGenerationService
	{
		private readonly IWorkoutPlanRepository _workoutPlanRepository;
		private readonly IExerciseRepository _exerciseRepository;
		private readonly IInBodyResultRepository _inBodyResultRepository;

		public WorkoutPlanGenerationService(
			IWorkoutPlanRepository workoutPlanRepository,
			IExerciseRepository exerciseRepository,
			IInBodyResultRepository inBodyResultRepository)
		{
			_workoutPlanRepository = workoutPlanRepository;
			_exerciseRepository = exerciseRepository;
			_inBodyResultRepository = inBodyResultRepository;
		}

		public async Task<WorkoutPlan> GenerateCustomWorkoutPlanAsync(string userId, string goal, string fitnessLevel)
		{
			// Get the user's latest InBody results
			var latestInBodyResult = await _inBodyResultRepository.GetLatestInBodyResultAsync(userId);

			// Determine the appropriate focus areas based on InBody results and goals
			List<string> focusAreas = DetermineFocusAreas(latestInBodyResult, goal);

			// Define the workout plan structure
			var workoutPlan = new WorkoutPlan
			{
				Name = $"Custom {goal} Plan",
				Description = $"Personalized workout plan based on your {goal} goal",
				UserId = userId,
				IsTemplate = false,
				DifficultyLevel = fitnessLevel,
				FocusArea = string.Join(", ", focusAreas),
				DurationWeeks = 4,
				CreatedAt = DateTime.UtcNow,
				WorkoutDays = new List<WorkoutDay>()
			};

			// Define workout days based on focus areas and fitness level
			workoutPlan.WorkoutDays = await CreateWorkoutDaysAsync(focusAreas, fitnessLevel);

			// Save the workout plan
			await _workoutPlanRepository.CreateWorkoutPlanAsync(workoutPlan);

			return workoutPlan;
		}

		private List<string> DetermineFocusAreas(InBodyResult inBodyResult, string goal)
		{
			List<string> focusAreas = new List<string>();

			switch (goal.ToLower())
			{
				case "weight loss":
					focusAreas.Add("Cardio");
					focusAreas.Add("Full Body");

					// If body fat % is high, add more cardio
					if (inBodyResult?.BodyFatPercentage > 25)
					{
						focusAreas.Add("HIIT");
					}
					break;

				case "muscle gain":
					focusAreas.Add("Strength");

					// If muscle mass is low, focus on compound exercises
					if (inBodyResult?.MuscleMass < 30)
					{
						focusAreas.Add("Compound");
					}
					else
					{
						focusAreas.Add("Hypertrophy");
					}
					break;

				case "general fitness":
					focusAreas.Add("Cardio");
					focusAreas.Add("Strength");
					focusAreas.Add("Flexibility");
					break;
			}

			return focusAreas;
		}

		private async Task<List<WorkoutDay>> CreateWorkoutDaysAsync(List<string> focusAreas, string fitnessLevel)
		{
			List<WorkoutDay> workoutDays = new List<WorkoutDay>();

			// Determine number of workout days based on fitness level
			int daysPerWeek = fitnessLevel.ToLower() switch
			{
				"beginner" => 3,
				"intermediate" => 4,
				"advanced" => 5,
				_ => 3
			};

			// Create workout structure based on days per week
			switch (daysPerWeek)
			{
				case 3:
					// 3-day split (Full Body approach)
					workoutDays.Add(await CreateFullBodyWorkoutDay(1, "Full Body A", fitnessLevel));
					workoutDays.Add(await CreateFullBodyWorkoutDay(3, "Full Body B", fitnessLevel));
					workoutDays.Add(await CreateFullBodyWorkoutDay(5, "Full Body C", fitnessLevel));
					break;

				case 4:
					// 4-day split (Upper/Lower approach)
					workoutDays.Add(await CreateWorkoutDayForMuscleGroup(1, "Upper Body", fitnessLevel, new[] { "Chest", "Back", "Shoulders", "Arms" }));
					workoutDays.Add(await CreateWorkoutDayForMuscleGroup(2, "Lower Body", fitnessLevel, new[] { "Legs", "Glutes", "Calves", "Core" }));
					workoutDays.Add(await CreateWorkoutDayForMuscleGroup(4, "Upper Body", fitnessLevel, new[] { "Chest", "Back", "Shoulders", "Arms" }));
					workoutDays.Add(await CreateWorkoutDayForMuscleGroup(5, "Lower Body", fitnessLevel, new[] { "Legs", "Glutes", "Calves", "Core" }));
					break;

				case 5:
					// 5-day split (Push/Pull/Legs approach)
					workoutDays.Add(await CreateWorkoutDayForMuscleGroup(1, "Push", fitnessLevel, new[] { "Chest", "Shoulders", "Triceps" }));
					workoutDays.Add(await CreateWorkoutDayForMuscleGroup(2, "Pull", fitnessLevel, new[] { "Back", "Biceps", "Forearms" }));
					workoutDays.Add(await CreateWorkoutDayForMuscleGroup(3, "Legs", fitnessLevel, new[] { "Quadriceps", "Hamstrings", "Glutes", "Calves" }));
					workoutDays.Add(await CreateWorkoutDayForMuscleGroup(5, "Upper Body", fitnessLevel, new[] { "Chest", "Back", "Shoulders", "Arms" }));
					workoutDays.Add(await CreateWorkoutDayForMuscleGroup(6, "Lower Body", fitnessLevel, new[] { "Legs", "Glutes", "Core" }));
					break;
			}

			// If "Cardio" is a focus area, add cardio to appropriate days
			if (focusAreas.Contains("Cardio"))
			{
				AddCardioToWorkoutDays(workoutDays, fitnessLevel);
			}

			return workoutDays;
		}

		private async Task<WorkoutDay> CreateFullBodyWorkoutDay(int dayNumber, string name, string fitnessLevel)
		{
			// Create a full body workout day
			var workoutDay = new WorkoutDay
			{
				Name = name,
				DayNumber = dayNumber,
				Exercises = new List<WorkoutExercise>()
			};

			// Get compound exercises for full body workout
			var compoundExercises = await _exerciseRepository.GetExercisesByTypeAsync("Compound");
			var selectedExercises = SelectExercisesBasedOnLevel(compoundExercises, fitnessLevel);

			// Add exercises to the workout day
			int exerciseOrder = 1;
			foreach (var exercise in selectedExercises)
			{
				workoutDay.Exercises.Add(CreateWorkoutExercise(exercise, exerciseOrder++, fitnessLevel));
			}

			return workoutDay;
		}

		private async Task<WorkoutDay> CreateWorkoutDayForMuscleGroup(int dayNumber, string name, string fitnessLevel, string[] muscleGroups)
		{
			var workoutDay = new WorkoutDay
			{
				Name = name,
				DayNumber = dayNumber,
				Exercises = new List<WorkoutExercise>()
			};

			int exerciseOrder = 1;

			// Add exercises for each muscle group
			foreach (var muscleGroup in muscleGroups)
			{
				var exercises = await _exerciseRepository.GetExercisesByMuscleGroupAsync(muscleGroup);
				var selectedExercises = SelectExercisesBasedOnLevel(exercises, fitnessLevel, 2); // 2 exercises per muscle group

				foreach (var exercise in selectedExercises)
				{
					workoutDay.Exercises.Add(CreateWorkoutExercise(exercise, exerciseOrder++, fitnessLevel));
				}
			}

			return workoutDay;
		}

		private IEnumerable<Exercise> SelectExercisesBasedOnLevel(IEnumerable<Exercise> exercises, string fitnessLevel, int count = 5)
		{
			// Select exercises appropriate for the fitness level
			var filteredExercises = exercises.Where(e => e.DifficultyLevel.ToLower() == fitnessLevel.ToLower() ||
													   (fitnessLevel.ToLower() == "intermediate" && e.DifficultyLevel.ToLower() == "beginner") ||
													   (fitnessLevel.ToLower() == "advanced" &&
														(e.DifficultyLevel.ToLower() == "intermediate" || e.DifficultyLevel.ToLower() == "advanced")));

			// If not enough exercises match the level, include exercises from adjacent levels
			if (!filteredExercises.Any())
			{
				filteredExercises = exercises;
			}

			// Get a random selection of exercises
			return filteredExercises.OrderBy(e => Guid.NewGuid()).Take(Math.Min(count, filteredExercises.Count()));
		}

		private WorkoutExercise CreateWorkoutExercise(Exercise exercise, int order, string fitnessLevel)
		{
			// Set appropriate sets and reps based on fitness level
			int sets = fitnessLevel.ToLower() switch
			{
				"beginner" => 3,
				"intermediate" => 4,
				"advanced" => 5,
				_ => 3
			};

			int? reps = exercise.Category.ToLower() switch
			{
				"strength" => fitnessLevel.ToLower() switch
				{
					"beginner" => 10,
					"intermediate" => 8,
					"advanced" => 6,
					_ => 10
				},
				"cardio" => null, // For timed exercises
				_ => 12
			};

			int? duration = exercise.Category.ToLower() == "cardio" ? 60 : null; // 60 seconds for cardio

			return new WorkoutExercise
			{
				ExerciseId = exercise.Id,
				Sets = sets,
				RepsPerSet = reps,
				Duration = duration,
				OrderInWorkout = order,
				RestBetweenSets = fitnessLevel.ToLower() switch
				{
					"beginner" => 60,
					"intermediate" => 45,
					"advanced" => 30,
					_ => 60
				},
				Notes = $"Focus on proper form. {exercise.Description}"
			};
		}

		private void AddCardioToWorkoutDays(List<WorkoutDay> workoutDays, string fitnessLevel)
		{
			// Determine cardio duration based on fitness level
			int cardioDuration = fitnessLevel.ToLower() switch
			{
				"beginner" => 20,
				"intermediate" => 30,
				"advanced" => 45,
				_ => 20
			};

			// Add note about cardio to each workout day
			foreach (var day in workoutDays)
			{
				day.Name += " + Cardio";
				// We would normally add specific cardio exercises here
				// For simplicity, just adding a note
			}
		}
	}
}
