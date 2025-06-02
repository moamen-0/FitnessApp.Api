using FitnessApp.Core.Entities;
using FitnessApp.Core.Interfaces.IRepository;
using FitnessApp.Core.Interfaces.IService;
using System;
using System.Collections.Generic;
using System.Linq;
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

		public async Task<WorkoutPlan> GenerateCustomWorkoutPlanAsync(string? userId, string goal, string fitnessLevel)
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
				DurationWeeks = DetermineDurationWeeks(fitnessLevel, goal),
				CreatedAt = DateTime.UtcNow,
				WorkoutDays = new List<WorkoutDay>()
			};

			// Define workout days based on focus areas and fitness level
			workoutPlan.WorkoutDays = await CreateWorkoutDaysAsync(focusAreas, fitnessLevel, latestInBodyResult);

			// Save the workout plan
			await _workoutPlanRepository.CreateWorkoutPlanAsync(workoutPlan);

			return workoutPlan;
		}

		private int DetermineDurationWeeks(string fitnessLevel, string goal)
		{
			// Determine appropriate program length based on fitness level and goal
			return (fitnessLevel.ToLower(), goal.ToLower()) switch
			{
				("beginner", _) => 6, // Beginners need more time to adapt
				(_, "weight loss") => 8, // Weight loss programs generally longer
				(_, "muscle gain") => 12, // Hypertrophy takes longer
				_ => 4 // Default for general fitness
			};
		}

		private List<string> DetermineFocusAreas(InBodyResult inBodyResult, string goal)
		{
			List<string> focusAreas = new List<string>();

			switch (goal.ToLower())
			{
				case "weight loss":
					focusAreas.Add("Cardio");

					// If body fat % is very high, prioritize cardio over resistance training
					if (inBodyResult?.BodyFatPercentage > 30)
					{
						focusAreas.Add("HIIT");
						focusAreas.Add("Full Body Circuit");
					}
					else if (inBodyResult?.BodyFatPercentage > 20)
					{
						focusAreas.Add("Full Body");
						focusAreas.Add("HIIT");
					}
					else
					{
						focusAreas.Add("Strength");
						focusAreas.Add("HIIT");
					}
					break;

				case "muscle gain":
					focusAreas.Add("Strength");

					// Analyze body composition to determine focus
					if (inBodyResult?.MuscleMass < 25)
					{
						focusAreas.Add("Compound");
						focusAreas.Add("Full Body");
					}
					else if (inBodyResult?.BodyFatPercentage > 20)
					{
						// Higher body fat - include some cardio
						focusAreas.Add("Compound");
						focusAreas.Add("Cardio");
					}
					else
					{
						focusAreas.Add("Hypertrophy");
						focusAreas.Add("Split");
					}
					break;

				case "general fitness":
					focusAreas.Add("Cardio");
					focusAreas.Add("Strength");
					focusAreas.Add("Flexibility");
					focusAreas.Add("Core");
					break;

				case "strength":
					focusAreas.Add("Strength");
					focusAreas.Add("Compound");
					focusAreas.Add("Power");
					break;

				case "endurance":
					focusAreas.Add("Cardio");
					focusAreas.Add("Endurance");
					focusAreas.Add("Circuit");
					break;

				default:
					// Default balanced approach
					focusAreas.Add("Cardio");
					focusAreas.Add("Strength");
					focusAreas.Add("Core");
					break;
			}

			return focusAreas;
		}

		private async Task<List<WorkoutDay>> CreateWorkoutDaysAsync(List<string> focusAreas, string fitnessLevel, InBodyResult inBodyResult)
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

			// Create appropriate split based on focus areas and fitness level
			if (focusAreas.Contains("Full Body"))
			{
				workoutDays = await CreateFullBodySplit(daysPerWeek, fitnessLevel);
			}
			else if (focusAreas.Contains("Split") || fitnessLevel == "advanced")
			{
				workoutDays = await CreateBodyPartSplit(daysPerWeek, fitnessLevel, inBodyResult);
			}
			else if (daysPerWeek <= 3)
			{
				workoutDays = await CreateFullBodySplit(daysPerWeek, fitnessLevel);
			}
			else
			{
				workoutDays = await CreateUpperLowerSplit(daysPerWeek, fitnessLevel);
			}

			// Add cardio when applicable
			if (focusAreas.Contains("Cardio") || focusAreas.Contains("HIIT"))
			{
				AddCardioToWorkoutDays(workoutDays, fitnessLevel, focusAreas.Contains("HIIT"));
			}

			// Add core/flexibility work
			if (focusAreas.Contains("Core") || focusAreas.Contains("Flexibility"))
			{
				AddCoreworkToWorkoutDays(workoutDays, fitnessLevel);
			}

			return workoutDays;
		}

		private async Task<List<WorkoutDay>> CreateFullBodySplit(int daysPerWeek, string fitnessLevel)
		{
			List<WorkoutDay> workoutDays = new List<WorkoutDay>();

			// Evenly distribute workout days throughout the week
			int[] dayNumbers = daysPerWeek switch
			{
				2 => new[] { 1, 4 }, // Monday, Thursday
				3 => new[] { 1, 3, 5 }, // Monday, Wednesday, Friday
				4 => new[] { 1, 3, 5, 6 }, // Monday, Wednesday, Friday, Saturday
				5 => new[] { 1, 2, 3, 5, 6 }, // Monday, Tuesday, Wednesday, Friday, Saturday
				_ => new[] { 1, 3, 5 } // Default to 3 days
			};

			// Create full body workouts with slight variations
			string[] variations = { "A", "B", "C", "D", "E" };

			for (int i = 0; i < daysPerWeek; i++)
			{
				workoutDays.Add(await CreateFullBodyWorkoutDay(
					dayNumbers[i],
					$"Full Body {variations[i % variations.Length]}",
					fitnessLevel,
					i % variations.Length) // Pass variation number to create slightly different workouts
				);
			}

			return workoutDays;
		}

		private async Task<List<WorkoutDay>> CreateUpperLowerSplit(int daysPerWeek, string fitnessLevel)
		{
			List<WorkoutDay> workoutDays = new List<WorkoutDay>();

			// Determine day arrangement based on days per week
			switch (daysPerWeek)
			{
				case 4:
					// Upper/Lower/Upper/Lower
					workoutDays.Add(await CreateWorkoutDayForMuscleGroup(1, "Upper Body A", fitnessLevel, new[] { "Chest", "Back", "Shoulders", "Arms" }));
					workoutDays.Add(await CreateWorkoutDayForMuscleGroup(2, "Lower Body A", fitnessLevel, new[] { "Legs", "Glutes", "Calves", "Core" }));
					workoutDays.Add(await CreateWorkoutDayForMuscleGroup(4, "Upper Body B", fitnessLevel, new[] { "Chest", "Back", "Shoulders", "Arms" }));
					workoutDays.Add(await CreateWorkoutDayForMuscleGroup(5, "Lower Body B", fitnessLevel, new[] { "Legs", "Glutes", "Calves", "Core" }));
					break;

				case 5:
					// Upper/Lower/Push/Pull/Legs
					workoutDays.Add(await CreateWorkoutDayForMuscleGroup(1, "Upper Body", fitnessLevel, new[] { "Chest", "Back", "Shoulders", "Arms" }));
					workoutDays.Add(await CreateWorkoutDayForMuscleGroup(2, "Lower Body", fitnessLevel, new[] { "Legs", "Glutes", "Calves", "Core" }));
					workoutDays.Add(await CreateWorkoutDayForMuscleGroup(3, "Push", fitnessLevel, new[] { "Chest", "Shoulders", "Triceps" }));
					workoutDays.Add(await CreateWorkoutDayForMuscleGroup(5, "Pull", fitnessLevel, new[] { "Back", "Biceps", "Forearms" }));
					workoutDays.Add(await CreateWorkoutDayForMuscleGroup(6, "Legs", fitnessLevel, new[] { "Quadriceps", "Hamstrings", "Glutes", "Calves" }));
					break;

				default:
					// Default Upper/Lower/Upper
					workoutDays.Add(await CreateWorkoutDayForMuscleGroup(1, "Upper Body A", fitnessLevel, new[] { "Chest", "Back", "Shoulders", "Arms" }));
					workoutDays.Add(await CreateWorkoutDayForMuscleGroup(3, "Lower Body", fitnessLevel, new[] { "Legs", "Glutes", "Calves", "Core" }));
					workoutDays.Add(await CreateWorkoutDayForMuscleGroup(5, "Upper Body B", fitnessLevel, new[] { "Chest", "Back", "Shoulders", "Arms" }));
					break;
			}

			return workoutDays;
		}

		private async Task<List<WorkoutDay>> CreateBodyPartSplit(int daysPerWeek, string fitnessLevel, InBodyResult inBodyResult)
		{
			List<WorkoutDay> workoutDays = new List<WorkoutDay>();

			// Use InBody results to determine weak points and priorities
			bool prioritizeLegs = inBodyResult?.MuscleMass < 25;
			bool prioritizeUpperBody = inBodyResult?.BodyFatPercentage > 20;

			// Advanced 5-day split
			if (daysPerWeek == 5 && fitnessLevel.ToLower() == "advanced")
			{
				// Push/Pull/Legs/Push/Pull
				workoutDays.Add(await CreateWorkoutDayForMuscleGroup(1, "Push A", fitnessLevel, new[] { "Chest", "Shoulders", "Triceps" }));
				workoutDays.Add(await CreateWorkoutDayForMuscleGroup(2, "Pull A", fitnessLevel, new[] { "Back", "Biceps", "Forearms" }));
				workoutDays.Add(await CreateWorkoutDayForMuscleGroup(3, "Legs", fitnessLevel, new[] { "Quadriceps", "Hamstrings", "Glutes", "Calves" }));
				workoutDays.Add(await CreateWorkoutDayForMuscleGroup(5, "Push B", fitnessLevel, new[] { "Chest", "Shoulders", "Triceps" }));
				workoutDays.Add(await CreateWorkoutDayForMuscleGroup(6, "Pull B", fitnessLevel, new[] { "Back", "Biceps", "Forearms" }));
			}
			else if (prioritizeLegs)
			{
				// Body part split with leg focus
				workoutDays.Add(await CreateWorkoutDayForMuscleGroup(1, "Chest and Triceps", fitnessLevel, new[] { "Chest", "Triceps" }));
				workoutDays.Add(await CreateWorkoutDayForMuscleGroup(2, "Back and Biceps", fitnessLevel, new[] { "Back", "Biceps" }));
				workoutDays.Add(await CreateWorkoutDayForMuscleGroup(3, "Leg Focus", fitnessLevel, new[] { "Quadriceps", "Hamstrings", "Glutes" }));
				workoutDays.Add(await CreateWorkoutDayForMuscleGroup(5, "Shoulders and Arms", fitnessLevel, new[] { "Shoulders", "Arms" }));
				workoutDays.Add(await CreateWorkoutDayForMuscleGroup(6, "Legs and Core", fitnessLevel, new[] { "Quadriceps", "Hamstrings", "Calves", "Core" }));
			}
			else
			{
				// Standard body part split
				workoutDays.Add(await CreateWorkoutDayForMuscleGroup(1, "Chest", fitnessLevel, new[] { "Chest", "Triceps" }));
				workoutDays.Add(await CreateWorkoutDayForMuscleGroup(2, "Back", fitnessLevel, new[] { "Back", "Biceps" }));
				workoutDays.Add(await CreateWorkoutDayForMuscleGroup(3, "Legs", fitnessLevel, new[] { "Quadriceps", "Hamstrings", "Glutes", "Calves" }));
				workoutDays.Add(await CreateWorkoutDayForMuscleGroup(5, "Shoulders", fitnessLevel, new[] { "Shoulders", "Traps" }));
				workoutDays.Add(await CreateWorkoutDayForMuscleGroup(6, "Arms and Core", fitnessLevel, new[] { "Biceps", "Triceps", "Forearms", "Core" }));
			}

			return workoutDays;
		}

		private async Task<WorkoutDay> CreateFullBodyWorkoutDay(int dayNumber, string name, string fitnessLevel, int variation = 0)
		{
			// Create a full body workout day
			var workoutDay = new WorkoutDay
			{
				Name = name,
				DayNumber = dayNumber,
				Exercises = new List<WorkoutExercise>()
			};

			// Get exercises for this full body workout
			// Each variation emphasizes different movement patterns or muscle groups
			string[] primaryFocus = variation switch
			{
				0 => new[] { "Compound", "Chest", "Back", "Legs" }, // Compound focus
				1 => new[] { "Legs", "Back", "Core" },              // Lower body focus
				2 => new[] { "Chest", "Shoulders", "Arms" },        // Upper body focus
				3 => new[] { "Pull", "Back", "Biceps" },            // Pull focus
				4 => new[] { "Push", "Chest", "Triceps" },          // Push focus
				_ => new[] { "Compound", "Chest", "Back", "Legs" }  // Default
			};

			// Get compound exercises first
			var compoundExercises = await _exerciseRepository.GetExercisesByTypeAsync("Compound");
			var selectedCompounds = SelectExercisesBasedOnLevel(
				compoundExercises.Where(e => primaryFocus.Any(f => e.MuscleGroup.Contains(f) || e.Category.Contains(f))),
				fitnessLevel,
				3); // 3 compound exercises

			// Get isolation exercises for remaining muscle groups
			var isolationExercises = await _exerciseRepository.GetAllExercisesAsync();
			var selectedIsolations = SelectExercisesBasedOnLevel(
				isolationExercises.Where(e => !e.Category.Contains("Compound") && primaryFocus.Any(f => e.MuscleGroup.Contains(f))),
				fitnessLevel,
				4); // 4 isolation exercises

			// Add exercises to the workout day
			int exerciseOrder = 1;

			// Add compound exercises first (heavier, more demanding movements)
			foreach (var exercise in selectedCompounds)
			{
				workoutDay.Exercises.Add(CreateWorkoutExercise(exercise, exerciseOrder++, fitnessLevel));
			}

			// Add isolation exercises
			foreach (var exercise in selectedIsolations)
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

			// First, add compound exercises for the targeted muscle groups
			var allExercises = await _exerciseRepository.GetAllExercisesAsync();
			var compoundExercises = allExercises.Where(e =>
				e.Category.Contains("Compound") &&
				muscleGroups.Any(mg => e.MuscleGroup.Contains(mg)));

			var selectedCompounds = SelectExercisesBasedOnLevel(compoundExercises, fitnessLevel, 2);

			foreach (var exercise in selectedCompounds)
			{
				workoutDay.Exercises.Add(CreateWorkoutExercise(exercise, exerciseOrder++, fitnessLevel));
			}

			// Then add isolation exercises for each muscle group
			foreach (var muscleGroup in muscleGroups)
			{
				// Skip if this is a general category like "Arms" and we're already adding specific arm muscles
				if ((muscleGroup == "Arms" && muscleGroups.Any(mg => mg == "Biceps" || mg == "Triceps")) ||
					(muscleGroup == "Legs" && muscleGroups.Any(mg => mg == "Quadriceps" || mg == "Hamstrings")))
					continue;

				var exercises = await _exerciseRepository.GetExercisesByMuscleGroupAsync(muscleGroup);
				var isolationExercises = exercises.Where(e => !e.Category.Contains("Compound"));

				// Determine how many exercises per muscle group based on focus
				int exercisesPerGroup = muscleGroups.Length <= 2 ? 3 : 2;

				var selectedExercises = SelectExercisesBasedOnLevel(isolationExercises, fitnessLevel, exercisesPerGroup);

				foreach (var exercise in selectedExercises)
				{
					workoutDay.Exercises.Add(CreateWorkoutExercise(exercise, exerciseOrder++, fitnessLevel));
				}
			}

			return workoutDay;
		}

		private IEnumerable<Exercise> SelectExercisesBasedOnLevel(IEnumerable<Exercise> exercises, string fitnessLevel, int count = 5)
		{
			// First filter by difficulty level appropriate for the user
			var filteredExercises = exercises.Where(e =>
				e.DifficultyLevel.ToLower() == fitnessLevel.ToLower() ||
				(fitnessLevel.ToLower() == "intermediate" && e.DifficultyLevel.ToLower() == "beginner") ||
				(fitnessLevel.ToLower() == "advanced" &&
				 (e.DifficultyLevel.ToLower() == "intermediate" || e.DifficultyLevel.ToLower() == "advanced")));

			// If not enough exercises match the level, include exercises from adjacent levels
			if (!filteredExercises.Any() || filteredExercises.Count() < count)
			{
				filteredExercises = exercises;
			}

			// Priority for exercises with AI model references for form checking
			var aiExercises = filteredExercises.Where(e => !string.IsNullOrEmpty(e.AIModelReference));

			// Combine AI-enabled exercises with regular exercises
			var selectedExercises = new List<Exercise>();

			// Add AI exercises first (up to half of count)
			selectedExercises.AddRange(aiExercises.OrderBy(e => Guid.NewGuid()).Take(count / 2));

			// Fill the rest with random exercises, excluding already selected ones
			var remainingExercises = filteredExercises.Where(e => !selectedExercises.Contains(e))
													 .OrderBy(e => Guid.NewGuid())
													 .Take(count - selectedExercises.Count);

			selectedExercises.AddRange(remainingExercises);

			return selectedExercises;
		}

		private WorkoutExercise CreateWorkoutExercise(Exercise exercise, int order, string fitnessLevel)
		{
			// Determine if the exercise is compound or isolation
			bool isCompound = exercise.Category.Contains("Compound");
			bool isCardio = exercise.Category.Contains("Cardio");

			// Set appropriate sets and reps based on fitness level and exercise type
			int sets = (fitnessLevel.ToLower(), isCompound) switch
			{
				("beginner", true) => 3,
				("beginner", false) => 2,
				("intermediate", true) => 4,
				("intermediate", false) => 3,
				("advanced", true) => 5,
				("advanced", false) => 4,
				_ => 3
			};

			int? reps = (exercise.Category.ToLower(), fitnessLevel.ToLower()) switch
			{
				var (cat, _) when cat.Contains("cardio") => null, // For timed exercises
				var (cat, level) when cat.Contains("strength") && level == "beginner" => 10,
				var (cat, level) when cat.Contains("strength") && level == "intermediate" => 8,
				var (cat, level) when cat.Contains("strength") && level == "advanced" => 6,
				var (cat, _) when cat.Contains("hypertrophy") => 12,
				var (cat, _) when cat.Contains("endurance") => 15,
				_ => isCompound ? 8 : 12 // Default reps for compound vs isolation
			};

			// Duration for cardio or timed exercises
			int? duration = isCardio ? fitnessLevel.ToLower() switch
			{
				"beginner" => 30,
				"intermediate" => 45,
				"advanced" => 60,
				_ => 30
			} : null;

			// Rest periods vary by exercise type and fitness level
			int? restBetweenSets = (isCompound, fitnessLevel.ToLower()) switch
			{
				(true, "beginner") => 90,
				(true, "intermediate") => 75,
				(true, "advanced") => 60,
				(false, "beginner") => 60,
				(false, "intermediate") => 45,
				(false, "advanced") => 30,
				_ => 60
			};

			// Create exercise with appropriate parameters
			return new WorkoutExercise
			{
				ExerciseId = exercise.Id,
				Sets = sets,
				RepsPerSet = reps,
				Duration = duration,
				OrderInWorkout = order,
				RestBetweenSets = restBetweenSets,
				Notes = $"Focus on proper form. {exercise.Description}"
			};
		}

		private void AddCardioToWorkoutDays(List<WorkoutDay> workoutDays, string fitnessLevel, bool includeHiit)
		{
			// Determine cardio parameters based on fitness level
			int cardioDuration = fitnessLevel.ToLower() switch
			{
				"beginner" => 20,
				"intermediate" => 30,
				"advanced" => 45,
				_ => 20
			};

			// HIIT parameters (if applicable)
			int hiitIntervals = fitnessLevel.ToLower() switch
			{
				"beginner" => 6,
				"intermediate" => 8,
				"advanced" => 10,
				_ => 6
			};

			// Add appropriate cardio to each workout day
			foreach (var day in workoutDays)
			{
				// Modify workout name to indicate cardio inclusion
				if (!day.Name.Contains("Cardio") && !day.Name.Contains("HIIT"))
				{
					day.Name += includeHiit ? " + HIIT" : " + Cardio";
				}

				// Create cardio exercise notes
				string cardioNotes = includeHiit
					? $"{hiitIntervals} intervals of 30s work, 30s rest. Choose from: sprints, burpees, or battle ropes."
					: $"{cardioDuration} minutes of steady-state cardio. Choose from: jogging, cycling, or elliptical.";

				// Add a note for the cardio component
				// In a full implementation, we would add actual cardio exercises here
			}
		}

		private void AddCoreworkToWorkoutDays(List<WorkoutDay> workoutDays, string fitnessLevel)
		{
			// Add core exercises to the end of each workout

			string coreNotes = fitnessLevel.ToLower() switch
			{
				"beginner" => "2 sets of 30s planks, 12 crunches, and 10 bird dogs per side.",
				"intermediate" => "3 sets of 45s planks, 15 Russian twists per side, and 12 mountain climbers per side.",
				"advanced" => "3 sets of 60s planks, 20 hanging leg raises, and 15 cable woodchoppers per side.",
				_ => "2-3 sets of core exercises of your choice."
			};

			// Add core notes to each workout day
			// In a full implementation, we would add actual core exercises here
		}
	}
}