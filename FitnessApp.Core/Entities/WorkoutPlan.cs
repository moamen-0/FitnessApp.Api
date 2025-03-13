﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FitnessApp.Core.Entities
{
	public class WorkoutPlan
	{
		public int Id { get; set; }
		public string Name { get; set; }
		public string Description { get; set; }
		public string UserId { get; set; }  // If user-specific; null if template plan
		public bool IsTemplate { get; set; } // Whether this is a template plan created by admin
		public string DifficultyLevel { get; set; } // Beginner, Intermediate, Advanced
		public string FocusArea { get; set; } // Strength, Cardio, Flexibility, etc.
		public int DurationWeeks { get; set; }
		public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

		// Navigation properties
		public virtual ApplicationUser User { get; set; }
		public virtual ICollection<WorkoutDay> WorkoutDays { get; set; }
	}
}
