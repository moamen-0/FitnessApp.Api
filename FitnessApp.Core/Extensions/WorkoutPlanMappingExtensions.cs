using FitnessApp.Core.Dtos;
using FitnessApp.Core.Entities;

namespace FitnessApp.Core.Extensions
{
    public static class WorkoutPlanMappingExtensions
    {
        public static WorkoutPlanDto ToDto(this WorkoutPlan plan)
        {
            return new WorkoutPlanDto
            {
                Id = plan.Id,
                Name = plan.Name,
                Description = plan.Description,
                DifficultyLevel = plan.DifficultyLevel,
                FocusArea = plan.FocusArea,
                DurationWeeks = plan.DurationWeeks,
                CreatedAt = plan.CreatedAt,
                IsTemplate = plan.IsTemplate,
                WorkoutDays = plan.WorkoutDays?.Select(wd => wd.ToDto()).ToList() ?? new List<WorkoutDayDto>()
            };
        }

        public static WorkoutDayDto ToDto(this WorkoutDay day)
        {
            return new WorkoutDayDto
            {
                Id = day.Id,
                Name = day.Name,
                DayNumber = day.DayNumber,
                Exercises = day.Exercises?.Select(ex => ex.ToDto()).ToList() ?? new List<WorkoutExerciseDto>()
            };
        }

        public static WorkoutExerciseDto ToDto(this WorkoutExercise exercise)
        {
            return new WorkoutExerciseDto
            {
                Id = exercise.Id,
                ExerciseId = exercise.ExerciseId,
                ExerciseName = exercise.Exercise?.Name,
                ExerciseDescription = exercise.Exercise?.Description,
                MuscleGroup = exercise.Exercise?.MuscleGroup,
                Equipment = exercise.Exercise?.Equipment,
                DifficultyLevel = exercise.Exercise?.DifficultyLevel,
                ExerciseType = exercise.Exercise?.Category,
                InstructionVideo = exercise.Exercise?.VideoUrl,
                Sets = exercise.Sets,
                Reps = exercise.RepsPerSet,
                Duration = exercise.Duration,
                Weight = null, // Not available in WorkoutExercise
                Notes = exercise.Notes,
                Order = exercise.OrderInWorkout,
                RestBetweenSets = exercise.RestBetweenSets?.ToString()
            };
        }
    }
}
