using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace FitnessApp.DAL.Utilities
{
	public static class SecurityUtilities
	{
		public static string GenerateSecretKey(int byteLength = 32)
		{
			var key = new byte[byteLength];
			using (var generator = RandomNumberGenerator.Create())
			{
				generator.GetBytes(key);
			}
			return Convert.ToBase64String(key);
		}
	}
	}
