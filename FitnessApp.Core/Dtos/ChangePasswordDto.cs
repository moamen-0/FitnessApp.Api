﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FitnessApp.Core.Dtos
{
	public class ChangePasswordDto
	{
		public string CurrentPassword { get; set; }
		public string NewPassword { get; set; }
	}
}
