using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FitnessApp.Core.Interfaces.IService
{
	public interface IEmailService
	{
		Task SendEmailAsync(string to, string subject, string body);
		Task SendPasswordResetEmailAsync(string toEmail, string resetLink);
	}
}
