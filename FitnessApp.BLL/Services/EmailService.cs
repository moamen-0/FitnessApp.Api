using FitnessApp.Core.Interfaces.IService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace FitnessApp.BLL.Services
{
	public class EmailService : IEmailService
	{
		private readonly SmtpClient _smtpClient;
		private readonly string _fromEmail;

		public EmailService(string smtpHost, int smtpPort, string fromEmail, string smtpUser, string smtpPass)
		{
			_smtpClient = new SmtpClient(smtpHost, smtpPort)
			{
				Credentials = new NetworkCredential(smtpUser, smtpPass),
				EnableSsl = true
			};
			_fromEmail = fromEmail;
		}

		public async Task SendEmailAsync(string to, string subject, string body)
		{
			var mailMessage = new MailMessage(_fromEmail, to, subject, body);
			await _smtpClient.SendMailAsync(mailMessage);
		}
	}
}
